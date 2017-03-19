//MIT, 2014-2017, WinterDev
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.Ease
{
    public class EaseHostInitReport
    {
        public List<string> reports = new List<string>();
        public void AddErrReport(string msg)
        {
            reports.Add(msg);
        }
        public bool HasSomeError
        {
            get { return reports.Count > 0; }
        }
    }
    public static class EaseHost
    {
        //static GraphicsPlatform gdiPlatform;
        //static UIPlatform uiPlatformWinForm;
        static bool useOpenGL = false;
        static bool isStarted = false;
        static object startLock = new object();
        //-----------------------------------------
        /// <summary>
        /// lib espr file
        /// </summary>
        public static string LibEspr
        {
            get;
            set;
        }
        /// <summary>
        /// load lib espr or not
        /// </summary>
        public static bool LoadLibEspr
        {
            get;
            set;
        }
        /// <summary>
        /// data file for ICU
        /// </summary>
        public static string IcuDataFile
        {
            get;
            set;
        }
        public static EaseHostInitReport Check()
        {
            var initReport = new EaseHostInitReport();
            //report error
            //1. check icu data dir  
            //2. check lib espr  
            return initReport;
        }
        public static void Init()
        {
            //init host system
            //if (LoadLibEspr)
            //{
            //    Espresso.JsBridge.LoadV8(LibEspr);
            //}
        }
        //-----------------------------------------

        public static void StartGraphicsHost()
        {
            lock (startLock)
            {
                if (isStarted) return;

                //var startParams = new LayoutFarm.UI.OpenGL.MyWinGdiPortalSetupParameters();
                //startParams.IcuDataFile = IcuDataFile;
                //gdiPlatform = LayoutFarm.UI.OpenGL.MyOpenGLPortal.Start(startParams);

                //--------------------
                isStarted = true;
                //--------------------
            }
        }

        public static EaseViewport CreateViewportControl(PixelFarm.Forms.Form hostForm, int w, int h)
        {

            LayoutFarm.UI.WinNeutral.UISurfaceViewportControl viewport;
            CreateNewFormCanvas(w, h, hostForm,
                 useOpenGL ? InnerViewportKind.GL : InnerViewportKind.GdiPlus,
                 out viewport);
            viewport.PaintMe();
            EaseViewport easeViewport = new EaseViewport(viewport);
            return easeViewport;
        }

        static void CreateReadyForm(
           out LayoutFarm.UI.WinNeutral.UISurfaceViewportControl viewport,
           out PixelFarm.Forms.Form formCanvas)
        {
            int w = 800;
            int h = 600;
            var rootgfx = new MyRootGraphic(
                LayoutFarm.UI.UIPlatformWinNeutral.platform,
                LayoutFarm.UI.UIPlatformWinNeutral.platform.GetIFonts(),
                w, h);
            var topRenderBox = rootgfx.TopWindowRenderBox;
            formCanvas = FormCanvasHelper.CreateNewFormCanvas(rootgfx,
                useOpenGL ? InnerViewportKind.GL : InnerViewportKind.GdiPlus,
                out viewport);
            //formCanvas.Text = "FormCanvas 1";
            viewport.PaintMe();
            //formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();
        }

        static void CreateNewFormCanvas(
          int w, int h,
          PixelFarm.Forms.Form form1,
          InnerViewportKind internalViewportKind,
          out LayoutFarm.UI.WinNeutral.UISurfaceViewportControl canvasViewport)
        {

            var rootgfx = new MyRootGraphic(
                LayoutFarm.UI.UIPlatformWinNeutral.platform,
                LayoutFarm.UI.UIPlatformWinNeutral.platform.GetIFonts(),
                w, h);
            var innerViewport = canvasViewport = new LayoutFarm.UI.WinNeutral.UISurfaceViewportControl();
            //temp fix
            Rectangle screenClientAreaRect = new Rectangle(0, 0, 800, 600); // Conv.ToRect(Screen.PrimaryScreen.WorkingArea);
            canvasViewport.InitRootGraphics(rootgfx, rootgfx.TopWinEventPortal, internalViewportKind);
            canvasViewport.Bounds =
                new Rectangle(0, 0,
                    screenClientAreaRect.Width,
                    screenClientAreaRect.Height);
            ////---------------------- 
            //form1.Controls.Add(canvasViewport);
            ////----------------------
            MakeFormCanvas(form1, canvasViewport);
            //form1.SizeChanged += (s, e) =>
            //{
            //    if (form1.WindowState == FormWindowState.Maximized)
            //    {
            //        Screen currentScreen = GetScreenFromX(form1.Left);
            //        //make full screen ?
            //        if (innerViewport != null)
            //        {
            //            innerViewport.Size = currentScreen.WorkingArea.Size;
            //        }
            //    }
            //};
        }
        static void MakeFormCanvas(PixelFarm.Forms.Form form1, LayoutFarm.UI.WinNeutral.UISurfaceViewportControl surfaceViewportControl)
        {
            //form1.FormClosing += (s, e) =>
            //{
            //    surfaceViewportControl.Close();
            //};
        }

        //static Screen GetScreenFromX(int xpos)
        //{
        //    Screen[] allScreens = Screen.AllScreens;
        //    int j = allScreens.Length;
        //    int accX = 0;
        //    for (int i = 0; i < j; ++i)
        //    {
        //        Screen sc1 = allScreens[i];
        //        if (accX + sc1.WorkingArea.Width > xpos)
        //        {
        //            return sc1;
        //        }
        //    }
        //    return Screen.PrimaryScreen;
        //}

        //public static Canvas CreatePrintCanvas(System.Drawing.Graphics g, int w, int h)
        //{
        //    CanvasInitParameters canvasInit = new CanvasInitParameters();
        //    canvasInit.externalCanvas = g;
        //    return gdiPlatform.CreateCanvas(0, 0, w, h, canvasInit);
        //}
    }
}
