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

using HtmlRenderer.Parse;
using HtmlRenderer.Utils;


namespace HtmlRenderer.Dom
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
        readonly BoxSpec _myspec;

        WellknownHtmlTagName wellKnownTagName;

#if DEBUG
        public readonly int __aa_dbugId = dbugTotalId++;
        static int dbugTotalId;
        public int dbugMark;
#endif

        public CssBox(CssBox parentBox, BridgeHtmlElement element, BoxSpec spec)
        {

            this._aa_boxes = new CssBoxCollection(this);
            if (parentBox != null)
            {
                parentBox.Boxes.Add(this);
            }
            this._htmlElement = element;
#if DEBUG
            if (element != null && element.Spec == null)
            {

            }
#endif
            if (element != null)
            {
                this.WellknownTagName = element.WellknownTagName;
            }

#if DEBUG
            if (!spec.IsFreezed)
            {
                //must be freeze
            }
#endif

            //assign spec

            this._myspec = spec;
            EvaluateSpec(spec);
            ChangeDisplayType(this, _myspec.CssDisplay);
        }



        /// <summary>
        /// Gets the HtmlContainer of the Box.
        /// WARNING: May be null.
        /// </summary>
        public HtmlContainer HtmlContainer
        {
            get { return _htmlContainer ?? (_parentBox != null ? _htmlContainer = _parentBox.HtmlContainer : null); }

        }
        public static void SetHtmlContainer(CssBox htmlRoot, HtmlContainer container)
        {
            htmlRoot._htmlContainer = container;
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
                parentBox.Boxes.Add(this);
                _htmlContainer = parentBox.HtmlContainer;
            }
        }
        internal void SetNewParentBox(int myIndexHint, CssBox parentBox)
        {
            if (this._parentBox != null)
            {
                this._parentBox.Boxes.RemoveAt(myIndexHint);
            }
            if (parentBox != null)
            {
                parentBox.Boxes.Add(this);
                _htmlContainer = parentBox.HtmlContainer;
            }
        }
        /// <summary>
        /// Is the box is of "br" element.
        /// </summary>
        public bool IsBrElement
        {
            get
            {
                return this.WellknownTagName == WellknownHtmlTagName.br;
            }
        }

        /// <summary>
        /// is the box "Display" is "Inline", is this is an inline box and not block.
        /// </summary>
        public bool IsInline
        {
            get
            {
                return (this._boxCompactFlags & CssBoxFlagsConst.IS_INLINE_BOX) != 0;
            }
            set
            {
                if (value)
                {
                    this._boxCompactFlags |= CssBoxFlagsConst.IS_INLINE_BOX;
                }
                else
                {
                    this._boxCompactFlags &= ~CssBoxFlagsConst.IS_INLINE_BOX;
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
                return this.CssDisplay == CssDisplay.Block;
            }
        }

        internal bool HasContainingBlockProperty
        {
            get
            {
                //this flags is evaluated when call ChangeDisplay ****
                return (this._boxCompactFlags & CssBoxFlagsConst.HAS_CONTAINER_PROP) != 0;
            }
        }
        /// <summary>
        /// Gets the containing block-box of this box. (The nearest parent box with display=block)
        /// </summary>
        internal CssBox SearchUpForContainingBlockBox()
        {

            if (ParentBox == null)
            {
                return this; //This is the initial containing block.
            }

            var box = ParentBox;
            while (box.CssDisplay < CssDisplay.__CONTAINER_BEGIN_HERE &&
                box.ParentBox != null)
            {
                box = box.ParentBox;
            }

            //Comment this following line to treat always superior box as block
            if (box == null)
                throw new Exception("There's no containing block on the chain");

            return box;

        }

        /// <summary>
        /// Gets the HtmlElement that hosts this box
        /// </summary>
        public IHtmlElement HtmlElement
        {
            get { return _htmlElement; }
        }

        /// <summary>
        /// Gets if this box represents an image
        /// </summary>
        public bool IsImage
        {
            get
            {
                return this.HasRuns && this.FirstRun.IsImage;
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
            tmpFlags &= ~CssBoxFlagsConst.HAS_EVAL_WHITESPACE;
            tmpFlags &= ~CssBoxFlagsConst.TEXT_IS_ALL_WHITESPACE;
            tmpFlags &= ~CssBoxFlagsConst.TEXT_IS_EMPTY;
            this._boxCompactFlags = tmpFlags;
        }


        bool _isAllWhitespace;
        internal void SetTextBuffer(char[] textBuffer)
        {
            this._buffer = textBuffer;
        }
        internal void SetContentRuns(List<CssRun> runs, bool isAllWhitespace)
        {
            this._aa_contentRuns = runs;
            this._isAllWhitespace = isAllWhitespace;
        }
        public bool MayHasSomeTextContent
        {
            get
            {
                return this._aa_contentRuns != null;
            }
        }
        internal static char[] UnsafeGetTextBuffer(CssBox box)
        {
            return box._buffer;
        }
        internal bool TextContentIsAllWhitespace
        {
            get
            {
                return this._isAllWhitespace;
                //if ((this._boxCompactFlags & CssBoxFlagsConst.HAS_EVAL_WHITESPACE) == 0)
                //{
                //    EvaluateWhitespace();
                //}
                //return (this._boxCompactFlags & CssBoxFlagsConst.TEXT_IS_ALL_WHITESPACE) != 0;
            }
        }
        internal bool TextContentIsWhitespaceOrEmptyText
        {
            get
            {
                if (this._aa_contentRuns != null)
                {
                    return this._isAllWhitespace;
                }
                else
                {
                    return ChildCount == 0;
                }
                //if (ChildCount == 0)
                //{
                //}
                //if ((this._boxCompactFlags & CssBoxFlagsConst.HAS_EVAL_WHITESPACE) == 0)
                //{
                //    EvaluateWhitespace();
                //}
                //return ((this._boxCompactFlags & CssBoxFlagsConst.TEXT_IS_ALL_WHITESPACE) != 0) ||
                //        ((this._boxCompactFlags & CssBoxFlagsConst.TEXT_IS_EMPTY) != 0);
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

        internal bool HasRuns
        {
            get
            {
                return this._aa_contentRuns != null && this._aa_contentRuns.Count > 0;
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
            PerformContentLayout(lay);
        }

        internal void ChangeSiblingOrder(int siblingIndex)
        {
            if (siblingIndex < 0)
            {
                throw new Exception("before box doesn't exist on parent");
            }
            this._parentBox.Boxes.ChangeSiblingIndex(this, siblingIndex);
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
                case Dom.CssDisplay.None:
                    {
                        return;
                    }
                default:
                    {
                        if (this.NeedComputedValueEvaluation) { this.ReEvaluateComputedValues(lay.LatestContainingBlock); }
                        this.MeasureRunsSize(lay);

                        //others
                    } break;
                case Dom.CssDisplay.BlockInsideInlineAfterCorrection:
                case Dom.CssDisplay.Block:
                case Dom.CssDisplay.ListItem:
                case Dom.CssDisplay.Table:
                case Dom.CssDisplay.InlineTable:
                case Dom.CssDisplay.TableCell:
                    {

                        CssBox myContainingBlock = lay.LatestContainingBlock;
                        if (this.NeedComputedValueEvaluation) { this.ReEvaluateComputedValues(myContainingBlock); }

                        this.MeasureRunsSize(lay);

                        if (CssDisplay != Dom.CssDisplay.TableCell)
                        {
                            //-------------------------------------------
                            if (this.CssDisplay != Dom.CssDisplay.Table)
                            {
                                float availableWidth = myContainingBlock.ClientWidth;

                                if (!this.Width.IsEmptyOrAuto)
                                {
                                    availableWidth = CssValueParser.ConvertToPx(Width, availableWidth, this);
                                }

                                this.SetWidth(availableWidth);
                                // must be separate because the margin can be calculated by percentage of the width
                                this.SetWidth(availableWidth - ActualMarginLeft - ActualMarginRight);
                            }
                            //-------------------------------------------

                            float localLeft = myContainingBlock.ClientLeft + this.ActualMarginLeft;
                            float localTop = 0; 
                            var prevSibling = lay.LatestSiblingBox;
                            if (prevSibling == null)
                            {
                                //this is first child of parent
                                if (this.ParentBox != null)
                                {
                                    localTop = myContainingBlock.ClientTop;
                                }
                            }
                            else
                            {
                                localTop = prevSibling.LocalBottom + prevSibling.ActualBorderBottomWidth;
                            }

                            localTop += MarginTopCollapse(prevSibling);

                            this.SetLocation(localLeft, localTop);
                            this.SetHeightToZero();
                        }

                        //--------------------------------------------------------------------------

                        switch (this.CssDisplay)
                        {
                            case Dom.CssDisplay.Table:
                            case Dom.CssDisplay.InlineTable:
                                {
                                    //If we're talking about a table here..

                                    lay.PushContaingBlock(this);
                                    var currentLevelLatestSibling = lay.LatestSiblingBox;
                                    lay.LatestSiblingBox = null;//reset

                                    CssTableLayoutEngine.PerformLayout(this, lay);

                                    lay.LatestSiblingBox = currentLevelLatestSibling;
                                    lay.PopContainingBlock();

                                } break;
                            default:
                                {

                                    //formatting context for
                                    //1. inline formatting context
                                    //2. block formatting context 
                                    if (DomUtils.ContainsInlinesOnly(this))
                                    {
                                        this.SetHeightToZero();
                                        CssLayoutEngine.FlowContentRuns(this, lay); //This will automatically set the bottom of this block

                                    }
                                    else if (_aa_boxes.Count > 0)
                                    {
                                        lay.PushContaingBlock(this);
                                        var currentLevelLatestSibling = lay.LatestSiblingBox;
                                        lay.LatestSiblingBox = null;//reset

                                        foreach (var childBox in Boxes)
                                        {
                                            childBox.PerformLayout(lay);

                                            if (childBox.CanBeRefererenceSibling)
                                            {
                                                lay.LatestSiblingBox = childBox;
                                            }
                                        }

                                        lay.LatestSiblingBox = currentLevelLatestSibling;
                                        lay.PopContainingBlock();
                                        //------------------------------------------------

                                        float width = this.CalculateActualWidth();
                                        if (lay.ContainerBlockGlobalX + width > ConstConfig.BOX_MAX_RIGHT)
                                        {

                                        }
                                        else
                                        {
                                            if (this.CssDisplay != Dom.CssDisplay.TableCell)
                                            {
                                                this.SetWidth(width);
                                            }
                                        }
                                        this.SetHeight(GetHeightAfterMarginBottomCollapse(lay.LatestContainingBlock));
                                    }
                                } break;
                        }

                        //--------------------------------------------------------------------------
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
        protected void SetHeightToZero()
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
            if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_RUNSIZE_MEASURE) != 0)
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
            if (this.HasRuns)
            {
                float actualWordspacing = MeasureWordSpacing(lay);
                Font actualFont = this.ActualFont;
                float fontHeight = FontsUtils.GetFontHeight(actualFont);

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
                                run.Width = FontsUtils.MeasureStringWidth(lay.Gfx,
                                    CssBox.UnsafeGetTextBuffer(this),
                                    textRun.TextStartIndex,
                                    textRun.TextLength,
                                    actualFont);

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
            this._boxCompactFlags |= CssBoxFlagsConst.LAY_RUNSIZE_MEASURE;
        } 

        /// <summary>
        /// Gets the minimum width that the box can be.
        /// *** The box can be as thin as the longest word plus padding
        /// </summary>
        /// <returns></returns>
        internal float CalculateMinimumWidth()
        {

            float maxWidth = 0;
            float padding = 0f;

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

            return maxWidth + padding;

        }
        static void CalculateMinimumWidthAndWidestRun(CssBox box, out float maxWidth, out CssRun maxWidthRun)
        {
            //use line-base style ***

            float maxRunWidth = 0;
            CssRun foundRun = null;
            foreach (CssLineBox lineBox in box.GetLineBoxIter())
            {
                foreach (CssRun run in lineBox.GetRunIter())
                {
                    if (run.Width >= maxRunWidth)
                    {
                        foundRun = run;
                        maxRunWidth = run.Width;
                    }
                }
            }
            maxWidth = maxRunWidth;
            maxWidthRun = foundRun;
        }
        float CalculateActualWidth()
        {
            float maxRight = 0;
            foreach (var box in Boxes)
            {
                maxRight = Math.Max(maxRight, box.LocalRight);
            }
            return maxRight + (this.ActualBorderLeftWidth + this.ActualPaddingLeft +
                this.ActualPaddingRight + this.ActualBorderRightWidth);
        }

        bool IsLastChild
        {
            get
            {
                return this.ParentBox.Boxes[this.ParentBox.ChildCount - 1] == this;
            }
        }
        /// <summary>
        /// Gets the result of collapsing the vertical margins of the two boxes
        /// </summary>
        /// <param name="upperSibling">the previous box under the same parent</param>
        /// <returns>Resulting top margin</returns>
        protected float MarginTopCollapse(CssBox upperSibling)
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
        private float GetHeightAfterMarginBottomCollapse(CssBox cbBox)
        {

            float margin = 0;
            //if (ParentBox != null && this.IsLastChild && _parentBox.ActualMarginBottom < 0.1)

            if (ParentBox != null && this.IsLastChild && cbBox.ActualMarginBottom < 0.1)
            {
                var lastChildBottomMargin = _aa_boxes[_aa_boxes.Count - 1].ActualMarginBottom;

                margin = (Height.IsAuto) ? Math.Max(ActualMarginBottom, lastChildBottomMargin) : lastChildBottomMargin;
            }
            return _aa_boxes[_aa_boxes.Count - 1].LocalBottom + margin + this.ActualPaddingBottom + ActualBorderBottomWidth;

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
        ///// On image load process complete with image request refresh for it to be painted.
        ///// </summary>
        ///// <param name="image">the image loaded or null if failed</param>
        ///// <param name="rectangle">the source rectangle to draw in the image (empty - draw everything)</param>
        ///// <param name="async">is the callback was called async to load image call</param>
        //private void OnImageLoadComplete(Image image, Rectangle rectangle, bool async)
        //{
        //    if (image != null && async)
        //    {
        //        HtmlContainer.RequestRefresh(false);
        //    }
        //}



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
            get { return this.CssDisplay != Dom.CssDisplay.None && this.Position != CssPosition.Absolute; }
        }


#if DEBUG
        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var tag = HtmlElement != null ? string.Format("<{0}>", HtmlElement.Name) : "anon";

            if (IsBlock)
            {
                return string.Format("{0}{1} Block {2}, Children:{3}", ParentBox == null ? "Root: " : string.Empty, tag, FontSize, Boxes.Count);
            }
            else if (this.CssDisplay == CssDisplay.None)
            {
                return string.Format("{0}{1} None", ParentBox == null ? "Root: " : string.Empty, tag);
            }
            else
            {
                if (this.MayHasSomeTextContent)
                {
                    return string.Format("{0}{1} {2}: {3}", ParentBox == null ? "Root: " : string.Empty, tag,
                        this.CssDisplay.ToCssStringValue(), this.dbugCopyTextContent());
                }
                else
                {
                    return string.Format("{0}{1} {2}: {3}", ParentBox == null ? "Root: " : string.Empty, tag,
                        this.CssDisplay.ToCssStringValue(), "");
                }
            }
        }
#endif
        #endregion





    }
}