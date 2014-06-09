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
    partial class CssBoxBase
    {
        /// <summary>
        /// Inherits inheritable values from specified box.
        /// </summary>
        /// <param name="clone">Set to true to inherit all CSS properties instead of only the ineritables</param>
        /// <param name="p">Box to inherit the properties</param>
        protected void InheritStyles(CssBoxBase p, bool clone)
        {
            if (p != null)
            {

                //---------------------------------------
                this._fontProps = p._fontProps;
                this._listProps = p._listProps;
                //--------------------------------------- 
                this._lineHeight = p._lineHeight;
                this._textIndent = p._textIndent;
                this._actualColor = p._actualColor;
                this._emptyCells = p._emptyCells;
                //--------------------------------------- 
                this._textAlign = p._textAlign;
                this._verticalAlign = p._verticalAlign;
                this._visibility = p._visibility;
                this._whitespace = p._whitespace;
                this._wordBreak = p._wordBreak;
                this._cssDirection = p._cssDirection;
                //---------------------------------------


                if (clone)
                {
                    //for clone only (eg. split a box into two parts)
                    //---------------------------------------
                    this._backgroundProps = p._backgroundProps;
                    this._borderProps = p._borderProps;
                    this._cornerProps = p._cornerProps;
                    //---------------------------------------

                    this._left = p._left;
                    this._top = p._top;
                    this._bottom = p._bottom;
                    this._right = p._right;

                    this._width = p._width;
                    this._height = p._height;
                    this._maxWidth = p._maxWidth;
                    this._position = p._position;


                    this._wordSpacing = p._wordSpacing;
                    this._lineHeight = p._lineHeight;
                    this._float = p._float;

                    this._cssDisplay = p._cssDisplay;
                    this._overflow = p._overflow;
                    this._textDecoration = p._textDecoration; 
                    //--------------------------------------- 
                }
            }
        }
        internal void SpecialCloneStyles(CssBoxBase p)
        { 
            //-----------------------
            InheritStyles(p, true); 
            //-----------------------
            this._paddingProps = p._paddingProps;
            this._marginProps = p._marginProps;
            //----------------------- 

            this._actualBorderLeftWidth = p._actualBorderLeftWidth;
            this._actualBorderTopWidth = p._actualBorderTopWidth;
            this._actualBorderRightWidth = p._actualBorderRightWidth;
            this._actualBorderBottomWidth = p._actualBorderBottomWidth;

            this._actualPaddingLeft = p._actualPaddingLeft;
            this._actualPaddingTop = p._actualPaddingTop;
            this._actualPaddingRight = p._actualPaddingRight;
            this._actualPaddingBottom = p._actualPaddingBottom;


            this._actualLineHeight = p._actualLineHeight;

            this._cssDirection = p._cssDirection;
           

        }
    }
}