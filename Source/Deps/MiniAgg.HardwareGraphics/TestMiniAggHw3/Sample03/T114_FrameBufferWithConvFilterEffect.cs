//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "114")]
    [Info("T114_FrameBuffer")]
    public class T114_FrameBufferWithConvFilterEffect : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        FrameBuffer frameBuffer;

        GLBitmap glbmp;
        bool isInit;
        bool frameBufferNeedUpdate;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            painter = new GLCanvasPainter(canvas2d, max, max);
            frameBuffer = canvas2d.CreateFrameBuffer(this.Width, this.Height);
            frameBufferNeedUpdate = true;
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
                glbmp = LoadTexture(@"..\leaves.jpg");
                isInit = true;
            }
            if (frameBuffer.FrameBufferId > 0)
            {
                if (frameBufferNeedUpdate)
                {
                    //------------------------------------------------------------------------------------           
                    //framebuffer
                    canvas2d.AttachFrameBuffer(frameBuffer);
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    canvas2d.Clear(PixelFarm.Drawing.Color.Red); 
                    canvas2d.DrawImageWithConv3x3(glbmp, Mat3x3ConvGen.emboss, 0, 300);
                    canvas2d.DetachFrameBuffer();

                    //after release current, we move back to default frame buffer again***
                    frameBufferNeedUpdate = false;

                }
                canvas2d.DrawFrameBuffer(frameBuffer, 15, 300);
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

