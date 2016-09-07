//BSD, 2014-2016, WinterDev 

namespace LayoutFarm.HtmlBoxes
{
    public class CssBlockRun : CssRun
    {
        CssBox contentBlockBox;
        public CssBlockRun(CssBox contentBlockBox)
            : base(CssRunKind.BlockRun)
        {
            this.contentBlockBox = contentBlockBox;
        }
        public CssBox ContentBox
        {
            get { return this.contentBlockBox; }
        }
        public override void WriteContent(System.Text.StringBuilder stbuilder, int start, int length)
        {
            throw new System.NotImplementedException();
        }
    }
}