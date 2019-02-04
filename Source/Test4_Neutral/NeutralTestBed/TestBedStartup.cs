
//Apache2, 2014-present, WinterDev
//#define GL_ENABLE
using System;
using PixelFarm;
using PixelFarm.Forms;
using Typography.FontManagement;
using LayoutFarm.UI;
using LayoutFarm.UI.WinNeutral;
using PixelFarm.CpuBlit;
using System.IO;
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

    class ImgCodecMemBitmapIO : PixelFarm.CpuBlit.MemBitmapIO
    {
        public override MemBitmap LoadImage(string filename)
        {
            OutputImageFormat format;
            //TODO: review img loading, we should not use only its extension 
            string fileExt = System.IO.Path.GetExtension(filename).ToLower();
            switch (fileExt)
            {
                case ".png":
                    {
                        using (FileStream fs = new FileStream(filename, FileMode.Open))
                        {
                            return PngIOStorage.Read(fs);
                        }
                    }
                case ".jpg":
                    {
                        format = OutputImageFormat.Jpeg;
                    }
                    break;
                default:
                    throw new System.NotSupportedException();
            }

            //TODO: don't directly access file here
            //we should access file from host request
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                return LoadImage(fs, format);
            }
        }

        public override MemBitmap LoadImage(Stream input)
        {
            throw new NotImplementedException();
        }
        public MemBitmap LoadImage(Stream input, OutputImageFormat format)
        {
            ImageTools.ExtendedImage extendedImg = new ImageTools.ExtendedImage();
            //TODO: review img loading, we should not use only its extension
            switch (format)
            {
                case OutputImageFormat.Png:
                    {
                        var decoder = new ImageTools.IO.Png.PngDecoder();
                        extendedImg.Load(input, decoder);
                    }
                    break;
                case OutputImageFormat.Jpeg:
                    {
                        var decoder = new ImageTools.IO.Jpeg.JpegDecoder();
                        extendedImg.Load(input, decoder);
                    }
                    break;
                default:
                    throw new System.NotSupportedException();

            }

            //assume 32 bit ?? 
            byte[] pixels = extendedImg.Pixels;
            unsafe
            {
                fixed (byte* p_src = &pixels[0])
                {
                    PixelFarm.CpuBlit.MemBitmap memBmp = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(
                       extendedImg.PixelWidth,
                       extendedImg.PixelHeight,
                       (IntPtr)p_src,
                       pixels.Length,
                       false
                       );

                    memBmp.IsBigEndian = true;
                    return memBmp;
                }
            }

            ////PixelFarm.CpuBlit.MemBitmap memBmp = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(
            ////    extendedImg.PixelWidth,
            ////    extendedImg.PixelHeight,
            ////    extendedImg.PixelWidth * 4, //assume
            ////    32, //assume?
            ////    extendedImg.Pixels,
            ////    false
            ////    );
            ////the imgtools load data as BigEndian
            //memBmp.IsBigEndian = true;
            //return memBmp;
        }

        public override void SaveImage(MemBitmap bitmap, Stream output, OutputImageFormat outputFormat, object saveParameters)
        {
            throw new NotImplementedException();
        }

        public override void SaveImage(MemBitmap bitmap, string filename, OutputImageFormat outputFormat, object saveParameters)
        {
            throw new NotImplementedException();
        }
    }

    static class PngIOStorage
    {
        //TODO: remove duplicated code here!!

        public static MemBitmap Read(Stream strm)
        {

            Hjg.Pngcs.PngReader reader = new Hjg.Pngcs.PngReader(strm);
            Hjg.Pngcs.ImageInfo imgInfo = reader.ImgInfo;
            Hjg.Pngcs.ImageLine iline2 = new Hjg.Pngcs.ImageLine(imgInfo, Hjg.Pngcs.ImageLine.ESampleType.BYTE);

            int imgH = imgInfo.Rows;
            int imgW = imgInfo.Cols;

            int widthPx = imgInfo.Cols;
            int stride = widthPx * 4;
            //expand to 32 bits 
            int[] buffer = new int[(stride / 4) * imgH];
            bool isInverted = false;
            if (isInverted)
            {
                //read each row 
                //and fill the glyph image 
                int startWriteAt = (imgW * (imgH - 1));
                int destIndex = startWriteAt;
                for (int row = 0; row < imgH; row++)
                {
                    Hjg.Pngcs.ImageLine iline = reader.ReadRowByte(row);
                    byte[] scline = iline.ScanlineB;
                    int b_src = 0;
                    destIndex = startWriteAt;


                    if (imgInfo.BitspPixel == 32)
                    {
                        for (int mm = 0; mm < imgW; ++mm)
                        {
                            byte b = scline[b_src];
                            byte g = scline[b_src + 1];
                            byte r = scline[b_src + 2];
                            byte a = scline[b_src + 3];
                            b_src += 4;

                            buffer[destIndex] = (b << 16) | (g << 8) | (r) | (a << 24);
                            destIndex++;
                        }

                    }
                    else if (imgInfo.BitspPixel == 24)
                    {
                        for (int mm = 0; mm < imgW; ++mm)
                        {
                            byte b = scline[b_src];
                            byte g = scline[b_src + 1];
                            byte r = scline[b_src + 2];
                            b_src += 3;
                            buffer[destIndex] = (b << 16) | (g << 8) | (r) | (255 << 24);
                            destIndex++;
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }




                    startWriteAt -= imgW;
                }
                return MemBitmap.CreateFromCopy(imgW, imgH, buffer);
            }
            else
            {
                //read each row 
                //and fill the glyph image 
                int startWriteAt = 0;
                int destIndex = startWriteAt;
                for (int row = 0; row < imgH; row++)
                {
                    Hjg.Pngcs.ImageLine iline = reader.ReadRowByte(row);
                    byte[] scline = iline.ScanlineB;

                    int b_src = 0;
                    destIndex = startWriteAt;


                    if (imgInfo.BitspPixel == 32)
                    {
                        for (int mm = 0; mm < imgW; ++mm)
                        {
                            byte b = scline[b_src];
                            byte g = scline[b_src + 1];
                            byte r = scline[b_src + 2];
                            byte a = scline[b_src + 3];
                            b_src += 4;
                            buffer[destIndex] = (b << 16) | (g << 8) | (r) | (a << 24);
                            destIndex++;
                        }
                        startWriteAt += imgW;
                    }
                    else if (imgInfo.BitspPixel == 24)
                    {
                        for (int mm = 0; mm < imgW; ++mm)
                        {
                            byte b = scline[b_src];
                            byte g = scline[b_src + 1];
                            byte r = scline[b_src + 2];
                            b_src += 3;
                            buffer[destIndex] = (b << 16) | (g << 8) | (r) | (255 << 24);
                            destIndex++;
                        }
                        startWriteAt += imgW;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                }
                return MemBitmap.CreateFromCopy(imgW, imgH, buffer);
            }


        }
        public static void Save(MemBitmap bmp, Stream strm)
        {
            //-------------
            unsafe
            {
                PixelFarm.CpuBlit.Imaging.TempMemPtr tmp = MemBitmap.GetBufferPtr(bmp);
                int* intBuffer = (int*)tmp.Ptr;

                int imgW = bmp.Width;
                int imgH = bmp.Height;

                Hjg.Pngcs.ImageInfo imgInfo = new Hjg.Pngcs.ImageInfo(imgW, imgH, 8, true); //8 bits per channel with alpha
                Hjg.Pngcs.PngWriter writer = new Hjg.Pngcs.PngWriter(strm, imgInfo);
                Hjg.Pngcs.ImageLine iline = new Hjg.Pngcs.ImageLine(imgInfo, Hjg.Pngcs.ImageLine.ESampleType.BYTE);
                int startReadAt = 0;

                int imgStride = imgW * 4;

                int srcIndex = 0;
                int srcIndexRowHead = (tmp.LengthInBytes / 4) - imgW;

                for (int row = 0; row < imgH; row++)
                {
                    byte[] scanlineBuffer = iline.ScanlineB;
                    srcIndex = srcIndexRowHead;
                    for (int b = 0; b < imgStride;)
                    {
                        int srcInt = intBuffer[srcIndex];
                        srcIndex++;
                        scanlineBuffer[b] = (byte)((srcInt >> 16) & 0xff);
                        scanlineBuffer[b + 1] = (byte)((srcInt >> 8) & 0xff);
                        scanlineBuffer[b + 2] = (byte)((srcInt) & 0xff);
                        scanlineBuffer[b + 3] = (byte)((srcInt >> 24) & 0xff);
                        b += 4;
                    }
                    srcIndexRowHead -= imgW;
                    startReadAt += imgStride;
                    writer.WriteRow(iline, row);
                }
                writer.End();
            }


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