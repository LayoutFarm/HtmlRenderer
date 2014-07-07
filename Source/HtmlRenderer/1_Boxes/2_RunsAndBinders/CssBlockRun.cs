//BSD 2014,WinterDev
using System.Drawing;

namespace HtmlRenderer.Dom
{
    class CssBlockRun : CssRun
    {
        CssBox blockBox;
        public CssBlockRun(CssBox blockBox)
            : base(CssRunKind.BlockRun)
        {
            this.blockBox = blockBox;
        }
        public CssBox BlockBox
        {
            get { return this.blockBox; }
        }
        public override CssBox OwnerBox
        {
            get
            {
                return this.blockBox;
            }
        }
    }
}