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
        public PartialBoxStrip(CssBox owner, float x, float y, float width, float height)
        {
            this.owner = owner;
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
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
        public RectangleF RectF
        {
            get { return new RectangleF(this._x, this._y, this.Width, this.Height); }
        }
        public void Offset(float xdiff, float ydiff)
        {
            this._x += xdiff;
            this._y += ydiff;
        }
        public void UpdateStripBound(RectangleF rectF)
        {
            this._x = rectF.X;
            this._y = rectF.Y;
            this._width = rectF.Width;
            this._height = rectF.Height;
        }
        public void UpdateStripBoundToOwnerBox()
        {
            owner.UpdateStripInfo(this.RectF);
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
        readonly Dictionary<CssBox, PartialBoxStrip> _boxStrips;

        internal LinkedListNode<CssLineBox> linkedNode;

        bool isClosed;

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
        internal void CloseCollection()
        {
            //update summary bound

            foreach (PartialBoxStrip strip in this.Rectangles.Values)
            {
                strip.UpdateStripBoundToOwnerBox();

            }
            //-----------------------------------------------

            this.isClosed = true;
            float height = 0;
            float bottom = 0;

            foreach (var rect in _boxStrips.Values)
            {
                height = Math.Max(height, rect.Height);
                bottom = Math.Max(bottom, rect.Bottom);
            }
            this.CacheLineHeight = height;
            this.CachedLineBottom = bottom;
            this.CachedLineTop = bottom - height;
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

        /// <summary>
        /// Gets the words inside the linebox
        /// </summary>
        List<CssRun> Words
        {
            get { return _runs; }
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
        internal IEnumerable<CssRun> GetRectWordIter()
        {
            foreach (var w in this._runs)
            {
                yield return w;
            }
        }
        //internal bool ContainsRun(CssRun run)
        //{
        //    return this.Words.Contains(run);
        //}
        /// <summary>
        /// offset 
        /// </summary>
        /// <param name="targetCssBox"></param>
        /// <param name="topOffset"></param>
        internal void OffsetTopRectsOf(CssBox targetCssBox, float topOffset)
        {
            PartialBoxStrip r;
            if (this._boxStrips.TryGetValue(targetCssBox, out r))
            {
                //var rr = r.rectF;
                //rr.Offset(0, topOffset);
                //r.rectF = rr;
                r.Offset(0, topOffset);
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
        /// Gets a List of rectangles that are to be painted on this linebox
        /// </summary>
        Dictionary<CssBox, PartialBoxStrip> Rectangles
        {
            get { return _boxStrips; }
        }

        /// <summary>
        /// Get the height of this box line (the max height of all the words)
        /// </summary>
        public float LineHeight
        {
            get
            {
                float height = 0;
                foreach (var strip in _boxStrips.Values)
                {
                    height = Math.Max(height, strip.Height);
                }
                return height;
            }
        }



        /// <summary>
        /// Lets the linebox add the word an its box to their lists if necessary.
        /// </summary>
        /// <param name="word"></param>
        internal void AddRun(CssRun word)
        {   
            Words.Add(word);//each word has only one owner linebox! 
        }
        internal IEnumerable<CssRun> GetWordIterOf(CssBox box)
        {
            foreach (CssRun word in this.Words)
            {
                if (word.OwnerBox == box)
                {
                    yield return word;
                }
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
        internal void BubbleUpdateRectangle(CssBox box, float x, float y, float r, float b)
        {
            //bubble up***
            //recursive up ***

            float leftspacing = box.ActualBorderLeftWidth + box.ActualPaddingLeft;
            float rightspacing = box.ActualBorderRightWidth + box.ActualPaddingRight;
            float topspacing = box.ActualBorderTopWidth + box.ActualPaddingTop;
            float bottomspacing = box.ActualBorderBottomWidth + box.ActualPaddingTop;

            if ((box.FirstHostingLineBox != null && box.FirstHostingLineBox.Equals(this)) || box.IsImage) x -= leftspacing;
            if ((box.LastHostingLineBox != null && box.LastHostingLineBox.Equals(this)) || box.IsImage) r += rightspacing;

            if (!box.IsImage)
            {
                y -= topspacing;
                b += bottomspacing;
            }
            PartialBoxStrip existingRect;
            if (!Rectangles.TryGetValue(box, out existingRect))
            {
                Rectangles.Add(box, new PartialBoxStrip(box, x, y, r - x, b - y));
            }
            else
            {
                //update                
                RectangleF existingR = existingRect.RectF;
                existingRect.UpdateStripBound(RectangleF.FromLTRB(
                    Math.Min(existingR.X, x), Math.Min(existingR.Y, y),
                    Math.Max(existingR.Right, r), Math.Max(existingR.Bottom, b)));

                //Rectangles[box] = RectangleF.FromLTRB(
                //    Math.Min(existingRect.X, x), Math.Min(existingRect.Y, y),
                //    Math.Max(existingRect.Right, r), Math.Max(existingRect.Bottom, b));
            }

            if (box.ParentBox != null && box.ParentBox.IsInline)
            {
                //recursive up ***
                BubbleUpdateRectangle(box.ParentBox, x, y, r, b);
            }
        }


        internal PartialBoxStrip GetStrip(CssBox box)
        {
            PartialBoxStrip found;
            this._boxStrips.TryGetValue(box, out found);
            return found;
        }
        internal void AddRectStripInfo(CssBox owner, float x, float y, float width, float height)
        {
            this._boxStrips.Add(owner, new PartialBoxStrip(owner, x, y, width, height));
        }
        internal void PaintLine(IGraphics g, PointF offset)
        {

            PaintBackgroundAndBorder(g, offset);
            PaintWords(g, offset);
            PaintDecoration(g, offset);
            //paint decoration
#if DEBUG

            //dbugPaintRects(g, offset);
            //dbugPaintWords(g, offset);
#endif
        }
        void PaintWords(IGraphics g, PointF offset)
        {
            //iterate from each words
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
                            w.OwnerBox.PaintTextWord(g, offset, w);
                        } break;
                    default:
                        {
#if DEBUG
                            w.OwnerBox.dbugPaintTextWordArea(g, offset, w);
#endif
                        } break;
                }
            }
        }

#if DEBUG
        internal void dbugPaintWords(IGraphics g, PointF offset)
        {
            foreach (CssRun w in this._runs)
            {
                var wordPoint = new PointF(w.Left + offset.X, w.Top + offset.Y);
                g.DrawRectangle(Pens.DeepPink, w.Left, w.Top, w.Width, w.Height);
            }
        }
        internal void dbugPaintRects(IGraphics g, PointF offset)
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
        internal void dbugPaintRects2(IGraphics g, PointF offset)
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
            //    g.DrawRectangle(Pens.Red, rect.X, rect.Y, rect.Width, rect.Height);
            //    //------------------- 
            //}
        }
        /// <summary>
        /// Draws the rectangles for debug purposes
        /// </summary>
        /// <param name="g"></param>
        internal void dbugDrawRectangles(Graphics g)
        {
            using (SolidBrush sb = new SolidBrush(Color.FromArgb(50, Color.Black)))
            {
                foreach (var strip in this.Rectangles.Values)
                {
                    RectangleF r = strip.RectF;
                    if (float.IsInfinity(r.Width))
                    {
                        continue;
                    }
                    g.FillRectangle(sb, Rectangle.Round(r));
                    g.DrawRectangle(Pens.Red, Rectangle.Round(r));
                }
            }

        }
#endif

        void PaintBackgroundAndBorder(IGraphics g, PointF offset)
        {
            //iterate from each rect 
            foreach (var strip in this._boxStrips.Values)
            {
                var ownerBox = strip.owner;// kp.Key;
                if (ownerBox.CssDisplay != CssDisplay.Inline)
                {
                    continue;
                }

                var rect = strip.RectF;
                rect.Offset(offset);
                if (ownerBox.FirstHostingLineBox == ownerBox.LastHostingLineBox)
                {
                    //if first line = last line 
                    //then this box is on single line
                    if (ownerBox.FirstHostingLineBox != null)
                    {

                        ownerBox.PaintBackground(g, rect, false, false);
                        //g.DrawRectangle(Pens, rect.X, rect.Y, rect.Width, rect.Height);//debug
                        HtmlRenderer.Handlers.BordersDrawHandler.DrawBoxBorders(g, ownerBox, rect, false, false);
                    }
                    else
                    {
                        ownerBox.PaintBackground(g, rect, true, true);
                        //g.DrawRectangle(Pens.DeepPink, rect.X, rect.Y, rect.Width, rect.Height); //debug
                        HtmlRenderer.Handlers.BordersDrawHandler.DrawBoxBorders(g, ownerBox, rect, true, true);
                    }
                }
                else
                {
                    //this box has multiple rect 
                    if (ownerBox.FirstHostingLineBox == this)
                    {
                        ownerBox.PaintBackground(g, rect, true, false);
                        //g.DrawRectangle(Pens.Red, rect.X, rect.Y, rect.Width, rect.Height); //debug
                        HtmlRenderer.Handlers.BordersDrawHandler.DrawBoxBorders(g, ownerBox, rect, true, false);
                    }
                    else
                    {
                        ownerBox.PaintBackground(g, rect, false, true);
                        //g.DrawRectangle(Pens.Green, rect.X, rect.Y, rect.Width, rect.Height); //debug
                        HtmlRenderer.Handlers.BordersDrawHandler.DrawBoxBorders(g, ownerBox, rect, false, true);
                    }
                }
                //-------------------
                //debug line
                // g.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
                //------------------- 
            }
        }

        void PaintDecoration(IGraphics g, PointF offset)
        {
            foreach (PartialBoxStrip strip in this._boxStrips.Values)
            {   
                CssBox ownerBox = strip.owner;
                if (ownerBox.CssDisplay != CssDisplay.Inline)
                {
                    continue;
                }

                var rect = strip.RectF;
                rect.Offset(offset);
                if (ownerBox.FirstHostingLineBox == ownerBox.LastHostingLineBox)
                {
                    //if first line = last line 
                    //then this box is on single line
                    if (ownerBox.FirstHostingLineBox != null)
                    {
                        ownerBox.PaintDecoration2(g, rect, false, false);

                    }
                    else
                    {
                        ownerBox.PaintDecoration2(g, rect, true, true);
                    }
                }
                else
                {
                    //this box has multiple rect 
                    if (ownerBox.FirstHostingLineBox == this)
                    {
                        ownerBox.PaintDecoration2(g, rect, true, false);
                    }
                    else
                    {
                        ownerBox.PaintDecoration2(g, rect, false, true);
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
            foreach (var word in this.GetWordIterOf(stripOwnerBox))
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
            int j = Words.Count;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < j; i++)
            {
                sb.Append(Words[i].Text);
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
}
