using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using HtmlRenderer.Parse;


namespace HtmlRenderer.Dom
{
    partial class CssBox
    {

        BoxLayoutState layoutState;

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
        //--------------------------------

        /// <summary>
        /// evaluate actual margins and padding 
        /// </summary>
        internal void EvaluateActualMarginsAndPaddings()
        {
            //some margins and widths value are computed value
            //that need its containing block width,height 
            this.layoutState = BoxLayoutState.EvaluateMarginAndPadding;  
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
            //if (this.dbugMark > 0)
            //{
            //}
            //if (widthChangeCount > 0)
            //{
            //    if (this.SizeWidth != width)
            //    {

            //    }
            //}
            //widthChangeCount++;
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
            //if (widthChangeCount > 0)
            //{
            //    if (this.SizeWidth != width)
            //    {

            //    }
            //}
            //if (this.dbugMark > 0)
            //{
            //}
            //widthChangeCount++;
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.HEIGHT) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.HEIGHT;
                    return _expectedHight = CssValueParser.ParseLength(Height, this.SizeHeight, this);
                }
                return _expectedHight;
            }
        }

        /// <summary>
        /// Gets the actual height
        /// </summary>
        public float ExpectedWidth
        {
            get
            {
                if ((this._prop_pass_eval & CssBoxBaseAssignments.WIDTH) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.WIDTH;
                    return _expectedWidth = CssValueParser.ParseLength(Width, this.SizeWidth, this);
                }
                return _expectedWidth;
            }
        }
        //---------------------------------------------------------
        /// <summary>
        /// Gets the actual top's padding
        /// </summary>
        public float ActualPaddingTop
        {
            get
            {
                if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_TOP) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_TOP;
                    return _actualPaddingTop = CssValueParser.ParseLength(PaddingTop, this.SizeWidth, this);
                }
                return _actualPaddingTop;
            }
        }

        /// <summary>
        /// Gets the actual padding on the left
        /// </summary>
        public float ActualPaddingLeft
        {
            get
            {
                if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_LEFT) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_LEFT;
                    return this._actualPaddingLeft = CssValueParser.ParseLength(PaddingLeft, this.SizeWidth, this);
                }
                return _actualPaddingLeft;
            }
        }
        /// <summary>
        /// Gets the actual padding on the right
        /// </summary>
        public float ActualPaddingRight
        {
            get
            {
                if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_RIGHT) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_RIGHT;
                    return _actualPaddingRight = CssValueParser.ParseLength(PaddingRight, SizeWidth, this);
                }
                return _actualPaddingRight;
            }
        }
        /// <summary>
        /// Gets the actual Padding of the bottom
        /// </summary>
        public float ActualPaddingBottom
        {
            get
            {
                if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_BOTTOM) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_BOTTOM;
                    return _actualPaddingBottom = CssValueParser.ParseLength(PaddingBottom, this.SizeWidth, this);
                }
                return _actualPaddingBottom;
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_TOP) == 0)
                {
                    if (this.MarginTop.IsAuto)
                    {
                        //MarginTop = CssLength.ZeroPx;
                        this._prop_pass_eval = CssBoxBaseAssignments.MARGIN_TOP;
                        return this._actualMarginTop = 0;
                    }


                    var value = CssValueParser.ParseLength(MarginTop, this.SizeWidth, this);
                    if (this.MarginLeft.IsPercentage)
                    {
                        return value;
                    }
                    else
                    {
                        this._prop_pass_eval = CssBoxBaseAssignments.MARGIN_TOP;
                        return this._actualMarginTop = value;
                    }

                }
                return _actualMarginTop;

            }
        }

        /// <summary>
        /// Gets the actual Margin on the left
        /// </summary>
        public float ActualMarginLeft
        {
            get
            {

                if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_LEFT) == 0)
                {
                    if (MarginLeft.IsAuto)
                    {
                        //MarginLeft = CssLength.ZeroPx;
                        this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_LEFT;
                        return _actualMarginLeft = 0;
                    }

                    var value = CssValueParser.ParseLength(MarginLeft, this.SizeWidth, this);

                    if (this.MarginLeft.IsPercentage)
                    {
                        return value;
                    }

                    this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_LEFT;
                    return _actualMarginLeft = value;
                }
                return _actualMarginLeft;

            }
        }

        /// <summary>
        /// Gets the actual Margin of the bottom
        /// </summary>
        public float ActualMarginBottom
        {
            get
            {

                if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_BOTTOM) == 0)
                {
                    if (MarginBottom.IsAuto)
                    {
                        //MarginBottom = CssLength.ZeroPx;
                        this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_BOTTOM;
                        return this._actualMarginBottom = 0;
                    }
                    var value = CssValueParser.ParseLength(MarginBottom, this.SizeWidth, this);

                    //margin left?
                    if (MarginLeft.IsPercentage)
                    {
                        return value;
                    }

                    this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_BOTTOM;
                    return this._actualMarginBottom = value;
                }
                return _actualMarginBottom;
            }
        }

        /// <summary>
        /// Gets the actual Margin on the right
        /// </summary>
        public float ActualMarginRight
        {
            get
            {
                if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_RIGHT) == 0)
                {
                    if (MarginRight.IsAuto)
                    {
                        this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_RIGHT;
                        return this._actualMarginRight = 0;
                    }

                    var value = CssValueParser.ParseLength(MarginRight, this.SizeWidth, this);

                    //margin left ?
                    if (MarginLeft.IsPercentage)
                    {
                        return value;
                    }
                    this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_RIGHT;
                    return this._actualMarginRight = value;
                }
                return _actualMarginRight;
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_TOP) == 0)
                {
                    //need evaluate
                    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_TOP;
                    return (this.BorderTopStyle == CssBorderStyle.None) ?
                        _actualBorderTopWidth = 0f :
                        _actualBorderTopWidth = CssValueParser.GetActualBorderWidth(BorderTopWidth, this);
                }
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

                if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_LEFT) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_LEFT;
                    return (this.BorderLeftStyle == CssBorderStyle.None) ?
                        _actualBorderLeftWidth = 0f :
                        _actualBorderLeftWidth = CssValueParser.GetActualBorderWidth(BorderLeftWidth, this);
                }
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_BOTTOM) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_BOTTOM;
                    return (this.BorderBottomStyle == CssBorderStyle.None) ?
                        _actualBorderBottomWidth = 0f :
                        _actualBorderBottomWidth = CssValueParser.GetActualBorderWidth(BorderBottomWidth, this);

                }
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_RIGHT) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_RIGHT;
                    return (this.BorderRightStyle == CssBorderStyle.None) ?
                        _actualBorderRightWidth = 0f :
                        _actualBorderRightWidth = CssValueParser.GetActualBorderWidth(BorderRightWidth, this);

                }
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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_NW) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_NW;
                    return _actualCornerNW = CssValueParser.ParseLength(CornerNWRadius, 0, this);
                }
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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_NE) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_NE;
                    return _actualCornerNE = CssValueParser.ParseLength(CornerNERadius, 0, this);
                }
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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_SE) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_SE;
                    return _actualCornerSE = CssValueParser.ParseLength(CornerSERadius, 0, this);
                }
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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_SW) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_SW;
                    return _actualCornerSW = CssValueParser.ParseLength(CornerSWRadius, 0, this);
                }
                return _actualCornerSW;
            }
        }

        /// <summary>
        /// Gets a value indicating if at least one of the corners of the box is rounded
        /// </summary>
        public bool IsRounded
        {
            get { return ActualCornerNE > 0f || ActualCornerNW > 0f || ActualCornerSE > 0f || ActualCornerSW > 0f; }
        }
    }
}
