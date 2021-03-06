﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
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
        driver.Manage().Window.Minimize();
    }

    // Will look up a patient from their NHS number and return all flags as a list of strings
    public List<string> GetFlags(string nhsNumber)
    {
        SearchNHS(nhsNumber);
        if (!checkNHS(nhsNumber))
        {
            throw new Exception("Error on page");
        }

        return CheckFlags();
    }

    // From the main page, this will access a specific patient's records
    private void SearchNHS(string number)
    {
        IReadOnlyCollection<IWebElement> nhsNumberField = GetElements(By.Name("nhsNumber"));
        if(nhsNumberField.Count == 0)
        {
            driver.Navigate().GoToUrl("https://portal2.national.ncrs.nhs.uk/summarycarerecord/patientsearch");
        }
        nhsNumberField = GetElements(By.Name("nhsNumber"));

        nhsNumberField.First().Clear();
        nhsNumberField.First().SendKeys(number);
        nhsNumberField.First().Submit();
    }

    private bool checkNHS(string nhsNum, int timeoutAccumulator = 0)
    {
        IReadOnlyCollection<IWebElement> nhsNumElem = GetElements(By.Id("keyDetailsNHSNumber"));

        if (nhsNumElem.Count == 0)
        {
            
            if (timeoutAccumulator > 5000)
            {
                return false;

            } else
            {
                Thread.Sleep(500);
                return checkNHS(nhsNum, timeoutAccumulator + 500);
            }
            
        }
        else if (String.Equals(nhsNumElem.First().Text.Replace(" ", ""), nhsNum))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    // Once on the patient's records, this will return a list of strings as to what flags they have active
    private List<string> CheckFlags()
    {
        List<string> flags = new List<string>();

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

        
        GetElements(By.Id("BasicSearch")).First().Click();

        return flags;
    }

    private IReadOnlyCollection<IWebElement> GetElements(By by, int sleepCumulator = 0)
    {

        Thread.Sleep(200);

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
            Thread.Sleep(500);
            return GetElements(by, sleepCumulator + 500);
        }

        return elems;
    }

    public void Close()
    {
        driver.Close();
    }
}
