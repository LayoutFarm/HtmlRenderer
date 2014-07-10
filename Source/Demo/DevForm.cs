using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace HtmlRenderer.Demo
{
    public partial class DevForm : Form
    {
        public DevForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DemoForm demoForm = new DemoForm();
            demoForm.StartAtSampleIndex = 2;
            demoForm.PrepareSamples();

            demoForm.Show();
            demoForm.Activate();
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
             
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; ++i)
            {
                DemoForm demoForm = new DemoForm();
                demoForm.StartAtSampleIndex = 2;
                demoForm.PrepareSamples();

                demoForm.Show();
                demoForm.Activate();

                System.Threading.Thread.Sleep(10);

                demoForm.Close();

            }
        }
    }
}
