using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SE2.Models;
using SE2.ViewModels;

namespace SE2.Views;

public partial class EditProductionUnitView : UserControl
{
    public EditProductionUnitView()
    {
        InitializeComponent();
    }

    private void OnTextChanged(object? sender, RoutedEventArgs args)
    {
        if (DataContext is EditProductionUnitViewModel model)
        {
            model.TestSave();
        }
    }
}