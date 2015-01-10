
// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using PixelFarm.Drawing;


namespace LayoutFarm.UI.GdiPlus
{
    class MyTopWindowBridgeGdiPlus : TopWindowBridge
    {
        Control windowControl;
        GdiPlusCanvasViewport gdiPlusViewport;
        public MyTopWindowBridgeGdiPlus(TopWindowRenderBox topwin, IUserEventPortal winEventBridge)
            : base(topwin, winEventBridge)
        {

        }
        public void BindWindowControl(Control windowControl)
        {
            //bind to anycontrol GDI control 
            this.topwin.MakeCurrentTopWindow();
            this.windowControl = windowControl;
            this.SetBaseCanvasViewport(this.gdiPlusViewport = new GdiPlusCanvasViewport(topwin, this.Size.ToSize(), 4));

#if DEBUG
            this.gdiPlusViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        System.Drawing.Size Size
        {
            get { return this.windowControl.Size; }
        }
        protected override void PaintToOutputWindow()
        {
            IntPtr hdc = Win32.Win32Utils.GetDC(this.windowControl.Handle);             
            this.gdiPlusViewport.PaintMe(hdc);
            Win32.Win32Utils.ReleaseDC(this.windowControl.Handle, hdc);
        }
        protected override void PaintToOutputWindowIfNeed()
        {
            if (!this.gdiPlusViewport.IsQuadPageValid)
            {
                //platform specific code ***
                IntPtr hdc = Win32.Win32Utils.GetDC(this.windowControl.Handle);
                this.gdiPlusViewport.PaintMe(hdc);
                Win32.Win32Utils.ReleaseDC(this.windowControl.Handle, hdc);
            }
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