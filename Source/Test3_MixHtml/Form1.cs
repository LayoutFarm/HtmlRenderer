using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutFarm.Presentation;


namespace TestGraphicPackage2
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
        static void CreateReadyForm(
          out ArtSurfaceViewportControl viewport,
          out Form formCanvas)
        {
            
            formCanvas = FormCanvasHelper.CreateNewFormCanvas(out viewport);
            formCanvas.Text = "FormCanvas 1";

            viewport.PaintMe();

            formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();

        }

        private void cmdMixHtml_Click(object sender, EventArgs e)
        {
            ArtSurfaceViewportControl viewport;
            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);
        }

      
    }
}
