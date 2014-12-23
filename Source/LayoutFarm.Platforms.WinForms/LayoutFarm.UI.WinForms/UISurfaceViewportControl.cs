//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI.WinForms
{

    public partial class UISurfaceViewportControl : UserControl
    {

        TopWindowRenderBox wintop;
        MyPlatformWindowBridge winBridge;
        public UISurfaceViewportControl()
        {
            InitializeComponent();
        }

        public void InitRootGraphics(TopWindowRenderBox wintop, IUserEventPortal userInputEvBridge )
        {
            //1.
            this.wintop = wintop;
            this.winBridge = new MyPlatformWindowBridge(wintop, userInputEvBridge);
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

        public void TopDownRecalculateContent()
        {
            wintop.TopDownReCalculateContentSize();
        }

        public void AddContent(RenderElement vi)
        {
            var layer0 = wintop.Layers.Layer0 as VisualPlainLayer;
            if (layer0 != null)
            {
                layer0.AddChild(vi);
                vi.InvalidateGraphic();
            }
        }
        
        public RootGraphic WinTopRootGfx
        {
            get
            {
                return this.wintop.Root;
            }
            
        }

        public void Close()
        {
            this.winBridge.Close();

        }
        public void PaintMe()
        {
            this.winBridge.PaintMe();

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
            this.winBridge.PaintMe(e);
            base.OnPaint(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.winBridge.HandleMouseWheel(e);
            base.OnMouseWheel(e);
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
            return;
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.winBridge.ProcessDialogKey(keyData))
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }



}
