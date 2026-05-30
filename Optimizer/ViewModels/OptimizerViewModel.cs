using System;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using SE2.Views;
using SE2.Domain;
using SE2.Data;
using SE2.Utils;
using Avalonia.VisualTree;
using Avalonia.Controls;

namespace SE2.ViewModels;

public partial class OptimizerViewModel : ViewModelBase
{
    [ObservableProperty]
    private double _totalProfit = 0;

    [ObservableProperty]
    private double _totalCost = 0;

    [ObservableProperty]
    private double _heatProduced = 0;

    [ObservableProperty]
    private double _electricityConsumed = 0;

    [ObservableProperty]
    private double _electricityProduced = 0;

    [ObservableProperty]
    private double _primaryEnergy = 0;

    [ObservableProperty]
    private double _co2Emissions = 0;

    [ObservableProperty]
    private bool _runEnabled = true;

    [ObservableProperty]
    private bool _exportEnabled = false;

    [ObservableProperty]
    private List<string> _productionUnits = ["All production units"];

    [ObservableProperty]
    private string _selectedProductionUnit = "All production units";

    [ObservableProperty]
    private bool _loadInProgress = false;

    [ObservableProperty]
    private ObservableCollection<ChartControlViewModel> _charts = [];

    [ObservableProperty]
    private string maintenanceText = "Maintenance period: Production unit {0} maintained from {1} to {2}";

    private readonly OptimizerView? view;

    public OptimizerViewModel() : this(null) { }

    public OptimizerViewModel(OptimizerView? view)
    {
        this.view = view;
        Load();
    }

    [RelayCommand]
    private async Task RunOptimization()
    {
        if (!RunEnabled)
        {
            return;
        }

        RunEnabled = false;
        await OpenPopup();
        RunEnabled = true;
        Load();
    }

    public async Task OpenPopup()
    {
        if (view == null)
        {
            throw new Exception("View must be passed for dialog to work properly");
        }

        var window = (Window?)view.GetVisualRoot();
        DialogHost dialogHost = window?.FindControl<DialogHost>("DialogHost")!;

        OptimizerPopupViewModel optimizerPopupViewModel = new(dialogHost);
        var tasks = new[] { DialogHost.Show(new OptimizerPopupView() { DataContext = optimizerPopupViewModel }, dialogHost), optimizerPopupViewModel.Run() };
        await Task.WhenAll(tasks);
    }

    [RelayCommand]
    private void Export()
    {
        DM.Export();
    }

    public void OnSelectedPeriodChange()
    {
        RunEnabled = true;
        Load();
    }

    private void Reset()
    {
        Console.WriteLine("Reset");
        ExportEnabled = false;

        Charts.Clear();

        TotalProfit = 0;
        TotalCost = 0;
        HeatProduced = 0;
        ElectricityConsumed = 0;
        ElectricityProduced = 0;
        PrimaryEnergy = 0;
        Co2Emissions = 0;
        MaintenanceText = "";

        List<string> prodUnits = [];
        foreach (string productionUnit in DM.AM.ScenarioData.AvailableUnits)
        {
            prodUnits.Add(productionUnit);
        }
        prodUnits.Add("All production units");
        ProductionUnits = prodUnits;
    }

    public void Load()
    {
        if (LoadInProgress)
        {
            return;
        }
        LoadInProgress = true;

        // TODO: Call reset for each scenario separately and when production units change.
        Reset();

        var results = DM.RDM.GetCurrentScenarioResultingData();
        if (results == null)
        {
            LoadInProgress = false;
            return;
        }

        // Update top part
        ExportEnabled = true;

        MaintenanceText = "";

        for (int i = 0; i < results.MaintenancePeriods.Count; i++)
        {
            MaintenancePeriod maintenancePeriod = results.MaintenancePeriods[i];

            MaintenanceText += $"Maintenance period: Production unit {maintenancePeriod.MaintainedUnit} maintained from {maintenancePeriod.MaintainedStart} to {maintenancePeriod.MaintainedEnd}";
            if (i != results.MaintenancePeriods.Count - 1)
            {
                maintenanceText += "\n";
            }
        }

        // Update charts
        List<ResultRow> resultRows = [];
        if (SelectedProductionUnit == "All production units")
        {
            resultRows = results.ResultRows;
            TotalCost = results.TotalExpenses;
            TotalProfit = results.TotalRevenue - results.TotalExpenses;
        }
        else
        {
            var unitSchedulerRows = results.SchedulerRows.Where(r => r.AssetName == SelectedProductionUnit).ToList();
            resultRows = unitSchedulerRows.Select(r =>
            {
                var production = 0.0;
                var consumption = 0.0;
                if (r.Electricity > 0)
                {
                    consumption = r.Electricity;
                }
                else
                {
                    production = Math.Abs(r.Electricity);
                }

                return new ResultRow
                {
                    Time = r.Time,
                    HeatProduction = r.HeatProduction,
                    Costs = r.Costs,
                    Production = production,
                    Consumption = consumption,
                    Emissions = r.Emissions,
                    PrimaryEnergy = r.PrimaryEnergy,
                };
            }).ToList();

			var unitRevenue = unitSchedulerRows.Sum(r => (double)r.ElectricityRevenue);
            var unitExpenses = unitSchedulerRows.Sum(r => (double)(r.HeatExpense + r.ElectricityExpense));

            TotalCost = unitExpenses;
            TotalProfit = unitRevenue - unitExpenses;
        }
        
        HeatProduced = resultRows.Sum(r => (double)r.HeatProduction);
        ElectricityConsumed = resultRows.Sum(r => (double)r.Consumption);
        ElectricityProduced = resultRows.Sum(r => (double)r.Production);
        PrimaryEnergy = resultRows.Sum(r => (double)r.PrimaryEnergy);
        Co2Emissions = resultRows.Sum(r => (double)r.Emissions);

        // Net cost chart
        List<NetCostData> netCostRows = [];
        if (SelectedProductionUnit == "All production units")
        {
            netCostRows = results.HourlyNetCost;
        }
        else
        {
            netCostRows = results.HourlyNetCost.Where(r => r.AssetName == SelectedProductionUnit).ToList();
        }

        List<ISeries> netCostSeries = [];
        IDictionary<string, DateTimePoint[]> netCostEntries = netCostRows
            .GroupBy(r => r.AssetName)
            .ToDictionary(r => r.Key, r => r.Select(c => new DateTimePoint(c.Time, (double)c.NetCost)).ToArray());

        foreach (KeyValuePair<string, DateTimePoint[]> kvp in netCostEntries)
        {
            var hexString = DM.AM.GetAssetByName(kvp.Key)!.Color;
            var color = SKColor.Parse(hexString);
            var series = GraphUtils.Series(kvp.Key, kvp.Value, color);
            netCostSeries.Add(series);
        }

        ChartControlViewModel netCostChart = new()
        {
            Title = "Unit Cost for 1 MWh",
            Series = netCostSeries.ToArray(),
            YAxes = GraphUtils.GetYAxis("DKK/MWh")
        };
        Charts.Add(netCostChart);

        // Unit scheduling
        List<SchedulerRow> schedulerRows = [];
        if (SelectedProductionUnit == "All production units")
        {
            schedulerRows = results.SchedulerRows;
        }
        else
        {
            schedulerRows = results.SchedulerRows.Where(r => r.AssetName == SelectedProductionUnit).ToList();
        }

        List<ISeries> heatSeries = [];
        IDictionary<string, List<DateTimePoint?>> heatEntries = schedulerRows
            .GroupBy(r => r.AssetName)
            .ToDictionary(r => r.Key, r => r.Select(c => new DateTimePoint(c.Time, c.HeatProduction)).ToList());
        InsertDataBreaks(heatEntries, results.ResultRows);

        foreach (KeyValuePair<string, List<DateTimePoint?>> kvp in heatEntries)
        {
            var hexString = DM.AM.GetAssetByName(kvp.Key)!.Color;
            var color = SKColor.Parse(hexString);
            var series = GraphUtils.Series(kvp.Key, kvp.Value, color);
            heatSeries.Add(series);
        }

        ChartControlViewModel heatSeriesChart = new()
        {
            Title = "Heat Production Scheduling",
            Series = heatSeries.ToArray(),
            YAxes = GraphUtils.GetYAxis("MWh")
        };
        Charts.Add(heatSeriesChart);

        // Unit cost
        List<ISeries> unitCostSeries = [];
        IDictionary<string, List<DateTimePoint?>> unitCostEntries = schedulerRows
            .GroupBy(r => r.AssetName)
            .ToDictionary(r => r.Key, r => r.Select(c => new DateTimePoint(c.Time, (double)c.Costs)).ToList());
        InsertDataBreaks(unitCostEntries, results.ResultRows);

        foreach (KeyValuePair<string, List<DateTimePoint?>> kvp in unitCostEntries)
        {
            var hexString = DM.AM.GetAssetByName(kvp.Key)!.Color;
            var color = SKColor.Parse(hexString);
            var series = GraphUtils.Series(kvp.Key, kvp.Value, color);
            unitCostSeries.Add(series);
        }

        ChartControlViewModel unitCostChart = new()
        {
            Title = "Unit Cost",
            Series = unitCostSeries.ToArray(),
            YAxes = GraphUtils.GetYAxis("DKK")
        };
        Charts.Add(unitCostChart);

        // Electricity usage
        List<ISeries> electricityConsumptionSeries = [];
        IDictionary<string, List<DateTimePoint?>> electricityConsumptionEntries = schedulerRows
            .GroupBy(r => r.AssetName)
            .ToDictionary(r => r.Key, r => r.Select(c => new DateTimePoint(c.Time, (double)c.Electricity)).ToList());
        InsertDataBreaks(electricityConsumptionEntries, results.ResultRows);

        foreach (KeyValuePair<string, List<DateTimePoint?>> kvp in electricityConsumptionEntries)
        {
            var hexString = DM.AM.GetAssetByName(kvp.Key)!.Color;
            var color = SKColor.Parse(hexString);
            var series = GraphUtils.Series(kvp.Key, kvp.Value, color);
            electricityConsumptionSeries.Add(series);
        }

        ChartControlViewModel electricityConsumptionChart = new()
        {
            Title = "Electricity usage (positive = consume, negative = produce)",
            Series = electricityConsumptionSeries.ToArray(),
            YAxes = GraphUtils.GetYAxis("MWh")
        };
        Charts.Add(electricityConsumptionChart);

        // CO2 Emissions
        List<ISeries> emissionsSeries = [];
        IDictionary<string, List<DateTimePoint?>> emissionsEntries = schedulerRows
            .GroupBy(r => r.AssetName)
            .ToDictionary(r => r.Key, r => r.Select(c => new DateTimePoint(c.Time, (double)c.Emissions)).ToList());
        InsertDataBreaks(emissionsEntries, results.ResultRows);

        foreach (KeyValuePair<string, List<DateTimePoint?>> kvp in emissionsEntries)
        {
            var hexString = DM.AM.GetAssetByName(kvp.Key)!.Color;
            var color = SKColor.Parse(hexString);
            var series = GraphUtils.Series(kvp.Key, kvp.Value, color);
            emissionsSeries.Add(series);
        }

        ChartControlViewModel emissionsChart = new()
        {
            Title = "CO2 Emissions",
            Series = emissionsSeries.ToArray(),
            YAxes = GraphUtils.GetYAxis("kg CO2")
        };
        Charts.Add(emissionsChart);

        LoadInProgress = false;
    }

    private void InsertDataBreaks(IDictionary<string, List<DateTimePoint?>> entries, List<ResultRow> resultRows)
    {
        foreach (KeyValuePair<string, List<DateTimePoint?>> kvp in entries)
        {
            var arr = kvp.Value;
            var currIndex = 0;
            var nullInserted = false;
            foreach (ResultRow row in resultRows)
            {
                var time = row.Time;
                if (currIndex >= arr.Count)
                {
                    break;
                }

                var indexTime = arr[currIndex].DateTime;
                if (time < indexTime && !nullInserted)
                {
                    arr.Insert(currIndex, null);
                    currIndex += 1;
                    nullInserted = true;
                }
                if (time == indexTime)
                {
                    currIndex += 1;
                    nullInserted = false;
                }
            }
        }
    }
}