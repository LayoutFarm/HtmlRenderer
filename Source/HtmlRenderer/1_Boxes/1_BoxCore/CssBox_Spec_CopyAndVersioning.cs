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

       
#if DEBUG 
        public static bool dbugCompare(dbugPropCheckReport dbugR, BoxSpec boxBase, BoxSpec spec)
        {

            int dd = boxBase.dbugId;
            dbugR.Check("_fontProps", CssFontFeature.dbugIsEq(dbugR, boxBase._fontFeats, spec._fontFeats));
            dbugR.Check("_listProps", CssListFeature.dbugIsEq(dbugR, boxBase._listFeats, spec._listFeats));
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

            dbugR.Check("_backgroundProps", CssBackgroundFeature.dbugIsEq(dbugR, boxBase._backgroundFeats, spec._backgroundFeats));
            dbugR.Check("_borderProps", CssBorderFeature.dbugIsEq(dbugR, boxBase._borderFeats, spec._borderFeats));
            dbugR.Check("_cornerProps", CssCornerFeature.dbugIsEq(dbugR, boxBase._cornerFeats, spec._cornerFeats));

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

        //void InternalInheritStyles(BoxSpec s, bool clone)
        //{
        //    if (s == null)
        //    {
        //        return;
        //    } 

        //    //---------------------------------------
        //    _initSpec._fontFeats = s._fontFeats;
        //    _initSpec._listFeats = s._listFeats;
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

        //    if (clone)
        //    {
        //        //for clone only (eg. split a box into two parts)
        //        //---------------------------------------
        //        _initSpec._backgroundFeats = s._backgroundFeats;

        //        _initSpec._borderFeats = s._borderFeats;
        //        _initSpec._cornerFeats = s._cornerFeats;
        //        //---------------------------------------

        //        _initSpec._left = s._left;
        //        _initSpec._top = s._top;
        //        _initSpec._bottom = s._bottom;
        //        _initSpec._right = s._right;

        //        _initSpec._width = s._width;
        //        _initSpec._height = s._height;
        //        _initSpec._maxWidth = s._maxWidth;
        //        _initSpec._position = s._position;


        //        _initSpec._wordSpacing = s._wordSpacing;
        //        _initSpec._lineHeight = s._lineHeight;
        //        _initSpec._float = s._float;


        //        _initSpec._cssDisplay = s._cssDisplay;
        //        _initSpec._overflow = s._overflow;
        //        _initSpec._textDecoration = s._textDecoration; 
        //    }

        //}

       

        ///// <summary>
        ///// clone all style from another box
        ///// </summary>
        ///// <param name="s"></param>
        //internal void CloneAllStyles(BoxSpec s)
        //{

        //    //1.
        //    //=====================================
        //    if (s._fontFeats.Owner == s)
        //    {
        //        _initSpec._fontFeats = s._fontFeats;
        //    }

        //    _initSpec._listFeats = s._listFeats;
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
        //    _initSpec._backgroundFeats = s._backgroundFeats;

        //    //if (this.dbugId == 36)
        //    //{
        //    //}

        //    if (s._borderFeats.Owner == s)
        //    {
        //        _initSpec._borderFeats = s._borderFeats;
        //    }
        //    _initSpec._borderFeats = s._borderFeats;


        //    _initSpec._cornerFeats = s._cornerFeats;
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
        //    _initSpec._marginFeats = s._marginFeats;
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


        protected int _prop_pass_eval;
        CssBorderFeature CheckBorderVersion()
        {
            return _initSpec._borderFeats = _initSpec._borderFeats.GetMyOwnVersion(this);
        }
        CssMarginFeature CheckMarginVersion()
        {
            return _initSpec._marginFeats = _initSpec._marginFeats.GetMyOwnVersion(this);
        }
        CssPaddingFeature CheckPaddingVersion()
        {
            return _initSpec._paddingProps = _initSpec._paddingProps.GetMyOwnVersion(this);
        }
        CssCornerFeature CheckCornerVersion()
        {
            return _initSpec._cornerFeats = _initSpec._cornerFeats.GetMyOwnVersion(this);
        }
        CssFontFeature CheckFontVersion()
        {
            return _initSpec._fontFeats = _initSpec._fontFeats.GetMyOwnVersion(this);
        }
        CssListFeature CheckListPropVersion()
        {
            return _initSpec._listFeats = _initSpec._listFeats.GetMyOwnVersion(this);
        }
        CssBackgroundFeature CheckBgVersion()
        {
            return _initSpec._backgroundFeats = _initSpec._backgroundFeats.GetMyOwnVersion(this);
        }


    }

}