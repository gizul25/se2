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
    private ISeries[] primaryEnergySeries = [];

    [ObservableProperty]
    private ISeries[] emissionsSeries = [];

    [ObservableProperty]
    private Axis[] xAxes = [];

    [ObservableProperty]
    private Axis[] heatYAxes = [];

    [ObservableProperty]
    private Axis[] electricityYAxes = [];

    [ObservableProperty]
    private Axis[] priceYAxes = [];

    [ObservableProperty]
    private Axis[] expenseYAxes = [];

    [ObservableProperty]
    private Axis[] primaryEnergyYAxes = [];

    [ObservableProperty]
    private Axis[] emissionsYAxes = [];

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
        HeatGrid = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", grid.Image));
        GridMapName = grid.City;

        var sources = DM.SDM.Sources;
        var results = DM.RDM.GetCurrentScenarioResultingData();
        if (sources == null || sources.Count == 0 || results == null || results.ResultRows.Count == 0)
        {
            HeatSeries = [];
            ElectricitySeries = [];
            PriceSeries = [];
            ExpenseSeries = [];
            XAxes = [];
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

        var heatDemand = sources.Select(s => new DateTimePoint(s.StartTime, s.HeatDemand)).ToArray();
        heatSeries.Add(GraphUtils.Series("Heat demand", heatDemand, GraphUtils.Black));
        HeatSeries = heatSeries.ToArray();

        var elecCons = results.ResultRows.Select(r => new DateTimePoint(r.Time, r.Consumption)).ToArray();
        var elecProd = results.ResultRows.Select(r => new DateTimePoint(r.Time, r.Production)).ToArray();
        ElectricitySeries =
        [
            GraphUtils.Series("Electricity consumption", elecCons, GraphUtils.BrightRed),
            GraphUtils.Series("Electricity production", elecProd, GraphUtils.BrightGreen)
        ];

        var elec = sources.Select(s => new DateTimePoint(s.StartTime, (double)s.ElectricityPrice)).ToArray();
        PriceSeries =
        [
            GraphUtils.Series("Electricity prices", elec, GraphUtils.BrightRed)
        ];
        
        var netCost = results.ResultRows.Select(r => new DateTimePoint(r.Time, (double)r.Costs)).ToArray();
        var revenue = results.ResultRows.Select(r => new DateTimePoint(r.Time, (double)r.Profits + (double)r.Expenses)).ToArray();
        var expense = results.ResultRows.Select(r => new DateTimePoint(r.Time, (double)r.Expenses)).ToArray();
        var profits = results.ResultRows.Select(r => new DateTimePoint(r.Time, (double)r.Profits)).ToArray();
        ExpenseSeries =
        [
            GraphUtils.Series("Net cost", netCost, GraphUtils.BrightRed),
            GraphUtils.Series("Revenue", revenue, GraphUtils.BrightGreen),
            GraphUtils.Series("Expenses", expense, GraphUtils.CherryRed),
            GraphUtils.Series("Profits", profits, GraphUtils.BrightOrange),
        ];

        var primaryEnergy = results.ResultRows.Select(r => new DateTimePoint(r.Time, (double)r.PrimaryEnergy)).ToArray();
        PrimaryEnergySeries =
        [
            GraphUtils.Series("Primary energy", primaryEnergy, GraphUtils.BrightRed)
        ];

        var emissions = results.ResultRows.Select(r => new DateTimePoint(r.Time, (double)r.Emissions)).ToArray();
        EmissionsSeries =
        [
            GraphUtils.Series("CO2 Emissions", emissions, GraphUtils.BrightRed)
        ];

        XAxes = GraphUtils.GetXAxis();
        HeatYAxes = GraphUtils.GetYAxis("MWh");
        ElectricityYAxes = GraphUtils.GetYAxis("MWh");
        PriceYAxes = GraphUtils.GetYAxis("DKK/MWh");
        ExpenseYAxes = GraphUtils.GetYAxis("DKK");
        PrimaryEnergyYAxes = GraphUtils.GetYAxis("MWh");
        EmissionsYAxes = GraphUtils.GetYAxis("kg CO2");
    }
}
