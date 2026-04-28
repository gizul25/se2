using System;
using System.IO;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SE2.Domain;
using System.Linq;
using System.Collections.Generic;

namespace SE2.ViewModels;

public partial class OverviewViewModel : ViewModelBase
{
    [ObservableProperty]
    private ISeries[] heatSeries = [];

    [ObservableProperty]
    private ISeries[] electricitySeries = [];

    [ObservableProperty]
    private ISeries[] priceSeries = [];

    [ObservableProperty]
    private ISeries[] expenseSeries = [];


    [ObservableProperty]
    private Axis[] xAxes = [];

    [ObservableProperty]
    private Axis[] yAxes = [];

    [ObservableProperty]
    private Bitmap? heatGrid = null;

	  public OverviewViewModel()
    {
        Load();
    }

    [RelayCommand]
    public void Load()
    {
        var grid = DM.AM.HeatingGrid;
        heatGrid = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", grid.Image));

        var sources = DM.SDM.Sources;
        var results = DM.RDM.ResultingData;


        if (sources == null || sources.Count == 0 || results == null || results.ResultRows.Count == 0)
        {
            HeatSeries = [];
            ElectricitySeries = [];
            PriceSeries = [];
            ExpenseSeries = [];
            XAxes = [];
            YAxes = [];
            return;
        }

        var elecCons = results.ResultRows.Select(r => new DateTimePoint(r.Time, r.Consumption)).ToArray();
        var elecProd = results.ResultRows.Select(r => new DateTimePoint(r.Time, 0d)).ToArray();

        var gas = sources.Select(s => new DateTimePoint(s.StartTime, (double)s.ElectricityPrice)).ToArray();
        var elec = sources.Select(s => new DateTimePoint(s.StartTime, (double)s.ElectricityPrice)).ToArray();

        var expenses = results.ResultRows.Select(r => new DateTimePoint(r.Time, (double)r.Costs)).ToArray();
        var profits = results.ResultRows.Select(r => new DateTimePoint(r.Time, (double)r.HeatProduction * 1000 - (double)r.Costs)).ToArray();

        List<ISeries> heatSeries = [];
        IDictionary<string, DateTimePoint[]> heatEntries = results.SchedulerRows
            .GroupBy(r => r.AssetName)
            .ToDictionary(r => r.Key, r => r.Select(c => new DateTimePoint(c.Time, c.HeatProduction)).ToArray());

        foreach (KeyValuePair<string, DateTimePoint[]> kvp in heatEntries)
        {
            var colorArr = DM.AM.GetAssetByName(kvp.Key)!.Color;
            var color = new SKColor((byte)colorArr[0], (byte)colorArr[1], (byte)colorArr[2]);
            var series = StackedColumnSeries(kvp.Key, kvp.Value, color);
            heatSeries.Add(series);
        }

        HeatSeries = heatSeries.ToArray();

        ElectricitySeries =
        [
            Series("Electricity consumption", elecCons, new SKColor(70,70,70)),
            Series("Electricity production", elecProd, new SKColor(150,150,150))
        ];

        PriceSeries =
        [
            Series("Gas price", gas, new SKColor(70,70,70)),
            Series("Electricity price", elec, new SKColor(150,150,150))
        ];

        ExpenseSeries =
        [
            Series("Expenses", expenses, new SKColor(70,70,70)),
            Series("Profits", profits, new SKColor(150,150,150))
        ];

        XAxes =
        [
            new DateTimeAxis(TimeSpan.FromHours(1), date => date.ToString("MM-dd"))
        ];
    }

    private static ISeries Series(string name, IReadOnlyCollection<DateTimePoint>? values, SKColor color)
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

    private static ISeries StackedColumnSeries(string name, IReadOnlyCollection<DateTimePoint>? values, SKColor color)
    {
        return new StackedColumnSeries<DateTimePoint>
        {
            Name = name,
            Values = values,
            Stroke = new SolidColorPaint(color) { StrokeThickness = 2 },
            Fill = new SolidColorPaint(new SKColor(color.Red, color.Green, color.Blue, 60))
        };
    }
}
