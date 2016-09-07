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

        protected void SwapBuffer() { }

        protected static PixelFarm.DrawingGL.GLBitmap LoadTexture(string imgFileName)
        {

            var bmp = new PixelFarm.Drawing.Bitmap(imgFileName);
            var glbmp = new PixelFarm.DrawingGL.GLBitmap(bmp, false);
            //we load image from myft's image module
            //its already big-endian
            glbmp.IsBigEndianPixel = true;
            return glbmp;
        }
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