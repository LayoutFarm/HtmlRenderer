//Apache2, 2014-2017, WinterDev


using System;
using System.Windows.Forms;

using PixelFarm.Drawing;
using Typography.TextServices;

namespace LayoutFarm.UI
{
    public static partial class FormCanvasHelper
    {
        static LayoutFarm.UI.UIPlatformWinForm s_platform;
        static IFontLoader s_fontstore;
        static void InitWinform()
        {
            if (s_platform != null) return;
            //----------------------------------------------------
            s_platform = new LayoutFarm.UI.UIPlatformWinForm();
            s_fontstore = new OpenFontStore();
        }
        public static Form CreateNewFormCanvas(
            int w, int h,
            InnerViewportKind internalViewportKind,
            out LayoutFarm.UI.UISurfaceViewportControl canvasViewport)
        {
            //1. init
            InitWinform();
            IFontLoader fontLoader = s_fontstore;
            //2. 
            PixelFarm.Drawing.ITextService ifont = null;
            switch (internalViewportKind)
            {
                default:
                    //ifont = PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.GetIFonts();
                    ifont = new OpenFontTextService();
                    break;
                case InnerViewportKind.GL:
                    ifont = new OpenFontTextService();
                    break;

            }

            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(fontLoader);

            //

            //---------------------------------------------------------------------------

            MyRootGraphic myRootGfx = new MyRootGraphic(
               w, h,
               ifont
               );

            //---------------------------------------------------------------------------

            var innerViewport = canvasViewport = new LayoutFarm.UI.UISurfaceViewportControl();
            Rectangle screenClientAreaRect = Conv.ToRect(Screen.PrimaryScreen.WorkingArea);

            canvasViewport.InitRootGraphics(myRootGfx, myRootGfx.TopWinEventPortal, internalViewportKind);
            canvasViewport.Bounds =
                new System.Drawing.Rectangle(10, 10,
                    screenClientAreaRect.Width,
                    screenClientAreaRect.Height);
            //---------------------- 
            Form form1 = new Form();
            //LayoutFarm.Dev.FormNoBorder form1 = new Dev.FormNoBorder();
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

        //
    }



}