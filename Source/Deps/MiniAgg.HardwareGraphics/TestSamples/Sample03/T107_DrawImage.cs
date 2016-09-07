//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "107")]
    [Info("T107_SampleDrawImage")]
    public class T107_SampleDrawImage : SampleBase
    {
        CanvasGL2d canvas2d;
        bool resInit;
        GLBitmap glbmp;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            if (!resInit)
            {
                //glbmp = LoadTexture(@"..\a00124.png");
                glbmp = LoadTexture(@"..\logo-dark.jpg");
                resInit = true;
            }

            canvas2d.DrawImage(glbmp, 0, 300);
            canvas2d.DrawImageWithBlurX(glbmp, 0, 600);
        }
    }
}

