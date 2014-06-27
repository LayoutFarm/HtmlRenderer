using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using HtmlRenderer.Parse;


namespace HtmlRenderer.Dom
{
    partial class CssBox
    {



        float _localX;
        float _localY;
        //location, size 
        float _sizeHeight;
        float _sizeWidth;

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

        //todo : use flags
        bool _hasRoundCorner;

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
            this._boxCompactFlags |= CssBoxFlagsConst.HAS_ASSIGNED_LOCATION;
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
            return CssValueParser.ParseLength(margin, cbWidth, this);
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
            return CssValueParser.ParseLength(padding, cbWidth, this);
        }

        //=============================================================

        static int num_count = 0;
        /// <summary>
        /// evaluate computed value
        /// </summary>
        internal void EvaluateComputedValues(CssBox containingBlock)
        {
            //see www.w3.org/TR/CSS2/box.html#padding-properties

            //width of some margins,paddings are computed value 
            //that need its containing block width (even for 'top' and 'bottom')
            if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) != 0)
            {
                 
                num_count++;
                //Console.WriteLine(num_count + " " + this.dbugId.ToString());
                return;
            } 
            //else if (this.dbugId == 35)
            //{
            //}



            //margin
            //-----------------------------------------------------------------------
            float cbWidth = containingBlock.SizeWidth;

            //www.w3.org/TR/CSS2/box.html#margin-properties
            //w3c: margin applies to all elements except elements table display type
            //other than table-caption,table and inline table
            var cssDisplay = this.CssDisplay;
            switch (cssDisplay)
            {
                case Dom.CssDisplay.None:
                    {
                        return;
                    }
                case Dom.CssDisplay.TableCell:
                case Dom.CssDisplay.TableColumn:
                case Dom.CssDisplay.TableColumnGroup:
                case Dom.CssDisplay.TableFooterGroup:
                case Dom.CssDisplay.TableHeaderGroup:
                case Dom.CssDisplay.TableRow:
                case Dom.CssDisplay.TableRowGroup:
                    {
                        //no margin

                    } break;
                default:
                    {
                        this._actualMarginLeft = RecalculateMargin(this.MarginLeft, cbWidth);
                        this._actualMarginTop = RecalculateMargin(this.MarginTop, cbWidth);
                        this._actualMarginRight = RecalculateMargin(this.MarginRight, cbWidth);
                        this._actualMarginBottom = RecalculateMargin(this.MarginBottom, cbWidth);

                    } break;
            }
            //www.w3.org/TR/CSS2/box.html#padding-properties
            switch (cssDisplay)
            {
                case Dom.CssDisplay.TableRowGroup:
                case Dom.CssDisplay.TableHeaderGroup:
                case Dom.CssDisplay.TableFooterGroup:
                case Dom.CssDisplay.TableRow:
                case Dom.CssDisplay.TableColumnGroup:
                case Dom.CssDisplay.TableColumn:
                    {
                        //no padding
                    } break;
                default:
                    {
                        //-----------------------------------------------------------------------
                        //padding
                        this._actualPaddingLeft = RecalculatePadding(this.PaddingLeft, cbWidth);
                        this._actualPaddingTop = RecalculatePadding(this.PaddingTop, cbWidth);
                        this._actualPaddingRight = RecalculatePadding(this.PaddingRight, cbWidth);
                        this._actualPaddingBottom = RecalculatePadding(this.PaddingBottom, cbWidth);
                    } break; 
            }
          
            //-----------------------------------------------------------------------
            //borders            
            this._actualBorderLeftWidth = (this.BorderLeftStyle == CssBorderStyle.None) ? 0 : CssValueParser.GetActualBorderWidth(BorderLeftWidth, this);
            this._actualBorderTopWidth = (this.BorderTopStyle == CssBorderStyle.None) ? 0 : CssValueParser.GetActualBorderWidth(BorderTopWidth, this);
            this._actualBorderRightWidth = (this.BorderRightStyle == CssBorderStyle.None) ? 0 : CssValueParser.GetActualBorderWidth(BorderRightWidth, this);
            this._actualBorderBottomWidth = (this.BorderBottomStyle == CssBorderStyle.None) ? 0 : CssValueParser.GetActualBorderWidth(BorderBottomWidth, this);
            //---------------------------------------------------------------------------

            //extension ***
            float c1, c2, c3, c4;
            this._actualCornerNE = c1 = CssValueParser.ParseLength(CornerNERadius, 0, this);
            this._actualCornerNW = c2 = CssValueParser.ParseLength(CornerNWRadius, 0, this);
            this._actualCornerSE = c3 = CssValueParser.ParseLength(CornerSERadius, 0, this);
            this._actualCornerSW = c4 = CssValueParser.ParseLength(CornerSWRadius, 0, this);
            this._hasRoundCorner = (c1 + c2 + c3 + c4) > 0;
            //---------------------------------------------------------------------------

            //if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_BOTTOM) == 0)
            //{
            //    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_BOTTOM;
            //    return (this.BorderBottomStyle == CssBorderStyle.None) ?
            //        _actualBorderBottomWidth = 0f :
            //        _actualBorderBottomWidth = CssValueParser.GetActualBorderWidth(BorderBottomWidth, this);

            //}


            //expected width expected height
            //this._expectedWidth = CssValueParser.ParseLength(Width, cbWidth, this);
            //this._expectedHight = CssValueParser.ParseLength(Height, containingBlock.SizeHeight, this);
            ////---------------------------------------------- 


            //www.w3.org/TR/CSS2/visudet.html#line-height
            //line height,
            //percent value of line height :
            // is refer to font size of the element itself
            //_actualLineHeight = .9f * CssValueParser.ParseLength(LineHeight, this.GetEmHeight(), this);

            this._boxCompactFlags |= CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES;

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
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
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
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif

                return this._expectedWidth;
            }
        }
        //---------------------------------------------------------
        internal static void ValidateComputeValues(CssBox box)
        {
            box._boxCompactFlags |= CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES;
        }

        /// <summary>
        /// Gets the actual top's padding
        /// </summary>
        public float ActualPaddingTop
        {
            get
            {
#if DEBUG
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif
                return this._actualPaddingTop;
                //if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_TOP) != 0)
                //{
                //    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_TOP;
                //    return _actualPaddingTop = CssValueParser.ParseLength(PaddingTop, this.SizeWidth, this);
                //}
                //return _actualPaddingTop;
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
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif
                return this._actualPaddingLeft;
                //if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_LEFT) != 0)
                //{
                //    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_LEFT;
                //    return this._actualPaddingLeft = CssValueParser.ParseLength(PaddingLeft, this.SizeWidth, this);
                //}
                //return _actualPaddingLeft;
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
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }

#endif
                return this._actualPaddingRight;
                //if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_RIGHT) != 0)
                //{
                //    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_RIGHT;
                //    return _actualPaddingRight = CssValueParser.ParseLength(PaddingRight, SizeWidth, this);
                //}
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
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif
                return this._actualPaddingBottom;
                //if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_BOTTOM) != 0)
                //{
                //    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_BOTTOM;
                //    return _actualPaddingBottom = CssValueParser.ParseLength(PaddingBottom, this.SizeWidth, this);
                //}
                //return _actualPaddingBottom;
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
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif
                return this._actualMarginTop;

                //if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_TOP) == 0)
                //{
                //    if (this.MarginTop.IsAuto)
                //    {
                //        this._prop_pass_eval = CssBoxBaseAssignments.MARGIN_TOP;
                //        return this._actualMarginTop = 0;
                //    }
                //    var value = CssValueParser.ParseLength(MarginTop, this.SizeWidth, this);
                //    if (this.MarginLeft.IsPercentage)
                //    {
                //        return value;
                //    }
                //    else
                //    {
                //        this._prop_pass_eval = CssBoxBaseAssignments.MARGIN_TOP;
                //        return this._actualMarginTop = value;
                //    }

                //}
                //return _actualMarginTop;
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
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif
                return this._actualMarginLeft;

                //if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_LEFT) == 0)
                //{
                //    if (MarginLeft.IsAuto)
                //    {
                //        //MarginLeft = CssLength.ZeroPx;
                //        this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_LEFT;
                //        return _actualMarginLeft = 0;
                //    }

                //    var value = CssValueParser.ParseLength(MarginLeft, this.SizeWidth, this);

                //    if (this.MarginLeft.IsPercentage)
                //    {
                //        return value;
                //    }

                //    this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_LEFT;
                //    return _actualMarginLeft = value;
                //}
                //return _actualMarginLeft;

            }
        }

        /// <summary>
        /// Gets the actual Margin of the bottom
        /// </summary>
        public float ActualMarginBottom
        {
            get
            {

#if DEBUG
                if (this.CssDisplay == Dom.CssDisplay.TableRow)
                {
                    return 0;
                }
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif
                return this._actualMarginBottom;
                //if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_BOTTOM) == 0)
                //{
                //    if (MarginBottom.IsAuto)
                //    {
                //        //MarginBottom = CssLength.ZeroPx;
                //        this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_BOTTOM;
                //        return this._actualMarginBottom = 0;
                //    }
                //    var value = CssValueParser.ParseLength(MarginBottom, this.SizeWidth, this);

                //    //margin left?
                //    if (MarginLeft.IsPercentage)
                //    {
                //        return value;
                //    }

                //    this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_BOTTOM;
                //    return this._actualMarginBottom = value;
                //}
                //return _actualMarginBottom;
            }
        }

        /// <summary>
        /// Gets the actual Margin on the right
        /// </summary>
        public float ActualMarginRight
        {
            get
            {
#if DEBUG
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif
                return this._actualMarginRight;

                //if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_RIGHT) == 0)
                //{
                //    if (MarginRight.IsAuto)
                //    {
                //        this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_RIGHT;
                //        return this._actualMarginRight = 0;
                //    }

                //    var value = CssValueParser.ParseLength(MarginRight, this.SizeWidth, this);

                //    //margin left ?
                //    if (MarginLeft.IsPercentage)
                //    {
                //        return value;
                //    }
                //    this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_RIGHT;
                //    return this._actualMarginRight = value;
                //}
                //return _actualMarginRight;
            }
        }
        //====================================================
        /// <summary>
        /// Gets the actual top border width
        /// </summary>
        public float ActualBorderTopWidth
        {
            get
            {
#if DEBUG
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif
                //if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_TOP) == 0)
                //{
                //    //need evaluate
                //    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_TOP;
                //    return (this.BorderTopStyle == CssBorderStyle.None) ?
                //        _actualBorderTopWidth = 0f :
                //        _actualBorderTopWidth = CssValueParser.GetActualBorderWidth(BorderTopWidth, this);
                //}
                return _actualBorderTopWidth;
            }
        }

        /// <summary>
        /// Gets the actual Left border width
        /// </summary>
        public float ActualBorderLeftWidth
        {
            get
            {

#if DEBUG
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif
                //if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_LEFT) == 0)
                //{
                //    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_LEFT;
                //    return (this.BorderLeftStyle == CssBorderStyle.None) ?
                //        _actualBorderLeftWidth = 0f :
                //        _actualBorderLeftWidth = CssValueParser.GetActualBorderWidth(BorderLeftWidth, this);
                //}
                return _actualBorderLeftWidth;
            }
        }

        /// <summary>
        /// Gets the actual Bottom border width
        /// </summary>
        public float ActualBorderBottomWidth
        {
            get
            {
#if DEBUG
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif
                //if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_BOTTOM) == 0)
                //{
                //    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_BOTTOM;
                //    return (this.BorderBottomStyle == CssBorderStyle.None) ?
                //        _actualBorderBottomWidth = 0f :
                //        _actualBorderBottomWidth = CssValueParser.GetActualBorderWidth(BorderBottomWidth, this);

                //}
                return _actualBorderBottomWidth;
            }
        }

        /// <summary>
        /// Gets the actual Right border width
        /// </summary>
        public float ActualBorderRightWidth
        {
            get
            {
#if DEBUG
                if ((this._boxCompactFlags & CssBoxFlagsConst.LAY_EVAL_COMPUTE_VALUES) == 0)
                {
                    //if not evaluate
                    System.Diagnostics.Debugger.Break();
                }
#endif

                //if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_RIGHT) == 0)
                //{
                //    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_RIGHT;
                //    return (this.BorderRightStyle == CssBorderStyle.None) ?
                //        _actualBorderRightWidth = 0f :
                //        _actualBorderRightWidth = CssValueParser.GetActualBorderWidth(BorderRightWidth, this);
                //}
                return _actualBorderRightWidth;
            }
        }
        //=======================================
        /// <summary>
        /// Gets the actual lenght of the north west corner
        /// </summary>
        public float ActualCornerNW
        {
            get
            {
                //if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_NW) != 0)
                //{
                //    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_NW;
                //    return _actualCornerNW = CssValueParser.ParseLength(CornerNWRadius, 0, this);
                //}
                return _actualCornerNW;
            }
        }

        /// <summary>
        /// Gets the actual lenght of the north east corner
        /// </summary>
        public float ActualCornerNE
        {
            get
            {
                //if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_NE) != 0)
                //{
                //    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_NE;
                //    return _actualCornerNE = CssValueParser.ParseLength(CornerNERadius, 0, this);
                //}
                return _actualCornerNE;
            }
        }

        /// <summary>
        /// Gets the actual lenght of the south east corner
        /// </summary>
        public float ActualCornerSE
        {
            get
            {
                //if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_SE) != 0)
                //{
                //    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_SE;
                //    return _actualCornerSE = CssValueParser.ParseLength(CornerSERadius, 0, this);
                //}
                return _actualCornerSE;
            }
        }

        /// <summary>
        /// Gets the actual lenght of the south west corner
        /// </summary>
        public float ActualCornerSW
        {
            get
            {
                //if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_SW) != 0)
                //{
                //    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_SW;
                //    return _actualCornerSW = CssValueParser.ParseLength(CornerSWRadius, 0, this);
                //}
                return _actualCornerSW;
            }
        }

        /// <summary>
        /// Gets a value indicating if at least one of the corners of the box is rounded
        /// </summary>
        public bool IsRounded
        {
            get
            {
                return this._hasRoundCorner;
            }
        }

    }
}
