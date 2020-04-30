using OpenQA.Selenium.IE;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SCR_Checker
{
    class Program
    {
        static void Main(string[] args)
        {
            // Won't work when exporting or using a different computer or whatever
            IWebDriver driver = new InternetExplorerDriver("C:\\Users\\Careway LINK\\source\\repos\\SCR Checker\\SCR Checker\\Driver");

            startWindow(driver);

            searchNHS(driver, "4929724910");
            // searchNHS(driver, "4308361001");
            checkFlag(driver);

        }

        private static void startWindow(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://portal2.national.ncrs.nhs.uk/summarycarerecord/");
            Thread.Sleep(4000);
            IWebElement nhsNumberField = driver.FindElement(By.Name("nhsNumber"));
            if (nhsNumberField.Displayed == false)
            {
                IWebElement newSearchButton = driver.FindElement(By.Name("searchBtn"));
                newSearchButton.Click();
                Thread.Sleep(2000);
            }
        }

        private static void searchNHS(IWebDriver driver, string number)
        {
            IWebElement nhsNumberField = driver.FindElement(By.Name("nhsNumber"));
            nhsNumberField.SendKeys(number);
            nhsNumberField.Submit();
            Thread.Sleep(2000);
        }

        private static List<IWebElement> checkFlag(IWebDriver driver)
        {
            List<IWebElement> elements = new List<IWebElement>();

            IReadOnlyCollection<IWebElement> positiveTestList = driver.FindElements(By.Name("covidPositiveTest"));
            IReadOnlyCollection<IWebElement> suspectedIndicatorList = driver.FindElements(By.Name("suspectedIndicator"));
            IReadOnlyCollection<IWebElement> covidAtRiskIfCaughtList = driver.FindElements(By.Name("covidAtRiskIfCaught"));

            if (covidAtRiskIfCaughtList.Count > 0)
            {
                if (positiveTestList.First().GetAttribute("value") == "True")
                {
                    elements.Add(positiveTestList.First());
                }
                if (suspectedIndicatorList.First().GetAttribute("value") == "True")
                {
                    elements.Add(suspectedIndicatorList.First());
                }
                if (covidAtRiskIfCaughtList.First().GetAttribute("value") == "True")
                {
                    elements.Add(covidAtRiskIfCaughtList.First());
                }
                IWebElement okButton = driver.FindElement(By.Name("covidModalOk"));
            }

            return elements;
        }
    }
}
