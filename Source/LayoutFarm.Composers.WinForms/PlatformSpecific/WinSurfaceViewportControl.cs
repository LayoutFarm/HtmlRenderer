//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using LayoutFarm.Drawing;

namespace LayoutFarm
{

    class WinSurfaceViewportControl : ISurfaceViewportControl
    {
        CanvasEventsStock eventStock = new CanvasEventsStock();
        CanvasViewport canvasViewport;
        LayoutFarm.TopBoxInputEventBridge topBoxInputEventBridge = new TopBoxInputEventBridge();
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

        MyTopWindowRenderBox wintop;
        EventHandler<EventArgs> parentFormClosedHandler;
        Control windowControl;

        public WinSurfaceViewportControl()
        {
        }

        public void BindWindowControl(Control windowControl)
        {
            this.windowControl = windowControl;
        }
        public void BindCanvasViewPort(CanvasViewport canvasViewport)
        {
            this.canvasViewport = canvasViewport;
        }
        public void BindWinTop(MyTopWindowRenderBox wintop)
        {
            //create view port ?
            this.wintop = wintop;
            // canvasViewport = new CanvasViewport(this.windowControl, wintop, this.Size.ToSize(), 4);
            wintop.CanvasForcePaint += (s, e) =>
            {
                canvasViewport.PaintMe();
            };
            topBoxInputEventBridge.Bind(wintop);
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

        public void Invoke(Delegate d, object o)
        {
            this.Invoke(d, o);
        }
        public void Close()
        {
            canvasViewport.Close();
        }
        public void PaintMe()
        {
            canvasViewport.PaintMe();
        }
        public void ScrollBy(int dx, int dy)
        {
            canvasViewport.ScrollBy(dx, dy);
        }
        public void ScrollTo(int x, int y)
        {
            canvasViewport.ScrollTo(x, y);
        }

        void ISurfaceViewportControl.viewport_HScrollChanged(object sender, UIScrollEventArgs e)
        {
            if (HScrollChanged != null)
            {
                HScrollChanged.Invoke(sender, e);
            }
        }
        void ISurfaceViewportControl.viewport_HScrollRequest(object sender, ScrollSurfaceRequestEventArgs e)
        {
            if (HScrollRequest != null)
            {
                HScrollRequest.Invoke(sender, e);
            }
        }
        void ISurfaceViewportControl.viewport_VScrollChanged(object sender, UIScrollEventArgs e)
        {
            if (VScrollChanged != null)
            {
                VScrollChanged.Invoke(sender, e);
            }
        }
        void ISurfaceViewportControl.viewport_VScrollRequest(object sender, ScrollSurfaceRequestEventArgs e)
        {
            if (VScrollRequest != null)
            {
                VScrollRequest.Invoke(sender, e);
            }
        }
        IntPtr ISurfaceViewportControl.Handle
        {
            get
            {
                return this.windowControl.Handle;
            }
        }
        public void OnGotFocus(EventArgs e)
        {
            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            
            canvasViewport.FullMode = false;

            focusEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            topBoxInputEventBridge.OnGotFocus(focusEventArg);
            canvasViewport.PaintIfNeed();

            eventStock.ReleaseEventArgs(focusEventArg);
        }
        public void OnLostFocus(EventArgs e)
        {
            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            canvasViewport.FullMode = false; 
            focusEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            topBoxInputEventBridge.OnLostFocus(focusEventArg);
            eventStock.ReleaseEventArgs(focusEventArg);
        }
        public void OnDoubleClick(EventArgs e)
        {
            MouseEventArgs newMouseEventArgs = new MouseEventArgs(MouseButtons.Left, 1,
                lastestLogicalMouseDownX,
                lastestLogicalMouseDownY, 0);

            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.wintop);
            SetUIMouseEventArgsInfo(mouseEventArg, newMouseEventArgs);
            canvasViewport.FullMode = false;
            topBoxInputEventBridge.OnDoubleClick(mouseEventArg);

            canvasViewport.PaintIfNeed();

            eventStock.ReleaseEventArgs(mouseEventArg);
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            MyRootGraphic.CurrentTopWindowRenderBox = this.wintop;

            isMouseDown = true;
            isDraging = false;

            Point viewLocation = canvasViewport.LogicalViewportLocation;
            lastestLogicalMouseDownX = (viewLocation.X + e.X);
            lastestLogicalMouseDownY = (viewLocation.Y + e.Y);


            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.wintop);
            SetUIMouseEventArgsInfo(mouseEventArg, e);
            canvasViewport.FullMode = false;

            topBoxInputEventBridge.OnMouseDown(mouseEventArg);

            this.canvasViewport.PaintIfNeed();
            //---------------
#if DEBUG
            RootGraphic visualroot = this.wintop.dbugVRoot;
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
                    this.topBoxInputEventBridge.OnDragStart(dragEventArg);
                    canvasViewport.PaintIfNeed();

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
                        topBoxInputEventBridge.OnDrag(dragEventArg);
                        canvasViewport.PaintIfNeed();


                        eventStock.ReleaseEventArgs(dragEventArg);
                    }
                }
            }
            else
            {
                UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.wintop);


                SetUIMouseEventArgsInfo(mouseEventArg, e);
                mouseEventArg.SetDiff(
                    (viewLocation.X + e.X) - prevLogicalMouseX, (viewLocation.Y + e.Y) - prevLogicalMouseY);

                mouseEventArg.OffsetCanvasOrigin(viewLocation);
                topBoxInputEventBridge.OnMouseMove(mouseEventArg);

                canvasViewport.PaintIfNeed();

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
                topBoxInputEventBridge.OnDragStop(mouseDragEventArg);
                canvasViewport.PaintIfNeed();

                eventStock.ReleaseEventArgs(mouseDragEventArg);

            }
            else
            {

                UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.wintop);

                SetUIMouseEventArgsInfo(mouseEventArg, e);
                canvasViewport.FullMode = false;
                mouseEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
                topBoxInputEventBridge.OnMouseUp(mouseEventArg);
                canvasViewport.PaintIfNeed();

                eventStock.ReleaseEventArgs(mouseEventArg);
            }
        }

        public void OnPaint(PaintEventArgs e)
        {
            if (canvasViewport != null)
            {
                //temp ? for debug
                canvasViewport.FullMode = true;
                canvasViewport.PaintMe();
            }
        }

        public void OnMouseWheel(MouseEventArgs e)
        {

            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.wintop);
     

            SetUIMouseEventArgsInfo(mouseEventArg, e);
            canvasViewport.FullMode = true;

            mouseEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            topBoxInputEventBridge.OnMouseWheel(mouseEventArg);
            canvasViewport.PaintIfNeed();

            eventStock.ReleaseEventArgs(mouseEventArg);
        }
        public void OnKeyDown(KeyEventArgs e)
        {

            MyRootGraphic.CurrentTopWindowRenderBox = this.wintop;

            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();

            SetKeyData(keyEventArgs, e);


            this.wintop.MyVisualRoot.TempStopCaret();
            canvasViewport.FullMode = false; 
            keyEventArgs.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
#if DEBUG
            wintop.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            wintop.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UIKeys)e.KeyData);
            wintop.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif

            topBoxInputEventBridge.OnKeyDown(keyEventArgs);
            canvasViewport.PaintIfNeed();
            eventStock.ReleaseEventArgs(keyEventArgs);
        }
        public void OnKeyUp(KeyEventArgs e)
        {
            MyRootGraphic.CurrentTopWindowRenderBox = this.wintop;
            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();
            SetKeyData(keyEventArgs, e);
 
            wintop.MyVisualRoot.TempStopCaret();

            canvasViewport.FullMode = false;
            keyEventArgs.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            topBoxInputEventBridge.OnKeyUp(keyEventArgs);


            wintop.MyVisualRoot.TempRunCaret();

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

            wintop.MyVisualRoot.TempStopCaret();
#if DEBUG
            wintop.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            wintop.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYPRESS " + e.KeyChar);
            wintop.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif

            canvasViewport.FullMode = false;
            keyPressEventArgs.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            topBoxInputEventBridge.OnKeyPress(keyPressEventArgs);
            canvasViewport.PaintIfNeed();


            eventStock.ReleaseEventArgs(keyPressEventArgs);
        }
        public bool ProcessDialogKey(Keys keyData)
        {

            UIKeyEventArgs keyEventArg = eventStock.GetFreeKeyEventArgs();

            keyEventArg.KeyData = (int)keyData;
             
            wintop.MyVisualRoot.TempStopCaret();
            canvasViewport.FullMode = false;

            keyEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            bool result = topBoxInputEventBridge.OnProcessDialogKey(keyEventArg);
            eventStock.ReleaseEventArgs(keyEventArg);

            if (result)
            {
                canvasViewport.PaintIfNeed();
                return true;
            }
            return false;
        }

#if DEBUG
        public event EventHandler dbug_VisualRootDrawMsg;
        public event EventHandler dbug_VisualRootHitChainMsg;
        List<dbugLayoutMsg> dbugrootDocDebugMsgs = new List<dbugLayoutMsg>();
        List<dbugLayoutMsg> dbugrootDocHitChainMsgs = new List<dbugLayoutMsg>();
        public List<dbugLayoutMsg> dbug_rootDocDebugMsgs
        {
            get
            {
                return this.dbugrootDocDebugMsgs;
            }
        }
        public List<dbugLayoutMsg> dbug_rootDocHitChainMsgs
        {
            get
            {
                return this.dbugrootDocHitChainMsgs;
            }
        }
        System.Drawing.Graphics dbugCreateGraphics()
        {
            return this.windowControl.CreateGraphics();
        }
        public void dbug_HighlightMeNow(Rectangle rect)
        {

            using (System.Drawing.Pen mpen = new System.Drawing.Pen(System.Drawing.Brushes.White, 2))
            using (System.Drawing.Graphics g = this.dbugCreateGraphics())
            {

                System.Drawing.Rectangle r = rect.ToRect();
                g.DrawRectangle(mpen, r);
                g.DrawLine(mpen, new System.Drawing.Point(r.X, r.Y), new System.Drawing.Point(r.Right, r.Bottom));
                g.DrawLine(mpen, new System.Drawing.Point(r.X, r.Bottom), new System.Drawing.Point(r.Right, r.Y));
            }

        }
        public void dbug_InvokeVisualRootDrawMsg()
        {
            if (dbug_VisualRootDrawMsg != null)
            {
                dbug_VisualRootDrawMsg(this, EventArgs.Empty);
            }
        }
        public void dbug_InvokeHitChainMsg()
        {
            if (dbug_VisualRootHitChainMsg != null)
            {
                dbug_VisualRootHitChainMsg(this, EventArgs.Empty);
            }
        }
        public void dbug_BeginLayoutTraceSession(string beginMsg)
        {
            this.wintop.dbugVisualRoot.dbug_BeginLayoutTraceSession(beginMsg);
        }
        public void dbug_DisableAllDebugInfo()
        {
            this.wintop.dbugVisualRoot.dbug_DisableAllDebugInfo();
        }
        public void dbug_EnableAllDebugInfo()
        {
            this.wintop.dbugVisualRoot.dbug_EnableAllDebugInfo();
        }
        public void dbug_ReArrangeWithBreakOnSelectedNode()
        {

            vinv_dbugBreakOnSelectedVisuallElement = true;
            this.wintop.TopDownReArrangeContentIfNeed();

        }
        protected bool vinv_dbugBreakOnSelectedVisuallElement
        {
            get;
            set;
        }
#endif

    }



}
