
#region Using Directives

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using Examples.Tutorial;
using Mini;

#endregion

using LayoutFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "102")]
    [Info("T102_BasicDraw")]
    public class T102_BasicDraw : PrebuiltGLControlDemoBase
    {

        CanvasGL2d canvas2d;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {

            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size
            int max = Math.Max(this.Width, this.Height);

            canvas2d = new CanvasGL2d(max, max);

            //square viewport
            GL.Viewport(0, 0, max, max);
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            canvas2d.StrokeColor = LayoutFarm.Drawing.Color.Blue;
             
            //line
            canvas2d.DrawLine(50, 50, 200, 200);
            //--------------------------------------------
            //rect
            canvas2d.DrawRect(2.5f, 1.5f, 50, 50);
            canvas2d.FillRect(LayoutFarm.Drawing.Color.Green, 50, 50, 50, 50);
            //--------------------------------------------

            //circle & ellipse
            canvas2d.DrawCircle(100, 100, 25);
            canvas2d.DrawEllipse(200, 200, 25, 50);

            canvas2d.FillCircle(LayoutFarm.Drawing.Color.OrangeRed, 100, 400, 25);
            canvas2d.FillEllipse(LayoutFarm.Drawing.Color.OrangeRed, 200, 400, 25, 50);
            //--------------------------------------------
            //polygon
            float[] polygon1 = new float[]{
                50,200,
                250,200,
                125,350
            };
            canvas2d.DrawPolygon(polygon1, 3);  

            float[] polygon2 = new float[]{
                250,400,
                450,400,
                325,550
            };
            canvas2d.FillPolygon(LayoutFarm.Drawing.Color.Green, polygon2);
            //--------------------------------------------
            miniGLControl.SwapBuffers();
        }

    }


}
