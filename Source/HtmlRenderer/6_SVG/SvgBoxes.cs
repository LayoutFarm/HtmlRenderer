//BSD 2014,WinterDev

using System.Drawing;
using HtmlRenderer.Drawing;

namespace HtmlRenderer.Boxes.Svg
{

    public sealed class SvgRootBox : CssBox
    {
        public SvgRootBox(CssBox owner, object controller, Css.BoxSpec spec)
            : base(owner, controller, spec, Css.CssDisplay.Block)
        {

        } 
    }
}