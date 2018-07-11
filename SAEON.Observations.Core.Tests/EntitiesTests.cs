using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAEON.Observations.Core.Entities;
using System.Data.Entity;
using System.Linq;

namespace SAEON.Observations.Core.Tests
{
    [TestClass]
    public class EntitiesTests
    {
        static ObservationsDbContext db = null;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            db = new ObservationsDbContext();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            db = null;
        }

        [TestMethod]
        public void OfferingHasPhenomenaTest()
        {
            var offering = db.Offerings.Where(i => i.Phenomena.Any()).Include(i => i.Phenomena).FirstOrDefault();
            Assert.IsNotNull(offering, "Offering is null");
            Assert.IsNotNull(offering.Phenomena, "Offering.Phenomena is null");
            Assert.IsTrue(offering.Phenomena.Any(), "Offering.Phenomena.Any() is false");
        }

        [TestMethod]
        public void UnitHasPhenomenaTest()
        {
            var unit = db.Units.Where(i => i.Phenomena.Any()).Include(i => i.Phenomena).FirstOrDefault();
            Assert.IsNotNull(unit, "Unit is null");
            Assert.IsNotNull(unit.Phenomena, "Unit.Phenomena is null");
            Assert.IsTrue(unit.Phenomena.Any(), "Unit.Phenomena.Any() is false");
        }
    }
}
