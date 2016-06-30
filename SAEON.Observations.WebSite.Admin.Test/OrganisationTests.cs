using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;

namespace SAEON.Observations.WebSite.Admin.Test
{
    [TestClass]
    public class OrganisationTests
    {
        static IWebDriver driver;
        string baseURL = "http://localhost:54378/Admin/Organisations";

        [AssemblyInitialize]
        public static void Setup(TestContext context)
        {
            driver = new ChromeDriver();
            //FirefoxProfile prof = new FirefoxProfile();
            //prof.SetPreference("browser.startup.homepage_override.mstone", "ignore");
            //prof.SetPreference("browser.startup.homepage", "about:blank");
            //prof.SetPreference("startup.homepage_welcome_url", "about:blank");
            //prof.SetPreference("startup.homepage_welcome_url.additional", "about:blank");
            //driver = new FirefoxDriver(prof);
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            //driver.Close();
        }

        private WebDriverWait WaitUpTo(TimeSpan timeSpan)
        {
            return new WebDriverWait(driver, timeSpan);
        }

        private void TryClick(By selector)
        {
            var wait = WaitUpTo(TimeSpan.FromSeconds(10));
            var element = wait.Until(ExpectedConditions.ElementIsVisible((selector)));
            WaitUpTo(TimeSpan.FromSeconds(10)).Until(d => element.Enabled);
            element.Click();
        }

        [TestMethod]
        public void AddOrganisation()
        {
            driver.Navigate().GoToUrl(baseURL);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ext-gen24"))).Click();
            driver.FindElement(By.Id("tfCode")).SendKeys("_Test_Org_2");
            driver.FindElement(By.Id("tfName")).SendKeys("_Test Org 2");
            driver.FindElement(By.Id("tfDescription")).SendKeys("Test Org 2");
            var btnSave = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//table[@id='ContentPlaceHolder1_btnSave']/tbody/tr[2]/td[2]")));
            btnSave.Click();
        }

        //[TestMethod]
        public void EditOrganisation()
        {
            driver.Navigate().GoToUrl(baseURL);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ext-gen216"))).Click();
            var tfDescription = driver.FindElement(By.Id("tfDescription"));
            tfDescription.Clear();
            tfDescription.SendKeys("Test Org Two");
            var btnSave = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//table[@id='ContentPlaceHolder1_btnSave']/tbody/tr[2]/td[2]")));
            btnSave.Click();
        }

    }
}
