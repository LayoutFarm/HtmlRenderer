//BSD 2014, WinterDev 
using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    partial class BoxSpec
    {
         

        public void InheritStylesFrom(BoxSpec s)
        {
            if (s == null)
            {
                return;
            }
            //---------------------------------------
            this._fontFeats = s._fontFeats;
            this._listFeats = s._listFeats;
            //--------------------------------------- 
            this._lineHeight = s._lineHeight;
            this._textIndent = s._textIndent;
            this._actualColor = s._actualColor;
            this._emptyCells = s._emptyCells;
            //--------------------------------------- 
            this._textAlign = s._textAlign;
            this._verticalAlign = s._verticalAlign;
            this._visibility = s._visibility;
            this._whitespace = s._whitespace;
            this._wordBreak = s._wordBreak;
            this._cssDirection = s._cssDirection;
            //--------------------------------------- 
             
        }

        public void CloneAllStylesFrom(BoxSpec s)
        {
            //1.
            //=====================================
            if (s._fontFeats.Owner == s)
            {
                //this._fontFeats = s._fontFeats;
            }
            this._fontFeats = s._fontFeats;
            this._listFeats = s._listFeats;
            //--------------------------------------- 
            this._lineHeight = s._lineHeight;
            this._textIndent = s._textIndent;
            this._actualColor = s._actualColor;
            this._emptyCells = s._emptyCells;
            //--------------------------------------- 
            this._textAlign = s._textAlign;
            this._verticalAlign = s._verticalAlign;
            this._visibility = s._visibility;
            this._whitespace = s._whitespace;
            this._wordBreak = s._wordBreak;
            this._cssDirection = s._cssDirection;
            //---------------------------------------
           
            //2.
            //for clone only (eg. split a box into two parts)
            //=======================================
            this._backgroundFeats = s._backgroundFeats;
            this._borderFeats = s._borderFeats; 
           
            this._cornerFeats = s._cornerFeats;
            this._marginFeats = s._marginFeats;
            this._paddingFeats = s._paddingFeats;
            //---------------------------------------
            this._cssDisplay = s._cssDisplay;
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
             
         
            this._overflow = s._overflow;
            this._textDecoration = s._textDecoration;

            //3.
            //===================================== 
            this._cssDirection = s._cssDirection; 
           
            //if (this._paddingFeats.Owner != this)
            //{
            //    this._paddingFeats = s._paddingFeats;
            //}
            //else
            //{
            //    //this._prop_wait_eval |= (CssBoxAssignments.PADDING_LEFT |
            //    //                         CssBoxAssignments.PADDING_TOP |
            //    //                         CssBoxAssignments.PADDING_RIGHT |
            //    //                         CssBoxAssignments.PADDING_BOTTOM);
            //}
            //-----------------------------------
        }
        public BoxSpec GetAnonVersion()
        {
            if (anonVersion != null)
            {
                return anonVersion;
            }
            this.anonVersion = new BoxSpec(WellknownHtmlTagName.Unknown);
            anonVersion.InheritStylesFrom(this);
            return anonVersion;
        }
        //---------------------------------------------------------------

        CssBorderFeature CheckBorderVersion()
        {
            return this._borderFeats = this._borderFeats.GetMyOwnVersion(this);
        }
        CssMarginFeature CheckMarginVersion()
        {
            return this._marginFeats = this._marginFeats.GetMyOwnVersion(this);
        }
        CssPaddingFeature CheckPaddingVersion()
        {
            return this._paddingFeats = this._paddingFeats.GetMyOwnVersion(this);
        }
        CssCornerFeature CheckCornerVersion()
        {
            return this._cornerFeats = this._cornerFeats.GetMyOwnVersion(this);
        }
        CssFontFeature CheckFontVersion()
        {
            return this._fontFeats = this._fontFeats.GetMyOwnVersion(this);
        }
        CssListFeature CheckListPropVersion()
        {
            return this._listFeats = this._listFeats.GetMyOwnVersion(this);
        }
        CssBackgroundFeature CheckBgVersion()
        {
            return this._backgroundFeats = this._backgroundFeats.GetMyOwnVersion(this);
        }


    }

}