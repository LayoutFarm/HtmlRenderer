//BSD, 2014-present, WinterDev 

namespace LayoutFarm.HtmlBoxes
{
    public class CssBlockRun : CssRun
    {
        CssBox _contentBlockBox;
        public CssBlockRun(CssBox contentBlockBox)
            : base(CssRunKind.BlockRun)
        {
            _contentBlockBox = contentBlockBox;
        }
        public CssBox ContentBox=>_contentBlockBox; 
        
        public override void WriteContent(System.Text.StringBuilder stbuilder, int start, int length)
        {
            throw new System.NotImplementedException();
        }
    }
}