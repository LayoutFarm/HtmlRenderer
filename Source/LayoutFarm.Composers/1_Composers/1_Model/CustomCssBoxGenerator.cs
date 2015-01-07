// 2015,2014 ,BSD, WinterDev 
using System.Collections.Generic;
using System.Globalization;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;

namespace LayoutFarm.Composers
{

    public abstract class CustomCssBoxGenerator
    {
        public abstract CssBox CreateCssBox(object tag, CssBox parentBox, BoxSpec spec, LayoutFarm.RootGraphic rootgfx);

        public CssBox CreateWrapper(object owner, RenderElement renderElement, BoxSpec spec)
        {
            return new RenderElementWrapperCssBox(owner, spec, renderElement);
        }
    }


}