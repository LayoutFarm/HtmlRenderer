//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using LayoutFarm.Drawing;

namespace LayoutFarm
{

    partial class WinViewportBridge 
    {
        CanvasEventsStock eventStock = new CanvasEventsStock();
        CanvasViewport canvasViewport;        
        UserInputEventBridge userInputEventBridge = new UserInputEventBridge();

        bool isMouseDown = false;
        bool isDraging = false;

        int prevLogicalMouseX = 0;
        int prevLogicalMouseY = 0;
        int lastestLogicalMouseDownX = 0;
        int lastestLogicalMouseDownY = 0;

        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;

        MyTopWindowRenderBox topwin;
        EventHandler<EventArgs> parentFormClosedHandler;
        Control windowControl;

        public WinViewportBridge(MyTopWindowRenderBox wintop)
        {
            //create view port ?
            this.topwin = wintop;
            wintop.CanvasForcePaint += (s, e) =>
            {
                PaintToOutputWindow();
            };
            userInputEventBridge.Bind(wintop);
        }
        void PaintToOutputWindow()
        {
            IntPtr hdc = GetDC(this.windowControl.Handle);
            canvasViewport.PaintMe(hdc);
            ReleaseDC(this.windowControl.Handle, hdc);
        }
        void PaintToOutputWindowIfNeed()
        {
            if (!this.canvasViewport.IsQuadPageValid)
            {
                IntPtr hdc = GetDC(this.windowControl.Handle);
                canvasViewport.PaintMe(hdc);
                ReleaseDC(this.windowControl.Handle, hdc);
            }
        }
        public void BindWindowControl(Control windowControl)
        {
            this.windowControl = windowControl;
            this.canvasViewport = new CanvasViewport(topwin, this.Size.ToSize(), 4);
#if DEBUG
            this.canvasViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        public void UpdateCanvasViewportSize(int w, int h)
        {
            this.canvasViewport.UpdateCanvasViewportSize(w, h);
        }


        System.Drawing.Size Size
        {
            get { return this.windowControl.Size; }
        }
        int Width
        {
            get { return this.windowControl.Width; }
        }
        int Height
        {
            get { return this.windowControl.Height; }
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
                e.Location.ToPoint(),
                GetUIMouseButton(e.Button),
                e.Clicks,
                e.Delta);

            mouseEventArg.OffsetCanvasOrigin(this.canvasViewport.LogicalViewportLocation);

        }

        public void Close()
        {
            canvasViewport.Close();
        }


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

       
        public void OnGotFocus(EventArgs e)
        {
            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);

            canvasViewport.FullMode = false;

            focusEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            userInputEventBridge.OnGotFocus(focusEventArg);
            PaintToOutputWindowIfNeed();

            eventStock.ReleaseEventArgs(focusEventArg);
        }
        public void OnLostFocus(EventArgs e)
        {
            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            canvasViewport.FullMode = false;
            focusEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            userInputEventBridge.OnLostFocus(focusEventArg);
            eventStock.ReleaseEventArgs(focusEventArg);
        }
        public void OnDoubleClick(EventArgs e)
        {
            MouseEventArgs newMouseEventArgs = new MouseEventArgs(MouseButtons.Left, 1,
                lastestLogicalMouseDownX,
                lastestLogicalMouseDownY, 0);

            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.topwin);
            SetUIMouseEventArgsInfo(mouseEventArg, newMouseEventArgs);
            canvasViewport.FullMode = false;
            userInputEventBridge.OnDoubleClick(mouseEventArg);

            PaintToOutputWindowIfNeed();

            eventStock.ReleaseEventArgs(mouseEventArg);
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            MyRootGraphic.CurrentTopWindowRenderBox = this.topwin;

            isMouseDown = true;
            isDraging = false;

            Point viewLocation = canvasViewport.LogicalViewportLocation;
            lastestLogicalMouseDownX = (viewLocation.X + e.X);
            lastestLogicalMouseDownY = (viewLocation.Y + e.Y);


            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.topwin);
            SetUIMouseEventArgsInfo(mouseEventArg, e);
            canvasViewport.FullMode = false;

            userInputEventBridge.OnMouseDown(mouseEventArg);

            PaintToOutputWindowIfNeed();
            //---------------
#if DEBUG
            RootGraphic visualroot = this.topwin.dbugVRoot;
            if (visualroot.dbug_RecordHitChain)
            {
                dbug_rootDocHitChainMsgs.Clear();
                visualroot.dbug_DumpCurrentHitChain(dbug_rootDocHitChainMsgs);
                dbug_InvokeHitChainMsg();
            }
#endif
            //----------- 
            eventStock.ReleaseEventArgs(mouseEventArg);
        }
        public void OnMouseMove(MouseEventArgs e)
        {
            Point viewLocation = canvasViewport.LogicalViewportLocation;

            if (isMouseDown)
            {
                int xdiff = (viewLocation.X + e.X) - prevLogicalMouseX;
                int ydiff = (viewLocation.Y + e.Y) - prevLogicalMouseY;
                if (!isDraging)
                {
                    UIDragEventArgs dragEventArg = eventStock.GetFreeDragEventArgs(
                        e.Location.ToPoint(),
                        GetUIMouseButton(e.Button),
                        lastestLogicalMouseDownX, lastestLogicalMouseDownY,
                        (viewLocation.X + e.X), (viewLocation.Y + e.Y),
                        xdiff, ydiff);

                    dragEventArg.OffsetCanvasOrigin(viewLocation);
                    canvasViewport.FullMode = false;
                    this.userInputEventBridge.OnDragStart(dragEventArg);
                    PaintToOutputWindowIfNeed();

                    isDraging = true;
                    eventStock.ReleaseEventArgs(dragEventArg);

                }
                else
                {
                    if (!(xdiff == 0 && ydiff == 0))
                    {
                        UIDragEventArgs dragEventArg = eventStock.GetFreeDragEventArgs(e.Location.ToPoint(),
                            GetUIMouseButton(e.Button),
                            lastestLogicalMouseDownX, lastestLogicalMouseDownY,
                            (viewLocation.X + e.X), (viewLocation.Y + e.Y),
                            xdiff, ydiff);

                        canvasViewport.FullMode = false;
                        dragEventArg.OffsetCanvasOrigin(viewLocation);
                        userInputEventBridge.OnDrag(dragEventArg);
                        PaintToOutputWindowIfNeed();


                        eventStock.ReleaseEventArgs(dragEventArg);
                    }
                }
            }
            else
            {
                UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.topwin);


                SetUIMouseEventArgsInfo(mouseEventArg, e);
                mouseEventArg.SetDiff(
                    (viewLocation.X + e.X) - prevLogicalMouseX, (viewLocation.Y + e.Y) - prevLogicalMouseY);

                mouseEventArg.OffsetCanvasOrigin(viewLocation);
                userInputEventBridge.OnMouseMove(mouseEventArg);

                PaintToOutputWindowIfNeed();

                eventStock.ReleaseEventArgs(mouseEventArg);
            }
            prevLogicalMouseX = (viewLocation.X + e.X);
            prevLogicalMouseY = (viewLocation.Y + e.Y); 
        }
        public void OnMouseUp(MouseEventArgs e)
        {

            isMouseDown = false;

            if (isDraging)
            {


                Point viewLocation = canvasViewport.LogicalViewportLocation;
                var mouseDragEventArg = eventStock.GetFreeDragEventArgs(
                   e.Location.ToPoint(),
                   GetUIMouseButton(e.Button),
                   lastestLogicalMouseDownX, lastestLogicalMouseDownY,
                   (viewLocation.X + e.X), (viewLocation.Y + e.Y),
                   (viewLocation.X + e.X) - lastestLogicalMouseDownX, (viewLocation.Y + e.Y) - lastestLogicalMouseDownY);

                canvasViewport.FullMode = false;
                mouseDragEventArg.OffsetCanvasOrigin(viewLocation);
                userInputEventBridge.OnDragStop(mouseDragEventArg);
                PaintToOutputWindowIfNeed();

                eventStock.ReleaseEventArgs(mouseDragEventArg);

            }
            else
            {

                UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.topwin);

                SetUIMouseEventArgsInfo(mouseEventArg, e);
                canvasViewport.FullMode = false;
                mouseEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
                userInputEventBridge.OnMouseUp(mouseEventArg);
                 
                PaintToOutputWindowIfNeed();
                eventStock.ReleaseEventArgs(mouseEventArg);
            }
        }
        public void PaintMe()
        {
            if (canvasViewport != null)
            {
                //temp ? for debug
                canvasViewport.FullMode = true;
                PaintToOutputWindow();
            }
        }

        public void PaintMe(PaintEventArgs e)
        {
            PaintMe();
        }

        public void OnMouseWheel(MouseEventArgs e)
        {

            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.topwin);
            SetUIMouseEventArgsInfo(mouseEventArg, e);
            canvasViewport.FullMode = true;

            mouseEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            userInputEventBridge.OnMouseWheel(mouseEventArg);
            PaintToOutputWindowIfNeed();

            eventStock.ReleaseEventArgs(mouseEventArg);
        }
        public void OnKeyDown(KeyEventArgs e)
        {

            MyRootGraphic.CurrentTopWindowRenderBox = this.topwin;

            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();

            SetKeyData(keyEventArgs, e);


            this.topwin.MyVisualRoot.TempStopCaret();
            canvasViewport.FullMode = false;
            keyEventArgs.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
#if DEBUG
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UIKeys)e.KeyData);
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif

            userInputEventBridge.OnKeyDown(keyEventArgs);
            PaintToOutputWindowIfNeed();
            eventStock.ReleaseEventArgs(keyEventArgs);
        }
        public void OnKeyUp(KeyEventArgs e)
        {
            MyRootGraphic.CurrentTopWindowRenderBox = this.topwin;
            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();
            SetKeyData(keyEventArgs, e);

            topwin.MyVisualRoot.TempStopCaret();

            canvasViewport.FullMode = false;
            keyEventArgs.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            userInputEventBridge.OnKeyUp(keyEventArgs);


            topwin.MyVisualRoot.TempRunCaret();

            eventStock.ReleaseEventArgs(keyEventArgs);
        }
        static void SetKeyData(UIKeyEventArgs keyEventArgs, KeyEventArgs e)
        {
            keyEventArgs.SetEventInfo((int)e.KeyCode, e.Shift, e.Alt, e.Control);
        }
        public void OnKeyPress(KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }


            UIKeyPressEventArgs keyPressEventArgs = eventStock.GetFreeKeyPressEventArgs();
            keyPressEventArgs.SetKeyChar(e.KeyChar);

            topwin.MyVisualRoot.TempStopCaret();
#if DEBUG
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYPRESS " + e.KeyChar);
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif

            canvasViewport.FullMode = false;
            keyPressEventArgs.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            userInputEventBridge.OnKeyPress(keyPressEventArgs);
            PaintToOutputWindowIfNeed();


            eventStock.ReleaseEventArgs(keyPressEventArgs);
        }
        public bool ProcessDialogKey(Keys keyData)
        {

            UIKeyEventArgs keyEventArg = eventStock.GetFreeKeyEventArgs();

            keyEventArg.KeyData = (int)keyData;

            topwin.MyVisualRoot.TempStopCaret();
            canvasViewport.FullMode = false;

            keyEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            bool result = userInputEventBridge.OnProcessDialogKey(keyEventArg);
            eventStock.ReleaseEventArgs(keyEventArg);

            if (result)
            {
                PaintToOutputWindowIfNeed();
                return true;
            }
            return false;
        }



        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hdc);

    }



}
