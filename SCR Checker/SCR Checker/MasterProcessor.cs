using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using SCR_Checker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

public class MasterProcessor
{
	SCR scr;

	public MasterProcessor()
	{
		IWebDriver driver = new InternetExplorerDriver(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Driver");
		scr = new SCR(driver);
	}

	public void Start(DateTime fromDate, DateTime toDate, ProgressBar progressBar)
	{
		Dictionary<string, string> nhsNumPatientName = SQLQueryer.GetDeliveryNHSNumbers(fromDate, toDate);

		progressBar.Minimum = 1;
		progressBar.Maximum = nhsNumPatientName.Count;
		progressBar.Value = 1;
		progressBar.Step = 1;

		SpreadsheetHandler doc = new SpreadsheetHandler();

		foreach (KeyValuePair<string, string> entry in nhsNumPatientName)
		{
			List<string> flags = new List<string>();
			try
			{
				flags = scr.GetFlags(entry.Key);
			}
			catch
			{
				flags.Add("ERROR - please look up manually");
			}

			List<string> row = new List<string>();
			row.Add(entry.Value);
			row.Add(entry.Key);
			row.AddRange(flags);

			doc.AddRow(row);

			progressBar.PerformStep();
		}

		doc.Save();
		doc.OpenFile();
	}
}
