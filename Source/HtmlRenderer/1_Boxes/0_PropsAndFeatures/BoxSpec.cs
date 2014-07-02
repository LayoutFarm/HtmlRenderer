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
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{


    public sealed partial class BoxSpec
    {
        internal int cssClassVersion;

        //==========================================================
        #region css values Inherit From Parent (by default)
        //inherit from parent by default
        internal CssFontFeature _fontFeats = CssFontFeature.Default;
        internal CssListFeature _listFeats = CssListFeature.Default;
        internal CssLength _lineHeight = CssLength.NormalWordOrLine;
        internal CssLength _textIndent = CssLength.ZeroNoUnit;
        internal Color _actualColor = System.Drawing.Color.Black;
        internal CssEmptyCell _emptyCells = CssEmptyCell.Show;
        internal CssTextAlign _textAlign = CssTextAlign.NotAssign;
        internal CssVerticalAlign _verticalAlign = CssVerticalAlign.Baseline;
        internal CssVisibility _visibility = CssVisibility.Visible;
        internal CssWhiteSpace _whitespace = CssWhiteSpace.Normal;
        internal CssWordBreak _wordBreak = CssWordBreak.Normal;
        internal CssDirection _cssDirection = CssDirection.Ltl;

        #endregion
        //==========================================================
        #region css values Not Inherit From Parent
        internal CssBorderFeature _borderFeats = CssBorderFeature.Default;
        internal CssPaddingFeature _paddingProps = CssPaddingFeature.Default;
        internal CssMarginFeature _marginFeats = CssMarginFeature.Default;
        internal CssCornerFeature _cornerFeats = CssCornerFeature.Default;
        internal Font _actualFont;
        internal CssBackgroundFeature _backgroundFeats = CssBackgroundFeature.Default;

        internal CssDisplay _cssDisplay = CssDisplay.Inline;
        internal CssFloat _float = CssFloat.None;
        //==========================================================
        internal CssLength _left = CssLength.AutoLength;//w3 css 
        internal CssLength _top = CssLength.AutoLength;//w3 css 
        internal CssLength _right = CssLength.AutoLength;//w3 css 
        internal CssLength _bottom = CssLength.AutoLength;//w3 css 

        internal CssLength _width = CssLength.AutoLength;
        internal CssLength _height = CssLength.AutoLength;
        //==========================================================
        internal CssLength _maxWidth = CssLength.NotAssign; //w3 css  
        internal CssOverflow _overflow = CssOverflow.Visible;
        internal CssTextDecoration _textDecoration = CssTextDecoration.NotAssign;
        internal CssPosition _position = CssPosition.Static;
        internal CssLength _wordSpacing = CssLength.NormalWordOrLine;
        //==========================================================
        internal WellknownHtmlTagName wellKnownTagName;
        #endregion
#if DEBUG
        public readonly int dbugId = dbugTotalId++;
        static int dbugTotalId;
        public int dbugMark;
#endif

        #region CSS Properties

        BoxSpec anonVersion;

        public BoxSpec(WellknownHtmlTagName wellknownTagName)
        {
            this.WellknownTagName = wellknownTagName;
        }
        internal BoxSpec(BridgeHtmlElement ownerElement)// WellknownHtmlTagName wellknownTagName)
        {

            this.WellknownTagName = ownerElement.WellknownTagName;
        }

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
            get { return this._cssDisplay; }
            set
            {
                this._cssDisplay = value;
            }
        }
        public CssDirection CssDirection
        {
            get { return this._cssDirection; }
            set { this._cssDirection = value; }
        }
        //--------------------------------------------------------------------------------------
        public CssLength BorderLeftWidth
        {
            get { return this._borderFeats.LeftWidth; }
            set
            {
                CheckBorderVersion().LeftWidth = value;
                //this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_LEFT;
            }
        }

        public CssLength BorderRightWidth
        {
            get { return this._borderFeats.RightWidth; }
            set
            {
                CheckBorderVersion().RightWidth = value;
                // this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_RIGHT;
            }
        }

        public CssLength BorderBottomWidth
        {
            get { return this._borderFeats.BottomWidth; }
            set
            {
                CheckBorderVersion().BottomWidth = value;
                //this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_BOTTOM;
            }
        }

        public CssLength BorderTopWidth
        {
            get { return this._borderFeats.TopWidth; }
            set
            {
                CheckBorderVersion().TopWidth = value;
                //this._prop_pass_eval &= ~CssBoxAssignments.BORDER_WIDTH_TOP;
            }
        }
        //--------------------------------------------------------------------------------------
        public CssBorderStyle BorderTopStyle
        {

            get { return this._borderFeats.TopStyle; }
            set { CheckBorderVersion().TopStyle = value; }

        }
        public CssBorderStyle BorderLeftStyle
        {
            get { return this._borderFeats.LeftStyle; }
            set { CheckBorderVersion().LeftStyle = value; }
        }
        public CssBorderStyle BorderRightStyle
        {
            get { return this._borderFeats.RightStyle; }
            set { CheckBorderVersion().RightStyle = value; }
        }

        public CssBorderStyle BorderBottomStyle
        {

            get { return this._borderFeats.BottomStyle; }
            set { CheckBorderVersion().BottomStyle = value; }
        }

        //--------------------------------------------
        public Color BorderBottomColor
        {
            get { return this._borderFeats.BottomColor; }
            set { CheckBorderVersion().BottomColor = value; }
        }
        public Color BorderLeftColor
        {
            get { return this._borderFeats.LeftColor; }
            set { CheckBorderVersion().LeftColor = value; }
        }
        //--------------------------------------------
        public Color BorderRightColor
        {
            get { return this._borderFeats.RightColor; }
            set { CheckBorderVersion().RightColor = value; }
        }

        public Color BorderTopColor
        {
            get { return this._borderFeats.TopColor; }
            set { CheckBorderVersion().TopColor = value; }
        }
        public CssLength BorderSpacingVertical
        {
            get { return this._borderFeats.BorderSpacingV; }
            set { CheckBorderVersion().BorderSpacingV = value; }
        }
        public CssLength BorderSpacingHorizontal
        {
            get { return this._borderFeats.BorderSpacingH; }
            set { CheckBorderVersion().BorderSpacingH = value; }
        }
        public CssBorderCollapse BorderCollapse
        {
            get { return this._borderFeats.BorderCollapse; }
            set { CheckBorderVersion().BorderCollapse = value; }
        }

        public bool IsBorderCollapse
        {
            get { return this.BorderCollapse == CssBorderCollapse.Collapse; }
        }
        //------------------------------------------------------
        public CssLength CornerNERadius
        {
            get { return this._cornerFeats.NERadius; }
            set
            {
                CheckCornerVersion().NERadius = value;
            }
        }
        public CssLength CornerNWRadius
        {
            get { return this._cornerFeats.NWRadius; }
            set
            {
                CheckCornerVersion().NWRadius = value;
            }
        }
        public CssLength CornerSERadius
        {
            get { return this._cornerFeats.SERadius; }
            set
            {
                CheckCornerVersion().SERadius = value;
            }
        }
        public CssLength CornerSWRadius
        {
            get { return this._cornerFeats.SWRadius; }
            set
            {
                CheckCornerVersion().SWRadius = value;
            }
        }
        //------------------------------------------------------
        public CssLength MarginBottom
        {
            get { return this._marginFeats.Bottom; }
            set { CheckMarginVersion().Bottom = value; }
        }

        public CssLength MarginLeft
        {
            get { return this._marginFeats.Left; }
            set { CheckMarginVersion().Left = value; }
        }

        public CssLength MarginRight
        {
            get { return this._marginFeats.Right; }
            set { CheckMarginVersion().Right = value; }
        }

        public CssLength MarginTop
        {
            get { return this._marginFeats.Top; }
            set { CheckMarginVersion().Top = value; }
        }

        public CssLength PaddingBottom
        {
            get { return this._paddingProps.Bottom; }
            set
            {
                CheckPaddingVersion().Bottom = value;
            }
        }

        public CssLength PaddingLeft
        {
            get { return this._paddingProps.Left; }
            set
            {
                CheckPaddingVersion().Left = value;
            }
        }

        public CssLength PaddingRight
        {
            get { return this._paddingProps.Right; }
            set
            {
                CheckPaddingVersion().Right = value;

            }
        }

        public CssLength PaddingTop
        {
            get
            {
                return this._paddingProps.Top;
            }
            set
            {
                CheckPaddingVersion().Top = value;
            }
        }
        public CssLength Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public CssLength Top
        {
            get { return _top; }
            set { _top = value; }
        }

        public CssLength Width
        {
            get { return this._width; }
            set { this._width = value; }
        }
        public CssLength MaxWidth
        {
            get { return _maxWidth; }
            set { _maxWidth = value; }
        }
        public CssLength Height
        {
            get { return this._height; }
            set { this._height = value; }
        }
        public Color BackgroundColor
        {
            get { return this._backgroundFeats.BackgroundColor; }
            set { CheckBgVersion().BackgroundColor = value; }
        }
        public ImageBinder BackgroundImageBinder
        {
            get { return this._backgroundFeats.BackgroundImageBinder; }
            set { CheckBgVersion().BackgroundImageBinder = value; }
        }


        public CssLength BackgroundPositionX
        {
            get { return this._backgroundFeats.BackgroundPosX; }
            set { CheckBgVersion().BackgroundPosX = value; }
        }
        public CssLength BackgroundPositionY
        {
            get { return this._backgroundFeats.BackgroundPosY; }
            set { CheckBgVersion().BackgroundPosY = value; }
        }
        public CssBackgroundRepeat BackgroundRepeat
        {
            get { return this._backgroundFeats.BackgroundRepeat; }
            set { CheckBgVersion().BackgroundRepeat = value; }
        }

        public Color BackgroundGradient
        {
            get { return this._backgroundFeats.BackgroundGradient; }
            set { CheckBgVersion().BackgroundGradient = value; }
        }

        public float BackgroundGradientAngle
        {
            get { return this._backgroundFeats.BackgroundGradientAngle; }
            set { CheckBgVersion().BackgroundGradientAngle = value; }
        }
        /// <summary>
        /// font color
        /// </summary>
        public Color Color
        {
            get { return _actualColor; }
            set { _actualColor = value; }
        }
        public CssEmptyCell EmptyCells
        {
            get { return _emptyCells; }
            set { _emptyCells = value; }
        }

        public CssFloat Float
        {
            get { return _float; }
            set { _float = value; }
        }
        public CssPosition Position
        {
            get { return this._position; }
            set { this._position = value; }
        }


        //----------------------------------------------------
        public CssLength LineHeight
        {
            get { return _lineHeight; }
            set
            {
                _lineHeight = value;
                // this._prop_pass_eval &= ~CssBoxAssignments.LINE_HEIGHT;
            }
        }
        public CssVerticalAlign VerticalAlign
        {
            get { return this._verticalAlign; }
            set { this._verticalAlign = value; }
        }
        public CssLength TextIndent
        {
            get { return _textIndent; }
            set { _textIndent = value; }
        }
        public CssTextAlign CssTextAlign
        {
            get { return this._textAlign; }
            set { this._textAlign = value; }
        }

        public CssTextDecoration TextDecoration
        {
            get { return _textDecoration; }
            set { _textDecoration = value; }
        }

        //-----------------------------------
        public CssWhiteSpace WhiteSpace
        {
            get { return this._whitespace; }
            set { this._whitespace = value; }
        }
        //----------------------------------- 
        public CssVisibility CssVisibility
        {
            get { return this._visibility; }
            set { this._visibility = value; }
        }
        public CssLength WordSpacing
        {
            get { return this._wordSpacing; }
            set { this._wordSpacing = value; }
        }

        public CssWordBreak WordBreak
        {
            get { return this._wordBreak; }
            set { this._wordBreak = value; }
        }

        public string FontFamily
        {
            get { return this._fontFeats.FontFamily; }
            set { CheckFontVersion().FontFamily = value; }
        }

        public CssLength FontSize
        {
            get { return this._fontFeats.FontSize; }
            set
            {
                CheckFontVersion().FontSize = value;
            }
        }

        public CssFontStyle FontStyle
        {
            get { return this._fontFeats.FontStyle; }
            set { CheckFontVersion().FontStyle = value; }
        }

        public CssFontVariant FontVariant
        {
            get { return this._fontFeats.FontVariant; }
            set { CheckFontVersion().FontVariant = value; }
        }

        public CssFontWeight FontWeight
        {
            get { return this._fontFeats.FontWeight; }
            set { CheckFontVersion().FontWeight = value; }
        }
        public CssOverflow Overflow
        {
            get { return this._overflow; }
            set { this._overflow = value; }
        }

        public string ListStyle
        {
            get { return this._listFeats.ListStyle; }
            set { CheckListPropVersion().ListStyle = value; }
        }
        public CssListStylePosition ListStylePosition
        {
            get { return this._listFeats.ListStylePosition; }
            set { CheckListPropVersion().ListStylePosition = value; }
        }
        public string ListStyleImage
        {
            get { return this._listFeats.ListStyleImage; }
            set { CheckListPropVersion().ListStyleImage = value; }
        }

        public CssListStyleType ListStyleType
        {
            get { return this._listFeats.ListStyleType; }
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
                return this._backgroundFeats.BackgroundGradient;
            }
        }

        /// <summary>
        /// Gets the actual angle specified for the background gradient
        /// </summary>
        public float ActualBackgroundGradientAngle
        {
            get
            {
                return this._backgroundFeats.BackgroundGradientAngle;
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

                return _actualColor;
            }
        }

        /// <summary>
        /// Gets the actual background color of the box
        /// </summary>
        public Color ActualBackgroundColor
        {
            get
            {
                return this._backgroundFeats.BackgroundColor;
            }
        }

        public Font GetFont(BoxSpec parentBox)
        {

            //---------------------------------------
            if (_actualFont != null)
            {
                return _actualFont;
            }
            //---------------------------------------
            bool relateToParent = false;

            string fontFam = this.FontFamily;
            if (string.IsNullOrEmpty(FontFamily))
            {
                fontFam = ConstConfig.DEFAULT_FONT_NAME;
            }


            //-----------------------------------------------------------------------------
            //style
            FontStyle st = System.Drawing.FontStyle.Regular;
            switch (FontStyle)
            {
                case CssFontStyle.Italic:
                case CssFontStyle.Oblique:
                    st |= System.Drawing.FontStyle.Italic;
                    break;
            }
            //-----------------------------------------------------
            //weight
            CssFontWeight fontWight = this.FontWeight;
            switch (this.FontWeight)
            {
                case CssFontWeight.Normal:
                case CssFontWeight.Lighter:
                case CssFontWeight.NotAssign:
                case CssFontWeight.Inherit:
                    {
                        //do nothing
                    } break;
                default:
                    {
                        st |= System.Drawing.FontStyle.Bold;
                    } break;
            }

            CssLength fontsize = this.FontSize;
            if (fontsize.IsEmpty || fontsize.Number == 0)
            {
                fontsize = CssLength.MakeFontSizePtUnit(ConstConfig.DEFAULT_FONT_SIZE);
            }

            float fsize = ConstConfig.DEFAULT_FONT_SIZE;

            if (fontsize.IsFontSizeName)
            {
                switch (fontsize.UnitOrNames)
                {
                    case CssUnitOrNames.FONTSIZE_MEDIUM:
                        fsize = ConstConfig.DEFAULT_FONT_SIZE; break;
                    case CssUnitOrNames.FONTSIZE_XX_SMALL:
                        fsize = ConstConfig.DEFAULT_FONT_SIZE - 4; break;
                    case CssUnitOrNames.FONTSIZE_X_SMALL:
                        fsize = ConstConfig.DEFAULT_FONT_SIZE - 3; break;
                    case CssUnitOrNames.FONTSIZE_LARGE:
                        fsize = ConstConfig.DEFAULT_FONT_SIZE + 2; break;
                    case CssUnitOrNames.FONTSIZE_X_LARGE:
                        fsize = ConstConfig.DEFAULT_FONT_SIZE + 3; break;
                    case CssUnitOrNames.FONTSIZE_XX_LARGE:
                        fsize = ConstConfig.DEFAULT_FONT_SIZE + 4; break;
                    case CssUnitOrNames.FONTSIZE_SMALLER:
                        {
                            relateToParent = true;
                            float parentFontSize = ConstConfig.DEFAULT_FONT_SIZE;
                            if (parentBox != null)
                            {
                                parentFontSize = parentBox._actualFont.Size;
                            }
                            fsize = parentFontSize - 2;

                        } break;
                    case CssUnitOrNames.FONTSIZE_LARGER:
                        {
                            relateToParent = true;
                            float parentFontSize = ConstConfig.DEFAULT_FONT_SIZE;
                            if (parentBox != null)
                            {
                                parentFontSize = parentBox._actualFont.Size;
                            }
                            fsize = parentFontSize + 2;

                        } break;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                fsize = fontsize.Number;
            }

            if (fontsize.UnitOrNames == CssUnitOrNames.Ems)
            {
                fsize = fontsize.Number * parentBox.FontSize.Number;
            }

            if (fsize <= 1f)
            {
                fsize = ConstConfig.DEFAULT_FONT_SIZE;
            }

            if (!relateToParent)
            {

                return FontsUtils.GetCachedFont(fontFam, fsize, st);
            }
            else
            {
                //not store to cache font
                return this._actualFont = FontsUtils.GetCachedFont(fontFam, fsize, st);
            }
        }

        /// <summary>
        /// Gets the height of the font in the specified units
        /// </summary>
        /// <returns></returns>
        public float GetEmHeight(BoxSpec parent)
        {
            return FontsUtils.GetFontHeight(GetFont(parent));
        }
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


    }


}