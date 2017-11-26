//Apache2, 2014-2017, WinterDev
using System;
using System.Windows.Forms;
using PixelFarm.Drawing;

namespace LayoutFarm.UI
{
    public static partial class FormCanvasHelper
    {
        static LayoutFarm.UI.UIPlatformWinForm s_platform;
        static PixelFarm.Drawing.Fonts.IFontLoader s_fontstore;
        static void InitWinform()
        {
            if (s_platform != null) return;
            //----------------------------------------------------
            s_platform = new LayoutFarm.UI.UIPlatformWinForm();
            s_fontstore = new PixelFarm.Drawing.Fonts.OpenFontStore();
        }
        public static Form CreateNewFormCanvas(
            int w, int h,
            InnerViewportKind internalViewportKind,
            out LayoutFarm.UI.UISurfaceViewportControl canvasViewport)
        {
            //1. init
            InitWinform();
            PixelFarm.Drawing.Fonts.IFontLoader fontLoader = s_fontstore;
            //2. 
            PixelFarm.Drawing.IFonts ifont = null;

            switch (internalViewportKind)
            {
                default:
                    ifont = new PixelFarm.Drawing.WinGdi.Gdi32IFonts();
                    break;
                case InnerViewportKind.GL:
                    ifont = new OpenFontIFonts(fontLoader);
                    break;

            }
            //PixelFarm.Drawing.WinGdi.Gdi32IFonts ifonts2 = new PixelFarm.Drawing.WinGdi.Gdi32IFonts();
            PixelFarm.Drawing.WinGdi.WinGdiFontFace.SetFontLoader(fontLoader);
            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(fontLoader);
            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontEncoding(System.Text.Encoding.ASCII);
            //

            //---------------------------------------------------------------------------
            UITimer timer = s_platform.CreateUITimer();
            MyRootGraphic myRootGfx = new MyRootGraphic(
               w, h,
               ifont,
               timer);
            //---------------------------------------------------------------------------

            var innerViewport = canvasViewport = new LayoutFarm.UI.UISurfaceViewportControl();
            Rectangle screenClientAreaRect = Conv.ToRect(Screen.PrimaryScreen.WorkingArea);

            canvasViewport.InitRootGraphics(myRootGfx, myRootGfx.TopWinEventPortal, internalViewportKind);
            canvasViewport.Bounds =
                new System.Drawing.Rectangle(0, 0,
                    screenClientAreaRect.Width,
                    screenClientAreaRect.Height);
            //---------------------- 
            Form form1 = new Form();
            form1.Controls.Add(canvasViewport);
            //----------------------
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
            //----------------------
            return form1;

        }
        public static void MakeFormCanvas(Form form1, LayoutFarm.UI.UISurfaceViewportControl surfaceViewportControl)
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


    }
}