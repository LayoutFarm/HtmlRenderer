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
        MyTopWindowBridge winViewportBridge;
        public UISurfaceViewportControl()
        {
            InitializeComponent();
        }

        public void InitRootGraphics(TopWindowRenderBox wintop, IUserEventPortal userInputEvBridge )
        {
            //1.
            this.wintop = wintop;
            this.winViewportBridge = new MyTopWindowBridge(wintop, userInputEvBridge);
            this.winViewportBridge.BindWindowControl(this);
        }
#if DEBUG
        public IdbugOutputWindow IOutputWin
        {
            get { return this.winViewportBridge; }
        }
#endif
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.winViewportBridge != null)
            {
                this.winViewportBridge.UpdateCanvasViewportSize(this.Width, this.Height);
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
        public void AddContent(LayoutFarm.UI.UIElement ui)
        {
            AddContent(ui.GetPrimaryRenderElement(wintop.Root));
        }
        public void Close()
        {
            this.winViewportBridge.Close();

        }
        public void PaintMe()
        {
            this.winViewportBridge.PaintMe();

        }

        //-----------------------------------------------------------------------------
        protected override void OnGotFocus(EventArgs e)
        {
            this.winViewportBridge.OnGotFocus(e);
            base.OnGotFocus(e);

        }
        protected override void OnLostFocus(EventArgs e)
        {
            this.winViewportBridge.OnGotFocus(e);
            base.OnLostFocus(e);
        }
        protected override void OnDoubleClick(EventArgs e)
        {
            this.winViewportBridge.OnDoubleClick(e);
            base.OnDoubleClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.winViewportBridge.OnMouseDown(e);
            base.OnMouseDown(e);

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.winViewportBridge.OnMouseMove(e);
            base.OnMouseMove(e);

        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.winViewportBridge.OnMouseUp(e);
            base.OnMouseUp(e);

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            this.winViewportBridge.PaintMe(e);
            base.OnPaint(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.winViewportBridge.OnMouseWheel(e);
            base.OnMouseWheel(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            this.winViewportBridge.OnKeyDown(e);
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            this.winViewportBridge.OnKeyUp(e);
            base.OnKeyUp(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            this.winViewportBridge.OnKeyPress(e);
            return;
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.winViewportBridge.ProcessDialogKey(keyData))
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }



    }



}
