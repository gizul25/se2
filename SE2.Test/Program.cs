using NUnit.Framework;

namespace UnitTests
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
        public void LoadNotExistingSDM()
        {
            SDM sdm = new();
            Assert.Throws<Exception>(() => sdm.Load("not existing"));
            Assert.Pass();
        }
    }
}

