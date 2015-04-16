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
        protected abstract HtmlHost MyHost { get; }
        public abstract CssBox CreateCssBox(LayoutFarm.WebDom.DomElement tag, CssBox parentBox, BoxSpec spec, LayoutFarm.RootGraphic rootgfx);

        public static CssBox CreateWrapper(object owner, RenderElement renderElement, BoxSpec spec, bool isInline)
        {
            var portalEvent = owner as IUserEventPortal;
            if (portalEvent == null)
            {
                var newporttalEvent = new UserEventPortal();
                newporttalEvent.BindTopRenderElement(renderElement);
                portalEvent = newporttalEvent;
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