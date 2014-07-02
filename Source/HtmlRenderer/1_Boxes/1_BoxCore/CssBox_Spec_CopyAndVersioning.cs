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
            return _spec._borderFeats = _spec._borderFeats.GetMyOwnVersion(this);
        }
        CssMarginFeature CheckMarginVersion()
        {
            return _spec._marginFeats = _spec._marginFeats.GetMyOwnVersion(this);
        }
        CssPaddingFeature CheckPaddingVersion()
        {
            return _spec._paddingProps = _spec._paddingProps.GetMyOwnVersion(this);
        }
        CssCornerFeature CheckCornerVersion()
        {
            return _spec._cornerFeats = _spec._cornerFeats.GetMyOwnVersion(this);
        }
        CssFontFeature CheckFontVersion()
        {
            return _spec._fontFeats = _spec._fontFeats.GetMyOwnVersion(this);
        }
        CssListFeature CheckListPropVersion()
        {
            return _spec._listFeats = _spec._listFeats.GetMyOwnVersion(this);
        }
        CssBackgroundFeature CheckBgVersion()
        {
            return _spec._backgroundFeats = _spec._backgroundFeats.GetMyOwnVersion(this);
        }


    }

}