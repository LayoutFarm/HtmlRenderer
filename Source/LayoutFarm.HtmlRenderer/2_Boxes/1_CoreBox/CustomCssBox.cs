using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using System.Globalization;
using System.Text;
using LayoutFarm.Css;

namespace LayoutFarm.HtmlBoxes
{

    public abstract class CustomCssBox : CssBox
    {
        public CustomCssBox(object controller, BoxSpec boxspec, RootGraphic rootgfx, CssDisplay fixDisplayType)
            : base(controller, boxspec, rootgfx, fixDisplayType)
        {
            SetAsCustomCssBox(this);
        }
        public abstract void CustomRecomputedValue(CssBox containingBlock, GraphicsPlatform gfxPlatform);
        public abstract bool CustomContentHitTest(float x, float y, CssBoxHitChain hitChain);

    }
}