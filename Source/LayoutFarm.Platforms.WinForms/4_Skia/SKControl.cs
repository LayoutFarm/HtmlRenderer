//SkiaSharp and 
//2016, MIT, WinterDev 

using System.Windows.Forms;

namespace LayoutFarm.UI.Skia
{
    class SKControl : Control
    {
        readonly bool designMode;
        MyTopWindowBridgeSkia winBridge;

        public SKControl()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);

            designMode = DesignMode;
        }
        public void Bind(MyTopWindowBridgeSkia winBridge)
        {
            //1. 
            this.winBridge = winBridge;
            this.winBridge.BindWindowControl(this);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (designMode)
                return;

            var clipRect = e.ClipRectangle;

            this.winBridge.InvalidateRootArea(
                new PixelFarm.Drawing.Rectangle(
                   clipRect.Left, clipRect.Top, clipRect.Width, clipRect.Height));

            this.winBridge.PaintToOutputWindow();
            base.OnPaint(e);
        }
    }
}
