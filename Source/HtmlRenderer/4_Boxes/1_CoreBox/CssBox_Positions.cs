//BSD 2014, WinterDev
//ArthurHub

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using HtmlRenderer.Css;


namespace HtmlRenderer.Boxes
{



    partial class CssBox
    {

        float _localX;
        float _localY;
        //location, size 
        float _sizeHeight;
        float _sizeWidth;
        //----------------------------------

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


        public float LocalX
        {
            get { return this._localX; }
        }
        public float LocalY
        {
            get { return this._localY; }
        }
        /// <summary>
        /// Gets the width available on the box, counting padding and margin.
        /// </summary> 
        //--------------------------------
        public float LocalRight
        {
            //from parent view
            get { return this.LocalX + this.SizeWidth; }
        }
        public float LocalBottom
        {
            //from parent view 
            get { return this.LocalY + this.SizeHeight; }
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

        public void ReEvaluateFont(HtmlRenderer.Drawing.IFonts iFonts, float parentFontSize)
        {
            HtmlRenderer.Drawing.FontInfo fontInfo = this._myspec.GetFont(iFonts, parentFontSize);
            this._actualFont = fontInfo.Font;
            this._actualLineHeight = fontInfo.LineHeight;
            this._actualEmHeight = fontInfo.LineHeight;
            if (_myspec.WordSpacing.IsNormalWordSpacing)
            {
                this._actualWordSpacing = iFonts.MeasureWhitespace(_actualFont);
            }
            else
            {
                this._actualWordSpacing = iFonts.MeasureWhitespace(_actualFont)
                    + CssValueParser.ConvertToPx(_myspec.WordSpacing, 1, this);
            }
        }
        public virtual void CustomRecomputedValue(CssBox containingBlock)
        {


        }
        /// <summary>
        /// evaluate computed value
        /// </summary>
        internal void ReEvaluateComputedValues(HtmlRenderer.Drawing.IFonts iFonts, CssBox containingBlock)
        {
            //see www.w3.org/TR/CSS2/box.html#padding-properties 
            //depend on parent
            //1. fonts 
            if (this.ParentBox != null)
            {
                ReEvaluateFont(iFonts, this.ParentBox.ActualFont.Size);
                //2. actual word spacing
                //this._actualWordSpacing = this.NoEms(this.InitSpec.LineHeight);
                //3. font size 
                //len = len.ConvertEmToPoints(parentBox.ActualFont.SizeInPoints);
            }
            else
            {
                throw new NotSupportedException();
                //this._actualFont = this.Spec.GetFont(containingBlock.Spec);
            }

            //-----------------------------------------------------------------------
            float cbWidth = containingBlock.SizeWidth;
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

                    } break;
                default:
                    {

                        this._actualMarginLeft = RecalculateMargin(spec.MarginLeft, cbWidth);
                        this._actualMarginTop = RecalculateMargin(spec.MarginTop, cbWidth);
                        this._actualMarginRight = RecalculateMargin(spec.MarginRight, cbWidth);
                        this._actualMarginBottom = RecalculateMargin(spec.MarginBottom, cbWidth);

                    } break;
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
                    } break;
                default:
                    {
                        //-----------------------------------------------------------------------
                        //padding
                        this._actualPaddingLeft = RecalculatePadding(spec.PaddingLeft, cbWidth);
                        this._actualPaddingTop = RecalculatePadding(spec.PaddingTop, cbWidth);
                        this._actualPaddingRight = RecalculatePadding(spec.PaddingRight, cbWidth);
                        this._actualPaddingBottom = RecalculatePadding(spec.PaddingBottom, cbWidth);
                    } break;
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

                this._boxCompactFlags |= BoxFlags.HAS_SOME_VISIBLE_BORDER;
            }
            else
            {
                this._boxCompactFlags &= ~BoxFlags.HAS_SOME_VISIBLE_BORDER;
            }
            //---------------------------------------------------------------------------

            this._actualCornerNE = a1 = CssValueParser.ConvertToPx(spec.CornerNERadius, 0, this);
            this._actualCornerNW = a2 = CssValueParser.ConvertToPx(spec.CornerNWRadius, 0, this);
            this._actualCornerSE = a3 = CssValueParser.ConvertToPx(spec.CornerSERadius, 0, this);
            this._actualCornerSW = a4 = CssValueParser.ConvertToPx(spec.CornerSWRadius, 0, this);

            if ((a1 + a2 + a3 + a4) > 0)
            {
                //evaluate 
                this._boxCompactFlags |= BoxFlags.HAS_ROUND_CORNER;
            }
            else
            {
                this._boxCompactFlags &= ~BoxFlags.HAS_ROUND_CORNER;
            }
            //---------------------------------------------------------------------------
            //evaluate bg 

            if (BackgroundGradient != System.Drawing.Color.Transparent ||
                Drawing.RenderUtils.IsColorVisible(ActualBackgroundColor))
            {
                this._boxCompactFlags |= BoxFlags.HAS_VISIBLE_BG;
            }
            else
            {
                this._boxCompactFlags &= ~BoxFlags.HAS_VISIBLE_BG;
            }


            if (spec.WordSpacing.IsNormalWordSpacing)
            {
                this._actualWordSpacing = iFonts.MeasureWhitespace(_actualFont);
            }
            else
            {
                this._actualWordSpacing = iFonts.MeasureWhitespace(_actualFont)
                    + CssValueParser.ConvertToPx(spec.WordSpacing, 1, this);
            }


            //text indent   

            this._actualTextIndent = CssValueParser.ConvertToPx(spec.TextIndent, containingBlock.SizeWidth, this);
            this._actualBorderSpacingHorizontal = spec.BorderSpacingHorizontal.Number;
            this._actualBorderSpacingVertical = spec.BorderSpacingVertical.Number;

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

        }

        //--------------------------------
        public float ClientLeft
        {
            get { return ActualBorderLeftWidth + ActualPaddingLeft; }
        }
        public float ClientRight
        {
            get { return this.SizeWidth - ActualPaddingRight - ActualBorderRightWidth; }
        }
        //--------------------------------
        public float ClientTop
        {
            get { return ActualBorderTopWidth + ActualPaddingTop; }
        }
        public float ClientBottom
        {
            get { return this.SizeHeight - (ActualPaddingBottom + ActualBorderBottomWidth); }
        }
        //------------------------------------------
        public float ClientWidth
        {
            get { return this.SizeWidth - (ActualBorderLeftWidth + ActualPaddingLeft + ActualPaddingRight + ActualBorderRightWidth); }
        }
        public float ClientHeight
        {
            get { return this.SizeHeight - (ActualBorderTopWidth + ActualPaddingTop + ActualPaddingBottom + ActualBorderBottomWidth); }
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

        public void SetSize(float width, float height)
        {
#if DEBUG

#endif
            if (!this.FreezeWidth)
            {
                this._sizeWidth = width;
            }

            this._sizeHeight = height;
        }
        public void SetHeight(float height)
        {

            this._sizeHeight = height;
        }
        public void SetWidth(float width)
        {
#if DEBUG

#endif

            if (!this.FreezeWidth)
            {
                this._sizeWidth = width;
            }

        }
        public float SizeWidth
        {
            get
            {
                return this._sizeWidth;
            }
        }
        public float SizeHeight
        {
            get
            {
                return this._sizeHeight;
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
#if DEBUG
                //if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                //{
                //    //if not evaluate
                //    System.Diagnostics.Debugger.Break();
                //}
#endif
                return this._expectedHight;
            }
        }

        /// <summary>
        /// Gets the actual height
        /// </summary>
        public float ExpectedWidth
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

                return this._expectedWidth;
            }
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
    }
}
