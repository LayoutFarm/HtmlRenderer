//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "110")]
    [Info("T110_DrawText")]
    public class T110_DrawText : SampleBase
    {
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            painter = new GLCanvasPainter(canvas2d, max, max);
            //convert lion vxs to renderVx 
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
            canvas2d.Clear(PixelFarm.Drawing.Color.Red);
            //-------------------------------
            painter.DrawString("OK", 0, 17);
            painter.DrawString("1234567890", 0, 17 * 3);
            //-------------------------------
            SwapBuffer();
        }
    }
}

