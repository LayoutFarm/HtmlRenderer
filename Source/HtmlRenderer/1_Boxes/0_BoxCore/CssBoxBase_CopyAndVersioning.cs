//BSD, 2014, WinterCore

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    partial class CssBox
    {

        /// <summary>
        /// Inherits inheritable values from specified box.
        /// </summary>
        /// <param name="s">source </param>
        /// <param name="clone">clone all </param>
        void InternalInheritStyles(CssBox s, bool clone)
        {
            if (s == null)
            {
                return;
            }


            //---------------------------------------
            this._fontProps = s._fontProps;
            this._listProps = s._listProps;
            //--------------------------------------- 
            this._lineHeight = s._lineHeight;
            this._textIndent = s._textIndent;
            this._actualColor = s._actualColor;
            this._emptyCells = s._emptyCells;
            //--------------------------------------- 
            this._textAlign = s._textAlign;
            this.VerticalAlign = s._verticalAlign;
            this._visibility = s._visibility;
            this._whitespace = s._whitespace;
            this._wordBreak = s._wordBreak;
            this._cssDirection = s._cssDirection;
            //--------------------------------------- 

            if (clone)
            {
                //for clone only (eg. split a box into two parts)
                //---------------------------------------
                this._backgroundProps = s._backgroundProps;

                this._borderProps = s._borderProps;
                this._cornerProps = s._cornerProps;
                //---------------------------------------

                this._left = s._left;
                this._top = s._top;
                this._bottom = s._bottom;
                this._right = s._right;

                this._width = s._width;
                this._height = s._height;
                this._maxWidth = s._maxWidth;
                this._position = s._position;


                this._wordSpacing = s._wordSpacing;
                this._lineHeight = s._lineHeight;
                this._float = s._float;


                this._cssDisplay = s._cssDisplay;
                this._overflow = s._overflow;
                this._textDecoration = s._textDecoration;
                //--------------------------------------- 


            }

        }

#if DEBUG

        public static bool Compare(dbugPropCheckReport dbugR, CssBox boxBase, BoxSpecBase spec)
        {

            int dd = boxBase.dbugId;
            dbugR.Check("_fontProps", CssFontProp.dbugIsEq(dbugR, boxBase._fontProps, spec._fontProps));
            dbugR.Check("_listProps", CssListProp.dbugIsEq(dbugR, boxBase._listProps, spec._listProps));
            dbugR.Check("_lineHeight", CssLength.IsEq(boxBase._lineHeight, spec._lineHeight));
            dbugR.Check("_textIndent", CssLength.IsEq(boxBase._textIndent, spec._textIndent));
            dbugR.Check("_actualColor", boxBase._actualColor == spec._actualColor);
            dbugR.Check("_emptyCells", boxBase._emptyCells == spec._emptyCells);
            dbugR.Check("_textAlign", boxBase._textAlign == spec._textAlign);

            dbugR.Check("_verticalAlign", boxBase._verticalAlign == spec._verticalAlign);
            dbugR.Check("_visibility", boxBase._visibility == spec._visibility);
            dbugR.Check("_whitespace", boxBase._whitespace == spec._whitespace);
            dbugR.Check("_wordBreak", boxBase._wordBreak == spec._wordBreak);
            dbugR.Check("_cssDirection", boxBase._cssDirection == spec._cssDirection);

            dbugR.Check("_backgroundProps", CssBackgroundProp.dbugIsEq(dbugR, boxBase._backgroundProps, spec._backgroundProps));
            dbugR.Check("_borderProps", CssBorderProp.dbugIsEq(dbugR, boxBase._borderProps, spec._borderProps));
            dbugR.Check("_cornerProps", CssCornerProp.dbugIsEq(dbugR, boxBase._cornerProps, spec._cornerProps));

            //---------------------------------------
            dbugR.Check("_left", CssLength.IsEq(boxBase._left, spec._left));
            dbugR.Check("_top", CssLength.IsEq(boxBase._top, spec._top));
            dbugR.Check("_bottom", CssLength.IsEq(boxBase._bottom, spec._bottom));
            dbugR.Check("_right", CssLength.IsEq(boxBase._right, spec._right));


            dbugR.Check("_width", CssLength.IsEq(boxBase._width, spec._width));
            dbugR.Check("_height", CssLength.IsEq(boxBase._height, spec._height));
            dbugR.Check("_maxWidth", CssLength.IsEq(boxBase._maxWidth, spec._maxWidth));


            dbugR.Check("_position", boxBase._position == spec._position);
            dbugR.Check("_wordSpacing", CssLength.IsEq(boxBase._wordSpacing, spec._wordSpacing));
            dbugR.Check("_float", boxBase._float == spec._float);
            dbugR.Check("_cssDisplay", boxBase._cssDisplay == spec._cssDisplay);
            dbugR.Check("_overflow", boxBase._overflow == spec._overflow);
            dbugR.Check("_textDecoration", boxBase._textDecoration == spec._textDecoration);


            if (dbugR.Count > 0)
            {
                return false;
            }
            return true;

        }
#endif

        void InternalInheritStyles(CssBox.BoxSpecBase s, bool clone)
        {
            if (s == null)
            {
                return;
            }


            //---------------------------------------
            this._fontProps = s._fontProps;
            this._listProps = s._listProps;
            //--------------------------------------- 
            this._lineHeight = s._lineHeight;
            this._textIndent = s._textIndent;
            this._actualColor = s._actualColor;
            this._emptyCells = s._emptyCells;
            //--------------------------------------- 
            this._textAlign = s._textAlign;
            this.VerticalAlign = s._verticalAlign;
            this._visibility = s._visibility;
            this._whitespace = s._whitespace;
            this._wordBreak = s._wordBreak;
            this._cssDirection = s._cssDirection;
            //--------------------------------------- 

            if (clone)
            {
                //for clone only (eg. split a box into two parts)
                //---------------------------------------
                this._backgroundProps = s._backgroundProps;

                this._borderProps = s._borderProps;
                this._cornerProps = s._cornerProps;
                //---------------------------------------

                this._left = s._left;
                this._top = s._top;
                this._bottom = s._bottom;
                this._right = s._right;

                this._width = s._width;
                this._height = s._height;
                this._maxWidth = s._maxWidth;
                this._position = s._position;


                this._wordSpacing = s._wordSpacing;
                this._lineHeight = s._lineHeight;
                this._float = s._float;


                this._cssDisplay = s._cssDisplay;
                this._overflow = s._overflow;
                this._textDecoration = s._textDecoration;
                //--------------------------------------- 


            }

        }

        /// <summary>
        /// clone all style from another box
        /// </summary>
        /// <param name="s"></param>
        internal void CloneAllStyles(CssBox s)
        {

            //1.
            //=====================================
            if (s._fontProps.Owner == s)
            {
                this._fontProps = s._fontProps;
            }

            this._listProps = s._listProps;
            //--------------------------------------- 
            this._lineHeight = s._lineHeight;
            this._textIndent = s._textIndent;
            this._actualColor = s._actualColor;
            this._emptyCells = s._emptyCells;
            //--------------------------------------- 
            this._textAlign = s._textAlign;
            this.VerticalAlign = s._verticalAlign;
            this._visibility = s._visibility;
            this._whitespace = s._whitespace;
            this._wordBreak = s._wordBreak;
            this._cssDirection = s._cssDirection;
            //---------------------------------------
            //2.
            //for clone only (eg. split a box into two parts)
            //=======================================
            this._backgroundProps = s._backgroundProps;

            //if (this.dbugId == 36)
            //{
            //}

            if (s._borderProps.Owner == s)
            {
                this._borderProps = s._borderProps;
            }
            this._borderProps = s._borderProps;


            this._cornerProps = s._cornerProps;
            //---------------------------------------

            this._left = s._left;
            this._top = s._top;
            this._bottom = s._bottom;
            this._right = s._right;

            this._width = s._width;
            this._height = s._height;
            this._maxWidth = s._maxWidth;
            this._position = s._position;

            this._wordSpacing = s._wordSpacing;
            this._lineHeight = s._lineHeight;
            this._float = s._float;

            //if (this.dbugId == 36)
            //{
            //}
            this._cssDisplay = s._cssDisplay;
            this._overflow = s._overflow;
            this._textDecoration = s._textDecoration;

            //3.
            //=====================================
            //if (this.dbugBB > 0)
            //{

            //}
            this._marginProps = s._marginProps;
            //--------------------------------------

            this._cssDirection = s._cssDirection;


            //-----------------------------------
            if (this._paddingProps.Owner != this)
            {
                this._paddingProps = s._paddingProps;
            }
            else
            {
                //this._prop_wait_eval |= (CssBoxAssignments.PADDING_LEFT |
                //                         CssBoxAssignments.PADDING_TOP |
                //                         CssBoxAssignments.PADDING_RIGHT |
                //                         CssBoxAssignments.PADDING_BOTTOM);
            }
            //-----------------------------------
        }

        /// <summary>
        /// clone all style from another box
        /// </summary>
        /// <param name="s"></param>
        internal void CloneAllStyles(CssBox.BoxSpecBase s)
        {

            //1.
            //=====================================
            if (s._fontProps.Owner == s)
            {
                this._fontProps = s._fontProps;
            }

            this._listProps = s._listProps;
            //--------------------------------------- 
            this._lineHeight = s._lineHeight;
            this._textIndent = s._textIndent;
            this._actualColor = s._actualColor;
            this._emptyCells = s._emptyCells;
            //--------------------------------------- 
            this._textAlign = s._textAlign;
            this.VerticalAlign = s._verticalAlign;
            this._visibility = s._visibility;
            this._whitespace = s._whitespace;
            this._wordBreak = s._wordBreak;
            this._cssDirection = s._cssDirection;
            //---------------------------------------
            //2.
            //for clone only (eg. split a box into two parts)
            //=======================================
            this._backgroundProps = s._backgroundProps;

            //if (this.dbugId == 36)
            //{
            //}

            if (s._borderProps.Owner == s)
            {
                this._borderProps = s._borderProps;
            }
            this._borderProps = s._borderProps;


            this._cornerProps = s._cornerProps;
            //---------------------------------------

            this._left = s._left;
            this._top = s._top;
            this._bottom = s._bottom;
            this._right = s._right;

            this._width = s._width;
            this._height = s._height;
            this._maxWidth = s._maxWidth;
            this._position = s._position;

            this._wordSpacing = s._wordSpacing;
            this._lineHeight = s._lineHeight;
            this._float = s._float;

            //if (this.dbugId == 36)
            //{
            //}
            this._cssDisplay = s._cssDisplay;
            this._overflow = s._overflow;
            this._textDecoration = s._textDecoration;

            //3.
            //=====================================
            //if (this.dbugBB > 0)
            //{

            //}
            this._marginProps = s._marginProps;
            //--------------------------------------

            this._cssDirection = s._cssDirection;


            //-----------------------------------
            if (this._paddingProps.Owner != this)
            {
                this._paddingProps = s._paddingProps;
            }
            else
            {
                //this._prop_wait_eval |= (CssBoxAssignments.PADDING_LEFT |
                //                         CssBoxAssignments.PADDING_TOP |
                //                         CssBoxAssignments.PADDING_RIGHT |
                //                         CssBoxAssignments.PADDING_BOTTOM);
            }
            //-----------------------------------
        }


        protected int _prop_pass_eval;
        CssBorderProp CheckBorderVersion()
        {
            return this._borderProps = this._borderProps.GetMyOwnVersion(this);
        }
        CssMarginProp CheckMarginVersion()
        {
            return this._marginProps = this._marginProps.GetMyOwnVersion(this);
        }
        CssPaddingProp CheckPaddingVersion()
        {
            return this._paddingProps = this._paddingProps.GetMyOwnVersion(this);
        }
        CssCornerProp CheckCornerVersion()
        {
            return this._cornerProps = this._cornerProps.GetMyOwnVersion(this);
        }
        CssFontProp CheckFontVersion()
        {
            return this._fontProps = this._fontProps.GetMyOwnVersion(this);
        }
        CssListProp CheckListPropVersion()
        {
            return this._listProps = this._listProps.GetMyOwnVersion(this);
        }
        CssBackgroundProp CheckBgVersion()
        {
            return this._backgroundProps = this._backgroundProps.GetMyOwnVersion(this);
        }


    }

}