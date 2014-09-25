using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutFarm;


namespace TestGraphicPackage2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        static void ShowFormLayoutInspector(UISurfaceViewportControl viewport)
        {

            var formLayoutInspector = new LayoutFarm.Dev.FormLayoutInspector();
            formLayoutInspector.Show();

            formLayoutInspector.FormClosed += (s, e2) =>
            {
                formLayoutInspector = null;
            };
            formLayoutInspector.Connect(viewport);
            formLayoutInspector.Show();

        }
        static void CreateReadyForm(
          out UISurfaceViewportControl viewport,
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
            UISurfaceViewportControl viewport;
            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            //==================================================
            //html box
            UIHtmlBox htmlBox = new UIHtmlBox(800, 600);

            viewport.AddContent(htmlBox);
            string html = @"<html><head></head><body><div>OK1</div><div>OK2</div></body></html>";
            htmlBox.LoadHtmlText(html);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LayoutFarm.Text.EditableTextFlowLayer.DefaultFontInfo =
                LayoutFarm.Drawing.CurrentGraphicPlatform.CreateTexFontInfo(
                    new System.Drawing.Font("tahoma", 10));


            UISurfaceViewportControl viewport;
            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);
            ////==================================================
            //html box
            UIHtmlBox htmlBox = new UIHtmlBox(800, 400);
            viewport.AddContent(htmlBox);
            string html = @"<html><head></head><body><div>OK1</div><div>OK2</div></body></html>";
            htmlBox.LoadHtmlText(html);
            //================================================== 

            //textbox
            var textbox = new LayoutFarm.SampleControls.UITextBox(400, 100, true);
            textbox.SetLocation(0, 200);
            viewport.AddContent(textbox);
            textbox.Focus();
        }


    }
}
