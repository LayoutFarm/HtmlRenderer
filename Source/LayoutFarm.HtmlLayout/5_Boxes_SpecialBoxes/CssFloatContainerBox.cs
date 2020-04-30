//BSD, 2014-present, WinterDev 


using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    class CssFloatContainerBox : CssBox
    {
        public CssFloatContainerBox(Css.BoxSpec boxSpec, Css.CssDisplay display)
            : base(boxSpec, display)
        {
        }
        internal override bool JustTempContainer => true;
    }
}