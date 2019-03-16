//BSD, 2014-present, WinterDev 

using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
namespace LayoutFarm.Composers
{
    public abstract class CustomCssBoxGenerator
    {
        public abstract CssBox CreateCssBox(HtmlElement tag,
            CssBox parentBox,
            BoxSpec spec,
            HtmlHost host);
        public static CssBox CreateCssWrapper(HtmlHost htmlhost,
            object owner,
            RenderElement renderElement,
            BoxSpec spec,
            bool isInline)
        {
            var portalEvent = owner as IEventPortal;
            if (portalEvent == null)
            {
                portalEvent = new RenderElementEventPortal2(renderElement);
            }

            if (isInline)
            {
                return new LayoutFarm.HtmlBoxes.InternalWrappers.WrapperInlineCssBox(htmlhost, portalEvent, spec, renderElement.Root, renderElement);
            }
            else
            {
                return new LayoutFarm.HtmlBoxes.InternalWrappers.WrapperBlockCssBox(htmlhost, portalEvent, spec, renderElement);
            }
        }
    }
}