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

namespace SE2.Views;

public partial class TopTabStrip : UserControl
{
    Panel? injectorNode;
    List<Control> nodes = [];

    public TopTabStrip()
    {
        InitializeComponent();

        TabNode.SelectionChanged += TabNode_SelectionChanged;
    }

    public void SetInjectorNode(Panel? injectorNode)
    {
        this.injectorNode = injectorNode;
    }

    public void AddControl(string name, Control node)
    {
        nodes.Add(node);

        var tabItem = new TabStripItem
        {
            Content = name,
        };
        TabNode.Items.Add(tabItem);
    }

    public void TabNode_SelectionChanged(object? sender, RoutedEventArgs e)
    {
        SelectIndex(TabNode.SelectedIndex);
    }

    public void SelectIndex(int index)
    {
        if (index < 0 || index >= nodes.Count)
        {
            return;
        }

        injectorNode?.Children.Clear();
        injectorNode?.Children.Add(nodes[index]);
    }
}