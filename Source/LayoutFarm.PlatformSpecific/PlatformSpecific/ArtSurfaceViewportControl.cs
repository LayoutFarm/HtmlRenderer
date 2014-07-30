//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using System.Text;
using System.Windows.Forms;



namespace LayoutFarm.Presentation
{




    public partial class ArtSurfaceViewportControl : UserControl, IArtSurfaceViewportControl
    {
        CanvasEventsStock eventStock = new CanvasEventsStock();
        CanvasViewport viewport;
        bool isMouseDown = false;
        bool isDraging = false;

        int prevLogicalMouseX = 0;
        int prevLogicalMouseY = 0;
        int lastestLogicalMouseDownX = 0;
        int lastestLogicalMouseDownY = 0;

        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<ArtScrollEventArgs> VScrollChanged;
        public event EventHandler<ArtScrollEventArgs> HScrollChanged;



        ArtVisualWindowImpl winroot;

        EventHandler<EventArgs> parentFormClosedHandler;

        public ArtSurfaceViewportControl()
        {
            InitializeComponent();
        }

        public void WhenParentFormClosed(EventHandler<EventArgs> handler)
        {
            this.parentFormClosedHandler = handler;
            this.ParentForm.FormClosed += new FormClosedEventHandler(ParentForm_FormClosed);
        }
        public void SetupWindowRoot(ArtVisualWindowImpl winroot)
        {

            this.winroot = winroot;
            viewport = new CanvasViewport(this, winroot, this.Size, 4);
            winroot.CanvasForcePaintMe += viewport.PaintMe;

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
            viewport.UpdateCanvasViewportSize(this.Width, this.Height);
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


        public ArtVisualRootWindow WinRoot
        {
            get
            {
                return winroot;
            }
        }

        public void AddContent(ArtVisualElement vi)
        {
            winroot.AddChild(vi);
        }
        public void Close()
        {
            viewport.Close();
        }
        public void PaintMe()
        {
            viewport.PaintMe(this, EventArgs.Empty);
        }
        public void ScrollBy(int dx, int dy)
        {
            viewport.ScrollBy(dx, dy);
        }
        public void ScrollTo(int x, int y)
        {
            viewport.ScrollTo(x, y);
        }
        public void viewport_HScrollChanged(object sender, ArtScrollEventArgs e)
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
        public void viewport_VScrollChanged(object sender, ArtScrollEventArgs e)
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
            ArtFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            viewport.OnGotFocus(focusEventArg);
            base.OnGotFocus(e);
            eventStock.ReleaseEventArgs(focusEventArg);
        }
        protected override void OnLostFocus(EventArgs e)
        {

            ArtFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            viewport.OnLostFocus(focusEventArg);
            base.OnLostFocus(e); eventStock.ReleaseEventArgs(focusEventArg);


        }
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            MouseEventArgs newMouseEventArgs = new MouseEventArgs(MouseButtons.Left, 1,
                lastestLogicalMouseDownX,
                lastestLogicalMouseDownY, 0);
            ArtMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs();

            mouseEventArg.SetWinRoot(winroot);
            SetArtMouseEventArgsInfo(mouseEventArg, newMouseEventArgs);


            viewport.OnDoubleClick(mouseEventArg);
            eventStock.ReleaseEventArgs(mouseEventArg);

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            isMouseDown = true;
            isDraging = false;

            Point viewLocation = viewport.LogicalViewportLocation;
            lastestLogicalMouseDownX = (viewLocation.X + e.X);
            lastestLogicalMouseDownY = (viewLocation.Y + e.Y);

            ArtMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs();
            mouseEventArg.SetWinRoot(winroot);
            SetArtMouseEventArgsInfo(mouseEventArg, e);

            base.OnMouseDown(e); viewport.OnMouseDown(mouseEventArg);
            eventStock.ReleaseEventArgs(mouseEventArg);

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point viewLocation = viewport.LogicalViewportLocation;

            if (isMouseDown)
            {
                int xdiff = (viewLocation.X + e.X) - prevLogicalMouseX; int ydiff = (viewLocation.Y + e.Y) - prevLogicalMouseY; if (!isDraging)
                {
                    ArtDragEventArgs dragEventArg = ArtDragEventArgs.GetFreeDragEventArgs(); dragEventArg.SetWinRoot(this.winroot);
                    dragEventArg.SetEventInfo(e.Location, GetArtMouseButton(e.Button),
                        lastestLogicalMouseDownX, lastestLogicalMouseDownY,
                        (viewLocation.X + e.X), (viewLocation.Y + e.Y),
                        xdiff, ydiff); viewport.OnDragStart(dragEventArg);
                    isDraging = true; ArtDragEventArgs.ReleaseEventArgs(dragEventArg);
                }
                else
                {
                    if (!(xdiff == 0 && ydiff == 0))
                    {
                        ArtDragEventArgs dragEventArg = ArtDragEventArgs.GetFreeDragEventArgs(); dragEventArg.SetWinRoot(this.winroot);
                        dragEventArg.SetEventInfo(e.Location, GetArtMouseButton(e.Button),
                            lastestLogicalMouseDownX, lastestLogicalMouseDownY,
                            (viewLocation.X + e.X), (viewLocation.Y + e.Y),
                            xdiff, ydiff); viewport.OnDrag(dragEventArg);
                        ArtDragEventArgs.ReleaseEventArgs(dragEventArg);
                    }
                }
            }
            else
            {
                ArtMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs();
                mouseEventArg.SetWinRoot(winroot);

                SetArtMouseEventArgsInfo(mouseEventArg, e);
                mouseEventArg.SetDiff(
                    (viewLocation.X + e.X) - prevLogicalMouseX, (viewLocation.Y + e.Y) - prevLogicalMouseY);
                viewport.OnMouseMove(mouseEventArg);
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
                ArtDragEventArgs mouseDragEventArg = ArtDragEventArgs.GetFreeDragEventArgs();
                Point viewLocation = viewport.LogicalViewportLocation;
                mouseDragEventArg.SetWinRoot(this.winroot);
                mouseDragEventArg.SetEventInfo(e.Location, GetArtMouseButton(e.Button),
                    lastestLogicalMouseDownX, lastestLogicalMouseDownY,
                    (viewLocation.X + e.X), (viewLocation.Y + e.Y),
                    (viewLocation.X + e.X) - lastestLogicalMouseDownX, (viewLocation.Y + e.Y) - lastestLogicalMouseDownY);
                base.OnMouseUp(e); viewport.OnDragStop(mouseDragEventArg);
                ArtDragEventArgs.ReleaseEventArgs(mouseDragEventArg);
            }
            else
            {
                ArtMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs();
                mouseEventArg.SetWinRoot(winroot);
                SetArtMouseEventArgsInfo(mouseEventArg, e);
                base.OnMouseUp(e); viewport.OnMouseUp(mouseEventArg);
                eventStock.ReleaseEventArgs(mouseEventArg);
            }
        }
        static ArtMouseButtons GetArtMouseButton(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.Right:
                    return ArtMouseButtons.Right;
                case MouseButtons.Middle:
                    return ArtMouseButtons.Middle;
                case MouseButtons.None:
                    return ArtMouseButtons.None;
                default:
                    return ArtMouseButtons.Left;
            }
        }
        static void SetArtMouseEventArgsInfo(ArtMouseEventArgs mouseEventArg, MouseEventArgs e)
        {
            mouseEventArg.SetEventInfo(e.Location, GetArtMouseButton(e.Button), e.Clicks, e.Delta);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (viewport != null)
            {
                viewport.SetFullMode(true);
                viewport.PaintMe(this, e);
            }
            base.OnPaint(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            ArtMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs();

            mouseEventArg.SetWinRoot(winroot);
            SetArtMouseEventArgsInfo(mouseEventArg, e);

            base.OnMouseWheel(e);
            viewport.OnMouseWheel(mouseEventArg);
            eventStock.ReleaseEventArgs(mouseEventArg);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            ArtKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();
            keyEventArgs.SetWinRoot(winroot);
            SetArtKeyData(keyEventArgs, e);
            base.OnKeyDown(e);
            viewport.OnKeyDown(keyEventArgs);
            eventStock.ReleaseEventArgs(keyEventArgs);

        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            ArtKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();

            keyEventArgs.SetWinRoot(winroot);
            SetArtKeyData(keyEventArgs, e);
            base.OnKeyUp(e);

            viewport.OnKeyUp(keyEventArgs);
            eventStock.ReleaseEventArgs(keyEventArgs);
        }
        static void SetArtKeyData(ArtKeyEventArgs keyEventArgs, KeyEventArgs e)
        {
            keyEventArgs.SetEventInfo((int)e.KeyCode, e.Shift, e.Alt, e.Control);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            ArtKeyPressEventArgs keyPressEventArgs = eventStock.GetFreeKeyPressEventArgs();

            keyPressEventArgs.SetWinRoot(winroot);
            keyPressEventArgs.SetKeyChar(e.KeyChar);

            base.OnKeyPress(e);
            viewport.OnKeyPress(keyPressEventArgs);

            eventStock.ReleaseEventArgs(keyPressEventArgs);
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {

            ArtKeyEventArgs keyEventArg = eventStock.GetFreeKeyEventArgs();
            keyEventArg.SetWinRoot(winroot);
            keyEventArg.KeyData = (int)keyData;
            if (viewport.OnProcessDialogKey(keyEventArg))
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
        public void dbug_HighlightMeNow(Rectangle r)
        {

            Pen mpen = new Pen(Brushes.White, 2);
            Graphics g = this.CreateGraphics();
            g.DrawRectangle(mpen, r);
            g.DrawLine(mpen, new Point(r.X, r.Y), new Point(r.Right, r.Bottom));
            g.DrawLine(mpen, new Point(r.X, r.Bottom), new Point(r.Right, r.Y));
            g.Dispose();
            mpen.Dispose();
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
            this.winroot.VisualRoot.dbug_BeginLayoutTraceSession(beginMsg);
        }
        public void dbug_DisableAllDebugInfo()
        {
            this.winroot.VisualRoot.dbug_DisableAllDebugInfo();
        }
        public void dbug_EnableAllDebugInfo()
        {
            this.winroot.VisualRoot.dbug_EnableAllDebugInfo();
        }
        public void dbug_ReArrangeWithBreakOnSelectedNode()
        {
            var vinv = this.winroot.GetVInv();
            vinv.ForceReArrange = true;
            vinv.dbugBreakOnSelectedVisuallElement = true;
            this.winroot.TopDownReArrangeContentIfNeed(vinv);
            this.winroot.FreeVInv(vinv);
        }

#endif

    }



}
