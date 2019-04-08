//MIT, 2017, Zou Wei(github/zwcloud)
//MIT, 2017, WinterDev (modified from Xamarin's Android code template)

using System.IO;
using System; 
using PixelFarm.DrawingGL;
using PixelFarm.Drawing;
using YourImplementation;
using PixelFarm.CpuBlit;

namespace CustomApp01
{
   
    class CustomApp
    {

        GLPainterContext _pcx;
        GLPainter _painter; 

        public void Setup(int canvasW, int canvasH)
        {
            //string curdir = Directory.GetCurrentDirectory();
            //string oneLevelDir = Path.GetDirectoryName(curdir);
            //string basedir = "/";// oneLevelDir + "/newdir";
            //LocalFileStorageProvider.s_globalBaseDir = basedir;
            //Directory.CreateDirectory(basedir);

            string basedir = "";
            PixelFarm.Platforms.StorageService.RegisterProvider(new LocalFileStorageProvider(basedir));
            PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new YourImplementation.ImgCodecMemBitmapIO();



            int max = Math.Max(canvasW, canvasH);
            _pcx = GLPainterContext.Create(max, max, canvasW, canvasH, true);
            _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;

            _painter = new GLPainter();
            _painter.BindToPainterContext(_pcx);
            _painter.SetClipBox(0, 0, canvasW, canvasH);
            _painter.TextPrinter = new GLBitmapGlyphTextPrinter(_painter, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);

            ////--------------------------------------
            ////TODO: review here again

            ////--------------------------------------
            //simpleCanvas = new SimpleCanvas(canvasW, canvasH);

            //var text = "Typography";


            ////optional ....
            ////var directory = AndroidOS.Environment.ExternalStorageDirectory;
            ////var fullFileName = Path.Combine(directory.ToString(), "TypographyTest.txt");
            ////if (File.Exists(fullFileName))
            ////{
            ////    text = File.ReadAllText(fullFileName);
            ////}
            ////-------------------------------------------------------------------------- 
            ////we want to create a prepared visual object ***
            ////textContext = new TypographyTextContext()
            ////{
            ////    FontFamily = "DroidSans.ttf", //corresponding to font file Assets/DroidSans.ttf
            ////    FontSize = 64,//size in Points
            ////    FontStretch = FontStretch.Normal,
            ////    FontStyle = FontStyle.Normal,
            ////    FontWeight = FontWeight.Normal,
            ////    Alignment = DrawingGL.Text.TextAlignment.Leading
            ////};
            ////-------------------------------------------------------------------------- 
            ////create blank text run 
            //textRun = new TextRun();
            ////generate glyph run inside text text run

            //TextPrinter textPrinter = simpleCanvas.TextPrinter;
            //textPrinter.FontFilename = "DroidSans.ttf"; //corresponding to font file Assets/DroidSans.ttf
            //textPrinter.FontSizeInPoints = 64;
            ////
            //simpleCanvas.TextPrinter.GenerateGlyphRuns(textRun, text.ToCharArray(), 0, text.Length);
            ////-------------------------------------------------------------------------- 

            //_memBmp = PixelFarm.CpuBlit.MemBitmap.LoadBitmap("rgb_test1.pngx");

            //_memBmp = new PixelFarm.CpuBlit.MemBitmap(64 * 2, 65);
            //PixelFarm.CpuBlit.AggPainter p = PixelFarm.CpuBlit.AggPainter.Create(_memBmp);
            //p.Clear(Color.Red);

            //_memBmp.SaveImage("output.png");
            //GL.Enable(EnableCap.Texture2D);
        }
        public void RenderFrame()
        {
            _painter.Clear(Color.White);
            _painter.FillColor = Color.Yellow;
            //for (int i = 0; i < 10; ++i)
            //{
            //    _painter.FillRect(100 + i * 120, 200 + i * 120, 100, 100);
            //}

            _painter.FontFillColor = Color.Black;
            _painter.FillRect(100, 250, 20, 20);
            _painter.DrawString("Hello!", 100, 250);
            _painter.DrawString("...from Typography", 100, 300);

        }
    }
}