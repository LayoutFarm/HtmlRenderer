//MIT, 2014-2016,WinterDev

using System;
using OpenTK.Graphics.ES20;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "105")]
    [Info("T105_Stencil")]
    public class T105_Stencil : SampleBase
    {
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        PixelFarm.Drawing.RenderVx stencilPolygon;
        PixelFarm.Drawing.RenderVx rectPolygon;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            painter = new GLCanvasPainter(canvas2d, max, max);
            stencilPolygon = painter.CreatePolygonRenderVx(new float[]
                {
                    20,20,
                    100,20,
                    60,80
                });
            rectPolygon = painter.CreatePolygonRenderVx(new float[]
            {
                    5,5,
                    100,5,
                    100,100,
                    5,100
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
            //-----------------------------
            //see:  lazyfoo.net/tutorials/OpenGL/26_the_stencil_buffer/index.php
            //-----------------------------

            canvas2d.Clear(PixelFarm.Drawing.Color.White);
            //-------------------
            //disable rendering to color buffer
            GL.ColorMask(false, false, false, false);
            //start using stencil
            GL.Enable(EnableCap.StencilTest);
            //place a 1 where rendered
            GL.StencilFunc(StencilFunction.Always, 1, 1);
            //replace where rendered
            GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
            //render  to stencill buffer

            painter.FillColor = PixelFarm.Drawing.Color.Black;
            painter.FillRenderVx(stencilPolygon);
            painter.StrokeColor = PixelFarm.Drawing.Color.Black;
            //render color
            GL.ColorMask(true, true, true, true);
            //where a 1 was not rendered
            GL.StencilFunc(StencilFunction.Equal, 1, 1);
            //keep the pixel
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            //draw  
            painter.FillColor = PixelFarm.Drawing.Color.Red;
            painter.FillRenderVx(rectPolygon);
            GL.Disable(EnableCap.StencilTest);
            //-----------------------------------------------------------
            SwapBuffer();
        }
    }
}

