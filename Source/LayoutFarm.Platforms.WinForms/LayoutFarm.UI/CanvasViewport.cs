//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.Drawing
{

    class CanvasViewport
    {
        int viewportX;
        int viewportY;
        int viewportWidth;
        int viewportHeight;
        TopWindowRenderBox topWindowBox;

        QuadPages quadPages = null;

        int h_smallChange = 0;
        int h_largeChange = 0;
        int v_smallChange = 0;
        int v_largeChange = 0;

        CanvasInvalidateRequestDelegate canvasInvalidateHandler;
        EventHandler<EventArgs> canvasSizeChangedHandler;

        bool fullMode = true;
        bool isClosed;//is this viewport closed
        RootGraphic rootGraphics;

        public CanvasViewport(TopWindowRenderBox wintop,
            Size viewportSize, int cachedPageNum)
        {
            this.rootGraphics = wintop.Root;

            this.topWindowBox = wintop;
            quadPages = new QuadPages(cachedPageNum, viewportSize.Width, viewportSize.Height * 2);

            this.viewportWidth = viewportSize.Width;
            this.viewportHeight = viewportSize.Height;

            canvasInvalidateHandler = Canvas_Invalidate;
            canvasSizeChangedHandler = Canvas_SizeChanged; 

            wintop.SetCanvasInvalidateRequest(canvasInvalidateHandler);
            viewportX = 0;
            viewportY = 0;

            CalculateCanvasPages();
        }
        ~CanvasViewport()
        {
            quadPages.Dispose();
        }
#if DEBUG
        public IdbugOutputWindow dbugOutputWindow
        {
            get;
            set;
        }
#endif
        public void UpdateCanvasViewportSize(int viewportWidth, int viewportHeight)
        {

            if (this.viewportWidth != viewportWidth || this.viewportHeight != viewportHeight)
            {
                this.viewportWidth = viewportWidth;
                this.viewportHeight = viewportHeight;

                quadPages.ResizeAllPages(viewportWidth, viewportHeight);
                CalculateCanvasPages();
                
                topWindowBox.ChangeRootGraphicSize(viewportWidth, viewportHeight);
            }

        }
        void Canvas_CursorChange(object sender, UICursorEventArgs e)
        {
        }
        void Canvas_SizeChanged(object sender, EventArgs e)
        {
            //EvaluateScrollBar();
        }
        void Canvas_Invalidate(ref Rectangle r)
        {
            quadPages.CanvasInvalidate(r);
        }


        public bool IsQuadPageValid
        {
            get { return this.quadPages.IsValid; }
        }
        public void PaintMe(IntPtr hdc)
        {
            if (isClosed) { return; }
            //------------------------------------ 

            topWindowBox.PrepareRender(); 

            //---------------
            this.rootGraphics.IsInRenderPhase = true;
#if DEBUG
            this.rootGraphics.dbug_rootDrawingMsg.Clear();
            this.rootGraphics.dbug_drawLevel = 0;
#endif
             

            if (fullMode)
            {
                quadPages.RenderToOutputWindowFullMode(topWindowBox, hdc, viewportX, viewportY, viewportWidth, viewportHeight);
            }
            else
            {
                //temp to full mode
                quadPages.RenderToOutputWindowFullMode(topWindowBox, hdc, viewportX, viewportY, viewportWidth, viewportHeight);
                //quadPages.RenderToOutputWindowPartialMode(topWindowBox, hdc, viewportX, viewportY, viewportWidth, viewportHeight);
            }
            this.rootGraphics.IsInRenderPhase = false;             

#if DEBUG

            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
                List<dbugLayoutMsg> outputMsgs = dbugOutputWindow.dbug_rootDocDebugMsgs;
                outputMsgs.Clear();
                outputMsgs.Add(new dbugLayoutMsg(null as RenderElement, "[" + debug_render_to_output_count + "]"));
                visualroot.dbug_DumpRootDrawingMsg(outputMsgs);
                dbugOutputWindow.dbug_InvokeVisualRootDrawMsg();
                debug_render_to_output_count++;
            }


            if (dbugHelper01.dbugVE_HighlightMe != null)
            {
                dbugOutputWindow.dbug_HighlightMeNow(dbugHelper01.dbugVE_HighlightMe.dbugGetGlobalRect());

            }
#endif
        }



#if DEBUG
        int debug_render_to_output_count = -1;
#endif


        internal bool FullMode
        {
            get { return this.fullMode; }
            set { this.fullMode = value; }
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

        public void EvaluateScrollBar(out ScrollSurfaceRequestEventArgs hScrollSupportEventArgs,
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


        public void Close()
        {
            this.isClosed = true;
            this.rootGraphics.CloseWinRoot();
             
        }
    }
}
