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

        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (isStart)
            {
                isStart = false;

                startButton.Text = "Cancel";
                progressBar1.Visible = true;

                processor = new MasterProcessor();

                processor.Start(fromDateTimePicker1.Value, toDateTimePicker2.Value, progressBar1);
            } else
            {
                isStart = true;

                startButton.Text = "Start";
                progressBar1.Value = 0;
                progressBar1.Visible = false;

                processor.Cancel();
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openBtn_Click(object sender, EventArgs e)
        {

        }

        private void saveBtn_Click(object sender, EventArgs e)
        {

        }

        public static void ShowEndButtons()
        {
            Form1.saveBtn.Visible = true;
            openBtn.Visible = true;
            startBtn.Visible = false;
            Form1.
        }

    }
}
