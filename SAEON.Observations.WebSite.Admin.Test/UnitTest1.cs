using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;

namespace SAEON.Observations.WebSite.Admin.Test
{
    [TestClass]
    public class UnitTest1
    {
        //static IWebDriver driverFirefox;
        static IWebDriver driverChrome;

        [AssemblyInitialize]
        public static void Setup(TestContext context)
        {
            //driverFirefox = new FirefoxDriver();
            driverChrome = new ChromeDriver();
        }

        //[TestMethod]
        //public void TestFirefox()
        //{
        //    driverFirefox.Navigate().GoToUrl("https://www.google.co.za");
        //    driverFirefox.FindElement(By.Id("lst-ib")).SendKeys("Selenium");
        //    driverFirefox.FindElement(By.Id("lst-ib")).SendKeys(Keys.Enter);
        //}

        [TestMethod]
        public void TestChrome()
        {
            driverChrome.Navigate().GoToUrl("https://www.google.co.za");
            driverChrome.FindElement(By.Id("lst-ib")).SendKeys("Selenium");
            driverChrome.FindElement(By.Id("lst-ib")).SendKeys(Keys.Enter);
        }

    }
}
