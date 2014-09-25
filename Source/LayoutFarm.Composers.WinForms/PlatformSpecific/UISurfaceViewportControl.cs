//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;


using System.Text;
using System.Windows.Forms;

using LayoutFarm.Drawing;

namespace LayoutFarm
{




    public partial class UISurfaceViewportControl : UserControl, ISurfaceViewportControl
    {
        CanvasEventsStock eventStock = new CanvasEventsStock();
        CanvasViewport canvasViewport;
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

        public UISurfaceViewportControl()
        {
            InitializeComponent();
        }

        public void InitRootGraphics(int width, int height)
        {
            var visualRoot = new MyRootGraphic(width, height);
            var windowRoot = new MyTopWindowRenderBox(visualRoot, width, height);
            SetupWindowRoot(windowRoot);
        }

        public void WhenParentFormClosed(EventHandler<EventArgs> handler)
        {
            this.parentFormClosedHandler = handler;
            this.ParentForm.FormClosed += new FormClosedEventHandler(ParentForm_FormClosed);
        }
        void SetupWindowRoot(MyTopWindowRenderBox winroot)
        {

            this.wintop = winroot;
            canvasViewport = new CanvasViewport(this, winroot, this.Size.ToSize(), 4);
            //if request from winroot
            winroot.CanvasForcePaint += canvasViewport.PaintMe;

        }
        void ParentForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (parentFormClosedHandler != null)
            {
                parentFormClosedHandler(sender, e);
            }
        }
        internal void UpdateRootdocViewportSize()
        {
            canvasViewport.UpdateCanvasViewportSize(this.Width, this.Height);
        }

        public void Invoke(Delegate d, object o)
        {
            this.Invoke(d, o);
        }
        public int WindowWidth
        {
            get
            {
                return this.Width;
            }
        }

        public int WindowHeight
        {
            get
            {
                return this.Height;
            }
        }
        public TopWindowRenderBox WinTop
        {
            get
            {
                return wintop;
            }
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
            AddContent(ui.GetPrimaryRenderElement(wintop.RootGraphic));
        }
        public void Close()
        {
            canvasViewport.Close();
        }
        public void PaintMe()
        {
            canvasViewport.PaintMe(this, EventArgs.Empty);
        }
        public void ScrollBy(int dx, int dy)
        {
            canvasViewport.ScrollBy(dx, dy);
        }
        public void ScrollTo(int x, int y)
        {
            canvasViewport.ScrollTo(x, y);
        }
        public void viewport_HScrollChanged(object sender, UIScrollEventArgs e)
        {
            if (HScrollChanged != null)
            {
                HScrollChanged.Invoke(sender, e);
            }
        }
        public void viewport_HScrollRequest(object sender, ScrollSurfaceRequestEventArgs e)
        {
            if (HScrollRequest != null)
            {
                HScrollRequest.Invoke(sender, e);
            }
        }
        public void viewport_VScrollChanged(object sender, UIScrollEventArgs e)
        {


            if (VScrollChanged != null)
            {
                VScrollChanged.Invoke(sender, e);
            }
        }
        public void viewport_VScrollRequest(object sender, ScrollSurfaceRequestEventArgs e)
        {
            if (VScrollRequest != null)
            {
                VScrollRequest.Invoke(sender, e);
            }
        }



        protected override void OnGotFocus(EventArgs e)
        {
            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            canvasViewport.OnGotFocus(focusEventArg);
            base.OnGotFocus(e);
            eventStock.ReleaseEventArgs(focusEventArg);
        }
        protected override void OnLostFocus(EventArgs e)
        {

            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            canvasViewport.OnLostFocus(focusEventArg);
            base.OnLostFocus(e); eventStock.ReleaseEventArgs(focusEventArg);


        }
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            MouseEventArgs newMouseEventArgs = new MouseEventArgs(MouseButtons.Left, 1,
                lastestLogicalMouseDownX,
                lastestLogicalMouseDownY, 0);
            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.wintop);

            mouseEventArg.SetWinRoot(wintop);
            SetArtMouseEventArgsInfo(mouseEventArg, newMouseEventArgs);


            canvasViewport.OnDoubleClick(mouseEventArg);
            eventStock.ReleaseEventArgs(mouseEventArg);

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            MyRootGraphic.CurrentTopWindowRenderBox = this.wintop;

            isMouseDown = true;
            isDraging = false;

            Point viewLocation = canvasViewport.LogicalViewportLocation;
            lastestLogicalMouseDownX = (viewLocation.X + e.X);
            lastestLogicalMouseDownY = (viewLocation.Y + e.Y);

            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.wintop);
            mouseEventArg.SetWinRoot(wintop);
            SetArtMouseEventArgsInfo(mouseEventArg, e);

            base.OnMouseDown(e); canvasViewport.OnMouseDown(mouseEventArg);
            eventStock.ReleaseEventArgs(mouseEventArg);

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point viewLocation = canvasViewport.LogicalViewportLocation;

            if (isMouseDown)
            {
                int xdiff = (viewLocation.X + e.X) - prevLogicalMouseX; int ydiff = (viewLocation.Y + e.Y) - prevLogicalMouseY; if (!isDraging)
                {
                    UIDragEventArgs dragEventArg = UIDragEventArgs.GetFreeDragEventArgs();
                    dragEventArg.SetWinRoot(this.wintop);
                    dragEventArg.SetEventInfo(
                        e.Location.ToPoint(),
                        GetArtMouseButton(e.Button),
                        lastestLogicalMouseDownX, lastestLogicalMouseDownY,
                        (viewLocation.X + e.X), (viewLocation.Y + e.Y),
                        xdiff, ydiff);
                    canvasViewport.OnDragStart(dragEventArg);
                    isDraging = true;
                    UIDragEventArgs.ReleaseEventArgs(dragEventArg);
                }
                else
                {
                    if (!(xdiff == 0 && ydiff == 0))
                    {
                        UIDragEventArgs dragEventArg = UIDragEventArgs.GetFreeDragEventArgs();
                        dragEventArg.SetWinRoot(this.wintop);
                        dragEventArg.SetEventInfo(e.Location.ToPoint(),
                            GetArtMouseButton(e.Button),
                            lastestLogicalMouseDownX, lastestLogicalMouseDownY,
                            (viewLocation.X + e.X), (viewLocation.Y + e.Y),
                            xdiff, ydiff);
                        canvasViewport.OnDrag(dragEventArg);
                        UIDragEventArgs.ReleaseEventArgs(dragEventArg);
                    }
                }
            }
            else
            {
                UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.wintop);
                mouseEventArg.SetWinRoot(wintop);

                SetArtMouseEventArgsInfo(mouseEventArg, e);
                mouseEventArg.SetDiff(
                    (viewLocation.X + e.X) - prevLogicalMouseX, (viewLocation.Y + e.Y) - prevLogicalMouseY);
                canvasViewport.OnMouseMove(mouseEventArg);
                eventStock.ReleaseEventArgs(mouseEventArg);
            }
            prevLogicalMouseX = (viewLocation.X + e.X);
            prevLogicalMouseY = (viewLocation.Y + e.Y);

            base.OnMouseMove(e);

        }
        protected override void OnMouseUp(MouseEventArgs e)
        {

            isMouseDown = false; if (isDraging)
            {
                UIDragEventArgs mouseDragEventArg = UIDragEventArgs.GetFreeDragEventArgs();
                Point viewLocation = canvasViewport.LogicalViewportLocation;
                mouseDragEventArg.SetWinRoot(this.wintop);
                mouseDragEventArg.SetEventInfo(e.Location.ToPoint(),
                    GetArtMouseButton(e.Button),
                    lastestLogicalMouseDownX, lastestLogicalMouseDownY,
                    (viewLocation.X + e.X), (viewLocation.Y + e.Y),
                    (viewLocation.X + e.X) - lastestLogicalMouseDownX, (viewLocation.Y + e.Y) - lastestLogicalMouseDownY);
                base.OnMouseUp(e);
                canvasViewport.OnDragStop(mouseDragEventArg);
                UIDragEventArgs.ReleaseEventArgs(mouseDragEventArg);
            }
            else
            {
                UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.wintop);
                mouseEventArg.SetWinRoot(wintop);
                SetArtMouseEventArgsInfo(mouseEventArg, e);
                base.OnMouseUp(e);
                canvasViewport.OnMouseUp(mouseEventArg);
                eventStock.ReleaseEventArgs(mouseEventArg);
            }
        }
        static UIMouseButtons GetArtMouseButton(MouseButtons mouseButton)
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
        static void SetArtMouseEventArgsInfo(UIMouseEventArgs mouseEventArg, MouseEventArgs e)
        {
            mouseEventArg.SetEventInfo(e.Location.ToPoint(),
                GetArtMouseButton(e.Button), e.Clicks, e.Delta);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (canvasViewport != null)
            {
                canvasViewport.SetFullMode(true);
                canvasViewport.PaintMe(this, e);
            }
            base.OnPaint(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.wintop);

            mouseEventArg.SetWinRoot(wintop);
            SetArtMouseEventArgsInfo(mouseEventArg, e);

            base.OnMouseWheel(e);
            canvasViewport.OnMouseWheel(mouseEventArg);
            eventStock.ReleaseEventArgs(mouseEventArg);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            MyRootGraphic.CurrentTopWindowRenderBox = this.wintop;

            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();
            keyEventArgs.SetWinRoot(wintop);
            SetArtKeyData(keyEventArgs, e);
            base.OnKeyDown(e);

            canvasViewport.OnKeyDown(keyEventArgs);
            eventStock.ReleaseEventArgs(keyEventArgs);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            MyRootGraphic.CurrentTopWindowRenderBox = this.wintop;

            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();

            keyEventArgs.SetWinRoot(wintop);
            SetArtKeyData(keyEventArgs, e);
            base.OnKeyUp(e);

            canvasViewport.OnKeyUp(keyEventArgs);
            eventStock.ReleaseEventArgs(keyEventArgs);
        }
        static void SetArtKeyData(UIKeyEventArgs keyEventArgs, KeyEventArgs e)
        {
            keyEventArgs.SetEventInfo((int)e.KeyCode, e.Shift, e.Alt, e.Control);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            UIKeyPressEventArgs keyPressEventArgs = eventStock.GetFreeKeyPressEventArgs();

            keyPressEventArgs.SetWinRoot(wintop);
            keyPressEventArgs.SetKeyChar(e.KeyChar);

            base.OnKeyPress(e);
            canvasViewport.OnKeyPress(keyPressEventArgs);

            eventStock.ReleaseEventArgs(keyPressEventArgs);
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {

            UIKeyEventArgs keyEventArg = eventStock.GetFreeKeyEventArgs();
            keyEventArg.SetWinRoot(wintop);
            keyEventArg.KeyData = (int)keyData;
            if (canvasViewport.OnProcessDialogKey(keyEventArg))
            {
                eventStock.ReleaseEventArgs(keyEventArg);
                return true;
            }
            eventStock.ReleaseEventArgs(keyEventArg); return base.ProcessDialogKey(keyData);
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
        public void dbug_HighlightMeNow(Rectangle rect)
        {

            using (System.Drawing.Pen mpen = new System.Drawing.Pen(System.Drawing.Brushes.White, 2))
            using (System.Drawing.Graphics g = this.CreateGraphics())
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
