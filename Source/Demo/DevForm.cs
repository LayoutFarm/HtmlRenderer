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
            demoForm.Show();
            demoForm.Activate();
        }

        private void button2_Click(object sender, EventArgs e)
        {

          
        }
    }
}
