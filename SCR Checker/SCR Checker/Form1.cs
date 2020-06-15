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
        bool isStart = true;
        MasterProcessor processor;
        static Button staticButton;

        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if(String.Equals(staticButton.Text, "Open")) {
                processor.OpenFile();
            }
            else if (isStart)
            {
                isStart = false;

                startButton.Text = "Cancel";
                progressBar1.Visible = true;

                processor = new MasterProcessor();

                processor.Start(fromDateTimePicker1.Value, toDateTimePicker2.Value, progressBar1, nomadCheckBox.Checked);
            } else
            {
                isStart = true;

                startButton.Text = "Start";
                progressBar1.Value = progressBar1.Minimum;
                progressBar1.Visible = false;

                processor.Cancel();
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            staticButton = startButton;
        }

        public static void ShowEndButtons()
        {
            staticButton.Text = "Open";
        }

    }
}
