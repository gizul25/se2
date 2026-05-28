using System;
using System.IO;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.Generic;
using SkiaSharp;

namespace SE2.Utils;

public class GraphUtils
{
    public static SKColor BrightRed = new SKColor(237, 7, 27);
    public static SKColor CherryRed = new SKColor(142, 0, 12);
    public static SKColor BrightGreen = new SKColor(34, 177, 76);
	public static SKColor Black = new SKColor(0, 0, 0);

    public static ISeries Series(string name, IReadOnlyCollection<DateTimePoint>? values, SKColor color)
    {
        return new LineSeries<DateTimePoint>
        {
            Name = name,
            Values = values,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(color) { StrokeThickness = 2 },
            Fill = new SolidColorPaint(new SKColor(color.Red, color.Green, color.Blue, 60)),
            GeometryFill = new SolidColorPaint(color),
            GeometryStroke = new SolidColorPaint(color) { StrokeThickness = 4 },
            LineSmoothness = 0.5
        };
    }

    public static ISeries StackedColumnSeries(string name, IReadOnlyCollection<DateTimePoint>? values, SKColor color)
    {
        return new StackedColumnSeries<DateTimePoint>
        {
            Name = name,
            Values = values,
            Stroke = new SolidColorPaint(color) { StrokeThickness = 2 },
            Fill = new SolidColorPaint(new SKColor(color.Red, color.Green, color.Blue, 60))
        };
    }

    public static Axis[] GetXAxis()
    {
        return [
            new DateTimeAxis(TimeSpan.FromHours(1), date => date.ToString("MM-dd"))
        ];
    }

    public static Axis[] GetYAxis(string label)
    {
        return [
            new Axis
            {
                Name = label,
                NameTextSize = 16,
            }
        ];
    }
}