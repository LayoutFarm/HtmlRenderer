////Apache2, 2014-2017, WinterDev
//using System;
//using PixelFarm.Forms;
//namespace LayoutFarm.UI
//{

//    partial class MyGlfwForm : GlFwForm
//    {
//        //this ui support gdi+ and skia on WinForms 
//        TopWindowBridgeWinNeutral winBridge;
//        public MyGlfwForm()
//        {
//            //this.MouseWheel += new MouseEventHandler(CpuGdiPlusSurfaceView_MouseWheel);
//        }
//        public void Bind(TopWindowBridgeWinNeutral winBridge)
//        {
//            //1. 
//            this.winBridge = winBridge;
//            this.winBridge.BindWindowControl(this);
//        }
//#if DEBUG
//        public IdbugOutputWindow IdebugOutputWin
//        {
//            get { return this.winBridge; }
//        }
//#endif
//        protected override void OnSizeChanged(EventArgs e)
//        {
//            if (this.winBridge != null)
//            {
//                this.winBridge.UpdateCanvasViewportSize(this.Width, this.Height);
//            }
//            base.OnSizeChanged(e);
//        }

//        protected virtual void OnMouseEnter(EventArgs e)
//        {
//            winBridge.HandleMouseEnterToViewport();

//        }
//        protected virtual void OnMouseLeave(EventArgs e)
//        {
//            winBridge.HandleMouseLeaveFromViewport();

//        }
//        //-----------------------------------------------------------------------------
//        protected virtual void OnGotFocus(EventArgs e)
//        {
//            winBridge.HandleGotFocus(e);
//        }
//        protected virtual void OnLostFocus(EventArgs e)
//        {
//            winBridge.HandleGotFocus(e);
//        }

//        protected virtual void OnMouseDown(UIMouseEventArgs e)
//        {
//            winBridge.HandleMouseDown(e.X, e.Y, e.Button);
//        }
        
//        protected virtual void OnMouseMove(UIMouseEventArgs e)
//        {
//            winBridge.HandleMouseMove(e.X, e.Y, e.Button);

//        }
//        protected virtual void OnMouseUp(UIMouseEventArgs e)
//        {
//            winBridge.HandleMouseUp(e.X, e.Y, e.Button);

//        }
//        //protected virtual void OnPaint(PaintEventArgs e)
//        //{
//        //    this.winBridge.InvalidateRootArea(
//        //        e.ClipRectangle.ToRect());
//        //    this.winBridge.PaintToOutputWindow();
//        //}
//        //protected virtual void OnMouseWheel(MouseEventArgs e)
//        //{
//        //    this.winBridge.HandleMouseWheel(e);
//        //    //not call to base class
//        //}
//        //protected virtual void OnKeyDown(KeyEventArgs e)
//        //{
//        //    this.winBridge.HandleKeyDown(e);

//        //}
//        //protected virtual void OnKeyUp(KeyEventArgs e)
//        //{
//        //    this.winBridge.HandleKeyUp(e);
//        //    base.OnKeyUp(e);
//        //}
//        //protected virtual void OnKeyPress(KeyPressEventArgs e)
//        //{
//        //    this.winBridge.HandleKeyPress(e);
//        //}
//        //protected virtual bool ProcessDialogKey(Keys keyData)
//        //{
//        //    if (this.winBridge.HandleProcessDialogKey(keyData))
//        //    {
//        //        return true;
//        //    }
//        //    return base.ProcessDialogKey(keyData);
//        //}

//        //void CpuGdiPlusSurfaceView_MouseWheel(object sender, MouseEventArgs e)
//        //{
//        //    this.winBridge.HandleMouseWheel(e);
//        //}
//    }
//}
