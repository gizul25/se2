using NUnit.Framework;
using SE2.Data;
using SE2.Domain;
using SE2.ViewModels;

namespace SE2.Test
{
    public class Winter_Empty : IPeriod
    {
        public string Period()
        {
            return "winter_empty";
        }
    }

    public class FakePeriod : IPeriod
    {
        public string Period()
        {
            return "not Existing";
        }
    }

    public class SE2Test
    {
        [Test]
        public void LoadWinterSDM()
        {
            SDM sdm = new();
            sdm.Load(new Winter());
            Assert.Pass();
        }

        [Test]
        public void LoadWinterSDMEmptyStart()
        {
            SDM sdm = new();
            Assert.Throws<IndexOutOfRangeException>(() => sdm.Load(new Winter_Empty()));
            Assert.Pass();
        }

        [Test]
        public void LoadNotExistingSDM()
        {
            SDM sdm = new();
            Assert.Throws<FileNotFoundException>(() => sdm.Load(new FakePeriod()));
            Assert.Pass();
        }

        [Test]
        public void SaveWinterRDM()
        {
            RDM rdm = new();
            rdm.Save(new Winter());
            Assert.Pass();
        }

        [Test]
        public void LoadScenarioJson()
        {
            ScenarioLoader scenarioLoader = new();
            ScenarioData scenarioData = scenarioLoader.Load("1");
            Assert.IsNotNull(scenarioData);
            Assert.Pass();
        }

        [Test]
        public void LoadScenarioJsonInvalid()
        {
            ScenarioLoader scenarioLoader = new();
            Assert.Throws<Exception>(() => scenarioLoader.Load("111"));
            Assert.Pass();
        }
    }
    
    public class OptimizerTests
        {
            private Optimizer CreateValidOptimizer()
            {
                return new Optimizer
                {
                    Sources = new List<SourceData>
                    {
                        new SourceData { StartTime = DateTime.Today, HeatDemand = 5, ElectricityPrice = 100 }
                    },
                    Assets = new List<Asset>
                    {
                        new Asset { Name = "Asset1", MaxHeat = 10, MaxElectricity = 20, ProductionCosts = 50, Image = "test.jpg" }
                    }
                };
            }

            [Test]
            public void LoadNotExistingAsset()
            {
                var optimizer = CreateValidOptimizer();
                optimizer.Assets.Add(null!);
                Assert.Throws<Exception>(() => optimizer.OptimizerInit());
            }
            
            [Test]
            public void CalculateSchedule_WithInsufficientAssets()
            {
                var optimizer = CreateValidOptimizer();
                optimizer.Assets.Clear();
                Assert.Throws<Exception>(() => optimizer.CalculateSchedule());
            }
            
            [Test]
            public void LoadExists()
            {
                var optimizer = CreateValidOptimizer();
                Assert.DoesNotThrow(() => optimizer.OptimizerInit());
            }

            [Test]
            public void CalculateSchedule_WithCorrectAssets()
            {
                var optimizer = CreateValidOptimizer();
                optimizer.OptimizerInit();
                var result = optimizer.CalculateSchedule();
                Assert.IsNotNull(result);
            }

            [Test]
            public void CalculateNetCosts_WithEmptyAssets()
            {
                var optimizer = CreateValidOptimizer();
                optimizer.Assets = new List<Asset>();
                Assert.Throws<Exception>(() => optimizer.CalculateNetCost());
            }

			[Test]
			public void OptimizerInit_InvalidMaintanance()
			{
				var optimizer = CreateValidOptimizer();
                optimizer.Assets.Add(new Asset 
				{ 
					Name = "TestAssetInvalid", 
					MaxHeat = 10, 
					MaintananceStart = DateTime.Today.AddHours(10), 
					MaintananceEnd = DateTime.Today.AddHours(5)
				});
                Assert.Throws<Exception>(() => optimizer.OptimizerInit());
			}

			[Test]
            public void OptimizerInit_ValidMaintanance()
			{
    			var optimizer = CreateValidOptimizer();
    			optimizer.Assets.Add(new Asset 
    			{ 
        			Name = "TestAssetValid", 
        			MaxHeat = 10, 
        			MaintananceStart = DateTime.Today, 
        			MaintananceEnd = DateTime.Today.AddHours(40) 
    			});
    			Assert.DoesNotThrow(() => optimizer.OptimizerInit());
			}

			[Test]
			public void CalculateSchedules_HeatOnly_CorrectTotals()
			{
				var optimizer = new Optimizer();
        
			    optimizer.Assets.Add(new Asset {
					Name = "Boiler A",
					MaxHeat = 100,
					MaxElectricity = 0,
					ProductionCosts = 50,
					Co2Emissions = 1,
					GasConsumption = 0.8f,
					OilConsumption = 0f
				});
        
				optimizer.Sources.Add(new SourceData {
					StartTime = new DateTime(2026, 5, 1, 8, 0, 0),
					HeatDemand = 100,
					ElectricityPrice = 100
				});
        
				optimizer.OptimizerInit();
				
				var result = optimizer.CalculateSchedule();
			
				Assert.AreEqual(100, result.HeatProduced);
				Assert.AreEqual(0, result.ElectricityProduced);
				Assert.AreEqual(0, result.ElectricityConsumed);
				Assert.AreEqual(5000, result.TotalCost, 0.01);
				Assert.AreEqual(1, result.Co2Emissions, 0.01);
				Assert.AreEqual(0.8, result.PrimaryEnergy, 0.01);
			}
			
			[Test]
			public void CalculateSchedule_MixedWithNegativePrice_SelectsCheaperAsset()
			{
				// Arrange
				var optimizer = new Optimizer();
        
				optimizer.Assets.Add(new Asset {
					Name = "Gas Motor",
					MaxHeat = 100,
					MaxElectricity = 50,
					ProductionCosts = 40,
					Co2Emissions = 1,
					GasConsumption = 0.7f,
					OilConsumption = 0f
				});
        
				optimizer.Sources.Add(new SourceData {
					StartTime = new DateTime(2026, 5, 1, 12, 0, 0),
					HeatDemand = 100,
					ElectricityPrice = -50
				});
        
				optimizer.OptimizerInit();
				
				var result = optimizer.CalculateSchedule();
			
				Assert.AreEqual(100, result.HeatProduced);
				Assert.AreEqual(50, result.ElectricityProduced);
				Assert.AreEqual(0, result.ElectricityConsumed);
				Assert.AreEqual(6500, result.TotalCost, 0.01);
			}
        }

		public class OverviewViewModelTests
        {
            [Test]
            public void OverviewViewModel_Initialization()
            {
                var viewModel = new OverviewViewModel();
                Assert.IsNotNull(viewModel);
                Assert.IsNotNull(viewModel.HeatSeries);
        		Assert.IsNotNull(viewModel.ElectricitySeries);
        		Assert.IsNotNull(viewModel.PriceSeries);
        		Assert.IsNotNull(viewModel.ExpenseSeries);
            }

			[Test]
			public void OverviewViewModel_LoadData()
			{
                var viewModel = new OverviewViewModel();	
				Assert.IsEmpty(viewModel.HeatSeries);
        		Assert.IsEmpty(viewModel.ElectricitySeries);
        		Assert.IsEmpty(viewModel.PriceSeries);
        		Assert.IsEmpty(viewModel.ExpenseSeries);
			}

			[Test]
			public void OverviewViewModel_LoadDataWithData()
			{
				DM.Init();
				DM.StartOptimizer();
				var viewModel = new OverviewViewModel();
				viewModel.LoadCommand.Execute(null);
				Assert.AreEqual(2, viewModel.HeatSeries.Length);
        		Assert.AreEqual(2, viewModel.ElectricitySeries.Length);
        		Assert.AreEqual(2, viewModel.PriceSeries.Length);
        		Assert.AreEqual(2, viewModel.ExpenseSeries.Length);
			}	
        }
}