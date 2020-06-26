//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using System;
using PixelFarm.Drawing;
using LayoutFarm.Css;
using LayoutFarm.WebDom.Parser;


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
        public float LocalX => _localX;
        /// <summary>
        /// local visual y 
        /// </summary>
        public float LocalY => _localY;
        public float LocalVisualRight => this.LocalX + this.VisualWidth;   //from parent view 
        public float LocalVisualBottom => this.LocalY + this.VisualHeight;   //from parent view 

        //--------------------------------
        /// <summary>
        /// set location relative to container box
        /// </summary>
        /// <param name="localX"></param>
        /// <param name="localY"></param>
        public void SetLocation(float localX, float localY)
        {
#if DEBUG
            //if (__aa_dbugId == 5)
            //{

            //    _localX = localX;
            //    _localY = localY;
            //    _boxCompactFlags |= BoxFlags.HAS_ASSIGNED_LOCATION;
            //    return;
            //}
#endif

            _localX = localX;
            _localY = localY;
            _boxCompactFlags |= BoxFlags.HAS_ASSIGNED_LOCATION;
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
            return CssLengthExt.ConvertToPx(margin, cbWidth, this);
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

            return CssLengthExt.ConvertToPx(padding, cbWidth, this);
        }

        //=============================================================
        public static void InvalidateComputeValue(CssBox box)
        {
            //box values need to recompute value again 
            box._boxCompactFlags &= ~BoxFlags.LAY_EVAL_COMPUTE_VALUES;
        }
        //
        internal bool NeedComputedValueEvaluation => (_boxCompactFlags & BoxFlags.LAY_EVAL_COMPUTE_VALUES) == 0;



        public void ReEvaluateFont(ITextService iFonts, float parentFontSize)
        {

            RequestFont fontInfo = _myspec.GetFont(parentFontSize);
            _resolvedFont = fontInfo;
            _resolvedFont1 = GlobalTextService.TextService2.ResolveFont(fontInfo);


            if (_myspec.WordSpacing.IsNormalWordSpacing)
            {
                //use normal spacing
                _actualWordSpacing = _resolvedFont1.WhitespaceWidth;//use pre-rounding (int) or exact scaled value ???
            }
            else
            {
                //TODO: review here,***
                //additional to original whitespacing or REPLACE with the new value
                _actualWordSpacing = _resolvedFont1.WhitespaceWidth +//use pre-rounding (int) or exact scaled value ???
                                          CssLengthExt.ConvertToPx(_myspec.WordSpacing, 1, this);
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
                if (this.ParentBox.ResolvedFont == null)
                {
                    //TODO: review this ... WHY?

                    //SIZE in point unit or pixel???
                    ReEvaluateFont(iFonts, containingBlock._resolvedFont1.SizeInPoints);
                }
                else
                {
                    ReEvaluateFont(iFonts, this.ParentBox._resolvedFont1.SizeInPoints);
                }


                //2. actual word spacing
                //_actualWordSpacing = this.NoEms(this.InitSpec.LineHeight);
                //3. font size 
                //len = len.ConvertEmToPoints(parentBox.ActualFont.SizeInPoints);
            }
            else
            {
                ReEvaluateFont(iFonts, containingBlock._resolvedFont1.SizeInPoints);
                //_actualFont = this.Spec.GetFont(containingBlock.Spec);
            }

            //-----------------------------------------------------------------------
            float cbWidth = containingBlock.VisualWidth;
            int tmpBoxCompactFlags = _boxCompactFlags;
            _boxCompactFlags |= BoxFlags.LAY_EVAL_COMPUTE_VALUES;
            //www.w3.org/TR/CSS2/box.html#margin-properties
            //w3c: margin applies to all elements except elements table display type
            //other than table-caption,table and inline table
            CssDisplay cssDisplay = this.CssDisplay;
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
                        //if (__aa_dbugId == 5)
                        //{
                        //    int a = spec.__aa_dbugId;

                        //}

                        _actualMarginLeft = RecalculateMargin(spec.MarginLeft, cbWidth);
                        _actualMarginTop = RecalculateMargin(spec.MarginTop, cbWidth);
                        _actualMarginRight = RecalculateMargin(spec.MarginRight, cbWidth);
                        _actualMarginBottom = RecalculateMargin(spec.MarginBottom, cbWidth);
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
                        _actualPaddingLeft = RecalculatePadding(spec.PaddingLeft, cbWidth);
                        _actualPaddingTop = RecalculatePadding(spec.PaddingTop, cbWidth);
                        _actualPaddingRight = RecalculatePadding(spec.PaddingRight, cbWidth);
                        _actualPaddingBottom = RecalculatePadding(spec.PaddingBottom, cbWidth);
                    }
                    break;
            }

            //-----------------------------------------------------------------------
            //borders         
            float a1, a2, a3, a4;
            _actualBorderLeftWidth = a1 = (spec.BorderLeftStyle == CssBorderStyle.None) ? 0 : CssLengthExt.GetActualBorderWidth(spec.BorderLeftWidth, this);
            _actualBorderTopWidth = a2 = (spec.BorderTopStyle == CssBorderStyle.None) ? 0 : CssLengthExt.GetActualBorderWidth(spec.BorderTopWidth, this);
            _actualBorderRightWidth = a3 = (spec.BorderRightStyle == CssBorderStyle.None) ? 0 : CssLengthExt.GetActualBorderWidth(spec.BorderRightWidth, this);
            _actualBorderBottomWidth = a4 = (spec.BorderBottomStyle == CssBorderStyle.None) ? 0 : CssLengthExt.GetActualBorderWidth(spec.BorderBottomWidth, this);
            //---------------------------------------------------------------------------

            _borderLeftVisible = a1 > 0 && spec.BorderLeftStyle >= CssBorderStyle.Visible;
            _borderTopVisible = a2 > 0 && spec.BorderTopStyle >= CssBorderStyle.Visible;
            _borderRightVisible = a3 > 0 && spec.BorderRightStyle >= CssBorderStyle.Visible;
            _borderBottomVisble = a4 > 0 && spec.BorderBottomStyle >= CssBorderStyle.Visible;
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

            _actualCornerNE = a1 = CssLengthExt.ConvertToPx(spec.CornerNERadius, 0, this);
            _actualCornerNW = a2 = CssLengthExt.ConvertToPx(spec.CornerNWRadius, 0, this);
            _actualCornerSE = a3 = CssLengthExt.ConvertToPx(spec.CornerSERadius, 0, this);
            _actualCornerSW = a4 = CssLengthExt.ConvertToPx(spec.CornerSWRadius, 0, this);
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
                (ActualBackgroundColor.A > 0))

            {
                tmpBoxCompactFlags |= BoxFlags.HAS_VISIBLE_BG;
            }
            else
            {
                tmpBoxCompactFlags &= ~BoxFlags.HAS_VISIBLE_BG;
            }




            if (spec.WordSpacing.IsNormalWordSpacing)
            {
                _actualWordSpacing = iFonts.MeasureWhitespace(_resolvedFont);
            }
            else
            {
                _actualWordSpacing = iFonts.MeasureWhitespace(_resolvedFont)
                    + CssLengthExt.ConvertToPx(spec.WordSpacing, 1, this);
            }
            //---------------------------------------------- 
            _boxCompactFlags = tmpBoxCompactFlags;
            //---------------------------------------------- 

            //text indent   
            _actualTextIndent = CssLengthExt.ConvertToPx(spec.TextIndent, containingBlock.VisualWidth, this);
            _actualBorderSpacingHorizontal = spec.BorderSpacingHorizontal.Number;
            _actualBorderSpacingVertical = spec.BorderSpacingVertical.Number;
            //-----------------------
            //_actualLineHeight = 0.9f * CssValueParser.ConvertToPx(LineHeight, this.GetEmHeight(), this); 
            //expected width expected height
            //_expectedWidth = CssValueParser.ParseLength(Width, cbWidth, this);
            //_expectedHight = CssValueParser.ParseLength(Height, containingBlock.SizeHeight, this);
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


            if (_myspec.HasBoxShadow)
            {
                //temp fix here
                //TODO: review move shadow to external decoration object/box
                if (_decorator == null)
                {
                    _decorator = new CssBoxDecorator();
                }
                _decorator.HBoxShadowOffset = (int)CssLengthExt.ConvertToPx(spec.BoxShadowHOffset, 0, this);
                _decorator.VBoxShadowOffset = (int)CssLengthExt.ConvertToPx(spec.BoxShadowVOffset, 0, this);
                _decorator.Color = spec.BoxShadowColor;
            }
            else
            {
                _decorator = null;
            }
        }

        //------------------------------------------ 
        internal bool FreezeWidth
        {
            //temporary fix table cell width problem
            get { return (_boxCompactFlags & BoxFlags.LAY_WIDTH_FREEZE) != 0; }
            set
            {
                if (value)
                {
                    _boxCompactFlags |= BoxFlags.LAY_WIDTH_FREEZE;
                }
                else
                {
                    _boxCompactFlags &= ~BoxFlags.LAY_WIDTH_FREEZE;
                }
            }
        }

#if DEBUG
        void dbugBeforeSetWidth(float width)
        {
            //if (width == 37)
            //{

            //}
            //Console.WriteLine(__aa_dbugId + " :" + width);
            //if (__aa_dbugId == 3)
            //{
            //}
        }
        void dbugBeforeSetHeight(float height)
        {
        }

        void dbugAferSetVisualWidth()
        {
            //if (_visualWidth == 37)
            //{ 
            //}
        }
        void dbugAfterSetCssWidth()
        {
            //if (_cssBoxWidth == 37)
            //{ 
            //}
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
            _cssBoxWidth = width;
#if DEBUG
            dbugAfterSetCssWidth();
#endif
            _visualWidth = width;
#if DEBUG             
            dbugAferSetVisualWidth();
#endif

            // must be separate because the margin can be calculated by percentage of the width
            //(again, because actual padding or margin may need css box with first)
            if (_boxSizing == CssBoxSizing.ContentBox)
            {
                _visualWidth = width +
                       this.ActualPaddingLeft + this.ActualPaddingRight +
                       +this.ActualBorderLeftWidth + this.ActualBorderRightWidth;
#if DEBUG
                dbugAferSetVisualWidth();
#endif
            }
            this.specificUserContentSizeWidth = true;


        }
        internal void SetCssBoxWidthLimitToContainerAvailableWidth(float containerClientWidth)
        {
#if DEBUG
            dbugBeforeSetWidth(containerClientWidth);
#endif
            _visualWidth = containerClientWidth;
#if DEBUG
            dbugAferSetVisualWidth();
#endif
            _cssBoxWidth = containerClientWidth - (
                       this.ActualPaddingLeft + this.ActualPaddingRight +
                       this.ActualBorderLeftWidth + this.ActualBorderRightWidth);//not include margin
#if DEBUG
            dbugAfterSetCssWidth();
#endif

        }
        internal void SetCssBoxHeight(float height)
        {
            //TODO: review here again 
            //depend on box-sizing model  ***

#if DEBUG
            dbugBeforeSetHeight(height);
#endif

            _cssBoxHeight = height;
            _visualHeight = height;
            // must be separate because the margin can be calculated by percentage of the width
            //(again, because actual padding or margin may need css box with first)
            if (_boxSizing == CssBoxSizing.ContentBox)
            {
                _visualHeight = height +
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
                _visualWidth = width;
#if DEBUG
                dbugAferSetVisualWidth();
#endif
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
            _visualHeight = height;
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
            dbugBeforeSetHeight(height);

#endif
            if (!this.FreezeWidth)
            {
                _visualWidth = width;
            }
            else
            {
                throw new NotSupportedException();
            }

            _visualHeight = height;
        }
        /// <summary>
        /// presentation width (border+ padding+ content), for clip area, not include margin
        /// </summary>
        public float VisualWidth => _visualWidth;
        /// <summary>
        /// presentaion height (border+padding+ content), for clip area,not include margin
        /// </summary>
        public float VisualHeight => _visualHeight;
        //-------------------------------------------------------
        /// <summary>
        /// Gets the actual height
        /// </summary>
        public float ExpectedHeight => _expectedHight;
        /// <summary>
        /// Gets the actual width 
        /// </summary>
        public float ExpectedWidth => _expectedWidth;
        internal bool HasClipArea
        {
            get;
            private set;
        }
        public void SetExpectedSize(float expectedW, float expectedH)
        {
            this.HasClipArea = true;
            _expectedWidth = expectedW;
            _expectedHight = expectedH;
        }

        //---------------------------------------------------------
        internal static void ValidateComputeValues(CssBox box)
        {
            box._boxCompactFlags |= BoxFlags.LAY_EVAL_COMPUTE_VALUES;
        }

        /// <summary>
        /// Gets the actual top's padding
        /// </summary>
        public float ActualPaddingTop => _actualPaddingTop;



        /// <summary>
        /// Gets the actual padding on the left
        /// </summary>
        public float ActualPaddingLeft => _actualPaddingLeft;


        /// <summary>
        /// Gets the actual padding on the right
        /// </summary>
        public float ActualPaddingRight => _actualPaddingRight;
        //return _actualPaddingRight;


        /// <summary>
        /// Gets the actual Padding of the bottom
        /// </summary>
        public float ActualPaddingBottom => _actualPaddingBottom;



        //============================================================
        /// <summary>
        /// Gets the actual top's Margin
        /// </summary>
        public float ActualMarginTop => _actualMarginTop;




        /// <summary>
        /// Gets the actual Margin on the left
        /// </summary>
        public float ActualMarginLeft => _actualMarginLeft;



        /// <summary>
        /// Gets the actual Margin of the bottom
        /// </summary>
        internal float ActualMarginBottom => _actualMarginBottom;



        /// <summary>
        /// Gets the actual Margin on the right
        /// </summary>
        internal float ActualMarginRight => _actualMarginRight;


        //====================================================
        /// <summary>
        /// Gets the actual top border width
        /// </summary>
        internal float ActualBorderTopWidth => _actualBorderTopWidth;



        /// <summary>
        /// Gets the actual Left border width
        /// </summary>
        internal float ActualBorderLeftWidth => _actualBorderLeftWidth;



        /// <summary>
        /// Gets the actual Bottom border width
        /// </summary>
        internal float ActualBorderBottomWidth => _actualBorderBottomWidth;



        /// <summary>
        /// Gets the actual Right border width
        /// </summary>
        internal float ActualBorderRightWidth => _actualBorderRightWidth;


        //=======================================
        /// <summary>
        /// Gets the actual lenght of the north west corner
        /// </summary>
        internal float ActualCornerNW => _actualCornerNW;



        /// <summary>
        /// Gets the actual lenght of the north east corner
        /// </summary>
        internal float ActualCornerNE => _actualCornerNE;



        /// <summary>
        /// Gets the actual lenght of the south east corner
        /// </summary>
        internal float ActualCornerSE => _actualCornerSE;



        /// <summary>
        /// Gets the actual lenght of the south west corner
        /// </summary>
        internal float ActualCornerSW => _actualCornerSW;



        /// <summary>
        /// Gets a value indicating if at least one of the corners of the box is rounded
        /// </summary>
        internal bool HasSomeRoundCorner => (_boxCompactFlags & BoxFlags.HAS_ROUND_CORNER) != 0;


        internal bool HasVisibleBgColor => (_boxCompactFlags & BoxFlags.HAS_VISIBLE_BG) != 0;



        internal bool HasSomeVisibleBorder => (_boxCompactFlags & BoxFlags.HAS_SOME_VISIBLE_BORDER) != 0;




        protected bool RunSizeMeasurePass
        {
            get
            {
                return (_boxCompactFlags & BoxFlags.LAY_RUNSIZE_MEASURE) != 0;
            }
            set
            {
                if (value)
                {
                    _boxCompactFlags |= BoxFlags.LAY_RUNSIZE_MEASURE;
                }
                else
                {
                    _boxCompactFlags &= ~BoxFlags.LAY_RUNSIZE_MEASURE;
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
                return (_boxCompactFlags & BoxFlags.LAY_TABLE_FIXED) != 0;
            }
            set
            {
                if (value)
                {
                    _boxCompactFlags |= BoxFlags.LAY_TABLE_FIXED;
                }
                else
                {
                    _boxCompactFlags &= ~BoxFlags.LAY_TABLE_FIXED;
                }
            }
        }

        protected virtual void GetGlobalLocationImpl(out float globalX, out float globalY)
        {
#if DEBUG
            if (_viewportX != 0 || _viewportY != 0)
            {

            }
#endif

            //TODO: review here again***
            globalX = _localX - _viewportX;
            globalY = _localY - _viewportY;

            //CssBox foundRoot = null;
            if (this.ParentBox != null)
            {
                float p_left, p_top;
                /* foundRoot = */
                this.ParentBox.GetGlobalLocation(out p_left, out p_top);
                globalX += p_left;
                globalY += p_top;
            }
            //return foundRoot;
        }
        public void GetGlobalLocation(out float globalX, out float globalY)
        {
            this.GetGlobalLocationImpl(out globalX, out globalY);
        }

        public void GetGlobalLocationRelativeToRoot(ref PointF location)
        {

            if (_justBlockRun != null)
            {
                //recursive
#if DEBUG
                if (_viewportX != 0 || _viewportY != 0)
                {

                }
#endif

                //location.Offset(
                //    (int)(_justBlockRun.Left),
                //    (int)(_justBlockRun.Top + _justBlockRun.HostLine.CachedLineTop));

                location = new PointF(
                   location.X + (int)(_justBlockRun.Left),
                   location.Y + (int)(_justBlockRun.Top + _justBlockRun.HostLine.CachedLineTop)
                   );

                //recursive
                _justBlockRun.HostLine.OwnerBox.GetGlobalLocationRelativeToRoot(ref location);
                return;//***
            }

            CssBox parentBox = _absLayerOwner ?? this.ParentBox;
            if (parentBox != null)
            {
#if DEBUG
                if (_viewportX != 0 || _viewportY != 0)
                {

                }
#endif
                //location.Offset((int)this.LocalX - _viewportX, (int)this.LocalY - _viewportY);

                location = new PointF(
                    location.X + (int)this.LocalX - _viewportX,
                    location.Y + (int)this.LocalY - _viewportY
                    );
                //recursive
                parentBox.GetGlobalLocationRelativeToRoot(ref location);
            }
        }
        public void GetGlobalLocationRelativeToRoot(out float globalX, out float globalY)
        {
            PointF location = new PointF(0, 0);
            GetGlobalLocationRelativeToRoot(ref location);
            globalX = location.X;
            globalY = location.Y;
        }
        /// <summary>
        /// inner content width
        /// </summary>
        public float InnerContentWidth
        {
            get => _innerContentW;
            internal set => _innerContentW = value;

        }
        /// <summary>
        /// inner content height
        /// </summary>
        public float InnerContentHeight
        {
            get => _innerContentH;
            internal set => _innerContentH = value;

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


        public int ViewportX => _viewportX;
        public int ViewportY => _viewportY;
        //
        public void SetViewport(int viewportX, int viewportY)
        {
            _viewportX = viewportX;
            _viewportY = viewportY;
            _mayHasViewport = true;
            this.InvalidateGraphics();
        }
    }
}
