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
    class CssRectangleF
    {
        internal CssBox owner;
        internal RectangleF rectF;
        public CssRectangleF(CssBox owner, RectangleF rectF)
        {
            this.owner = owner;
            this.rectF = rectF;
        }
    }
    //--------------------------
    //struct CssBoxAndRectKeyValuePair
    //{
    //    public readonly CssBox box;
    //    public readonly RectangleF rectF;
    //    public CssBoxAndRectKeyValuePair(CssBox box, RectangleF rectF)
    //    {
    //        this.box = box;
    //        this.rectF = rectF;
    //    }
    //}
    /// <summary>
    /// Represents a line of text.
    /// </summary>
    /// <remarks>
    /// To learn more about line-boxes see CSS spec:
    /// http://www.w3.org/TR/CSS21/visuren.html
    /// </remarks>
    internal sealed class CssLineBox
    {
        #region Fields and Consts

        private readonly CssBox _ownerBox;
        private readonly List<CssRect> _words;
        //linebox and CssRectangleF is 1:1 relation 
        readonly Dictionary<CssBox, CssRectangleF> _rects; //rectStrips
        internal LinkedListNode<CssLineBox> linkedNode;


        bool isClosed;
        // private readonly List<CssBox> _relatedBoxes;

        #endregion


        /// <summary>
        /// Creates a new LineBox
        /// </summary>
        public CssLineBox(CssBox ownerBox)
        {
            this._rects = new Dictionary<CssBox, CssRectangleF>();
            //  _relatedBoxes = new List<CssBox>();
            _words = new List<CssRect>();
            _ownerBox = ownerBox;
            //_ownerBox.LineBoxes.Add(this); 
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
            this.isClosed = true;
            float height = 0;
            float bottom = 0;

            foreach (var rect in _rects.Values)
            {
                height = Math.Max(height, rect.rectF.Height);
                bottom = Math.Max(bottom, rect.rectF.Bottom);
            }
            this.CacheLineHeight = height;
            this.CachedLineBottom = bottom;
            this.CachedLineTop = bottom - height;
        }

        /// <summary>
        /// Get the bottom of this box line (the max bottom of all the words)
        /// </summary>
        public float LineBottom
        {
            get
            {
                float bottom = 0;
                foreach (var rect in _rects)
                {
                    bottom = Math.Max(bottom, rect.Value.rectF.Bottom);
                }
                return bottom;
            }
        }
        internal IEnumerable<CssBox> GetBoxIter()
        {
            foreach (var kp in this._rects)
            {
                yield return kp.Key;
            }
        }
        internal float CalculateTotalBoxBaseLine()
        {
            float baseline = Single.MinValue;
            //foreach (var r in this._words)  //iter from word in this line
            //{
            //    baseline = Math.Max(baseline, r.Top);//?top 
            //}
            foreach (var kp in this._rects)  //iter from word in this line
            {
                baseline = Math.Max(baseline, kp.Value.rectF.Top);//?top 
            }
            return baseline;
        }
        internal void ApplyBaseline2(float baseline)
        {
            //Important notes on http://www.w3.org/TR/CSS21/tables.html#height-layout
            //iterate from rectstrip
            //In a single LineBox ,  CssBox:RectStrip => 1:1 relation 

            foreach (CssRectangleF rstrip in this._rects.Values)
            {
                CssBox rstripOwnerBox = rstrip.owner;

                switch (rstripOwnerBox.VerticalAlign)
                {
                    //case CssConstants.Sub:
                    case CssVerticalAlign.Sub:
                        {
                            var rectF = rstrip.rectF;
                            this.SetBaseLine2(rstrip, baseline + rectF.Height * .2f);
                        } break;
                    //case CssConstants.Super:
                    case CssVerticalAlign.Super:
                        {
                            var rectF = rstrip.rectF;
                            this.SetBaseLine2(rstrip, baseline - rectF.Height * .2f);
                        } break;
                    case CssVerticalAlign.TextTop:
                    case CssVerticalAlign.TextBottom:
                    case CssVerticalAlign.Top:
                    case CssVerticalAlign.Bottom:
                    case CssVerticalAlign.Middle:
                        break;
                    default:
                        //case: baseline
                        this.SetBaseLine2(rstrip, baseline);
                        break;
                }
            }
        }
        //internal void ApplyBaseline(IGraphics g, float baseline)
        //{

        //    foreach (var rectStrip in this._rects.Values)
        //    {
        //        CssBox box = rectStrip.owner;

        //        //Important notes on http://www.w3.org/TR/CSS21/tables.html#height-layout
        //        switch (box.VerticalAlign)
        //        {
        //            //case CssConstants.Sub:
        //            case CssVerticalAlign.Sub:
        //                {
        //                    var rectF = rectStrip.rectF;
        //                    this.SetBaseLine(g, box, baseline + rectF.Height * .2f);
        //                } break;
        //            //case CssConstants.Super:
        //            case CssVerticalAlign.Super:
        //                {
        //                    var rectF = rectStrip.rectF;
        //                    this.SetBaseLine(g, box, baseline - rectF.Height * .2f);
        //                } break;
        //            case CssVerticalAlign.TextTop:
        //            case CssVerticalAlign.TextBottom:
        //            case CssVerticalAlign.Top:
        //            case CssVerticalAlign.Bottom:
        //            case CssVerticalAlign.Middle:
        //                break;
        //            default:
        //                //case: baseline
        //                this.SetBaseLine(g, box, baseline);
        //                break;
        //        }
        //    }
        //}

        ///// <summary>
        ///// in a single line, relation box cssbox:recstrip relation is 1:1
        ///// </summary>
        ///// <returns></returns>
        //internal IEnumerable<CssRectangleF> GetRectStripIter()
        //{
        //    foreach (var r in this._rects.Values)
        //    {
        //        yield return r;
        //    }
        //}
        internal IEnumerable<CssRect> GetRectWordIter()
        {
            foreach (var w in this._words)
            {
                yield return w;
            }
        }
        internal IEnumerable<RectangleF> GetAreaStripRectIter()
        {
            foreach (var r in this._rects)
            {
                yield return r.Value.rectF;
            }
        }

        /////// <summary>
        /////// Gets a list of boxes related with the linebox. 
        /////// To know the words of the box inside this linebox, use the <see cref="WordsOf"/> method.
        /////// </summary>
        //public List<CssBox> RelatedBoxes
        //{
        //    get { return _relatedBoxes; }
        //}

        /// <summary>
        /// Gets the words inside the linebox
        /// </summary>
        public List<CssRect> Words
        {
            get { return _words; }
        }

        /// <summary>
        /// offset 
        /// </summary>
        /// <param name="targetCssBox"></param>
        /// <param name="topOffset"></param>
        internal void OffsetTopRectsOf(CssBox targetCssBox, float topOffset)
        {
            CssRectangleF r;
            if (this._rects.TryGetValue(targetCssBox, out r))
            {
                var rr = r.rectF;
                rr.Offset(0, topOffset);
                r.rectF = rr;
            }
        }

        /// <summary>
        /// Gets the owner box
        /// </summary>
        public CssBox OwnerBox
        {
            get { return _ownerBox; }
        }
        public void RemoveAllReferencedContent()
        {
            //for (int i = _relatedBoxes.Count - 1; i > -1; --i)
            //{
            //    _relatedBoxes[i].dbugOwnerLineBox = null;
            //} 
            //for (int i = _words.Count - 1; i > -1; --i)
            //{
            //    _words[i].ownerLineBox = null;
            //}
        }
        /// <summary>
        /// Gets a List of rectangles that are to be painted on this linebox
        /// </summary>
        Dictionary<CssBox, CssRectangleF> Rectangles
        {
            get { return _rects; }
        }

        /// <summary>
        /// Get the height of this box line (the max height of all the words)
        /// </summary>
        public float LineHeight
        {
            get
            {
                float height = 0;
                foreach (var rect in _rects)
                {
                    height = Math.Max(height, rect.Value.rectF.Height);
                }
                return height;
            }
        }



        /// <summary>
        /// Lets the linebox add the word an its box to their lists if necessary.
        /// </summary>
        /// <param name="word"></param>
        internal void ReportExistanceOf(CssRect word)
        {
#if DEBUG
            //not need to check !
            //if (this.Words.Contains(word))
            //{
            //}
#endif
            word.ownerLineBox = this;
            Words.Add(word);
            //if (!RelatedBoxes.Contains(word.OwnerBox))
            //{
            //    RelatedBoxes.Add(word.OwnerBox);
            //}
        }

        ///// <summary>
        ///// Return the words of the specified box that live in this linebox
        ///// </summary>
        ///// <param name="box"></param>
        ///// <returns></returns>
        //internal List<CssRect> WordsOf(CssBox box)
        //{
        //    List<CssRect> r = new List<CssRect>();

        //    foreach (CssRect word in Words)
        //        if (word.OwnerBox.Equals(box)) r.Add(word);

        //    return r;
        //}
        internal IEnumerable<CssRect> GetWordIterOf(CssBox box)
        {
            foreach (CssRect word in this.Words)
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

            //RectangleF existingRect;
            //if (!Rectangles.TryGetValue(box, out existingRect))
            //{
            //    Rectangles.Add(box, RectangleF.FromLTRB(x, y, r, b));
            //}
            //else
            //{
            //    //update
            //    Rectangles[box] = RectangleF.FromLTRB(
            //        Math.Min(existingRect.X, x), Math.Min(existingRect.Y, y),
            //        Math.Max(existingRect.Right, r), Math.Max(existingRect.Bottom, b));
            //}

            CssRectangleF existingRect;
            if (!Rectangles.TryGetValue(box, out existingRect))
            {
                Rectangles.Add(box, new CssRectangleF(box, RectangleF.FromLTRB(x, y, r, b)));
            }
            else
            {
                //update                
                RectangleF existingR = existingRect.rectF;
                existingRect.rectF = RectangleF.FromLTRB(
                    Math.Min(existingR.X, x), Math.Min(existingR.Y, y),
                    Math.Max(existingR.Right, r), Math.Max(existingR.Bottom, b));
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

        /// <summary>
        /// Copies the rectangles to their specified box
        /// </summary>
        internal void AssignRectanglesToBoxes()
        {
            foreach (var cssRectF in this.Rectangles.Values)
            {
                cssRectF.owner.UpdateStripInfo(cssRectF.rectF); 
                //kp.Key.AddRectStripInfo(kp.Value.rectF);
            }
        }
        internal CssRectangleF GetStrip(CssBox box)
        {
            CssRectangleF found;
            this._rects.TryGetValue(box, out found);
            return found;
        }
        internal void AddRectStripInfo(CssBox owner, RectangleF r)
        {

            this._rects.Add(owner, new CssRectangleF(owner, r));

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
            foreach (CssRect w in this._words)
            {
                switch (w.RectKind)
                {
                    case CssRectKind.Image:
                        {
                            CssBoxImage owner = (CssBoxImage)w.OwnerBox;
                            owner.PaintImage(g, offset, w);
                        } break;
                    case CssRectKind.Space:
                    case CssRectKind.LineBreak:
                    case CssRectKind.Unknown:
                        break;
                    case CssRectKind.Text:
                        {
                            w.OwnerBox.PaintTextWord(g, offset, w);
                        } break;
                    default:
                        break;
                }
            }
        }
        
#if DEBUG
        internal void dbugPaintWords(IGraphics g, PointF offset)
        {
            foreach (CssRect w in this._words)
            {
                var wordPoint = new PointF(w.Left + offset.X, w.Top + offset.Y);
                g.DrawRectangle(Pens.DeepPink, w.Left, w.Top, w.Width, w.Height);
            }
        }
        internal void dbugPaintRects(IGraphics g, PointF offset)
        {
            foreach (var kp in this._rects)
            {
                var ownerBox = kp.Key;
                var rect = kp.Value.rectF;

                rect.Offset(offset);
                //if (ownerBox.CssDisplay != CssBoxDisplayType.Inline)
                //{
                //    continue;
                //}
                //-------------------
                //debug line
                g.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
                //------------------- 
            }
        }
        internal void dbugPaintRects2(IGraphics g, PointF offset)
        {
            foreach (var kp in this._rects)
            {
                var ownerBox = kp.Key;
                var rect = kp.Value.rectF;

                rect.Offset(offset);
                //if (ownerBox.CssDisplay != CssBoxDisplayType.Inline)
                //{
                //    continue;
                //}
                //-------------------
                //debug line
                g.DrawRectangle(Pens.Red, rect.X, rect.Y, rect.Width, rect.Height);
                //------------------- 
            }
        }
#endif
        /// <summary>
        /// paint rect content in this line
        /// </summary>
        /// <param name="g"></param>
        void PaintBackgroundAndBorder(IGraphics g, PointF offset)
        {
            //iterate from each rect

            foreach (var kp in this._rects)
            {
                var ownerBox = kp.Key;
                if (ownerBox.CssDisplay != CssBoxDisplayType.Inline)
                {
                    continue;
                }

                var rect = kp.Value.rectF;
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
            foreach (var kp in this._rects)
            {
                var ownerBox = kp.Key;
                if (ownerBox.CssDisplay != CssBoxDisplayType.Inline)
                {
                    continue;
                }

                var rect = kp.Value.rectF;
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
        /// Draws the rectangles for debug purposes
        /// </summary>
        /// <param name="g"></param>
        internal void dbugDrawRectangles(Graphics g)
        {
            using (SolidBrush sb = new SolidBrush(Color.FromArgb(50, Color.Black)))
            {
                foreach (var kp in this.Rectangles)
                {
                    RectangleF r = kp.Value.rectF;
                    if (float.IsInfinity(r.Width))
                    {
                        continue;
                    }
                    g.FillRectangle(sb, Rectangle.Round(r));
                    g.DrawRectangle(Pens.Red, Rectangle.Round(r));
                }
            }

            //foreach (CssBox b in Rectangles.Keys)
            //{
            //    if (float.IsInfinity(Rectangles[b].Width))
            //    {
            //        continue;
            //    }
            //    g.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Black)),
            //        Rectangle.Round(Rectangles[b]));
            //    g.DrawRectangle(Pens.Red, Rectangle.Round(Rectangles[b]));
            //}
        }

        /// <summary>
        /// Gets the baseline Height of the rectangle
        /// </summary>
        /// <param name="b"> </param>
        /// <param name="g"></param>
        /// <returns></returns>
        public float GetBaseLineHeight(CssBox b, Graphics g)
        {
            Font f = b.ActualFont;
            FontFamily ff = f.FontFamily;
            FontStyle s = f.Style;
            return f.GetHeight(g) * ff.GetCellAscent(s) / ff.GetLineSpacing(s);
        }

        /// <summary>
        /// Gets the maximum bottom of the words
        /// </summary>
        /// <returns></returns>
        public float GetMaxWordBottom()
        {
            float res = float.MinValue;
            foreach (CssRect word in Words)
            {
                res = Math.Max(res, word.Bottom);
            }
            return res;
        }
        static List<CssRect> MakeList(IEnumerable<CssRect> ienumRect)
        {
            List<CssRect> newlist = new List<CssRect>();
            foreach (CssRect r in ienumRect)
            {
                newlist.Add(r);
            }
            return newlist;
        }
        void SetBaseLine2(CssRectangleF rstrip, float baseline)
        {

            //----------------------------------------
            CssBox rstripOwner = rstrip.owner;
            //TODO: Aqui me quede, checar poniendo "by the" con un font-size de 3em  
            RectangleF rstripRect = rstrip.rectF;
            //----------------------------------------

            //Save top of words related to the top of rectangle
            float gap = 0f;

            //make word list of 'b' only found in this line.
            List<CssRect> ws = MakeList(this.GetWordIterOf(rstripOwner));//ws =WordsOf(b);             

            if (ws.Count > 0)
            {
                //get first word only
                gap = ws[0].Top - rstripRect.Top;
                if (gap != 0)
                {
                }
            }
            else
            {
                //should not happen ?!
                CssRect firstw = rstripOwner.FirstWordOccourence(rstripOwner, this);
                if (firstw != null)
                {
                    gap = firstw.Top - rstripRect.Top;
                }
            }

            //New top that words will have
            //float newtop = baseline - (Height - OwnerBox.FontDescent - 3); //OLD
            float newtop = baseline;// -GetBaseLineHeight(b, g); //OLD
            var bParent = rstripOwner.ParentBox; //***
            if (bParent != null)
            {
                throw new NotSupportedException();

                ////bParent's Rects contains info about
                ////rect strip of B expand across multiple line
                //RectangleF rectOfThisLineOnParent;
                //if (bParent.Rectangles.TryGetValue(this, out rectOfThisLineOnParent)
                //    && rstripRect.Height < rectOfThisLineOnParent.Height)
                //{
                //    float recttop = newtop - gap;
                //    //new version  
                //    RectangleF newr = new RectangleF(rstripRect.X, recttop, rstripRect.Width, rstripRect.Height);
                //    //replace old rect with new one
                //    rstrip.rectF = newr; //***
                //    //Rectangles[b] = newr; //update back ?

                //    //update back
                //    rstripOwner.OffsetRectangle(this, gap);
                //}
            }
            //if (b.ParentBox != null &&
            //    b.ParentBox.Rectangles.ContainsKey(this) &&
            //    r.Height < b.ParentBox.Rectangles[this].Height)
            //{
            //    //Do this only if rectangle is shorter than parent's
            //    float recttop = newtop - gap;
            //    RectangleF newr = new RectangleF(r.X, recttop, r.Width, r.Height);
            //    //replace old rect with new one
            //    Rectangles[b] = newr;
            //    b.OffsetRectangle(this, gap);
            //}

            foreach (var word in ws)
            {
                if (!word.IsImage)
                {
                    word.Top = newtop;
                }
            }
        }

        ///// <summary>
        ///// Sets the baseline of the words of the specified box to certain height
        ///// </summary>
        ///// <param name="g">Device info</param>
        ///// <param name="b">box to check words</param>
        ///// <param name="baseline">baseline</param>
        //void SetBaseLine(IGraphics g, CssBox b, float baseline)
        //{
        //    //TODO: Aqui me quede, checar poniendo "by the" con un font-size de 3em 

        //    CssRectangleF rectStrip;
        //    if (!Rectangles.TryGetValue(b, out rectStrip))
        //    {
        //        return;
        //    }

        //    RectangleF r = rectStrip.rectF;
        //    //Save top of words related to the top of rectangle
        //    float gap = 0f;

        //    //make word list of 'b' only found in this line.
        //    List<CssRect> ws = MakeList(this.GetWordIterOf(b));//ws =WordsOf(b);             

        //    if (ws.Count > 0)
        //    {
        //        //get first word only
        //        gap = ws[0].Top - r.Top;
        //    }
        //    else
        //    {
        //        //should not happen !
        //        CssRect firstw = b.FirstWordOccourence(b, this);
        //        if (firstw != null)
        //        {
        //            gap = firstw.Top - r.Top;
        //        }
        //    }

        //    //New top that words will have
        //    //float newtop = baseline - (Height - OwnerBox.FontDescent - 3); //OLD
        //    float newtop = baseline;// -GetBaseLineHeight(b, g); //OLD

        //    var bParent = b.ParentBox; //***
        //    if (bParent != null)
        //    {
        //        RectangleF rectOfThisLineOnParent;
        //        if (bParent.Rectangles.TryGetValue(this, out rectOfThisLineOnParent)
        //            && r.Height < rectOfThisLineOnParent.Height)
        //        {
        //            float recttop = newtop - gap;
        //            //new version 

        //            RectangleF newr = new RectangleF(r.X, recttop, r.Width, r.Height);
        //            //replace old rect with new one
        //            rectStrip.rectF = newr; //***
        //            //Rectangles[b] = newr; //update back ?

        //            //update back
        //            b.OffsetRectangle(this, gap);
        //        }
        //    }
        //    //if (b.ParentBox != null &&
        //    //    b.ParentBox.Rectangles.ContainsKey(this) &&
        //    //    r.Height < b.ParentBox.Rectangles[this].Height)
        //    //{
        //    //    //Do this only if rectangle is shorter than parent's
        //    //    float recttop = newtop - gap;
        //    //    RectangleF newr = new RectangleF(r.X, recttop, r.Width, r.Height);
        //    //    //replace old rect with new one
        //    //    Rectangles[b] = newr;
        //    //    b.OffsetRectangle(this, gap);
        //    //}

        //    foreach (var word in ws)
        //    {
        //        if (!word.IsImage)
        //        {
        //            word.Top = newtop;
        //        }
        //    }
        //}

        /// <summary>
        /// Check if the given word is the last selected word in the line.<br/>
        /// It can either be the last word in the line or the next word has no selection.
        /// </summary>
        /// <param name="word">the word to check</param>
        /// <returns></returns>
        public bool IsLastSelectedWord(CssRect word)
        {
            for (int i = 0; i < _words.Count - 1; i++)
            {
                if (_words[i] == word)
                {
                    return !_words[i + 1].Selected;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the words of the linebox
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string[] ws = new string[Words.Count];
            for (int i = 0; i < ws.Length; i++)
            {
                ws[i] = Words[i].Text;
            }
            return string.Join(" ", ws);
        }
    }
}
