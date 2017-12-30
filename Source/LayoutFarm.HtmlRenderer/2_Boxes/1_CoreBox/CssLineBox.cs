//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
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
                sR = right;
            }
            if (bottom > sB)
            {
                sB = bottom;
            }

            this._width = sR - this._x;
            this._height = sB - this._y;
        }

#if DEBUG
        public override string ToString()
        {
            return this.owner.__aa_dbugId + " left:" + this.Left + ",width:" + this.Width + " " + this.owner.ToString();
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
        PartialBoxStrip[] _bottomUpBoxStrips;
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
            //relative top compare to its parent
            get;
            set;
        }

        internal float CachedLineContentWidth
        {
            get;
            set;
        }
        internal float CachedExactContentWidth
        {
            get;
            set;
        }
        internal float CalculateLineHeight()
        {
            float maxBottom = 0;
            List<CssRun> myruns = this._runs;
            int j = myruns.Count;
            for (int i = 0; i < j; ++i)
            {
                CssRun run = myruns[i];
                //maxRight = run.Right > maxRight ? run.Right : maxRight;
                maxBottom = run.Bottom > maxBottom ? run.Bottom : maxBottom;
            }
            return maxBottom;
        }

        internal void CloseLine(LayoutVisitor lay)
        {
#if DEBUG
            this.dbugIsClosed = true;
#endif

            //=============================================================
            //part 1: MakeStrips()
            //=============================================================
            //***
            List<CssRun> myruns = this._runs;
            CssBox lineOwner = this._ownerBox;
            List<PartialBoxStrip> tmpStrips = lay.GetReadyStripList();
            //--------------------------------------------------------------------------- 
            //first level 
            Dictionary<CssBox, PartialBoxStrip> unqiueStrips = lay.GetReadyStripDic();
            //location of run and strip related to its containng block
            float maxRight = 0;
            float maxBottom = 0;
            int j = myruns.Count;
            float firstRunStartAt = 0;
            for (int i = 0; i < j; ++i)
            {
                CssRun run = myruns[i];
                if (i == 0)
                {
                    firstRunStartAt = run.Left;
                }
                maxRight = run.Right > maxRight ? run.Right : maxRight;
                maxBottom = run.Bottom > maxBottom ? run.Bottom : maxBottom;
                if (run.IsSpaces)
                {
                    //strip size include whitespace ?
                    continue;
                }
                //-------------
                //first level data
                RegisterStripPart(run.OwnerBox, run.Left, run.Top, run.Right, run.Bottom, tmpStrips, unqiueStrips);
            }

            //---------------------------------------------------------------------------
            //other step to upper layer, until no new strip     
            int newStripIndex = 0;
            for (int numNewStripCreate = tmpStrips.Count; numNewStripCreate > 0; newStripIndex += numNewStripCreate)
            {
                numNewStripCreate = StepUpRegisterStrips(unqiueStrips, lineOwner, tmpStrips, newStripIndex);
            }

            this._bottomUpBoxStrips = tmpStrips.ToArray();
            lay.ReleaseStripList(tmpStrips);
            lay.ReleaseStripDic(unqiueStrips);
            //=============================================================
            //part 2: Calculate 
            //=============================================================     

            this.CacheLineHeight = maxBottom;
            this.CachedLineContentWidth = maxRight;
            this.CachedExactContentWidth = (maxRight - firstRunStartAt);
            if (lineOwner.VisualWidth < CachedLineContentWidth)
            {
                this.CachedLineContentWidth = this.OwnerBox.VisualWidth;
            }
        }
        internal void OffsetTop(float ydiff)
        {
            this.CachedLineTop += ydiff;
        }
        internal bool CanDoMoreLeftOffset(float newOffset, float rightLimit)
        {
            int j = this._runs.Count;
            if (j > 0)
            {
                //last run right position
                //1. find current left start  
                if (_runs[j - 1].Right + newOffset > rightLimit)
                {
                    //can offset
                    //then offset
                    return false;
                }
            }
            return true;
        }
        internal void DoLeftOffset(float diff)
        {
            for (int i = this._runs.Count - 1; i >= 0; --i)
            {
                this._runs[i].Left += diff;
            }
        }
        internal float GetRightOfLastRun()
        {
            int j = this.RunCount;
            if (j > 0)
            {
                return this._runs[j - 1].Right;
            }
            return 0;
        }
        public bool HitTest(float x, float y)
        {
            if (y >= this.CachedLineTop && y <= this.CachedLineBottom)
            {
                return true;
            }
            return false;
        }

        public float CalculateTotalBoxBaseLine(LayoutVisitor lay)
        {
            //not correct !! 
            float maxRunHeight = 0;
            CssRun maxRun = null;
            for (int i = this._runs.Count - 1; i >= 0; --i)
            {
                var run = this._runs[i];
                if (run.Height > maxRunHeight)
                {
                    maxRun = run;
                    maxRunHeight = run.Height;
                }
            }
            if (maxRun != null)
            {
                //lay.GraphicsPlatform.SampleIFonts
                //var f = maxRun.OwnerBox.ResolvedFont;
                //PixelFarm.Drawing.Fonts.ActualFont ff = f.ActualFont;
                //return (float)(f.Height * ff.AscentInPixels / f.Height);

                //return fontHeight* fontAscent / lineSpacing )
            }
            return 0;
            //int j = this._runs.Count;
            //for (int i = this._runs.Count - 1; i >= 0; --i)
            //{
            //    Font ownerFont = _runs[i].OwnerBox.ActualFont;
            //    HtmlRenderer.Drawing.FontsUtils.GetDescent(
            //}
            //return 0;

            //float baseline = Single.MinValue;
            //for (int i = _bottomUpBoxStrips.Count - 1; i >= 0; --i)
            //{
            //    baseline = Math.Max(baseline, _bottomUpBoxStrips[i].Top);//?top 
            //}
            //return baseline;
        }
        public void ApplyBaseline(float baseline)
        {
            //Important notes on http://www.w3.org/TR/CSS21/tables.html#height-layout
            //iterate from rectstrip
            //In a single LineBox ,  CssBox:RectStrip => 1:1 relation   


            for (int i = this._runs.Count - 1; i >= 0; --i)
            {
                var run = this._runs[i];
                //adjust base line
                run.SetLocation(run.Left, baseline);
            }

            //if (this._bottomUpBoxStrips == null)
            //{
            //    return;
            //}
            //for (int i = _bottomUpBoxStrips.Length - 1; i >= 0; --i)
            //{
            //    var rstrip = _bottomUpBoxStrips[i];
            //    var rstripOwnerBox = rstrip.owner;
            //    switch (rstripOwnerBox.VerticalAlign)
            //    {
            //        case Css.CssVerticalAlign.Sub:
            //            {
            //                this.SetBaseLine(rstripOwnerBox, baseline + rstrip.Height * .2f);
            //            } break;
            //        case Css.CssVerticalAlign.Super:
            //            {
            //                this.SetBaseLine(rstripOwnerBox, baseline - rstrip.Height * .2f);
            //            } break;
            //        case Css.CssVerticalAlign.TextTop:
            //        case Css.CssVerticalAlign.TextBottom:
            //        case Css.CssVerticalAlign.Top:
            //        case Css.CssVerticalAlign.Bottom:
            //        case Css.CssVerticalAlign.Middle:
            //            break;
            //        default:
            //            //case: baseline
            //            this.SetBaseLine(rstripOwnerBox, baseline);
            //            break;
            //    }
            //}
        }
        public IEnumerable<float> GetAreaStripTopPosIter()
        {
            for (int i = _bottomUpBoxStrips.Length - 1; i >= 0; --i)
            {
                yield return _bottomUpBoxStrips[i].Top;
            }
        }
        internal int RunCount
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
        internal void PaintRuns(PaintVisitor p)
        {
            //iterate from each words 
            CssBox latestOwner = null;
            DrawBoard innerCanvas = p.InnerCanvas;
            RequestFont enterFont = innerCanvas.CurrentFont;
            Color enterColor = innerCanvas.CurrentTextColor;
            List<CssRun> tmpRuns = this._runs;
            int j = tmpRuns.Count;
            for (int i = 0; i < j; ++i)
            {
                //-----------------
#if DEBUG
                dbugCounter.dbugRunPaintCount++;
#endif
                //-----------------

                CssRun w = tmpRuns[i];
                switch (w.Kind)
                {
                    case CssRunKind.SolidContent:
                        {
                            w.OwnerBox.Paint(p, new RectangleF(w.Left, w.Top, w.Width, w.Height));
                        }
                        break;
                    case CssRunKind.BlockRun:
                        {
                            //Console.WriteLine("blockrun"); 
                            CssBlockRun blockRun = (CssBlockRun)w;
                            int ox = p.CanvasOriginX;
                            int oy = p.CanvasOriginY;
                            p.SetCanvasOrigin(ox + (int)blockRun.Left, oy + (int)blockRun.Top);
                            blockRun.ContentBox.Paint(p);
                            p.SetCanvasOrigin(ox, oy);
                        }
                        break;
                    case CssRunKind.Text:
                        {
                            if (latestOwner != w.OwnerBox)
                            {
                                //change
                                latestOwner = w.OwnerBox;
                                //change font when change owner 

                                p.InnerCanvas.CurrentFont = latestOwner.ResolvedFont;
                                p.InnerCanvas.CurrentTextColor = latestOwner.ActualColor;
                            }

                            CssTextRun textRun = (CssTextRun)w;
                            PointF wordPoint = new PointF(w.Left, w.Top);
                            p.DrawText(CssBox.UnsafeGetTextBuffer(w.OwnerBox),
                               textRun.TextStartIndex,
                               textRun.TextLength, wordPoint,
                               new SizeF(w.Width, w.Height));
                        }
                        break;
                    default:
                        {
#if DEBUG
                            // w.OwnerBox.dbugPaintTextWordArea(g, offset, w);
#endif
                        }
                        break;
                }
            }

            //---
            //exit
            if (j > 0)
            {
                innerCanvas.CurrentFont = enterFont;
                innerCanvas.CurrentTextColor = enterColor;
            }
        }
#if DEBUG
        internal void dbugPaintRuns(PaintVisitor p)
        {
            if (!PaintVisitor.dbugDrawWireFrame)
            {
                return;
            }
            //linebox  
            float x1 = 0;
            float y1 = 0;
            float x2 = x1 + this.CachedLineContentWidth;
            float y2 = y1 + this.CacheLineHeight;
            //draw diagonal  
            p.dbugDrawDiagonalBox(Color.Blue, x1, y1, x2, y2);
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
                p.DrawRectangle(Color.DeepPink, w.Left, w.Top, w.Width, w.Height);
                //p.dbugDrawDiagonalBox(Color.DeepPink, w.Left, w.Top, w.Width, w.Height);
            }

            p.FillRectangle(Color.Red, 0, 0, 5, 5);
        }

#endif

        internal SelectionSegment SelectionSegment
        {
            get;
            set;
        }


        internal void PaintBackgroundAndBorder(PaintVisitor p)
        {
            //iterate each strip
            //if (_bottomUpBoxStrips == null)
            //{
            //    return;
            //}
            for (int i = _bottomUpBoxStrips.Length - 1; i >= 0; --i)
            {
                PartialBoxStrip strip = _bottomUpBoxStrips[i];
                CssBox stripOwner = strip.owner;
                if (!stripOwner.HasVisibleBgColor)
                {
                    continue;
                }
                //-----------------------------------------------------------------
                RectangleF stripArea = strip.Bound;
                bool isFirstLine, isLastLine;
                CssBox.GetSplitInfo(stripOwner, this, out isFirstLine, out isLastLine);
                stripOwner.PaintBackground(p, stripArea, isFirstLine, isLastLine);
                //if (stripOwner.CssDisplay != Css.CssDisplay.TableCell
                //    && stripOwner.HasSomeVisibleBorder)
                //{
                //    p.PaintBorders(stripOwner, stripArea, isFirstLine, isLastLine);
                //}

            }
        }

        internal void PaintDecoration(PaintVisitor p)
        {
            //if (_bottomUpBoxStrips == null)
            //{
            //    return;
            //}
            for (int i = _bottomUpBoxStrips.Length - 1; i >= 0; --i)
            {
                PartialBoxStrip strip = _bottomUpBoxStrips[i];
                CssBox ownerBox = strip.owner;
                bool isFirstLine, isLastLine;
                CssBox.GetSplitInfo(ownerBox, this, out isFirstLine, out isLastLine);
                ownerBox.PaintDecoration(p.InnerCanvas, strip.Bound, isFirstLine, isLastLine);
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
            foreach (CssRun word in this.GetRunIter(stripOwnerBox))
            {
                if (!word.IsSolidContent)
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
                PartialBoxStrip strip = inputList[i];
                CssBox upperBox = strip.owner.ParentBox;
                if (upperBox != null && upperBox != lineOwnerBox && upperBox.OutsideDisplayIsInline)
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
        internal CssRun FindMaxWidthRun(float minimum)
        {
            float max = minimum;
            CssRun maxRun = null;
            for (int i = this._runs.Count - 1; i >= 0; --i)
            {
                CssRun r = _runs[i];
                if (r.Width > max)
                {
                    max = r.Width;
                    maxRun = r;
                }
            }
            return maxRun;
        }
    }
}
