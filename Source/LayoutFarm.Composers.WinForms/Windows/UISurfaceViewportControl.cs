//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

    public partial class UISurfaceViewportControl : UserControl
    {


        WinViewportBridge winBridge;
        TopWindowRenderBox wintop;

        MyWinTimer myWinTimer = new MyWinTimer();

        public UISurfaceViewportControl()
        {
            InitializeComponent();
        }

        public void InitRootGraphics(int width, int height)
        {
            var myRootGraphic = new MyRootGraphic(myWinTimer, width, height);
            this.wintop = new TopWindowRenderBox(myRootGraphic, width, height);

            this.winBridge = new WinViewportBridge(this.wintop);
            this.winBridge.BindWindowControl(this);
        }
#if DEBUG
        public IdbugOutputWindow IOutputWin
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
        public void AddContent(LayoutFarm.UI.UIElement ui)
        {
            AddContent(ui.GetPrimaryRenderElement(wintop.Root));
        }
        public void Close()
        {
            this.winBridge.Close();

        }
        public void PaintMe()
        {
            this.winBridge.PaintMe();

        }

        //-----------------------------------------------------------------------------
        protected override void OnGotFocus(EventArgs e)
        {
            this.winBridge.OnGotFocus(e);
            base.OnGotFocus(e);

        }
        protected override void OnLostFocus(EventArgs e)
        {
            this.winBridge.OnGotFocus(e);
            base.OnLostFocus(e);
        }
        protected override void OnDoubleClick(EventArgs e)
        {
            this.winBridge.OnDoubleClick(e);
            base.OnDoubleClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.winBridge.OnMouseDown(e);
            base.OnMouseDown(e);

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.winBridge.OnMouseMove(e);
            base.OnMouseMove(e);

        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.winBridge.OnMouseUp(e);
            base.OnMouseUp(e);

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            this.winBridge.PaintMe(e);
            base.OnPaint(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.winBridge.OnMouseWheel(e);
            base.OnMouseWheel(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            this.winBridge.OnKeyDown(e);
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            this.winBridge.OnKeyUp(e);
            base.OnKeyUp(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            this.winBridge.OnKeyPress(e);
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
