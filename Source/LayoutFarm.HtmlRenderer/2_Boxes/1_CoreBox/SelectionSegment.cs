//BSD 2014 ,WinterDev 
using System;
using System.Collections.Generic;
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
        public static readonly SelectionSegment AllLineSelection = new SelectionSegment();

        private SelectionSegment()
        {
            this.Kind = SelectionSegmentKind.FullLine;
        }
        public SelectionSegment(int beginAtPixel, int width)
        {
            this.Kind = SelectionSegmentKind.Partial;
            this.BeginAtPixel = beginAtPixel;
            this.WidthPx = width;
        }

        public SelectionSegmentKind Kind { get; private set; }
        public void Clear()
        {

        }
        public int BeginAtPixel { get; private set; }
        public int WidthPx { get; private set; }

        public int BeginAtCharIndex { get; set; }

        public int EndAtCharIndex { get; set; }


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
                 this.BeginAtPixel, 0,
                 this.WidthPx,
                (int)line.CacheLineHeight);
            }
        }
    }

}