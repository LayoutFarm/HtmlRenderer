//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace LayoutFarm.Presentation
{

    partial class ArtVisualContainerBase
    {
        VisualScrollableSurface vscrollableSurface;

        public void EnableScrollableMechanism()
        {
            if (vscrollableSurface == null)
            {
                int initW = VisualRoot.ScreenWidth;
                int initH = VisualRoot.ScreenHeight;
                if (this.Width < initW / 3)
                {
                    initW /= 2;
                }
                if (this.Height < initH / 3)
                {
                    initH /= 2;
                }

                vscrollableSurface = new VisualScrollableSurface(this, initW, initH);
            }

            this.IsScrollable = true;
            this.HasDoubleScrollableSurface = true;

        }
        public void DisableScrollableMechanism()
        {

        }

        public void ScrollableDrawContent(ArtCanvas destPage, InternalRect updateArea)
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

            ArtScrollEventArgs hScrollEventArgs;
            ArtScrollEventArgs vScrollEventArgs;
            MyScrollToNotRaiseEvent(x, y, out hScrollEventArgs, out vScrollEventArgs, vinv);

        }



        void MyScrollToNotRaiseEvent(int x, int y,
out ArtScrollEventArgs hScrollEventArgs, out ArtScrollEventArgs vScrollEventArgs,
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
                    hScrollEventArgs = new ArtScrollEventArgs(ArtScrollEventType.ThumbPosition, myviewportX, x, ArtScrollOrientation.HorizontalScroll);

                }
                if (myviewportY != y && vscrollableSurface.HasVScrollChanged)
                {
                    vScrollEventArgs = new ArtScrollEventArgs(ArtScrollEventType.ThumbPosition, myviewportY, y, ArtScrollOrientation.VerticalScroll);
                }
                myviewportX = x;
                myviewportY = y;


                vscrollableSurface.QuadPagesCalculateCanvas();
                vscrollableSurface.FullModeUpdate = true;
                this.InvalidateGraphic(vinv);

                vscrollableSurface.FullModeUpdate = false;
            }







        }
        void MyScrollByNotRaiseEvent(int dx, int dy, out ArtScrollEventArgs hScrollEventArgs, out ArtScrollEventArgs vScrollEventArgs)
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
                    vScrollEventArgs = new ArtScrollEventArgs(
                        ArtScrollEventType.ThumbPosition,
                        old_y, myviewportY,
                        ArtScrollOrientation.VerticalScroll);

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
                    vScrollEventArgs = new ArtScrollEventArgs(ArtScrollEventType.ThumbPosition, old_y, myviewportY, ArtScrollOrientation.VerticalScroll);

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
                    hScrollEventArgs = new ArtScrollEventArgs(ArtScrollEventType.ThumbPosition, old_x, myviewportX, ArtScrollOrientation.HorizontalScroll);

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
                    hScrollEventArgs = new ArtScrollEventArgs(ArtScrollEventType.ThumbPosition,
                        old_x, myviewportX, ArtScrollOrientation.HorizontalScroll);

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
            ArtScrollEventArgs hScrollEventArgs;
            ArtScrollEventArgs vScrollEventArgs;

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

            ArtScrollEventArgs hScrollEventArgs;
            ArtScrollEventArgs vScrollEventArgs;
            MyScrollToNotRaiseEvent(x, y, out hScrollEventArgs, out vScrollEventArgs, vinv);


            if (vscrollableSurface != null)
            {
                vscrollableSurface.RaiseProperEvents(hScrollEventArgs, vScrollEventArgs);
            }
        }

        public void ContainerDrawOriginalContent(ArtCanvas destPage, InternalRect updateArea)
        {
            if (this.IsScrollable && !this.HasDoubleScrollableSurface)
            {
                destPage.OffsetCanvasOrigin(-myviewportX, -myviewportY);
                updateArea.Offset(myviewportX, myviewportY);
            }
            if (Beh == null)
            {
                DrawBackground(this, destPage, updateArea);
                ((ArtVisualContainerBase)this).DrawChildContent(destPage, updateArea);


            }
            else
            {

                BoxStyle beh = (BoxStyle)Beh;
                DrawBackground(this, destPage, updateArea);
                ((ArtVisualContainerBase)this).DrawChildContent(destPage, updateArea);
            }

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


        public void AddVScrollHandler(EventHandler<ArtScrollEventArgs> vscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> vscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.VScrollChanged += vscrollChanged;
                vscrollableSurface.VScrollRequest += vscrollSupport;
            }
        }
        public void RemoveVScrollHandler(EventHandler<ArtScrollEventArgs> vscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> vscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.VScrollChanged -= vscrollChanged;
                vscrollableSurface.VScrollRequest -= vscrollSupport;
            }
        }
        public void AddHScrollHandler(EventHandler<ArtScrollEventArgs> hscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> hscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.HScrollChanged += hscrollChanged;
                vscrollableSurface.HScrollRequest += hscrollSupport;
            }
        }
        public void RemoveHScrollHandler(EventHandler<ArtScrollEventArgs> hscrollChanged, EventHandler<ScrollSurfaceRequestEventArgs> hscrollSupport)
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.HScrollChanged -= hscrollChanged;
                vscrollableSurface.HScrollRequest -= hscrollSupport;
            }
        }


    }
}