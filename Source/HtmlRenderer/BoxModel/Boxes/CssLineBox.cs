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

        public PartialBoxStrip(CssBox owner, float x, float y, float w, float h)
        {
            this.owner = owner;
            this._x = x;
            this._y = y;
            this._width = w;
            this._height = h;
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
        public float Right
        {
            get { return this._x + this._width; }
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
        public void MergeBound(float left, float top, float right, float bottom)
        {

            float sR = this.Right;
            float sB = this.Bottom;

            if (left < this._x)
            {
                this._x = left;
            }
            if (top < this._y)
            {
                this._y = top;
            }
            if (right > sR)
            {
                this._width = right - this._x;
            }
            if (bottom > sB)
            {
                this._height = bottom - this._y;
            }
        }
        public void SetTop(float y)
        {
            this._y = y;
        }
        public void SetLeft(float x)
        {
            this._x = x;
        }

#if DEBUG
        public override string ToString()
        {
            return this.owner.dbugId + " left:" + this.Left + ",width:" + this.Width + " " + this.owner.ToString();
        }
#endif
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
        readonly List<CssRun> _runs = new List<CssRun>();

        //linebox and PartialBoxStrip is 1:1 relation 
        //a CssBox (Inline-splittable) may be splitted into many CssLineBoxes

        /// <summary>
        /// handle part of cssBox in this line, handle task about bg/border/bounday of cssBox owner of strip        
        /// </summary>
        readonly List<PartialBoxStrip> _bottomUpBoxStrips = new List<PartialBoxStrip>();

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

        /// <summary>
        /// relative to owner cssbox
        /// </summary>
        internal float LineBoxTop { get; set; }
        internal float LineBoxHeight { get; set; }
        //---------------------------------

        internal float CachedLineBottom
        {
            get { return this.CachedLineTop + this.CacheLineHeight; }
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

            //=============================================================
            //part 1: MakeStrips()
            //=============================================================
            //***
            var myruns = this._runs;
            CssBox lineOwner = this._ownerBox;
            int j = myruns.Count;

            List<PartialBoxStrip> totalStrips = this._bottomUpBoxStrips;
            //---------------------------------------------------------------------------
            //first level
            Dictionary<CssBox, PartialBoxStrip> dicStrips = new Dictionary<CssBox, PartialBoxStrip>();

            for (int i = 0; i < j; ++i)
            {
                var run = myruns[i];
                if (run.IsSpaces)
                {
                    continue;
                }
                //-------------
                //first level data
                RegisterStripPart(run.OwnerBox, run.Left, run.Top, run.Right, run.Bottom, totalStrips, dicStrips);
            }
            //---------------------------------------------------------------------------
            //other step to upper layer, until no new strip     
            int newStripIndex = 0;
            for (int numNewStripCreate = totalStrips.Count; numNewStripCreate > 0; newStripIndex += numNewStripCreate)
            {
                numNewStripCreate = StepUpRegisterStrips(dicStrips, lineOwner, totalStrips, newStripIndex);
            }
            //=============================================================
            //part 2: CalculateCacheData()
            //=============================================================
            float local_height = 0;
            float local_bottom = 0;
            float contentRight = 0;

            for (int i = _bottomUpBoxStrips.Count - 1; i >= 0; --i)
            {
                var strip = _bottomUpBoxStrips[i];
                local_height = Math.Max(local_height, strip.Height);
                local_bottom = Math.Max(local_bottom, strip.Bottom);
                contentRight = Math.Max(contentRight, strip.Right);
            }

            this.CacheLineHeight = local_height;
            float cacheLineTop = 0;
            this.CachedLineTop = cacheLineTop = local_bottom - local_height;

            //----------------------- 
            //make strip and run related to its line
            for (int i = _bottomUpBoxStrips.Count - 1; i >= 0; --i)
            {
                var strip = _bottomUpBoxStrips[i];
                strip.SetTop(strip.Top - cacheLineTop);
            }
            for (int i = _runs.Count - 1; i >= 0; --i)
            {
                var run = _runs[i];

                run.SetLocation(run.Left, run.Top - cacheLineTop);
            }
            //----------------------- 


            this.CachedLineContentWidth = contentRight - lineOwner.LocationX;

            if (lineOwner.SizeWidth < CachedLineContentWidth)
            {
                this.CachedLineContentWidth = this.OwnerBox.SizeWidth;
            }
        }


        internal void OffsetTop(float ydiff)
        {
             

            this.CachedLineTop += ydiff;

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
            for (int i = _bottomUpBoxStrips.Count - 1; i >= 0; --i)
            {
                baseline = Math.Max(baseline, _bottomUpBoxStrips[i].Top);//?top 
            }

            return baseline;
        }
        public void ApplyBaseline(float baseline)
        {
            //Important notes on http://www.w3.org/TR/CSS21/tables.html#height-layout
            //iterate from rectstrip
            //In a single LineBox ,  CssBox:RectStrip => 1:1 relation             
            for (int i = _bottomUpBoxStrips.Count - 1; i >= 0; --i)
            {
                var rstrip = _bottomUpBoxStrips[i];
                var rstripOwnerBox = rstrip.owner;
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
            for (int i = _bottomUpBoxStrips.Count - 1; i >= 0; --i)
            {
                yield return _bottomUpBoxStrips[i].Top;
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
            //if (run is CssTextRun)
            //{ 
            //    CssTextRun textRun = (CssTextRun)run;
            //    if (textRun.Text.Contains("Cell"))
            //    {

            //    }
            //}
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



        internal void PaintRuns(IGraphics g, PaintingArgs args)
        {
            //iterate from each words


            CssBox latestOwner = null;
            Font font = null;

            Color color = Color.Empty;
            var tmpRuns = this._runs;
            int j = tmpRuns.Count;

            for (int i = 0; i < j; ++i)
            {
                CssRun w = tmpRuns[i];
                switch (w.Kind)
                {
                    case CssRunKind.Image:
                        {
                            CssBoxImage owner = (CssBoxImage)w.OwnerBox;
                            owner.PaintImage(g, w, args);

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
                            //if (textRun.Text.Contains("Cell"))
                            //{

                            //}
                            var wordPoint = new PointF(w.Left, w.Top);

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

        internal void dbugPaintRuns(IGraphics g, PaintingArgs args)
        {
            //return;
            //linebox  
            float x1 = this.OwnerBox.LocationX;
            float y1 = 0;
            float x2 = x1 + this.CachedLineContentWidth;
            float y2 = y1 + this.CacheLineHeight;
            //draw diagonal
            dbugDrawDiagonalBox(g, Pens.Blue, x1, y1, x2, y2);

            //g.DrawRectangle(Pens.Blue,
            //    this.OwnerBox.LocationX,
            //    this.CachedLineTop,
            //    this.CachedLineContentWidth,
            //    this.CacheLineHeight);
            //foreach (var strip in this._boxStrips.Values)
            //{
            //    var bound = strip.Bound;
            //    bound.Offset(offset);
            //    dbugDrawDiagnalBox(g, Pens.Green, bound.X, bound.Y, bound.Right, bound.Bottom);
            //}

            //return;
            foreach (CssRun w in this._runs)
            {
                g.DrawRectangle(Pens.DeepPink, w.Left, w.Top, w.Width, w.Height);
            }

            g.FillRectangle(Brushes.Red, this._ownerBox.LocationX, 0, 5, 5);

        }
        void dbugDrawDiagonalBox(IGraphics g, Pen pen, float x1, float y1, float x2, float y2)
        {
            g.DrawRectangle(pen, x1, y1, x2 - x1, y2 - y1);
            g.DrawLine(pen, x1, y1, x2, y2);
            g.DrawLine(pen, x1, y2, x2, y1);
        }

#endif

        internal void PaintBackgroundAndBorder(IGraphics g, PaintingArgs args)
        {
            //iterate each strip

            for (int i = _bottomUpBoxStrips.Count - 1; i >= 0; --i)
            {
                var strip = _bottomUpBoxStrips[i];
                var ownerBox = strip.owner;

                if (ownerBox.CssDisplay != CssDisplay.Inline)
                {
                    throw new NotSupportedException();
                    continue;
                }

                var stripArea = strip.Bound;

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
                    //this box is splited into multiple strip
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

        internal void PaintDecoration(IGraphics g, PaintingArgs args)
        {

            for (int i = _bottomUpBoxStrips.Count - 1; i >= 0; --i)
            {
                var strip = _bottomUpBoxStrips[i];
                CssBox ownerBox = strip.owner;
                if (ownerBox.CssDisplay != CssDisplay.Inline)
                {
                    continue;
                }

                var rect = strip.Bound;

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
                    //this box is splited into multiple strips
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

        static int StepUpRegisterStrips(Dictionary<CssBox, PartialBoxStrip> dicStrips,
            CssBox lineOwnerBox,
            List<PartialBoxStrip> inputList, int startInputAt)
        {

            int j = inputList.Count;
            for (int i = startInputAt; i < j; ++i)
            {
                //step up
                var strip = inputList[i];
                var upperBox = strip.owner.ParentBox;
                if (upperBox != null && upperBox != lineOwnerBox && upperBox.IsInline)
                {
                    RegisterStripPart(upperBox, strip.Left, strip.Top, strip.Right, strip.Bottom, inputList, dicStrips);
                }
            }
            return inputList.Count - j;

        }
        static void RegisterStripPart(CssBox runOwner,
            float left, float top, float right, float bottom,
            List<PartialBoxStrip> newStrips, Dictionary<CssBox, PartialBoxStrip> dic)
        {

            PartialBoxStrip strip;
            if (!dic.TryGetValue(runOwner, out strip))
            {
                strip = new PartialBoxStrip(runOwner, left, top, right - left, bottom - top);
                dic.Add(runOwner, strip);
                newStrips.Add(strip);
            }
            else
            {
                strip.MergeBound(left, top, right, bottom);
            }
        }


        //---------------------------------
        internal bool IsFirstLine
        {
            get { return this.linkedNode.Previous == null; }
        }
        internal bool IsLastLine
        {
            get { return this.linkedNode.Next == null; }
        }
        //---------------------------------
    }
}
