//BSD 2014, WinterFarm

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

using System.Globalization;
using System.Text.RegularExpressions;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
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


        #region CSS Properties
        public WellknownHtmlTagName WellknownTagName
        {
            get
            {
                return this.wellKnownTagName;
            }
            protected set
            {
                this.wellKnownTagName = value;
            }
        }

        public CssDisplay CssDisplay
        {
            get
            {

                return this._myspec.CssDisplay;
            }
            set
            {
                this._myspec.CssDisplay = value;
            }
        }
        public CssDirection CssDirection
        {
            get { return this._myspec.CssDirection; }
            set
            {
                this._myspec.CssDirection = value;
            }
        }
        //--------------------------------------------------------------------------------------
        public CssLength BorderLeftWidth
        {
            get { return this._myspec.BorderLeftWidth; }
            set
            {
                this._myspec.BorderLeftWidth = value;
                //this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_LEFT;
            }
        }

        public CssLength BorderRightWidth
        {
            get { return this._myspec.BorderRightWidth; }
            set
            {
                this._myspec.BorderRightWidth = value;
                //CheckBorderVersion().RightWidth = value;
                // this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_RIGHT;
            }
        }

        public CssLength BorderBottomWidth
        {
            get { return this._myspec.BorderBottomWidth; }
            set
            {
                this._myspec.BorderBottomWidth = value;
                // CheckBorderVersion().BottomWidth = value;
                //this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_BOTTOM;
            }
        }

        public CssLength BorderTopWidth
        {
            get { return this._myspec.BorderTopWidth; }
            set
            {
                this._myspec.BorderTopWidth = value;
                //CheckBorderVersion().TopWidth = value;
                //this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_TOP;
            }
        }
        //--------------------------------------------------------------------------------------
        public CssBorderStyle BorderTopStyle
        {
            get { return this._myspec.BorderTopStyle; }
            set { this._myspec.BorderTopStyle = value; }

        }
        public CssBorderStyle BorderLeftStyle
        {
            get { return this._myspec.BorderLeftStyle; }
            set { this._myspec.BorderLeftStyle = value; }
        }
        public CssBorderStyle BorderRightStyle
        {
            get { return this._myspec.BorderRightStyle; }
            set { this._myspec.BorderRightStyle = value; }
        }

        public CssBorderStyle BorderBottomStyle
        {

            get { return this._myspec.BorderBottomStyle; }
            set { this._myspec.BorderBottomStyle = value; }
        }

        //--------------------------------------------
        public Color BorderBottomColor
        {
            get { return this._myspec.BorderBottomColor; }
            set { this._myspec.BorderBottomColor = value; }
        }
        public Color BorderLeftColor
        {
            get { return this._myspec.BorderLeftColor; }
            set { this._myspec.BorderLeftColor = value; }
        }
        //--------------------------------------------
        public Color BorderRightColor
        {
            get { return this._myspec.BorderRightColor; }
            set { this._myspec.BorderRightColor = value; }
        }

        public Color BorderTopColor
        {
            get { return this._myspec.BorderTopColor; }
            set { this._myspec.BorderTopColor = value; }
        }
        public CssLength BorderSpacingVertical
        {
            get { return this._myspec.BorderSpacingVertical; }
            set { _myspec.BorderSpacingVertical = value; }
        }
        public CssLength BorderSpacingHorizontal
        {
            get { return this._myspec.BorderSpacingHorizontal; }
            set { this._myspec.BorderSpacingHorizontal = value; }
        }
        public CssBorderCollapse BorderCollapse
        {
            get { return this._myspec.BorderCollapse; }
            set { this._myspec.BorderCollapse = value; }
        }

        public bool IsBorderCollapse
        {
            get { return this.BorderCollapse == CssBorderCollapse.Collapse; }
        }
        //------------------------------------------------------
        public CssLength CornerNERadius
        {
            get { return this._myspec.CornerNERadius; }
            set
            {
                this._myspec.CornerNERadius = value;
            }
        }
        public CssLength CornerNWRadius
        {
            get { return this._myspec.CornerNWRadius; }
            set
            {
                this._myspec.CornerNWRadius = value;
            }
        }
        public CssLength CornerSERadius
        {
            get { return this._myspec.CornerSERadius; }
            set
            {
                this._myspec.CornerSERadius = value;
            }
        }
        public CssLength CornerSWRadius
        {
            get { return this._myspec.CornerSWRadius; }
            set
            {
                this._myspec.CornerSWRadius = value;
            }
        }
        //------------------------------------------------------
        public CssLength MarginBottom
        {
            get { return this._myspec.MarginBottom; }
            set { this._myspec.MarginBottom = value; }
        }

        public CssLength MarginLeft
        {
            get { return this._myspec.MarginLeft; }
            set { this._myspec.MarginLeft = value; }
        }

        public CssLength MarginRight
        {
            get { return this._myspec.MarginRight; }
            set { this._myspec.MarginRight = value; }
        }

        public CssLength MarginTop
        {
            get { return this._myspec.MarginTop; }
            set { this._myspec.MarginTop = value; }
        }

        public CssLength PaddingBottom
        {
            get { return this._myspec.MarginBottom; }
            set
            {
                this._myspec.MarginBottom = value;
            }
        }

        public CssLength PaddingLeft
        {
            get { return this._myspec.PaddingLeft; }
            set
            {
                this._myspec.PaddingLeft = value;
            }
        }

        public CssLength PaddingRight
        {
            get { return this._myspec.PaddingRight; }
            set
            {
                this._myspec.PaddingRight = value;
            }
        }

        public CssLength PaddingTop
        {
            get
            {
                return this._myspec.PaddingTop;
            }
            set
            {
                this._myspec.PaddingTop = value;
            }
        }
        public CssLength Left
        {
            get { return this._myspec.Left; }
            set { this._myspec.Left = value; }
        }

        public CssLength Top
        {
            get { return this._myspec.Top; }
            set { this._myspec.Top = value; }
        }

        public CssLength Width
        {
            get { return this._myspec.Width; }
            set { this._myspec.Width = value; }
        }
        public CssLength MaxWidth
        {
            get { return this._myspec.MaxWidth; }
            set { this._myspec.MaxWidth = value; }
        }
        public CssLength Height
        {
            get { return this._myspec.Height; }
            set { this._myspec.Height = value; }
        }
        public Color BackgroundColor
        {
            get { return this._myspec.BackgroundColor; }
            set { this._myspec.BackgroundColor = value; }
        }
        public ImageBinder BackgroundImageBinder
        {
            get { return this._myspec.BackgroundImageBinder; }
            set { this._myspec.BackgroundImageBinder = value; }
        }

        public CssLength BackgroundPositionX
        {
            get { return this._myspec.BackgroundPositionX; }
            set { this._myspec.BackgroundPositionX = value; }
        }
        public CssLength BackgroundPositionY
        {
            get { return this._myspec.BackgroundPositionY; }
            set { this._myspec.BackgroundPositionY = value; }
        }
        public CssBackgroundRepeat BackgroundRepeat
        {
            get { return this._myspec.BackgroundRepeat; }
            set { this._myspec.BackgroundRepeat = value; }
        }

        public Color BackgroundGradient
        {
            get { return this._myspec.BackgroundGradient; }
            set { this._myspec.BackgroundGradient = value; }
        }

        public float BackgroundGradientAngle
        {
            get { return this._myspec.ActualBackgroundGradientAngle; }
            set { this._myspec.ActualBackgroundGradientAngle = value; }
        }
        /// <summary>
        /// font color
        /// </summary>
        public Color Color
        {
            get { return this._myspec.Color; }
            set { this._myspec.Color = value; }
        }
        public CssEmptyCell EmptyCells
        {
            get { return this._myspec.EmptyCells; }
            set { this._myspec.EmptyCells = value; }
        }

        public CssFloat Float
        {
            get { return this._myspec.Float; }
            set { this._myspec.Float = value; }
        }
        public CssPosition Position
        {
            get { return this._myspec.Position; }
            set { this._myspec.Position = value; }
        }


        //----------------------------------------------------
        public CssLength LineHeight
        {
            get { return this._myspec.LineHeight; }
            set
            {
                this._myspec.LineHeight = value;
                this._prop_pass_eval &= ~CssBoxAssignments.LINE_HEIGHT;
            }
        }
        public CssVerticalAlign VerticalAlign
        {
            get { return this._myspec.VerticalAlign; }
            set
            {
                this._myspec.VerticalAlign = value;
            }
        }
        public CssLength TextIndent
        {
            get { return this._myspec.TextIndent; }
            set { this._myspec.TextIndent = value; }
        }
        public CssTextAlign CssTextAlign
        {
            get { return this._myspec.CssTextAlign; }
            set { this._myspec.CssTextAlign = value; }
        }

        public CssTextDecoration TextDecoration
        {
            get { return this._myspec.TextDecoration; }
            set { this._myspec.TextDecoration = value; }
        }

        //-----------------------------------
        public CssWhiteSpace WhiteSpace
        {
            get { return this._myspec.WhiteSpace; }
            set { this._myspec.WhiteSpace = value; }
        }
        //----------------------------------- 
        public CssVisibility CssVisibility
        {
            get { return this._myspec.CssVisibility; }
            set { this._myspec.CssVisibility = value; }
        }
        public CssLength WordSpacing
        {
            get { return this._myspec.WordSpacing; }
            set { this._myspec.WordSpacing = value; }
        }

        public CssWordBreak WordBreak
        {
            get { return this._myspec.WordBreak; }
            set { this._myspec.WordBreak = value; }
        }

        public string FontFamily
        {
            get { return this._myspec.FontFamily; }
            set { this._myspec.FontFamily = value; }
        }

        public CssLength FontSize
        {
            get { return this._myspec.FontSize; }
            set { this._myspec.FontSize = value; }
        }

        public CssFontStyle FontStyle
        {
            get { return this._myspec.FontStyle; }
            set { this._myspec.FontStyle = value; }
        }

        public CssFontVariant FontVariant
        {
            get { return this._myspec.FontVariant; }
            set { this._myspec.FontVariant = value; }
        }

        public CssFontWeight FontWeight
        {
            get { return this._myspec.FontWeight; }
            set { this._myspec.FontWeight = value; }
        }
        public CssOverflow Overflow
        {
            get { return this._myspec.Overflow; }
            set { this._myspec.Overflow = value; }
        }

        public string ListStyle
        {
            get { return this._myspec.ListStyle; }
            set { this._myspec.ListStyle = value; }
        }
        public CssListStylePosition ListStylePosition
        {
            get { return this._myspec.ListStylePosition; }
            set { this._myspec.ListStylePosition = value; }
        }
        public string ListStyleImage
        {
            get { return this._myspec.ListStyleImage; }
            set { this._myspec.ListStyleImage = value; }
        }

        public CssListStyleType ListStyleType
        {
            get { return this._myspec.ListStyleType; }
            set { this._myspec.ListStyleType = value; }
        }

        #endregion


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
                if (this._actualFont == null)
                {

                }
                var h = _actualFont.GetHeight();
                return this._actualFont;
            }
        }
        public int cssClassVersion
        {
            get
            {
                return this._myspec.VersionNumber;
            }
            set { this._myspec.VersionNumber = value; }
        }

        /// <summary>
        /// Get the parent of this css properties instance.
        /// </summary>
        /// <returns></returns>
        public virtual CssBox GetParent()
        {
            return this.ParentBox;
        }


        /// <summary>
        /// Gets the height of the font in the specified units
        /// </summary>
        /// <returns></returns>
        public float GetEmHeight()
        {
            return FontsUtils.GetFontHeight(ActualFont);
        }

        ///// <summary>
        ///// Ensures that the specified length is converted to pixels if necessary
        ///// </summary>
        ///// <param name="length"></param>
        //public CssLength NoEms(CssLength length)
        //{
        //    if (length.UnitOrNames == CssUnitOrNames.Ems)
        //    {
        //        return length.ConvertEmToPixels(GetEmHeight());
        //    }
        //    return length;
        //}

    }
}