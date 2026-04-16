using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SE2.Domain;
using System.Linq;

namespace SE2.ViewModels;

public class OverviewViewModel
{
    public ISeries[] HeatSeries { get; set; }
    public ISeries[] ElectricitySeries { get; set; }
    public ISeries[] PriceSeries { get; set; }
    public ISeries[] ExpenseSeries { get; set; }

    public Axis[] XAxes { get; set; }
    public Axis[] YAxes { get; set; }

    public OverviewViewModel()
    {
        /*var r = new Random();

        double[] Make(int min, int max)
        {
            var arr = new double[120];
            for (int i = 0; i < arr.Length; i++) arr[i] = r.Next(min, max);
            return arr;
        }*/

		var sources = DM.SDM.Sources;

        var heatDemand = sources.Select(s => (double)s.HeatDemand).ToArray();
        var heatProduction = sources.Select(s => (double)s.HeatDemand).ToArray();

        var elecCons = sources.Select(s => (double)s.HeatDemand).ToArray();
        var elecProd = sources.Select(s => (double)s.HeatDemand).ToArray();

        var gas = sources.Select(s => (double)s.ElectricityPrice).ToArray();
        var elec = sources.Select(s => (double)s.ElectricityPrice).ToArray();

        var expenses = sources.Select(s => (double)s.ElectricityPrice).ToArray();
        var profits = sources.Select(s => (double)s.ElectricityPrice).ToArray();

        HeatSeries =
        [
            Series("Heat demand", heatDemand, new SKColor(70,70,70)),
            Series("Heat production", heatProduction, new SKColor(150,150,150))
        ];

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

		var yMax = Math.Max(1, Math.Ceiling(allValues.DefaultIfEmpty(0).Max() * 1.1));

        YAxes =
        [
            new Axis
            {
                MinLimit = 0,
                MaxLimit = yMax,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                SeparatorsPaint = new SolidColorPaint(new SKColor(230,230,230)),
                TextSize = 12
            }
        ];

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