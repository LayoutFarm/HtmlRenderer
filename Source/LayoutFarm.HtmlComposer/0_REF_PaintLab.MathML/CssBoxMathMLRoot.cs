//MS-PL, Apache2, 2014-present, WinterDev
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PaintLab.Svg;

namespace LayoutFarm.HtmlBoxes
{

    public sealed class CssBoxMathMLRoot : CssBox
    {
        //
        public CssBoxMathMLRoot(Css.BoxSpec spec)
         : base(spec, Css.CssDisplay.Block)
        {
        }
    }
}