
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

using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "101")]
    [Info("T101_BlankCanvas")]
    public class T101_BlankCanvas : PrebuiltGLControlDemoBase
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

        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            miniGLControl.SwapBuffers();
        }

    }


}
