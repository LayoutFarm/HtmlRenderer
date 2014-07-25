//BSD 2014, WinterDev 
//ArthurHub

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
using System.Globalization;
using System.Text;


using HtmlRenderer.Drawing;
using HtmlRenderer.Css;

namespace HtmlRenderer.Boxes
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
        readonly object _controller;

#if DEBUG
        public readonly int __aa_dbugId = dbugTotalId++;
        static int dbugTotalId;
        public int dbugMark;
#endif
 
    
        public CssBox(object controller, Css.BoxSpec spec) 
        {
            this._aa_boxes = new CssBoxCollection();
            this._controller = controller; 
#if DEBUG
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
        public CssBox(object controller, Css.BoxSpec spec, Css.CssDisplay fixDisplayType) 
        {
            this._aa_boxes = new CssBoxCollection();
            this._controller = controller; 

#if DEBUG
            if (!spec.IsFreezed)
            {
                //must be freezed 
                throw new NotSupportedException();
            }
#endif

            //assign spec             
            this._boxCompactFlags |= BoxFlags.FIXED_DISPLAY_TYPE;
            this._cssDisplay = fixDisplayType;
            //----------------------------
            this._myspec = spec;
            EvaluateSpec(spec);
            ChangeDisplayType(this, _myspec.CssDisplay);


        }
       
        /// <summary>
        /// Gets the parent box of this box
        /// </summary>
        public CssBox ParentBox
        {
            get { return _parentBox; }
        }

        /// <summary>
        /// 1. remove this box from its parent and 2. add to new parent box
        /// </summary>
        /// <param name="parentBox"></param>
        internal void SetNewParentBox(CssBox parentBox)
        {
            if (this._parentBox != null)
            {
                this._parentBox.Boxes.Remove(this);
            }
            if (parentBox != null)
            {
                parentBox.Boxes.AddChild(parentBox, this);
            }
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
        public bool IsSvgRootElement
        {
            get
            {
                return (this._boxCompactFlags & BoxFlags.IS_CUSTOM_CSSBOX) != 0;
            }
        }
        /// <summary>
        /// is the box "Display" is "Inline", is this is an inline box and not block.
        /// </summary>
        public bool IsInline
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
                return this.RunCount == 1 && this.FirstRun.IsImage;
            }
        }

        /// <summary>
        /// Tells if the box is empty or contains just blank spaces
        /// </summary>
        public bool IsSpaceOrEmpty
        {
            get
            {
                if (this.Boxes.Count != 0)
                {
                    return true;
                }
                else if (this._aa_contentRuns != null)
                {
                    return this._aa_contentRuns.Count > 0;
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
        protected void NeedRecomputeMinimalRun()
        {


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

        internal bool HasRuns
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
        #region Private Methods

        //static int dbugCC = 0;

        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.<br/>
        /// </summary>
        /// <param name="g">Device context to use</param>
        protected virtual void PerformContentLayout(LayoutVisitor lay)
        {
            //int dbugStep = dbugCC++;
            //----------------------------------------------------------- 
            switch (this.CssDisplay)
            {
                case Css.CssDisplay.None:
                    {
                        return;
                    }
                default:
                    {
                        //others ... 
                        if (this.NeedComputedValueEvaluation) { this.ReEvaluateComputedValues(lay.Gfx, lay.LatestContainingBlock); }
                        this.MeasureRunsSize(lay);

                    } break;
                //case Css.CssDisplay.BlockInsideInlineAfterCorrection:
                case Css.CssDisplay.Block:
                case Css.CssDisplay.ListItem:
                case Css.CssDisplay.Table:
                case Css.CssDisplay.InlineTable:
                case Css.CssDisplay.TableCell:
                    {
                        //this box has its own  container property
                        //this box may use...
                        // 1) line formatting context  , or
                        // 2) block formatting context  
                        if (this.NeedComputedValueEvaluation) { this.ReEvaluateComputedValues(lay.Gfx, lay.LatestContainingBlock); }
                        this.MeasureRunsSize(lay);
                        //---------------------------------------------------------
                        //for general block layout 
                        CssLayoutEngine.PerformContentLayout(this, lay);

                    } break;
            }
            //----------------------------------------------------------------------------- 
            //set height  
            UpdateIfHigher(this, ExpectedHeight);

            if (_subBoxes != null)
            {
                //layout
                _subBoxes.PerformLayout(this, lay);
            }
            //update back 
            lay.UpdateRootSize(this);
        }

        static void UpdateIfHigher(CssBox box, float newHeight)
        {
            if (newHeight > box.SizeHeight)
            {
                box.SetHeight(newHeight);
            }
        }
        internal void SetHeightToZero()
        {
            this.SetHeight(0);
        }
        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        /// <param name="g"></param>
        internal virtual void MeasureRunsSize(LayoutVisitor lay)
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
            if (this.RunCount > 0)
            {
                //find word spacing  
                float actualWordspacing = this._actualWordSpacing;
                Font actualFont = this.ActualFont;
                var fontInfo = lay.GetFontInfo(actualFont);
                float fontHeight = fontInfo.LineHeight;

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
                                run.Width = lay.MeasureStringWidth(
                                    CssBox.UnsafeGetTextBuffer(this),
                                    textRun.TextStartIndex,
                                    textRun.TextLength,
                                    actualFont);

                                //run.Width = FontsUtils.MeasureStringWidth(lay.Gfx,
                                //    CssBox.UnsafeGetTextBuffer(this),
                                //    textRun.TextStartIndex,
                                //    textRun.TextLength,
                                //    actualFont);

                            } break;
                        case CssRunKind.SingleSpace:
                            {
                                run.Width = actualWordspacing;
                            } break;
                        case CssRunKind.Space:
                            {
                                //other space size                                     
                                run.Width = actualWordspacing * ((CssTextRun)run).TextLength;
                            } break;
                        case CssRunKind.LineBreak:
                            {
                                run.Width = 0;
                            } break;
                    }
                }
            }
            this._boxCompactFlags |= BoxFlags.LAY_RUNSIZE_MEASURE;
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

            if (_lastCalculationEpisodeNum == calculationEpisode)
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


        bool IsLastChild
        {
            get
            {
                return this.ParentBox.Boxes.GetLastChild() == this;
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
        internal float GetHeightAfterMarginBottomCollapse(CssBox cbBox)
        {

            float margin = 0;
            if (ParentBox != null && this.IsLastChild && cbBox.ActualMarginBottom < 0.1)
            {
                var lastChildBottomMargin = _aa_boxes.GetLastChild().ActualMarginBottom;
                margin = (Height.IsAuto) ? Math.Max(ActualMarginBottom, lastChildBottomMargin) : lastChildBottomMargin;
            }
            return _aa_boxes.GetLastChild().LocalBottom + margin + this.ActualPaddingBottom + ActualBorderBottomWidth;

            //must have at least 1 child 
            //float lastChildBottomWithMarginRelativeToMe = this.LocalY + _boxes[_boxes.Count - 1].LocalActualBottom + margin + this.ActualPaddingBottom + this.ActualBorderBottomWidth;
            //return Math.Max(GlobalActualBottom, lastChildBottomWithMarginRelativeToMe);
            //return Math.Max(GlobalActualBottom, _boxes[_boxes.Count - 1].GlobalActualBottom + margin + this.ActualPaddingBottom + this.ActualBorderBottomWidth);
        }
        internal void OffsetLocalTop(float dy)
        {
            this._localY += dy;
        }





        ///// <summary>
        ///// Get brush for selection background depending if it has external and if alpha is required for images.
        ///// </summary>
        ///// <param name="forceAlpha">used for images so they will have alpha effect</param>
        //protected Brush GetSelectionBackBrush(bool forceAlpha)
        //{
        //    var backColor = HtmlContainer.SelectionBackColor;
        //    if (backColor != System.Drawing.Color.Empty)
        //    {
        //        if (forceAlpha && backColor.A > 180)
        //            return RenderUtils.GetSolidBrush(System.Drawing.Color.FromArgb(180, backColor.R, backColor.G, backColor.B));
        //        else
        //            return RenderUtils.GetSolidBrush(backColor);
        //    }
        //    else
        //    {
        //        return CssUtils.DefaultSelectionBackcolor;
        //    }
        //}


        internal bool CanBeRefererenceSibling
        {
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
        #endregion





    }
}