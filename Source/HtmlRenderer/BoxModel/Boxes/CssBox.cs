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

            this._boxes = new CssBoxCollection(this);

            if (parentBox != null)
            {
                parentBox.Boxes.Add(this);
            }

            _htmltag = tag;
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
            get { return _htmlContainer ?? (_parentBox != null ? _parentBox.HtmlContainer : null); }
            set
            {
                _htmlContainer = value;
            }
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
                return this.WellknownTagName == WellknownHtmlTagName.BR;
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

                //if (this.IsBlock)
                //{
                //    return true;
                //}
                //else
                //{
                //    switch (this.CssDisplay)
                //    {
                //        case CssDisplay.ListItem:
                //        case CssDisplay.Table:
                //        case CssDisplay.TableCell:
                //            return true;
                //        default:
                //            return false;
                //    }
                //}
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
        /// Gets the HTMLTag that hosts this box
        /// </summary>
        public IHtmlElement HtmlTag
        {
            get { return _htmltag; }
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

        internal bool ContainsSelectedRun
        {
            get;
            set;
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
            return box._textBuffer;
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
            this._textBuffer = chars;
            ResetTextFlags();
        }
        public bool MayHasSomeTextContent
        {
            get
            {
                return this._textBuffer != null;
            }
        }
        void EvaluateWhitespace()
        {

            this._boxCompactFlags |= CssBoxFlagsConst.HAS_EVAL_WHITESPACE;
            char[] tmp;

            if ((tmp = this._textBuffer) == null)
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
            if (this._textBuffer != null)
            {
                return new string(this._textBuffer);
            }
            else
            {
                return null;
            }
        }
        internal void AddLineBox(CssLineBox linebox)
        {
            if (this._clientLineBoxes == null)
            {
                this._clientLineBoxes = new LinkedList<CssLineBox>();
            }

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
        internal IEnumerable<CssLineBox> GetLineBoxIter()
        {
            var node = this._clientLineBoxes.First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
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

        internal void UpdateStripInfo(RectangleF r)
        {
            //update summary bound
            this.SummaryBound = RectangleF.Union(this.SummaryBound, r);
        }
        internal RectangleF SummaryBound
        {
            get;
            private set;
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
        /// Gets or sets the first linebox where content of this box appear
        /// </summary>
        internal CssLineBox FirstHostingLineBox
        {
            get { return _firstHostingLineBox; }
            set { _firstHostingLineBox = value; }
        }

        /// <summary>
        /// Gets or sets the last linebox where content of this box appear
        /// </summary>
        internal CssLineBox LastHostingLineBox
        {
            get { return _lastHostingLineBox; }
            set { _lastHostingLineBox = value; }
        }
        /// <summary>
        /// all parts are in the same line box 
        /// </summary>
        internal bool AllPartsAreInTheSameLineBox
        {
            get
            {
                return this._firstHostingLineBox == this._lastHostingLineBox;
            }
        }

        //------------------------------------------------------------------
        /// <summary>
        /// Create new css box for the given parent with the given html tag.<br/>
        /// </summary>
        /// <param name="tag">the html tag to define the box</param>
        /// <param name="parent">the box to add the new box to it as child</param>
        /// <returns>the new box</returns>
        public static CssBox CreateBox(IHtmlElement tag, CssBox parent = null)
        {

            ArgChecker.AssertArgNotNull(tag, "tag");
            switch (tag.WellknownTagName)
            {
                case WellknownHtmlTagName.IMG:
                    return new CssBoxImage(parent, tag);
                case WellknownHtmlTagName.IFREAME:
                    return new CssBoxHr(parent, tag);
                case WellknownHtmlTagName.HR:
                    return new CssBoxHr(parent, tag);
                //test extension box
                case WellknownHtmlTagName.X:
                    var customBox = CreateCustomBox(tag, parent);
                    if (customBox == null)
                    {
                        return new CssBox(parent, tag);
                    }
                    else
                    {
                        return customBox;
                    }
                default:
                    return new CssBox(parent, tag);
            }
        }
        static CssBox CreateCustomBox(IHtmlElement tag, CssBox parent)
        {
            for (int i = generators.Count - 1; i >= 0; --i)
            {
                var newbox = generators[i].CreateCssBox(tag, parent);
                if (newbox != null)
                {
                    return newbox;
                }
            }
            return null;
        }
        /// <summary>
        /// Create new css block box.
        /// </summary>
        /// <returns>the new block box</returns>
        internal static CssBox CreateRootBlock()
        {
            var box = new CssBox(null, null);
            box.CssDisplay = CssDisplay.Block;
            return box;
        }
        /// <summary>
        /// Create new css box for the given parent with the given optional html tag and insert it either
        /// at the end or before the given optional box.<br/>
        /// If no html tag is given the box will be anonymous.<br/>
        /// If no before box is given the new box will be added at the end of parent boxes collection.<br/>
        /// If before box doesn't exists in parent box exception is thrown.<br/>
        /// </summary>
        /// <remarks>
        /// To learn more about anonymous inline boxes visit: http://www.w3.org/TR/CSS21/visuren.html#anonymous
        /// </remarks>
        /// <param name="parent">the box to add the new box to it as child</param>
        /// <param name="tag">optional: the html tag to define the box</param>
        /// <param name="before">optional: to insert as specific location in parent box</param>
        /// <returns>the new box</returns>
        public static CssBox CreateBox(CssBox parent, IHtmlElement tag = null, int insertAt = -1)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            var newBox = new CssBox(parent, tag);
            newBox.InheritStyles(parent);
            if (insertAt > -1)
            {
                newBox.ChangeSiblingOrder(insertAt);
            }
            return newBox;
        }



        /// <summary>
        /// Create new css block box for the given parent with the given optional html tag and insert it either
        /// at the end or before the given optional box.<br/>
        /// If no html tag is given the box will be anonymous.<br/>
        /// If no before box is given the new box will be added at the end of parent boxes collection.<br/>
        /// If before box doesn't exists in parent box exception is thrown.<br/>
        /// </summary>
        /// <remarks>
        /// To learn more about anonymous block boxes visit CSS spec:
        /// http://www.w3.org/TR/CSS21/visuren.html#anonymous-block-level
        /// </remarks>
        /// <param name="parent">the box to add the new block box to it as child</param>
        /// <param name="tag">optional: the html tag to define the box</param>
        /// <param name="before">optional: to insert as specific location in parent box</param>
        /// <returns>the new block box</returns>
        internal static CssBox CreateAnonBlock(CssBox parent, int insertAt = -1)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            var newBox = CreateBox(parent, null, insertAt);
            newBox.CssDisplay = CssDisplay.Block;
            return newBox;
        }

        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.
        /// </summary>
        /// <param name="g">Device context to use</param>
        public void PerformLayout(IGraphics g)
        {
            PerformLayoutImp(g);

        }
        void ChangeSiblingOrder(int siblingIndex)
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
            if (_imageLoadHandler != null)
            {
                _imageLoadHandler.Dispose();
            }

            foreach (var childBox in Boxes)
            {
                childBox.Dispose();
            }
        }


        #region Private Methods

        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.<br/>
        /// </summary>
        /// <param name="g">Device context to use</param>
        protected virtual void PerformLayoutImp(IGraphics g)
        {
            //if (Display != CssConstants.None)
            if (this.CssDisplay != CssDisplay.None)
            {
                ResetSummaryBound();
                MeasureRunsSize(g);
            }


            if (IsBlock ||
                this.CssDisplay == CssDisplay.ListItem ||
                this.CssDisplay == CssDisplay.Table ||
                this.CssDisplay == CssDisplay.InlineTable ||
                this.CssDisplay == CssDisplay.TableCell)
            {
                // Because their width and height are set by CssTable
                //if (Display != CssConstants.TableCell && Display != CssConstants.Table)
                if (this.CssDisplay != CssDisplay.TableCell && this.CssDisplay != CssDisplay.TableCell)
                {
                    float width = ContainingBlock.Size.Width
                                  - ContainingBlock.ActualPaddingLeft - ContainingBlock.ActualPaddingRight
                                  - ContainingBlock.ActualBorderLeftWidth - ContainingBlock.ActualBorderRightWidth;

                    if (!this.Width.IsAuto && !this.Width.IsEmpty)
                    {
                        width = CssValueParser.ParseLength(Width, width, this);
                    }

                    Size = new SizeF(width, Size.Height);

                    // must be separate because the margin can be calculated by percentage of the width
                    Size = new SizeF(width - ActualMarginLeft - ActualMarginRight, Size.Height);
                }

                if (this.CssDisplay != CssDisplay.TableCell)
                {
                    var prevSibling = CssBox.GetPreviousSibling(this);

                    this.LocationX = ContainingBlock.LocationX + ContainingBlock.ActualPaddingLeft + ActualMarginLeft + ContainingBlock.ActualBorderLeftWidth;
                    float top = this.LocationY = (prevSibling == null && ParentBox != null ? ParentBox.ClientTop : ParentBox == null ? this.LocationY : 0) + MarginTopCollapse(prevSibling) + (prevSibling != null ? prevSibling.ActualBottom + prevSibling.ActualBorderBottomWidth : 0);

                    ActualBottom = top;
                }

                //If we're talking about a table here..

                if (this.CssDisplay == CssDisplay.Table || this.CssDisplay == CssDisplay.InlineTable)
                {
                    CssLayoutEngineTable.PerformLayout(g, this);
                }
                else
                {
                    //If there's just inline boxes, create LineBoxes
                    if (DomUtils.ContainsInlinesOnly(this))
                    {
                        ActualBottom = this.LocationY;//Location.Y;
                        CssLayoutEngine.CreateLineBoxes(g, this); //This will automatically set the bottom of this block
                    }
                    else if (_boxes.Count > 0)
                    {
                        foreach (var childBox in Boxes)
                        {
                            childBox.PerformLayout(g);
                        }
                        ActualRight = CalculateActualRight();
                        ActualBottom = MarginBottomCollapse();
                    }
                }
            }
            else
            {

                var prevSibling = CssBox.GetPreviousSibling(this);
                if (prevSibling != null)
                {
                    if (!this.HasAssignLocation)
                    {

                        this.SetLocation(prevSibling.LocationX, prevSibling.LocationY);
                    }

                    ActualBottom = prevSibling.ActualBottom;
                }
            }
            ActualBottom = Math.Max(ActualBottom, this.LocationY + ActualHeight);

            CreateListItemBox(g);

            var actualWidth = Math.Max(GetMinimumWidth() + GetWidthMarginDeep(this), Size.Width < 90999 ? ActualRight : 0);

            //update back
            HtmlContainer.ActualSize = CommonUtils.Max(HtmlContainer.ActualSize, new SizeF(actualWidth, ActualBottom - HtmlContainer.Root.LocationY));
        }

        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        /// <param name="g"></param>
        internal virtual void MeasureRunsSize(IGraphics g)
        {
            if (!_wordsSizeMeasured)
            {
                if (BackgroundImage != CssConstants.None && _imageLoadHandler == null)
                {
                    _imageLoadHandler = new ImageLoadHandler(HtmlContainer, OnImageLoadComplete);
                    _imageLoadHandler.LoadImage(BackgroundImage, HtmlTag);
                }

                MeasureWordSpacing(g);

                if (this.HasRuns)
                {
                    Font actualFont = this.ActualFont;
                    float fontHeight = FontsUtils.GetFontHeight(actualFont);
                    char[] myTextBuffer = CssBox.UnsafeGetTextBuffer(this);
                    float actualWordspacing = this.ActualWordSpacing;

                    foreach (CssRun boxWord in Runs)
                    {
                        //if this is newline then width =0 ***                         
                        switch (boxWord.Kind)
                        {
                            case CssRunKind.Text:
                                {

                                    CssTextRun textRun = (CssTextRun)boxWord;
                                    boxWord.Width = FontsUtils.MeasureStringWidth(g,
                                        myTextBuffer,
                                        textRun.TextStartIndex,
                                        textRun.TextLength,
                                        actualFont);

                                } break;
                            case CssRunKind.SingleSpace:
                                {
                                    boxWord.Width = actualWordspacing;
                                } break;
                            case CssRunKind.Space:
                                {
                                    //other space size                                     
                                    boxWord.Width = actualWordspacing * ((CssTextRun)boxWord).TextLength;
                                } break;
                            case CssRunKind.LineBreak:
                                {
                                    boxWord.Width = 0;
                                } break;
                        }
                        boxWord.Height = fontHeight;
                    }
                }

                _wordsSizeMeasured = true;
            }
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
                        //if (b.Display == CssConstants.ListItem)
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
        private void CreateListItemBox(IGraphics g)
        {
            //if (Display == CssConstants.ListItem && ListStyleType != CssConstants.None)
            if (this.CssDisplay == CssDisplay.ListItem && ListStyleType != CssListStyleType.None)
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
                    _listItemBox.PerformLayoutImp(g);

                    var fRun = _listItemBox.FirstRun;

                    _listItemBox.FirstRun.SetSize(fRun.Width, fRun.Height);
                }
                _listItemBox.FirstRun.SetLocation(this.LocationX - _listItemBox.Size.Width - 5, this.LocationY + ActualPaddingTop);

            }
        }
        void ParseWordContent()
        {
            CssTextSplitter.DefaultSplitter.ParseWordContent(this);
        }
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
            return HtmlTag != null ? HtmlTag.TryGetAttribute(attribute, defaultValue) : defaultValue;
        }

        /// <summary>
        /// Gets the minimum width that the box can be.<br/>
        /// The box can be as thin as the longest word plus padding.<br/>
        /// The check is deep thru box tree.<br/>
        /// </summary>
        /// <returns>the min width of the box</returns>
        internal float GetMinimumWidth()
        {
            float maxWidth = 0;
            CssRun maxWidthWord = null;
            GetMinimumWidth_LongestWord(this, ref maxWidth, ref maxWidthWord);

            float padding = 0f;
            if (maxWidthWord != null)
            {
                var box = maxWidthWord.OwnerBox;
                while (box != null)
                {
                    padding += box.ActualBorderRightWidth + box.ActualPaddingRight + box.ActualBorderLeftWidth + box.ActualPaddingLeft;
                    box = box != this ? box.ParentBox : null;
                }
            }

            return maxWidth + padding;
        }

        /// <summary>
        /// Gets the longest word (in width) inside the box, deeply.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="maxWidth"> </param>
        /// <param name="maxWidthWord"> </param>
        /// <returns></returns>
        private static void GetMinimumWidth_LongestWord(CssBox box, ref float maxWidth, ref CssRun maxWidthWord)
        {
            if (box.HasRuns)
            {
                foreach (CssRun cssRect in box.Runs)
                {
                    if (cssRect.Width > maxWidth)
                    {
                        maxWidth = cssRect.Width;
                        maxWidthWord = cssRect;
                    }
                }
            }
            else
            {
                foreach (CssBox childBox in box.Boxes)
                    GetMinimumWidth_LongestWord(childBox, ref maxWidth, ref maxWidthWord);
            }
        }

        /// <summary>
        /// Get the total margin value (left and right) from the given box to the given end box.<br/>
        /// </summary>
        /// <param name="box">the box to start calculation from.</param>
        /// <returns>the total margin</returns>
        private static float GetWidthMarginDeep(CssBox box)
        {
            float sum = 0f;
            if (box.Size.Width > 90999 || (box.ParentBox != null && box.ParentBox.Size.Width > 90999))
            {
                while (box != null)
                {
                    sum += box.ActualMarginLeft + box.ActualMarginRight;
                    box = box.ParentBox;
                }
            }
            return sum;
        }

        /// <summary>
        /// Gets the maximum bottom of the boxes inside the startBox
        /// </summary>
        /// <param name="startBox"></param>
        /// <param name="currentMaxBottom"></param>
        /// <returns></returns>
        internal float GetMaximumBottom(CssBox startBox, float currentMaxBottom)
        {
            try
            {
                float maxc2 = currentMaxBottom;

                //foreach (CssLineBox hostline in startBox.GetHostLineIter())
                //{
                //    PartialBoxStrip r = hostline.GetStrip(this);
                //    if (r != null)
                //    {
                //        maxc2 = Math.Max(maxc2, r.Bottom);
                //    }
                //}

                currentMaxBottom = Math.Max(currentMaxBottom, startBox.SummaryBound.Bottom);

                //if (maxc2 != currentMaxBottom)
                //{ 
                //} 

                foreach (var b in startBox.Boxes)
                {
                    currentMaxBottom = Math.Max(currentMaxBottom, GetMaximumBottom(b, currentMaxBottom));
                }

                return currentMaxBottom;
            }
            catch (Exception e)
            {

            }
            return 0;
        }



        /// <summary>
        /// Gets the rectangles where inline box will be drawn. See Remarks for more info.
        /// </summary>
        /// <returns>Rectangles where content should be placed</returns>
        /// <remarks>
        /// Inline boxes can be split across different LineBoxes, that's why this method
        /// Delivers a rectangle for each LineBox related to this box, if inline.
        /// </remarks>

        /// <summary>
        /// Inherits inheritable values from parent.
        /// </summary>
        internal new void InheritStyles(CssBoxBase box, bool clone = false)
        {
            base.InheritStyles(box, clone);
        }
      
        /// <summary>
        /// Gets the result of collapsing the vertical margins of the two boxes
        /// </summary>
        /// <param name="prevSibling">the previous box under the same parent</param>
        /// <returns>Resulting top margin</returns>
        protected float MarginTopCollapse(CssBoxBase prevSibling)
        {
            float value;
            if (prevSibling != null)
            {
                value = Math.Max(prevSibling.ActualMarginBottom, ActualMarginTop);
                CollapsedMarginTop = value;
            }
            else if (_parentBox != null && ActualPaddingTop < 0.1 && ActualPaddingBottom < 0.1 && _parentBox.ActualPaddingTop < 0.1 && _parentBox.ActualPaddingBottom < 0.1)
            {
                value = Math.Max(0, ActualMarginTop - Math.Max(_parentBox.ActualMarginTop, _parentBox.CollapsedMarginTop));
            }
            else
            {
                value = ActualMarginTop;
            }

            // fix for hr tag
            // if (value < 0.1 && HtmlTag != null && HtmlTag.Name == "hr")
            if (value < 0.1 && this.WellknownTagName == WellknownHtmlTagName.HR)
            {
                value = GetEmHeight() * 1.1f;
            }

            return value;
        }

        /// <summary>
        /// Calculate the actual right of the box by the actual right of the child boxes if this box actual right is not set.
        /// </summary>
        /// <returns>the calculated actual right value</returns>
        private float CalculateActualRight()
        {
            if (ActualRight > 90999)
            {
                var maxRight = 0f;
                foreach (var box in Boxes)
                {
                    maxRight = Math.Max(maxRight, box.ActualRight + box.ActualMarginRight);
                }
                return maxRight + ActualPaddingRight + ActualMarginRight + ActualBorderRightWidth;
            }
            else
            {
                return ActualRight;
            }
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
        /// <returns>Resulting bottom margin</returns>
        private float MarginBottomCollapse()
        {

            float margin = 0;
            //if (ParentBox != null && ParentBox.Boxes.IndexOf(this) == ParentBox.Boxes.Count - 1 && _parentBox.ActualMarginBottom < 0.1)
            if (ParentBox != null && this.IsLastChild && _parentBox.ActualMarginBottom < 0.1)
            {
                var lastChildBottomMargin = _boxes[_boxes.Count - 1].ActualMarginBottom;
                margin = (Height.IsAuto) ? Math.Max(ActualMarginBottom, lastChildBottomMargin) : lastChildBottomMargin;
            }
            return Math.Max(ActualBottom, _boxes[_boxes.Count - 1].ActualBottom + margin + ActualPaddingBottom + ActualBorderBottomWidth);
        }

        /// <summary>
        /// Deeply offsets the top of the box and its contents
        /// </summary>
        /// <param name="amount"></param>
        internal void OffsetTop(float amount)
        {
            if (amount == 0)
            {
                return;
            }
            //offset all runs
            if (this.HasRuns)
            {
                foreach (CssRun word in Runs)
                {
                    word.Top += amount;
                }
            }

            foreach (var hostline in this.GetMyHostLineIter())
            {
                //update all strip in host line
                hostline.OffsetTopStrip(this, amount);
            }

            //offset all boxes
            foreach (CssBox b in Boxes)
            {
                b.OffsetTop(amount);
            }

            if (_listItemBox != null)
            {
                _listItemBox.OffsetTop(amount);
            }

            //Location = new PointF(Location.X, Location.Y + amount);
            this.LocationY += amount;
        }
        /// <summary>
        /// Paints the background of the box
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="rect">the bounding rectangle to draw in</param>
        /// <param name="isFirst">is it the first rectangle of the element</param>
        /// <param name="isLast">is it the last rectangle of the element</param>
        internal void PaintBackground(IGraphics g, RectangleF rect, bool isFirst, bool isLast)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Brush brush = null;
                bool dispose = false;
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
                    if (IsRounded)
                    {
                        roundrect = RenderUtils.GetRoundRect(rect, ActualCornerNW, ActualCornerNE, ActualCornerSE, ActualCornerSW);
                    }

                    if (HtmlContainer != null && !HtmlContainer.AvoidGeometryAntialias && IsRounded)
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

                if (_imageLoadHandler != null && _imageLoadHandler.Image != null && isFirst)
                {
                    BackgroundImageDrawHandler.DrawBackgroundImage(g, this, _imageLoadHandler, rect);
                }
            }
        }


#if DEBUG
        internal void dbugPaintTextWordArea(IGraphics g, PointF offset, CssRun word)
        {
            //g.DrawRectangle(Pens.Blue, word.Left, word.Top, word.Width, word.Height);

        }
#endif


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

        internal void ResetSummaryBound()
        {
            this.SummaryBound = RectangleF.Empty;
        }

        /// <summary>
        /// On image load process complete with image request refresh for it to be painted.
        /// </summary>
        /// <param name="image">the image loaded or null if failed</param>
        /// <param name="rectangle">the source rectangle to draw in the image (empty - draw everything)</param>
        /// <param name="async">is the callback was called async to load image call</param>
        private void OnImageLoadComplete(Image image, Rectangle rectangle, bool async)
        {
            if (image != null && async)
                HtmlContainer.RequestRefresh(false);
        }

        /// <summary>
        /// Get brush for the text depending if there is selected text color set.
        /// </summary>
        protected Color GetSelectionForeColor()
        {
            return HtmlContainer.SelectionForeColor != System.Drawing.Color.Empty ? HtmlContainer.SelectionForeColor : ActualColor;
        }

        /// <summary>
        /// Get brush for selection background depending if it has external and if alpha is required for images.
        /// </summary>
        /// <param name="forceAlpha">used for images so they will have alpha effect</param>
        protected Brush GetSelectionBackBrush(bool forceAlpha)
        {
            var backColor = HtmlContainer.SelectionBackColor;
            if (backColor != System.Drawing.Color.Empty)
            {
                if (forceAlpha && backColor.A > 180)
                    return RenderUtils.GetSolidBrush(System.Drawing.Color.FromArgb(180, backColor.R, backColor.G, backColor.B));
                else
                    return RenderUtils.GetSolidBrush(backColor);
            }
            else
            {
                return CssUtils.DefaultSelectionBackcolor;
            }
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var tag = HtmlTag != null ? string.Format("<{0}>", HtmlTag.Name) : "anon";

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
    }
}