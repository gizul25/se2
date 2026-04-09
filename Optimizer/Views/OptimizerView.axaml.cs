using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SE2.ViewModels;
using System.Collections.Specialized;

namespace SE2.Views;

public partial class OptimizerView : UserControl
{
    private OptimizerViewModel viewModel;

    public OptimizerView()
    {
        InitializeComponent();
        viewModel = new OptimizerViewModel();
        DataContext = viewModel;
        viewModel.Charts.CollectionChanged += OnChartsChanged;
        viewModel.SelectedProductionUnit = (string)ProductionUnitsComboBox?.SelectedItem!;
        viewModel.Load();
    }

    private void OnChartsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ChartGrid.Children.Clear();

        var charts = viewModel.Charts;
        for (int i = 0; i < charts.Count; i++)
        {
            int col = i % 3;
            int row = i / 3;

            ChartControlViewModel chart = charts[i];
            ChartControl chartControl = new()
            {
                DataContext = chart
            };
            Grid.SetColumn(chartControl, col);
            Grid.SetRow(chartControl, row);
            ChartGrid.Children.Add(chartControl);
        }
    }

    private void SelectionChanged(object? sender, RoutedEventArgs e)
    {
        var selectedItem = ProductionUnitsComboBox?.SelectedItem;
        if (selectedItem == null)
        {
            return;
        }

        viewModel.SelectedProductionUnit = (string)selectedItem;
        viewModel.Load();
    }
}