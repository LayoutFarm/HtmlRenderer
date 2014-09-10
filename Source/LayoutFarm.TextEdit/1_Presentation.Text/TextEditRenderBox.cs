//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm.Presentation.Text
{


    partial class TextEditRenderBox
    {
        RenderSurfaceScrollRelation scrollRelation;
        CustomRenderSurface vscrollableSurface;
        public CustomRenderSurface ScrollableSurface
        {
            get { return this.vscrollableSurface; }
            set { this.vscrollableSurface = value; }
        }
        public RenderSurfaceScrollRelation ScrollRelation
        {
            get { return this.scrollRelation; }
            set { this.scrollRelation = value; }
        }
   
        public override void CustomDrawToThisPage(CanvasBase canvasPage, InternalRect updateArea)
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.DrawToThisPage(canvasPage, updateArea);
            }
            base.CustomDrawToThisPage(canvasPage, updateArea);
        }
        internal void BoxEvaluateScrollBar()
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.ConfirmSizeChanged();
            }
        }
        public void ScrollToNotRaiseEvent(int x, int y, VisualElementArgs vinv)
        {

            if (!this.IsScrollable)
            {
                return;
            }
            MyScrollToNotRaiseEvent(x, y, vinv);

        }

        public void MyScrollToNotRaiseEvent(int x, int y, VisualElementArgs vinv)
        {
            if (y == this.ViewportY && x == this.ViewportX)
            {
                return;
            }

            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            MyScrollToNotRaiseEvent(x, y, out hScrollEventArgs, out vScrollEventArgs, vinv);

        }



        void MyScrollToNotRaiseEvent(int x, int y,
            out UIScrollEventArgs hScrollEventArgs,
            out UIScrollEventArgs vScrollEventArgs,
            VisualElementArgs vinv)
        {
            hScrollEventArgs = null;
            vScrollEventArgs = null;

            System.Drawing.Size innerContentSize = this.InnerContentSize; if (x < 0)
            {
                x = 0;
            }
            else if (x > 0)
            {
                if (x > innerContentSize.Width - Width)
                {
                    x = innerContentSize.Width - Width;
                    if (x < 0)
                    {
                        x = 0;
                    }
                }
            }
            if (y < 0)
            {
                y = 0;
            }
            else if (y > 0)
            {

                if (y > innerContentSize.Height - Height)
                {

                    y = innerContentSize.Height - Height;
                    if (y < 0)
                    {
                        y = 0;
                    }
                }
            }

            if (vscrollableSurface == null)
            {
                this.InvalidateGraphic(vinv);
                this.ViewportX = x;
                this.ViewportY = y;

                this.InvalidateGraphic(vinv);
            }
            else
            {
                if (ViewportX != x && scrollRelation.HasHScrollChanged)
                {
                    hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, this.ViewportX, x, UIScrollOrientation.HorizontalScroll);

                }
                if (ViewportY != y && scrollRelation.HasVScrollChanged)
                {
                    vScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, this.ViewportY, y, UIScrollOrientation.VerticalScroll);
                }
                this.ViewportX = x;
                this.ViewportY = y;

                vscrollableSurface.QuadPagesCalculateCanvas();
                vscrollableSurface.FullModeUpdate = true;
                this.InvalidateGraphic(vinv);

                vscrollableSurface.FullModeUpdate = false;
            }

        }
        void MyScrollByNotRaiseEvent(int dx, int dy, out UIScrollEventArgs hScrollEventArgs, out UIScrollEventArgs vScrollEventArgs)
        {
            vScrollEventArgs = null;
            System.Drawing.Size innerContentSize = this.InnerContentSize; ;
            if (dy < 0)
            {
                int old_y = this.ViewportY;
                if (ViewportY + dy < 0)
                {
                    dy = -ViewportY;
                    ViewportY = 0;
                }
                else
                {
                    ViewportY += dy;
                }

                if (this.vscrollableSurface != null && scrollRelation.HasVScrollChanged)
                {
                    vScrollEventArgs = new UIScrollEventArgs(
                        UIScrollEventType.ThumbPosition,
                        old_y, ViewportY,
                        UIScrollOrientation.VerticalScroll);

                }
            }
            else if (dy > 0)
            {
                int old_y = ViewportY;
                int viewportButtom = ViewportY + Height;
                if (viewportButtom + dy > innerContentSize.Height)
                {
                    if (viewportButtom < innerContentSize.Height)
                    {
                        ViewportY = innerContentSize.Height - Height;
                    }
                }
                else
                {

                    this.ViewportY += dy;
                }
                if (vscrollableSurface != null && scrollRelation.HasVScrollChanged)
                {
                    vScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_y, this.ViewportY, UIScrollOrientation.VerticalScroll);

                }
            }
            hScrollEventArgs = null;
            if (dx == 0)
            {
            }
            else if (dx > 0)
            {

                int old_x = this.ViewportX;
                int viewportRight = ViewportX + Width; if (viewportRight + dx > innerContentSize.Width)
                {
                    if (this.IsTextEditContainer)
                    {
                        ViewportX += dx;
                    }
                    else
                    {
                        if (viewportRight < innerContentSize.Width)
                        {
                            ViewportX = innerContentSize.Width - Width;
                        }
                    }
                }
                else
                {
                    ViewportX += dx;
                }
                if (vscrollableSurface != null && scrollRelation.HasHScrollChanged)
                {
                    hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_x, ViewportX, UIScrollOrientation.HorizontalScroll);

                }
            }
            else
            {
                int old_x = this.ViewportX;
                if (old_x + dx < 0)
                {
                    dx = -ViewportX;
                    ViewportX = 0;
                }
                else
                {
                    ViewportX += dx;
                }
                if (vscrollableSurface != null && scrollRelation.HasHScrollChanged)
                {
                    hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition,
                        old_x, this.ViewportX, UIScrollOrientation.HorizontalScroll);

                }
            }


            if (vscrollableSurface != null)
            {
                vscrollableSurface.QuadPagesCalculateCanvas();
                vscrollableSurface.FullModeUpdate = true;
            }
        }
        void MyScrollBy(int dx, int dy, VisualElementArgs vinv)
        {
            if (dy == 0 && dx == 0)
            {
                return;
            }
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;

            MyScrollByNotRaiseEvent(dx, dy, out hScrollEventArgs, out vScrollEventArgs);
            if (vscrollableSurface != null)
            {
                scrollRelation.RaiseProperEvents(hScrollEventArgs, vScrollEventArgs);
                this.InvalidateGraphic(vinv);
                vscrollableSurface.FullModeUpdate = false;
            }
            else
            {
                this.InvalidateGraphic(vinv);
            }
        }
        void MyScrollTo(int x, int y, VisualElementArgs vinv)
        {
            if (y == this.ViewportY && x == this.ViewportX)
            {
                return;
            }
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            MyScrollToNotRaiseEvent(x, y, out hScrollEventArgs, out vScrollEventArgs, vinv);


            if (vscrollableSurface != null)
            {
                scrollRelation.RaiseProperEvents(hScrollEventArgs, vScrollEventArgs);
            }
        }
        public int HorizontalLargeChange
        {
            get
            {

                if (vscrollableSurface != null)
                {
                    return scrollRelation.HorizontalLargeChange;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int HorizontalSmallChange
        {
            get
            {
                if (vscrollableSurface != null)
                {
                    return scrollRelation.HorizontalSmallChange;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int VerticalLargeChange
        {
            get
            {
                if (vscrollableSurface != null)
                {
                    return scrollRelation.VerticalLargeChange;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int VerticalSmallChange
        {
            get
            {
                if (vscrollableSurface != null)
                {
                    return scrollRelation.VerticalSmallChange;
                }
                else
                {
                    return 0;
                }
            }
        }


        public void AddVScrollHandler(EventHandler<UIScrollEventArgs> vscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> vscrollSupport)
        {
            if (vscrollableSurface != null)
            {

                scrollRelation.VScrollChanged += vscrollChanged;
                scrollRelation.VScrollRequest += vscrollSupport;
            }
        }
        public void RemoveVScrollHandler(EventHandler<UIScrollEventArgs> vscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> vscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                scrollRelation.VScrollChanged -= vscrollChanged;
                scrollRelation.VScrollRequest -= vscrollSupport;
            }
        }
        public void AddHScrollHandler(EventHandler<UIScrollEventArgs> hscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> hscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                scrollRelation.HScrollChanged += hscrollChanged;
                scrollRelation.HScrollRequest += hscrollSupport;
            }
        }
        public void RemoveHScrollHandler(EventHandler<UIScrollEventArgs> hscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> hscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                scrollRelation.HScrollChanged -= hscrollChanged;
                scrollRelation.HScrollRequest -= hscrollSupport;
            }
        }
        public void ScrollTo(int x, int y, VisualElementArgs vinv)
        {
            if (!this.IsScrollable)
            {
                return;
            }
            MyScrollTo(x, y, vinv);
        }
        public void ScrollBy(int dx, int dy, VisualElementArgs vinv)
        {
            if (!this.IsScrollable)
            {
                return;
            }
            MyScrollBy(dx, dy, vinv);
        }
        public CustomRenderSurface VisualScrollableSurface
        {
            get
            {
                return vscrollableSurface;
            }
        }



    }
}