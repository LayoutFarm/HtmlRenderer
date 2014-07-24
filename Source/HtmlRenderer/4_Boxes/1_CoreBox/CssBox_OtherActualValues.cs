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

        /// <summary>
        /// Gets the actual horizontal border spacing for tables
        /// </summary>
        public float ActualBorderSpacingHorizontal
        {
            get
            {
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
        public static void ChangeDisplayType(CssBox box, CssDisplay newdisplay)
        {

            if ((box._boxCompactFlags & BoxFlags.FIXED_DISPLAY_TYPE) == 0)
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
                    box._boxCompactFlags |= BoxFlags.HAS_CONTAINER_PROP;
                    break;
                default:
                    //not container properties 
                    box._boxCompactFlags &= ~BoxFlags.HAS_CONTAINER_PROP;
                    break;
            }
            //-------------------------
        }
        public static void SetAsBrBox(CssBox box)
        {
            box._boxCompactFlags |= BoxFlags.IS_BR_ELEM;
        }
        public static void SetAsCustomCssBox(CssBox box)
        {
            box._boxCompactFlags |= BoxFlags.IS_CUSTOM_CSSBOX;
        }
    }
}