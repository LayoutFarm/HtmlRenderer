using System;
using System.Windows.Forms;
#if ENABLE_DESKTOP_OPENGL
using OpenTK.Graphics.OpenGL;
#else
using OpenTK.Graphics.ES20;
#endif
namespace OpenTK
{
    public partial class MyGLControl : GLControl
    {


        OpenTK.Graphics.Color4 clearColor;
        EventHandler glPaintHandler;
        static OpenTK.Graphics.GraphicsMode gfxmode = new OpenTK.Graphics.GraphicsMode(
             DisplayDevice.Default.BitsPerPixel,//default 32 bits color
             16,//depth buffer => 16
             8,  //stencil buffer => 8 (  //if want to use stencil buffer then set stencil buffer too! )
             0,//number of sample of FSAA
             0,  //accum buffer
             2, // n buffer, 2=> double buffer
             false);//sterio
        public MyGLControl()
            : base(gfxmode, 2, 0, OpenTK.Graphics.GraphicsContextFlags.Embedded)
        {
            this.InitializeComponent();
        }
        public void InitSetup2d(int x, int y, int w, int h)
        {

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
                    GL.ClearColor(clearColor);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);
            if (!this.DesignMode)
            {
                MakeCurrent();
                if (glPaintHandler != null)
                {
                    glPaintHandler(this, e);
                }
            }

            //------------------------------------------
            //if (!this.DesignMode)
            //{
            //    MakeCurrent();
            //    GL.Clear(ClearBufferMask.ColorBufferBit);
            //    if (glPaintHandler != null)
            //    {
            //        glPaintHandler(this, e);
            //    }
            //    SwapBuffers();
            //}
        }

    }
}
