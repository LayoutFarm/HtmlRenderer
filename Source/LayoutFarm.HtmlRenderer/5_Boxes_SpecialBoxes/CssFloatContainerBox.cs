//BSD, 2014-2017, WinterDev


using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    class CssFloatContainerBox : CssBox
    {
        public CssFloatContainerBox(Css.BoxSpec boxSpec, RootGraphic rootgfx, Css.CssDisplay display)
            : base(boxSpec, rootgfx, display)
        {
        }
        internal override bool JustTempContainer
        {
            get
            {
                return true;
            }
        }
    }
}