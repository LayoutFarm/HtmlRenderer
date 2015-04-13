//BSD 2014-2015 ,WinterDev 
using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.ContentManagers;
using LayoutFarm.UI;

namespace LayoutFarm.HtmlBoxes
{
    class ScrollComponent
    {
        //create special scroll component
        CssScrollWrapper scrollView;
        MyCssBoxDescorator myCssBoxDecor;
        ScrollBar vscbar;
        ScrollingRelation scRelation;
        RenderElement vscBarPrimaryRenderE;

        public ScrollComponent(MyCssBoxDescorator myCssBoxDecor)
        {
            this.myCssBoxDecor = myCssBoxDecor;
            CssBox cssbox = myCssBoxDecor.TargetBox;
            this.scrollView = new CssScrollWrapper(cssbox);

            //vertical scrollbar
            var vscbar = new ScrollBar(10, (int)cssbox.SizeHeight);
            vscbar.SetLocation(0, 0);
            vscbar.MinValue = 0;
            vscbar.MaxValue = cssbox.SizeHeight;
            vscbar.SmallChange = 20;

            //add relation between viewpanel and scroll bar 
            scRelation = new ScrollingRelation(vscbar, scrollView);
            vscBarPrimaryRenderE = vscbar.GetPrimaryRenderElement((RootGraphic)myCssBoxDecor.TargetBox.RootGfx);
        }
        public void Draw(PaintVisitor p)
        {
            vscBarPrimaryRenderE.DrawToThisCanvas(p.InnerCanvas,
                new Rectangle(0, 0, vscbar.Width, vscbar.Height));
        }
        public void EvaluateScrollBar()
        {

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
                get { return (int)this.cssbox.SizeWidth; }
            }

            int IScrollable.ViewportHeight
            {
                get { return (int)this.cssbox.SizeHeight; }
            }
            int IScrollable.DesiredHeight
            {
                //content height of the cssbox
                get { return cssbox.InnerContentHeight; }
            }

            int IScrollable.DesiredWidth
            {
                //content width of the cssbox
                get { return cssbox.InnerContentWidth; }
            }

            event EventHandler IScrollable.LayoutFinished
            {
                add { }
                remove { }
            }
        }

    }
}