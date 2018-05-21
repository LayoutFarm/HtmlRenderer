//BSD, 2014-2018, WinterDev 

using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    enum SelectionSegmentKind
    {
        FullLine,
        Partial
    }

    class SelectionSegment
    {
        public static readonly SelectionSegment FullLine = new SelectionSegment();
        private SelectionSegment()
        {
            this.Kind = SelectionSegmentKind.FullLine;
        }
        public SelectionSegment(int beginAtPixel, int width)
        {
            this.Kind = SelectionSegmentKind.Partial;
            this.BeginAtPx = beginAtPixel;
            this.WidthPx = width;
        }

        public SelectionSegmentKind Kind { get; private set; }
        public void Clear()
        {
        }
        public int BeginAtPx { get; private set; }
        public int WidthPx { get; private set; }
        public CssRun StartHitRun { get; set; }
        public int StartHitCharIndex { get; set; }
        public CssRun EndHitRun { get; set; }
        public int EndHitCharIndex { get; set; }

        public void PaintSelection(PaintVisitor p, CssLineBox line)
        {
            if (this.Kind == SelectionSegmentKind.FullLine)
            {
                p.FillRectangle(Color.LightGray,
                       0,
                       0,
                       line.CachedLineContentWidth,
                       line.CacheLineHeight);
            }
            else
            {
                p.FillRectangle(
                 Color.LightGray,
                 this.BeginAtPx, 0,
                 this.WidthPx,
                (int)line.CacheLineHeight);
            }
        }
    }
}