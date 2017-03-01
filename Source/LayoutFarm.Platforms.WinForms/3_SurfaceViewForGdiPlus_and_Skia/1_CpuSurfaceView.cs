//Apache2, 2014-2017, WinterDev
using System;
using System.Windows.Forms;
namespace LayoutFarm.UI
{

    partial class CpuSurfaceView : UserControl
    {
        //this ui support gdi+ and skia on WinForms

        TopWindowBridgeWinForm winBridge;
        public CpuSurfaceView()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(CpuGdiPlusSurfaceView_MouseWheel);
        }
        public void Bind(TopWindowBridgeWinForm winBridge)
        {
            //1. 
            this.winBridge = winBridge;
            this.winBridge.BindWindowControl(this);
        }
#if DEBUG
        public IdbugOutputWindow IdebugOutputWin
        {
            get { return this.winBridge; }
        }
#endif
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.winBridge != null)
            {
                this.winBridge.UpdateCanvasViewportSize(this.Width, this.Height);
            }
            base.OnSizeChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.winBridge.HandleMouseEnterToViewport();
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            this.winBridge.HandleMouseLeaveFromViewport();
            base.OnMouseLeave(e);
        }
        //-----------------------------------------------------------------------------
        protected override void OnGotFocus(EventArgs e)
        {
            this.winBridge.HandleGotFocus(e);
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            this.winBridge.HandleGotFocus(e);
            base.OnLostFocus(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.winBridge.HandleMouseDown(e);
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.winBridge.HandleMouseMove(e);
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.winBridge.HandleMouseUp(e);
            base.OnMouseUp(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            this.winBridge.InvalidateRootArea(
                e.ClipRectangle.ToRect());
            this.winBridge.PaintToOutputWindow();
            base.OnPaint(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.winBridge.HandleMouseWheel(e);
            //not call to base class
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            this.winBridge.HandleKeyDown(e);
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            this.winBridge.HandleKeyUp(e);
            base.OnKeyUp(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            this.winBridge.HandleKeyPress(e);
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.winBridge.HandleProcessDialogKey(keyData))
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        void CpuGdiPlusSurfaceView_MouseWheel(object sender, MouseEventArgs e)
        {
            this.winBridge.HandleMouseWheel(e);
        }
    }
}
