//BSD 2014, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.Drawing;
 
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    /// <summary>
    /// Base class for css box to handle the css properties.<br/>
    /// Has field and property for every css property that can be set, the properties add additional parsing like
    /// setting the correct border depending what border value was set (single, two , all four).<br/>
    /// Has additional fields to control the location and size of the box and 'actual' css values for some properties
    /// that require additional calculations and parsing.<br/>
    /// </summary>
    partial class CssBox
    {    
        public CssLength Height
        { 
            get { return this._myspec.Height; }  
        }

        public CssDirection CssDirection
        {
            get { return this._myspec.CssDirection; }
        }
        //---------------------------------------------- 
        public CssLength BorderLeftWidth
        {
            get { return this._myspec.BorderLeftWidth; }
        }
        public CssLength BorderTopWidth
        {
            get { return this._myspec.BorderTopWidth; }
        }
        public CssLength BorderRightWidth
        {
            get { return this._myspec.BorderRightWidth; }
        }

        public CssLength BorderBottomWidth
        {
            get { return this._myspec.BorderBottomWidth; }
        }
        //----------------------------------------------- 
        public CssBorderStyle BorderLeftStyle
        {
            get { return this._myspec.BorderLeftStyle; }
        }
        public CssBorderStyle BorderTopStyle
        {
            get { return this._myspec.BorderTopStyle; }
        }
        public CssBorderStyle BorderRightStyle
        {
            get { return this._myspec.BorderRightStyle; }
        }
        public CssBorderStyle BorderBottomStyle
        {
            get { return this._myspec.BorderBottomStyle; }
        }
        //--------------------------------------------
        public Color BorderLeftColor
        {
            get { return this._myspec.BorderLeftColor; }
        }
        public Color BorderTopColor
        {
            get { return this._myspec.BorderTopColor; }
        }
        public Color BorderRightColor
        {
            get { return this._myspec.BorderRightColor; }
        }
        public Color BorderBottomColor
        {
            get { return this._myspec.BorderBottomColor; }
        }
        //--------------------------------------------
        public CssLength BorderSpacingVertical
        {
            get { return this._myspec.BorderSpacingVertical; }
        }
        public CssLength BorderSpacingHorizontal
        {
            get { return this._myspec.BorderSpacingHorizontal; }
        }
        public CssBorderCollapse BorderCollapse
        {
            get { return this._myspec.BorderCollapse; }
        }

        public bool IsBorderCollapse
        {
            get { return this.BorderCollapse == CssBorderCollapse.Collapse; }
        }
        //------------------------------------------------------
        public CssLength CornerNERadius
        {
            get { return this._myspec.CornerNERadius; }
        }
        public CssLength CornerNWRadius
        {
            get { return this._myspec.CornerNWRadius; }
        }
        public CssLength CornerSERadius
        {
            get { return this._myspec.CornerSERadius; }
        }
        public CssLength CornerSWRadius
        {
            get { return this._myspec.CornerSWRadius; }
        }
        //------------------------------------------------------ 
        public CssLength MarginLeft
        {
            get { return this._myspec.MarginLeft; }
        }
        public CssLength MarginTop
        {
            get { return this._myspec.MarginTop; }
        }
        public CssLength MarginRight
        {
            get { return this._myspec.MarginRight; }
        }
        public CssLength MarginBottom
        {
            get { return this._myspec.MarginBottom; }
        }
        //------------------------------------------------------
        public CssLength PaddingLeft
        {
            get { return this._myspec.PaddingLeft; }
        }
        public CssLength PaddingTop
        {
            get { return this._myspec.PaddingTop; }
        }
        public CssLength PaddingRight
        {
            get { return this._myspec.PaddingRight; }
        }
        public CssLength PaddingBottom
        {
            get { return this._myspec.MarginBottom; }
        }
        //------------------------------------------------------ 
        public CssLength Left
        {
            get { return this._myspec.Left; }
        }
        public CssLength Top
        {
            get { return this._myspec.Top; }
        }
        public CssLength Width
        {
            get { return this._myspec.Width; }
        }
        public CssLength MaxWidth
        {
            get { return this._myspec.MaxWidth; }
        }

        //------------------------------------------------------ 
        public Color BackgroundColor
        {
            get { return this._myspec.BackgroundColor; }
        }
        public ImageBinder BackgroundImageBinder
        {
            get { return this._myspec.BackgroundImageBinder; }
        }
        public CssLength BackgroundPositionX
        {
            get { return this._myspec.BackgroundPositionX; }
        }
        public CssLength BackgroundPositionY
        {
            get { return this._myspec.BackgroundPositionY; }
        }
        public CssBackgroundRepeat BackgroundRepeat
        {
            get { return this._myspec.BackgroundRepeat; }
        }
        public Color BackgroundGradient
        {
            get { return this._myspec.BackgroundGradient; }
        }

        public float BackgroundGradientAngle
        {
            get { return this._myspec.ActualBackgroundGradientAngle; }
        }
        //------------------------------------------------------ 
        /// <summary>
        /// font color
        /// </summary>
        public Color Color
        {
            get { return this._myspec.Color; }
        }
        public CssEmptyCell EmptyCells
        {
            get { return this._myspec.EmptyCells; }
        }
        public CssFloat Float
        {
            get { return this._myspec.Float; }
        }
        public CssPosition Position
        {
            get { return this._myspec.Position; }
        }
        //----------------------------------------------------
        public CssLength LineHeight
        {
            get { return this._myspec.LineHeight; }

        }
        public CssVerticalAlign VerticalAlign
        {
            get { return this._myspec.VerticalAlign; }
        }
        public CssLength TextIndent
        {
            get { return this._myspec.TextIndent; }
        }
        public CssTextAlign CssTextAlign
        {
            get { return this._myspec.CssTextAlign; }
        }
        public CssTextDecoration TextDecoration
        {
            get { return this._myspec.TextDecoration; }
        }
        //-----------------------------------
        public CssWhiteSpace WhiteSpace
        {
            get { return this._myspec.WhiteSpace; }
        }
        //----------------------------------- 
        public CssVisibility Visibility
        {
            get { return this._myspec.Visibility; }
        }
        public CssLength WordSpacing
        {
            get { return this._myspec.WordSpacing; }
        }

        public CssWordBreak WordBreak
        {
            get { return this._myspec.WordBreak; }
        }
        //----------------------------------- 
        public string FontFamily
        {
            get { return this._myspec.FontFamily; }
        }

        public CssLength FontSize
        {
            get { return this._myspec.FontSize; }
        }

        public CssFontStyle FontStyle
        {
            get { return this._myspec.FontStyle; }
        }

        public CssFontVariant FontVariant
        {
            get { return this._myspec.FontVariant; }
        }

        public CssFontWeight FontWeight
        {
            get { return this._myspec.FontWeight; }
        }
        public CssOverflow Overflow
        {
            get { return this._myspec.Overflow; }
        }
        //----------------------------------- 
        public string ListStyle
        {
            get { return this._myspec.ListStyle; }
        }
        public CssListStylePosition ListStylePosition
        {
            get { return this._myspec.ListStylePosition; }
        }
        public string ListStyleImage
        {
            get { return this._myspec.ListStyleImage; }
        }

        public CssListStyleType ListStyleType
        {
            get { return this._myspec.ListStyleType; }
        }

        /// <summary>
        /// Gets the second color that creates a gradient for the background
        /// </summary>
        public Color ActualBackgroundGradient
        {
            get
            {
                return this._myspec.BackgroundGradient;
            }
        }

        /// <summary>
        /// Gets the actual angle specified for the background gradient
        /// </summary>
        public float ActualBackgroundGradientAngle
        {
            get
            {
                return this._myspec.BackgroundGradientAngle;
            }
        }

        /// <summary>
        /// 
        /// Gets the actual color for the text.
        /// </summary>
        public Color ActualColor
        {
            get
            {

                return this._myspec.ActualColor;
            }
        }

        /// <summary>
        /// Gets the actual background color of the box
        /// </summary>
        public Color ActualBackgroundColor
        {
            get
            {
                return this._myspec.ActualBackgroundColor;
            }
        }
        /// <summary>
        /// Gets the font that should be actually used to paint the text of the box
        /// </summary>
        public Font ActualFont
        {
            get
            {
                //if (this._actualFont == null)
                //{

                //}
                //var h = _actualFont.GetHeight();
                return this._actualFont;
            }
        }



        /// <summary>
        /// Gets the height of the font in the specified units
        /// </summary>
        /// <returns></returns>
        internal float GetEmHeight()
        {
            //after has actual font
            return FontsUtils.GetFontHeight(ActualFont);
        }


    }
}