//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace LayoutFarm.Presentation
{

    partial class MultiLayerRenderBox
    {
        ScrollableSurface vscrollableSurface;

        public ScrollableSurface ScrollableSurface
        {
            get { return this.vscrollableSurface; }
            set { this.vscrollableSurface = value; }
        }
        public void ScrollableDrawContent(CanvasBase destPage, InternalRect updateArea)
        {
            vscrollableSurface.DrawToThisPage(destPage, updateArea);
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
            if (y == myviewportY && x == myviewportX)
            {
                return;
            }

            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            MyScrollToNotRaiseEvent(x, y, out hScrollEventArgs, out vScrollEventArgs, vinv);

        }



        void MyScrollToNotRaiseEvent(int x, int y,
        out UIScrollEventArgs hScrollEventArgs, out UIScrollEventArgs vScrollEventArgs,
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
                myviewportX = x;
                myviewportY = y;
                this.InvalidateGraphic(vinv);
            }
            else
            {
                if (myviewportX != x && vscrollableSurface.HasHScrollChanged)
                {
                    hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, myviewportX, x, UIScrollOrientation.HorizontalScroll);

                }
                if (myviewportY != y && vscrollableSurface.HasVScrollChanged)
                {
                    vScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, myviewportY, y, UIScrollOrientation.VerticalScroll);
                }
                myviewportX = x;
                myviewportY = y;


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
                int old_y = myviewportY;
                if (myviewportY + dy < 0)
                {
                    dy = -myviewportY;
                    myviewportY = 0;
                }
                else
                {
                    myviewportY += dy;
                }

                if (this.vscrollableSurface != null && vscrollableSurface.HasVScrollChanged)
                {
                    vScrollEventArgs = new UIScrollEventArgs(
                        UIScrollEventType.ThumbPosition,
                        old_y, myviewportY,
                        UIScrollOrientation.VerticalScroll);

                }
            }
            else if (dy > 0)
            {
                int old_y = myviewportY;
                int viewportButtom = myviewportY + Height;
                if (viewportButtom + dy > innerContentSize.Height)
                {
                    if (viewportButtom < innerContentSize.Height)
                    {
                        myviewportY = innerContentSize.Height - Height;
                    }
                }
                else
                {

                    myviewportY += dy;
                }
                if (vscrollableSurface != null && vscrollableSurface.HasVScrollChanged)
                {
                    vScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_y, myviewportY, UIScrollOrientation.VerticalScroll);

                }
            }
            hScrollEventArgs = null;
            if (dx == 0)
            {
            }
            else if (dx > 0)
            {

                int old_x = myviewportX;
                int viewportRight = myviewportX + Width; if (viewportRight + dx > innerContentSize.Width)
                {
                    if (this.IsTextEditContainer)
                    {
                        myviewportX += dx;
                    }
                    else
                    {
                        if (viewportRight < innerContentSize.Width)
                        {
                            myviewportX = innerContentSize.Width - Width;
                        }
                    }
                }
                else
                {
                    myviewportX += dx;
                }
                if (vscrollableSurface != null && vscrollableSurface.HasHScrollChanged)
                {
                    hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition, old_x, myviewportX, UIScrollOrientation.HorizontalScroll);

                }
            }
            else
            {
                int old_x = myviewportX;
                if (old_x + dx < 0)
                {
                    dx = -myviewportX;
                    myviewportX = 0;
                }
                else
                {
                    myviewportX += dx;
                }
                if (vscrollableSurface != null && vscrollableSurface.HasHScrollChanged)
                {
                    hScrollEventArgs = new UIScrollEventArgs(UIScrollEventType.ThumbPosition,
                        old_x, myviewportX, UIScrollOrientation.HorizontalScroll);

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
                vscrollableSurface.RaiseProperEvents(hScrollEventArgs, vScrollEventArgs);
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
            if (y == myviewportY && x == myviewportX)
            {
                return;
            }

            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            MyScrollToNotRaiseEvent(x, y, out hScrollEventArgs, out vScrollEventArgs, vinv);


            if (vscrollableSurface != null)
            {
                vscrollableSurface.RaiseProperEvents(hScrollEventArgs, vScrollEventArgs);
            }
        }

        public void ContainerDrawOriginalContent(CanvasBase destPage, InternalRect updateArea)
        {
            if (this.IsScrollable && !this.HasDoubleScrollableSurface)
            {
                destPage.OffsetCanvasOrigin(-myviewportX, -myviewportY);
                updateArea.Offset(myviewportX, myviewportY);
            }

            DrawBackground(this, destPage, updateArea);
            ((MultiLayerRenderBox)this).DrawChildContent(destPage, updateArea);

            if (this.IsScrollable && !this.HasDoubleScrollableSurface)
            {
                destPage.OffsetCanvasOrigin(myviewportX, myviewportY);
                updateArea.Offset(-myviewportX, -myviewportY);
            }
        }

        public int HorizontalLargeChange
        {
            get
            {

                if (vscrollableSurface != null)
                {
                    return vscrollableSurface.HorizontalLargeChange;
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
                    return vscrollableSurface.HorizontalSmallChange;
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
                    return vscrollableSurface.VerticalLargeChange;
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
                    return vscrollableSurface.VerticalSmallChange;
                }
                else
                {
                    return 0;
                }
            }
        }
        public Size InnerContentSize
        {
            get
            {
                VisualLayer groundLayer = GetGroundLayer();
                if (groundLayer == null)
                {
                    return this.Size;
                }
                else
                {
                    Size s1 = groundLayer.PostCalculateContentSize;
                    if (s1.Width < this.Width)
                    {
                        s1.Width = this.Width;
                    }
                    if (s1.Height < this.Height)
                    {
                        s1.Height = this.Height;
                    }
                    return s1;
                }
            }
        }

        public int ViewportBottom
        {
            get
            {
                return this.Bottom + myviewportY;
            }
        }
        public int ViewportRight
        {
            get
            {
                return this.Right + this.myviewportX;
            }
        }
        public int ViewportY
        {
            get
            {
                return this.myviewportY;
            }
            set
            {
                this.myviewportY = value;
            }
        }
        public int ViewportX
        {
            get
            {

                return this.myviewportX;
            }
            set
            {
                this.myviewportX = value;
            }
        }

        public void AddVScrollHandler(EventHandler<UIScrollEventArgs> vscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> vscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.VScrollChanged += vscrollChanged;
                vscrollableSurface.VScrollRequest += vscrollSupport;
            }
        }
        public void RemoveVScrollHandler(EventHandler<UIScrollEventArgs> vscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> vscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.VScrollChanged -= vscrollChanged;
                vscrollableSurface.VScrollRequest -= vscrollSupport;
            }
        }
        public void AddHScrollHandler(EventHandler<UIScrollEventArgs> hscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> hscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.HScrollChanged += hscrollChanged;
                vscrollableSurface.HScrollRequest += hscrollSupport;
            }
        }
        public void RemoveHScrollHandler(EventHandler<UIScrollEventArgs> hscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> hscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.HScrollChanged -= hscrollChanged;
                vscrollableSurface.HScrollRequest -= hscrollSupport;
            }
        }


    }
}