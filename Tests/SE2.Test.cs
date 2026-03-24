using NUnit.Framework;
using SE2.Data;
using SE2.Domain;

namespace SE2.Test
{
    public class SE2Test
    {
        [Test]
        public void LoadWinterSDM()
        {
            SDM sdm = new();
            sdm.Load("winter");
            Assert.Pass();
        }

        [Test]
        public void LoadWinterSDMEmptyStart()
        {
            SDM sdm = new();
            Assert.Throws<IndexOutOfRangeException>(() => sdm.Load("winter_empty"));
            Assert.Pass();
        }

        [Test]
        public void LoadNotExistingSDM()
        {
            SDM sdm = new();
            Assert.Throws<FileNotFoundException>(() => sdm.Load("not existing"));
            Assert.Pass();
        }

        [Test]
        public void SaveWinterRDM()
        {
            RDM rdm = new();
            rdm.Save("winter");
            Assert.Pass();
        }

        [Test]
        public void LoadNotExistingRDM()
        {
            RDM rdm = new();
            Assert.Throws<Exception>(() => rdm.Save("not existing"));
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
                Assert.Throws<Exception>(() => optimizer.calculateSchedule());
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
                var result = optimizer.calculateSchedule();
                Assert.IsNotNull(result);
                Assert.IsNotEmpty(result);
            }

            [Test]
            public void CalculateNetCosts_WithEmptyAssets()
            {
                var optimizer = CreateValidOptimizer();
                optimizer.Assets = new List<Asset>();
                Assert.Throws<Exception>(() => optimizer.calculateNetCost());
            }
        }
}
