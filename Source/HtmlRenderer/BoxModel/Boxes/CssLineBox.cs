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

        public float Bottom
        {
            get { return this._y + _height; }
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
        public void UpdateStripBoundToOwnerBox()
        {
            owner.UpdateStripInfo(this.Bound);
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
            _ownerBox.AddLineBox(this);

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
            //update summary bound 
            foreach (PartialBoxStrip strip in this._boxStrips.Values)
            {
                strip.UpdateStripBoundToOwnerBox();
            }
            //-----------------------------------------------

            this.dbugIsClosed = true;
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
            this.CachedLineContentWidth = contentRight;

            if (this.OwnerBox.SizeWidth < contentRight)
            {
                this.CachedLineContentWidth = this.OwnerBox.SizeWidth;
            }

        }

        /// <summary>
        /// Get the bottom of this box line (the max bottom of all the words)
        /// </summary>
        public float ReCalculateLineBottom()
        {
            float bottom = 0;
            foreach (var strip in _boxStrips.Values)
            {
                bottom = Math.Max(bottom, strip.Bottom);
            }
            return bottom;
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
                    //case CssConstants.Sub:
                    case CssVerticalAlign.Sub:
                        {
                            this.SetBaseLine(rstripOwnerBox, baseline + rstrip.Height * .2f);
                        } break;
                    //case CssConstants.Super:
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
        /// offset 
        /// </summary>
        /// <param name="targetCssBox"></param>
        /// <param name="topOffset"></param>
        internal void OffsetTopStrip(CssBox targetCssBox, float topOffset)
        {
            PartialBoxStrip found;
            if (this._boxStrips.TryGetValue(targetCssBox, out found))
            {
                found.Offset(0, topOffset);
            }

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
        /// <param name="word"></param>
        internal void AddRun(CssRun word)
        {
#if DEBUG
            if (this.dbugIsClosed)
            {
                throw new NotSupportedException();
            }
#endif
            this._runs.Add(word);//each word has only one owner linebox! 
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
        //internal void AddStripInfo(CssBox owner, float x, float y, float width, float height)
        //{
        //    //this._boxStrips.Add(owner, new PartialBoxStrip(owner, x, y, width, height));
        //}
        //        internal void PaintLine(IGraphics g, PointF offset)
        //        {

        //            PaintBackgroundAndBorder(g, offset);

        //            PaintRuns(g, offset);
        //            PaintDecoration(g, offset);
        //#if DEBUG
        //            dbugPaintRuns(g, offset);
        //#endif
        //        }
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
                            //g.DrawString(w.Text, font,
                            //    color, wordPoint,
                            //    new SizeF(w.Width, w.Height));
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
            return;
            foreach (CssRun w in this._runs)
            { 
                g.DrawRectangle(Pens.DeepPink, w.Left + offset.X, w.Top + offset.Y, w.Width, w.Height);
            }
             
        }
        internal void dbugPaintStrips(IGraphics g, PointF offset)
        {
            //foreach (var kp in this._boxStrips)
            //{
            //    var ownerBox = kp.Key;
            //    var rect = kp.Value.rectF; 
            //    rect.Offset(offset);
            //    //if (ownerBox.CssDisplay != CssBoxDisplayType.Inline)
            //    //{
            //    //    continue;
            //    //}
            //    //-------------------
            //    //debug line
            //    g.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
            //    //------------------- 
            //}
        }
#endif
        internal void PaintSelectedArea(IGraphics g, PointF offset)
        {
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
                g.FillRectangle(Brushes.LightGray, stripArea.X, stripArea.Y, stripArea.Width, stripArea.Height);

            }
        }
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
                //-------------------
                //debug line
                // g.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
                //------------------- 
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
                //-------------------
                //debug line
                // g.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
                //------------------- 
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

        //public float GetBaseLineHeight(CssBox b, Graphics g)
        //{
        //    Font f = b.ActualFont;
        //    FontFamily ff = f.FontFamily;
        //    FontStyle s = f.Style;
        //    return f.GetHeight(g) * ff.GetCellAscent(s) / ff.GetLineSpacing(s);
        //} 
    }



    //class CssStripCollection
    //{
    //    Dictionary<StripKey, PartialBoxStrip> strips = new Dictionary<StripKey, PartialBoxStrip>();

    //    public PartialBoxStrip GetOrCreateStripInfo(CssLineBox lineBox,
    //        CssBox box, out bool isNew)
    //    {
    //        PartialBoxStrip result;
    //        StripKey key = new StripKey(lineBox, box);
    //        if (isNew = !this.strips.TryGetValue(key, out result))
    //        {
    //            //not exist then create
    //            result = new PartialBoxStrip(box);
    //            this.strips.Add(key, result);
    //        }
    //        return result;
    //    }
    //    struct StripKey
    //    {
    //        public readonly CssLineBox lineBox;
    //        public readonly CssBox box;
    //        public StripKey(CssLineBox lineBox, CssBox box)
    //        {
    //            this.lineBox = lineBox;
    //            this.box = box;
    //        }

    //    }
    //}
}