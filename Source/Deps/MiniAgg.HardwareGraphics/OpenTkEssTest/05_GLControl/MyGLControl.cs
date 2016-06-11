using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace OpenTK
{
    public partial class MyGLControl : GLControl
    {
        OpenTK.Graphics.Color4 clearColor;
        EventHandler glPaintHandler;
        public MyGLControl()
        {
            OpenTK.Graphics.GraphicsMode gfxmode = new OpenTK.Graphics.GraphicsMode(
             DisplayDevice.Default.BitsPerPixel,//default 32 bits color
             16,//depth buffer => 16
             8,  //stencil buffer => 8 (  //if want to use stencil buffer then set stencil buffer too! )
             0,//number of sample of FSAA
             0,  //accum buffer
             2, // n buffer, 2=> double buffer
             false);//sterio
            ChildCtorOnlyResetGraphicMode(gfxmode);
            //-----------
            this.InitializeComponent();
        }
        public void SetGLPaintHandler(EventHandler glPaintHandler)
        {
            this.glPaintHandler = glPaintHandler;
        }
        public OpenTK.Graphics.Color4 ClearColor
        {
            get { return clearColor; }
            set
            {
                clearColor = value;
                if (!this.DesignMode)
                {
                    MakeCurrent();
                    GL.ClearColor(
                        (float)clearColor.R / 255f,
                        (float)clearColor.G / 255f,
                        (float)clearColor.B / 255f,
                        (float)clearColor.A / 255f);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //------------------------------------------
            if (!this.DesignMode)
            {
                MakeCurrent();
                GL.Clear(ClearBufferMask.ColorBufferBit);
                if (glPaintHandler != null)
                {
                    glPaintHandler(this, e);
                }
                SwapBuffers();
            }
        }
        public void InitSetup2d(Rectangle screenBound)
        {
            int max = Math.Max(screenBound.Width, screenBound.Height);
            int min = Math.Min(screenBound.Width, screenBound.Height);
            //init
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //---------------------------------
            //-1 temp fix split scanline in some screen
            GL.Viewport(0, 0, max, max - 1);
            //---------------------------------

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, max, 0, max, 0.0, 100);
            //GL.Ortho(0, max, max, 0, 0.0, 100);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }
    }
}
