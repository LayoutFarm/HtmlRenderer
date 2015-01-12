// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using PixelFarm.Drawing.WinGdi;

namespace LayoutFarm.UI
{

    abstract class CanvasViewport
    {
        int viewportX;
        int viewportY;
        int viewportWidth;
        int viewportHeight;

        protected IRenderElement topWindowBox;
        protected RootGraphic rootGraphics;

        int h_smallChange = 0;
        int h_largeChange = 0;
        int v_smallChange = 0;
        int v_largeChange = 0;

        CanvasPaintToOutputDelegate canvasPaintToOutputDel;
        EventHandler<EventArgs> canvasSizeChangedHandler;

        bool fullMode = true;
        bool isClosed;//is this viewport closed


        public CanvasViewport(RootGraphic rootgfx,
            Size viewportSize, int cachedPageNum)
        {
            this.rootGraphics = rootgfx;
            this.topWindowBox = rootgfx.TopWindowRenderBox;

            this.viewportWidth = viewportSize.Width;
            this.viewportHeight = viewportSize.Height;

            canvasPaintToOutputDel = Canvas_Invalidated;
            canvasSizeChangedHandler = Canvas_SizeChanged;


            rootgfx.SetCanvasPaintToOutputDel(canvasPaintToOutputDel);
            viewportX = 0;
            viewportY = 0;

        }
        public bool IsClosed
        {
            get { return this.isClosed; }
        }
        protected int ViewportX
        {
            get { return this.viewportX; }
        }
        protected int ViewportY
        {
            get { return this.viewportY; }
        }
        protected int ViewportWidth
        {
            get { return this.viewportWidth; }
        }
        protected int ViewportHeight
        {
            get { return this.viewportHeight; }
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

                ResetQuadPages(viewportWidth, viewportHeight);
                CalculateCanvasPages();
            }
        }
        void ChangeRootGraphicSize(int width, int height)
        {
            //Size currentSize = topWindowBox.Size;
            //if (currentSize.Width != width || currentSize.Height != height)
            //{
            //    topWindowBox.SetSize(width, height);
            //    topWindowBox.InvalidateContentArrangementFromContainerSizeChanged();
            //    topWindowBox.TopDownReCalculateContentSize();
            //    topWindowBox.TopDownReArrangeContentIfNeed();
            //}
        }
        protected virtual void ResetQuadPages(int viewportWidth, int viewportHeight)
        {

        }
        protected virtual void Canvas_CursorChange(object sender, UICursorEventArgs e)
        {

        }
        protected virtual void Canvas_SizeChanged(object sender, EventArgs e)
        {
            //EvaluateScrollBar();
        }
        protected virtual void Canvas_Invalidated(Rectangle r)
        {

        }
        public virtual bool IsQuadPageValid
        {
            get { return true; }
        }




#if DEBUG
        internal int debug_render_to_output_count = -1;
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

        protected virtual void CalculateCanvasPages()
        {

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
                int viewportButtom = viewportY + viewportHeight; if (viewportButtom + dy > rootGraphics.RootHeight)
                {
                    if (viewportButtom < rootGraphics.RootHeight)
                    {
                        viewportY = rootGraphics.RootHeight - viewportHeight;
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
                int viewportRight = viewportX + viewportWidth; if (viewportRight + dx > rootGraphics.RootWidth)
                {
                    if (viewportRight < rootGraphics.RootWidth)
                    {
                        viewportX = rootGraphics.RootWidth - viewportWidth;
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
            if (x > rootGraphics.RootWidth - viewportWidth)
            {
                x = rootGraphics.RootWidth - viewportWidth;

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
                if (y > rootGraphics.RootHeight - viewportHeight)
                {
                    y = rootGraphics.RootHeight - viewportHeight;
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

            if (rootGraphics.RootHeight <= viewportHeight)
            {

                vScrollSupportEventArgs = new ScrollSurfaceRequestEventArgs(false);
            }
            else
            {
                vScrollSupportEventArgs = new ScrollSurfaceRequestEventArgs(true);
            }

            if (rootGraphics.RootWidth <= viewportWidth)
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
            OnClosing();
            this.isClosed = true;
            this.rootGraphics.CloseWinRoot();
        }

        protected virtual void OnClosing()
        {
        }
    }
}
