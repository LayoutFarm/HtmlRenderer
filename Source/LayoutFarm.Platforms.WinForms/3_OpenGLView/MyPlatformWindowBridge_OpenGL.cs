
//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.Drawing;

using OpenTK.Graphics;

namespace LayoutFarm.UI.OpenGLView
{
    class MyPlatformWindowBridgeOpenGL : PlatformWindowBridge
    {
        bool isInitGLControl;
        GpuOpenGLSurfaceView windowControl;
        Canvas canvas;
        OpenGLCanvasViewport openGLViewport;
        //---------
        public MyPlatformWindowBridgeOpenGL(TopWindowRenderBox topwin, IUserEventPortal winEventBridge)
            : base(topwin, winEventBridge)
        {
        }
        /// <summary>
        /// bind to gl control
        /// </summary>
        /// <param name="myGLControl"></param>
        public void BindGLControl(GpuOpenGLSurfaceView myGLControl)
        {
            this.canvas = LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, myGLControl.Width, myGLControl.Height);
            this.topwin.MakeCurrent();
            this.windowControl = myGLControl;
            SetBaseCanvasViewport(this.openGLViewport = new OpenGLCanvasViewport(topwin, this.Size.ToSize(), 4));


#if DEBUG
            this.openGLViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        internal override void OnHostControlLoaded()
        {

            if (!isInitGLControl)
            {
                //init gl after this control is loaded
                //set myGLControl detail
                //1.
                windowControl.InitSetup2d(Screen.PrimaryScreen.Bounds);
                isInitGLControl = true;
                //2.
                windowControl.ClearColor = new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f);
                //3.

            }
        }

        //void GLPaintMe(Object sender, EventArgs e)
        //{

        //}
        protected override void PaintToOutputWindow()
        {
            if (!isInitGLControl)
            {
                return;
            }
            //----------------------------------
            //gl paint here
            canvas.ClearSurface(Color.White);
            //test draw rect
            canvas.StrokeColor = LayoutFarm.Drawing.Color.Blue;
            canvas.DrawRectangle(Color.Blue, 20, 20, 200, 200);

            //------------------------
            windowControl.SwapBuffers();
        }
        protected override void PaintToOutputWindowIfNeed()
        {

        }
        protected override void ChangeCursorStyle(UIMouseEventArgs mouseEventArg)
        {

        }
        System.Drawing.Size Size
        {
            get { return this.windowControl.Size; }
        }

    }
}