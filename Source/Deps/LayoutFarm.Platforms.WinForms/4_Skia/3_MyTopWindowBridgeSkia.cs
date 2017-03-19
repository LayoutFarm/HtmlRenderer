//Apache2, 2014-2017, WinterDev

using System;
using System.Windows.Forms;
using PixelFarm.Drawing;
#if __SKIA__
namespace LayoutFarm.UI.Skia
{
    class MyTopWindowBridgeSkia : TopWindowBridgeWinForm
    {
        Control windowControl;
        SkiaCanvasViewport canvasViewport;
        public MyTopWindowBridgeSkia(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {
        }
        public override void BindWindowControl(Control windowControl)
        {
            //bind to anycontrol GDI control  
            this.windowControl = windowControl;
            this.SetBaseCanvasViewport(this.canvasViewport = new SkiaCanvasViewport(this.RootGfx,
                this.Size.ToSize(), 4));

            this.RootGfx.SetPaintDelegates(
                    this.canvasViewport.CanvasInvlidateArea,
                    this.PaintToOutputWindow);
#if DEBUG
            this.dbugWinControl = windowControl;
            this.canvasViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        System.Drawing.Size Size
        {
            get { return this.windowControl.Size; }
        }
        public override void InvalidateRootArea(Rectangle r)
        {
            Rectangle rect = r;
            this.RootGfx.InvalidateGraphicArea(
                RootGfx.TopWindowRenderBox,
                ref rect);
        }
        public override void PaintToOutputWindow()
        {
            //*** force paint to output viewdow
            IntPtr hdc = Win32.MyWin32.GetDC(this.windowControl.Handle);
            this.canvasViewport.PaintMe(hdc);
            Win32.MyWin32.ReleaseDC(this.windowControl.Handle, hdc);
        }
        //public void PrintToCanvas(PixelFarm.Drawing.WinGdi.MyGdiPlusCanvas canvas)
        //{
        //    this.canvasViewport.PaintMe(canvas);
        //}
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

#endif