//BSD, 2014-present, WinterDev 

using System.Text;

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
        public CssBox ContentBox => _contentBlockBox;

        public override void WriteContent(System.Text.StringBuilder stbuilder, int start, int length)
        {
            //nothing to write?
#if DEBUG
            System.Diagnostics.Debug.WriteLine("write_content: on CssBlockRun");
#endif
            
        }
        public override void WriteContent(StringBuilder stbuilder, int start = 0)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("write_content: on CssBlockRun");
#endif

        }
    }
}