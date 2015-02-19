using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutFarm.Ease;

namespace Test5_Ease
{
    public partial class Form1 : Form
    {
        EaseViewport easeViewport;
        public Form1()
        {
            InitializeComponent();

            //1. create viewport
            easeViewport = EaseHost.CreateViewportControl(this, 800, 600);
            //2. add
            this.panel1.Controls.Add(easeViewport.ViewportControl);
            //this.Controls.Add(easeViewport.ViewportControl);
            this.Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            //load sample html text
            easeViewport.Ready();
            string filename = @"..\..\..\HtmlRenderer.Demo\Samples\ClassicSamples\00.Intro.htm";
            //read text file
            var fileContent = System.IO.File.ReadAllText(filename);
            easeViewport.LoadHtml(filename, fileContent);
        }

    }
}
