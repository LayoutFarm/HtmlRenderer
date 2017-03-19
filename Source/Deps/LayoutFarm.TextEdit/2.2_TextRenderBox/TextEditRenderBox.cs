//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.Text
{
    partial class TextEditRenderBox
    {
        RenderSurfaceScrollRelation scrollRelation;
        CustomRenderSurface vscrollableSurface;
        public Color BackgroundColor { get; set; }
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
        protected override void DrawBoxContent(Canvas canvas, Rectangle updateArea)
        {
            var enterFont = canvas.CurrentFont;
            canvas.CurrentFont = this.CurrentTextSpanStyle.FontInfo;
            if (vscrollableSurface != null)
            {
                vscrollableSurface.DrawToThisPage(canvas, updateArea);
            }
            //1. bg 
            if (BackgroundColor.A > 0)
            {
                //canvas.FillRectangle(BackgroundColor, 0, 0, updateArea.Width, updateArea.Height);
                canvas.FillRectangle(BackgroundColor, 0, 0, this.Width, this.Height);
            }


            //2. sub ground 
            if (internalTextLayerController.SelectionRange != null)
            {
                internalTextLayerController.SelectionRange.Draw(canvas, updateArea);
            }

            //3. each layer
            if (vscrollableSurface != null)
            {
                vscrollableSurface.DrawToThisPage(canvas, updateArea);
            }
            else
            {
                this.textLayer.DrawChildContent(canvas, updateArea);
                if (this.HasDefaultLayer)
                {
                    this.DrawDefaultLayer(canvas, ref updateArea);
                }
            }

#if DEBUG
            //for debug
            canvas.FillRectangle(Color.Red, 0, 0, 5, 5);
#endif  
            //4. caret 
            if (this.stateShowCaret)
            {
                Point textManCaretPos = internalTextLayerController.CaretPos;
                this.myCaret.DrawCaret(canvas, textManCaretPos.X, textManCaretPos.Y);
            }
            else
            {
            }
            canvas.CurrentFont = enterFont;
        }

        internal void BoxEvaluateScrollBar()
        {
            if (vscrollableSurface != null)
            {
                vscrollableSurface.ConfirmSizeChanged();
            }
        }
        public void ScrollToNotRaiseEvent(int x, int y)
        {
            if (!this.MayHasViewport)
            {
                return;
            }
            MyScrollToNotRaiseEvent(x, y);
        }

        public void MyScrollToNotRaiseEvent(int x, int y)
        {
            if (y == this.ViewportY && x == this.ViewportX)
            {
                return;
            }

            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            MyScrollToNotRaiseEvent(x, y, out hScrollEventArgs, out vScrollEventArgs);
        }



        void MyScrollToNotRaiseEvent(int x, int y,
            out UIScrollEventArgs hScrollEventArgs,
            out UIScrollEventArgs vScrollEventArgs)
        {
            hScrollEventArgs = null;
            vScrollEventArgs = null;
            Size innerContentSize = this.InnerContentSize;
            if (x < 0)
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
                this.InvalidateGraphics();
                this.SetViewport(x, y);
                this.InvalidateGraphics();
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

                this.SetViewport(x, y);
                vscrollableSurface.QuadPagesCalculateCanvas();
                vscrollableSurface.FullModeUpdate = true;
                this.InvalidateGraphics();
                vscrollableSurface.FullModeUpdate = false;
            }
        }
        void MyScrollByNotRaiseEvent(int dx, int dy, out UIScrollEventArgs hScrollEventArgs, out UIScrollEventArgs vScrollEventArgs)
        {
            vScrollEventArgs = null;
            Size innerContentSize = this.InnerContentSize;
            if (dy < 0)
            {
                int old_y = this.ViewportY;
                if (ViewportY + dy < 0)
                {
                    dy = -ViewportY;
                    this.SetViewport(this.ViewportX, 0);
                }
                else
                {
                    this.SetViewport(this.ViewportX, this.ViewportY + dy);
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
                        this.SetViewport(this.ViewportX, innerContentSize.Height - Height);
                    }
                }
                else
                {
                    this.SetViewport(this.ViewportX, innerContentSize.Height + dy);
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
                int viewportRight = ViewportX + Width;
                if (viewportRight + dx > innerContentSize.Width)
                {
                    if (viewportRight < innerContentSize.Width)
                    {
                        this.SetViewport(innerContentSize.Width - Width, this.ViewportY);
                    }
                }
                else
                {
                    this.SetViewport(this.ViewportX + dx, this.ViewportY);
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
                    SetViewport(0, this.ViewportY);
                }
                else
                {
                    SetViewport(this.ViewportX + dx, this.ViewportY);
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
        void MyScrollBy(int dx, int dy)
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
                this.InvalidateGraphics();
                vscrollableSurface.FullModeUpdate = false;
            }
            else
            {
                this.InvalidateGraphics();
            }
        }
        void MyScrollTo(int x, int y)
        {
            if (y == this.ViewportY && x == this.ViewportX)
            {
                return;
            }
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            MyScrollToNotRaiseEvent(x, y, out hScrollEventArgs, out vScrollEventArgs);
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
        public void ScrollTo(int x, int y)
        {
            if (!this.MayHasViewport)
            {
                return;
            }

            MyScrollTo(x, y);
        }
        public void ScrollBy(int dx, int dy)
        {
            if (!this.MayHasViewport)
            {
                return;
            }
            MyScrollBy(dx, dy);
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