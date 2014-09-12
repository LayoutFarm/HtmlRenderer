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

        static void ShowFormLayoutInspector(UISurfaceViewportControl viewport)
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
          out UISurfaceViewportControl viewport,
          out Form formCanvas)
        {
            LayoutFarm.Presentation.Text.EditableTextFlowLayer.DefaultFontInfo = new TextFontInfo(new Font("tahoma", 10), new BasicGdi32FontHelper());
            formCanvas = FormCanvasHelper.CreateNewFormCanvas(out viewport);
            formCanvas.Text = "FormCanvas 1";

            viewport.PaintMe();

            formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();

        }

        private void cmdShowBasicFormCanvas_Click(object sender, EventArgs e)
        {
            UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            var sampleButton = new LayoutFarm.Presentation.SampleControls.UIButton(30, 30);
            viewport.AddContent(sampleButton);


            viewport.WinRoot.TopDownReCalculateContentSize();
            sampleButton.PrimaryRenderElement.InvalidateGraphic();

            //==================================================  
            viewport.PaintMe();
            //ShowFormLayoutInspector(viewport);


            int count = 0;
            sampleButton.MouseDown += new EventHandler<UIMouseEventArgs>((s, e2) =>
            {
                this.Text = "Click!" + count++;
            });
        }

        private void cmdSampleTextBox_Click(object sender, EventArgs e)
        {

            UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            var textbox = new LayoutFarm.Presentation.SampleControls.UITextBox(200, 30);
            viewport.AddContent(textbox);
            // ShowFormLayoutInspector(viewport);
        }
        private void cmdMultilineTextBox_Click(object sender, EventArgs e)
        {

            UISurfaceViewportControl viewport;
            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            var textbox = new LayoutFarm.Presentation.SampleControls.UIMultiLineTextBox(400, 500, true);
            viewport.AddContent(textbox);
            ShowFormlayoutInspectIfNeed(viewport);
        }

        void LoadHtmlTestView(string filename)
        {

            UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            ShowFormlayoutInspectIfNeed(viewport);

        }
        private void cmdMultiLineTextWithFormat_Click(object sender, EventArgs e)
        {

            UISurfaceViewportControl viewport;
            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);
            var textbox = new LayoutFarm.Presentation.SampleControls.UIMultiLineTextBox(400, 500, true);
            viewport.AddContent(textbox);
            ShowFormlayoutInspectIfNeed(viewport);
        }
        private void cmdTestTextDom_Click(object sender, EventArgs e)
        {




            UISurfaceViewportControl viewport;
            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);
            var textbox = new LayoutFarm.Presentation.SampleControls.UIMultiLineTextBox(400, 500, true);
            viewport.AddContent(textbox);
            ShowFormlayoutInspectIfNeed(viewport);
        }
        void ShowFormlayoutInspectIfNeed(UISurfaceViewportControl viewport)
        {
            if (this.chkShowLayoutInspector.Checked)
            {
                ShowFormLayoutInspector(viewport);
            }
        }
        static TextRunStyle[] CreateRoleSet(TextFontInfo fontInfo, params Color[] colors)
        {
            int j = colors.Length;
            TextRunStyle[] roleSet = new TextRunStyle[j];
            for (int i = 0; i < j; ++i)
            {
                roleSet[i] = CreateSimpleTextRole(fontInfo, colors[i]);
            }
            return roleSet;
        }
        static TextRunStyle CreateSimpleTextRole(TextFontInfo textFontInfo, Color textColor)
        {

            TextRunStyle beh = new TextRunStyle();
            beh.FontColor = textColor;


            return beh;
        }

        private void cmdShowMultipleBox_Click(object sender, EventArgs e)
        {
            UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            for (int i = 0; i < 5; ++i)
            {
                var textbox = new LayoutFarm.Presentation.SampleControls.UIButton(30, 30);
                textbox.SetLocation(i * 40, i * 40); 
          
                viewport.AddContent(textbox);


                viewport.WinRoot.TopDownReCalculateContentSize();

                textbox.PrimaryRenderElement.InvalidateGraphic();

            }

            viewport.PaintMe();
            //ShowFormLayoutInspector(viewport);
        }
    }
}
