using Avalonia.Controls;
using HeatItOn.ViewModels;

namespace HeatItOn.Views;

public partial class OverviewPage : UserControl
{
    public OverviewPage()
    {
        InitializeComponent();
        DataContext = new OverviewViewModel();
    }

}