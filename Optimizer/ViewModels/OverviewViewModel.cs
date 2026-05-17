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
using SE2.Utils;

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

    [ObservableProperty]
    private string gridMapName = "";

    public OverviewViewModel()
    {
        Load();
    }

    [RelayCommand]
    public void Load()
    {
        var grid = DM.AM.HeatingGrid;
        heatGrid = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", grid.Image));
        gridMapName = grid.City;

        var sources = DM.SDM.Sources;
        var results = DM.RDM.GetCurrentScenarioResultingData();
        if (results == null)
        {
            return;
        }

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

        List<ISeries> heatSeries = [];
        IDictionary<string, DateTimePoint[]> heatEntries = results.SchedulerRows
            .GroupBy(r => r.AssetName)
            .ToDictionary(r => r.Key, r => r.Select(c => new DateTimePoint(c.Time, c.HeatProduction)).ToArray());

        foreach (KeyValuePair<string, DateTimePoint[]> kvp in heatEntries)
        {
            var hexString = DM.AM.GetAssetByName(kvp.Key)!.Color;
            var color = SKColor.Parse(hexString);
            var series = GraphUtils.StackedColumnSeries(kvp.Key, kvp.Value, color);
            heatSeries.Add(series);
        }
        HeatSeries = heatSeries.ToArray();

        var elecCons = results.ResultRows.Select(r => new DateTimePoint(r.Time, r.Consumption)).ToArray();
        var elecProd = results.ResultRows.Select(r => new DateTimePoint(r.Time, 0d)).ToArray();
        ElectricitySeries =
        [
            GraphUtils.Series("Electricity consumption", elecCons, GraphUtils.BrightRed),
            GraphUtils.Series("Electricity production", elecProd, GraphUtils.CherryRed)
        ];

        var gas = sources.Select(s => new DateTimePoint(s.StartTime, (double)s.ElectricityPrice)).ToArray();
        var elec = sources.Select(s => new DateTimePoint(s.StartTime, (double)s.ElectricityPrice)).ToArray();
        PriceSeries =
        [
            GraphUtils.Series("Gas price", gas, GraphUtils.BrightRed),
            GraphUtils.Series("Electricity price", elec, GraphUtils.CherryRed)
        ];

        var expenses = results.ResultRows.Select(r => new DateTimePoint(r.Time, (double)r.Costs)).ToArray();
        var profits = results.ResultRows.Select(r => new DateTimePoint(r.Time, (double)r.HeatProduction * 1000 - (double)r.Costs)).ToArray();
        ExpenseSeries =
        [
            GraphUtils.Series("Expenses", expenses, GraphUtils.BrightRed),
            GraphUtils.Series("Profits", profits, GraphUtils.CherryRed)
        ];

        XAxes =
        [
            new DateTimeAxis(TimeSpan.FromHours(1), date => date.ToString("MM-dd"))
        ];
    }
}
