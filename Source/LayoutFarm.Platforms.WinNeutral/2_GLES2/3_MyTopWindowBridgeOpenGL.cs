//Apache2, 2014-2017, WinterDev


using PixelFarm.Drawing;
using PixelFarm.Forms;
using LayoutFarm.UI.WinNeutral;

namespace LayoutFarm.UI.OpenGL
{

    class MyTopWindowBridgeOpenGL : TopWindowBridgeWinNeutral
    {

        bool isInitGLControl;
        OpenGLCanvasViewport openGLViewport;
        UISurfaceViewportControl windowControl;
        public MyTopWindowBridgeOpenGL(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {

        }
        public void SetupCanvas(Canvas canvas)
        {

            openGLViewport.SetCanvas(canvas);
        }
        public override void BindWindowControl(Control windowControl)
        {
            this.windowControl = (UISurfaceViewportControl)windowControl;
            SetBaseCanvasViewport(this.openGLViewport = new OpenGLCanvasViewport(
                this.RootGfx,
                new Size(windowControl.Width, windowControl.Height), 4));

            RootGfx.SetPaintDelegates(
                (r) =>
                {

                }, //still do nothing
                this.PaintToOutputWindow); 

            openGLViewport.NotifyWindowControlBinding();

#if DEBUG
            this.openGLViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }

        protected override void OnClosing()
        {
            //make current before clear GL resource
            this.windowControl.MakeCurrent();
        }
        internal override void OnHostControlLoaded()
        {

            if (!isInitGLControl)
            {
                //init gl after this control is loaded
                //set myGLControl detail
                //1.
                //TODO: review here again 
                windowControl.InitSetup2d(new Rectangle(0, 0, 800, 600));// Screen.PrimaryScreen.Bounds);
                isInitGLControl = true;
                //2.
                windowControl.ClearColor = new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f);
                //3.

            }
        }
        //public override void PaintToCanvas(Canvas canvas)
        //{
        //    throw new NotImplementedException();
        //}

        public override void PaintToOutputWindow()
        {
            //if (!isInitGLControl)
            //{
            //    return;
            //}
            //var innumber = dbugCount;
            //dbugCount++;
            //Console.WriteLine(">" + innumber);
            //TODO: review here

            windowControl.MakeCurrent();
            this.openGLViewport.PaintMe();
            windowControl.SwapBuffers();

            //Console.WriteLine("<" + innumber); 
        }
        protected override void ChangeCursorStyle(MouseCursorStyle cursorStyle)
        {

        }

        public override void InvalidateRootArea(Rectangle r)
        {

        }
    }
}
