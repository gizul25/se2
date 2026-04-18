using Avalonia.Controls;
using SE2.ViewModels;

namespace SE2.Views;

public partial class OverviewView : UserControl
{
    private readonly OverviewViewModel viewModel;

    public OverviewView()
    {
        InitializeComponent();

        viewModel = new OverviewViewModel();
        DataContext = viewModel;

        // ScenarioNav re-attaches this control when switching tabs; refresh with latest optimizer output.
        AttachedToVisualTree += (_, _) => viewModel.Load();
    }
}