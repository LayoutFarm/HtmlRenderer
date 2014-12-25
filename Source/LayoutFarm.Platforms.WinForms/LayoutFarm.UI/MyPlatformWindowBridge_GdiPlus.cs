
//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.Drawing;


namespace LayoutFarm.UI
{
    class MyPlatformWindowBridgeGdiPlus : MyPlatformWindowBridge
    {
        Control windowControl;
        public MyPlatformWindowBridgeGdiPlus(TopWindowRenderBox topwin, IUserEventPortal winEventBridge)
            : base(topwin, winEventBridge)
        {
        }
        System.Drawing.Size Size
        {
            get { return this.windowControl.Size; }
        }

        protected override void PaintToOutputWindow()
        {
            IntPtr hdc = DrawingBridge.Win32Utils.GetDC(this.windowControl.Handle);
            canvasViewport.PaintMe(hdc);
            DrawingBridge.Win32Utils.ReleaseDC(this.windowControl.Handle, hdc);
        }
        protected override void PaintToOutputWindowIfNeed()
        {
            if (!this.canvasViewport.IsQuadPageValid)
            {
                IntPtr hdc = DrawingBridge.Win32Utils.GetDC(this.windowControl.Handle);
                canvasViewport.PaintMe(hdc);
                DrawingBridge.Win32Utils.ReleaseDC(this.windowControl.Handle, hdc);
            }
        }
        public void BindWindowControl(Control windowControl)
        {
            //bind to anycontrol GDI control 
            this.topwin.MakeCurrent();
            this.windowControl = windowControl;
            this.canvasViewport = new CanvasViewport(topwin, this.Size.ToSize(), 4);

#if DEBUG
            this.canvasViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        protected override void ChangeCursorStyle(UIMouseEventArgs mouseEventArg)
        {
            switch (mouseEventArg.MouseCursorStyle)
            {
                case MouseCursorStyle.Pointer:
                    {
                        windowControl.Cursor = Cursors.Hand;
                    } break;
                case MouseCursorStyle.IBeam:
                    {
                        windowControl.Cursor = Cursors.IBeam;
                    } break;
                default:
                    {
                        windowControl.Cursor = Cursors.Default;
                    } break;
            }
            this.currentCursorStyle = mouseEventArg.MouseCursorStyle;
        }
    }
}