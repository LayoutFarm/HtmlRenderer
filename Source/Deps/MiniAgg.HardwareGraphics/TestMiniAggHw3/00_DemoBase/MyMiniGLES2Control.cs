using System;
using System.Windows.Forms;
using OpenTK;
namespace Mini
{
    public partial class MyMiniGLES2Control : GLControl
    {
        PixelFarm.Drawing.Color clearColor;
        EventHandler glPaintHandler;
        static OpenTK.Graphics.GraphicsMode gfxmode = new OpenTK.Graphics.GraphicsMode(
             DisplayDevice.Default.BitsPerPixel,//default 32 bits color
             16,//depth buffer => 24
             8,  //stencil buffer => 8 (  //if want to use stencil buffer then set stencil buffer too! )
             0,//number of sample of FSAA
             0,  //accum buffer
             2, // n buffer, 2=> double buffer
             false);//sterio
        public MyMiniGLES2Control()
            : base(gfxmode, 2, 0, OpenTK.Graphics.GraphicsContextFlags.Embedded)
        {
            this.InitializeComponent();
        }
        public void SetGLPaintHandler(EventHandler glPaintHandler)
        {
            this.glPaintHandler = glPaintHandler;
        }
        public PixelFarm.Drawing.Color ClearColor
        {
            get { return clearColor; }
            set
            {
                clearColor = value;
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
        }
    }
}
