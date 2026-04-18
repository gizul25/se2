using Avalonia.Controls;
using SE2.ViewModels;

namespace SE2.Views;

public partial class OverviewView : UserControl
{
    public OverviewView()
    {
        InitializeComponent();
        var viewModel = new OverviewViewModel();
        DataContext = new OverviewViewModel();
        viewModel.Load();
    }
}