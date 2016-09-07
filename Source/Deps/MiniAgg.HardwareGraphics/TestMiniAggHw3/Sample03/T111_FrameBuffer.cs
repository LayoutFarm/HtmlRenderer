//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
using OpenTK.Graphics.ES20;
namespace OpenTkEssTest
{
    [Info(OrderCode = "111")]
    [Info("T111_FrameBuffer")]
    public class T111_FrameBuffer : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        FrameBuffer frameBuffer;
        bool isInit;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            painter = new GLCanvasPainter(canvas2d, max, max);
            frameBuffer = canvas2d.CreateFrameBuffer(max, max);
            //------------ 
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.Clear(PixelFarm.Drawing.Color.White);
            canvas2d.ClearColorBuffer();
            //-------------------------------
            if (!isInit)
            {
                isInit = true;
            }
            if (frameBuffer.FrameBufferId > 0)
            {
                //------------------------------------------------------------------------------------
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer.FrameBufferId);
                //--------
                //do draw to frame buffer here
                GL.ClearColor(OpenTK.Graphics.Color4.Red);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                //------------------------------------------------------------------------------------



                //------------------------------------------------------------------------------------ 
                GL.BindTexture(TextureTarget.Texture2D, frameBuffer.TextureId);
                GL.GenerateMipmap(TextureTarget.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                //------------------------------------------------------------------------------------

                GLBitmap bmp = new GLBitmap(frameBuffer.TextureId, frameBuffer.Width, frameBuffer.Height);
                bmp.IsBigEndianPixel = true;
                canvas2d.DrawImage(bmp, 15, 300);
            }
            else
            {
                canvas2d.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            miniGLControl.SwapBuffers();
        }
    }
}

