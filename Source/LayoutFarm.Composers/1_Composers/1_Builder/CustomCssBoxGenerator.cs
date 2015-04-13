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
        protected abstract HtmlHost MyHost { get; }
        public abstract CssBox CreateCssBox(LayoutFarm.WebDom.DomElement tag, CssBox parentBox, BoxSpec spec, LayoutFarm.RootGraphic rootgfx);
         
        public CssBox CreateWrapper(object owner, RenderElement renderElement, BoxSpec spec, bool isInline)
        {
            if (isInline)
            {
                return new LayoutFarm.HtmlBoxes.InternalWrappers.WrapperInlineCssBox(owner, spec, renderElement.Root, renderElement); 
            }
            else
            {
                return new LayoutFarm.HtmlBoxes.InternalWrappers.WrapperBlockCssBox(owner, spec, renderElement);
            }
        }
         
    }

   

}