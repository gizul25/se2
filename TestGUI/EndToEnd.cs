using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using CommunityToolkit.Mvvm.Input;
using SE2.Views;
using SE2.ViewModels;
using SE2.Domain;
using SE2.Data;
using Xunit;

namespace SE2.Headless.XUnit;

public class EndToEndTests
{
    public EndToEndTests()
    {
        DM.Init();
        DM.StartOptimizer();
    }

    
    
    [AvaloniaFact]
    public async Task Positive_FullFlow_SelectScenario_Period_RunOptimizer_CheckCharts()
    {
        var window = new MainWindow();
        window.Show();

        var scenarioNav = GetFirstScenarioNav(window);

        
        var scenarioList = scenarioNav.FindControl<ListBox>("ScenarioList");
        Assert.NotNull(scenarioList);
        scenarioList.SelectedIndex = 0;

        
        var periodCombo = scenarioNav.FindControl<ComboBox>("PeriodComboBox");
        Assert.NotNull(periodCombo);
        periodCombo.SelectedItem = "Winter period";

        
        var optimizerView = GetOptimizerView(window);

        
        var optimizerBtn = optimizerView.FindControl<Button>("OptimizerBtn");
        Assert.NotNull(optimizerBtn);

        var cmd = optimizerBtn.Command as IAsyncRelayCommand;
        Assert.NotNull(cmd);

        await cmd!.ExecuteAsync(null);

    
        var vm = optimizerView.DataContext as OptimizerViewModel;
        Assert.NotNull(vm);

        Assert.True(vm!.ExportEnabled);
        Assert.True(vm.Charts.Count >= 5);

        foreach (var chart in vm.Charts)
            Assert.NotEmpty(chart.Series);

        Assert.NotEqual(0, vm.TotalCost);
        Assert.NotEqual(0, vm.HeatProduced);
    }

    [AvaloniaFact]
    public async Task Positive_ChangePeriod_ReloadsCharts()
    {
        var window = new MainWindow();
        window.Show();

        var scenarioNav = GetFirstScenarioNav(window);

        var periodCombo = scenarioNav.FindControl<ComboBox>("PeriodComboBox");
        Assert.NotNull(periodCombo);

        periodCombo.SelectedItem = "Winter period";

        var optimizerView = GetOptimizerView(window);
        var vm = optimizerView.DataContext as OptimizerViewModel;

        var optimizerBtn = optimizerView.FindControl<Button>("OptimizerBtn");
        var cmd = optimizerBtn.Command as IAsyncRelayCommand;

        await cmd!.ExecuteAsync(null);

        var initialCount = vm!.Charts.Count;

        
        periodCombo.SelectedItem = "Summer period";

        await cmd.ExecuteAsync(null);

        Assert.True(vm.Charts.Count == initialCount);
        foreach (var chart in vm.Charts)
            Assert.NotEmpty(chart.Series);
    }

    
    [Fact]
    public void Negative_InvalidScenario_Throws()
    {
        var loader = new ScenarioLoader();
        Assert.ThrowsAny<Exception>(() => loader.Load("999"));
    }


    [Fact]
    public async Task Negative_OptimizerWithoutAssets_Throws()
    {
        DM.Init();
        DM.AM.Assets.Clear();

        var vm = new OptimizerViewModel();
        var cmd = vm.RunOptimizationCommand;

        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await cmd.ExecuteAsync(null);
        });
    }

    

    [Fact]
    public void Edge_DefaultPeriod_IsWinter()
    {
        var vm = new ScenarioNavViewModel();
        Assert.Equal("Winter period", vm.SelectedPeriod);
    }

    [Fact]
    public void Edge_ChartsEmpty_WhenNoResults()
    {
        DM.Init();
        DM.RDM.SetCurrentScenarioResultingData(null!);

        var vm = new OptimizerViewModel();

        Assert.Empty(vm.Charts);
        Assert.False(vm.ExportEnabled);
    }

   

    private static ScenarioNav GetFirstScenarioNav(MainWindow window)
    {
        var injectorNode = window.FindControl<Panel>("InjectorNode");
        Assert.NotNull(injectorNode);

        var scenarioNav = injectorNode!.Children.OfType<ScenarioNav>().FirstOrDefault();
        Assert.NotNull(scenarioNav);

        return scenarioNav!;
    }

    private static OptimizerView GetOptimizerView(MainWindow window)
    {
        var scenarioNav = GetFirstScenarioNav(window);

        var topTabStrip = scenarioNav.FindControl<TopTabStrip>("TopTabStripNode");
        var scenarioInjector = scenarioNav.FindControl<Panel>("InjectorNode");

        Assert.NotNull(topTabStrip);
        Assert.NotNull(scenarioInjector);

        topTabStrip!.SelectIndex(1);

        var optimizerView = scenarioInjector!.Children.OfType<OptimizerView>().FirstOrDefault();
        Assert.NotNull(optimizerView);

        return optimizerView!;
    }
}
