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
using LayoutFarm.Css;

namespace LayoutFarm.HtmlBoxes
{


    /// <summary>
    /// Represents a CSS Box of text or replaced elements.
    /// </summary>
    /// <remarks>
    /// The Box can contains other boxes, that's the way that the CSS Tree
    /// is composed.
    /// 
    /// To know more about boxes visit CSS spec:
    /// http://www.w3.org/TR/CSS21/box.html
    /// </remarks>
    public partial class CssBox
    {
        readonly Css.BoxSpec _myspec;
        object _controller;
        IRootGraphics rootgfx;
#if DEBUG
        public int dbugMark1;
        public readonly int __aa_dbugId = dbugTotalId++;
        static int dbugTotalId;
        public int dbugMark2;
#endif
        public CssBox(BoxSpec spec, IRootGraphics rootgfx)
        {
            this.rootgfx = rootgfx;
            this._aa_boxes = new CssBoxCollection();
#if DEBUG
            //if (this.__aa_dbugId == 6)
            //{
            //}
            if (!spec.IsFreezed)
            {
                //must be freezed
                throw new NotSupportedException();
            }
#endif

            //assign spec 
            this._myspec = spec;
            EvaluateSpec(spec);
            ChangeDisplayType(this, _myspec.CssDisplay);
        }
        public CssBox(BoxSpec spec, IRootGraphics rootgfx, CssDisplay displayType)
        {
            this.rootgfx = rootgfx;
            this._aa_boxes = new CssBoxCollection();
#if DEBUG
            //if (this.__aa_dbugId == 6)
            //{
            //}
            if (!spec.IsFreezed)
            {
                //must be freezed 
                throw new NotSupportedException();
            }
#endif

            //assign spec             
            this._boxCompactFlags |= BoxFlags.DONT_CHANGE_DISPLAY_TYPE;
            this._cssDisplay = displayType;
            this._myspec = spec;
            //---------------------------- 
            EvaluateSpec(spec);
            ChangeDisplayType(this, _myspec.CssDisplay);
        }
        public void SetController(object controller)
        {
            this._controller = controller;
        }
        public IRootGraphics RootGfx
        {
            get
            {
                return rootgfx;
            }
        }
        //public RootGraphic GetInternalRootGfx()
        //{
        //    return ((CssBoxRootGfxBridge)rootgfx).RootGfx;
        //}

        /// <summary>
        /// Gets the parent box of this box
        /// </summary>
        public CssBox ParentBox
        {
            get { return _parentBox; }
        }
        internal bool HasContainerProperty
        {
            get
            {
                switch (this._cssDisplay)
                {
                    case Css.CssDisplay.Block:
                    case Css.CssDisplay.Table:
                    case Css.CssDisplay.TableCell:
                    case Css.CssDisplay.ListItem:
                    case Css.CssDisplay.Flex:
                        return true;
                }
                return false;
            }
        }
        public CssBox GetTopRootCssBox()
        {
            var topmost = this;
            var cur_box = this;
            while (cur_box._parentBox != null)
            {
                topmost = cur_box._parentBox;
                cur_box = topmost;
            }
            return topmost;
        }


        internal virtual bool JustTempContainer
        {
            //temp fixed for FloatBox
            //TODO: review here again
            get { return false; }
        }
        /// <summary>
        /// Is the box is of "br" element.
        /// </summary>
        public bool IsBrElement
        {
            get
            {
                return (this._boxCompactFlags & BoxFlags.IS_BR_ELEM) != 0;
            }
        }
        public bool IsCustomCssBox
        {
            get
            {
                return (this._boxCompactFlags & BoxFlags.IS_CUSTOM_CSSBOX) != 0;
            }
        }

        /// <summary>
        /// is the box "Display" is "Inline", is this is an inline box and not block.
        /// </summary>
        internal bool OutsideDisplayIsInline
        {
            get
            {
                return (this._boxCompactFlags & BoxFlags.IS_INLINE_BOX) != 0;
            }
            set
            {
                if (value)
                {
                    this._boxCompactFlags |= BoxFlags.IS_INLINE_BOX;
                }
                else
                {
                    this._boxCompactFlags &= ~BoxFlags.IS_INLINE_BOX;
                }
            }
        }


        /// <summary>
        /// is the box "Display" is "Block", is this is an block box and not inline.
        /// </summary>
        public bool IsBlock
        {
            get
            {
                return this.CssDisplay == Css.CssDisplay.Block;
            }
        }

        internal bool HasContainingBlockProperty
        {
            get
            {
                //this flags is evaluated when call ChangeDisplay ****
                return (this._boxCompactFlags & BoxFlags.HAS_CONTAINER_PROP) != 0;
            }
        }

        /// <summary>
        /// Gets if this box represents an image
        /// </summary>
        public bool IsImage
        {
            get
            {
                return this.RunCount == 1 && this.FirstRun.IsSolidContent;
            }
        }

        /// <summary>
        /// Tells if the box is empty or contains just blank spaces
        /// </summary>
        bool IsSpaceOrEmpty
        {
            get
            {
                //TODO: review here

                if (this._aa_boxes.Count != 0)
                {
                    return false;
                }
                else if (this._aa_contentRuns != null)
                {
                    return this._aa_contentRuns.Count == 0;
                }
                return true;
            }
        }
        void ResetTextFlags()
        {
            int tmpFlags = this._boxCompactFlags;
            tmpFlags &= ~BoxFlags.HAS_EVAL_WHITESPACE;
            tmpFlags &= ~BoxFlags.TEXT_IS_ALL_WHITESPACE;
            tmpFlags &= ~BoxFlags.TEXT_IS_EMPTY;
            this._boxCompactFlags = tmpFlags;
        }

        internal static char[] UnsafeGetTextBuffer(CssBox box)
        {
            return box._buffer;
        }

        internal bool TextContentIsWhitespaceOrEmptyText
        {
            get
            {
                if (this._aa_contentRuns != null)
                {
                    return (this._boxCompactFlags & BoxFlags.TEXT_IS_ALL_WHITESPACE) != 0;
                }
                else
                {
                    return ChildCount == 0;
                }
            }
        }
#if DEBUG
        internal string dbugCopyTextContent()
        {
            if (this._aa_contentRuns != null)
            {
                return new string(this._buffer);
            }
            else
            {
                return null;
            }
        }
#endif
        internal void AddLineBox(CssLineBox linebox)
        {
            linebox.linkedNode = this._clientLineBoxes.AddLast(linebox);
        }
        internal int LineBoxCount
        {
            get
            {
                if (this._clientLineBoxes == null)
                {
                    return 0;
                }
                else
                {
                    return this._clientLineBoxes.Count;
                }
            }
        }

        internal static void GetSplitInfo(CssBox box, CssLineBox lineBox, out bool isFirstLine, out bool isLastLine)
        {
            CssLineBox firstHostLine, lastHostLine;
            var runList = box.Runs;
            if (runList == null)
            {
                firstHostLine = lastHostLine = null;
            }
            else
            {
                int j = runList.Count;
                firstHostLine = runList[0].HostLine;
                lastHostLine = runList[j - 1].HostLine;
            }
            if (firstHostLine == lastHostLine)
            {
                //is on the same line 
                if (lineBox == firstHostLine)
                {
                    isFirstLine = isLastLine = true;
                }
                else
                {
                    isFirstLine = isLastLine = false;
                }
            }
            else
            {
                if (firstHostLine == lineBox)
                {
                    isFirstLine = true;
                    isLastLine = false;
                }
                else
                {
                    isFirstLine = false;
                    isLastLine = true;
                }
            }
        }

        internal IEnumerable<CssLineBox> GetLineBoxIter()
        {
            if (this._clientLineBoxes != null)
            {
                var node = this._clientLineBoxes.First;
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Next;
                }
            }
        }
        internal IEnumerable<CssLineBox> GetLineBoxBackwardIter()
        {
            if (this._clientLineBoxes != null)
            {
                var node = this._clientLineBoxes.Last;
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Previous;
                }
            }
        }
        internal CssLineBox GetFirstLineBox()
        {
            return this._clientLineBoxes.First.Value;
        }
        internal CssLineBox GetLastLineBox()
        {
            return this._clientLineBoxes.Last.Value;
        }


        /// <summary>
        /// Gets the BoxWords of text in the box
        /// </summary>
        List<CssRun> Runs
        {
            get
            {
                return this._aa_contentRuns;
            }
        }
        /// <summary>
        /// box has only runs
        /// </summary>
        internal bool HasOnlyRuns
        {
            get
            {
                return this._aa_contentRuns != null;
            }
        }

        /// <summary>
        /// Gets the first word of the box
        /// </summary>
        internal CssRun FirstRun
        {
            get { return Runs[0]; }
        }
        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.
        /// </summary>
        /// <param name="g">Device context to use</param>
        public void PerformLayout(LayoutVisitor lay)
        {
            //derived class can perform its own layout algo            
            //by override performContentLayout 
            PerformContentLayout(lay);
        }
        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.<br/>
        /// </summary>
        /// <param name="g">Device context to use</param>
        protected virtual void PerformContentLayout(LayoutVisitor lay)
        {
            switch (this.CssDisplay)
            {
                case Css.CssDisplay.None:
                    {
                        return;
                    }
                default:
                    {
                        //others ... 
                        if (this.NeedComputedValueEvaluation) { this.ReEvaluateComputedValues(lay.SampleIFonts, lay.LatestContainingBlock); }
                        this.MeasureRunsSize(lay);
                    }
                    break;
                case Css.CssDisplay.Block:
                case Css.CssDisplay.ListItem:
                case Css.CssDisplay.Table:
                case Css.CssDisplay.InlineTable:
                case Css.CssDisplay.TableCell:
                case Css.CssDisplay.Flex:
                case Css.CssDisplay.InlineFlex:
                    {
                        //this box has its own  container property
                        //this box may be used for ...
                        // 1) line formatting context  , or
                        // 2) block formatting context  
                        if (this.NeedComputedValueEvaluation) { this.ReEvaluateComputedValues(lay.SampleIFonts, lay.LatestContainingBlock); }
                        this.MeasureRunsSize(lay);
                        //for general block layout 
                        CssLayoutEngine.PerformContentLayout(this, lay);
                    }
                    break;
            }

            //set height  
            UpdateIfHigher(this, ExpectedHeight);
            //update back 

            lay.UpdateRootSize(this);
        }

        static void UpdateIfHigher(CssBox box, float newHeight)
        {
            if (newHeight > box.VisualHeight)
            {
                box.SetVisualHeight(newHeight);
            }
        }
        internal void SetHeightToZero()
        {
            this.SetVisualHeight(0);
        }
        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        /// <param name="g"></param>
        public virtual void MeasureRunsSize(LayoutVisitor lay)
        {
            //measure once !
            if ((this._boxCompactFlags & BoxFlags.LAY_RUNSIZE_MEASURE) != 0)
            {
                return;
            }
            //-------------------------------- 
            if (this.BackgroundImageBinder != null)
            {
                //this has background
                if (this.BackgroundImageBinder.State == ImageBinderState.Unload)
                {
                    lay.RequestImage(this.BackgroundImageBinder, this);
                }
            }
            //-------------------------------- 
            if (this.RunCount > 0)
            {
                //find word spacing  
                float actualWordspacing = this._actualWordSpacing;
                RequestFont actualFont = this.ResolvedFont;


                float fontHeight = (actualFont.AscentInPixels - actualFont.DescentInPixels + actualFont.LineGapInPixels);
                fontHeight += 4;



                var tmpRuns = this.Runs;
                for (int i = tmpRuns.Count - 1; i >= 0; --i)
                {
                    CssRun run = tmpRuns[i];
                    run.Height = fontHeight;
                    //if this is newline then width =0 ***                         
                    switch (run.Kind)
                    {
                        case CssRunKind.Text:
                            {
                                CssTextRun textRun = (CssTextRun)run;
                                //run.Width = lay.MeasureStringWidth(
                                //    CssBox.UnsafeGetTextBuffer(this),
                                //    textRun.TextStartIndex,
                                //    textRun.TextLength,
                                //    actualFont);
                                Size ss = lay.MeasureStringSize(CssBox.UnsafeGetTextBuffer(this),
                                    textRun.TextStartIndex,
                                    textRun.TextLength,
                                    actualFont);
                                run.SetSize(ss.Width, ss.Height);

                            }
                            break;
                        case CssRunKind.SingleSpace:
                            {
                                run.Width = actualWordspacing;
                            }
                            break;
                        case CssRunKind.Space:
                            {
                                //other space size                                     
                                run.Width = actualWordspacing * ((CssTextRun)run).TextLength;
                            }
                            break;
                        case CssRunKind.LineBreak:
                            {
                                run.Width = 0;
                            }
                            break;
                    }
                }
            }
            this._boxCompactFlags |= BoxFlags.LAY_RUNSIZE_MEASURE;
        }


        internal float LatestCacheMinimumWidth
        {
            get { return this._cachedMinimumWidth; }
        }
        /// <summary>
        /// Gets the minimum width that the box can be.
        /// *** The box can be as thin as the longest word plus padding
        /// </summary>
        /// <returns></returns>
        internal float CalculateMinimumWidth(int calculationEpisode)
        {
            float maxWidth = 0;
            float padding = 0f;
            if (this._lastCalculationEpisodeNum == calculationEpisode)
            {
                return _cachedMinimumWidth;
            }
            //---------------------------------------------------
            if (this.LineBoxCount > 0)
            {
                //use line box technique *** 
                CssRun maxWidthRun = null;
                CalculateMinimumWidthAndWidestRun(this, out maxWidth, out maxWidthRun);
                //--------------------------------  
                if (maxWidthRun != null)
                {
                    //bubble up***
                    var box = maxWidthRun.OwnerBox;
                    while (box != null)
                    {
                        padding += (box.ActualBorderRightWidth + box.ActualPaddingRight) +
                            (box.ActualBorderLeftWidth + box.ActualPaddingLeft);
                        if (box == this)
                        {
                            break;
                        }
                        else
                        {
                            //bubble up***
                            box = box.ParentBox;
                        }
                    }
                }
            }
            this._lastCalculationEpisodeNum = calculationEpisode;
            return _cachedMinimumWidth = maxWidth + padding;
        }


        static void CalculateMinimumWidthAndWidestRun(CssBox box, out float maxWidth, out CssRun maxWidthRun)
        {
            //use line-base style ***

            float maxRunWidth = 0;
            CssRun foundRun = null;
            if (box._clientLineBoxes != null)
            {
                var lineNode = box._clientLineBoxes.First;
                while (lineNode != null)
                {
                    //------------------------
                    var line = lineNode.Value;
                    var tmpRun = line.FindMaxWidthRun(maxRunWidth);
                    if (tmpRun != null)
                    {
                        maxRunWidth = tmpRun.Width;
                        foundRun = tmpRun;
                    }
                    //------------------------
                    lineNode = lineNode.Next;
                }
            }

            maxWidth = maxRunWidth;
            maxWidthRun = foundRun;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="calculationEpisode"></param>
        /// <returns></returns>
        internal static float GetLatestCachedMinWidth(CssBox targetBox)
        {
            //---------------------------------------------------
            if (targetBox.LineBoxCount > 0)
            {
                float maxWidth = 0;
                foreach (var line in targetBox._clientLineBoxes)
                {
                    var lineContentW = line.CachedLineContentWidth;
                    if (maxWidth < lineContentW)
                    {
                        maxWidth = lineContentW;
                    }
                }
                return maxWidth;
            }
            else
            {
                float minWidth = 0;
                if (targetBox._cssDisplay == CssDisplay.Table)
                {
                    //special for table element
                    if (targetBox._aa_boxes != null)
                    {
                        foreach (var tr in targetBox._aa_boxes)
                        {
                            //td
                            float trW = 0;
                            int j = tr._aa_boxes.Count;
                            if (j > 0)
                            {
                                var lastTd = tr._aa_boxes.GetLastChild();
                                //TODO: review here again-> how to get rightmost position
                                trW = (lastTd.LocalX + lastTd.VisualWidth + lastTd._actualPaddingRight);
                            }
                            if (trW > minWidth)
                            {
                                minWidth = trW;
                            }
                        }
                    }
                }
                else
                {
                    if (targetBox._aa_boxes != null)
                    {
                        foreach (var box in targetBox._aa_boxes)
                        {
                            float w = GetLatestCachedMinWidth(box);
                            if (w > minWidth)
                            {
                                minWidth = w;
                            }
                        }
                    }
                }
                return minWidth;
            }
        }
        bool IsLastChild
        {
            get
            {
                return this.ParentBox._aa_boxes.GetLastChild() == this;
            }
        }
        /// <summary>
        /// Gets the result of collapsing the vertical margins of the two boxes
        /// </summary>
        /// <param name="upperSibling">the previous box under the same parent</param>
        /// <returns>Resulting top margin</returns>
        public float UpdateMarginTopCollapse(CssBox upperSibling)
        {
            float value;
            if (upperSibling != null)
            {
                value = Math.Max(upperSibling.ActualMarginBottom, this.ActualMarginTop);
                this.CollapsedMarginTop = value;
            }
            else if (_parentBox != null &&
                ActualPaddingTop < 0.1 &&
                ActualPaddingBottom < 0.1 &&
                _parentBox.ActualPaddingTop < 0.1 &&
                _parentBox.ActualPaddingBottom < 0.1)
            {
                value = Math.Max(0, ActualMarginTop - Math.Max(_parentBox.ActualMarginTop, _parentBox.CollapsedMarginTop));
            }
            else
            {
                value = ActualMarginTop;
            }
            return value;
        }
        /// <summary>
        /// Gets the result of collapsing the vertical margins of the two boxes
        /// </summary>
        /// <returns>Resulting bottom margin</returns>
        internal float GetHeightAfterMarginBottomCollapse(CssBox containerBox)
        {
            //TODO: review again 
            float margin = 0;
            if (ParentBox != null && this.IsLastChild && containerBox.ActualMarginBottom < 0.1)
            {
                var lastChildBottomMargin = _aa_boxes.GetLastChild().ActualMarginBottom;
                //margin = (Height.IsAuto) ?
                //    Math.Max(ActualMarginBottom, lastChildBottomMargin)
                //    : lastChildBottomMargin;
                margin = lastChildBottomMargin;
            }
            //exclude float box
            var cnode = _aa_boxes.GetLastLinkedNode();
            float lastChildBotom = 0;
            while (cnode != null)
            {
                CssBox box = cnode.Value;
                if (box.Float == CssFloat.None)
                {
                    lastChildBotom = box.LocalVisualBottom;
                    //found static child

                    return lastChildBotom + margin + this.ActualPaddingBottom + ActualBorderBottomWidth;
                }
                else
                {
                    cnode = cnode.Previous;
                }
            }
            //here not found any static child 

            cnode = _aa_boxes.GetLastLinkedNode();
            if (this.Height.IsAuto && cnode != null)
            {
                CssBox box = cnode.Value;
                lastChildBotom = box.LocalVisualBottom;
                //found static child 
                return lastChildBotom + margin + this.ActualPaddingBottom + ActualBorderBottomWidth;
            }
            return this.ActualPaddingTop + this.ActualBorderTopWidth + this.ActualPaddingBottom + ActualBorderBottomWidth;
        }
        internal void OffsetLocalTop(float dy)
        {
            this._localY += dy;
        }
        internal bool CanBeReferenceSibling
        {
            //TODO: review here, fixed position can be reference sibling?

            get { return this.CssDisplay != Css.CssDisplay.None && this.Position != Css.CssPosition.Absolute; }
        }

#if DEBUG
        ///// <summary>
        ///// ToString override.
        ///// </summary>
        ///// <returns></returns>
        //public override string ToString()
        //{
        //    var tag = HtmlElement != null ? string.Format("<{0}>", HtmlElement.Name) : "anon";

        //    if (IsBlock)
        //    {
        //        return string.Format("{0}{1} Block {2}, Children:{3}", ParentBox == null ? "Root: " : string.Empty, tag, FontSize, Boxes.Count);
        //    }
        //    else if (this.CssDisplay == CssDisplay.None)
        //    {
        //        return string.Format("{0}{1} None", ParentBox == null ? "Root: " : string.Empty, tag);
        //    }
        //    else
        //    {
        //        if (this.MayHasSomeTextContent)
        //        {
        //            return string.Format("{0}{1} {2}: {3}", ParentBox == null ? "Root: " : string.Empty, tag,
        //                this.CssDisplay.ToCssStringValue(), this.dbugCopyTextContent());
        //        }
        //        else
        //        {
        //            return string.Format("{0}{1} {2}: {3}", ParentBox == null ? "Root: " : string.Empty, tag,
        //                this.CssDisplay.ToCssStringValue(), "");
        //        }
        //    }
        //}
#endif


        //-----------------------------------------------------------------
        public static CssBox AddNewAnonInline(CssBox parent)
        {
            var spec = CssBox.UnsafeGetBoxSpec(parent);
            var newBox = new CssBox(spec.GetAnonVersion(), parent.rootgfx);
            parent.AppendChild(newBox);
            CssBox.ChangeDisplayType(newBox, Css.CssDisplay.Inline);
            return newBox;
        }
    }
}