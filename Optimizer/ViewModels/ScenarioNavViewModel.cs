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
using Avalonia.Interactivity;
using SE2.Domain;
using SE2.Data;

namespace SE2.ViewModels;

public partial class ScenarioNavViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _selectedPeriod = "Winter period";

    [ObservableProperty]
    private List<string> _periods = ["Winter period", "Summer period"];
}