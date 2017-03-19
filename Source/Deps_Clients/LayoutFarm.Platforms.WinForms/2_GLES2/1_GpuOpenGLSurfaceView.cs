#if GL_ENABLE
using System;
using System.Windows.Forms;
namespace LayoutFarm.UI.OpenGL
{
    //app specific
    partial class GpuOpenGLSurfaceView : OpenTK.MyGLControl
    {
        MyTopWindowBridgeOpenGL winBridge;
        public GpuOpenGLSurfaceView()
        {
        }

        public void Bind(MyTopWindowBridgeOpenGL winBridge)
        {
            //1. 
            this.winBridge = winBridge;
            this.winBridge.BindWindowControl(this);
        }

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
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            this.winBridge.HandleKeyPress(e);
            return;
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.winBridge.HandleProcessDialogKey(keyData))
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

    }
}
#endif