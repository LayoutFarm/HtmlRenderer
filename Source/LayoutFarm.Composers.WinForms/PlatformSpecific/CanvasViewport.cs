//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{

    public class CanvasViewport
    {
        int viewportX;
        int viewportY;
        int viewportWidth;
        int viewportHeight;


        MyTopWindowRenderBox topWindowBox;

        QuadPages quadPages = null;

        int h_smallChange = 0;
        int h_largeChange = 0;
        int v_smallChange = 0;
        int v_largeChange = 0;
        EventHandler<UIInvalidateEventArgs> canvasInvalidateHandler;
        EventHandler<UICursorEventArgs> canvasCursorChangedHandler;
        EventHandler<EventArgs> canvasSizeChangedHandler;
        bool fullMode = true;
        ISurfaceViewportControl outputWindow;
        public CanvasViewport(ISurfaceViewportControl outputWindow,
            MyTopWindowRenderBox wintop,
            Size viewportSize, int cachedPageNum)
        {
            this.outputWindow = outputWindow;
            this.topWindowBox = wintop;

            quadPages = new QuadPages(cachedPageNum, viewportSize.Width, viewportSize.Height * 2);

            this.viewportWidth = viewportSize.Width;
            this.viewportHeight = viewportSize.Height;


            canvasInvalidateHandler = Canvas_Invalidate;
            canvasCursorChangedHandler = Canvas_CursorChange;
            canvasSizeChangedHandler = Canvas_SizeChanged;



            wintop.CanvasInvalidatedEvent += canvasInvalidateHandler;

            viewportX = 0;
            viewportY = 0;
            CalculateCanvasPages();
            EvaluateScrollBar();
        }

        public void UpdateCanvasViewportSize(int viewportWidth, int viewportHeight)
        {

            if (this.viewportWidth != viewportWidth || this.viewportHeight != viewportHeight)
            {
                this.viewportWidth = viewportWidth;
                this.viewportHeight = viewportHeight;

                quadPages.ResizeAllPages(viewportWidth, viewportHeight);
                CalculateCanvasPages();

                topWindowBox.ChangeVisualRootSize(viewportWidth, viewportHeight);
            }

        }

        ~CanvasViewport()
        {
            quadPages.Dispose();
        }


        void Canvas_CursorChange(object sender, UICursorEventArgs e)
        {

        }

        void Canvas_SizeChanged(object sender, EventArgs e)
        {
            EvaluateScrollBar();
        }
        void Canvas_Invalidate(object sender, UIInvalidateEventArgs e)
        {
            quadPages.CanvasInvalidate(e.InvalidArea);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hdc);

        void PaintMe()
        {
            IntPtr hdc = GetDC(outputWindow.Handle);
            topWindowBox.PrepareRender();
            topWindowBox.ClearNotificationSizeChangeList();
            topWindowBox.BeginRenderPhase();

            if (fullMode)
            {
                quadPages.RenderToOutputWindowFullMode(topWindowBox, hdc, viewportX, viewportY, viewportWidth, viewportHeight);
            }
            else
            {
                quadPages.RenderToOutputWindowPartialMode(topWindowBox, hdc, viewportX, viewportY, viewportWidth, viewportHeight);
            }

            topWindowBox.EndRenderPhase();

#if DEBUG

            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
                List<dbugLayoutMsg> outputMsgs = outputWindow.dbug_rootDocDebugMsgs;
                outputMsgs.Clear();
                outputMsgs.Add(new dbugLayoutMsg(null as RenderElement, "[" + debug_render_to_output_count + "]"));
                visualroot.dbug_DumpRootDrawingMsg(outputMsgs);
                outputWindow.dbug_InvokeVisualRootDrawMsg();
                debug_render_to_output_count++;
            }
#endif


            ReleaseDC(outputWindow.Handle, hdc);
#if DEBUG

            if (MyTopWindowRenderBox.dbugVE_HighlightMe != null)
            {
                outputWindow.dbug_HighlightMeNow(MyTopWindowRenderBox.dbugVE_HighlightMe.GetGlobalRect());

            }
#endif
        }

        public void PaintMe(object sender, EventArgs e)
        {
            PaintMe();
        }


#if DEBUG
        int debug_render_to_output_count = -1;
#endif
        internal void OnMouseMove(UIMouseEventArgs e)
        {
            e.OffsetCanvasOrigin(-viewportX, -viewportY); topWindowBox.OnMouseMove(e);
            e.OffsetCanvasOrigin(viewportX, viewportY); if (!quadPages.IsValid)
            {
                PaintMe();
            }
        }

        internal void OnDoubleClick(UIMouseEventArgs e)
        {
            e.OffsetCanvasOrigin(-viewportX, -viewportY); topWindowBox.OnDoubleClick(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);
            if (!quadPages.IsValid)
            {
                PaintMe();
            }
        }
        internal void OnMouseWheel(UIMouseEventArgs e)
        {
            fullMode = true;
            e.OffsetCanvasOrigin(-viewportX, -viewportY); topWindowBox.OnMouseWheel(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);
            if (!quadPages.IsValid)
            {
                PaintMe();
            }
        }
        internal void OnMouseUp(UIMouseEventArgs e)
        {
#if DEBUG
            dbugMouseDown = false;
#endif
            fullMode = false;
            e.OffsetCanvasOrigin(-viewportX, -viewportY);
            topWindowBox.OnMouseUp(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);

            if (!quadPages.IsValid)
            {
                PaintMe();
            }

            //if (topRenderBox.IsCurrentElementUseCaret)
            //{
            //    ShowCaret();
            //}
        }
        internal void OnLostFocus(UIFocusEventArgs e)
        {

            fullMode = false;
            e.OffsetCanvasOrigin(-viewportX, -viewportY);
            topWindowBox.OnLostFocus(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);

        }
        internal void OnGotFocus(UIFocusEventArgs e)
        {
            fullMode = false;
            e.OffsetCanvasOrigin(-viewportX, -viewportY);
            topWindowBox.OnGotFocus(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);


        }
        internal void OnDrag(UIDragEventArgs e)
        {
            fullMode = false;
            e.OffsetCanvasOrigin(-viewportX, -viewportY);
            topWindowBox.OnDrag(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);
            if (!quadPages.IsValid)
            {
                PaintMe();
            }
        }
#if DEBUG
        public static bool dbugMouseDown = false;
#endif
        internal void OnMouseDown(UIMouseEventArgs e)
        {

#if DEBUG
            dbugMouseDown = true;
#endif
            e.OffsetCanvasOrigin(-viewportX, -viewportY);
            topWindowBox.OnMouseDown(e);

            e.OffsetCanvasOrigin(viewportX, viewportY);

            if (!quadPages.IsValid)
            {
                PaintMe();
            }

#if DEBUG
            RootGraphic visualroot = topWindowBox.dbugVRoot;
            if (visualroot.dbug_RecordHitChain)
            {
                outputWindow.dbug_rootDocHitChainMsgs.Clear();
                visualroot.dbug_DumpCurrentHitChain(outputWindow.dbug_rootDocHitChainMsgs);
                outputWindow.dbug_InvokeHitChainMsg();
            }
#endif
        }
        internal void OnDragStart(UIDragEventArgs e)
        {
            fullMode = false;
            e.OffsetCanvasOrigin(-viewportX, -viewportY);
            topWindowBox.OnDragStart(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);
            if (!quadPages.IsValid)
            {
                PaintMe();
            }
        }

        internal void OnDragStop(UIDragEventArgs e)
        {
            fullMode = false;
            e.OffsetCanvasOrigin(-viewportX, -viewportY);
            topWindowBox.OnDragStop(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);

            if (!quadPages.IsValid)
            {
                PaintMe();
            }

        }
        internal void OnKeyDown(UIKeyEventArgs e)
        {
            topWindowBox.MyVisualRoot.TempStopCaret();

            fullMode = false;
            e.OffsetCanvasOrigin(-viewportX, -viewportY);
#if DEBUG
            topWindowBox.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            topWindowBox.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UIKeys)e.KeyData);
            topWindowBox.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif

            topWindowBox.OnKeyDown(e);

            e.OffsetCanvasOrigin(viewportX, viewportY);


            if (!quadPages.IsValid)
            {
                PaintMe();
            }
        }
        internal void OnKeyPress(UIKeyPressEventArgs e)
        {
            topWindowBox.MyVisualRoot.TempStopCaret();
#if DEBUG
            topWindowBox.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            topWindowBox.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYPRESS " + e.KeyChar);
            topWindowBox.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif

            fullMode = false;
            e.OffsetCanvasOrigin(-viewportX, -viewportY);
            topWindowBox.OnKeyPress(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);

            if (!quadPages.IsValid)
            {
                PaintMe();
            }
        }
        internal void OnKeyUp(UIKeyEventArgs e)
        {
            topWindowBox.MyVisualRoot.TempStopCaret();

            fullMode = false;
            e.OffsetCanvasOrigin(-viewportX, -viewportY);
            topWindowBox.OnKeyUp(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);

            topWindowBox.MyVisualRoot.TempRunCaret();
        }
        internal bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            topWindowBox.MyVisualRoot.TempStopCaret();
            fullMode = false;
            e.OffsetCanvasOrigin(-viewportX, -viewportY); bool result = topWindowBox.OnProcessDialogKey(e);
            e.OffsetCanvasOrigin(viewportX, viewportY);

            if (!quadPages.IsValid)
            {
                PaintMe();
            }
            return result;
        }

        public Point LogicalViewportLocation
        {
            get
            {
                return new Point(viewportX, viewportY);
            }

        }



        void CalculateCanvasPages()
        {
            quadPages.CalculateCanvasPages(viewportX, viewportY, viewportWidth, viewportHeight);
            fullMode = true;
        }

        public void ScrollByNotRaiseEvent(int dx, int dy, out UIScrollEventArgs hScrollEventArgs, out UIScrollEventArgs vScrollEventArgs)
        {
            vScrollEventArgs = null;
            if (dy < 0)
            {
                int old_y = viewportY;
                if (viewportY + dy < 0)
                {
                    dy = -viewportY;
                    viewportY = 0;
                }
                else
                {
                    viewportY += dy;
                }
                vScrollEventArgs = new UIScrollEventArgs(
                    UIScrollEventType.ThumbPosition,
                    old_y,
                    viewportY, UIScrollOrientation.VerticalScroll);

            }
            else if (dy > 0)
            {
                int old_y = viewportY;
                int viewportButtom = viewportY + viewportHeight; if (viewportButtom + dy > topWindowBox.Height)
                {
                    if (viewportButtom < topWindowBox.Height)
                    {
                        viewportY = topWindowBox.Height - viewportHeight;
                    }
                }
                else
                {

                    viewportY += dy;
                }
                vScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_y, viewportY, UIScrollOrientation.VerticalScroll);

            }
            hScrollEventArgs = null;
            if (dx == 0)
            {
            }
            else if (dx > 0)
            {

                int old_x = viewportX;
                int viewportRight = viewportX + viewportWidth; if (viewportRight + dx > topWindowBox.Width)
                {
                    if (viewportRight < topWindowBox.Width)
                    {
                        viewportX = topWindowBox.Width - viewportWidth;
                    }
                }
                else
                {
                    viewportX += dx;
                }
                hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_x, viewportX, UIScrollOrientation.HorizontalScroll);

            }
            else
            {
                int old_x = viewportX;
                if (old_x + dx < 0)
                {
                    dx = -viewportX;
                    viewportX = 0;
                }
                else
                {
                    viewportX += dx;
                }
                hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_x, viewportX, UIScrollOrientation.HorizontalScroll);

            }
            CalculateCanvasPages();

        }
        internal void ScrollBy(int dx, int dy)
        {
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            ScrollByNotRaiseEvent(dx, dy, out hScrollEventArgs, out vScrollEventArgs);
            if (vScrollEventArgs != null)
            {
                outputWindow.viewport_VScrollChanged(this, vScrollEventArgs);

            }
            if (hScrollEventArgs != null)
            {
                outputWindow.viewport_HScrollChanged(this, hScrollEventArgs);
            }
            PaintMe();
        }
        public void ScrollToNotRaiseScrollChangedEvent(int x, int y, out UIScrollEventArgs hScrollEventArgs, out UIScrollEventArgs vScrollEventArgs)
        {
            hScrollEventArgs = null;
            vScrollEventArgs = null;
            if (x > topWindowBox.Width - viewportWidth)
            {
                x = topWindowBox.Width - viewportWidth;

            }
            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }
            else if (y > 0)
            {
                if (y > topWindowBox.Height - viewportHeight)
                {

                    y = topWindowBox.Height - viewportHeight;
                    if (y < 0)
                    {
                        y = 0;
                    }
                }
            }
            int old_y = viewportY; viewportX = x;
            viewportY = y;
            vScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_y, viewportY, UIScrollOrientation.VerticalScroll);
            CalculateCanvasPages();

        }
        public void ScrollTo(int x, int y)
        {
            if (this.viewportY == y && this.viewportX == x)
            {
                return;
            }
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            ScrollToNotRaiseScrollChangedEvent(x, y, out hScrollEventArgs, out vScrollEventArgs);

            if (vScrollEventArgs != null)
            {
                outputWindow.viewport_VScrollChanged(this, vScrollEventArgs);

            }
            if (hScrollEventArgs != null)
            {

                outputWindow.viewport_HScrollChanged(this, vScrollEventArgs);
            }
            PaintMe();
        }
        void EvaluateScrollBar(out ScrollSurfaceRequestEventArgs hScrollSupportEventArgs,
            out ScrollSurfaceRequestEventArgs vScrollSupportEventArgs)
        {




            hScrollSupportEventArgs = null;
            vScrollSupportEventArgs = null;


            v_largeChange = viewportHeight;
            v_smallChange = v_largeChange / 4;

            h_largeChange = viewportWidth;
            h_smallChange = h_largeChange / 4;

            if (topWindowBox.Height <= viewportHeight)
            {

                vScrollSupportEventArgs = new ScrollSurfaceRequestEventArgs(false);
            }
            else
            {
                vScrollSupportEventArgs = new ScrollSurfaceRequestEventArgs(true);
            }

            if (topWindowBox.Width <= viewportWidth)
            {
                hScrollSupportEventArgs = new ScrollSurfaceRequestEventArgs(false);
            }
            else
            {
                hScrollSupportEventArgs = new ScrollSurfaceRequestEventArgs(true);
            }
        }
        void EvaluateScrollBar()
        {

            ScrollSurfaceRequestEventArgs hScrollSupportEventArgs;
            ScrollSurfaceRequestEventArgs vScrollSupportEventArgs;
            EvaluateScrollBar(out hScrollSupportEventArgs, out vScrollSupportEventArgs);
            if (hScrollSupportEventArgs != null)
            {
                outputWindow.viewport_HScrollRequest(this, hScrollSupportEventArgs);
            }
            if (vScrollSupportEventArgs != null)
            {
                outputWindow.viewport_VScrollRequest(this, vScrollSupportEventArgs);
            }
        }

        public void SetFullMode(bool value)
        {
            fullMode = value;
        }

        public void Close()
        {
            topWindowBox.CloseWinRoot();

        }
    }
}
