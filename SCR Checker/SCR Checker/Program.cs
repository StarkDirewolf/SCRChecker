using OpenQA.Selenium.IE;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System;
using System.Windows.Forms;

namespace SCR_Checker
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        // Won't work when exporting or using a different computer or whatever
    //        //IWebDriver driver = new InternetExplorerDriver(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Driver");

    //       // SCR scr = new SCR(driver);
    //        //MasterProcessor proc = new MasterProcessor(scr);
    //        MasterProcessor.test();

    //        //searchNHS(driver, "4929724910");
    //        // searchNHS(driver, "4308361001");

    //    }


    //}

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
