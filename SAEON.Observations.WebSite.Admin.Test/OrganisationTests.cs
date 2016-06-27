using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace SAEON.Observations.WebSite.Admin.Test
{
    [TestClass]
    public class OrganisationTests
    {
        static IWebDriver driverChrome;

        [AssemblyInitialize]
        public static void Setup(TestContext context)
        {
            driverChrome = new ChromeDriver();
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
