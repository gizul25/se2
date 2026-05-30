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
using SE2;
using Xunit;

namespace EndToEnd
{
    public class EndToEndTests
    {
        [AvaloniaFact]
        public void Positive_MainWindow_OpensAndShowsScenarioNavigation()
        {
            SE2.Domain.DM.Init();
            SE2.Domain.DM.StartOptimizer();

            var window = new MainWindow();
            window.Show();

            var scenarioNav = GetFirstScenarioNav(window);

            Assert.NotNull(scenarioNav);
            Assert.NotNull(scenarioNav.FindControl<TopTabStrip>("TopTabStripNode"));
        }

        [AvaloniaFact]
        public void Positive_OptimizerButton_CommandCanExecute()
        {
            SE2.Domain.DM.Init();
            SE2.Domain.DM.StartOptimizer();

            var window = new MainWindow();
            window.Show();

            var optimizerView = GetOptimizerView(window);
            var optimizerBtn = optimizerView.FindControl<Button>("OptimizerBtn");

            Assert.NotNull(optimizerBtn);
            Assert.NotNull(optimizerBtn!.Command);

            var exception = Record.Exception(() => optimizerBtn.Command.Execute(null));

            Assert.Null(exception);
        }

        [Fact]
        public void Negative_InvalidScenarioId_ThrowsFileNotFoundException()
        {
            var scenarioLoader = new ScenarioLoader();

            Assert.ThrowsAny<Exception>(() => scenarioLoader.Load("999"));
        }

        [Fact]
        public void Negative_OptimizerWithoutAssets_ThrowsException()
        {
            var optimizer = new Optimizer();

            Assert.ThrowsAny<Exception>(() => optimizer.CalculateSchedule());
        }

        [Fact]
        public void Edge_DefaultScenarioPeriod_IsWinter()
        {
            var viewModel = new ScenarioNavViewModel();

            Assert.Equal("Winter period", viewModel.SelectedPeriod);
        }

        [Fact]
        public void Edge_ChartControlViewModel_DefaultSeries_IsEmpty()
        {
            var viewModel = new ChartControlViewModel();

            Assert.NotNull(viewModel.Series);
            Assert.Empty(viewModel.Series);
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
}