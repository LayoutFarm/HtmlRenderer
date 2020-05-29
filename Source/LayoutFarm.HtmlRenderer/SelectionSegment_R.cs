//BSD, 2014-present, WinterDev 


namespace LayoutFarm.HtmlBoxes
{
    
    partial class SelectionSegment
    { 
        public void PaintSelection(PaintVisitor p, CssLineBox line)
        {
            if (this.Kind == SelectionSegmentKind.FullLine)
            {
                p.FillRectangle(p.CssBoxSelectionColor, //should  be configurable
                       0,
                       0,
                       line.CachedLineContentWidth,
                       line.CacheLineHeight);

            }
            else
            {
                p.FillRectangle(
                      p.CssBoxSelectionColor,
                      this.BeginAtPx, 0,
                      this.WidthPx,
                      (int)line.CacheLineHeight);
            }
        }
    }
}