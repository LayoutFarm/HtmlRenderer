//Apache2, 2014-present, WinterDev

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

        static void ShowFormLayoutInspector(LayoutFarm.UI.GraphicsViewRoot viewroot)
        {
            var formLayoutInspector = new LayoutFarm.Dev.FormLayoutInspector();
            formLayoutInspector.Show();
            formLayoutInspector.FormClosed += (s, e2) =>
            {
                formLayoutInspector = null;
            };
            formLayoutInspector.Connect(viewroot);
        }


        private void cmdShowBasicFormCanvas_Click(object sender, EventArgs e)
        {

            Form formCanvas = FormCanvasHelper.CreateNewFormCanvas(800, 600,
               InnerViewportKind.GdiPlus,
               out LayoutFarm.UI.GraphicsViewRoot viewroot);
            viewroot.PaintToOutputWindow();
            formCanvas.Show();
            ShowFormLayoutInspector(viewroot);
        }

        static AbstractTopWindowBridge GetTopWindowBridge(
          InnerViewportKind innerViewportKind,
          LayoutFarm.RootGraphic rootgfx,
          ITopWindowEventRoot topWindowEventRoot)
        {
            switch (innerViewportKind)
            {
                default: throw new NotSupportedException();
                case InnerViewportKind.GdiPlusOnGLES:
                case InnerViewportKind.AggOnGLES:
                case InnerViewportKind.GLES:
                    return new LayoutFarm.UI.OpenGL.MyTopWindowBridgeOpenGL(rootgfx, topWindowEventRoot);
                case InnerViewportKind.PureAgg:
                    return new LayoutFarm.UI.GdiPlus.MyTopWindowBridgeAgg(rootgfx, topWindowEventRoot); //bridge to agg      
                case InnerViewportKind.GdiPlus:
                    return new LayoutFarm.UI.GdiPlus.MyTopWindowBridgeAgg(rootgfx, topWindowEventRoot); //bridge to agg       
            }
        }


        private void cmdShowEmbededViewport_Click(object sender, EventArgs e)
        {
            Form simpleForm = new Form();
            simpleForm.Text = "SimpleForm2";
            simpleForm.WindowState = FormWindowState.Maximized;
            Rectangle screenClientAreaRect = Screen.PrimaryScreen.WorkingArea;

            MyWinFormsControl actualWinUI = new MyWinFormsControl();
            simpleForm.Controls.Add(actualWinUI);


            InnerViewportKind internalViewportKind = InnerViewportKind.GdiPlus;


            int w = 800;
            int h = 600;
            PixelFarm.Drawing.ITextService textService = new PixelFarm.Drawing.OpenFontTextService();
            MyRootGraphic myRootGfx = new MyRootGraphic(w, h, textService);

            var viewport = new GraphicsViewRoot(screenClientAreaRect.Width, screenClientAreaRect.Height);

            AbstractTopWindowBridge bridge = GetTopWindowBridge(
                internalViewportKind,
                myRootGfx,
                myRootGfx.TopWinEventPortal);

            IGpuOpenGLSurfaceView viewAbstraction = actualWinUI.CreateWindowWrapper(bridge);


            var rootgfx = new MyRootGraphic(
                w, h,
                PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.GetTextService());
            viewport.InitRootGraphics(rootgfx,
                rootgfx.TopWinEventPortal,
                InnerViewportKind.GdiPlus,
                viewAbstraction,
                bridge);

            viewport.PaintToOutputWindow();
            simpleForm.Show();
            ShowFormLayoutInspector(viewport);
        }
    }
}
