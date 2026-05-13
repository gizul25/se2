using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace SE2.ViewModels;

public partial class ChartControlViewModel : ViewModelBase
{
    [ObservableProperty]
    public string _title = "";

    public ISeries[] Series { get; set; } = [];
    public Axis[] XAxes { get; set; } = [
        new DateTimeAxis(TimeSpan.FromHours(1), date => date.ToString("MM-dd"))
    ];
}