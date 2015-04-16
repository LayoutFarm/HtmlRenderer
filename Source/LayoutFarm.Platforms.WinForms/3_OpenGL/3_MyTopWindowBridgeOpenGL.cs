// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using PixelFarm.Drawing;

using OpenTK.Graphics;

namespace LayoutFarm.UI.OpenGL
{
    class MyTopWindowBridgeOpenGL : TopWindowBridge
    {
        bool isInitGLControl;
        GpuOpenGLSurfaceView windowControl;
        OpenGLCanvasViewport openGLViewport;
        //---------
        public MyTopWindowBridgeOpenGL(RootGraphic root, ITopWindowEventPortal topWinEventPortal)
            : base(root, topWinEventPortal)
        {

        }
        /// <summary>
        /// bind to gl control
        /// </summary>
        /// <param name="myGLControl"></param>
        public void BindGLControl(GpuOpenGLSurfaceView myGLControl)
        {
            this.windowControl = myGLControl;
            SetBaseCanvasViewport(this.openGLViewport = new OpenGLCanvasViewport(this.RootGfx, this.Size.ToSize(), 4));
            RootGfx.SetPaintDelegates(
                (r) => { }, //still do nothing
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
                windowControl.InitSetup2d(Screen.PrimaryScreen.Bounds);
                isInitGLControl = true;
                //2.
                windowControl.ClearColor = new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f);
                //3.

            }
        }
        public override void PaintToCanvas(Canvas canvas)
        {
            throw new NotImplementedException();
        }
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


        protected override void ChangeCursorStyle(UIMouseEventArgs mouseEventArg)
        {

        }
        System.Drawing.Size Size
        {
            get { return this.windowControl.Size; }
        }

    }
}