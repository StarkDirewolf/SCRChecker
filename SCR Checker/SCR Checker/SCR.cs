using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class SCR
{
    private const string POSITIVE_TEST = "Positive test", SUSPECTED_INDICATOR = "Suspected indicator", AT_RISK = "Shielded", NONE = "None";
	private IWebDriver driver;

    public SCR(IWebDriver driver)
    {
        this.driver = driver;

        // Open up the correct page
        driver.Navigate().GoToUrl("https://portal2.national.ncrs.nhs.uk/summarycarerecord/");
        Thread.Sleep(4000);

        // If it attempts to restore a previous search, go back to the main page
        IWebElement nhsNumberField = driver.FindElement(By.Name("nhsNumber"));
        if (nhsNumberField.Displayed == false)
        {
            IWebElement newSearchButton = driver.FindElement(By.Name("searchBtn"));
            newSearchButton.Click();
            Thread.Sleep(2000);

        }
    }

    // Will look up a patient from their NHS number and return all flags as a list of strings
    public List<string> GetFlags(string nhsNumber)
    {
        SearchNHS(nhsNumber);
        return CheckFlags();
    }

    // From the main page, this will access a specific patient's records
    private void SearchNHS(string number)
    {
        IWebElement nhsNumberField = driver.FindElement(By.Name("nhsNumber"));
        nhsNumberField.SendKeys(number);
        nhsNumberField.Submit();
        Thread.Sleep(2000);
    }

    // Once on the patient's records, this will return a list of strings as to what flags they have active
    private List<string> CheckFlags()
    {
        List<string> flags = new List<string>();

        IReadOnlyCollection<IWebElement> positiveTestList = driver.FindElements(By.Name("covidPositiveTest"));
        IReadOnlyCollection<IWebElement> suspectedIndicatorList = driver.FindElements(By.Name("suspectedIndicator"));
        IReadOnlyCollection<IWebElement> covidAtRiskIfCaughtList = driver.FindElements(By.Name("covidAtRiskIfCaught"));

        if (covidAtRiskIfCaughtList.Count > 0)
        {
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
            IWebElement okButton = driver.FindElement(By.Name("covidModalOk"));
        }

        if (flags.Count == 0) flags.Add(NONE);

        // This may break as the name was guessed
        driver.FindElement(By.Name("findPatient")).Click();
        Thread.Sleep(2000);

        return flags;
    }
}
