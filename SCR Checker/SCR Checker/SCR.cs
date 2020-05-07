using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

public class SCR
{
    private const string POSITIVE_TEST = "Positive test", SUSPECTED_INDICATOR = "Suspected indicator", AT_RISK = "Shielded", NONE = "None", PAGE_DIDNT_LOAD = "ERROR: SCR page didn't load";
	private IWebDriver driver;
    private const int TIMEOUT = 15000;

    public SCR(IWebDriver driver)
    {
        this.driver = driver;

        // Open up the correct page
        driver.Navigate().GoToUrl("https://portal2.national.ncrs.nhs.uk/summarycarerecord/");

        IReadOnlyCollection<IWebElement> nhsNumber = GetElements(By.Name("nhsNumber"));
        // If it attempts to restore a previous search, go back to the main page
        if (!nhsNumber.First().Displayed)
        {
            IWebElement newSearchButton = GetElements(By.Name("searchBtn")).First();
            newSearchButton.Click();

        }
    }

    // Will look up a patient from their NHS number and return all flags as a list of strings
    public List<string> GetFlags(string nhsNumber)
    {
        SearchNHS(nhsNumber);
        return CheckFlags(nhsNumber);
    }

    // From the main page, this will access a specific patient's records
    private void SearchNHS(string number)
    {
        IReadOnlyCollection<IWebElement> nhsNumberField = GetElements(By.Name("nhsNumber"));
        if(nhsNumberField.Count == 0)
        {
            IReadOnlyCollection<IWebElement> okButton = GetElements(By.Name("covidModalOk"));
            if (okButton.Count > 0)
            {
                okButton.First().Click();
            }
            IReadOnlyCollection<IWebElement> searchButton = GetElements(By.Id("BasicSearch"));
            if (searchButton.Count > 0)
            {
                searchButton.First().Click();
            }
            nhsNumberField = GetElements(By.Name("nhsNumber"));
        }
        nhsNumberField.First().Clear();
        nhsNumberField.First().SendKeys(number);
        nhsNumberField.First().Submit();
    }

    // Once on the patient's records, this will return a list of strings as to what flags they have active
    private List<string> CheckFlags(string nhsNum)
    {
        List<string> flags = new List<string>();

        IReadOnlyCollection<IWebElement> nhsNumElem = GetElements(By.Id("keyDetailsNHSNumber"));
        if (nhsNumElem.Count == 0)
        {
            string noSpaceNHS = Regex.Replace(nhsNumElem.First().Text, @"\s", "");
            if (!noSpaceNHS.Equals(nhsNum))
            {
                flags.Add(PAGE_DIDNT_LOAD);
                return flags;
            }
        }

        IReadOnlyCollection<IWebElement> covidAtRiskIfCaughtList = GetElements(By.Name("covidAtRiskIfCaught"));

        if (covidAtRiskIfCaughtList.Count > 0)
        {
            IReadOnlyCollection<IWebElement> positiveTestList = GetElements(By.Name("covidPositiveTest"));
            IReadOnlyCollection<IWebElement> suspectedIndicatorList = GetElements(By.Name("suspectedIndicator"));
            if (positiveTestList.First().GetAttribute("value") == "True")
            {
                flags.Add(POSITIVE_TEST);
            }
            if (suspectedIndicatorList.First().GetAttribute("value") == "True")
            {
                flags.Add(SUSPECTED_INDICATOR);
            }
            if (covidAtRiskIfCaughtList.First().GetAttribute("value") == "True")
            {
                flags.Add(AT_RISK);
            }
            IWebElement okButton = GetElements(By.Name("covidModalOk")).First();
            okButton.Click();
        }

        if (flags.Count == 0) flags.Add(NONE);

        // This may break as the name was guessed
        GetElements(By.Id("BasicSearch")).First().Click();

        return flags;
    }

    private IReadOnlyCollection<IWebElement> GetElements(By by, int sleepCumulator = 0)
    {
        Thread.Sleep(400);
        IReadOnlyCollection<IWebElement> elems = driver.FindElements(by);

        if (elems.Count == 0)
        {
            if (driver.FindElements(By.Id("BasicSearch")).Count > 0)
            {
                elems = driver.FindElements(by);
                return elems;
            }
            if (sleepCumulator > TIMEOUT)
            {
                return elems;
            }
            Thread.Sleep(1000);
            return GetElements(by, sleepCumulator + 1000);
        }

        return elems;
    }
}
