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
            LayoutFarm.Text.EditableTextFlowLayer.DefaultFontInfo = new TextFontInfo(new Font("tahoma", 10), new BasicGdi32FontHelper());
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

            var sampleButton = new LayoutFarm.SampleControls.UIButton(30, 30);
            viewport.AddContent(sampleButton);
            viewport.WinTop.TopDownReCalculateContentSize();
            sampleButton.InvalidateGraphic();
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

            var textbox = new LayoutFarm.SampleControls.UITextBox(200, 30);
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

            var textbox = new LayoutFarm.SampleControls.UIMultiLineTextBox(400, 500, true);
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
            var textbox = new LayoutFarm.SampleControls.UIMultiLineTextBox(400, 500, true);
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
            var textbox = new LayoutFarm.SampleControls.UIMultiLineTextBox(400, 500, true);
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
        //static TextRunStyle[] CreateRoleSet(TextFontInfo fontInfo, params Color[] colors)
        //{
        //    int j = colors.Length;
        //    TextRunStyle[] roleSet = new TextRunStyle[j];
        //    for (int i = 0; i < j; ++i)
        //    {
        //        roleSet[i] = CreateSimpleTextRole(fontInfo, colors[i]);
        //    }
        //    return roleSet;
        //}
        //static TextRunStyle CreateSimpleTextRole(TextFontInfo textFontInfo, Color textColor)
        //{

        //    TextRunStyle beh = new TextRunStyle();
        //    beh.FontColor = textColor;


        //    return beh;
        //}

        private void cmdShowMultipleBox_Click(object sender, EventArgs e)
        {
            UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            for (int i = 0; i < 5; ++i)
            {
                var textbox = new LayoutFarm.SampleControls.UIButton(30, 30);
                textbox.SetLocation(i * 40, i * 40);

                viewport.AddContent(textbox);
                viewport.WinTop.TopDownReCalculateContentSize();
                textbox.InvalidateGraphic();
            }

            viewport.PaintMe();
            //ShowFormLayoutInspector(viewport);
        }

        private void cmdSampleGridBox_Click(object sender, EventArgs e)
        {
            UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            var gridBox = new LayoutFarm.SampleControls.UIGridBox(100, 100);
            gridBox.SetLocation(50, 50);

            viewport.AddContent(gridBox);


            viewport.WinTop.TopDownReCalculateContentSize();
            gridBox.InvalidateGraphic();

            //==================================================  
            viewport.PaintMe();
            //ShowFormLayoutInspector(viewport);

        }

        private void cmdSampleScrollbar_Click(object sender, EventArgs e)
        {
            UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            var scbar = new LayoutFarm.SampleControls.UIScrollBar(15, 200);
            scbar.SetLocation(10, 10);
            viewport.AddContent(scbar);
            viewport.WinTop.TopDownReCalculateContentSize();
            scbar.InvalidateGraphic();

            viewport.PaintMe();
        }

        private void cmdDragSample_Click(object sender, EventArgs e)
        {
            UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            {
                var box1 = new LayoutFarm.SampleControls.UIButton(50, 50);
                box1.BackColor = Color.Red;
                box1.SetLocation(10, 10);
                SetupActiveBoxProperties(box1);
                viewport.AddContent(box1);
                box1.InvalidateGraphic();
            }
            //--------------------------------
            {
                var box2 = new LayoutFarm.SampleControls.UIButton(30, 30);
                box2.SetLocation(50, 50);
                SetupActiveBoxProperties(box2);
                viewport.AddContent(box2);
                box2.InvalidateGraphic();
            }
            //--------------------------------
            viewport.WinTop.TopDownReCalculateContentSize();

            //---------------------------------------------------------

            viewport.PaintMe();
        }



        void SetupActiveBoxProperties(LayoutFarm.SampleControls.UIButton box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = Color.DeepSkyBlue;
                box.InvalidateGraphic();
            };
            
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                box.BackColor = Color.LightGray;
                box.InvalidateGraphic();
            };
            //3. drag
            box.Dragging += (s, e) =>
            {
                box.BackColor = Color.GreenYellow; 
                Point pos= box.Position;
                box.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
                
                box.InvalidateGraphic();     
            };
            box.DragStop += (s, e) =>
            {
                box.BackColor = Color.LightGray;
                box.InvalidateGraphic();
            };

        }
    }
}
