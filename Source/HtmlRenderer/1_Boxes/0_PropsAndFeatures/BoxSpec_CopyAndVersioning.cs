//BSD 2014, WinterFarm 
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

        /// <summary>
        /// Inherits inheritable values from specified box.
        /// </summary>
        /// <param name="s">source </param>
        /// <param name="clone">clone all </param>
        internal void InheritStyles(BoxSpec s, bool clone)
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

            if (clone)
            {
                //for clone only (eg. split a box into two parts)
                //---------------------------------------
                this._backgroundFeats = s._backgroundFeats;

                this._borderFeats = s._borderFeats;
                this._cornerFeats = s._cornerFeats;
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
        internal void CloneAllStyles(BoxSpec s)
        {
            //1.
            //=====================================
            if (s._fontFeats.Owner == s)
            {
                this._fontFeats = s._fontFeats;
            }

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

            //if (this.dbugId == 36)
            //{
            //}

            if (s._borderFeats.Owner == s)
            {
                this._borderFeats = s._borderFeats;
            }
            this._borderFeats = s._borderFeats;


            this._cornerFeats = s._cornerFeats;
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
            this._marginFeats = s._marginFeats;
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



        public CssBox GetParent()
        {
            return null;
        }

        public void InheritStylesFrom(BoxSpec source)
        {
            this.InheritStyles(source, false);
        }

        public void CloneAllStylesFrom(BoxSpec source)
        {
            this.InheritStyles(source, true);
        }
        public BoxSpec GetAnonVersion()
        {
            if (anonVersion != null)
            {
                return anonVersion;
            }
            this.anonVersion = new BoxSpec(WellknownHtmlTagName.Unknown);
            anonVersion.InheritStyles(this, false);
            return anonVersion;
        }

        protected int _prop_pass_eval;
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
            return this._paddingProps = this._paddingProps.GetMyOwnVersion(this);
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