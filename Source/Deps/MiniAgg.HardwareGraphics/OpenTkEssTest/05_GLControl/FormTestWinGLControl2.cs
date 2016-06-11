using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace OpenTkEssTest
{
    public partial class FormTestWinGLControl2 : Form
    {
        Button b1;
        public FormTestWinGLControl2()
        {
            InitializeComponent();
            this.myGLControl1.SetBounds(0, 30, 800, 600);
            //------------------------------------------------------
            b1 = new Button();
            this.Controls.Add(b1);
            b1.Text = "Refresh GLControl";
            b1.Size = new Size(200, 30);
            b1.Click += new EventHandler((o, s) => { this.myGLControl1.Refresh(); });
            this.Load += new EventHandler(FormTestWinGLControl_Load);
        }


        void FormTestWinGLControl_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.myGLControl1.ClearColor = new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f);
            if (!this.DesignMode)
            {
                //for 2d 
                this.myGLControl1.InitSetup2d(Screen.PrimaryScreen.Bounds);
            }
        }
        public void SetGLPaintHandler(EventHandler handler)
        {
            this.myGLControl1.SetGLPaintHandler(handler);
        }
    }
}
