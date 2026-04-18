using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SE2.Domain;
using System.Linq;

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

	public OverviewViewModel()
    {
        Load();
    }

	[RelayCommand]
    public void Load()
    {
        if (DM.SDM.Sources == null || DM.SDM.Sources.Count == 0 || DM.RDM.ResultingData == null || DM.RDM.ResultingData.ResultRows.Count == 0)
        {
            return;
        }

		var sources = DM.SDM.Sources;
        var results = DM.RDM.ResultingData;

        var heatDemand = sources.Select(s => (double)s.HeatDemand).ToArray();
        var heatProduction = results.ResultRows.Select(r => (double)r.HeatProduction).ToArray();

        var elecCons =  results.ResultRows.Select(r => r.Consumption).ToArray();
        var elecProd = results.ResultRows.Select(r => 0d).ToArray();

        var gas = sources.Select(s => (double)s.ElectricityPrice).ToArray();
        var elec = sources.Select(s => (double)s.ElectricityPrice).ToArray();

        var expenses = results.ResultRows.Select(r => (double)r.Costs).ToArray();
        var profits = results.ResultRows.Select(r => (double)r.HeatProduction * 1000 - (double)r.Costs).ToArray();

        heatSeries =
        [
            Series("Heat demand", heatDemand, new SKColor(70,70,70)),
            Series("Heat production", heatProduction, new SKColor(150,150,150))
        ];

        electricitySeries =
        [
            Series("Electricity consumption", elecCons, new SKColor(70,70,70)),
            Series("Electricity production", elecProd, new SKColor(150,150,150))
        ];

        priceSeries =
        [
            Series("Gas price", gas, new SKColor(70,70,70)),
            Series("Electricity price", elec, new SKColor(150,150,150))
        ];

        expenseSeries =
        [
            Series("Expenses", expenses, new SKColor(70,70,70)),
            Series("Profits", profits, new SKColor(150,150,150))
        ];

        xAxes =
        [
            new Axis
            {
                Labeler = value =>
                {
                    var months = new[]
                    {
                        "Jan","Feb","Mar","Apr","May","Jun",
                        "Jul","Aug","Sep","Oct","Nov","Dec"
                    };

                    int index = (int)(value / 10); // 120 points / 12 months

                    if (value % 10 == 0 && index >= 0 && index < 12)
                        return months[index];

                    return "";
                },

                MinLimit = 0,
                MaxLimit = 119,

                MinStep = 10,
                ForceStepToMin = true,

                LabelsPaint = new SolidColorPaint(SKColors.Black),
                SeparatorsPaint = new SolidColorPaint(new SKColor(230,230,230)),
                TextSize = 12
            }
        ];

		var allValues = heatDemand
    		.Concat(heatProduction)
    		.Concat(elecCons)
    		.Concat(elecProd)
    		.Concat(gas)
    		.Concat(elec)
    		.Concat(expenses)
    		.Concat(profits);

		/*var yMax = Math.Max(1, Math.Ceiling(allValues.DefaultIfEmpty(0).Max() * 1.1));

        yAxes =
        [
            new Axis
            {
                MinLimit = 0,
                MaxLimit = yMax,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                SeparatorsPaint = new SolidColorPaint(new SKColor(230,230,230)),
                TextSize = 12
            }
        ]; */

    }

    private static ISeries Series(string name, double[] values, SKColor color)
    {
        return new LineSeries<double>
        {
            Name = name,
            Values = values,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(color) { StrokeThickness = 2 },
            Fill = new SolidColorPaint(new SKColor(color.Red, color.Green, color.Blue, 60)),
            LineSmoothness = 0.5
        };
    }
}