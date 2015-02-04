// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;

namespace LayoutFarm.HtmlWidgets
{

    public abstract class HtmlWidgetBase
    {

        public HtmlWidgetBase(int w, int h)
        {
        }
        public abstract DomElement GetPrimaryDomElement();
    }
}