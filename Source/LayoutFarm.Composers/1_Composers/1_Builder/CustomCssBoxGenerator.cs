// 2015,2014 ,BSD, WinterDev 
using System.Collections.Generic;
using System.Globalization;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.Composers
{
    

    public abstract class CustomCssBoxGenerator
    {

        public abstract CssBox CreateCssBox(LayoutFarm.WebDom.DomElement tag,
            CssBox parentBox, BoxSpec spec, HtmlHost host);


        public static CssBox CreateWrapper(object owner, RenderElement renderElement, BoxSpec spec, bool isInline)
        {
            var portalEvent = owner as IEventPortal;
            if (portalEvent == null)
            {
                portalEvent = new RenderElementEventPortal(renderElement);
            }

            if (isInline)
            {
                return new LayoutFarm.HtmlBoxes.InternalWrappers.WrapperInlineCssBox(portalEvent, spec, renderElement.Root, renderElement);
            }
            else
            {
                return new LayoutFarm.HtmlBoxes.InternalWrappers.WrapperBlockCssBox(portalEvent, spec, renderElement);
            }
        }
    }
     

}