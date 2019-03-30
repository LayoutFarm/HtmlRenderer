//MIT, 2015-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
namespace LayoutFarm.HtmlBoxes
{
    class CssScrollView : CssBox
    {
        CssScrollWrapper _scrollView;
        //vertical scrollbar
        ScrollingRelation _vscRelation;
        ScrollBar _vscbar;
        //horizontal scrollbar
        ScrollingRelation _hscRelation;
        ScrollBar _hscbar;
        CssBox _innerBox;
        HtmlHost _htmlhost;

        public CssScrollView(HtmlHost htmlhost, Css.BoxSpec boxSpec,
            IRootGraphics rootgfx)
            : base(boxSpec, rootgfx)
        {
            _htmlhost = htmlhost;
        }
        public CssBox InnerBox => _innerBox;

        public void SetInnerBox(CssBox innerBox)
        {
            if (_innerBox != null)
            {
                return;
            }

            _innerBox = innerBox;
            _scrollView = new CssScrollWrapper(innerBox);
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
                _vscbar = new ScrollBar(scBarWidth, needHScrollBar ? newH : originalBoxH);
                _vscbar.ScrollBarType = ScrollBarType.Vertical;
                _vscbar.MinValue = 0;
                _vscbar.MaxValue = innerBox.VisualHeight;
                _vscbar.SmallChange = 20;
                //add relation between viewpanel and scroll bar 
                _vscRelation = new ScrollingRelation(_vscbar.SliderBox, _scrollView);
                //---------------------- 
                CssBox scBarWrapCssBox = LayoutFarm.Composers.CustomCssBoxGenerator.CreateCssWrapper(
                            _htmlhost,
                             _vscbar,
                             _vscbar.GetPrimaryRenderElement((RootGraphic)this.GetInternalRootGfx()),
                             CssBox.UnsafeGetBoxSpec(this),
                             null,
                             false);
                scBarWrapCssBox.SetLocation(newW, 0);
                this.AppendToAbsoluteLayer(scBarWrapCssBox);
            }

            if (needHScrollBar)
            {
                _hscbar = new ScrollBar(needVScrollBar ? newW : originalBoxW, scBarWidth);
                _hscbar.ScrollBarType = ScrollBarType.Horizontal;
                _hscbar.MinValue = 0;
                _hscbar.MaxValue = innerBox.VisualHeight;
                _hscbar.SmallChange = 20;
                //add relation between viewpanel and scroll bar 
                _hscRelation = new ScrollingRelation(_hscbar.SliderBox, _scrollView);
                //---------------------- 

                CssBox scBarWrapCssBox = LayoutFarm.Composers.CustomCssBoxGenerator.CreateCssWrapper(
                        _htmlhost,
                         _hscbar,
                         _hscbar.GetPrimaryRenderElement((RootGraphic)this.GetInternalRootGfx()),
                         CssBox.UnsafeGetBoxSpec(this),
                         null,
                         false);
                scBarWrapCssBox.SetLocation(0, newH);
                this.AppendToAbsoluteLayer(scBarWrapCssBox);
            }
        }



        class CssScrollWrapper : IScrollable
        {
            CssBox _cssbox;
            EventHandler<ViewportChangedEventArgs> _viewportChanged;
            public CssScrollWrapper(CssBox cssbox)
            {
                _cssbox = cssbox;
            }
            void IScrollable.SetViewport(int x, int y, object reqBy)
            {
                _cssbox.SetViewport(x, y);
            }

            int IScrollable.ViewportLeft => _cssbox.ViewportX;
            int IScrollable.ViewportTop => _cssbox.ViewportY;
            int IScrollable.ViewportWidth => (int)_cssbox.VisualWidth;
            int IScrollable.ViewportHeight => (int)_cssbox.VisualHeight;

            int IScrollable.InnerHeight => (int)_cssbox.InnerContentHeight;   //content height of the cssbox
            int IScrollable.InnerWidth => (int)_cssbox.InnerContentWidth;    //content width of the cssbox
             
            event EventHandler<ViewportChangedEventArgs> IScrollable.ViewportChanged
            {
                //TODO: review this
                add
                {
                    if (_viewportChanged == null)
                    {
                        _viewportChanged = value;
                    }
                    else
                    {
                        _viewportChanged += value;

                    }
                }
                remove
                {
                    if (_viewportChanged != null)
                    {
                        _viewportChanged -= value;
                    }

                }
            }
        }
    }
}