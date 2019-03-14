
//Apache2, 2014-present, WinterDev

using System;
using PixelFarm;
using PixelFarm.Forms;
using Typography.FontManagement;
using LayoutFarm.UI;
using LayoutFarm.UI.WinNeutral;

namespace YourImplementation
{
    //TODO: review this with TestGLES_GLFW_Neutral


    abstract class GlfwAppBase
    {
        public abstract void UpdateViewContent(PaintEventArgs formRenderUpdateEventArgs);
    }


    class MyApp : GlfwAppBase
    {
        LayoutFarm.App _app;

        static InstalledTypefaceCollection s_typefaceStore;
        static LayoutFarm.OpenFontTextService s_textServices;

        UISurfaceViewportControl _surfaceViewport;
        public MyApp(LayoutFarm.App app = null)
        {
            if (s_typefaceStore == null)
            {
                s_typefaceStore = new InstalledTypefaceCollection();
                s_textServices = new LayoutFarm.OpenFontTextService();
            }

            _app = app;
        }
        public void CreateMainForm(int w, int h)
        {

            GlFwForm form1 = new GlFwForm(w, h, "PixelFarm on GLfw and GLES2");
            MyRootGraphic myRootGfx = new MyRootGraphic(w, h, s_textServices);
            var canvasViewport = new UISurfaceViewportControl();
            canvasViewport.InitRootGraphics(myRootGfx, myRootGfx.TopWinEventPortal, InnerViewportKind.GLES);
            canvasViewport.SetBounds(0, 0, w, h);
            form1.Controls.Add(canvasViewport);


            _surfaceViewport = canvasViewport;
            LayoutFarm.AppHostNeutral appHost = new LayoutFarm.AppHostNeutral(canvasViewport);
            form1.SetDrawFrameDelegate(e =>
            {
                _surfaceViewport.PaintMeFullMode();
            });
            if (_app != null)
            {
                appHost.StartApp(_app);//start app
                canvasViewport.TopDownRecalculateContent();
                canvasViewport.PaintMe();
            }
        }
        public override void UpdateViewContent(PaintEventArgs formRenderUpdateEventArgs)
        {
            _surfaceViewport.PaintMeFullMode();
        }
    }


    class GLFWProgram
    {

        
        static LocalFileStorageProvider s_LocalStorageProvider = new LocalFileStorageProvider("");
        public static void Start()
        {

            PixelFarm.Platforms.StorageService.RegisterProvider(s_LocalStorageProvider);
            //---------------------------------------------------
            PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new ImgCodecMemBitmapIO();
            //PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new PixelFarm.Drawing.WinGdi.GdiBitmapIO();

            if (!GLFWPlatforms.Init())
            {
                System.Diagnostics.Debug.WriteLine("can't init glfw");
                return;
            }

            bool useMyGLFWForm = true;
            if (!useMyGLFWForm)
            {
                GlFwForm form1 = new GlFwForm(800, 600, "PixelFarm on GLfw and GLES2");
                MyApp glfwApp = new MyApp();
                form1.SetDrawFrameDelegate(e => glfwApp.UpdateViewContent(e));
            }
            else
            {
                var myApp = new MyApp();
                myApp.CreateMainForm(800, 600);

            }
            GlfwApp.RunMainLoop();
        }
    }


}