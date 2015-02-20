// 2015,2014 ,MIT, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;


using LayoutFarm;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.Ease
{
    public static class EaseHost
    {
        static readonly PixelFarm.Drawing.GraphicsPlatform gdiPlatform = LayoutFarm.UI.GdiPlus.MyWinGdiPortal.Start();
        static readonly PixelFarm.Drawing.GraphicsPlatform openGLPlatform = LayoutFarm.UI.OpenGL.MyOpenGLPortal.Start();

        static UIPlatform uiPlatformWinForm;

        static bool useOpenGL = false;
        public static void StartHost()
        {
            var platform = LayoutFarm.UI.GdiPlus.MyWinGdiPortal.Start();
            LayoutFarm.Text.TextEditRenderBox.DefaultFontInfo = platform.GetFont("tahoma", 10, PixelFarm.Drawing.FontStyle.Regular);


            uiPlatformWinForm = new LayoutFarm.UI.UIPlatformWinForm();

        }

        public static EaseViewport CreateViewportControl(Form hostForm, int w, int h)
        {

            MyRootGraphic rootgfx = new MyRootGraphic(uiPlatformWinForm,
                useOpenGL ? openGLPlatform : gdiPlatform,
                w, h);
            TopWindowRenderBox topRenderBox = rootgfx.TopWindowRenderBox;
            LayoutFarm.UI.UISurfaceViewportControl viewport;
            CreateNewFormCanvas(hostForm, rootgfx,
                 useOpenGL ? InnerViewportKind.GL : InnerViewportKind.GdiPlus,
                 out viewport);


            viewport.PaintMe();

            EaseViewport easeViewport = new EaseViewport(viewport);

            return easeViewport;
        }

        static void CreateReadyForm(
           out LayoutFarm.UI.UISurfaceViewportControl viewport,
           out Form formCanvas)
        {

            int w = 800;
            int h = 600;

            MyRootGraphic rootgfx = new MyRootGraphic(uiPlatformWinForm,
                useOpenGL ? openGLPlatform : gdiPlatform,
                w, h);

            TopWindowRenderBox topRenderBox = rootgfx.TopWindowRenderBox;

            formCanvas = FormCanvasHelper.CreateNewFormCanvas(rootgfx,
                useOpenGL ? InnerViewportKind.GL : InnerViewportKind.GdiPlus,
                out viewport);

            formCanvas.Text = "FormCanvas 1";

            viewport.PaintMe();

            formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();
        }

        static void CreateNewFormCanvas(
          Form form1,
          MyRootGraphic myRootGfx,
          InnerViewportKind internalViewportKind,
          out LayoutFarm.UI.UISurfaceViewportControl canvasViewport)
        {

            var innerViewport = canvasViewport = new LayoutFarm.UI.UISurfaceViewportControl();
            Rectangle screenClientAreaRect = Conv.ToRect(Screen.PrimaryScreen.WorkingArea);

            canvasViewport.InitRootGraphics(myRootGfx, myRootGfx.UserInputEventAdapter, internalViewportKind);
            canvasViewport.Bounds =
                new System.Drawing.Rectangle(0, 0,
                    screenClientAreaRect.Width,
                    screenClientAreaRect.Height);
            ////---------------------- 
            //form1.Controls.Add(canvasViewport);
            ////----------------------
            MakeFormCanvas(form1, canvasViewport);
            form1.SizeChanged += (s, e) =>
            {
                if (form1.WindowState == FormWindowState.Maximized)
                {
                    Screen currentScreen = GetScreenFromX(form1.Left);
                    //make full screen ?
                    if (innerViewport != null)
                    {
                        innerViewport.Size = currentScreen.WorkingArea.Size;
                    }
                }
            };

        }

        static void MakeFormCanvas(Form form1, LayoutFarm.UI.UISurfaceViewportControl surfaceViewportControl)
        {
            form1.FormClosing += (s, e) =>
            {
                surfaceViewportControl.Close();
            };

        }

        static Screen GetScreenFromX(int xpos)
        {
            Screen[] allScreens = Screen.AllScreens;
            int j = allScreens.Length;
            int accX = 0;
            for (int i = 0; i < j; ++i)
            {
                Screen sc1 = allScreens[i];
                if (accX + sc1.WorkingArea.Width > xpos)
                {
                    return sc1;
                }
            }
            return Screen.PrimaryScreen;
        }

        public static EaseCanvas CreatePrintCanvas(System.Drawing.Graphics g, int w, int h)
        {
            return new EaseCanvas(gdiPlatform.CreateCanvas(g, 0, 0, w, h));
        }
    }
}
