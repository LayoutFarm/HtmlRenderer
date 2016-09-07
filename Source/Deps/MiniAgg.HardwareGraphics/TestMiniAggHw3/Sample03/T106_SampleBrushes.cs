//MIT, 2014-2016,WinterDev

using System;
using PixelFarm.Drawing;
using OpenTK.Graphics.ES20;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "106")]
    [Info("T106_SampleBrushes")]
    public class T106_SampleBrushes : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        RenderVx polygon1;
        RenderVx polygon2;
        RenderVx polygon3;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            painter = new GLCanvasPainter(canvas2d, max, max);
            polygon1 = painter.CreatePolygonRenderVx(new float[]
                {
                    0,50,
                    50,50,
                    10,100
                });
            polygon2 = painter.CreatePolygonRenderVx(new float[]
              {
                   200, 50,
                   250, 50,
                   210, 100
              });
            polygon3 = painter.CreatePolygonRenderVx(new float[]
              {
                 400, 50,
                 450, 50,
                 410, 100
              });
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
            painter.FillColor = PixelFarm.Drawing.Color.Black;
            painter.FillRectLBWH(0, 0, 150, 150);
            GLBitmap glBmp = LoadTexture("..\\logo-dark.jpg");
            var textureBrush = new TextureBrush(new GLImage(glBmp));
            painter.FillRenderVx(textureBrush, polygon1);
            //------------------------------------------------------------------------- 
            var linearGrBrush2 = new LinearGradientBrush(
              new PointF(0, 50), Color.Red,
              new PointF(400, 100), Color.White);
            //fill polygon with gradient brush  
            painter.FillColor = Color.Yellow;
            painter.FillRectLBWH(200, 0, 150, 150);
            painter.FillRenderVx(linearGrBrush2, polygon2);
            painter.FillColor = Color.Black;
            painter.FillRectLBWH(400, 0, 150, 150);
            //------------------------------------------------------------------------- 

            //another  ...                

            painter.FillRenderVx(linearGrBrush2, polygon3);
            //------------------------------------------------------------------------- 


            miniGLControl.SwapBuffers();
        }
    }
}

