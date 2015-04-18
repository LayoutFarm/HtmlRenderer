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
        static bool isStarted = false;
        static object startLock = new object();
        public static void StartGraphicsHost()
        {
            lock (startLock)
            {
                if (isStarted) return;

                var platform = LayoutFarm.UI.GdiPlus.MyWinGdiPortal.Start();
                uiPlatformWinForm = new LayoutFarm.UI.UIPlatformWinForm();


                //--------------------
                isStarted = true;
                //--------------------
            }
        }

        public static EaseViewport CreateViewportControl(Form hostForm, int w, int h)
        {


            var rootgfx = new MyRootGraphic(uiPlatformWinForm,
                useOpenGL ? openGLPlatform : gdiPlatform,
                w, h);

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

            var rootgfx = new MyRootGraphic(uiPlatformWinForm,
                useOpenGL ? openGLPlatform : gdiPlatform,
                w, h);

            var topRenderBox = rootgfx.TopWindowRenderBox;


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

            canvasViewport.InitRootGraphics(myRootGfx, myRootGfx.TopWinEventPortal, internalViewportKind);
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

        public static Canvas CreatePrintCanvas(System.Drawing.Graphics g, int w, int h)
        {
            return gdiPlatform.CreateCanvas(g, 0, 0, w, h);
        }
    }
}
