using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
    private List<string> _productionUnits = ["Unit 1", "Unit 2", "Unit 3", "All production units"];

    [ObservableProperty]
    private ObservableCollection<ChartControlViewModel> _charts = [];

    [RelayCommand]
    private void RunOptimization()
    {
        RunEnabled = false;
    }

    // Selection name is anything contained in _productionUnits, that is unit name or "All production units"
    public void Load(string selectionName)
    {
        Charts.Clear();

        var r = new Random();

        Axis[] xAxes =
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

        var heatProductionData = Make(r, 120, 400, 900);
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

        var netProductionData = Make(r, 120, 100, 200);
        ISeries[] netProductionSeries =
        [
            Series("Net Production Cost", netProductionData, new SKColor(70,70,70)),
        ];
        ChartControlViewModel chartControlViewModel2 = new()
        {
            Title = "Net Production Cost",
            Series = netProductionSeries,
            XAxes = xAxes,
        };
        Charts.Add(chartControlViewModel2);

        var additionalData = Make(r, 120, 100, 200);
        ISeries[] additionalSeries =
        [
            Series("Additional Data", additionalData, new SKColor(70,70,70)),
        ];
        ChartControlViewModel chartControlViewModel3 = new()
        {
            Title = "Additional Data",
            Series = additionalSeries,
            XAxes = xAxes,
        };
        Charts.Add(chartControlViewModel3);
    }

    private static ISeries Series(string name, List<double> values, SKColor color)
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