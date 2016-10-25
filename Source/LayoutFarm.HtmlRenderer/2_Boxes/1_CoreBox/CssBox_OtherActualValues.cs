//BSD, 2014-2016, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using PixelFarm.Drawing;
using LayoutFarm.Css;
namespace LayoutFarm.HtmlBoxes
{
    partial class CssBox
    {
        RequestFont _resolvedFont;
        float _actualLineHeight;
        float _actualWordSpacing; //assign for whitespace run ?
        float _actualTextIndent;
        float _actualEmHeight;
        float _actualBorderSpacingHorizontal;
        float _actualBorderSpacingVertical;
        CssDisplayOutside _displayOutside;
        CssDisplayInside _displayInside;
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
                //TODO review here
                return this._cssDisplay;
            }
        }
        public CssDisplayOutside CssDisplayOutside
        {
            get { return this._displayOutside; }
        }
        public CssDisplayInside CssDisplayInside
        {
            get { return this._displayInside; }
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
                    }
                    break;
                case CssSide.Top:
                    {
                        this._actualBorderTopWidth = w;
                    }
                    break;
                case CssSide.Right:
                    {
                        this._actualBorderRightWidth = w;
                    }
                    break;
                case CssSide.Bottom:
                    {
                        this._actualBorderBottomWidth = w;
                    }
                    break;
            }
        }

        static void TransplateDisplayOutsideInside(CssDisplay cssDisplay, out CssDisplayOutside outside, out CssDisplayInside inside)
        {
            //Short display 	Full display 	Generated box
            //none 	〃 	                        subtree omitted from box tree
            //contents 	〃 	                    element replaced by contents in box tree

            //block 	    'block flow' 	    block-level block container aka block box
            //flow-root 	'block flow-root' 	block-level block container that establishes a new block formatting context (BFC)
            //inline 	    'inline flow' 	    inline box
            //inline-block 	'inline flow-root' 	inline-level block container
            //run-in 	    'run-in flow' 	    run-in box (inline box with special box-tree-munging rules)
            //list-item 	 'list-item block flow' 	block box with additional marker box
            //inline list-item 	'list-item inline flow' 	inline box with additional marker box
            //flex 	         'block flex'       block-level flex container
            //inline-flex 	 'inline flex' 	inline-level flex container
            //grid 	         'block grid' 	block-level grid container
            //inline-grid 	 'inline grid' 	inline-level grid container
            //ruby 	         'inline ruby' 	inline-level ruby container
            //block ruby 	 'block ruby' 	block box containing ruby container
            //table 	     'block table' 	block-level table wrapper box containing table box
            //inline-table 	 'inline table' 	inline-level table wrapper box containing table box
            //table-cell 	 'table-cell flow' 	table cell block container
            //table-caption  'table-caption flow' 	table cell block container
            //ruby-base 	 'ruby-base flow' 	layout-specific internal box
            //ruby-text 	  'ruby-text flow' 	layout-specific internal box
            //other <display-internal> 	〃 	layout-specific internal box
            switch (cssDisplay)
            {
                default:
                    throw new NotSupportedException();
                case CssDisplay.TableColumn:
                case CssDisplay.TableColumnGroup:
                case CssDisplay.TableRow:
                case CssDisplay.TableRowGroup:
                case CssDisplay.TableHeaderGroup:
                case CssDisplay.TableFooterGroup:
                case CssDisplay.None:
                    outside = CssDisplayOutside.Internal;
                    inside = CssDisplayInside.Internal;
                    break;
                //outside -> inline
                case CssDisplay.Inline:
                    outside = CssDisplayOutside.Inline; //*
                    inside = CssDisplayInside.Flow;
                    break;
                case CssDisplay.InlineBlock:
                    outside = CssDisplayOutside.Inline; //*
                    inside = CssDisplayInside.FlowRoot;
                    break;
                case CssDisplay.InlineTable:
                    outside = CssDisplayOutside.Inline; //*
                    inside = CssDisplayInside.Table;
                    break;
                case CssDisplay.InlineFlex:
                    outside = CssDisplayOutside.Inline; //*
                    inside = CssDisplayInside.Flex;
                    break;
                //-------
                //outside -> block
                case CssDisplay.ListItem:
                    outside = CssDisplayOutside.Block;
                    inside = CssDisplayInside.Flow;
                    break;
                case CssDisplay.Flex:
                    outside = CssDisplayOutside.Block;
                    inside = CssDisplayInside.Flex;
                    break;
                case CssDisplay.Block:
                    outside = CssDisplayOutside.Block;
                    inside = CssDisplayInside.Flow;
                    break;
                case CssDisplay.Table:
                    outside = CssDisplayOutside.Block;
                    inside = CssDisplayInside.Table;
                    break;
                //-----------------
                //special
                case CssDisplay.TableCaption:
                    outside = CssDisplayOutside.TableCaption;
                    inside = CssDisplayInside.Flow;
                    break;
                case CssDisplay.TableCell:
                    outside = CssDisplayOutside.TableCell;
                    inside = CssDisplayInside.Flow;
                    break;
            }
        }
        public static void ChangeDisplayType(CssBox box, CssDisplay newdisplay)
        {
            TransplateDisplayOutsideInside(newdisplay, out box._displayOutside, out box._displayInside);
            if ((box._boxCompactFlags & BoxFlags.DONT_CHANGE_DISPLAY_TYPE) == 0)
            {
                box._cssDisplay = newdisplay;
            }


            //box.OutsideDisplayIsInline = ((newdisplay == CssDisplay.Inline ||
            //        newdisplay == CssDisplay.InlineBlock ||
            //        newdisplay == CssDisplay.InlineFlex)
            //        && !box.IsBrElement);
            box.OutsideDisplayIsInline = box._displayOutside == CssDisplayOutside.Inline && !box.IsBrElement;
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
                case CssDisplay.InlineBlock:

                case CssDisplay.ListItem:
                case CssDisplay.Table:
                case CssDisplay.InlineTable:
                case CssDisplay.TableCell:

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