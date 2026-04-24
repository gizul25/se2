using System;
using System.Threading;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SE2.Domain;

namespace SE2.Views;

public partial class SideTabStrip : UserControl
{
    Panel? injectorNode;
    List<ScenarioNav> scenarios = [];

    public SideTabStrip()
    {
        InitializeComponent();

        TabNode.SelectionChanged += TabNode_SelectionChanged;
    }

    public void SetInjectorNode(Panel? injectorNode)
    {
        this.injectorNode = injectorNode;
    }

    public void AddScenario(string name, ScenarioNav instance)
    {
        scenarios.Add(instance);

        var tabItem = new TabStripItem
        {
            Content = name,
        };
        tabItem.Classes.Add("SidePanel");
        TabNode.Items.Add(tabItem);
    }

    public void TabNode_SelectionChanged(object? sender, RoutedEventArgs e)
    {
        SelectIndex(TabNode.SelectedIndex);
    }

    public void SelectIndex(int index)
    {
        if (index < 0 || index >= scenarios.Count)
        {
            return;
        }

        DM.SetScenario(index);

        injectorNode?.Children.Clear();
        injectorNode?.Children.Add(scenarios[index]);
    }
}