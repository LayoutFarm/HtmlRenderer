//MIT, 2016-2017, WinterDev
using System;
using Pencil.Gaming;
using PixelFarm;
using PixelFarm.Drawing;
using PixelFarm.DrawingGL;

using PixelFarm.Forms;
using OpenTK.Graphics.ES20;
using SkiaSharp;

using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
using LayoutFarm.HtmlBoxes;
using LayoutFarm;

namespace TestGlfw
{

    class GLFWProgram
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Start();
        }


        static bool needUpdateContent = false;
        static MyNativeRGBA32BitsImage myImg;
        static GLBitmap glBmp;

        static void UpdateViewContent(FormRenderUpdateEventArgs formRenderUpdateEventArgs)
        {

            needUpdateContent = false;
            //1. create platform bitmap 
            // create the surface
            int w = 800;
            int h = 600;
            if (myImg == null)
            {
                myImg = new TestGlfw.MyNativeRGBA32BitsImage(w, h);
            }


            int testNo = 2;
            if (testNo == 0)
            {
                //test1
                // create the surface
                var info = new SKImageInfo(w, h, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                using (var surface = SKSurface.Create(info, myImg.Scan0, myImg.Stride))
                {
                    // start drawing
                    SKCanvas canvas = surface.Canvas;
                    DrawWithSkia(canvas);
                    surface.Canvas.Flush();
                }
                glBmp = new PixelFarm.DrawingGL.GLBitmap(w, h, myImg.Scan0);
            }
            else
            {
                ////---------------------------------------------------------------------------------------
                //test2
                var lionShape = new PixelFarm.Agg.SpriteShape();
                lionShape.ParseLion();
                var lionBounds = lionShape.Bounds;
                //-------------
                var aggImage = new PixelFarm.Agg.ActualImage((int)lionBounds.Width, (int)lionBounds.Height, PixelFarm.Agg.PixelFormat.ARGB32);
                var imgGfx2d = new PixelFarm.Agg.ImageGraphics2D(aggImage);
                var aggPainter = new PixelFarm.Agg.AggCanvasPainter(imgGfx2d);
                DrawLion(aggPainter, lionShape, lionShape.Path.Vxs);
                //-------------



                //convert affImage to texture 
                glBmp = LoadTexture(aggImage);
            }
        }
        static PixelFarm.DrawingGL.GLBitmap LoadTexture(PixelFarm.Agg.ActualImage actualImg)
        {
            return new PixelFarm.DrawingGL.GLBitmap(actualImg.Width,
                actualImg.Height,
                PixelFarm.Agg.ActualImage.GetBuffer(actualImg), false);
        }
        static void DrawLion(PixelFarm.Agg.CanvasPainter p, PixelFarm.Agg.SpriteShape shape, PixelFarm.Agg.VertexStore myvxs)
        {
            int j = shape.NumPaths;
            int[] pathList = shape.PathIndexList;
            Color[] colors = shape.Colors;
            for (int i = 0; i < j; ++i)
            {
                p.FillColor = colors[i];
                p.Fill(new PixelFarm.Agg.VertexStoreSnap(myvxs, pathList[i]));
            }
        }
        static void DrawWithSkia(SKCanvas canvas)
        {
            canvas.Clear(new SKColor(255, 255, 255, 255));
            using (SKPaint p = new SKPaint())
            {
                p.TextSize = 36.0f;
                p.Color = (SKColor)0xFF4281A4;
                p.StrokeWidth = 2;
                p.IsAntialias = true;
                canvas.DrawLine(0, 0, 100, 100, p);
                p.Color = SKColors.Red;
                canvas.DrawText("Hello!", 20, 100, p);
            }
        }

        static PixelFarm.DrawingGL.CanvasGL2d canvasGL2d;
        public static void Start()
        {

            if (!Glfw.Init())
            {
                Console.WriteLine("can't init glfw");
                return;
            }
            //---------------------------------------------------
            //specific OpenGLES ***
            Glfw.WindowHint(WindowHint.GLFW_CLIENT_API, (int)OpenGLAPI.OpenGLESAPI);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_CREATION_API, (int)OpenGLContextCreationAPI.GLFW_EGL_CONTEXT_API);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MAJOR, 2);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MINOR, 0);
            //---------------------------------------------------


            Glfw.SwapInterval(1);
            GlFwForm form1 = GlfwApp.CreateGlfwForm(
                800,
                600,
                "PixelFarm + Skia on GLfw and OpenGLES2");
            form1.MakeCurrent();
            //------------------------------------
            //***
            GLFWPlatforms.CreateGLESContext();
            //------------------------------------
            form1.Activate();

            int ww_w = 800;
            int ww_h = 600;
            int max = Math.Max(ww_w, ww_h);
            canvasGL2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(max, max);

            //------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //--------------------------------------------------------------------------------
            //setup viewport size
            //set up canvas
            needUpdateContent = true;

            //GL.Viewport(0, 0, 800, 600);
            GL.Viewport(0, 0, max, max);

            FormRenderUpdateEventArgs formRenderUpdateEventArgs = new FormRenderUpdateEventArgs();
            formRenderUpdateEventArgs.form = form1;

            LayoutFarm.Ease.EaseHost.StartGraphicsHost();

            var rootgfx = new MyRootGraphic(
                LayoutFarm.UI.UIPlatformWinNeutral.platform,
                LayoutFarm.UI.UIPlatformWinNeutral.platform.GetIFonts(),
                ww_w, ww_h);

            var surfaceViewportControl = new LayoutFarm.UI.WinNeutral.UISurfaceViewportControl();

            surfaceViewportControl.InitRootGraphics(rootgfx, rootgfx.TopWinEventPortal, InnerViewportKind.GL);


            //lion fill sample
            OpenTkEssTest.T108_LionFill lionFill = new OpenTkEssTest.T108_LionFill();
            lionFill.Init2(canvasGL2d);
            GLCanvasPainter painter1 = lionFill.Painter;

            var myCanvasGL = new PixelFarm.Drawing.GLES2.MyGLCanvas(painter1, 0, 0, 800, 600);
            //(PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvas(0, 0, 800, 600, canvasGL2d, painter1);

            surfaceViewportControl.SetupCanvas(myCanvasGL);

            SampleViewport viewport = new LayoutFarm.SampleViewport(surfaceViewportControl);
            HtmlHost htmlHost;
            htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport, null, null);
            ////==================================================
            //html box
            HtmlBox lightHtmlBox = new HtmlBox(htmlHost, 800, 50);
            {

                lightHtmlBox.SetLocation(50, 450);
                viewport.AddContent(lightHtmlBox);
                //light box can't load full html
                //all light boxs of the same lightbox host share resource with the host
                string html = @"<div>OK1</div><div>OK2</div>";
                //if you want to use full html-> use HtmlBox instead  
                lightHtmlBox.LoadHtmlString(html);
            }

            form1.SetDrawFrameDelegate(() =>
            {
                //render each frame
                if (needUpdateContent)
                {
                    UpdateViewContent(formRenderUpdateEventArgs);
                }
                canvasGL2d.Clear(Color.White);

                //canvasGL2d.DrawRect(0, 0, 200, 200);
                ////canvasGL2d.DrawImage(glBmp, 0, 600);
                //int tmp_x = lightHtmlBox.Left;
                //int tmp_y = lightHtmlBox.Top;
                //myCanvasGL.SetCanvasOrigin(tmp_x, tmp_y);

                canvasGL2d.SmoothMode = CanvasSmoothMode.No;
                //---------
                //flip y axis for html box (and other UI)
                canvasGL2d.FlipY = true;
                lightHtmlBox.CurrentPrimaryRenderElement.DrawToThisCanvas(
                    myCanvasGL, new Rectangle(0, 0, 800, 600));
                canvasGL2d.FlipY = false;
                //myCanvasGL.SetCanvasOrigin(tmp_x, -tmp_y);
                //lion use canvas coordinate system
                lionFill.TestRender();

                //surfaceViewportControl.PaintMe(canvasGL2d);
            });



            while (!GlfwApp.ShouldClose())
            {
                //---------------
                //render phase and swap
                GlfwApp.UpdateWindowsFrame();
                /* Poll for and process events */
                Glfw.PollEvents();
            }

            Glfw.Terminate();
        }
    }
}