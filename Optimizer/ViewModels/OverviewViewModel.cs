using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

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
        var r = new Random();

        double[] Make(int min, int max)
        {
            var arr = new double[120];
            for (int i = 0; i < arr.Length; i++) arr[i] = r.Next(min, max);
            return arr;
        }

        var heatDemand = Make(400, 900);
        var heatProduction = Make(350, 950);

        var elecCons = Make(250, 750);
        var elecProd = Make(250, 800);

        var gas = Make(200, 1000);
        var elec = Make(250, 900);

        var expenses = Make(300, 900);
        var profits = Make(450, 1000);

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

        YAxes =
        [
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 1200,
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