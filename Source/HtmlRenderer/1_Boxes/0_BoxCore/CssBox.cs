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
using System.Drawing.Drawing2D;
using System.Globalization;
using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;
using System.Text;

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
    public partial class CssBox : CssBoxBase, IDisposable
    {

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="parentBox">optional: the parent of this css box in html</param>
        /// <param name="tag">optional: the html tag associated with this css box</param>
        public CssBox(CssBox parentBox, IHtmlElement tag)
        {
             
            this._aa_boxes = new CssBoxCollection(this);
            if (parentBox != null)
            {
                parentBox.Boxes.Add(this);
            }
            
            _htmlElement = tag;
            if (tag != null)
            {
                this.WellknownTagName = tag.WellknownTagName;
            }
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
                return (this.CssDisplay == CssDisplay.Inline
                    || this.CssDisplay == CssDisplay.InlineBlock)
                    && !IsBrElement;
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



        /// <summary>
        /// Get the href link of the box (by default get "href" attribute)
        /// </summary>
        public virtual string HrefLink
        {
            get { return GetAttribute("href"); }
        }
        internal bool HasContainingBlockProperty
        {
            get
            {
                switch (this.CssDisplay)
                {
                    case CssDisplay.Block:
                    case CssDisplay.ListItem:
                    case CssDisplay.Table:
                    case CssDisplay.TableCell:
                        return true;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// Gets the containing block-box of this box. (The nearest parent box with display=block)
        /// </summary>
        public CssBox ContainingBlock
        {
            get
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
                if ((Runs.Count != 0 || Boxes.Count != 0) && (Runs.Count != 1 || !Runs[0].IsSpaces))
                {
                    foreach (CssRun word in Runs)
                    {
                        if (!word.IsSpaces)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        public static char[] UnsafeGetTextBuffer(CssBox box)
        {
            return box._aa_textBuffer;
        } 
        void ResetTextFlags()
        {
            int tmpFlags = this._boxCompactFlags;
            tmpFlags &= ~CssBoxFlagsConst.HAS_EVAL_WHITESPACE;
            tmpFlags &= ~CssBoxFlagsConst.TEXT_IS_ALL_WHITESPACE;
            tmpFlags &= ~CssBoxFlagsConst.TEXT_IS_EMPTY;
            this._boxCompactFlags = tmpFlags;
        } 
        internal void SetTextContent(char[] chars)
        {
            this._aa_textBuffer = chars;
            ResetTextFlags();
        }
        internal void SetTextContent2(char[] chars)
        {   
            
            this._aa_textBuffer = chars;
            ResetTextFlags();
        }
        public bool MayHasSomeTextContent
        {
            get
            {
                return this._aa_textBuffer != null;
            }
        }
        void EvaluateWhitespace()
        {

            this._boxCompactFlags |= CssBoxFlagsConst.HAS_EVAL_WHITESPACE;
            char[] tmp;

            if ((tmp = this._aa_textBuffer) == null)
            {

                this._boxCompactFlags |= CssBoxFlagsConst.TEXT_IS_EMPTY;
                return;
            }
            for (int i = tmp.Length - 1; i >= 0; --i)
            {
                if (!char.IsWhiteSpace(tmp[i]))
                {
                    return;
                }
            }

            //all is whitespace
            this._boxCompactFlags |= CssBoxFlagsConst.TEXT_IS_ALL_WHITESPACE;
        }
        internal bool TextContentIsAllWhitespace
        {
            get
            {
                if ((this._boxCompactFlags & CssBoxFlagsConst.HAS_EVAL_WHITESPACE) == 0)
                {
                    EvaluateWhitespace();
                }
                return (this._boxCompactFlags & CssBoxFlagsConst.TEXT_IS_ALL_WHITESPACE) != 0;
            }
        }
        internal bool TextContentIsWhitespaceOrEmptyText
        {
            get
            {
                if ((this._boxCompactFlags & CssBoxFlagsConst.HAS_EVAL_WHITESPACE) == 0)
                {
                    EvaluateWhitespace();
                }
                return ((this._boxCompactFlags & CssBoxFlagsConst.TEXT_IS_ALL_WHITESPACE) != 0) ||
                        ((this._boxCompactFlags & CssBoxFlagsConst.TEXT_IS_EMPTY) != 0);
            }
        }
        internal string CopyTextContent()
        {
            if (this._aa_textBuffer != null)
            {
                return new string(this._aa_textBuffer);
            }
            else
            {
                return null;
            }
        }
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
            if (box._boxRuns == null)
            {
                firstHostLine = lastHostLine = null;
            }
            else
            {
                int j = box._boxRuns.Count;

                firstHostLine = box._boxRuns[0].HostLine;
                lastHostLine = box._boxRuns[j - 1].HostLine;
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
            get { return _boxRuns; }
        }

        internal bool HasRuns
        {
            get
            {
                return this._boxRuns != null && this._boxRuns.Count > 0;
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            //if (_imageLoadHandler != null)
            //{
            //    _imageLoadHandler.Dispose();
            //}

            foreach (var childBox in Boxes)
            {
                childBox.Dispose();
            }
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
                                    availableWidth = CssValueParser.ParseLength(Width, availableWidth, this);
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
                        //If we're talking about a table here..
                        switch (this.CssDisplay)
                        {
                            case Dom.CssDisplay.Table:
                            case Dom.CssDisplay.InlineTable:
                                {


                                    lay.PushContaingBlock(this);
                                    var currentLevelLatestSibling = lay.LatestSiblingBox;
                                    lay.LatestSiblingBox = null;//reset

                                    CssTableLayoutEngine.PerformLayout(this, lay);

                                    lay.LatestSiblingBox = currentLevelLatestSibling;
                                    lay.PopContainingBlock();

                                } break;
                            default:
                                {
                                    //If there's just inline boxes, create LineBoxes
                                    if (DomUtils.ContainsInlinesOnly(this))
                                    {
                                        this.SetHeightToZero();
                                        //CssLayoutEngine.FlowContentRuns(this, lay); //This will automatically set the bottom of this block
                                        CssLayoutEngine.FlowContentRunsV2(this, lay); //This will automatically set the bottom of this block
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

                                        float width = this.CalculateActualWidth();
                                        if (lay.ContainerBlockGlobalX + width > CssBoxConst.MAX_RIGHT)
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

            this.CreateListItemBox(lay);
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

                var tmpRuns = this._boxRuns;
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
        /// Get the parent of this css properties instance.
        /// </summary>
        /// <returns></returns>
        public sealed override CssBoxBase GetParent()
        {
            return _parentBox;
        }

        /// <summary>
        /// Gets the index of the box to be used on a (ordered) list
        /// </summary>
        /// <returns></returns>
        private int GetIndexForList()
        {
            bool reversed = !string.IsNullOrEmpty(ParentBox.GetAttribute("reversed"));
            int index;
            if (!int.TryParse(ParentBox.GetAttribute("start"), out index))
            {
                if (reversed)
                {
                    index = 0;
                    foreach (CssBox b in ParentBox.Boxes)
                    {
                        if (b.CssDisplay == CssDisplay.ListItem)
                        {
                            index++;
                        }
                    }
                }
                else
                {
                    index = 1;
                }
            }

            foreach (CssBox b in ParentBox.Boxes)
            {
                if (b.Equals(this))
                    return index;

                //if (b.Display == CssConstants.ListItem)
                if (b.CssDisplay == CssDisplay.ListItem)
                    index += reversed ? -1 : 1;
            }

            return index;
        }
        static readonly char[] discItem = new[] { '•' };
        static readonly char[] circleItem = new[] { 'o' };
        static readonly char[] squareItem = new[] { '♠' };


        /// <summary>
        /// Creates the <see cref="_listItemBox"/>
        /// </summary>
        /// <param name="g"></param>
        void CreateListItemBox(LayoutVisitor lay)
        {

            if (this.CssDisplay == CssDisplay.ListItem &&
                ListStyleType != CssListStyleType.None)
            {
                if (_listItemBox == null)
                {
                    _listItemBox = new CssBox(null, null);
                    _listItemBox.InheritStyles(this);
                    _listItemBox.CssDisplay = CssDisplay.Inline;
                    _listItemBox._htmlContainer = HtmlContainer;

                    switch (this.ListStyleType)
                    {
                        case CssListStyleType.Disc:
                            {
                                _listItemBox.SetTextContent(discItem);
                            } break;
                        case CssListStyleType.Circle:
                            {
                                _listItemBox.SetTextContent(circleItem);
                            } break;
                        case CssListStyleType.Square:
                            {
                                _listItemBox.SetTextContent(squareItem);
                            } break;
                        case CssListStyleType.Decimal:
                            {
                                _listItemBox.SetTextContent((GetIndexForList().ToString(CultureInfo.InvariantCulture) + ".").ToCharArray());
                            } break;
                        case CssListStyleType.DecimalLeadingZero:
                            {
                                _listItemBox.SetTextContent((GetIndexForList().ToString("00", CultureInfo.InvariantCulture) + ".").ToCharArray());
                            } break;
                        default:
                            {
                                _listItemBox.SetTextContent((CommonUtils.ConvertToAlphaNumber(GetIndexForList(), ListStyleType) + ".").ToCharArray());
                            } break;
                    }


                    _listItemBox.ParseWordContent();

                    var prevSibling = lay.LatestSiblingBox;
                    lay.LatestSiblingBox = null;//reset
                    _listItemBox.PerformLayout(lay);
                    lay.LatestSiblingBox = prevSibling;


                    var fRun = _listItemBox.FirstRun;

                    _listItemBox.FirstRun.SetSize(fRun.Width, fRun.Height);
                }

                _listItemBox.FirstRun.SetLocation(_listItemBox.SizeWidth - 5, ActualPaddingTop);

            }
        } 
        internal void ParseWordContent()
        {
            ContentTextSplitter.DefaultSplitter.ParseWordContent(this);
        }
#if DEBUG
        internal string dbugGetTextContent()
        {
            return new string(this._aa_textBuffer);
        }
#endif
        /// <summary>
        /// Gets the specified Attribute, returns string.Empty if no attribute specified
        /// </summary>
        /// <param name="attribute">Attribute to retrieve</param>
        /// <returns>Attribute value or string.Empty if no attribute specified</returns>
        public string GetAttribute(string attribute)
        {
            return GetAttribute(attribute, string.Empty);
        }

        /// <summary>
        /// Gets the value of the specified attribute of the source HTML tag.
        /// </summary>
        /// <param name="attribute">Attribute to retrieve</param>
        /// <param name="defaultValue">Value to return if attribute is not specified</param>
        /// <returns>Attribute value or defaultValue if no attribute specified</returns>
        public string GetAttribute(string attribute, string defaultValue)
        {
            return HtmlElement != null ? HtmlElement.TryGetAttribute(attribute, defaultValue) : defaultValue;
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


        /// <summary>
        /// Inherits inheritable values from parent.
        /// </summary>
        internal new void InheritStyles(CssBoxBase box, bool clone = false)
        {
            base.InheritStyles(box, clone);
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
        /// <summary>
        /// Paints the background of the box
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="rect">the bounding rectangle to draw in</param>
        /// <param name="isFirst">is it the first rectangle of the element</param>
        /// <param name="isLast">is it the last rectangle of the element</param>
        internal void PaintBackground(PaintVisitor p, RectangleF rect, bool isFirst, bool isLast)
        {


            if (rect.Width > 0 && rect.Height > 0)
            {
                Brush brush = null;
                bool dispose = false;
                IGraphics g = p.Gfx;

                SmoothingMode smooth = g.SmoothingMode;

                if (BackgroundGradient != System.Drawing.Color.Transparent)
                {
                    brush = new LinearGradientBrush(rect,
                        ActualBackgroundColor,
                        ActualBackgroundGradient,
                        ActualBackgroundGradientAngle);

                    dispose = true;
                }
                else if (RenderUtils.IsColorVisible(ActualBackgroundColor))
                {

                    brush = RenderUtils.GetSolidBrush(ActualBackgroundColor);
                }

                if (brush != null)
                {
                    // atodo: handle it correctly (tables background)
                    // if (isLast)
                    //  rectangle.Width -= ActualWordSpacing + CssUtils.GetWordEndWhitespace(ActualFont);

                    GraphicsPath roundrect = null;
                    bool isRound = this.HasRoundCorner;
                    if (isRound)
                    {
                        roundrect = RenderUtils.GetRoundRect(rect, ActualCornerNW, ActualCornerNE, ActualCornerSE, ActualCornerSW);
                    }

                    if (!p.AvoidGeometryAntialias && isRound)
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                    }

                    if (roundrect != null)
                    {
                        g.FillPath(brush, roundrect);
                    }
                    else
                    {
                        g.FillRectangle(brush, (float)Math.Ceiling(rect.X), (float)Math.Ceiling(rect.Y), rect.Width, rect.Height);
                    }

                    g.SmoothingMode = smooth;

                    if (roundrect != null) roundrect.Dispose();
                    if (dispose) brush.Dispose();
                }

                if (isFirst)
                {
                    var bgImageBinder = this.BackgroundImageBinder;
                    if (bgImageBinder != null && bgImageBinder.Image != null)
                    {
                        BackgroundImageDrawHandler.DrawBackgroundImage(g, this, bgImageBinder, rect);
                    }
                }
            }
        }

        internal void PaintDecoration(IGraphics g, RectangleF rectangle, bool isFirst, bool isLast)
        {
            float y = 0f;
            switch (this.TextDecoration)
            {
                default:
                    return;
                case CssTextDecoration.Underline:
                    {
                        var h = g.MeasureString(" ", ActualFont).Height;
                        float desc = FontsUtils.GetDescent(ActualFont, g);
                        y = (float)Math.Round(rectangle.Top + h - desc + 0.5);
                    } break;
                case CssTextDecoration.LineThrough:
                    {
                        y = rectangle.Top + rectangle.Height / 2f;
                    } break;
                case CssTextDecoration.Overline:
                    {
                        y = rectangle.Top;
                    } break;
            }


            y -= ActualPaddingBottom - ActualBorderBottomWidth;

            float x1 = rectangle.X;
            if (isFirst)
            {
                x1 += ActualPaddingLeft + ActualBorderLeftWidth;
            }


            float x2 = rectangle.Right;
            if (isLast)
            {
                x2 -= ActualPaddingRight + ActualBorderRightWidth;
            }

            var pen = RenderUtils.GetPen(ActualColor);

            g.DrawLine(pen, x1, y, x2, y);
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
                        this.CssDisplay.ToCssStringValue(), this.CopyTextContent());
                }
                else
                {
                    return string.Format("{0}{1} {2}: {3}", ParentBox == null ? "Root: " : string.Empty, tag,
                        this.CssDisplay.ToCssStringValue(), "");
                }
            }
        }

        #endregion



        internal bool IsPointInClientArea(float x, float y)
        {
            //from parent view
            return x >= this.ClientLeft && x < this.ClientRight &&
                   y >= this.ClientTop && y < this.ClientBottom;
        }
        internal bool IsPointInArea(float x, float y)
        {
            //from parent view
            return x >= this.LocalX && x < this.LocalRight &&
                   y >= this.LocalY && y < this.LocalBottom;
        }



    }
}