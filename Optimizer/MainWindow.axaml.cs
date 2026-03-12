using Avalonia.Controls;
using SE2.Views;

namespace SE2;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // CreateScenarioNav(1);
        // CreateScenarioNav(2);

        SideTabStripNode.SetInjectorNode(InjectorNode);
        SideTabStripNode.AddScenario("Scenario 1", new ScenarioNav("Scenario 1"));
        SideTabStripNode.AddScenario("Scenario 2", new ScenarioNav("Scenario 2"));
    }

    public void CreateScenarioNav(int id)
    {
        var scenarioNav = new ScenarioNav();
        var tabItem = new TabItem
        {
            Header = $"Scenario {id}",
            Content = scenarioNav,
        };
        // ScenarioTabs.Items.Add(tabItem);
    }
}