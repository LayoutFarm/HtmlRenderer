//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI.WinForms
{

    partial class BasicSurfaceView : UserControl
    {


        MyPlatformWindowBridgeGdiPlus winBridge;
        public BasicSurfaceView()
        {
            InitializeComponent();
        }

        public void InitRootGraphics(MyPlatformWindowBridgeGdiPlus winBridge)
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
