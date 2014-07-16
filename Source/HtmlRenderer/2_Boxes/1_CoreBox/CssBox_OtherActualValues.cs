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
        Font _actualFont;
        float _actualLineHeight;
        float _actualWordSpacing;
        float _actualTextIndent;
        float _actualEmHeight;

        float _actualBorderSpacingHorizontal;
        float _actualBorderSpacingVertical;

        /// <summary>
        /// Gets the line height
        /// </summary>
        public float ActualLineHeight
        {
            get
            {
                return _actualLineHeight;
            }
        }
        public CssDisplay CssDisplay
        {
            get
            {
                return this._cssDisplay;
            }
        }
        /// <summary>
        /// Gets the text indentation (on first line only)
        /// </summary>
        public float ActualTextIndent
        {
            get
            {
                return _actualTextIndent;
            }
        }

        /// <summary>
        /// Gets the actual width of whitespace between words.
        /// </summary>
        public float ActualWordSpacing
        {
            get { return _actualWordSpacing; }
        }
        protected float MeasureWordSpacing(LayoutVisitor lay)
        {
            if ((this._prop_pass_eval & CssBoxAssignments.WORD_SPACING) == 0)
            {
                this._prop_pass_eval |= CssBoxAssignments.WORD_SPACING;
                _actualWordSpacing = lay.MeasureWhiteSpace(this);
                if (!this.WordSpacing.IsNormalWordSpacing)
                {
                    _actualWordSpacing += CssValueParser.ConvertToPx(this.WordSpacing, 1, this);
                }
            }
            return this._actualWordSpacing;
        }
        /// <summary>
        /// Gets the actual horizontal border spacing for tables
        /// </summary>
        public float ActualBorderSpacingHorizontal
        {
            get
            {
                if ((this._prop_pass_eval & CssBoxAssignments.BORDER_SPACING_H) == 0)
                {
                    this._prop_pass_eval |= CssBoxAssignments.BORDER_SPACING_H;
                    _actualBorderSpacingHorizontal = this.BorderSpacingHorizontal.Number;
                }
                return _actualBorderSpacingHorizontal;
            }
        }

        /// <summary>
        /// Gets the actual vertical border spacing for tables
        /// </summary>
        public float ActualBorderSpacingVertical
        {
            get
            {
                if ((this._prop_pass_eval & CssBoxAssignments.BORDER_SPACING_V) == 0)
                {
                    this._prop_pass_eval |= CssBoxAssignments.BORDER_SPACING_V;
                    _actualBorderSpacingVertical = this.BorderSpacingVertical.Number;
                }

                return _actualBorderSpacingVertical;
            }
        }
        //========================================================================
        internal void DirectSetBorderWidth(CssSide side, float w)
        {
            switch (side)
            {
                case CssSide.Left:
                    {
                        this._actualBorderLeftWidth = w;
                    } break;
                case CssSide.Top:
                    {
                        this._actualBorderTopWidth = w;
                    } break;
                case CssSide.Right:
                    {
                        this._actualBorderRightWidth = w;
                    } break;
                case CssSide.Bottom:
                    {
                        this._actualBorderBottomWidth = w;
                    } break;
            }
        }
        internal void DirectSetBorderStyle(float leftWpx, float topWpx, float rightWpx, float bottomWpx)
        {
            this._actualBorderLeftWidth = leftWpx;
            this._actualBorderTopWidth = topWpx;
            this._actualBorderRightWidth = rightWpx;
            this._actualBorderBottomWidth = bottomWpx;
        }
        internal void DirectSetHeight(float px)
        {
            this._sizeHeight = px;
        }
        internal static void ChangeDisplayType(CssBox box, CssDisplay newdisplay)
        {
             

            if (!box._fixDisplayType)
            {
                box._cssDisplay = newdisplay;
            }

            box.IsInline = ((newdisplay == CssDisplay.Inline ||
                    newdisplay == CssDisplay.InlineBlock)
                    && !box.IsBrElement);
            //---------------------------
            box._isVisible = box._cssDisplay != CssDisplay.None && box._myspec.Visibility == CssVisibility.Visible;
            //-------------------------
            //check containing property 
            //-------------------------
            switch (newdisplay)
            {
                //case CssDisplay.BlockInsideInlineAfterCorrection:
                case CssDisplay.Block:
                case CssDisplay.ListItem:
                case CssDisplay.Table:
                case CssDisplay.TableCell:
                    box._boxCompactFlags |= CssBoxFlagsConst.HAS_CONTAINER_PROP;
                    break;
                default:
                    //not container properties 
                    box._boxCompactFlags &= ~CssBoxFlagsConst.HAS_CONTAINER_PROP;
                    break;
            }

            //-------------------------
        }
        internal static void SetAsBrBox(CssBox box)
        {
            box._isBrElement = true;
        }
    }
}