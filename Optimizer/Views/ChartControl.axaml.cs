using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SE2.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace SE2.Views;

public partial class ChartControl : UserControl
{
    public ChartControl()
    {
        InitializeComponent();
    }
}