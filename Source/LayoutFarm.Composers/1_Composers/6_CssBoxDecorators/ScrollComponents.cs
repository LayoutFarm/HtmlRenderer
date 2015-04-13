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
        CssBox scrollView;
        MyCssBoxDescorator myCssBoxDecor;
        public ScrollComponent(MyCssBoxDescorator myCssBoxDecor)
        {
            this.myCssBoxDecor = myCssBoxDecor;
        }
        public void EvaluateScrollBar()
        {

        }
    }
}