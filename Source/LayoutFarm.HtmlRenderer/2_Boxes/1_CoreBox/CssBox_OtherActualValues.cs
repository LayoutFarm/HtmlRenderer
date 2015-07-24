// 2015,2014 ,BSD, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.Css;

namespace LayoutFarm.HtmlBoxes
{
    partial class CssBox
    {
        Font _actualFont;

        float _actualLineHeight;
        float _actualWordSpacing; //assign for whitespace run ?

        float _actualTextIndent;
        float _actualEmHeight;

        float _actualBorderSpacingHorizontal;
        float _actualBorderSpacingVertical;

        /// <summary>
        /// Gets the line height
        /// </summary>
        float ActualLineHeight
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
        float ActualWordSpacing
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

        public static void ChangeDisplayType(CssBox box, CssDisplay newdisplay)
        {
            if (box._cssDisplay == CssDisplay.Block &&  newdisplay == CssDisplay.Inline)
            {

            }
            if ((box._boxCompactFlags & BoxFlags.DONT_CHANGE_DISPLAY_TYPE) == 0)
            {
                box._cssDisplay = newdisplay;
            }


            box.IsInline = ((newdisplay == CssDisplay.Inline ||
                    newdisplay == CssDisplay.InlineBlock ||
                    newdisplay == CssDisplay.InlineFlex)
                    && !box.IsBrElement);
            //---------------------------

            box._isVisible = box._cssDisplay != CssDisplay.None && box._myspec.Visibility == CssVisibility.Visible;

            box._renderBGAndBorder = box._cssDisplay != Css.CssDisplay.Inline ||
                   box.Position == CssPosition.Absolute || //out of flow
                   box.Position == CssPosition.Fixed; //out of flow

            //-------------------------
            //check containing property 
            //-------------------------
            switch (newdisplay)
            {
                case CssDisplay.Block:
                case CssDisplay.ListItem:
                case CssDisplay.Table:
                case CssDisplay.TableCell:
                case CssDisplay.InlineBlock:
                case CssDisplay.InlineTable:
                case CssDisplay.Flex:
                case CssDisplay.InlineFlex:
                    box._boxCompactFlags |= BoxFlags.HAS_CONTAINER_PROP;
                    break;
                default:
                    //no container properties 
                    box._boxCompactFlags &= ~BoxFlags.HAS_CONTAINER_PROP;
                    break;
            }
            //-------------------------
        }
        public static void SetAsBrBox(CssBox box)
        {
            box._boxCompactFlags |= BoxFlags.IS_BR_ELEM;
        }
        protected static void SetAsCustomCssBox(CssBox box)
        {
            box._boxCompactFlags |= BoxFlags.IS_CUSTOM_CSSBOX;
        }

    }
}