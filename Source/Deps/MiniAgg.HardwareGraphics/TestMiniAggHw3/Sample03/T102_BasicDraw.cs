//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "102")]
    [Info("T102_BasicDraw")]
    public class T102_BasicDraw : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        PixelFarm.Drawing.RenderVx polygon1;
        PixelFarm.Drawing.RenderVx polygon2;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            painter = new GLCanvasPainter(canvas2d, max, max);
            polygon1 = painter.CreatePolygonRenderVx(new float[]
            {
                50,200,
                250,200,
                125,350
            });
            polygon2 = painter.CreatePolygonRenderVx(new float[]
            {
                250,400,
                450,400,
                325,550
            });
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            Test2();
        }
        void Test2()
        {
            canvas2d.ClearColorBuffer();
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.StrokeWidth = 1;
            painter.StrokeWidth = 1;
            ////line
            painter.FillColor = PixelFarm.Drawing.Color.Green;
            painter.FillRectLBWH(100, 100, 50, 50);
            canvas2d.DrawLine(50, 50, 200, 200);
            canvas2d.DrawRect(10, 10, 50, 50);
            painter.FillRenderVx(polygon2);
            painter.StrokeColor = PixelFarm.Drawing.Color.Blue;
            painter.DrawRenderVx(polygon2);
            //-------------------------------------------
            ////polygon 
            painter.DrawRenderVx(polygon1);
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Green;
            ////--------------------------------------------
            painter.DrawCircle(100, 100, 25);
            painter.DrawEllipse(200, 200, 225, 250);
            ////
            painter.FillColor = PixelFarm.Drawing.Color.OrangeRed;
            painter.FillCircle(100, 400, 25);
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.OrangeRed;
            painter.DrawCircle(100, 400, 25);
            ////
            painter.FillColor = PixelFarm.Drawing.Color.OrangeRed;
            painter.FillEllipse(200, 400, 225, 450);
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.OrangeRed;
            painter.DrawEllipse(200, 400, 225, 450);
            //-------------------------------------------
            miniGLControl.SwapBuffers();
        }
    }
}
