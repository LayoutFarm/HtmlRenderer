//MIT, 2015-2017, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.InternalUI;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
namespace LayoutFarm.HtmlBoxes
{
    class CssScrollView : CssBox
    {
        CssScrollWrapper scrollView;
        //vertical scrollbar
        ScrollingRelation vscRelation;
        ScrollBar vscbar;
        //horizontal scrollbar
        ScrollingRelation hscRelation;
        ScrollBar hscbar;
        CssBox innerBox;
        public CssScrollView(Css.BoxSpec boxSpec,
            IRootGraphics rootgfx)
            : base(boxSpec, rootgfx)
        {
        }
        public CssBox InnerBox
        {
            get { return this.innerBox; }
        }
        public void SetInnerBox(CssBox innerBox)
        {
            if (this.innerBox != null)
            {
                return;
            }

            this.innerBox = innerBox;
            this.scrollView = new CssScrollWrapper(innerBox);
            //scroll barwidth = 10;
            bool needHScrollBar = false;
            bool needVScrollBar = false;
            int originalBoxW = (int)innerBox.VisualWidth;
            int originalBoxH = (int)innerBox.VisualHeight;
            int newW = originalBoxW;
            int newH = originalBoxH;
            const int scBarWidth = 10;
            if (innerBox.InnerContentHeight > innerBox.ExpectedHeight)
            {
                needVScrollBar = true;
                newW -= scBarWidth;
            }
            if (innerBox.InnerContentWidth > innerBox.ExpectedWidth)
            {
                needHScrollBar = true;
                newH -= scBarWidth;
            }
            innerBox.SetVisualSize(newW, newH);
            innerBox.SetExpectedSize(newW, newH);
            this.AppendToAbsoluteLayer(innerBox);
            //check if need vertical scroll and/or horizontal scroll

            //vertical scrollbar
            if (needVScrollBar)
            {
                this.vscbar = new ScrollBar(scBarWidth, needHScrollBar ? newH : originalBoxH);
                vscbar.ScrollBarType = ScrollBarType.Vertical;
                vscbar.MinValue = 0;
                vscbar.MaxValue = innerBox.VisualHeight;
                vscbar.SmallChange = 20;
                //add relation between viewpanel and scroll bar 
                vscRelation = new ScrollingRelation(vscbar.SliderBox, scrollView);
                //---------------------- 
                var scBarWrapCssBox = LayoutFarm.Composers.CustomCssBoxGenerator.CreateWrapper(
                           this.vscbar,
                           this.vscbar.GetPrimaryRenderElement((RootGraphic)this.GetInternalRootGfx()),
                           CssBox.UnsafeGetBoxSpec(this), false);
                scBarWrapCssBox.SetLocation(newW, 0);
                this.AppendToAbsoluteLayer(scBarWrapCssBox);
            }

            if (needHScrollBar)
            {
                this.hscbar = new ScrollBar(needVScrollBar ? newW : originalBoxW, scBarWidth);
                hscbar.ScrollBarType = ScrollBarType.Horizontal;
                hscbar.MinValue = 0;
                hscbar.MaxValue = innerBox.VisualHeight;
                hscbar.SmallChange = 20;
                //add relation between viewpanel and scroll bar 
                hscRelation = new ScrollingRelation(hscbar.SliderBox, scrollView);
                //---------------------- 
                var renderE = this.hscbar.GetPrimaryRenderElement((RootGraphic)this.GetInternalRootGfx());
                var scBarWrapCssBox = LayoutFarm.Composers.CustomCssBoxGenerator.CreateWrapper(
                         this.hscbar,
                         this.hscbar.GetPrimaryRenderElement((RootGraphic)this.GetInternalRootGfx()),
                         CssBox.UnsafeGetBoxSpec(this), false);
                scBarWrapCssBox.SetLocation(0, newH);
                this.AppendToAbsoluteLayer(scBarWrapCssBox);
            }
        }



        class CssScrollWrapper : IScrollable
        {
            CssBox cssbox;
            public CssScrollWrapper(CssBox cssbox)
            {
                this.cssbox = cssbox;
            }
            void IScrollable.SetViewport(int x, int y)
            {
                this.cssbox.SetViewport(x, y);
            }

            int IScrollable.ViewportX
            {
                get { return this.cssbox.ViewportX; }
            }

            int IScrollable.ViewportY
            {
                get { return this.cssbox.ViewportY; }
            }

            int IScrollable.ViewportWidth
            {
                get { return (int)this.cssbox.VisualWidth; }
            }

            int IScrollable.ViewportHeight
            {
                get { return (int)this.cssbox.VisualHeight; }
            }
            int IScrollable.DesiredHeight
            {
                //content height of the cssbox
                get { return (int)cssbox.InnerContentHeight; }
            }

            int IScrollable.DesiredWidth
            {
                //content width of the cssbox
                get { return (int)cssbox.InnerContentWidth; }
            }

            event EventHandler IScrollable.LayoutFinished
            {
                add { }
                remove { }
            }
        }
    }
}