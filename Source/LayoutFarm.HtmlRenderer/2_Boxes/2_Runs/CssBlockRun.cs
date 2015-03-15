//BSD 2014-2015,WinterDev
using PixelFarm.Drawing;

namespace LayoutFarm.HtmlBoxes
{
    class CssBlockRun : CssRun
    {
        CssBox contentBlockBox;
        public CssBlockRun(CssBox contentBlockBox)
            : base(CssRunKind.BlockRun)
        {
            this.contentBlockBox = contentBlockBox;
        }
        public CssBox BlockBox
        {
            get { return this.contentBlockBox; }
        }
        

    }
}