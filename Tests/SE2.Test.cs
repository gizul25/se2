using NUnit.Framework;
using SE2.Data;

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
}

