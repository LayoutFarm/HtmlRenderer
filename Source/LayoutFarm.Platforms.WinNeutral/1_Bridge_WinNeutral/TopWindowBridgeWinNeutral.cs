//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Drawing;
using PixelFarm.Forms;
namespace LayoutFarm.UI
{
    abstract partial class TopWindowBridgeWinNeutral
    {
        RootGraphic rootGraphic;
        ITopWindowEventRoot topWinEventRoot;
        CanvasViewport canvasViewport;
        MouseCursorStyle currentCursorStyle = MouseCursorStyle.Default;
        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;
        public TopWindowBridgeWinNeutral(RootGraphic rootGraphic, ITopWindowEventRoot topWinEventRoot)
        {
            this.topWinEventRoot = topWinEventRoot;
            this.rootGraphic = rootGraphic;
        }
        public abstract void BindWindowControl(Control windowControl);
        public abstract void InvalidateRootArea(Rectangle r);
        public RootGraphic RootGfx
        {
            get { return this.rootGraphic; }
        }
        protected abstract void ChangeCursorStyle(MouseCursorStyle cursorStyle);
        protected void SetBaseCanvasViewport(CanvasViewport canvasViewport)
        {
            this.canvasViewport = canvasViewport;
        }
        internal virtual void OnHostControlLoaded()
        {
        }
        public void PaintToOutputWindowFullMode()
        {
            Rectangle rect = new Rectangle(0, 0, rootGraphic.Width, rootGraphic.Height);
            rootGraphic.InvalidateGraphicArea(
                rootGraphic.TopWindowRenderBox,
                ref rect);
            this.PaintToOutputWindow();
        }
        public abstract void PaintToOutputWindow();

        public void UpdateCanvasViewportSize(int w, int h)
        {
            this.canvasViewport.UpdateCanvasViewportSize(w, h);
        }

        public void Close()
        {
            OnClosing();
            canvasViewport.Close();
        }
        protected virtual void OnClosing()
        {
        }
        //---------------------------------------------------------------------
        public void EvaluateScrollbar()
        {
            ScrollSurfaceRequestEventArgs hScrollSupportEventArgs;
            ScrollSurfaceRequestEventArgs vScrollSupportEventArgs;
            canvasViewport.EvaluateScrollBar(out hScrollSupportEventArgs, out vScrollSupportEventArgs);
            if (hScrollSupportEventArgs != null)
            {
                viewport_HScrollRequest(this, hScrollSupportEventArgs);
            }
            if (vScrollSupportEventArgs != null)
            {
                viewport_VScrollRequest(this, vScrollSupportEventArgs);
            }
        }
        public void ScrollBy(int dx, int dy)
        {
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            canvasViewport.ScrollByNotRaiseEvent(dx, dy, out hScrollEventArgs, out vScrollEventArgs);
            if (vScrollEventArgs != null)
            {
                viewport_VScrollChanged(this, vScrollEventArgs);
            }
            if (hScrollEventArgs != null)
            {
                viewport_HScrollChanged(this, hScrollEventArgs);
            }


            PaintToOutputWindow();
        }
        public void ScrollTo(int x, int y)
        {
            Point viewporyLocation = canvasViewport.LogicalViewportLocation;
            if (viewporyLocation.Y == y && viewporyLocation.X == x)
            {
                return;
            }
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            canvasViewport.ScrollToNotRaiseScrollChangedEvent(x, y, out hScrollEventArgs, out vScrollEventArgs);
            if (vScrollEventArgs != null)
            {
                viewport_VScrollChanged(this, vScrollEventArgs);
            }
            if (hScrollEventArgs != null)
            {
                viewport_HScrollChanged(this, vScrollEventArgs);
            }

            PaintToOutputWindow();
        }

        void viewport_HScrollChanged(object sender, UIScrollEventArgs e)
        {
            if (HScrollChanged != null)
            {
                HScrollChanged.Invoke(sender, e);
            }
        }
        void viewport_HScrollRequest(object sender, ScrollSurfaceRequestEventArgs e)
        {
            if (HScrollRequest != null)
            {
                HScrollRequest.Invoke(sender, e);
            }
        }
        void viewport_VScrollChanged(object sender, UIScrollEventArgs e)
        {
            if (VScrollChanged != null)
            {
                VScrollChanged.Invoke(sender, e);
            }
        }
        void viewport_VScrollRequest(object sender, ScrollSurfaceRequestEventArgs e)
        {
            if (VScrollRequest != null)
            {
                VScrollRequest.Invoke(sender, e);
            }
        }
        public void HandleMouseEnterToViewport()
        {
            //System.Windows.Forms.Cursor.Hide();
        }
        public void HandleMouseLeaveFromViewport()
        {
            //platform specific to hide or show cursor
            //System.Windows.Forms.Cursor.Show();
        }
        public void HandleGotFocus(EventArgs e)
        {
            if (canvasViewport.IsClosed)
            {
                return;
            }

            canvasViewport.FullMode = false;
            this.topWinEventRoot.RootGotFocus();
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleLostFocus(EventArgs e)
        {
            canvasViewport.FullMode = false;
            this.topWinEventRoot.RootLostFocus();
            PrepareRenderAndFlushAccumGraphics();
        }
        //------------------------------------------------------------------------
        public void HandleMouseDown(int x, int y, UIMouseButtons b)
        {
            canvasViewport.FullMode = false;
            this.topWinEventRoot.RootMouseDown(
                x + this.canvasViewport.ViewportX,
                y + this.canvasViewport.ViewportY,
                b);
            if (currentCursorStyle != this.topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this.currentCursorStyle = this.topWinEventRoot.MouseCursorStyle);
            }

            PrepareRenderAndFlushAccumGraphics();
#if DEBUG
            RootGraphic visualroot = this.dbugTopwin.dbugVRoot;
            if (visualroot.dbug_RecordHitChain)
            {
                dbug_rootDocHitChainMsgs.Clear();
                visualroot.dbug_DumpCurrentHitChain(dbug_rootDocHitChainMsgs);
                dbug_InvokeHitChainMsg();
            }
#endif

        }
        public void HandleMouseMove(int x, int y, UIMouseButtons b)
        {
            this.topWinEventRoot.RootMouseMove(
                    x + this.canvasViewport.ViewportX,
                    y + this.canvasViewport.ViewportY,
                    b);
            if (currentCursorStyle != this.topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this.currentCursorStyle = this.topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();
        }
        //static UIMouseButtons GetMouseButton(System.Windows.Forms.MouseButtons button)
        //{
        //    switch (button)
        //    {
        //        case MouseButtons.Left:
        //            return UIMouseButtons.Left;
        //        case MouseButtons.Right:
        //            return UIMouseButtons.Right;
        //        case MouseButtons.Middle:
        //            return UIMouseButtons.Middle;
        //        default:
        //            return UIMouseButtons.Left;
        //    }
        //}
        public void HandleMouseUp(int x, int y, UIMouseButtons b)
        {
            canvasViewport.FullMode = false;
            topWinEventRoot.RootMouseUp(
                   x + this.canvasViewport.ViewportX,
                   y + this.canvasViewport.ViewportY,
                   b);
            if (currentCursorStyle != this.topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this.currentCursorStyle = this.topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleMouseWheel(int delta)
        {
            canvasViewport.FullMode = true;
            this.topWinEventRoot.RootMouseWheel(delta);
            if (currentCursorStyle != this.topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this.currentCursorStyle = this.topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleKeyDown(int keyValue)
        {
//#if DEBUG
//            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
//            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UI.UIKeys)e.KeyCode);
//            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
//#endif
            canvasViewport.FullMode = false;
            this.topWinEventRoot.RootKeyDown(keyValue);
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleKeyUp(int keyValue)
        {
            canvasViewport.FullMode = false;
            this.topWinEventRoot.RootKeyUp(keyValue);
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleKeyPress(char c)
        {
            if (char.IsControl(c))
            {
                return;
            }
#if DEBUG
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYPRESS " + c);
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif
            canvasViewport.FullMode = false;
            this.topWinEventRoot.RootKeyPress(c);
            PrepareRenderAndFlushAccumGraphics();
        }
        public bool HandleProcessDialogKey(int keyData)
        {
            canvasViewport.FullMode = false;
            bool result = this.topWinEventRoot.RootProcessDialogKey((int)keyData);
            if (result)
            {
                PrepareRenderAndFlushAccumGraphics();
            }
            return result;
        }

        void PrepareRenderAndFlushAccumGraphics()
        {
            this.rootGraphic.PrepareRender();
            this.rootGraphic.FlushAccumGraphics();
        }
    }
}
