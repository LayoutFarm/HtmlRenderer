//BSD 2014, WinterDev 
using System.Collections.Generic;
using System.Globalization;
using HtmlRenderer.Drawing;
using HtmlRenderer.WebDom;
using HtmlRenderer.Css;
using HtmlRenderer.Boxes;

namespace HtmlRenderer.Composers
{

    public abstract class CustomCssBoxGenerator
    {
        public abstract CssBox CreateCssBox(object tag, CssBox parentBox, BoxSpec spec);
    }

   
}