using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SE2.ViewModels;
using SE2.Domain;
using SE2.Data;

namespace SE2.Views;

public partial class ScenarioNav : UserControl
{
    internal event EventHandler? Update;

    private ScenarioNavViewModel viewModel;
    private OverviewView overviewView = new();
    private OptimizerView optimizerView = new();
    private ProductionUnitsView productionUnitsView = new();

    public ScenarioNav() : this("Scenario 111") { }

    public ScenarioNav(string title)
    {
        InitializeComponent();
        Update += optimizerView.OnUpdate;
        viewModel = new ScenarioNavViewModel();
        DataContext = viewModel;

        Title.Text = title;

        TopTabStripNode.SetInjectorNode(InjectorNode);
        TopTabStripNode.AddControl("Overview", overviewView);
        TopTabStripNode.AddControl("Optimizer", optimizerView);
        TopTabStripNode.AddControl("Production units", productionUnitsView);

        PeriodComboBox.SelectionChanged += PeriodComboBox_SelectionChanged;
    }

    private void PeriodComboBox_SelectionChanged(object? sender, RoutedEventArgs e)
    {
        string periodText = (string)(PeriodComboBox.SelectedItem?? "");
        IPeriod period = periodText switch
        {
            "Winter period" => new Winter(),
            "Summer period" => new Summer(),
            _ => new Winter()
        };
        DM.UpdatePeriod(period);
        Update?.Invoke(this, new());
    }
}