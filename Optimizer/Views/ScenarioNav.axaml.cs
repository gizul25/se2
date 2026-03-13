using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace SE2.Views;

public partial class ScenarioNav : UserControl
{
    private OverviewView overviewView = new();
    private OptimizerView optimizerView = new();
    private ProductionUnitsView productionUnitsView = new();
    private ConfigurationView configurationView = new();

    public ScenarioNav() : this("Scenario 111") { }

    public ScenarioNav(string title)
    {
        InitializeComponent();

        Title.Text = title;

        TopTabStripNode.SetInjectorNode(InjectorNode);
        TopTabStripNode.AddControl("Overview", overviewView);
        TopTabStripNode.AddControl("Optimizer", optimizerView);
        TopTabStripNode.AddControl("Production units", productionUnitsView);
        TopTabStripNode.AddControl("Configuration", configurationView);
    }
}