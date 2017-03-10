//BSD, 2014-2017, WinterDev
using System;
namespace OpenTkEssTest
{
    public abstract class SampleBase
    {
        bool _enableAnimationTimer;

        public void InitGLProgram()
        {
            OnInitGLProgram(this, EventArgs.Empty);
        }
        public void Close()
        {
            DemoClosing();
        }
        public void Render()
        {
            OnGLRender(this, EventArgs.Empty);
        }
        protected abstract void OnInitGLProgram(object sender, EventArgs e);
        protected abstract void DemoClosing();
        protected virtual void OnGLRender(object sender, EventArgs args) { }
        public int Width { get; set; }
        public int Height { get; set; }

        protected void SwapBuffer()
        {
        }
        public PixelFarm.DrawingGL.CanvasGL2d CreateCanvasGL2d(int w, int h)
        {
            return PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(w, h);
        }
        protected void SetPrinter(PixelFarm.DrawingGL.GLCanvasPainter painter)
        {

            //var textPrinter = new PixelFarm.DrawingGL.GLBmpGlyphTextPrinter(painter, YourImplementation.BootStrapOpenGLES2.myFontLoader);
            var textPrinter = new PixelFarm.DrawingGL.AggTextSpanPrinter(painter, 100, 20);
            painter.TextPrinter = textPrinter;
        }
        //protected static PixelFarm.DrawingGL.GLBitmap LoadTexture(string imgFileName)
        //{
        //    //1. create native image
        //    var nativeImg = new PixelFarm.Drawing.Imaging.NativeImage(imgFileName); 
        //    //2. create glbmp from native img
        //    var glbmp = new PixelFarm.DrawingGL.GLBitmap(nativeImg, false);
        //    //we load image from myft's image module
        //    //its already big-endian
        //    glbmp.IsBigEndianPixel = true;
        //    return glbmp;
        //}
        protected bool EnableAnimationTimer
        {
            get { return _enableAnimationTimer; }
            set
            {
                _enableAnimationTimer = value;
            }
        }
        protected virtual void OnTimerTick(object sender, EventArgs e)
        {

        }
    }
}