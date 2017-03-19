//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Forms;
using PixelFarm.Drawing;
namespace LayoutFarm.UI.Skia
{
    class MyTopWindowBridgeSkia : TopWindowBridgeWinNeutral
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
                new Size(windowControl.Width, windowControl.Height), 4));

            this.RootGfx.SetPaintDelegates(
                    this.canvasViewport.CanvasInvlidateArea,
                    this.PaintToOutputWindow);
#if DEBUG
            this.dbugWinControl = windowControl;
            this.canvasViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
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
            //TODO: review here
            throw new NotSupportedException();
            ////*** force paint to output viewdow
            //IntPtr hdc = Win32.MyWin32.GetDC(this.windowControl.Handle);
            //this.canvasViewport.PaintMe(hdc);
            //Win32.MyWin32.ReleaseDC(this.windowControl.Handle, hdc);
        }
        //public void PrintToCanvas(PixelFarm.Drawing.WinGdi.MyGdiPlusCanvas canvas)
        //{
        //    this.canvasViewport.PaintMe(canvas);
        //}
        protected override void ChangeCursorStyle(MouseCursorStyle cursorStyle)
        {
            //implement change cursor style 
            //switch (cursorStyle)
            //{
            //    case MouseCursorStyle.Pointer:
            //        {
            //            windowControl.Cursor = Cursors.Hand;
            //        }
            //        break;
            //    case MouseCursorStyle.IBeam:
            //        {
            //            windowControl.Cursor = Cursors.IBeam;
            //        }
            //        break;
            //    default:
            //        {
            //            windowControl.Cursor = Cursors.Default;
            //        }
            //        break;
            //}
        }
    }
}