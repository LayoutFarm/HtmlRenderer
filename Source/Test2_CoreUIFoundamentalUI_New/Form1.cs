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


            LayoutFarm.Presentation.Text.EditableTextFlowLayer.DefaultFontInfo = new TextFontInfo(new Font("tahoma", 10), new BasicGdi32FontHelper());
            formCanvas = FormCanvasHelper.CreateNewFormCanvas(out viewport);
            formCanvas.Text = "FormCanvas 1";                        
                         
            viewport.PaintMe();

            formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();
        
        }

        private void cmdShowBasicFormCanvas_Click(object sender, EventArgs e)
        {

        }

        private void cmdSampleTextBox_Click(object sender, EventArgs e)
        {
            
            ArtSurfaceViewportControl viewport;
            
            Form formCanvas;
            CreateReadyForm( 
                out viewport, 
                out formCanvas);

            var textbox = new LayoutFarm.Presentation.SampleControls.ArtUITextBox(200, 30);
            viewport.AddContent(textbox.PrimaryVisualElement);
            ShowFormLayoutInspector(viewport);
        }
        private void cmdMultilineTextBox_Click(object sender, EventArgs e)
        {
          
            ArtSurfaceViewportControl viewport; 
            Form formCanvas;
            CreateReadyForm( 
                out viewport, 
                out formCanvas);

            var textbox = new LayoutFarm.Presentation.SampleControls.ArtUIMultiLineTextBox(400, 500, true);
            viewport.AddContent(textbox.PrimaryVisualElement);
            ShowFormlayoutInspectIfNeed(viewport);
        }
        private void cmdHtmlView_Click(object sender, EventArgs e)
        {


        }
        void LoadHtmlTestView(string filename)
        {
          
            ArtSurfaceViewportControl viewport;
        
            Form formCanvas;
            CreateReadyForm( 
                out viewport,
                out formCanvas); 

            ShowFormlayoutInspectIfNeed(viewport);



        }
        private void cmdMultiLineTextWithFormat_Click(object sender, EventArgs e)
        {
           
            ArtSurfaceViewportControl viewport; 
            Form formCanvas;
            CreateReadyForm( 
                out viewport, 
                out formCanvas);
            var textbox = new LayoutFarm.Presentation.SampleControls.ArtUIMultiLineTextBox(400, 500, true);
            viewport.AddContent(textbox.PrimaryVisualElement);
            ShowFormlayoutInspectIfNeed(viewport);
        }
        private void cmdTestTextDom_Click(object sender, EventArgs e)
        {

            string value = "OKOKOK";

           
            ArtSurfaceViewportControl viewport; 
            Form formCanvas;
            CreateReadyForm( 
                out viewport, 
                out formCanvas);
            var textbox = new LayoutFarm.Presentation.SampleControls.ArtUIMultiLineTextBox(400, 500, true);
            viewport.AddContent(textbox.PrimaryVisualElement);
            ShowFormlayoutInspectIfNeed(viewport);
        }
        void ShowFormlayoutInspectIfNeed(ArtSurfaceViewportControl viewport)
        {
            if (this.chkShowLayoutInspector.Checked)
            {
                ShowFormLayoutInspector(viewport);
            }
        }
        static BoxStyle[] CreateRoleSet(TextFontInfo fontInfo, params Color[] colors)
        {
            int j = colors.Length;
            BoxStyle[] roleSet = new BoxStyle[j];
            for (int i = 0; i < j; ++i)
            {
                roleSet[i] = CreateSimpleTextRole(fontInfo, colors[i]);
            }
            return roleSet;
        }
        static BoxStyle CreateSimpleTextRole(TextFontInfo textFontInfo, Color textColor)
        {

            BoxStyle beh = new BoxStyle();
            beh.FontColor = textColor;


            return beh;
        }

        private void cmdShowMultipleBox_Click(object sender, EventArgs e)
        {






        }

        private void cmdHtmlViewNew_Click(object sender, EventArgs e)
        {





        }


    }
}
