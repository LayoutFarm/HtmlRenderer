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

    /// <summary>
    /// Base class for spec box to handle the css properties.<br/>
    /// Has field and property for every css property that can be set, the properties add additional parsing like
    /// setting the correct border depending what border value was set (single, two , all four).<br/>
    /// </summary>
    public sealed partial class BoxSpec
    {


        internal int cssClassVersion;
        //==========================================================
        //css values Inherit From Parent (by default)  
        CssFontProp _fontProps = CssFontProp.Default;
        CssListProp _listProps = CssListProp.Default;

        CssLength _lineHeight = CssLength.NormalWordOrLine;
        CssLength _textIndent = CssLength.ZeroNoUnit;
        Color _actualColor = Color.Empty;//varies from one browser to another
        CssEmptyCell _emptyCells = CssEmptyCell.Show;
        CssTextAlign _textAlign = CssTextAlign.NotAssign;
        CssVerticalAlign _verticalAlign = CssVerticalAlign.Baseline;
        CssVisibility _visibility = CssVisibility.Visible;
        CssWhiteSpace _whitespace = CssWhiteSpace.Normal;
        CssWordBreak _wordBreak = CssWordBreak.Normal;
        CssDirection _cssDirection = CssDirection.Ltl;

        //==========================================================
        // css values Not Inherit From Parent
        CssBorderProp _borderProps = CssBorderProp.Default;
        CssPaddingProp _paddingProps = CssPaddingProp.Default;
        CssMarginProp _marginProps = CssMarginProp.Default;
        CssCornerProp _cornerProps = CssCornerProp.Default;
        //==========================================================

        Font _actualFont;
        CssBackgroundProp _backgroundProps = CssBackgroundProp.Default;

        CssDisplay _cssDisplay = CssDisplay.Inline;
        CssFloat _float = CssFloat.None;

        CssLength _left = CssLength.AutoLength;//w3 css 
        CssLength _top = CssLength.AutoLength;//w3 css 
        CssLength _right = CssLength.AutoLength;//w3 css 
        CssLength _bottom = CssLength.AutoLength;//w3 css 

        CssLength _width = CssLength.AutoLength;
        CssLength _height = CssLength.AutoLength;

        CssLength _maxWidth = CssLength.NotAssign; //w3 css  
        CssOverflow _overflow = CssOverflow.Visible;
        CssTextDecoration _textDecoration = CssTextDecoration.NotAssign;
        CssPosition _position = CssPosition.Static;
        CssLength _wordSpacing = CssLength.NormalWordOrLine;



#if DEBUG
        public readonly int dbugId = dbugTotalId++;
        static int dbugTotalId;
        public int dbugMark;
#endif

        public BoxSpec()
        {
            _actualColor = System.Drawing.Color.Black;
        }
      
        public void InheritStylesFrom(BoxSpec source)
        {
            this.InheritStyles(source, false);
        }
        public void CloneAllStylesFrom(BoxSpec source)
        {
            this.InheritStyles(source, true);
        }

        #region CSS Properties

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
            get { return this._borderProps.LeftWidth; }
            set
            {
                CheckBorderVersion().LeftWidth = value;
                //this._prop_pass_eval &= ~CssBoxBaseAssignments.BORDER_WIDTH_LEFT;
            }
        }

        public CssLength BorderRightWidth
        {
            get { return this._borderProps.RightWidth; }
            set
            {
                CheckBorderVersion().RightWidth = value;
                // this._prop_pass_eval &= ~CssBoxBaseAssignments.BORDER_WIDTH_RIGHT;
            }
        }

        public CssLength BorderBottomWidth
        {
            get { return this._borderProps.BottomWidth; }
            set
            {
                CheckBorderVersion().BottomWidth = value;
                //this._prop_pass_eval &= ~CssBoxBaseAssignments.BORDER_WIDTH_BOTTOM;
            }
        }

        public CssLength BorderTopWidth
        {
            get { return this._borderProps.TopWidth; }
            set
            {
                CheckBorderVersion().TopWidth = value;
                //this._prop_pass_eval &= ~CssBoxBaseAssignments.BORDER_WIDTH_TOP;
            }
        }
        //--------------------------------------------------------------------------------------
        public CssBorderStyle BorderTopStyle
        {

            get { return this._borderProps.TopStyle; }
            set { CheckBorderVersion().TopStyle = value; }

        }
        public CssBorderStyle BorderLeftStyle
        {
            get { return this._borderProps.LeftStyle; }
            set { CheckBorderVersion().LeftStyle = value; }
        }
        public CssBorderStyle BorderRightStyle
        {
            get { return this._borderProps.RightStyle; }
            set { CheckBorderVersion().RightStyle = value; }
        }

        public CssBorderStyle BorderBottomStyle
        {

            get { return this._borderProps.BottomStyle; }
            set { CheckBorderVersion().BottomStyle = value; }
        }

        //--------------------------------------------
        public Color BorderBottomColor
        {
            get { return this._borderProps.BottomColor; }
            set { CheckBorderVersion().BottomColor = value; }
        }
        public Color BorderLeftColor
        {
            get { return this._borderProps.LeftColor; }
            set { CheckBorderVersion().LeftColor = value; }
        }
        //--------------------------------------------
        public Color BorderRightColor
        {
            get { return this._borderProps.RightColor; }
            set { CheckBorderVersion().RightColor = value; }
        }

        public Color BorderTopColor
        {
            get { return this._borderProps.TopColor; }
            set { CheckBorderVersion().TopColor = value; }
        }
        public CssLength BorderSpacingVertical
        {
            get { return this._borderProps.BorderSpacingV; }
            set { CheckBorderVersion().BorderSpacingV = value; }
        }
        public CssLength BorderSpacingHorizontal
        {
            get { return this._borderProps.BorderSpacingH; }
            set { CheckBorderVersion().BorderSpacingH = value; }
        }
        public CssBorderCollapse BorderCollapse
        {
            get { return this._borderProps.BorderCollapse; }
            set { CheckBorderVersion().BorderCollapse = value; }
        }

        public bool IsBorderCollapse
        {
            get { return this.BorderCollapse == CssBorderCollapse.Collapse; }
        }
        //------------------------------------------------------
        public CssLength CornerNERadius
        {
            get { return this._cornerProps.NERadius; }
            set
            {
                CheckCornerVersion().NERadius = value;
            }
        }
        public CssLength CornerNWRadius
        {
            get { return this._cornerProps.NWRadius; }
            set
            {
                CheckCornerVersion().NWRadius = value;
            }
        }
        public CssLength CornerSERadius
        {
            get { return this._cornerProps.SERadius; }
            set
            {
                CheckCornerVersion().SERadius = value;
            }
        }
        public CssLength CornerSWRadius
        {
            get { return this._cornerProps.SWRadius; }
            set
            {
                CheckCornerVersion().SWRadius = value;
            }
        }
        //------------------------------------------------------
        public CssLength MarginBottom
        {
            get { return this._marginProps.Bottom; }
            set { CheckMarginVersion().Bottom = value; }
        }

        public CssLength MarginLeft
        {
            get { return this._marginProps.Left; }
            set { CheckMarginVersion().Left = value; }
        }

        public CssLength MarginRight
        {
            get { return this._marginProps.Right; }
            set { CheckMarginVersion().Right = value; }
        }

        public CssLength MarginTop
        {
            get { return this._marginProps.Top; }
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
            get { return this._backgroundProps.BackgroundColor; }
            set { CheckBgVersion().BackgroundColor = value; }
        }
        public ImageBinder BackgroundImageBinder
        {
            get { return this._backgroundProps.BackgroundImageBinder; }
            set { CheckBgVersion().BackgroundImageBinder = value; }
        }


        public CssLength BackgroundPositionX
        {
            get { return this._backgroundProps.BackgroundPosX; }
            set { CheckBgVersion().BackgroundPosX = value; }
        }
        public CssLength BackgroundPositionY
        {
            get { return this._backgroundProps.BackgroundPosY; }
            set { CheckBgVersion().BackgroundPosY = value; }
        }
        public CssBackgroundRepeat BackgroundRepeat
        {
            get { return this._backgroundProps.BackgroundRepeat; }
            set { CheckBgVersion().BackgroundRepeat = value; }
        }

        public Color BackgroundGradient
        {
            get { return this._backgroundProps.BackgroundGradient; }
            set { CheckBgVersion().BackgroundGradient = value; }
        }

        public float BackgroundGradientAngle
        {
            get { return this._backgroundProps.BackgroundGradientAngle; }
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
            {    //2014,
                //from www.w3c.org/wiki/Css/Properties/line-height

                //line height in <percentage> : 
                //The computed value if the property is percentage multiplied by the 
                //element's computed font size. 
                _lineHeight = value;
                //this._prop_pass_eval &= ~CssBoxBaseAssignments.LINE_HEIGHT;
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
            get { return this._fontProps.FontFamily; }
            set { CheckFontVersion().FontFamily = value; }
        }

        public CssLength FontSize
        {
            get { return this._fontProps.FontSize; }
            set { CheckFontVersion().FontSize = value; }
        }

        public CssFontStyle FontStyle
        {
            get { return this._fontProps.FontStyle; }
            set { CheckFontVersion().FontStyle = value; }
        }

        public CssFontVariant FontVariant
        {
            get { return this._fontProps.FontVariant; }
            set { CheckFontVersion().FontVariant = value; }
        }

        public CssFontWeight FontWeight
        {
            get { return this._fontProps.FontWeight; }
            set { CheckFontVersion().FontWeight = value; }
        }
        public CssOverflow Overflow
        {
            get { return this._overflow; }
            set { this._overflow = value; }
        }

        public string ListStyle
        {
            get { return this._listProps.ListStyle; }
            set { CheckListPropVersion().ListStyle = value; }
        }
        public CssListStylePosition ListStylePosition
        {
            get { return this._listProps.ListStylePosition; }
            set { CheckListPropVersion().ListStylePosition = value; }
        }
        public string ListStyleImage
        {
            get { return this._listProps.ListStyleImage; }
            set { CheckListPropVersion().ListStyleImage = value; }
        }

        public CssListStyleType ListStyleType
        {
            get { return this._listProps.ListStyleType; }
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
                return this._backgroundProps.BackgroundGradient;
            }
        }

        /// <summary>
        /// Gets the actual angle specified for the background gradient
        /// </summary>
        public float ActualBackgroundGradientAngle
        {
            get
            {
                return this._backgroundProps.BackgroundGradientAngle;
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
                return this._backgroundProps.BackgroundColor;
            }
        }
        public Font GetFont(BoxSpec parent)
        {
            //get font (compare with parent)
            //absolute font size be cached 
            //percentage font size can't be cached             

            if (_actualFont != null)
            {
                return _actualFont;
            }
            _actualFont = GetCacheFont(parent);
            return _actualFont;
        }
        Font GetCacheFont(BoxSpec parentBox)
        {
            throw new NotSupportedException();
            //if (this._cacheFont != null)
            //{
            //    return _cacheFont;
            //}
            ////---------------------------------------
            //string fontFam = this.FontFamily;
            //if (string.IsNullOrEmpty(FontFamily))
            //{
            //    fontFam = CssConstants.FontSerif;
            //}

            //CssLength fontsize = this.FontSize;
            //if (fontsize.IsEmpty)
            //{
            //    fontsize = CssLength.MakeFontSizePtUnit(CssConstants.FontSize);
            //}

            ////-----------------------------------------------------------------------------
            //FontStyle st = System.Drawing.FontStyle.Regular;
            //if (FontStyle == CssFontStyle.Italic || FontStyle == CssFontStyle.Oblique)
            //{
            //    st |= System.Drawing.FontStyle.Italic;
            //}

            //CssFontWeight fontWight = this.FontWeight;
            //if (fontWight != CssFontWeight.Normal &&
            //    fontWight != CssFontWeight.Lighter &&
            //    fontWight != CssFontWeight.NotAssign &&
            //    fontWight != CssFontWeight.Inherit)
            //{
            //    st |= System.Drawing.FontStyle.Bold;
            //}

            //float fsize = CssConstants.FontSize;
            //bool relateToParent = false;

            //if (fontsize.IsFontSizeName)
            //{
            //    switch (fontsize.UnitOrNames)
            //    {

            //        case CssUnitOrNames.FONTSIZE_MEDIUM:
            //            fsize = CssConstants.FontSize; break;
            //        case CssUnitOrNames.FONTSIZE_XX_SMALL:
            //            fsize = CssConstants.FontSize - 4; break;
            //        case CssUnitOrNames.FONTSIZE_X_SMALL:
            //            fsize = CssConstants.FontSize - 3; break;
            //        case CssUnitOrNames.FONTSIZE_LARGE:
            //            fsize = CssConstants.FontSize + 2; break;
            //        case CssUnitOrNames.FONTSIZE_X_LARGE:
            //            fsize = CssConstants.FontSize + 3; break;
            //        case CssUnitOrNames.FONTSIZE_XX_LARGE:
            //            fsize = CssConstants.FontSize + 4; break;
            //        case CssUnitOrNames.FONTSIZE_SMALLER:
            //            {
            //                relateToParent = true;
            //                float parentFontSize = CssConstants.FontSize;
            //                if (parentBox != null)
            //                {
            //                    parentFontSize = parentBox.ActualFont.Size;
            //                }
            //                fsize = parentFontSize - 2;

            //            } break;
            //        case CssUnitOrNames.FONTSIZE_LARGER:
            //            {
            //                relateToParent = true;
            //                float parentFontSize = CssConstants.FontSize;
            //                if (parentBox != null)
            //                {
            //                    parentFontSize = parentBox.ActualFont.Size;
            //                }
            //                fsize = parentFontSize + 2;

            //            } break;
            //        default:
            //            throw new NotSupportedException();
            //    }
            //}
            //else
            //{
            //    fsize = fontsize.Number;
            //}

            //if (fsize <= 1f)
            //{
            //    fsize = CssConstants.FontSize;
            //}

            //if (!relateToParent)
            //{
            //    return this._cacheFont = FontsUtils.GetCachedFont(fontFam, fsize, st);
            //}
            //else
            //{
            //    //not store to cache font
            //    return FontsUtils.GetCachedFont(fontFam, fsize, st);
            //}
        }

        /// <summary>
        /// Gets the height of the font in the specified units
        /// </summary>
        /// <returns></returns>
        public float GetEmHeight(BoxSpec parent)
        {
            return FontsUtils.GetFontHeight(GetFont(parent));
        }



    }
}