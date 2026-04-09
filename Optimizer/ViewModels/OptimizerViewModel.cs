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

    public void Load()
    {
        if (LoadInProgress)
        {
            return;
        }

        Charts.Clear();
        if (DM.RDM.ResultingData == null)
        {
            return;
        }

        LoadInProgress = true;

        var r = new Random();

        Axis[] xAxes =
        [
            new DateTimeAxis(TimeSpan.FromHours(1), date => date.ToString("MM-dd"))
        ];

        List<string> prodUnits = [];
        foreach (string productionUnit in DM.SelectedAssetNames)
        {
            prodUnits.Add(productionUnit);
        }
        prodUnits.Add("All production units");
        ProductionUnits = prodUnits;

        List<ResultRow> resultRows = [];
        if (SelectedProductionUnit == "All production units")
        {
            resultRows = DM.RDM.ResultingData.ResultRows;
        }
        else
        {
            resultRows = DM.RDM.ResultingData.SchedulerRows.Where(r => r.AssetName == SelectedProductionUnit).Select(r => new ResultRow {
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
        ChartControlViewModel chartControlViewModel = new()
        {
            Title = "Optimized Heat Production Schedule",
            Series = heatProductionSeries,
            XAxes = xAxes,
        };
        Charts.Add(chartControlViewModel);

        // var netProductionData = Make(r, 120, 100, 200);
        // ISeries[] netProductionSeries =
        // [
        //     Series("Net Production Cost", netProductionData, new SKColor(70,70,70)),
        // ];
        // ChartControlViewModel chartControlViewModel2 = new()
        // {
        //     Title = "Net Production Cost",
        //     Series = netProductionSeries,
        //     XAxes = xAxes,
        // };
        // Charts.Add(chartControlViewModel2);

        // var additionalData = Make(r, 120, 100, 200);
        // ISeries[] additionalSeries =
        // [
        //     Series("Additional Data", additionalData, new SKColor(70,70,70)),
        // ];
        // ChartControlViewModel chartControlViewModel3 = new()
        // {
        //     Title = "Additional Data",
        //     Series = additionalSeries,
        //     XAxes = xAxes,
        // };
        // Charts.Add(chartControlViewModel3);
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