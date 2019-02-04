
//Apache2, 2014-present, WinterDev
//#define GL_ENABLE
using System;
using PixelFarm;
using PixelFarm.Forms;
using Typography.FontManagement;
using LayoutFarm.UI;
using LayoutFarm.UI.WinNeutral;

namespace YourImplementation
{
    //TODO: review this with TestGLES_GLFW_Neutral

    class MyGLFWForm : GlFwForm
    {
        public MyGLFWForm(int w, int h, string title)
            : base(w, h, title)
        {
        }
    }

    abstract class GlfwAppBase
    {
        public abstract void UpdateViewContent(PaintEventArgs formRenderUpdateEventArgs);
    }


    class MyApp : GlfwAppBase
    {
        LayoutFarm.App _app;
        //static GLDemoContext _demoContext = null;
        static InstalledTypefaceCollection s_typefaceStore;
        static LayoutFarm.OpenFontTextService s_textServices;

        UISurfaceViewportControl _surfaceViewport;
        public MyApp(LayoutFarm.App app = null)
        {
            s_typefaceStore = new InstalledTypefaceCollection();
            s_textServices = new LayoutFarm.OpenFontTextService();

            _app = app;
        }


        public void CreateMainForm()
        {
            int w = 800, h = 600;
            MyGLFWForm form1 = new MyGLFWForm(w, h, "PixelFarm on GLfw and GLES2");
            MyRootGraphic myRootGfx = new MyRootGraphic(w, h, s_textServices);
            var canvasViewport = new UISurfaceViewportControl();
            canvasViewport.InitRootGraphics(myRootGfx, myRootGfx.TopWinEventPortal, InnerViewportKind.GLES);
            canvasViewport.SetBounds(0, 0, w, h);
            form1.Controls.Add(canvasViewport);


            _surfaceViewport = canvasViewport;
            LayoutFarm.AppHostNeutral appHost = new LayoutFarm.AppHostNeutral(canvasViewport);

            //demoContext2.LoadDemo(new T45_TextureWrap());
            //demoContext2.LoadDemo(new T48_MultiTexture());
            //demoContext2.LoadDemo(new T107_1_DrawImages()); 
            //_demoBase = new T108_LionFill();//new T45_TextureWrap(),T48_MultiTexture()
            //_demoBase = new T110_DrawText();
            //_demoBase = new T107_1_DrawImages();

            //_demoContext = new GLDemoContext(w, h);
            //_demoContext.SetTextPrinter(painter =>
            //{

            //    var printer = new PixelFarm.DrawingGL.GLBitmapGlyphTextPrinter(painter, s_textServices);
            //    painter.TextPrinter = printer;
            //    //create text printer for opengl 
            //    //----------------------
            //    //1. win gdi based
            //    //var printer = new WinGdiFontPrinter(canvas2d, w, h);
            //    //canvasPainter.TextPrinter = printer;
            //    //----------------------
            //    //2. raw vxs
            //    //var printer = new PixelFarm.Drawing.Fonts.VxsTextPrinter(canvasPainter);
            //    //canvasPainter.TextPrinter = printer;
            //    //----------------------
            //    //3. agg texture based font texture
            //    //var printer = new AggFontPrinter(canvasPainter, w, h);
            //    //canvasPainter.TextPrinter = printer;
            //    //----------------------
            //    //4. texture atlas based font texture 
            //    //------------
            //    //resolve request font 
            //    //var printer = new GLBmpGlyphTextPrinter(canvasPainter, YourImplementation.BootStrapWinGdi.myFontLoader);
            //    //canvasPainter.TextPrinter = printer;

            //});


            //form1.SetDrawFrameDelegate(e => _demoContext.Render());
            form1.SetDrawFrameDelegate(e =>
            {
                _surfaceViewport.PaintMeFullMode();
            });
            if (_app != null)
            {
                // _demoContext.LoadApp(_app);
                appHost.StartApp(_app);//start app
                canvasViewport.TopDownRecalculateContent();
                canvasViewport.PaintMe();
            }

            //_demoContext.LoadDemo(_demoBase);
        }
        public override void UpdateViewContent(PaintEventArgs formRenderUpdateEventArgs)
        {
            _surfaceViewport.PaintMeFullMode();
        }
    }


    class GLFWProgram
    {

        class LocalFileStorageProvider : PixelFarm.Platforms.StorageServiceProvider
        {
            public override bool DataExists(string dataName)
            {
                //implement with file
                return System.IO.File.Exists(dataName);
            }
            public override byte[] ReadData(string dataName)
            {
                return System.IO.File.ReadAllBytes(dataName);
            }
            public override void SaveData(string dataName, byte[] content)
            {
                System.IO.File.WriteAllBytes(dataName, content);
            }

        }


        static LocalFileStorageProvider s_LocalStorageProvider = new LocalFileStorageProvider();
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
                myApp.CreateMainForm();

            }
            GlfwApp.RunMainLoop();
        }
    }


}