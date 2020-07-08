//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

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
            _x = x;
            _y = y;
            _width = w;
            _height = h;
        }
        //
        public float Left => _x;
        //
        public float Top => _y;
        //
        public float Width => _width;
        //
        public float Right => _x + _width;
        //
        public float Height => _height;
        //
        public float Bottom => _y + _height;
        //
        public RectangleF Bounds => new RectangleF(_x, _y, this.Width, this.Height);
        //

        public void MergeBound(float left, float top, float right, float bottom)
        {
            float sR = this.Right;
            float sB = this.Bottom;
            if (left < _x)
            {
                _x = left;
            }
            if (top < _y)
            {
                _y = top;
            }
            if (right > sR)
            {
                sR = right;
            }
            if (bottom > sB)
            {
                sB = bottom;
            }

            _width = sR - _x;
            _height = sB - _y;
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
    sealed partial class CssLineBox
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

        internal CssLineBox NextLine => linkedNode.Next?.Value;

        internal float CachedLineBottom => this.CachedLineTop + this.CacheLineHeight;
        internal float CacheLineHeight { get; private set; }
        /// <summary>
        /// line top relative to its parent 
        /// </summary>
        internal float CachedLineTop { get; set; }
        //
        internal float CachedLineContentWidth { get; set; }
        internal float CachedExactContentWidth { get; set; }
        //
        internal float CalculateLineHeight()
        {
            float maxBottom = 0;
            List<CssRun> myruns = _runs;
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
            List<CssRun> myruns = _runs;
            CssBox lineOwner = _ownerBox;
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

            _bottomUpBoxStrips = tmpStrips.ToArray();
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
            int j = _runs.Count;
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
            for (int i = _runs.Count - 1; i >= 0; --i)
            {
                _runs[i].Left += diff;
            }
        }
        internal float GetRightOfLastRun()
        {
            int j = this.RunCount;
            if (j > 0)
            {
                return _runs[j - 1].Right;
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
            for (int i = _runs.Count - 1; i >= 0; --i)
            {
                CssRun run = _runs[i];
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
            //int j = _runs.Count;
            //for (int i = _runs.Count - 1; i >= 0; --i)
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


            for (int i = _runs.Count - 1; i >= 0; --i)
            {
                CssRun run = _runs[i];
                //adjust base line
                run.SetLocation(run.Left, baseline);
            }

            //if (_bottomUpBoxStrips == null)
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
        //
        internal int RunCount => _runs.Count;
        //
        internal CssRun GetRun(int index) => _runs[index];
        //
        internal CssRun GetFirstRun() => _runs[0];
        //
        internal CssRun GetLastRun() => _runs[_runs.Count - 1];
        /// <summary>
        /// Gets the owner box
        /// </summary>
        public CssBox OwnerBox => _ownerBox;
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
            _runs.Add(run);//each word has only one owner linebox! 
            CssRun.SetHostLine(run, this);
        }
        internal IEnumerable<CssRun> GetRunIter(CssBox box)
        {
            List<CssRun> tmpRuns = _runs;
            int j = tmpRuns.Count;
            CssRun run;
            for (int i = 0; i < j; ++i)
            {
                if ((run = tmpRuns[i]).OwnerBox == box)
                {
                    yield return run;
                }
            }
        }
        internal SelectionSegment SelectionSegment { get; set; }
        internal IEnumerable<CssRun> GetRunIter()
        {
            List<CssRun> tmpRuns = _runs;
            int j = tmpRuns.Count;
            for (int i = 0; i < j; ++i)
            {
                yield return tmpRuns[i];
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
            if (!dic.TryGetValue(runOwner, out PartialBoxStrip strip))
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
        internal bool IsFirstLine => this.linkedNode.Previous == null;
        internal bool IsLastLine => this.linkedNode.Next == null;
        //--------------------------------- 
        internal CssRun FindMaxWidthRun(float minimum)
        {
            float max = minimum;
            CssRun maxRun = null;
            for (int i = _runs.Count - 1; i >= 0; --i)
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
