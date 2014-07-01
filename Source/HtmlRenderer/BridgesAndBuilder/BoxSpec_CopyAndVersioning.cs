//BSD 2014, WinterCore 
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
        protected void InheritStyles(CssBox s, bool clone)
        {
            if (s == null)
            {
                return;
            }
            //---------------------------------------
            this._fontProps = s._initSpec._fontProps;
            this._listProps = s._initSpec._listProps;
            //--------------------------------------- 
            this._lineHeight = s._initSpec._lineHeight;
            this._textIndent = s._initSpec._textIndent;
            this._actualColor = s._initSpec._actualColor;
            this._emptyCells = s._initSpec._emptyCells;
            //--------------------------------------- 
            this._textAlign = s._initSpec._textAlign;
            this.VerticalAlign = s._initSpec._verticalAlign;
            this._visibility = s._initSpec._visibility;
            this._whitespace = s._initSpec._whitespace;
            this._wordBreak = s._initSpec._wordBreak;
            this._cssDirection = s._initSpec._cssDirection;
            //--------------------------------------- 

            if (clone)
            {
                //for clone only (eg. split a box into two parts)
                //---------------------------------------
                this._backgroundProps = s._initSpec._backgroundProps;

                this._borderProps = s._initSpec._borderProps;
                this._cornerProps = s._initSpec._cornerProps;
                //---------------------------------------

                this._left = s._initSpec._left;
                this._top = s._initSpec._top;
                this._bottom = s._initSpec._bottom;
                this._right = s._initSpec._right;

                this._width = s._initSpec._width;
                this._height = s._initSpec._height;
                this._maxWidth = s._initSpec._maxWidth;
                this._position = s._initSpec._position;


                this._wordSpacing = s._initSpec._wordSpacing;
                this._lineHeight = s._initSpec._lineHeight;
                this._float = s._initSpec._float;


                this._cssDisplay = s._initSpec._cssDisplay;
                this._overflow = s._initSpec._overflow;
                this._textDecoration = s._initSpec._textDecoration;
                //--------------------------------------- 


            }

        }

        /// <summary>
        /// Inherits inheritable values from specified box.
        /// </summary>
        /// <param name="s">source </param>
        /// <param name="clone">clone all </param>
        protected void InheritStyles(BoxSpec s, bool clone)
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
        internal void CloneAllStyles(BoxSpec s)
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
            this._verticalAlign = s._verticalAlign;
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