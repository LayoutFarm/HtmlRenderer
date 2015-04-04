//BSD 2014 ,WinterDev 
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace LayoutFarm.HtmlBoxes
{
    enum SelectionSegmentKind
    {
        AllLine,
        SingleSegmentPerLine,
        MultipleSegmentPerLine
    }


    class SelectionSegment
    {
        public static readonly SelectionSegment AllLineSelection = new SelectionSegment() { Kind = SelectionSegmentKind.AllLine };

        public SelectionSegment()
        {
            this.Kind = SelectionSegmentKind.SingleSegmentPerLine;
        }
        public SelectionSegment(int beginAtPixel, int width)
        {
            this.Kind = SelectionSegmentKind.SingleSegmentPerLine;
            this.BeginAtPixel = beginAtPixel;
            this.EndAtPixel = beginAtPixel + width;
        }
        protected SelectionSegment(SelectionSegmentKind kind)
        {
            this.Kind = kind;
        }
        public SelectionSegmentKind Kind { get; private set; }
        public virtual void Clear()
        {
        }
        public int BeginAtCharIndex { get; set; }
        public int BeginAtPixel { get; set; }
        public int EndAtCharIndex { get; set; }
        public int EndAtPixel { get; set; }

        public virtual void PaintSelection(PaintVisitor p, int lineHeight)
        {
            p.FillRectangle(
               Color.LightGray,
               this.BeginAtPixel, 0,
               this.EndAtPixel - this.BeginAtPixel,
               lineHeight);
        }
    }

    class MultiSegmentPerLine : SelectionSegment
    {
        List<SelectionSegment> segmentList = new List<SelectionSegment>();
        public MultiSegmentPerLine()
            : base(SelectionSegmentKind.MultipleSegmentPerLine)
        {
        }
        public void AddSubSegment(SelectionSegment segment)
        {
            this.segmentList.Add(segment);
        }
        public override void Clear()
        {
            this.segmentList.Clear();
        }
        public override void PaintSelection(PaintVisitor p, int lineHeight)
        {
            //paint each segment
            int j = segmentList.Count;
            for (int i = 0; i < j; ++i)
            {
                segmentList[i].PaintSelection(p, lineHeight);
            }
        }

    }

}