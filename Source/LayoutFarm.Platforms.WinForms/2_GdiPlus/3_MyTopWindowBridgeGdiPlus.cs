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
        public MyTopWindowBridgeGdiPlus(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {

        }
        internal void BindWindowControl(Control windowControl)
        {
            //bind to anycontrol GDI control  
            this.windowControl = windowControl;
            this.SetBaseCanvasViewport(this.gdiPlusViewport = new GdiPlusCanvasViewport(this.RootGfx, this.Size.ToSize(), 4));
            this.RootGfx.SetPaintDelegates(
                    this.gdiPlusViewport.CanvasInvlidateArea,
                    this.PaintToOutputWindow);

#if DEBUG
            this.dbugWinControl = windowControl;
            this.gdiPlusViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        System.Drawing.Size Size
        {
            get { return this.windowControl.Size; }
        }
        public void InvalidateRootArea(Rectangle r)
        {
            Rectangle rect = r;
            this.RootGfx.InvalidateGraphicArea(
                RootGfx.TopWindowRenderBox,
                ref rect);
        }
        public override void PaintToOutputWindow()
        {
            IntPtr hdc = Win32.Win32Utils.GetDC(this.windowControl.Handle);
            this.gdiPlusViewport.PaintMe(hdc);
            Win32.Win32Utils.ReleaseDC(this.windowControl.Handle, hdc);
        }
        public override void PaintToCanvas(Canvas canvas)
        {
            this.gdiPlusViewport.PaintMe(canvas);

        }
        protected override void ChangeCursorStyle(MouseCursorStyle cursorStyle)
        {
            switch (cursorStyle)
            {
                case MouseCursorStyle.Pointer:
                    {
                        windowControl.Cursor = Cursors.Hand;
                    }
                    break;
                case MouseCursorStyle.IBeam:
                    {
                        windowControl.Cursor = Cursors.IBeam;
                    }
                    break;
                default:
                    {
                        windowControl.Cursor = Cursors.Default;
                    }
                    break;
            }
        }

    }
}