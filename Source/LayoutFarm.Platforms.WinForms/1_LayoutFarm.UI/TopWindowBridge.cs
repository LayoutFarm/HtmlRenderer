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
        CanvasEventsStock eventStock = new CanvasEventsStock();
        IUserEventPortal userEventPortal;
        protected TopWindowRenderBox topwin;
        CanvasViewport canvasViewport;


        bool isMouseDown;
        bool isDragging;


        bool lastKeydownWithControl;
        bool lastKeydownWithAlt;
        bool lastKeydownWithShift;


        protected MouseCursorStyle currentCursorStyle = MouseCursorStyle.Default;

        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;

        RootGraphic rootGraphic;

        public TopWindowBridge(RootGraphic rootGraphic, IUserEventPortal winEventBridge)
        {
            this.userEventPortal = winEventBridge;
            this.rootGraphic = rootGraphic;
            this.topwin = rootGraphic.TopWindowRenderBox;
        }
#if DEBUG
        internal Control dbugWinControl;
#endif
        public RootGraphic RootGfx
        {
            get { return this.rootGraphic; }
        }
        protected void SetBaseCanvasViewport(CanvasViewport canvasViewport)
        {
            this.canvasViewport = canvasViewport;
        }
        internal virtual void OnHostControlLoaded()
        {
        }
        public void PaintToOutputWindowFullMode()
        {

            Rectangle rect = new Rectangle(0, 0, rootGraphic.RootWidth, rootGraphic.RootHeight);
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




        static UIMouseButtons GetUIMouseButton(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.Right:
                    return UIMouseButtons.Right;
                case MouseButtons.Middle:
                    return UIMouseButtons.Middle;
                case MouseButtons.None:
                    return UIMouseButtons.None;
                default:
                    return UIMouseButtons.Left;
            }
        }
        void SetUIMouseEventArgsInfo(UIMouseEventArgs mouseEventArg, MouseEventArgs e)
        {
            mouseEventArg.SetEventInfo(
                e.X, e.Y,
                GetUIMouseButton(e.Button),
                e.Clicks,
                e.Delta,
                this.isDragging);

            OffsetCanvasOrigin(mouseEventArg, this.canvasViewport.LogicalViewportLocation);
        }
        static void OffsetCanvasOrigin(UIEventArgs e, PixelFarm.Drawing.Point p)
        {
            e.OffsetCanvasOrigin(p.X, p.Y);
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

            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            canvasViewport.FullMode = false;

            OffsetCanvasOrigin(focusEventArg, canvasViewport.LogicalViewportLocation);
            this.userEventPortal.PortalGotFocus(focusEventArg);
            eventStock.ReleaseEventArgs(focusEventArg);




        }
        public void HandleLostFocus(EventArgs e)
        {
            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            canvasViewport.FullMode = false;
            OffsetCanvasOrigin(focusEventArg, canvasViewport.LogicalViewportLocation);
            this.userEventPortal.PortalLostFocus(focusEventArg);
            eventStock.ReleaseEventArgs(focusEventArg);
        }
        UIMouseEventArgs GetReadyMouseEventArgs(MouseEventArgs e)
        {
            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs();
            SetUIMouseEventArgsInfo(mouseEventArg, e);

            return mouseEventArg;
        }
        void ReleaseMouseEvent(UIMouseEventArgs e)
        {
            eventStock.ReleaseEventArgs(e);
        }
        //------------------------------------------------------------------------
        public void HandleMouseDown(MouseEventArgs e)
        {
            //clear
            this.isMouseDown = true;
            this.isDragging = false;
            //---------------------

            canvasViewport.FullMode = false;
            UIMouseEventArgs mouseEventArg = GetReadyMouseEventArgs(e);
            this.userEventPortal.PortalMouseDown(mouseEventArg);
            if (currentCursorStyle != mouseEventArg.MouseCursorStyle)
            {
                //change cursor if need
                ChangeCursorStyle(mouseEventArg);
            }
            ReleaseMouseEvent(mouseEventArg);
            //----------- 

#if DEBUG
            RootGraphic visualroot = this.topwin.dbugVRoot;
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

            Point viewLocation = canvasViewport.LogicalViewportLocation;

            UIMouseEventArgs mouseEventArg = GetReadyMouseEventArgs(e);
            mouseEventArg.IsDragging = this.isDragging = this.isMouseDown;
            this.userEventPortal.PortalMouseMove(mouseEventArg);
            
         
            if (currentCursorStyle != mouseEventArg.MouseCursorStyle)
            {
                //change cursor if need
                ChangeCursorStyle(mouseEventArg);
            }
            ReleaseMouseEvent(mouseEventArg);

        }
        public void HandleMouseUp(MouseEventArgs e)
        {

            UIMouseEventArgs mouseEventArg = GetReadyMouseEventArgs(e);
            this.isMouseDown = this.isDragging = false;//after GetReadyMouseEventArgs *** 


            canvasViewport.FullMode = false;

            this.userEventPortal.PortalMouseUp(mouseEventArg);
            if (this.currentCursorStyle != mouseEventArg.MouseCursorStyle)
            {
                //change cursor if need
                ChangeCursorStyle(mouseEventArg);
            }

            ReleaseMouseEvent(mouseEventArg);

        }
        public void HandleMouseWheel(MouseEventArgs e)
        {

            UIMouseEventArgs mouseEventArg = GetReadyMouseEventArgs(e);
            canvasViewport.FullMode = true;
            this.userEventPortal.PortalMouseWheel(mouseEventArg);
            ReleaseMouseEvent(mouseEventArg);

        }
        protected abstract void ChangeCursorStyle(UIMouseEventArgs mouseEventArg);

        public void HandleKeyDown(KeyEventArgs e)
        {

            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();
            SetKeyData(keyEventArgs, e);

            StopCaretBlink();
            canvasViewport.FullMode = false;
            OffsetCanvasOrigin(keyEventArgs, canvasViewport.LogicalViewportLocation);

#if DEBUG

            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UI.UIKeys)e.KeyCode);
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif


            this.userEventPortal.PortalKeyDown(keyEventArgs);

            eventStock.ReleaseEventArgs(keyEventArgs);

        }
        void StartCaretBlink()
        {
            this.rootGraphic.CaretStartBlink();
        }
        void StopCaretBlink()
        {
            this.rootGraphic.CaretStopBlink();
        }
        public void HandleKeyUp(KeyEventArgs e)
        {


            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();
            SetKeyData(keyEventArgs, e);

            StopCaretBlink();
            canvasViewport.FullMode = false;

            OffsetCanvasOrigin(keyEventArgs, canvasViewport.LogicalViewportLocation);

            this.userEventPortal.PortalKeyUp(keyEventArgs);
            eventStock.ReleaseEventArgs(keyEventArgs);


            StartCaretBlink();

        }
        void SetKeyData(UIKeyEventArgs keyEventArgs, KeyEventArgs e)
        {
            keyEventArgs.SetEventInfo((int)e.KeyData, lastKeydownWithShift, lastKeydownWithAlt, lastKeydownWithControl);
        }
        public void HandleKeyPress(KeyPressEventArgs e)
        {

            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            UIKeyEventArgs keyPressEventArgs = eventStock.GetFreeKeyPressEventArgs();

            keyPressEventArgs.SetKeyChar(e.KeyChar);

            StopCaretBlink();
#if DEBUG
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYPRESS " + e.KeyChar);
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif

            canvasViewport.FullMode = false;

            OffsetCanvasOrigin(keyPressEventArgs, canvasViewport.LogicalViewportLocation);
            this.userEventPortal.PortalKeyPress(keyPressEventArgs);

            eventStock.ReleaseEventArgs(keyPressEventArgs);


        }

        public bool HandleProcessDialogKey(Keys keyData)
        {


            UIKeyEventArgs keyEventArg = eventStock.GetFreeKeyEventArgs();
            keyEventArg.KeyData = (int)keyData;
            keyEventArg.SetEventInfo(
                (int)keyData,
                this.lastKeydownWithShift = ((keyData & Keys.Shift) == Keys.Shift),
                this.lastKeydownWithAlt = ((keyData & Keys.Alt) == Keys.Alt),
                this.lastKeydownWithControl = ((keyData & Keys.Control) == Keys.Control));

            StopCaretBlink();
            canvasViewport.FullMode = false;

            OffsetCanvasOrigin(keyEventArg, canvasViewport.LogicalViewportLocation);
            bool result = this.userEventPortal.PortalProcessDialogKey(keyEventArg);
            eventStock.ReleaseEventArgs(keyEventArg);
            return result;
        }


    }



}
