//MIT, 2014-2016,WinterDev

using System;
using PixelFarm.Drawing;
using OpenTK.Graphics.ES20;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "107")]
    [Info("T107_SampleDrawImage")]
    public class T107_SampleDrawImage : PrebuiltGLControlDemoBase
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
                //glbmp = LoadTexture(@"..\logo-dark.jpg");
                glbmp = LoadTexture(@"..\plain01.png");
                resInit = true;
            }

            canvas2d.DrawSubImage(glbmp, 10, 10, 100, 100, 200, 400);
            canvas2d.DrawImage(glbmp, 0, 300);
            canvas2d.DrawImageWithBlurX(glbmp, 0, 600);
            miniGLControl.SwapBuffers();
        }
    }
}

