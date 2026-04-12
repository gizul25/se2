using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DialogHostAvalonia;
using SE2;
using SE2.ViewModels;
using SE2.Models;

namespace SE2.Views;

public partial class ProductionUnitsView : UserControl
{
    public ProductionUnitsView()
    {
        InitializeComponent();
        DataContext = new ProductionUnitsViewModel();
    }

    private async void OpenEditUnitMenu(object? sender, RoutedEventArgs e)
    {
        Heading.Text = "Edit production unit";
        DeleteButton.Content = "Delete production unit";
        await DialogHost.Show(Resources["EditProductionUnitView"]!, "MainDialogHost");
    }
    private async void OpenAddUnitMenu(object? sender, RoutedEventArgs e)
    {
        Heading.Text = "Add production unit";
        DeleteButton.Content = "Cancel";
        await DialogHost.Show(Resources["EditProductionUnitView"]!, "MainDialogHost");
    }
}