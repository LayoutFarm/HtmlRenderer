
//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.Drawing;

using OpenTK.Graphics;

namespace LayoutFarm.UI
{
    class MyPlatformWindowBridgeOpenGL : MyPlatformWindowBridge
    {
        OpenTK.MyGLControl windowControl;
        public MyPlatformWindowBridgeOpenGL(TopWindowRenderBox topwin, IUserEventPortal winEventBridge)
            : base(topwin, winEventBridge)
        {
        }

        /// <summary>
        /// bind to gl control
        /// </summary>
        /// <param name="myGLControl"></param>
        public void BindGLControl(OpenTK.MyGLControl myGLControl)
        {
            //bind to anycontrol GDI control
            this.topwin.MakeCurrent();
            this.windowControl = myGLControl;
            this.canvasViewport = new CanvasViewport(topwin, this.Size.ToSize(), 4);

#if DEBUG
            this.canvasViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        protected override void PaintToOutputWindow()
        {

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