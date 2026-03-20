using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using SE2.Views;
using Xunit;

namespace SE2.Headless.XUnit;

public class UnitTest1
{
    [AvaloniaFact]
    public void Avalonia_TestWorking()
    {
        // Setup controls:
        var textBox = new TextBox();
        var window = new Window { Content = textBox };

        // Open window:
        window.Show();

        // Focus text box:
        textBox.Focus();

        // Simulate text input:
        window.KeyTextInput("Hello World");

        // Assert:
        Assert.Equal("Hello World", textBox.Text);
    }

    [AvaloniaFact]
    public void MainNav_CorrectScenarioCount()
    {
        var window = new MainWindow();

        window.Show();
        SideTabStrip strip = (SideTabStrip)window.SideTabStripNode;
        Assert.Equal(2, strip.TabNode.Items.Count);
    }

    [AvaloniaFact]
    public void Optimizer_ButtonDisabled()
    {
        var window = new MainWindow();

        window.Show();

        StackPanel injectorNode = window.InjectorNode;
        ScenarioNav scenarioNav = (ScenarioNav)injectorNode.Children[0];
        TopTabStrip strip = (TopTabStrip)scenarioNav.TopTabStripNode;
        strip.SelectIndex(1);
        Panel panel = (Panel)scenarioNav.InjectorNode;
        OptimizerView optimizerView = (OptimizerView)panel.Children[0];
        Button optimizerBtn = optimizerView.OptimizerBtn;

        Avalonia.Pointer btnPos = new Avalonia.Pointer(50, 50);
        Assert.True(optimizerBtn.IsEnabled);
        window.MouseDown(btnPos, MouseButton.Left);
        window.MouseUp(btnPos, MouseButton.Left);
        Assert.False(optimizerBtn.IsEnabled);
    }

    [AvaloniaFact]
    public void Optimizer_ButtonSendCommand()
    {
        var window = new MainWindow();

        window.Show();

        StackPanel injectorNode = window.InjectorNode;
        ScenarioNav scenarioNav = (ScenarioNav)injectorNode.Children[0];
        TopTabStrip strip = (TopTabStrip)scenarioNav.TopTabStripNode;
        strip.SelectIndex(1);
        Panel panel = (Panel)scenarioNav.InjectorNode;
        OptimizerView optimizerView = (OptimizerView)panel.Children[0];
        Button optimizerBtn = optimizerView.OptimizerBtn;

        Avalonia.Pointer btnPos = new Avalonia.Pointer(50, 50);
        Assert.Equal(0, optimizerBtn.ExecutedCommands);
        window.MouseDown(btnPos, MouseButton.Left);
        window.MouseUp(btnPos, MouseButton.Left);
        Assert.Equal(1, optimizerBtn.ExecutedCommands);
    }

    [AvaloniaFact]
    public void Optimizer_ButtonMultipleClicks()
    {
        var window = new MainWindow();

        window.Show();

        StackPanel injectorNode = window.InjectorNode;
        ScenarioNav scenarioNav = (ScenarioNav)injectorNode.Children[0];
        TopTabStrip strip = (TopTabStrip)scenarioNav.TopTabStripNode;
        strip.SelectIndex(1);
        Panel panel = (Panel)scenarioNav.InjectorNode;
        OptimizerView optimizerView = (OptimizerView)panel.Children[0];
        Button optimizerBtn = optimizerView.OptimizerBtn;

        Avalonia.Pointer btnPos = new Avalonia.Pointer(50, 50);
        Assert.Equal(0, optimizerBtn.ExecutedCommands);
        window.MouseDown(btnPos, MouseButton.Left);
        window.MouseUp(btnPos, MouseButton.Left);
        window.MouseDown(btnPos, MouseButton.Left);
        window.MouseUp(btnPos, MouseButton.Left);
        window.MouseDown(btnPos, MouseButton.Left);
        window.MouseUp(btnPos, MouseButton.Left);
        window.MouseDown(btnPos, MouseButton.Left);
        window.MouseUp(btnPos, MouseButton.Left);
        Assert.Equal(1, optimizerBtn.ExecutedCommands);
    }

    [AvaloniaFact]
    public void Optimizer_DropDownOpen()
    {
        var window = new MainWindow();

        window.Show();

        StackPanel injectorNode = window.InjectorNode;
        ScenarioNav scenarioNav = (ScenarioNav)injectorNode.Children[0];
        TopTabStrip strip = (TopTabStrip)scenarioNav.TopTabStripNode;
        strip.SelectIndex(1);
        Panel panel = (Panel)scenarioNav.InjectorNode;
        OptimizerView optimizerView = (OptimizerView)panel.Children[0];
        ComboBox comboBox = optimizerView.ProductionUnitsComboBox;

        Avalonia.Pointer comboBoxPos = new Avalonia.Pointer(50, 50);
        Assert.False(comboBox.IsDropDownOpen);
        window.MouseDown(comboBoxPos, MouseButton.Left);
        window.MouseUp(comboBoxPos, MouseButton.Left);
        Assert.True(comboBox.IsDropDownOpen);
    }

    [AvaloniaFact]
    public void Optimizer_DropDownEmptyOpen()
    {
        var window = new MainWindow();

        window.Show();

        StackPanel injectorNode = window.InjectorNode;
        ScenarioNav scenarioNav = (ScenarioNav)injectorNode.Children[0];
        TopTabStrip strip = (TopTabStrip)scenarioNav.TopTabStripNode;
        strip.SelectIndex(1);
        Panel panel = (Panel)scenarioNav.InjectorNode;
        OptimizerView optimizerView = (OptimizerView)panel.Children[0];
        ComboBox comboBox = optimizerView.ProductionUnitsComboBox;
        comboBox.Items.Clear();

        Avalonia.Pointer comboBoxPos = new Avalonia.Pointer(50, 50);
        Assert.False(comboBox.IsDropDownOpen);
        window.MouseDown(comboBoxPos, MouseButton.Left);
        window.MouseUp(comboBoxPos, MouseButton.Left);
        Assert.False(comboBox.IsDropDownOpen);
    }

    [AvaloniaFact]
    public void Optimizer_DropDownQuickPress()
    {
        var window = new MainWindow();

        window.Show();

        StackPanel injectorNode = window.InjectorNode;
        ScenarioNav scenarioNav = (ScenarioNav)injectorNode.Children[0];
        TopTabStrip strip = (TopTabStrip)scenarioNav.TopTabStripNode;
        strip.SelectIndex(1);
        Panel panel = (Panel)scenarioNav.InjectorNode;
        OptimizerView optimizerView = (OptimizerView)panel.Children[0];
        ComboBox comboBox = optimizerView.ProductionUnitsComboBox;

        Avalonia.Pointer comboBoxPos = new Avalonia.Pointer(50, 50);
        Assert.False(comboBox.IsDropDownOpen);
        window.MouseDown(comboBoxPos, MouseButton.Left);
        window.MouseUp(comboBoxPos, MouseButton.Left);
        window.MouseDown(comboBoxPos, MouseButton.Left);
        window.MouseUp(comboBoxPos, MouseButton.Left);
        window.MouseDown(comboBoxPos, MouseButton.Left);
        window.MouseUp(comboBoxPos, MouseButton.Left);
        window.MouseDown(comboBoxPos, MouseButton.Left);
        window.MouseUp(comboBoxPos, MouseButton.Left);
        Assert.False(comboBox.IsDropDownOpen);
    }

    [AvaloniaFact]
    public void Configuration_FromRangeValidNumber()
    {
        var window = new MainWindow();

        window.Show();

        StackPanel injectorNode = window.InjectorNode;
        ScenarioNav scenarioNav = (ScenarioNav)injectorNode.Children[0];
        TopTabStrip strip = (TopTabStrip)scenarioNav.TopTabStripNode;
        strip.SelectIndex(3);
        Panel panel = (Panel)scenarioNav.InjectorNode;
        ConfigurationView configurationView = (ConfigurationView)panel.Children[0];
        TextBox textBox = optimizerView.FromTxtInput;

        Assert.Equal("", textBox.Text);
        textBox.Focus();
        window.KeyTextInput("30");
        Assert.Equal("30", textBox.Text);
    }

    [AvaloniaFact]
    public void Configuration_FromRangeInvalidNumber()
    {
        var window = new MainWindow();

        window.Show();

        StackPanel injectorNode = window.InjectorNode;
        ScenarioNav scenarioNav = (ScenarioNav)injectorNode.Children[0];
        TopTabStrip strip = (TopTabStrip)scenarioNav.TopTabStripNode;
        strip.SelectIndex(3);
        Panel panel = (Panel)scenarioNav.InjectorNode;
        ConfigurationView configurationView = (ConfigurationView)panel.Children[0];
        TextBox textBox = optimizerView.FromTxtInput;

        Assert.Equal("", textBox.Text);
        textBox.Focus();
        window.KeyTextInput("asd[]/");
        Assert.Equal("", textBox.Text);
    }

    [AvaloniaFact]
    public void Configuration_FromRangeLargeNumber()
    {
        var window = new MainWindow();

        window.Show();

        StackPanel injectorNode = window.InjectorNode;
        ScenarioNav scenarioNav = (ScenarioNav)injectorNode.Children[0];
        TopTabStrip strip = (TopTabStrip)scenarioNav.TopTabStripNode;
        strip.SelectIndex(3);
        Panel panel = (Panel)scenarioNav.InjectorNode;
        ConfigurationView configurationView = (ConfigurationView)panel.Children[0];
        TextBox textBox = optimizerView.FromTxtInput;

        Assert.Equal("", textBox.Text);
        textBox.Focus();
        window.KeyTextInput("99999999999");
        Assert.Equal("99999999999", textBox.Text);
    }
}
