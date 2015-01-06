//BSD 2014, WinterDev 
using System.Collections.Generic;
using System.Globalization;
using LayoutFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.Boxes;

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