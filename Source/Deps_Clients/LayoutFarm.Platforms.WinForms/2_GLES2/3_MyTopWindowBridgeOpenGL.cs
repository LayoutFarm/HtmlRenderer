//Apache2, 2014-2017, WinterDev
#if GL_ENABLE 
using System.Windows.Forms;
using PixelFarm.Drawing;


namespace LayoutFarm.UI.OpenGL
{
    class MyTopWindowBridgeOpenGL : TopWindowBridgeWinForm
    {


        bool isInitGLControl;
        GpuOpenGLSurfaceView windowControl;
        OpenGLCanvasViewport openGLViewport;

        public MyTopWindowBridgeOpenGL(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {

        }
        public void SetCanvas(Canvas canvas)
        {
            this.openGLViewport.SetCanvas(canvas);
        }
        public override void InvalidateRootArea(Rectangle r)
        {

        }
        public override void BindWindowControl(Control windowControl)
        {
            this.BindGLControl((GpuOpenGLSurfaceView)windowControl);

        }

        /// <summary>
        /// bind to gl control
        /// </summary>
        /// <param name="myGLControl"></param>
        void BindGLControl(GpuOpenGLSurfaceView myGLControl)
        {
            this.windowControl = myGLControl;
            SetBaseCanvasViewport(this.openGLViewport = new OpenGLCanvasViewport(this.RootGfx, this.windowControl.Size.ToSize(), 4));
            RootGfx.SetPaintDelegates(
                (r) => {

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
                var bounds = Screen.PrimaryScreen.Bounds;
                windowControl.InitSetup2d(bounds.X, bounds.Y, bounds.Width, bounds.Height);
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
            if (!isInitGLControl)
            {
                return;
            }
            //var innumber = dbugCount;
            //dbugCount++;
            //Console.WriteLine(">" + innumber);
            windowControl.MakeCurrent();
            this.openGLViewport.PaintMe();
            windowControl.SwapBuffers();
            //Console.WriteLine("<" + innumber); 
        }
        protected override void ChangeCursorStyle(MouseCursorStyle cursorStyle)
        {

        }
    }
}
#endif