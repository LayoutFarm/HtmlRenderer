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
<<<<<<< HEAD
        CssBox scrollView;
        MyCssBoxDescorator myCssBoxDecor;
        public ScrollComponent(MyCssBoxDescorator myCssBoxDecor)
        {
            this.myCssBoxDecor = myCssBoxDecor;
=======
        CssScrollWrapper scrollView;
        MyCssBoxDescorator myCssBoxDecor;
        ScrollBar scBar;
        public ScrollComponent(MyCssBoxDescorator myCssBoxDecor)
        {
            this.myCssBoxDecor = myCssBoxDecor;
            CssBox cssbox = myCssBoxDecor.TargetBox;
            this.scrollView = new CssScrollWrapper(cssbox);
            //Create 

            scBar = new ScrollBar((int)cssbox.SizeWidth, (int)cssbox.SizeHeight);


>>>>>>> v_err
        }
        public void EvaluateScrollBar()
        {

        }
<<<<<<< HEAD
=======

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
                get { throw new NotImplementedException(); }
            }

            int IScrollable.DesiredWidth
            {
                //content width of the cssbox
                get { throw new NotImplementedException(); }
            }

            event EventHandler IScrollable.LayoutFinished
            {
                add { throw new NotImplementedException(); }
                remove { throw new NotImplementedException(); }
            }
        }

>>>>>>> v_err
    }
}