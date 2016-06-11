//2014 BSD, WinterDev

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
namespace Mini
{
    partial class FormDev : Form
    {
        public FormDev()
        {
            InitializeComponent();
            this.Load += new EventHandler(DevForm_Load);
            this.listBox1.DoubleClick += new EventHandler(listBox1_DoubleClick);
            this.Text = "DevForm: Double Click The Example!";
        }
        void listBox1_DoubleClick(object sender, EventArgs e)
        {
            //load sample form
            ExampleAndDesc exAndDesc = this.listBox1.SelectedItem as ExampleAndDesc;
            if (exAndDesc != null)
            {
                DemoBase exBase = Activator.CreateInstance(exAndDesc.Type) as DemoBase;
                if (exBase == null)
                {
                    return;
                }
                exBase.Init();
                //FormTestBed1 testBed = new FormTestBed1();
                //testBed.WindowState = FormWindowState.Maximized;
                //testBed.Show();
                //testBed.LoadExample(exAndDesc);
            }
        }
        void DevForm_Load(object sender, EventArgs e)
        {
            //load examples
            Type[] allTypes = this.GetType().Assembly.GetTypes();
            Type exBase = typeof(Mini.DemoBase);
            int j = allTypes.Length;
            List<ExampleAndDesc> exlist = new List<ExampleAndDesc>();
            for (int i = 0; i < j; ++i)
            {
                Type t = allTypes[i];
                if (exBase.IsAssignableFrom(t) && t != exBase)
                {
                    ExampleAndDesc ex = new ExampleAndDesc(t, t.Name);
                    exlist.Add(ex);
                }
            }
            //-------
            exlist.Sort((ex1, ex2) =>
            {
                return ex1.OrderCode.CompareTo(ex2.OrderCode);
            });
            this.listBox1.Items.Clear();
            j = exlist.Count;
            for (int i = 0; i < j; ++i)
            {
                this.listBox1.Items.Add(exlist[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
}
