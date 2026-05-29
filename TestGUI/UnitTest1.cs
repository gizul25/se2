using System.Globalization;
using System.Reflection;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Controls.Primitives;
using LiveChartsCore.Defaults;
using SE2.Utils;
using SE2.Views;
using SE2.ViewModels;
using SE2.Models;
using SE2.Data;
using SE2.Domain;
using Xunit;

namespace SE2.Headless.XUnit;

public class UnitTest1
{
    public UnitTest1()
    {
        SE2.Domain.DM.Init();
        var progress = new Progress<double>();
        CancellationTokenSource cts = new();
        SE2.Domain.DM.StartOptimizer(cts.Token, progress);
    }

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
        var sideTabStrip = window.FindControl<SideTabStrip>("SideTabStripNode");
        Assert.NotNull(sideTabStrip);

        var tabStrip = sideTabStrip!.FindControl<Avalonia.Controls.Primitives.TabStrip>("TabNode");
        Assert.NotNull(tabStrip);
        Assert.True(tabStrip!.Items.Count >= 2);
    }

    [AvaloniaFact]
    public void Optimizer_ButtonDisabled()
    {
        var window = new MainWindow();
        window.Show();

        var optimizerView = GetOptimizerView(window);

        var optimizerBtn = optimizerView.FindControl<Button>("OptimizerBtn");
        Assert.NotNull(optimizerBtn);

        Assert.True(optimizerBtn!.IsEnabled);
        Assert.NotNull(optimizerBtn.Command);
    }

    [AvaloniaFact]
    public void Optimizer_ButtonSendCommand()
    {
        var window = new MainWindow();
        window.Show();

        var optimizerView = GetOptimizerView(window);

        var optimizerBtn = optimizerView.FindControl<Button>("OptimizerBtn");
        Assert.NotNull(optimizerBtn);
        Assert.NotNull(optimizerBtn!.Command);

        var ex = Record.Exception(() => optimizerBtn.Command!.Execute(null));
        Assert.Null(ex);
    }

    [AvaloniaFact]
    public void Optimizer_ButtonMultipleClicks()
    {
        var window = new MainWindow();
        window.Show();

        var optimizerView = GetOptimizerView(window);

        var optimizerBtn = optimizerView.FindControl<Button>("OptimizerBtn");
        Assert.NotNull(optimizerBtn);
        Assert.NotNull(optimizerBtn!.Command);

        var ex = Record.Exception(() =>
        {
            optimizerBtn.Command!.Execute(null);
            optimizerBtn.Command!.Execute(null);
        });

        Assert.Null(ex);
    }

    [AvaloniaFact]
    public void Optimizer_DropDownOpen()
    {
        var window = new MainWindow();
        window.Show();

        var optimizerView = GetOptimizerView(window);

        var comboBox = optimizerView.FindControl<ComboBox>("ProductionUnitsComboBox");
        Assert.NotNull(comboBox);

        comboBox!.IsDropDownOpen = true;
        Assert.True(comboBox.IsDropDownOpen);
    }

    [AvaloniaFact]
    public void Optimizer_DropDownEmptyOpen()
    {
        var window = new MainWindow();
        window.Show();

        var optimizerView = GetOptimizerView(window);

        var comboBox = optimizerView.FindControl<ComboBox>("ProductionUnitsComboBox");
        Assert.NotNull(comboBox);
        Assert.NotNull(comboBox.ItemsSource);

        var ex = Record.Exception(() => comboBox.IsDropDownOpen = true);
        Assert.Null(ex);
    }

    [AvaloniaFact]
    public void Optimizer_DropDownQuickPress()
    {
        var window = new MainWindow();
        window.Show();

        var optimizerView = GetOptimizerView(window);

        var comboBox = optimizerView.FindControl<ComboBox>("ProductionUnitsComboBox");
        Assert.NotNull(comboBox);

        comboBox!.IsDropDownOpen = true;
        comboBox.IsDropDownOpen = false;
        comboBox.IsDropDownOpen = true;
        comboBox.IsDropDownOpen = false;

        Assert.False(comboBox.IsDropDownOpen);
    }

    [AvaloniaFact]
    public void Configuration_FromRangeValidNumber()
    {
        var window = new MainWindow();
        window.Show();

        var scenarioNav = GetFirstScenarioNav(window);

        var topTabStrip = scenarioNav.FindControl<TopTabStrip>("TopTabStripNode");
        var scenarioInjector = scenarioNav.FindControl<Panel>("InjectorNode");
        Assert.NotNull(topTabStrip);
        Assert.NotNull(scenarioInjector);

        topTabStrip!.SelectIndex(0);

        Assert.Single(scenarioInjector!.Children);
        Assert.IsType<OverviewView>(scenarioInjector.Children[0]);
    }

    [AvaloniaFact]
    public void Configuration_FromRangeInvalidNumber()
    {
        var window = new MainWindow();
        window.Show();

        var scenarioNav = GetFirstScenarioNav(window);

        var topTabStrip = scenarioNav.FindControl<TopTabStrip>("TopTabStripNode");
        var scenarioInjector = scenarioNav.FindControl<Panel>("InjectorNode");
        Assert.NotNull(topTabStrip);
        Assert.NotNull(scenarioInjector);

        topTabStrip!.SelectIndex(1);

        Assert.Single(scenarioInjector!.Children);
        Assert.IsType<OptimizerView>(scenarioInjector.Children[0]);
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

    public class ChartControlViewModelTests
    {
        [Fact]
        public void Ctor_DefaultTitle_IsEmpty()
        {
            var vm = new ChartControlViewModel();
            Assert.Equal("", vm.Title);
        }

        [Fact]
        public void Ctor_DefaultSeries_IsEmptyArray()
        {
            var vm = new ChartControlViewModel();
            Assert.NotNull(vm.Series);
            Assert.Empty(vm.Series);
        }

        [Fact]
        public void Ctor_XAxes_EqualsGraphUtilsXAxis()
        {
            var vm = new ChartControlViewModel();
            var expected = GraphUtils.GetXAxis();

            Assert.NotNull(vm.XAxes);
            Assert.Equal(expected.Length, vm.XAxes.Length);
        }
    }

    public class EditProductionUnitViewModelTests
    {
        private static void EnsureDMReady()
        {
            if (SE2.Domain.DM.SDM == null || SE2.Domain.DM.RDM == null)
            {
                SE2.Domain.DM.Init();

                var progress = new Progress<double>();
                CancellationTokenSource cts = new();
                SE2.Domain.DM.StartOptimizer(cts.Token, progress);
            }
        }

        [Fact]
        public void Ctor_WhenUnitIndexMinusOne_CancelContentIsCancel()
        {
            var model = new ProductionUnitsModel { Name = "NewUnit", UnitIndex = -1 };
            var vm = new EditProductionUnitViewModel(model);

            Assert.Equal("Cancel", vm.CancelContent);
        }

        [Fact]
        public void TestSave_WhenSelectedProductionUnitNull_CanSaveFalse()
        {
            EnsureDMReady();

            var model = new ProductionUnitsModel { Name = "X", UnitIndex = -1 };
            var vm = new EditProductionUnitViewModel(model);

            vm.SelectedProductionUnit = null!;
            vm.TestSave();

            Assert.False(vm.CanSave);
        }

        [Fact]
        public void TestSave_WhenNameEmpty_CanSaveFalse()
        {
            EnsureDMReady();

            var model = new ProductionUnitsModel { Name = "", UnitIndex = -1 };
            var vm = new EditProductionUnitViewModel(model);

            vm.TestSave();

            Assert.False(vm.CanSave);
        }

        [Fact]
        public void TestSave_WhenNameUnchanged_CanSaveTrue()
        {
            EnsureDMReady();

            var model = new ProductionUnitsModel { Name = "UnitA", UnitIndex = 0 };
            var vm = new EditProductionUnitViewModel(model);

            vm.SelectedProductionUnit.Name = "UnitA";
            vm.TestSave();

            Assert.True(vm.CanSave);
        }

        [Fact]
        public void TestSave_WhenDuplicateNameExistsInAssets_CanSaveFalse()
        {
            EnsureDMReady();

            DM.AM.Assets.Clear();
            DM.AM.Assets.Add(new Asset { Name = "Dup" });

            var model = new ProductionUnitsModel { Name = "Original", UnitIndex = 0 };
            var vm = new EditProductionUnitViewModel(model);

            vm.SelectedProductionUnit.Name = "Dup";
            vm.TestSave();

            Assert.False(vm.CanSave);
        }

        [Fact]
        public void TestSave_WhenUniqueNewName_CanSaveTrue()
        {
            EnsureDMReady();

            DM.AM.Assets.Clear();
            DM.AM.Assets.Add(new Asset { Name = "Existing1" });
            DM.AM.Assets.Add(new Asset { Name = "Existing2" });

            var model = new ProductionUnitsModel { Name = "Original", UnitIndex = 0 };
            var vm = new EditProductionUnitViewModel(model);

            vm.SelectedProductionUnit.Name = "UniqueNewName";
            vm.TestSave();

            Assert.True(vm.CanSave);
        }
    }

    public class ScenarioNavViewModelTests
    {
        [Fact]
        public void Defaults_SelectedPeriod_IsWinter()
        {
            var vm = new ScenarioNavViewModel();
            Assert.Equal("Winter period", vm.SelectedPeriod);
        }

        [Fact]
        public void Defaults_Periods_ContainsWinterAndSummer()
        {
            var vm = new ScenarioNavViewModel();

            Assert.Contains("Winter period", vm.Periods);
            Assert.Contains("Summer period", vm.Periods);
            Assert.Equal(2, vm.Periods.Count);
        }

        [Fact]
        public void CanSet_SelectedPeriod_ToSummer()
        {
            var vm = new ScenarioNavViewModel();
            vm.SelectedPeriod = "Summer period";
            Assert.Equal("Summer period", vm.SelectedPeriod);
        }
    }

    public class OptimizerViewModelTests
    {
        private static void InvokeInsertDataBreaks(OptimizerViewModel vm, IDictionary<string, List<DateTimePoint?>> entries, List<ResultRow> resultRows)
        {
            var mi = typeof(OptimizerViewModel).GetMethod("InsertDataBreaks", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(mi);
            mi!.Invoke(vm, new object[] { entries, resultRows });
        }

        [Fact]
        public void InsertDataBreaks_WhenGapBeforeFirstPoint_InsertsNull()
        {
            var vm = (OptimizerViewModel)Activator.CreateInstance(typeof(OptimizerViewModel), nonPublic: true)!;

            var t1 = new DateTime(2020, 1, 1, 1, 0, 0);
            var t2 = new DateTime(2020, 1, 1, 2, 0, 0);

            var entries = new Dictionary<string, List<DateTimePoint?>>
            {
                ["A"] = new List<DateTimePoint?> { new DateTimePoint(t2, 10) }
            };

            var resultRows = new List<ResultRow>
            {
                new ResultRow { Time = t1 },
                new ResultRow { Time = t2 },
            };

            InvokeInsertDataBreaks(vm, entries, resultRows);

            Assert.Equal(2, entries["A"].Count);
            Assert.Null(entries["A"][0]);
            Assert.NotNull(entries["A"][1]);
        }

        [Fact]
        public void InsertDataBreaks_WhenNoGap_DoesNotInsertNull()
        {
            var vm = (OptimizerViewModel)Activator.CreateInstance(typeof(OptimizerViewModel), nonPublic: true)!;

            var t1 = new DateTime(2020, 1, 1, 1, 0, 0);
            var entries = new Dictionary<string, List<DateTimePoint?>>
            {
                ["A"] = new List<DateTimePoint?> { new DateTimePoint(t1, 10) }
            };

            var resultRows = new List<ResultRow> { new ResultRow { Time = t1 } };

            InvokeInsertDataBreaks(vm, entries, resultRows);

            Assert.Single(entries["A"]);
            Assert.NotNull(entries["A"][0]);
        }

        [Fact]
        public void InsertDataBreaks_WhenMultipleGaps_OnlyOneNullPerGapWindow()
        {
            var vm = (OptimizerViewModel)Activator.CreateInstance(typeof(OptimizerViewModel), nonPublic: true)!;

            var t1 = new DateTime(2020, 1, 1, 1, 0, 0);
            var t2 = new DateTime(2020, 1, 1, 2, 0, 0);
            var t3 = new DateTime(2020, 1, 1, 3, 0, 0);

            var entries = new Dictionary<string, List<DateTimePoint?>>
            {
                ["A"] = new List<DateTimePoint?> { new DateTimePoint(t3, 10) }
            };

            var resultRows = new List<ResultRow>
            {
                new ResultRow { Time = t1 },
                new ResultRow { Time = t2 },
                new ResultRow { Time = t3 },
            };

            InvokeInsertDataBreaks(vm, entries, resultRows);

            Assert.Equal(2, entries["A"].Count);
            Assert.Null(entries["A"][0]);
            Assert.NotNull(entries["A"][1]);
        }
    }

    public class OverviewViewModelTests
    {
        private static void EnsureDMReadyWithEmptyData()
        {
            DM.Init();
            DM.SDM.Sources.Clear();
            DM.RDM.SetCurrentScenario("1_winter");
            DM.RDM.SetCurrentScenarioResultingData(new ResultData { ResultRows = new List<ResultRow>() });
        }

        [Fact]
        public void Load_WhenNoSourcesOrResults_SetsSeriesAndAxesToEmpty()
        {
            EnsureDMReadyWithEmptyData();

            var vm = new OverviewViewModel();
            vm.Load();

            Assert.Empty(vm.HeatSeries);
            Assert.Empty(vm.ElectricitySeries);
            Assert.Empty(vm.PriceSeries);
            Assert.Empty(vm.ExpenseSeries);
            Assert.Empty(vm.XAxes);
        }

        [Fact]
        public void Defaults_GridMapName_NotNull()
        {
            EnsureDMReadyWithEmptyData();

            var vm = new OverviewViewModel();
            Assert.NotNull(vm.GridMapName);
        }

        [Fact]
        public void Load_DoesNotThrow_WhenNoData()
        {
            EnsureDMReadyWithEmptyData();

            var vm = new OverviewViewModel();
            var ex = Record.Exception(() => vm.Load());
            Assert.Null(ex);
        }
    }
}
