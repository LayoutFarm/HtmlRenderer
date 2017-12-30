//Apache2, 2014-2017, WinterDev

using System;
using System.Drawing;
using System.Windows.Forms;
using LayoutFarm.UI;
namespace TestGraphicPackage
{
    public partial class Form1 : Form
    {
        //UIPlatform uiPlatformWinForm;
        //PixelFarm.Drawing.GraphicsPlatform gfxPlatform;
        public Form1()
        {
            InitializeComponent();
            //this.uiPlatformWinForm = new LayoutFarm.UI.UIPlatformWinForm();
            //LayoutFarm.UI.UIPlatform.CurrentUIPlatform = uiPlatformWinForm;
            //this.gfxPlatform = p;
        }

        static void ShowFormLayoutInspector(LayoutFarm.UI.UISurfaceViewportControl viewport)
        {
            var formLayoutInspector = new LayoutFarm.Dev.FormLayoutInspector();
            formLayoutInspector.Show();
            formLayoutInspector.FormClosed += (s, e2) =>
            {
                formLayoutInspector = null;
            };
            formLayoutInspector.Connect(viewport);
        }


        private void cmdShowBasicFormCanvas_Click(object sender, EventArgs e)
        {
            LayoutFarm.UI.UISurfaceViewportControl viewport;

            Form formCanvas = FormCanvasHelper.CreateNewFormCanvas(800, 600,
               InnerViewportKind.GdiPlus,
               out viewport);
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
            var viewport = new LayoutFarm.UI.UISurfaceViewportControl();
            viewport.Bounds = new Rectangle(0, 0, screenClientAreaRect.Width, screenClientAreaRect.Height);
            simpleForm.Controls.Add(viewport);
            int w = 800;
            int h = 600;
            var ifont = PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.GetIFonts();
            var rootgfx = new MyRootGraphic(
                w, h,
                ifont);
            viewport.InitRootGraphics(rootgfx, rootgfx.TopWinEventPortal,
                InnerViewportKind.GdiPlus);
            viewport.PaintMe();
            simpleForm.Show();
            ShowFormLayoutInspector(viewport);
        }
    }
}
