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
                if (exBase.IsAssignableFrom(t) && t != exBase && !t.IsAbstract)
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            Graphics g = panel1.CreateGraphics();
            float x0 = 100, y0 = 100, x1 = 250, y1 = 20;
            g.DrawLine(Pens.Red, x0, y0, x1, y1);
            float dy = y1 - y0;
            float dx = x1 - x0;
            float len = 20;
            double rad1 = Math.Atan2(dy, dx);
            double rad2 = Math.Atan2(dy, dx);
            //PixelFarm.VectorMath.Vector2 v2 = new PixelFarm.VectorMath.Vector2(dx, dy);
            //float rad90 = OpenTK.Functions.DegreesToRadians(90);
            //v2.Rotate(rad90);
            //g.DrawLine(Pens.Green, x0, y0, x0 + (float)v2.x, y0 + (float)v2.y);

            float x2 = -(float)(Math.Sin(rad1) * len);
            float y2 = (float)(Math.Cos(rad1) * len);
            float x3 = (float)(Math.Sin(rad1) * len);
            float y3 = -(float)(Math.Cos(rad1) * len);
            g.DrawLine(Pens.Blue, x0, y0, x0 + x2, y0 + y2);
            g.DrawLine(Pens.Orange, x0, y0, x0 + x3, y0 + y3);
            //find normal vector
            //Vector2 n1 = new Vector2(-dy, dx);
            //Vector2 n2 = new Vector2(dy, -dx);
            //n1.Normalize();
            //n2.Normalize();

            ////vec4 delta = vec4(-sin(n_rad) * u_linewidth, cos(n_rad) * u_linewidth, 0, 1);
            //var xx0 = (float)(-Math.Sin(rad1) * 1); var yy0 = (float)Math.Cos(rad1) * 1; 
            //var xx1 = (float)Math.Sin(rad1) * 1; var yy1 = (float)(-Math.Cos(rad1) * 1); 

            //float[] normals = new float[] { n1.X,n1.Y,
            //   n2.X,n2.Y,
            //   n2.X,n2.Y,
            //   //---------
            //   n2.X,n2.Y,
            //   n1.X,n1.Y,
            //   n1.X,n1.Y,
            //};
            //float[] normals = new float[] {
            //   xx0,yy0,
            //   xx1,yy1,
            //   xx1,yy1,
            //   //---------
            //   xx1,yy1,
            //   xx0,yy0,
            //   xx0,yy0,
            //};

        }
    }
}
