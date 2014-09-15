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


            htmlBox.InvalidateGraphic();

            //================================================== 



            viewport.PaintMe();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LayoutFarm.Text.EditableTextFlowLayer.DefaultFontInfo = new TextFontInfo(new Font("tahoma", 10), new BasicGdi32FontHelper());


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
            //ArtVisualHtmlBox.DirectSetVisualElementLocation(innerHtmlBox, 100, 100);
            htmlBox.LoadHtmlText(html);

            htmlBox.InvalidateGraphic();

            //================================================== 

            //textbox
            var textbox = new LayoutFarm.SampleControls.UIMultiLineTextBox(400, 100, true);
            var renderTextBox = textbox.GetPrimaryRenderElement(viewport.WinTop.RootGraphic);
            viewport.AddContent(textbox);

            //var vinv2 = visualTextBox.WinRoot.GetVInv(); 
            textbox.InvalidateGraphic();
            //visualTextBox.WinRoot.FreeVInv();

            RenderElement.DirectSetVisualElementLocation(renderTextBox, 0, 200);
            //vinv2 = visualTextBox.WinRoot.GetVInv();
            textbox.InvalidateGraphic();


            viewport.WinTop.CurrentKeyboardFocusedElement = renderTextBox;
            viewport.PaintMe();
        }


    }
}
