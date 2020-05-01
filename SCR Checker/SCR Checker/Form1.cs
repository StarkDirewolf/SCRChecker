using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCR_Checker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Visible = false;
            progressBar1.Visible = true;

            IWebDriver driver = new InternetExplorerDriver(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Driver");
            SCR scr = new SCR(driver);
            MasterProcessor processor = new MasterProcessor(scr);

            processor.Start(fromDateTimePicker1.Value, toDateTimePicker2.Value, progressBar1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
