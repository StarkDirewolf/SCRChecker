using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using SCR_Checker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

public class MasterProcessor
{
	SCR scr;

	public MasterProcessor()
	{
		IWebDriver driver = new InternetExplorerDriver(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Driver");
		scr = new SCR(driver);
	}

	public async void Start(DateTime fromDate, DateTime toDate, ProgressBar progressBar)
	{
		Dictionary<string, string> nhsNumPatientName = await Task.Run(() => SQLQueryer.GetDeliveryNHSNumbers(fromDate, toDate));

		progressBar.Minimum = 1;
		progressBar.Maximum = nhsNumPatientName.Count;
		progressBar.Value = 1;
		progressBar.Step = 1;

		SpreadsheetHandler doc = new SpreadsheetHandler();

		bool dontStop = true;
		int attempt = 0;
		int maxAttempts = 3;
		IEnumerator<KeyValuePair<string, string>> e = nhsNumPatientName.GetEnumerator();
		e.MoveNext();
		
		while (dontStop)
		{
			KeyValuePair<string, string> entry = e.Current;

			List<string> flags = new List<string>();
			await Task.Run(() =>
			{
				try
				{
					flags = scr.GetFlags(entry.Key);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					attempt += 1;
					if (attempt == maxAttempts)
					{
						flags.Add("ERROR - please look up manually");
						attempt = 0;
					}

				}
			});

			if (flags.Count > 0)
			{
				List<string> row = new List<string>();
				row.Add(entry.Value);
				row.Add(entry.Key);
				row.AddRange(flags);

				doc.AddRow(row);

				progressBar.PerformStep();

				if (!e.MoveNext())
				{
					dontStop = false;
				}
			}

		}

		scr.Close();
		doc.Save();
		doc.OpenFile();

	}
}
