// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using PixelFarm.Drawing;

namespace LayoutFarm.UI
{

    abstract partial class TopWindowBridge
    {

        RootGraphic rootGraphic;       
        ITopWindowEventRoot topWinEventRoot;

        CanvasViewport canvasViewport;
        MouseCursorStyle currentCursorStyle = MouseCursorStyle.Default;

        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;

        public TopWindowBridge(RootGraphic rootGraphic, ITopWindowEventRoot topWinEventRoot)
        {
            this.topWinEventRoot = topWinEventRoot;
            this.rootGraphic = rootGraphic;  
        } 
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
        public abstract void PaintToCanvas(Canvas canvas);

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
            System.Windows.Forms.Cursor.Show();
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
        public void HandleMouseDown(MouseEventArgs e)
        {
            canvasViewport.FullMode = false;

            this.topWinEventRoot.RootMouseDown(
                e.X + this.canvasViewport.ViewportX,
                e.Y + this.canvasViewport.ViewportY,
                (int)e.Button);

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
        public void HandleMouseMove(MouseEventArgs e)
        {

            this.topWinEventRoot.RootMouseMove(
                    e.X + this.canvasViewport.ViewportX,
                    e.Y + this.canvasViewport.ViewportY,
                    (int)e.Button);

            if (currentCursorStyle != this.topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this.currentCursorStyle = this.topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();

        }
        public void HandleMouseUp(MouseEventArgs e)
        {
            canvasViewport.FullMode = false;
            topWinEventRoot.RootMouseUp(
                     e.X + this.canvasViewport.ViewportX,
                     e.Y + this.canvasViewport.ViewportY,
                    (int)e.Button);

            if (currentCursorStyle != this.topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this.currentCursorStyle = this.topWinEventRoot.MouseCursorStyle);
            } 
            PrepareRenderAndFlushAccumGraphics();
        }
        public void HandleMouseWheel(MouseEventArgs e)
        {
            canvasViewport.FullMode = true;
            this.topWinEventRoot.RootMouseWheel(e.Delta);

            if (currentCursorStyle != this.topWinEventRoot.MouseCursorStyle)
            {
                ChangeCursorStyle(this.currentCursorStyle = this.topWinEventRoot.MouseCursorStyle);
            }
            PrepareRenderAndFlushAccumGraphics();
        } 
        public void HandleKeyDown(KeyEventArgs e)
        {

#if DEBUG
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UI.UIKeys)e.KeyCode);
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif
            canvasViewport.FullMode = false;
            this.topWinEventRoot.RootKeyDown(e.KeyValue);
            PrepareRenderAndFlushAccumGraphics();
        }      
        public void HandleKeyUp(KeyEventArgs e)
        {
            
            canvasViewport.FullMode = false; 
            this.topWinEventRoot.RootKeyUp(e.KeyValue);

            PrepareRenderAndFlushAccumGraphics(); 
        }
        public void HandleKeyPress(KeyPressEventArgs e)
        {

            if (char.IsControl(e.KeyChar))
            {
                return;
            }
#if DEBUG
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYPRESS " + e.KeyChar);
            dbugTopwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif
            canvasViewport.FullMode = false; 
            this.topWinEventRoot.RootKeyPress(e.KeyChar);

            PrepareRenderAndFlushAccumGraphics();
        } 
        public bool HandleProcessDialogKey(Keys keyData)
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
