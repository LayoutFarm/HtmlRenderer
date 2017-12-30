//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using PixelFarm.Drawing;
using LayoutFarm.Css;

namespace LayoutFarm.HtmlBoxes
{
    partial class CssBox
    {
        /// <summary>
        /// the html tag that is associated with this css box, null if anonymous box
        /// </summary> 
        int _boxCompactFlags;
        //html rowspan: for td,th 
        int _rowSpan;
        int _colSpan;
        //----------------------------------
        float _localX;
        float _localY;
        //location, size 
        float _visualWidth;
        float _visualHeight;
        //----------------------------------
        //absolute layer width,height
        float _innerContentW;
        float _innerContentH;
        //----------------------------------
        int _viewportX;
        int _viewportY;
        //TODO: review here again!
        bool specificUserContentSizeWidth;
        float _cssBoxWidth;
        float _cssBoxHeight;
        CssBoxSizing _boxSizing;
        /// <summary>
        /// user's expected height
        /// </summary>
        float _expectedHight;
        /// <summary>
        /// user's expected width 
        /// </summary>
        float _expectedWidth;
        float _actualPaddingTop;
        float _actualPaddingBottom;
        float _actualPaddingRight;
        float _actualPaddingLeft;
        float _actualMarginTop;
        float _actualMarginBottom;
        float _actualMarginRight;
        float _actualMarginLeft;
        float _actualBorderTopWidth;
        float _actualBorderLeftWidth;
        float _actualBorderBottomWidth;
        float _actualBorderRightWidth;
        //corner
        float _actualCornerNW;
        float _actualCornerNE;
        float _actualCornerSW;
        float _actualCornerSE;
        //------------------------------
        int _lastCalculationEpisodeNum = 0;
        float _cachedMinimumWidth = 0;
        //------------------------------

        /// <summary>
        /// local visual X
        /// </summary>
        public float LocalX
        {
            get { return this._localX; }
        }
        /// <summary>
        /// local visual y 
        /// </summary>
        public float LocalY
        {
            get { return this._localY; }
        }
        public float LocalVisualRight
        {
            //from parent view
            get { return this.LocalX + this.VisualWidth; }
        }
        public float LocalVisualBottom
        {
            //from parent view 
            get { return this.LocalY + this.VisualHeight; }
        }
        //--------------------------------
        /// <summary>
        /// set location relative to container box
        /// </summary>
        /// <param name="localX"></param>
        /// <param name="localY"></param>
        public void SetLocation(float localX, float localY)
        {
            this._localX = localX;
            this._localY = localY;
            this._boxCompactFlags |= BoxFlags.HAS_ASSIGNED_LOCATION;
        }

        //=============================================================
        /// <summary>
        /// recalculate margin
        /// </summary>
        /// <param name="margin">marin side</param>
        /// <param name="cbWidth">width of containging block</param>
        /// <returns></returns>
        float RecalculateMargin(CssLength margin, float cbWidth)
        {
            //www.w3.org/TR/CSS2/box.html#margin-properties
            if (margin.IsAuto)
            {
                return 0;
            }
            return CssValueParser.ConvertToPx(margin, cbWidth, this);
        }
        /// <summary>
        /// recalculate padding
        /// </summary>
        /// <param name="padding"></param>
        /// <param name="cbWidth"></param>
        /// <returns></returns>
        float RecalculatePadding(CssLength padding, float cbWidth)
        {
            //www.w3.org/TR/CSS2/box.html#padding-properties

            if (padding.IsAuto)
            {
                return 0;
            }

            return CssValueParser.ConvertToPx(padding, cbWidth, this);
        }

        //=============================================================
        public static void InvalidateComputeValue(CssBox box)
        {
            //box values need to recompute value again 
            box._boxCompactFlags &= ~BoxFlags.LAY_EVAL_COMPUTE_VALUES;
        }
        internal bool NeedComputedValueEvaluation
        {
            get { return (this._boxCompactFlags & BoxFlags.LAY_EVAL_COMPUTE_VALUES) == 0; }
        }

        public void ReEvaluateFont(ITextService iFonts, float parentFontSize)
        {
            RequestFont fontInfo = this._myspec.GetFont(parentFontSize);

            this._resolvedFont = fontInfo;


            if (_myspec.WordSpacing.IsNormalWordSpacing)
            {
                this._actualWordSpacing = iFonts.MeasureWhitespace(_resolvedFont);
            }
            else
            {
                this._actualWordSpacing = iFonts.MeasureWhitespace(_resolvedFont)
                    + CssValueParser.ConvertToPx(_myspec.WordSpacing, 1, this);
            }
        }

        /// <summary>
        /// evaluate computed value
        /// </summary>
        internal void ReEvaluateComputedValues(ITextService iFonts, CssBox containingBlock)
        {

            //depend on parent
            //1. fonts 
            if (this.ParentBox != null)
            {
                ReEvaluateFont(iFonts, this.ParentBox.ResolvedFont.SizeInPixels);
                //2. actual word spacing
                //this._actualWordSpacing = this.NoEms(this.InitSpec.LineHeight);
                //3. font size 
                //len = len.ConvertEmToPoints(parentBox.ActualFont.SizeInPoints);
            }
            else
            {
                ReEvaluateFont(iFonts, containingBlock.ResolvedFont.SizeInPixels);
                //this._actualFont = this.Spec.GetFont(containingBlock.Spec);
            }

            //-----------------------------------------------------------------------
            float cbWidth = containingBlock.VisualWidth;
            int tmpBoxCompactFlags = this._boxCompactFlags;
            this._boxCompactFlags |= BoxFlags.LAY_EVAL_COMPUTE_VALUES;
            //www.w3.org/TR/CSS2/box.html#margin-properties
            //w3c: margin applies to all elements except elements table display type
            //other than table-caption,table and inline table
            var cssDisplay = this.CssDisplay;
            BoxSpec spec = _myspec;
            switch (cssDisplay)
            {
                case CssDisplay.None:
                    {
                        return;
                    }
                case CssDisplay.TableCell:
                case CssDisplay.TableColumn:
                case CssDisplay.TableColumnGroup:
                case CssDisplay.TableFooterGroup:
                case CssDisplay.TableHeaderGroup:
                case CssDisplay.TableRow:
                case CssDisplay.TableRowGroup:
                    {
                        //no margin

                    }
                    break;
                default:
                    {
                        //if (this.__aa_dbugId == 5)
                        //{
                        //    int a = spec.__aa_dbugId;

                        //}

                        this._actualMarginLeft = RecalculateMargin(spec.MarginLeft, cbWidth);
                        this._actualMarginTop = RecalculateMargin(spec.MarginTop, cbWidth);
                        this._actualMarginRight = RecalculateMargin(spec.MarginRight, cbWidth);
                        this._actualMarginBottom = RecalculateMargin(spec.MarginBottom, cbWidth);
                    }
                    break;
            }
            //www.w3.org/TR/CSS2/box.html#padding-properties
            switch (cssDisplay)
            {
                case CssDisplay.TableRowGroup:
                case CssDisplay.TableHeaderGroup:
                case CssDisplay.TableFooterGroup:
                case CssDisplay.TableRow:
                case CssDisplay.TableColumnGroup:
                case CssDisplay.TableColumn:
                    {
                        //no padding
                    }
                    break;
                default:
                    {
                        //-----------------------------------------------------------------------
                        //padding
                        this._actualPaddingLeft = RecalculatePadding(spec.PaddingLeft, cbWidth);
                        this._actualPaddingTop = RecalculatePadding(spec.PaddingTop, cbWidth);
                        this._actualPaddingRight = RecalculatePadding(spec.PaddingRight, cbWidth);
                        this._actualPaddingBottom = RecalculatePadding(spec.PaddingBottom, cbWidth);
                    }
                    break;
            }

            //-----------------------------------------------------------------------
            //borders         
            float a1, a2, a3, a4;
            this._actualBorderLeftWidth = a1 = (spec.BorderLeftStyle == CssBorderStyle.None) ? 0 : CssValueParser.GetActualBorderWidth(spec.BorderLeftWidth, this);
            this._actualBorderTopWidth = a2 = (spec.BorderTopStyle == CssBorderStyle.None) ? 0 : CssValueParser.GetActualBorderWidth(spec.BorderTopWidth, this);
            this._actualBorderRightWidth = a3 = (spec.BorderRightStyle == CssBorderStyle.None) ? 0 : CssValueParser.GetActualBorderWidth(spec.BorderRightWidth, this);
            this._actualBorderBottomWidth = a4 = (spec.BorderBottomStyle == CssBorderStyle.None) ? 0 : CssValueParser.GetActualBorderWidth(spec.BorderBottomWidth, this);
            //---------------------------------------------------------------------------

            this._borderLeftVisible = a1 > 0 && spec.BorderLeftStyle >= CssBorderStyle.Visible;
            this._borderTopVisible = a2 > 0 && spec.BorderTopStyle >= CssBorderStyle.Visible;
            this._borderRightVisible = a3 > 0 && spec.BorderRightStyle >= CssBorderStyle.Visible;
            this._borderBottomVisble = a4 > 0 && spec.BorderBottomStyle >= CssBorderStyle.Visible;
            //extension ***
            if (a1 + a2 + a3 + a4 > 0)
            {
                //css 2.1 border can't be nagative values 

                tmpBoxCompactFlags |= BoxFlags.HAS_SOME_VISIBLE_BORDER;
            }
            else
            {
                tmpBoxCompactFlags &= ~BoxFlags.HAS_SOME_VISIBLE_BORDER;
            }
            //---------------------------------------------------------------------------

            this._actualCornerNE = a1 = CssValueParser.ConvertToPx(spec.CornerNERadius, 0, this);
            this._actualCornerNW = a2 = CssValueParser.ConvertToPx(spec.CornerNWRadius, 0, this);
            this._actualCornerSE = a3 = CssValueParser.ConvertToPx(spec.CornerSERadius, 0, this);
            this._actualCornerSW = a4 = CssValueParser.ConvertToPx(spec.CornerSWRadius, 0, this);
            if ((a1 + a2 + a3 + a4) > 0)
            {
                //evaluate 
                tmpBoxCompactFlags |= BoxFlags.HAS_ROUND_CORNER;
            }
            else
            {
                tmpBoxCompactFlags &= ~BoxFlags.HAS_ROUND_CORNER;
            }
            //---------------------------------------------------------------------------
            //evaluate bg 

            if (BackgroundGradient != Color.Transparent ||
                RenderUtils.IsColorVisible(ActualBackgroundColor))
            {
                tmpBoxCompactFlags |= BoxFlags.HAS_VISIBLE_BG;
            }
            else
            {
                tmpBoxCompactFlags &= ~BoxFlags.HAS_VISIBLE_BG;
            }




            if (spec.WordSpacing.IsNormalWordSpacing)
            {
                this._actualWordSpacing = iFonts.MeasureWhitespace(_resolvedFont);
            }
            else
            {
                this._actualWordSpacing = iFonts.MeasureWhitespace(_resolvedFont)
                    + CssValueParser.ConvertToPx(spec.WordSpacing, 1, this);
            }
            //---------------------------------------------- 
            this._boxCompactFlags = tmpBoxCompactFlags;
            //---------------------------------------------- 

            //text indent   
            this._actualTextIndent = CssValueParser.ConvertToPx(spec.TextIndent, containingBlock.VisualWidth, this);
            this._actualBorderSpacingHorizontal = spec.BorderSpacingHorizontal.Number;
            this._actualBorderSpacingVertical = spec.BorderSpacingVertical.Number;
            //-----------------------
            //this._actualLineHeight = 0.9f * CssValueParser.ConvertToPx(LineHeight, this.GetEmHeight(), this); 
            //expected width expected height
            //this._expectedWidth = CssValueParser.ParseLength(Width, cbWidth, this);
            //this._expectedHight = CssValueParser.ParseLength(Height, containingBlock.SizeHeight, this);
            ////---------------------------------------------- 


            //www.w3.org/TR/CSS2/visudet.html#line-height
            //line height,
            //percent value of line height :
            // is refer to font size of the element itself
            //if (this.LineHeight.Number > 0)
            //{
            //    _actualLineHeight = .9f * CssValueParser.ConvertToPx(LineHeight, this.GetEmHeight(), this);
            //}
            //else
            //{
            //    _actualLineHeight = .9f * (this.GetEmHeight());
            //} 


            if (this._myspec.HasBoxShadow)
            {
                //temp fix here
                //TODO: review move shadow to external decoration object/box
                if (decorator == null)
                {
                    decorator = new CssBoxDecorator();
                }
                decorator.HBoxShadowOffset = (int)CssValueParser.ConvertToPx(spec.BoxShadowHOffset, 0, this);
                decorator.VBoxShadowOffset = (int)CssValueParser.ConvertToPx(spec.BoxShadowVOffset, 0, this);
                decorator.Color = spec.BoxShadowColor;
            }
            else
            {
                this.decorator = null;
            }
        }

        //------------------------------------------ 
        internal bool FreezeWidth
        {
            //temporary fix table cell width problem
            get { return (this._boxCompactFlags & BoxFlags.LAY_WIDTH_FREEZE) != 0; }
            set
            {
                if (value)
                {
                    this._boxCompactFlags |= BoxFlags.LAY_WIDTH_FREEZE;
                }
                else
                {
                    this._boxCompactFlags &= ~BoxFlags.LAY_WIDTH_FREEZE;
                }
            }
        }

#if DEBUG
        void dbugBeforeSetWidth(float width)
        {
            //Console.WriteLine(this.__aa_dbugId + " :" + width);
            //if (this.__aa_dbugId == 3)
            //{
            //}
        }
        void dbugBeforeSetHeight(float height)
        {
        }
#endif
        /// <summary>
        /// set box width related to its boxsizing model  and recalcualte visual width 
        /// </summary>
        /// <param name="width"></param>
        internal void SetCssBoxWidth(float width)
        {
            //TODO: review here again 
            //depend on box-sizing model  ***

#if DEBUG
            dbugBeforeSetWidth(width);
#endif
            this._cssBoxWidth = width;
            this._visualWidth = width;
            // must be separate because the margin can be calculated by percentage of the width
            //(again, because actual padding or margin may need css box with first)
            if (_boxSizing == CssBoxSizing.ContentBox)
            {
                this._visualWidth = width +
                       this.ActualPaddingLeft + this.ActualPaddingRight +
                       +this.ActualBorderLeftWidth + this.ActualBorderRightWidth;
            }

            this.specificUserContentSizeWidth = true;
        }
        internal void SetCssBoxFromContainerAvailableWidth(float containerClientWidth)
        {
#if DEBUG
            dbugBeforeSetWidth(containerClientWidth);
#endif
            this._visualWidth = containerClientWidth;
            this._cssBoxWidth = containerClientWidth - (
                       this.ActualPaddingLeft + this.ActualPaddingRight +
                       +this.ActualBorderLeftWidth + this.ActualBorderRightWidth);
        }
        internal void SetCssBoxHeight(float height)
        {
            //TODO: review here again 
            //depend on box-sizing model  ***

#if DEBUG
            dbugBeforeSetHeight(height);
#endif

            this._cssBoxHeight = height;
            this._visualHeight = height;
            // must be separate because the margin can be calculated by percentage of the width
            //(again, because actual padding or margin may need css box with first)
            if (_boxSizing == CssBoxSizing.ContentBox)
            {
                this._visualHeight = height +
                       this.ActualPaddingTop + this.ActualPaddingBottom +
                       +this.ActualBorderTopWidth + this.ActualBorderBottomWidth;
            }

            this.specificUserContentSizeWidth = true;
        }
        public void SetVisualWidth(float width)
        {
#if DEBUG
            dbugBeforeSetWidth(width);
#endif
            if (!this.FreezeWidth)
            {
                this._visualWidth = width;
            }
            else
            {
                // throw new NotSupportedException();
            }
        }
        public void SetVisualHeight(float height)
        {
#if DEBUG
            dbugBeforeSetHeight(height);
#endif
            this._visualHeight = height;
        }
        /// <summary>
        /// set presentation (visual) size width,height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetVisualSize(float width, float height)
        {
#if DEBUG
            dbugBeforeSetWidth(width);
#endif
#if DEBUG
            dbugBeforeSetHeight(height);
#endif
            if (!this.FreezeWidth)
            {
                this._visualWidth = width;
            }
            else
            {
                throw new NotSupportedException();
            }

            this._visualHeight = height;
        }
        /// <summary>
        /// presentation width (border+ padding+ content), for clip area, not include margin
        /// </summary>
        public float VisualWidth
        {
            get
            {
                return this._visualWidth;
            }
        }
        /// <summary>
        /// presentaion height (border+padding+ content), for clip area,not include margin
        /// </summary>
        public float VisualHeight
        {
            get
            {
                return this._visualHeight;
            }
        }
        //-------------------------------------------------------
        /// <summary>
        /// Gets the actual height
        /// </summary>
        public float ExpectedHeight
        {
            get
            {
                return this._expectedHight;
            }
        }
        /// <summary>
        /// Gets the actual width 
        /// </summary>
        public float ExpectedWidth
        {
            get
            {
                return this._expectedWidth;
            }
        }
        internal bool HasClipArea
        {
            get;
            private set;
        }
        public void SetExpectedSize(float expectedW, float expectedH)
        {
            this.HasClipArea = true;
            this._expectedWidth = expectedW;
            this._expectedHight = expectedH;
        }

        //---------------------------------------------------------
        internal static void ValidateComputeValues(CssBox box)
        {
            box._boxCompactFlags |= BoxFlags.LAY_EVAL_COMPUTE_VALUES;
        }

        /// <summary>
        /// Gets the actual top's padding
        /// </summary>
        public float ActualPaddingTop
        {
            get
            {
#if DEBUG
                //if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                //{
                //    //if not evaluate
                //    System.Diagnostics.Debugger.Break();
                //}
                //if (this._actualPaddingBottom > 10)
                //{
                //}
#endif
                return this._actualPaddingTop;
            }
        }

        /// <summary>
        /// Gets the actual padding on the left
        /// </summary>
        public float ActualPaddingLeft
        {
            get
            {
#if DEBUG
                //if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                //{
                //    //if not evaluate
                //    System.Diagnostics.Debugger.Break();
                //}
                //if (this._actualPaddingLeft > 10)
                //{

                //}
#endif
                return this._actualPaddingLeft;
            }
        }
        /// <summary>
        /// Gets the actual padding on the right
        /// </summary>
        public float ActualPaddingRight
        {
            get
            {
#if DEBUG
                //if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                //{
                //    //if not evaluate
                //    System.Diagnostics.Debugger.Break();
                //}
                //if (this._actualPaddingLeft > 10)
                //{ 
                //}
#endif
                return this._actualPaddingRight;
                //return _actualPaddingRight;
            }
        }
        /// <summary>
        /// Gets the actual Padding of the bottom
        /// </summary>
        public float ActualPaddingBottom
        {
            get
            {
#if DEBUG
                //if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                //{
                //    //if not evaluate
                //    System.Diagnostics.Debugger.Break();
                //}
                //if (this._actualPaddingBottom > 10)
                //{
                //}
#endif
                return this._actualPaddingBottom;
            }
        }

        //============================================================
        /// <summary>
        /// Gets the actual top's Margin
        /// </summary>
        public float ActualMarginTop
        {
            get
            {
#if DEBUG
                //if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                //{
                //    //if not evaluate
                //    System.Diagnostics.Debugger.Break();
                //}
#endif
                return this._actualMarginTop;
            }
        }


        /// <summary>
        /// Gets the actual Margin on the left
        /// </summary>
        public float ActualMarginLeft
        {
            get
            {
#if DEBUG
                //if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                //{
                //    //if not evaluate
                //    System.Diagnostics.Debugger.Break();
                //}
#endif
                return this._actualMarginLeft;
            }
        }

        /// <summary>
        /// Gets the actual Margin of the bottom
        /// </summary>
        internal float ActualMarginBottom
        {
            get
            {
                return this._actualMarginBottom;
            }
        }

        /// <summary>
        /// Gets the actual Margin on the right
        /// </summary>
        internal float ActualMarginRight
        {
            get
            {
                return this._actualMarginRight;
            }
        }
        //====================================================
        /// <summary>
        /// Gets the actual top border width
        /// </summary>
        internal float ActualBorderTopWidth
        {
            get
            {
#if DEBUG
                //if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                //{
                //    //if not evaluate
                //    System.Diagnostics.Debugger.Break();
                //}
#endif

                return _actualBorderTopWidth;
            }
        }

        /// <summary>
        /// Gets the actual Left border width
        /// </summary>
        internal float ActualBorderLeftWidth
        {
            get
            {
                return _actualBorderLeftWidth;
            }
        }

        /// <summary>
        /// Gets the actual Bottom border width
        /// </summary>
        internal float ActualBorderBottomWidth
        {
            get
            {
                return _actualBorderBottomWidth;
            }
        }

        /// <summary>
        /// Gets the actual Right border width
        /// </summary>
        internal float ActualBorderRightWidth
        {
            get
            {
                return _actualBorderRightWidth;
            }
        }
        //=======================================
        /// <summary>
        /// Gets the actual lenght of the north west corner
        /// </summary>
        internal float ActualCornerNW
        {
            get
            {
                return _actualCornerNW;
            }
        }

        /// <summary>
        /// Gets the actual lenght of the north east corner
        /// </summary>
        internal float ActualCornerNE
        {
            get
            {
                return _actualCornerNE;
            }
        }

        /// <summary>
        /// Gets the actual lenght of the south east corner
        /// </summary>
        internal float ActualCornerSE
        {
            get
            {
                return _actualCornerSE;
            }
        }

        /// <summary>
        /// Gets the actual lenght of the south west corner
        /// </summary>
        internal float ActualCornerSW
        {
            get
            {
                return _actualCornerSW;
            }
        }

        /// <summary>
        /// Gets a value indicating if at least one of the corners of the box is rounded
        /// </summary>
        internal bool HasSomeRoundCorner
        {
            get
            {
                return (this._boxCompactFlags & BoxFlags.HAS_ROUND_CORNER) != 0;
            }
        }
        internal bool HasVisibleBgColor
        {
            get
            {
                return (this._boxCompactFlags & BoxFlags.HAS_VISIBLE_BG) != 0;
            }
        }

        internal bool HasSomeVisibleBorder
        {
            get
            {
                return (this._boxCompactFlags & BoxFlags.HAS_SOME_VISIBLE_BORDER) != 0;
            }
        }


        protected bool RunSizeMeasurePass
        {
            get
            {
                return (this._boxCompactFlags & BoxFlags.LAY_RUNSIZE_MEASURE) != 0;
            }
            set
            {
                if (value)
                {
                    this._boxCompactFlags |= BoxFlags.LAY_RUNSIZE_MEASURE;
                }
                else
                {
                    this._boxCompactFlags &= ~BoxFlags.LAY_RUNSIZE_MEASURE;
                }
            }
        }
        public bool IsPointInArea(float x, float y)
        {
            //from parent view
            return x >= this.LocalX && x < this.LocalVisualRight &&
                   y >= this.LocalY && y < this.LocalVisualBottom;
        }


        /// <remarks>
        /// Flag that indicates that CssTable algorithm already made fixes on it.
        /// </remarks>
        internal bool IsTableFixed
        {
            get
            {
                return (this._boxCompactFlags & BoxFlags.LAY_TABLE_FIXED) != 0;
            }
            set
            {
                if (value)
                {
                    this._boxCompactFlags |= BoxFlags.LAY_TABLE_FIXED;
                }
                else
                {
                    this._boxCompactFlags &= ~BoxFlags.LAY_TABLE_FIXED;
                }
            }
        }

        protected virtual CssBox GetGlobalLocationImpl(out float globalX, out float globalY)
        {
            globalX = this._localX;
            globalY = this._localY;
            CssBox foundRoot = null;
            if (this.ParentBox != null)
            {
                float p_left, p_top;
                foundRoot = this.ParentBox.GetGlobalLocation(out p_left, out p_top);
                globalX += p_left;
                globalY += p_top;
            }
            return foundRoot;
        }
        internal CssBox GetGlobalLocationRelativeToRoot(out float globalX, out float globalY)
        {
            if (this.justBlockRun != null)
            {
                var hostLineOfThisRun = this.justBlockRun.HostLine;
                var hostBox = hostLineOfThisRun.OwnerBox;
                CssBox foundRoot = hostBox.GetGlobalLocationRelativeToRoot(out globalX, out globalY);
                globalX += this.justBlockRun.Left;
                globalY += this.justBlockRun.Top + hostLineOfThisRun.CachedLineTop;
                return foundRoot;
            }
            else
            {
                globalX = this._localX;
                globalY = this._localY;
                CssBox foundRoot = null;
                if (this.ParentBox != null)
                {
                    float p_left, p_top;
                    foundRoot = this.ParentBox.GetGlobalLocationRelativeToRoot(out p_left, out p_top);
                    globalX += p_left;
                    globalY += p_top;
                }
                return foundRoot;
            }
        }
        public CssBox GetGlobalLocation(out float globalX, out float globalY)
        {
            return this.GetGlobalLocationImpl(out globalX, out globalY);
        }

        /// <summary>
        /// inner content width
        /// </summary>
        public float InnerContentWidth
        {
            get { return this._innerContentW; }
            internal set
            {
                this._innerContentW = value;
            }
        }
        /// <summary>
        /// inner content height
        /// </summary>
        public float InnerContentHeight
        {
            get { return this._innerContentH; }
            internal set
            {
                this._innerContentH = value;
            }
        }

        //-----------
        //if this is custom box then must implement these methods
        public virtual void CustomRecomputedValue(CssBox containingBlock)
        {
            throw new NotImplementedException();
        }

        public virtual bool CustomContentHitTest(float x, float y, CssBoxHitChain hitChain)
        {
            throw new NotImplementedException();
        }


        public int ViewportX { get { return this._viewportX; } }
        public int ViewportY { get { return this._viewportY; } }
        public void SetViewport(int viewportX, int viewportY)
        {
            this._viewportX = viewportX;
            this._viewportY = viewportY;
            this.mayHasViewport = true;
            this.InvalidateGraphics();
        }
    }
}
