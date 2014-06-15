//BSD 2014, WinterCore

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.Drawing;

namespace HtmlRenderer.Dom
{
    //--------------------------
    class PartialBoxStrip
    {
        internal readonly CssBox owner;
        float _x;
        float _y;
        float _width;
        float _height;

        public PartialBoxStrip(CssBox owner)
        {
            this.owner = owner;
        }

        public float Left
        {
            get { return this._x; }
        }
        public float Top
        {
            get { return this._y; }
        }
        public float Width
        {
            get { return this._width; }
        }
        public float Height
        {
            get { return this._height; }
        }

        public float Bottom
        {
            get { return this._y + _height; }
        }
       
        public RectangleF Bound
        {
            get { return new RectangleF(this._x, this._y, this.Width, this.Height); }
        }
        public void Offset(float xdiff, float ydiff)
        {
            this._x += xdiff;
            this._y += ydiff;
        }
        public void UpdateStripBound(float x, float y, float w, float h)
        {
            this._x = x;
            this._y = y;
            this._width = w;
            this._height = h;
        }
        
    }



    /// <summary>
    /// Represents a line of text.
    /// </summary>
    /// <remarks>
    /// To learn more about line-boxes see CSS spec:
    /// http://www.w3.org/TR/CSS21/visuren.html
    /// </remarks>
    sealed class CssLineBox
    {
        readonly CssBox _ownerBox;

        //a run may come from another CssBox (not from _ownerBox)
        readonly List<CssRun> _runs;
        //linebox and PartialBoxStrip is 1:1 relation 
        //a CssBox (Inline-splittable) may be splitted into many CssLineBoxes

        /// <summary>
        /// handle part of cssBox in this line, handle task about bg/border/bounday of cssBox owner of strip
        /// </summary>
        readonly Dictionary<CssBox, PartialBoxStrip> _boxStrips;

        internal LinkedListNode<CssLineBox> linkedNode;
#if DEBUG
        bool dbugIsClosed;
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;

#endif
        /// <summary>
        /// Creates a new LineBox
        /// </summary>
        public CssLineBox(CssBox ownerBox)
        {
            _boxStrips = new Dictionary<CssBox, PartialBoxStrip>();
            _runs = new List<CssRun>();
            _ownerBox = ownerBox;

        }
        internal CssLineBox NextLine
        {
            get
            {
                var nn = this.linkedNode.Next;
                if (nn == null)
                {
                    return null;
                }
                else
                {
                    return nn.Value;
                }
            }
        }
        internal float CachedLineBottom
        {
            get;
            private set;
        }
        internal float CacheLineHeight
        {
            get;
            private set;
        }
        internal float CachedLineTop
        {
            get;
            private set;
        }
        internal float CachedLineContentWidth
        {
            get;
            private set;
        }
        internal void CloseLine()
        {  
#if DEBUG
            this.dbugIsClosed = true;
#endif

            float height = 0;
            float bottom = 0;
            float contentRight = 0;

            foreach (PartialBoxStrip strip in _boxStrips.Values)
            {
                height = Math.Max(height, strip.Height);
                bottom = Math.Max(bottom, strip.Bottom);
                contentRight = Math.Max(contentRight, strip.Left + strip.Width);
            }
            this.CacheLineHeight = height;
            this.CachedLineBottom = bottom;
            this.CachedLineTop = bottom - height;
            this.CachedLineContentWidth = contentRight - this.OwnerBox.LocationX;

            if (this.OwnerBox.SizeWidth < CachedLineContentWidth)
            {
                this.CachedLineContentWidth = this.OwnerBox.SizeWidth;
            }
        }
        internal void OffsetTop(float ydiff)
        {
            float height = 0;
            float bottom = 0;
            float contentRight = 0;
            foreach (PartialBoxStrip strip in this._boxStrips.Values)
            {
                strip.Offset(0, ydiff);
                

                height = Math.Max(height, strip.Height);
                bottom = Math.Max(bottom, strip.Bottom);
                contentRight = Math.Max(contentRight, strip.Left + strip.Width);
            }

            foreach (CssRun run in this._runs)
            {
                run.OffsetY(ydiff);
            }

            this.CacheLineHeight = height;
            this.CachedLineBottom = bottom;
            this.CachedLineTop = bottom - height;
            this.CachedLineContentWidth = contentRight - this.OwnerBox.LocationX;

            if (this.OwnerBox.SizeWidth < CachedLineContentWidth)
            {
                this.CachedLineContentWidth = this.OwnerBox.SizeWidth;
            }

        }

        public bool HitTest(int x, int y)
        {
            if (y >= this.CachedLineTop && y <= this.CachedLineBottom)
            {
                return true;
            }
            return false;
        }

        public float CalculateTotalBoxBaseLine()
        {
            float baseline = Single.MinValue;
            foreach (var strip in this._boxStrips.Values)  //iter from word in this line
            {
                baseline = Math.Max(baseline, strip.Top);//?top 
            }
            return baseline;
        }
        public void ApplyBaseline(float baseline)
        {
            //Important notes on http://www.w3.org/TR/CSS21/tables.html#height-layout
            //iterate from rectstrip
            //In a single LineBox ,  CssBox:RectStrip => 1:1 relation 

            foreach (PartialBoxStrip rstrip in this._boxStrips.Values)
            {
                CssBox rstripOwnerBox = rstrip.owner;
                switch (rstripOwnerBox.VerticalAlign)
                {

                    case CssVerticalAlign.Sub:
                        {
                            this.SetBaseLine(rstripOwnerBox, baseline + rstrip.Height * .2f);
                        } break;
                    case CssVerticalAlign.Super:
                        {
                            this.SetBaseLine(rstripOwnerBox, baseline - rstrip.Height * .2f);
                        } break;
                    case CssVerticalAlign.TextTop:
                    case CssVerticalAlign.TextBottom:
                    case CssVerticalAlign.Top:
                    case CssVerticalAlign.Bottom:
                    case CssVerticalAlign.Middle:
                        break;
                    default:
                        //case: baseline
                        this.SetBaseLine(rstripOwnerBox, baseline);
                        break;
                }
            }
        }

        public IEnumerable<float> GetAreaStripTopPosIter()
        {
            foreach (var r in this._boxStrips.Values)
            {
                yield return r.Top;
            }
        }
        internal int WordCount
        {
            get
            {
                return this._runs.Count;
            }
        }
        internal CssRun GetRun(int index)
        {
            return this._runs[index];
        }
        internal CssRun GetFirstRun()
        {
            return this._runs[0];
        }
        internal CssRun GetLastRun()
        {
            return this._runs[this._runs.Count - 1];
        }
         

        /// <summary>
        /// Gets the owner box
        /// </summary>
        public CssBox OwnerBox
        {
            get { return _ownerBox; }
        }
        /// <summary>
        /// Lets the linebox add the word an its box to their lists if necessary.
        /// </summary>
        /// <param name="run"></param>
        internal void AddRun(CssRun run)
        {
#if DEBUG
            if (this.dbugIsClosed)
            {
                throw new NotSupportedException();
            }
#endif
            this._runs.Add(run);//each word has only one owner linebox! 
            CssRun.SetHostLine(run, this);
        }
        internal IEnumerable<CssRun> GetRunIter(CssBox box)
        {
            List<CssRun> tmpRuns = this._runs;
            int j = tmpRuns.Count;
            CssRun run = null;
            for (int i = 0; i < j; ++i)
            {
                if ((run = tmpRuns[i]).OwnerBox == box)
                {
                    yield return run;
                }
            }
        }
        internal IEnumerable<CssRun> GetRunIter()
        {
            List<CssRun> tmpRuns = this._runs;
            int j = tmpRuns.Count;
            for (int i = 0; i < j; ++i)
            {
                yield return tmpRuns[i];
            }
        }
        /// <summary>
        /// Updates the specified rectangle of the specified box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r"></param>
        /// <param name="b"></param>
        internal void BubbleStripUpdate(CssBox box, float x, float y, float r, float b)
        {
            //bubble up***
            //recursive up ***

            float leftspacing = box.ActualBorderLeftWidth + box.ActualPaddingLeft;
            float rightspacing = box.ActualBorderRightWidth + box.ActualPaddingRight;
            float topspacing = box.ActualBorderTopWidth + box.ActualPaddingTop;
            float bottomspacing = box.ActualBorderBottomWidth + box.ActualPaddingTop;

            if (box.FirstHostingLineBox == this || box.IsImage) x -= leftspacing;
            if (box.LastHostingLineBox == this || box.IsImage) r += rightspacing;


            if (!box.IsImage)
            {
                y -= topspacing;
                b += bottomspacing;
            }


            PartialBoxStrip strip;
            if (!this._boxStrips.TryGetValue(box, out strip))
            {
                //new 
                strip = new PartialBoxStrip(box);
                strip.UpdateStripBound(x, y, r - x, b - y);
                this._boxStrips.Add(box, strip);
            }
            else
            {
                RectangleF bound = strip.Bound;
                float sX;
                float sY;

                strip.UpdateStripBound(
                    sX = Math.Min(bound.X, x), //left
                    sY = Math.Min(bound.Y, y),//top
                    Math.Max(bound.Right, r) - sX, //width
                    Math.Max(bound.Bottom, b) - sY);//height 
            }
            if (box.ParentBox != null && box.ParentBox.IsInline)
            {
                //recursive up ***
                BubbleStripUpdate(box.ParentBox, x, y, r, b);
            }
        }

        internal void PaintRuns(IGraphics g, PointF offset)
        {
            //iterate from each words

            CssBox latestOwner = null;
            Font font = null;
            Color color = Color.Empty;

            foreach (CssRun w in this._runs)
            {

                switch (w.Kind)
                {
                    case CssRunKind.Image:
                        {
                            CssBoxImage owner = (CssBoxImage)w.OwnerBox;
                            owner.PaintImage(g, offset, w);

                        } break;
                    case CssRunKind.Text:
                        {
                            if (latestOwner != w.OwnerBox)
                            {
                                latestOwner = w.OwnerBox;
                                font = latestOwner.ActualFont;
                                color = latestOwner.ActualColor;
                            }
                            CssTextRun textRun = (CssTextRun)w;
                            var wordPoint = new PointF(w.Left + offset.X, w.Top + offset.Y);

                            char[] ownerBuffer = CssBox.UnsafeGetTextBuffer(w.OwnerBox);

                            g.DrawString2(ownerBuffer,
                               textRun.TextStartIndex,
                               textRun.TextLength, font,
                               color, wordPoint,
                               new SizeF(w.Width, w.Height));

                        } break;
                    default:
                        {
#if DEBUG
                            // w.OwnerBox.dbugPaintTextWordArea(g, offset, w);
#endif
                        } break;
                }
            }
        }

#if DEBUG

        internal void dbugPaintRuns(IGraphics g, PointF offset)
        {
            //return;
            //linebox //draw diagonal
            float x1 = this.OwnerBox.LocationX + offset.X;
            float y1 = this.CachedLineTop + offset.Y;
            float x2 = x1 + this.CachedLineContentWidth;
            float y2 = y1 + this.CacheLineHeight;

            g.DrawRectangle(Pens.Blue, x1, y1, x2 - x1, y2 - y1);
            g.DrawLine(Pens.Blue, x1, y1, x2, y2);
            g.DrawLine(Pens.Blue, x1, y2, x2, y1);

            //g.DrawRectangle(Pens.Blue,
            //    this.OwnerBox.LocationX,
            //    this.CachedLineTop,
            //    this.CachedLineContentWidth,
            //    this.CacheLineHeight);

            //return;
            foreach (CssRun w in this._runs)
            {
                g.DrawRectangle(Pens.DeepPink, w.Left + offset.X, w.Top + offset.Y, w.Width, w.Height);
            }

        }

#endif
         
        internal void PaintBackgroundAndBorder(IGraphics g, PointF offset)
        {
            //iterate each strip
            foreach (PartialBoxStrip strip in this._boxStrips.Values)
            {
                var ownerBox = strip.owner;
                if (ownerBox.CssDisplay != CssDisplay.Inline)
                {
                    throw new NotSupportedException();
                    continue;
                }

                var stripArea = strip.Bound;
                stripArea.Offset(offset);
                if (ownerBox.AllPartsAreInTheSameLineBox)
                {
                    //if first line = last line 
                    //then this box is on single line
                    if (ownerBox.FirstHostingLineBox != null)
                    {

                        ownerBox.PaintBackground(g, stripArea, false, false);
                        //g.DrawRectangle(Pens, rect.X, rect.Y, rect.Width, rect.Height);//debug
                        HtmlRenderer.Handlers.BordersDrawHandler.DrawBoxBorders(g, ownerBox, stripArea, false, false);
                    }
                    else
                    {

                        ownerBox.PaintBackground(g, stripArea, true, true);
                        //g.DrawRectangle(Pens.DeepPink, rect.X, rect.Y, rect.Width, rect.Height); //debug
                        HtmlRenderer.Handlers.BordersDrawHandler.DrawBoxBorders(g, ownerBox, stripArea, true, true);
                    }
                }
                else
                {
                    //this box has multiple rect 
                    if (ownerBox.FirstHostingLineBox == this)
                    {
                        ownerBox.PaintBackground(g, stripArea, true, false);
                        //g.DrawRectangle(Pens.Red, rect.X, rect.Y, rect.Width, rect.Height); //debug
                        HtmlRenderer.Handlers.BordersDrawHandler.DrawBoxBorders(g, ownerBox, stripArea, true, false);
                    }
                    else
                    {
                        ownerBox.PaintBackground(g, stripArea, false, true);
                        //g.DrawRectangle(Pens.Green, rect.X, rect.Y, rect.Width, rect.Height); //debug
                        HtmlRenderer.Handlers.BordersDrawHandler.DrawBoxBorders(g, ownerBox, stripArea, false, true);
                    }
                }
            }
        }

        internal void PaintDecoration(IGraphics g, PointF offset)
        {
            foreach (PartialBoxStrip strip in this._boxStrips.Values)
            {
                CssBox ownerBox = strip.owner;
                if (ownerBox.CssDisplay != CssDisplay.Inline)
                {
                    continue;
                }

                var rect = strip.Bound;
                rect.Offset(offset);
                if (ownerBox.FirstHostingLineBox == ownerBox.LastHostingLineBox)
                {
                    //if first line = last line 
                    //then this box is on single line
                    if (ownerBox.FirstHostingLineBox != null)
                    {
                        ownerBox.PaintDecoration(g, rect, false, false);
                    }
                    else
                    {
                        ownerBox.PaintDecoration(g, rect, true, true);
                    }
                }
                else
                {
                    //this box has multiple rect 
                    if (ownerBox.FirstHostingLineBox == this)
                    {
                        ownerBox.PaintDecoration(g, rect, true, false);
                    }
                    else
                    {
                        ownerBox.PaintDecoration(g, rect, false, true);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the baseline Height of the rectangle
        /// </summary>
        /// <param name="b"> </param>
        /// <param name="g"></param>
        /// <returns></returns>

        void SetBaseLine(CssBox stripOwnerBox, float baseline)
        {
            float newtop = baseline;
            foreach (var word in this.GetRunIter(stripOwnerBox))
            {
                if (!word.IsImage)
                {
                    word.Top = newtop;
                }
            }
        }

        /// <summary>
        /// Returns the words of the linebox
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            int j = _runs.Count;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < j; i++)
            {
                sb.Append(_runs[i].Text);
            }
            return sb.ToString();
        }
    }
}
