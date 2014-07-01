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
            _initSpec._fontProps = s._initSpec._fontProps;
            _initSpec._listProps = s._initSpec._listProps;
            //--------------------------------------- 
            _initSpec._lineHeight = s._initSpec._lineHeight;
            _initSpec._textIndent = s._initSpec._textIndent;
            _initSpec._actualColor = s._initSpec._actualColor;
            _initSpec._emptyCells = s._initSpec._emptyCells;
            //--------------------------------------- 
            _initSpec._textAlign = s._initSpec._textAlign;
            _initSpec.VerticalAlign = s._initSpec._verticalAlign;
            _initSpec._visibility = s._initSpec._visibility;
            _initSpec._whitespace = s._initSpec._whitespace;
            _initSpec._wordBreak = s._initSpec._wordBreak;
            _initSpec._cssDirection = s._initSpec._cssDirection;
            //--------------------------------------- 

            if (clone)
            {
                //for clone only (eg. split a box into two parts)
                //---------------------------------------
                _initSpec._backgroundProps = s._initSpec._backgroundProps;

                _initSpec._borderProps = s._initSpec._borderProps;
                _initSpec._cornerProps = s._initSpec._cornerProps;
                //---------------------------------------

                _initSpec._left = s._initSpec._left;
                _initSpec._top = s._initSpec._top;
                _initSpec._bottom = s._initSpec._bottom;
                _initSpec._right = s._initSpec._right;

                _initSpec._width = s._initSpec._width;
                _initSpec._height = s._initSpec._height;
                _initSpec._maxWidth = s._initSpec._maxWidth;
                _initSpec._position = s._initSpec._position;


                _initSpec._wordSpacing = s._initSpec._wordSpacing;
                _initSpec._lineHeight = s._initSpec._lineHeight;
                _initSpec._float = s._initSpec._float;


                _initSpec._cssDisplay = s._initSpec._cssDisplay;
                _initSpec._overflow = s._initSpec._overflow;
                _initSpec._textDecoration = s._initSpec._textDecoration;
                //--------------------------------------- 


            }

        }

#if DEBUG

        public static bool Compare(dbugPropCheckReport dbugR, BoxSpec boxBase, BoxSpec spec)
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

        void InternalInheritStyles(BoxSpec s, bool clone)
        {
            if (s == null)
            {
                return;
            }


            //---------------------------------------
            _initSpec._fontProps = s._fontProps;
            _initSpec._listProps = s._listProps;
            //--------------------------------------- 
            _initSpec._lineHeight = s._lineHeight;
            _initSpec._textIndent = s._textIndent;
            _initSpec._actualColor = s._actualColor;
            _initSpec._emptyCells = s._emptyCells;
            //--------------------------------------- 
            _initSpec._textAlign = s._textAlign;
            _initSpec.VerticalAlign = s._verticalAlign;
            _initSpec._visibility = s._visibility;
            _initSpec._whitespace = s._whitespace;
            _initSpec._wordBreak = s._wordBreak;
            _initSpec._cssDirection = s._cssDirection;
            //--------------------------------------- 

            if (clone)
            {
                //for clone only (eg. split a box into two parts)
                //---------------------------------------
                _initSpec._backgroundProps = s._backgroundProps;

                _initSpec._borderProps = s._borderProps;
                _initSpec._cornerProps = s._cornerProps;
                //---------------------------------------

                _initSpec._left = s._left;
                _initSpec._top = s._top;
                _initSpec._bottom = s._bottom;
                _initSpec._right = s._right;

                _initSpec._width = s._width;
                _initSpec._height = s._height;
                _initSpec._maxWidth = s._maxWidth;
                _initSpec._position = s._position;


                _initSpec._wordSpacing = s._wordSpacing;
                _initSpec._lineHeight = s._lineHeight;
                _initSpec._float = s._float;


                _initSpec._cssDisplay = s._cssDisplay;
                _initSpec._overflow = s._overflow;
                _initSpec._textDecoration = s._textDecoration;
                //--------------------------------------- 


            }

        }

        ///// <summary>
        ///// clone all style from another box
        ///// </summary>
        ///// <param name="s"></param>
        //internal void CloneAllStyles(BoxSpecBase s)
        //{

        //    //1.
        //    //=====================================
        //    if (s._initSpec._fontProps.Owner == s)
        //    {
        //        _initSpec._fontProps = s._fontProps;
        //    }

        //    _initSpec._listProps = s._listProps;
        //    //--------------------------------------- 
        //    _initSpec._lineHeight = s._lineHeight;
        //    _initSpec._textIndent = s._textIndent;
        //    _initSpec._actualColor = s._actualColor;
        //    _initSpec._emptyCells = s._emptyCells;
        //    //--------------------------------------- 
        //    _initSpec._textAlign = s._textAlign;
        //    _initSpec.VerticalAlign = s._verticalAlign;
        //    _initSpec._visibility = s._visibility;
        //    _initSpec._whitespace = s._whitespace;
        //    _initSpec._wordBreak = s._wordBreak;
        //    _initSpec._cssDirection = s._cssDirection;
        //    //---------------------------------------
        //    //2.
        //    //for clone only (eg. split a box into two parts)
        //    //=======================================
        //    _initSpec._backgroundProps = s._backgroundProps;

        //    //if (this.dbugId == 36)
        //    //{
        //    //}

        //    if (s._borderProps.Owner == s)
        //    {
        //        _initSpec._borderProps = s._borderProps;
        //    }
        //    _initSpec._borderProps = s._borderProps;


        //    _initSpec._cornerProps = s._cornerProps;
        //    //---------------------------------------

        //    _initSpec._left = s._left;
        //    _initSpec._top = s._top;
        //    _initSpec._bottom = s._bottom;
        //    _initSpec._right = s._right;

        //    _initSpec._width = s._width;
        //    _initSpec._height = s._height;
        //    _initSpec._maxWidth = s._maxWidth;
        //    _initSpec._position = s._position;

        //    _initSpec._wordSpacing = s._wordSpacing;
        //    _initSpec._lineHeight = s._lineHeight;
        //    _initSpec._float = s._float;

        //    //if (this.dbugId == 36)
        //    //{
        //    //}
        //    _initSpec._cssDisplay = s._cssDisplay;
        //    _initSpec._overflow = s._overflow;
        //    _initSpec._textDecoration = s._textDecoration;

        //    //3.
        //    //=====================================
        //    //if (this.dbugBB > 0)
        //    //{

        //    //}
        //    _initSpec._marginProps = s._marginProps;
        //    //--------------------------------------

        //    _initSpec._cssDirection = s._cssDirection;


        //    //-----------------------------------
        //    if (_initSpec._paddingProps.Owner != this)
        //    {
        //        _initSpec._paddingProps = s._paddingProps;
        //    }
        //    else
        //    {
        //        //this._prop_wait_eval |= (CssBoxAssignments.PADDING_LEFT |
        //        //                         CssBoxAssignments.PADDING_TOP |
        //        //                         CssBoxAssignments.PADDING_RIGHT |
        //        //                         CssBoxAssignments.PADDING_BOTTOM);
        //    }
        //    //-----------------------------------
        //}

        /// <summary>
        /// clone all style from another box
        /// </summary>
        /// <param name="s"></param>
        internal void CloneAllStyles(BoxSpec s)
        {

            //1.
            //=====================================
            if (s._fontProps.Owner == s)
            {
                _initSpec._fontProps = s._fontProps;
            }

            _initSpec._listProps = s._listProps;
            //--------------------------------------- 
            _initSpec._lineHeight = s._lineHeight;
            _initSpec._textIndent = s._textIndent;
            _initSpec._actualColor = s._actualColor;
            _initSpec._emptyCells = s._emptyCells;
            //--------------------------------------- 
            _initSpec._textAlign = s._textAlign;
            _initSpec.VerticalAlign = s._verticalAlign;
            _initSpec._visibility = s._visibility;
            _initSpec._whitespace = s._whitespace;
            _initSpec._wordBreak = s._wordBreak;
            _initSpec._cssDirection = s._cssDirection;
            //---------------------------------------
            //2.
            //for clone only (eg. split a box into two parts)
            //=======================================
            _initSpec._backgroundProps = s._backgroundProps;

            //if (this.dbugId == 36)
            //{
            //}

            if (s._borderProps.Owner == s)
            {
                _initSpec._borderProps = s._borderProps;
            }
            _initSpec._borderProps = s._borderProps;


            _initSpec._cornerProps = s._cornerProps;
            //---------------------------------------

            _initSpec._left = s._left;
            _initSpec._top = s._top;
            _initSpec._bottom = s._bottom;
            _initSpec._right = s._right;

            _initSpec._width = s._width;
            _initSpec._height = s._height;
            _initSpec._maxWidth = s._maxWidth;
            _initSpec._position = s._position;

            _initSpec._wordSpacing = s._wordSpacing;
            _initSpec._lineHeight = s._lineHeight;
            _initSpec._float = s._float;

            //if (this.dbugId == 36)
            //{
            //}
            _initSpec._cssDisplay = s._cssDisplay;
            _initSpec._overflow = s._overflow;
            _initSpec._textDecoration = s._textDecoration;

            //3.
            //=====================================
            //if (this.dbugBB > 0)
            //{

            //}
            _initSpec._marginProps = s._marginProps;
            //--------------------------------------

            _initSpec._cssDirection = s._cssDirection;


            //-----------------------------------
            if (_initSpec._paddingProps.Owner != this)
            {
                _initSpec._paddingProps = s._paddingProps;
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
            return _initSpec._borderProps = _initSpec._borderProps.GetMyOwnVersion(this);
        }
        CssMarginProp CheckMarginVersion()
        {
            return _initSpec._marginProps = _initSpec._marginProps.GetMyOwnVersion(this);
        }
        CssPaddingProp CheckPaddingVersion()
        {
            return _initSpec._paddingProps = _initSpec._paddingProps.GetMyOwnVersion(this);
        }
        CssCornerProp CheckCornerVersion()
        {
            return _initSpec._cornerProps = _initSpec._cornerProps.GetMyOwnVersion(this);
        }
        CssFontProp CheckFontVersion()
        {
            return _initSpec._fontProps = _initSpec._fontProps.GetMyOwnVersion(this);
        }
        CssListProp CheckListPropVersion()
        {
            return _initSpec._listProps = _initSpec._listProps.GetMyOwnVersion(this);
        }
        CssBackgroundProp CheckBgVersion()
        {
            return _initSpec._backgroundProps = _initSpec._backgroundProps.GetMyOwnVersion(this);
        }


    }

}