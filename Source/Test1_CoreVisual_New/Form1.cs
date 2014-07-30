//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutFarm.Presentation;

namespace TestGraphicPackage
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        static void ShowFormLayoutInspector(ArtSurfaceViewportControl viewport)
        {

            var formLayoutInspector = new LayoutFarm.Presentation.Dev.FormLayoutInspector();
            formLayoutInspector.Show();

            formLayoutInspector.FormClosed += (s, e2) =>
            {
                formLayoutInspector = null;
            };
            formLayoutInspector.Connect(viewport);
            formLayoutInspector.Show();

        }
        private void cmdShowBasicFormCanvas_Click(object sender, EventArgs e)
        {

            FormCanvas formCanvas = new FormCanvas();
            formCanvas.Text = "FormCanvas 1";
            formCanvas.InitViewport();

            var visualRoot = new VisualRootImpl();


            var windowRoot = new ArtVisualWindowImpl(visualRoot, this.Width, this.Height);

            var viewport = formCanvas.SurfaceViewport;
            viewport.SetupWindowRoot(windowRoot);
            viewport.PaintMe();

            formCanvas.Show();

            ShowFormLayoutInspector(viewport);
        }

        private void cmdShowEmbededViewport_Click(object sender, EventArgs e)
        {
            Form simpleForm = new Form();
            simpleForm.Text = "SimpleForm2";
            simpleForm.WindowState = FormWindowState.Maximized;
            Rectangle screenClientAreaRect = Screen.PrimaryScreen.WorkingArea;
            ArtSurfaceViewportControl viewport = new ArtSurfaceViewportControl();
            viewport.Bounds = new Rectangle(0, 0, screenClientAreaRect.Width, screenClientAreaRect.Height);
            simpleForm.Controls.Add(viewport);
            var visualRoot = new VisualRootImpl();

            var windowRoot = new ArtVisualWindowImpl(visualRoot, this.Width, this.Height);


            viewport.SetupWindowRoot(windowRoot);
            viewport.PaintMe();

            simpleForm.Show();

            ShowFormLayoutInspector(viewport);
        }
    }
}
