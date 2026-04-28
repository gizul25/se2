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

    // bug if selected not all production units and then run optimization
    [ObservableProperty]
    private List<string> _productionUnits = ["Unit 1", "Unit 2", "Unit 3", "All production units"];

    [ObservableProperty]
    private string _selectedProductionUnit = "All production units";

    [ObservableProperty]
    private bool _loadInProgress = false;

    [ObservableProperty]
    private ObservableCollection<ChartControlViewModel> _charts = [];

    [RelayCommand]
    private void RunOptimization()
    {
        RunEnabled = false;
        DM.StartOptimizer();
        ExportEnabled = true;
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
        ExportEnabled = false;
        DM.RDM.ResultingData = new();
        Load();
    }

    public void Load()
    {
        if (LoadInProgress)
        {
            return;
        }

        Charts.Clear();
        if (DM.RDM.ResultingData == null)
        {
            TotalProfit = 0;
            TotalCost = 0;
            HeatProduced = 0;
            ElectricityConsumed = 0;
            ElectricityProduced = 0;
            PrimaryEnergy = 0;
            Co2Emissions = 0;
            return;
        }

        var results = DM.RDM.ResultingData;

        LoadInProgress = true;

        TotalProfit = results.TotalProfit;
        TotalCost = results.TotalCost;
        HeatProduced = results.HeatProduced;
        ElectricityConsumed = results.ElectricityConsumed;
        ElectricityProduced = results.ElectricityProduced;
        PrimaryEnergy = results.PrimaryEnergy;
        Co2Emissions = results.Co2Emissions;

        Axis[] xAxes =
        [
            new DateTimeAxis(TimeSpan.FromHours(1), date => date.ToString("MM-dd"))
        ];

        List<string> prodUnits = [];
        foreach (string productionUnit in DM.AM.ScenarioData.AvailableUnits)
        {
            prodUnits.Add(productionUnit);
        }
        prodUnits.Add("All production units");
        ProductionUnits = prodUnits;

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
        ISeries[] heatProductionSeries =
        [
            Series("Optimized Heat Production Schedule", heatProductionData, new SKColor(70,70,70)),
        ];
        ChartControlViewModel heatProductionChart = new()
        {
            Title = "Optimized Heat Production Schedule",
            Series = heatProductionSeries,
            XAxes = xAxes,
        };
        Charts.Add(heatProductionChart);

        List<DateTimePoint> costsData = resultRows.Select(r => new DateTimePoint(r.Time, (double)r.Costs)).ToList();
        ISeries[] costsSeries =
        [
            Series("Costs Data", costsData, new SKColor(70,70,70)),
        ];
        ChartControlViewModel costsChart = new()
        {
            Title = "Costs Data",
            Series = costsSeries,
            XAxes = xAxes,
        };
        Charts.Add(costsChart);

        List<DateTimePoint> consumptionData = resultRows.Select(r => new DateTimePoint(r.Time, r.Consumption)).ToList();
        ISeries[] consumptionSeries =
        [
            Series("Consumption Data", consumptionData, new SKColor(70,70,70)),
        ];
        ChartControlViewModel consumptionChart = new()
        {
            Title = "Consumption Data",
            Series = consumptionSeries,
            XAxes = xAxes,
        };
        Charts.Add(consumptionChart);

        List<DateTimePoint> emissionsData = resultRows.Select(r => new DateTimePoint(r.Time, r.Emissions)).ToList();
        ISeries[] emissionsSeries =
        [
            Series("Emissions Data", emissionsData, new SKColor(70,70,70)),
        ];
        ChartControlViewModel emissionsChart = new()
        {
            Title = "Emissions Data",
            Series = emissionsSeries,
            XAxes = xAxes,
        };
        Charts.Add(emissionsChart);

        LoadInProgress = false;
    }

    private static ISeries Series(string name, List<DateTimePoint> values, SKColor color)
    {
        return new LineSeries<DateTimePoint>
        {
            Name = name,
            Values = values,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(color) { StrokeThickness = 2 },
            Fill = new SolidColorPaint(new SKColor(color.Red, color.Green, color.Blue, 60)),
            LineSmoothness = 0.5
        };
    }

    private List<double> Make(Random r, int count, int min, int max)
    {
        List<double> list = [];
        for (int i = 0; i < count; i++)
        {
            list.Add(r.Next(min, max));
        }
        return list;
    }
}