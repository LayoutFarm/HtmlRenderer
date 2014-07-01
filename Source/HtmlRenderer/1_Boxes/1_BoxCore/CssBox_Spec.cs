//BSD 2014, WinterCore

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
        WellknownHtmlTagName wellKnownTagName;

#if DEBUG
        public readonly int dbugId = dbugTotalId++;
        static int dbugTotalId;
        public int dbugMark;
#endif

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
            get { return this._initSpec.CssDisplay; }
            set
            {
                this._initSpec.CssDisplay = value;
            }
        }
        public CssDirection CssDirection
        {
            get { return this._initSpec._cssDirection; }
            set { this._initSpec._cssDirection = value; }
        }
        //--------------------------------------------------------------------------------------
        public CssLength BorderLeftWidth
        {
            get { return this._initSpec._borderFeats.LeftWidth; }
            set
            {
                CheckBorderVersion().LeftWidth = value;
                //this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_LEFT;
            }
        }

        public CssLength BorderRightWidth
        {
            get { return this._initSpec._borderFeats.RightWidth; }
            set
            {
                CheckBorderVersion().RightWidth = value;
                // this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_RIGHT;
            }
        }

        public CssLength BorderBottomWidth
        {
            get { return this._initSpec._borderFeats.BottomWidth; }
            set
            {
                CheckBorderVersion().BottomWidth = value;
                //this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_BOTTOM;
            }
        }

        public CssLength BorderTopWidth
        {
            get { return this._initSpec._borderFeats.TopWidth; }
            set
            {
                CheckBorderVersion().TopWidth = value;
                //this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_TOP;
            }
        }
        //--------------------------------------------------------------------------------------
        public CssBorderStyle BorderTopStyle
        {

            get { return this._initSpec._borderFeats.TopStyle; }
            set { CheckBorderVersion().TopStyle = value; }

        }
        public CssBorderStyle BorderLeftStyle
        {
            get { return this._initSpec._borderFeats.LeftStyle; }
            set { CheckBorderVersion().LeftStyle = value; }
        }
        public CssBorderStyle BorderRightStyle
        {
            get { return this._initSpec._borderFeats.RightStyle; }
            set { CheckBorderVersion().RightStyle = value; }
        }

        public CssBorderStyle BorderBottomStyle
        {

            get { return this._initSpec._borderFeats.BottomStyle; }
            set { CheckBorderVersion().BottomStyle = value; }
        }

        //--------------------------------------------
        public Color BorderBottomColor
        {
            get { return this._initSpec._borderFeats.BottomColor; }
            set { CheckBorderVersion().BottomColor = value; }
        }
        public Color BorderLeftColor
        {
            get { return this._initSpec._borderFeats.LeftColor; }
            set { CheckBorderVersion().LeftColor = value; }
        }
        //--------------------------------------------
        public Color BorderRightColor
        {
            get { return this._initSpec._borderFeats.RightColor; }
            set { CheckBorderVersion().RightColor = value; }
        }

        public Color BorderTopColor
        {
            get { return this._initSpec._borderFeats.TopColor; }
            set { CheckBorderVersion().TopColor = value; }
        }
        public CssLength BorderSpacingVertical
        {
            get { return this._initSpec._borderFeats.BorderSpacingV; }
            set { CheckBorderVersion().BorderSpacingV = value; }
        }
        public CssLength BorderSpacingHorizontal
        {
            get { return this._initSpec._borderFeats.BorderSpacingH; }
            set { CheckBorderVersion().BorderSpacingH = value; }
        }
        public CssBorderCollapse BorderCollapse
        {
            get { return this._initSpec._borderFeats.BorderCollapse; }
            set { CheckBorderVersion().BorderCollapse = value; }
        }

        public bool IsBorderCollapse
        {
            get { return this.BorderCollapse == CssBorderCollapse.Collapse; }
        }
        //------------------------------------------------------
        public CssLength CornerNERadius
        {
            get { return this._initSpec._cornerFeats.NERadius; }
            set
            {
                CheckCornerVersion().NERadius = value;
            }
        }
        public CssLength CornerNWRadius
        {
            get { return this._initSpec._cornerFeats.NWRadius; }
            set
            {
                CheckCornerVersion().NWRadius = value;
            }
        }
        public CssLength CornerSERadius
        {
            get { return this._initSpec._cornerFeats.SERadius; }
            set
            {
                CheckCornerVersion().SERadius = value;
            }
        }
        public CssLength CornerSWRadius
        {
            get { return this._initSpec._cornerFeats.SWRadius; }
            set
            {
                CheckCornerVersion().SWRadius = value;
            }
        }
        //------------------------------------------------------
        public CssLength MarginBottom
        {
            get { return this._initSpec._marginFeats.Bottom; }
            set { CheckMarginVersion().Bottom = value; }
        }

        public CssLength MarginLeft
        {
            get { return this._initSpec._marginFeats.Left; }
            set { CheckMarginVersion().Left = value; }
        }

        public CssLength MarginRight
        {
            get { return this._initSpec._marginFeats.Right; }
            set { CheckMarginVersion().Right = value; }
        }

        public CssLength MarginTop
        {
            get { return this._initSpec._marginFeats.Top; }
            set { CheckMarginVersion().Top = value; }
        }

        public CssLength PaddingBottom
        {
            get { return this._initSpec._paddingProps.Bottom; }
            set
            {
                CheckPaddingVersion().Bottom = value;
            }
        }

        public CssLength PaddingLeft
        {
            get { return this._initSpec._paddingProps.Left; }
            set
            {
                CheckPaddingVersion().Left = value;
            }
        }

        public CssLength PaddingRight
        {
            get { return this._initSpec._paddingProps.Right; }
            set
            {
                CheckPaddingVersion().Right = value;

            }
        }

        public CssLength PaddingTop
        {
            get
            {
                return this._initSpec._paddingProps.Top;
            }
            set
            {
                CheckPaddingVersion().Top = value;
            }
        }
        public CssLength Left
        {
            get { return this._initSpec._left; }
            set { this._initSpec._left = value; }
        }

        public CssLength Top
        {
            get { return this._initSpec._top; }
            set { this._initSpec._top = value; }
        }

        public CssLength Width
        {
            get { return this._initSpec._width; }
            set { this._initSpec._width = value; }
        }
        public CssLength MaxWidth
        {
            get { return this._initSpec._maxWidth; }
            set { this._initSpec._maxWidth = value; }
        }
        public CssLength Height
        {
            get { return this._initSpec._height; }
            set { this._initSpec._height = value; }
        }
        public Color BackgroundColor
        {
            get { return this._initSpec._backgroundFeats.BackgroundColor; }
            set { CheckBgVersion().BackgroundColor = value; }
        }
        public ImageBinder BackgroundImageBinder
        {
            get { return this._initSpec._backgroundFeats.BackgroundImageBinder; }
            set { CheckBgVersion().BackgroundImageBinder = value; }
        }


        public CssLength BackgroundPositionX
        {
            get { return this._initSpec._backgroundFeats.BackgroundPosX; }
            set { CheckBgVersion().BackgroundPosX = value; }
        }
        public CssLength BackgroundPositionY
        {
            get { return this._initSpec._backgroundFeats.BackgroundPosY; }
            set { CheckBgVersion().BackgroundPosY = value; }
        }
        public CssBackgroundRepeat BackgroundRepeat
        {
            get { return this._initSpec._backgroundFeats.BackgroundRepeat; }
            set { CheckBgVersion().BackgroundRepeat = value; }
        }

        public Color BackgroundGradient
        {
            get { return this._initSpec._backgroundFeats.BackgroundGradient; }
            set { CheckBgVersion().BackgroundGradient = value; }
        }

        public float BackgroundGradientAngle
        {
            get { return this._initSpec._backgroundFeats.BackgroundGradientAngle; }
            set { CheckBgVersion().BackgroundGradientAngle = value; }
        }
        /// <summary>
        /// font color
        /// </summary>
        public Color Color
        {
            get { return this._initSpec._actualColor; }
            set { this._initSpec._actualColor = value; }
        }
        public CssEmptyCell EmptyCells
        {
            get { return this._initSpec._emptyCells; }
            set { this._initSpec._emptyCells = value; }
        }

        public CssFloat Float
        {
            get { return this._initSpec._float; }
            set { this._initSpec._float = value; }
        }
        public CssPosition Position
        {
            get { return this._initSpec._position; }
            set { this._initSpec._position = value; }
        }


        //----------------------------------------------------
        public CssLength LineHeight
        {
            get { return this._initSpec._lineHeight; }
            set
            {
                this._initSpec._lineHeight = value;
                this._prop_pass_eval &= ~CssBoxAssignments.LINE_HEIGHT;
            }
        }
        public CssVerticalAlign VerticalAlign
        {
            get { return this._initSpec._verticalAlign; }
            set
            {   
                this._initSpec._verticalAlign = value;
            }
        }
        public CssLength TextIndent
        {
            get { return this._initSpec._textIndent; }
            set { this._initSpec._textIndent = NoEms(value); }
        }
        public CssTextAlign CssTextAlign
        {
            get { return this._initSpec._textAlign; }
            set { this._initSpec._textAlign = value; }
        }

        public CssTextDecoration TextDecoration
        {
            get { return this._initSpec._textDecoration; }
            set { this._initSpec._textDecoration = value; }
        }

        //-----------------------------------
        public CssWhiteSpace WhiteSpace
        {
            get { return this._initSpec._whitespace; }
            set { this._initSpec._whitespace = value; }
        }
        //----------------------------------- 
        public CssVisibility CssVisibility
        {
            get { return this._initSpec._visibility; }
            set { this._initSpec._visibility = value; }
        }
        public CssLength WordSpacing
        {
            get { return this._initSpec._wordSpacing; }
            set { this._initSpec._wordSpacing = this.NoEms(value); }
        }

        public CssWordBreak WordBreak
        {
            get { return this._initSpec._wordBreak; }
            set { this._initSpec._wordBreak = value; }
        }

        public string FontFamily
        {
            get { return this._initSpec._fontFeats.FontFamily; }
            set { CheckFontVersion().FontFamily = value; }
        }

        public CssLength FontSize
        {
            get { return this._initSpec._fontFeats.FontSize; }
            set { CheckFontVersion().FontSize = value; }
        }

        public CssFontStyle FontStyle
        {
            get { return this._initSpec._fontFeats.FontStyle; }
            set { CheckFontVersion().FontStyle = value; }
        }

        public CssFontVariant FontVariant
        {
            get { return this._initSpec._fontFeats.FontVariant; }
            set { CheckFontVersion().FontVariant = value; }
        }

        public CssFontWeight FontWeight
        {
            get { return this._initSpec._fontFeats.FontWeight; }
            set { CheckFontVersion().FontWeight = value; }
        }
        public CssOverflow Overflow
        {
            get { return this._initSpec._overflow; }
            set { this._initSpec._overflow = value; }
        }

        public string ListStyle
        {
            get { return this._initSpec._listFeats.ListStyle; }
            set { CheckListPropVersion().ListStyle = value; }
        }
        public CssListStylePosition ListStylePosition
        {
            get { return this._initSpec._listFeats.ListStylePosition; }
            set { CheckListPropVersion().ListStylePosition = value; }
        }
        public string ListStyleImage
        {
            get { return this._initSpec._listFeats.ListStyleImage; }
            set { CheckListPropVersion().ListStyleImage = value; }
        }

        public CssListStyleType ListStyleType
        {
            get { return this._initSpec._listFeats.ListStyleType; }
            set { CheckListPropVersion().ListStyleType = value; }
        }

        #endregion


        /// <summary>
        /// Gets the second color that creates a gradient for the background
        /// </summary>
        public Color ActualBackgroundGradient
        {
            get
            {
                return this._initSpec._backgroundFeats.BackgroundGradient;
            }
        }

        /// <summary>
        /// Gets the actual angle specified for the background gradient
        /// </summary>
        public float ActualBackgroundGradientAngle
        {
            get
            {
                return this._initSpec._backgroundFeats.BackgroundGradientAngle;
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

                return this._initSpec._actualColor;
            }
        }

        /// <summary>
        /// Gets the actual background color of the box
        /// </summary>
        public Color ActualBackgroundColor
        {
            get
            {
                return this._initSpec._backgroundFeats.BackgroundColor;
            }
        }
        /// <summary>
        /// Gets the font that should be actually used to paint the text of the box
        /// </summary>
        public Font ActualFont
        {
            get
            {
                //depend on parent
                if (_actualFont != null)
                {
                    return _actualFont;
                }
                //-----------------------------------------------------------------------------                
                _actualFont = this._initSpec._fontFeats.GetCacheFont(this.GetParent());
                return _actualFont;
            }
        }
        public int cssClassVersion
        {
            get { return this._initSpec.cssClassVersion; }
            set { this._initSpec.cssClassVersion = value; }
        }
        Font _actualFont;
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

        /// <summary>
        /// Ensures that the specified length is converted to pixels if necessary
        /// </summary>
        /// <param name="length"></param>
        public CssLength NoEms(CssLength length)
        {
            if (length.UnitOrNames == CssUnitOrNames.Ems)
            {
                return length.ConvertEmToPixels(GetEmHeight());
            }
            return length;
        }

    }
}