using System;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SE2.Domain;
using SE2.Data;
using SE2.Utils;

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

    public OptimizerViewModel()
    {
        Load();
    }

    [RelayCommand]
    private void RunOptimization()
    {
        RunEnabled = false;
        DM.StartOptimizer();
        RunEnabled = true;
        Load();
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

        TotalProfit = results.TotalProfit;
        TotalCost = results.TotalCost;
        HeatProduced = results.HeatProduced;
        ElectricityConsumed = results.ElectricityConsumed;
        ElectricityProduced = results.ElectricityProduced;
        PrimaryEnergy = results.PrimaryEnergy;
        Co2Emissions = results.Co2Emissions;
        MaintenanceText = $"Maintenance period: Production unit {results.MaintainedUnit} maintained from {results.MaintainedStart} to {results.MaintainedEnd}";

        // Update charts
        List<ResultRow> resultRows = [];
        if (SelectedProductionUnit == "All production units")
        {
            resultRows = results.ResultRows;
        }
        else
        {
            resultRows = results.SchedulerRows.Where(r => r.AssetName == SelectedProductionUnit).Select(r => new ResultRow
            {
                Time = r.Time,
                HeatProduction = r.HeatProduction,
                Costs = r.Costs,
                Consumption = r.Consumption,
                Emissions = r.Emissions,
            }).ToList();
        }

        List<DateTimePoint> heatProductionData = resultRows.Select(r => new DateTimePoint(r.Time, r.HeatProduction)).ToList();
        ChartControlViewModel heatProductionChart = new()
        {
            Title = "Optimized Heat Production Schedule",
            Series = [
                GraphUtils.Series("Optimized Heat Production Schedule", heatProductionData, GraphUtils.BrightRed),
            ],
        };
        Charts.Add(heatProductionChart);

        List<DateTimePoint> costsData = resultRows.Select(r => new DateTimePoint(r.Time, (double)r.Costs)).ToList();
        ChartControlViewModel costsChart = new()
        {
            Title = "Costs Data",
            Series = [
                GraphUtils.Series("Costs Data", costsData, GraphUtils.BrightRed),
            ],
        };
        Charts.Add(costsChart);

        List<DateTimePoint> consumptionData = resultRows.Select(r => new DateTimePoint(r.Time, r.Consumption)).ToList();
        ChartControlViewModel consumptionChart = new()
        {
            Title = "Consumption Data",
            Series = [
                GraphUtils.Series("Consumption Data", consumptionData, GraphUtils.BrightRed),
            ],
        };
        Charts.Add(consumptionChart);

        List<DateTimePoint> emissionsData = resultRows.Select(r => new DateTimePoint(r.Time, r.Emissions)).ToList();
        ChartControlViewModel emissionsChart = new()
        {
            Title = "Emissions Data",
            Series = [
                GraphUtils.Series("Emissions Data", emissionsData, GraphUtils.BrightRed),
            ],
        };
        Charts.Add(emissionsChart);

        LoadInProgress = false;
    }
}