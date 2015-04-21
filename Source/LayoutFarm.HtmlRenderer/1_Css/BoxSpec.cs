// 2015,2014 ,BSD, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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
using PixelFarm.Drawing;

namespace LayoutFarm.Css
{


    public sealed partial class BoxSpec
    {

        bool _freezed;

        //==========================================================
        #region css values Inherit From Parent (by default)
        //inherit from parent by default
        CssFontFeature _fontFeats = CssFontFeature.Default; //features 1
        CssListFeature _listFeats = CssListFeature.Default; //features 2
        CssLength _lineHeight = CssLength.NormalWordOrLine;
        CssLength _textIndent = CssLength.ZeroNoUnit;
        Color _actualColor = Color.Black;
        CssEmptyCell _emptyCells = CssEmptyCell.Show;
        CssTextAlign _textAlign = CssTextAlign.NotAssign;
        CssVerticalAlign _verticalAlign = CssVerticalAlign.Baseline;
        CssVisibility _visibility = CssVisibility.Visible;
        CssWhiteSpace _whitespace = CssWhiteSpace.Normal;
        CssWordBreak _wordBreak = CssWordBreak.Normal;
        CssDirection _cssDirection = CssDirection.Ltl;
        #endregion


        //==========================================================
        #region css values Not Inherit From Parent
        CssBorderFeature _borderFeats = CssBorderFeature.Default; //features 3
        CssPaddingFeature _paddingFeats = CssPaddingFeature.Default;//features 4
        CssMarginFeature _marginFeats = CssMarginFeature.Default;//features 5
        CssCornerFeature _cornerFeats = CssCornerFeature.Default;//features  6      
        CssBackgroundFeature _backgroundFeats = CssBackgroundFeature.Default;//features  7 
        CssBoxShadowFeature _boxShadow = CssBoxShadowFeature.Default;
        CssFlexFeature _flexFeats = CssFlexFeature.Default;

        FontInfo _actualFontInfo;
        Font _actualFont;

        CssDisplay _cssDisplay = CssDisplay.Inline;
        CssFloat _float = CssFloat.None;
        //==========================================================
        CssLength _left = CssLength.AutoLength;//w3 css 
        CssLength _top = CssLength.AutoLength;//w3 css 
        CssLength _right = CssLength.AutoLength;//w3 css 
        CssLength _bottom = CssLength.AutoLength;//w3 css 

        CssLength _width = CssLength.AutoLength;
        CssLength _height = CssLength.AutoLength;
        //==========================================================
        CssLength _maxWidth = CssLength.NotAssign; //w3 css  
        CssOverflow _overflow = CssOverflow.Visible;
        CssTextDecoration _textDecoration = CssTextDecoration.NotAssign;
        CssPosition _position = CssPosition.Static;
        CssLength _wordSpacing = CssLength.NormalWordOrLine;
        //==========================================================


        #endregion
#if DEBUG
        public readonly int __aa_dbugId = dbugTotalId++;
        static int dbugTotalId;
        public int dbugMark;
#endif

        #region CSS Properties

        //------------------
        BoxSpec anonVersion;
        //------------------
        public BoxSpec()
        {
            //if (this.__aa_dbugId == 8)
            //{
            //}
        }
        public bool IsTemplate { get; set; }
        public void Freeze()
        {
            this._freezed = true;
            _fontFeats.Freeze(); //1.
            _listFeats.Freeze(); //2. 

            _borderFeats.Freeze();//3.
            _paddingFeats.Freeze();//4.
            _marginFeats.Freeze();//5.
            _cornerFeats.Freeze();//6.
            _backgroundFeats.Freeze();//7   

            _boxShadow.Freeze(); //8
            _flexFeats.Freeze(); //9.
        }

        public bool DoNotCache
        {
            get;
            set;
        }
        public void Defreeze()
        {
            this._freezed = false;
            _fontFeats.DeFreeze(); //1.
            _listFeats.DeFreeze(); //2. 

            _borderFeats.DeFreeze();//3.
            _paddingFeats.DeFreeze();//4.
            _marginFeats.DeFreeze();//5.
            _cornerFeats.DeFreeze();//6.
            _backgroundFeats.DeFreeze();//7   

            _boxShadow.DeFreeze(); //8
            _flexFeats.DeFreeze(); //9.
        }
        public bool IsFreezed
        {
            get { return this._freezed; }
        }
        bool Assignable()
        {
#if DEBUG
            if (_freezed)
            {
                //throw new NotSupportedException();
            }
#endif
            return !_freezed;
        }
        //---------------------------------------------------------------

        public CssDisplay CssDisplay
        {
            get { return this._cssDisplay; }
            set
            {
                //if (value == Css.CssDisplay.Flex)
                //{
                //}
                //if (this.__aa_dbugId == 8)
                //{
                //}
                if (Assignable()) { this._cssDisplay = value; }
            }
        }
        public CssDirection CssDirection
        {
            get { return this._cssDirection; }
            set { if (Assignable())this._cssDirection = value; }
        }
        //--------------------------------------------------------------------------------------
        public CssLength BorderLeftWidth
        {
            get { return this._borderFeats.LeftWidth; }
            set { if (Assignable()) CheckBorderVersion().LeftWidth = value; }
        }

        public CssLength BorderRightWidth
        {
            get { return this._borderFeats.RightWidth; }
            set
            {
                if (Assignable()) CheckBorderVersion().RightWidth = value;
            }
        }

        public CssLength BorderBottomWidth
        {
            get { return this._borderFeats.BottomWidth; }
            set
            {
                if (Assignable()) CheckBorderVersion().BottomWidth = value;

            }
        }

        public CssLength BorderTopWidth
        {
            get { return this._borderFeats.TopWidth; }
            set
            {
                if (Assignable()) CheckBorderVersion().TopWidth = value;

            }
        }
        //--------------------------------------------------------------------------------------
        public CssBorderStyle BorderTopStyle
        {
            get { return this._borderFeats.TopStyle; }
            set { if (Assignable()) CheckBorderVersion().TopStyle = value; }
        }
        public CssBorderStyle BorderLeftStyle
        {
            get { return this._borderFeats.LeftStyle; }
            set
            {
                if (Assignable()) CheckBorderVersion().LeftStyle = value;
            }
        }
        public CssBorderStyle BorderRightStyle
        {
            get { return this._borderFeats.RightStyle; }
            set { if (Assignable()) CheckBorderVersion().RightStyle = value; }
        }

        public CssBorderStyle BorderBottomStyle
        {

            get { return this._borderFeats.BottomStyle; }
            set { if (Assignable()) CheckBorderVersion().BottomStyle = value; }
        }

        //--------------------------------------------
        public Color BorderBottomColor
        {
            get { return this._borderFeats.BottomColor; }
            set { if (Assignable()) CheckBorderVersion().BottomColor = value; }
        }
        public Color BorderLeftColor
        {
            get { return this._borderFeats.LeftColor; }
            set { if (Assignable()) CheckBorderVersion().LeftColor = value; }
        }
        //--------------------------------------------
        public Color BorderRightColor
        {
            get { return this._borderFeats.RightColor; }
            set
            {
                if (Assignable()) CheckBorderVersion().RightColor = value;
            }

        }

        public Color BorderTopColor
        {
            get { return this._borderFeats.TopColor; }
            set
            {
                if (Assignable()) CheckBorderVersion().TopColor = value;
            }
        }
        public CssLength BorderSpacingVertical
        {
            get { return this._borderFeats.BorderSpacingV; }
            set { if (Assignable()) CheckBorderVersion().BorderSpacingV = value; }
        }
        public CssLength BorderSpacingHorizontal
        {
            get { return this._borderFeats.BorderSpacingH; }
            set { if (Assignable()) CheckBorderVersion().BorderSpacingH = value; }
        }
        public CssBorderCollapse BorderCollapse
        {
            get { return this._borderFeats.BorderCollapse; }
            set { if (Assignable()) CheckBorderVersion().BorderCollapse = value; }
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
                if (Assignable()) CheckCornerVersion().NERadius = value;
            }
        }
        public CssLength CornerNWRadius
        {
            get { return this._cornerFeats.NWRadius; }
            set
            {
                if (Assignable()) CheckCornerVersion().NWRadius = value;
            }
        }
        public CssLength CornerSERadius
        {
            get { return this._cornerFeats.SERadius; }
            set
            {
                if (Assignable()) CheckCornerVersion().SERadius = value;
            }
        }
        public CssLength CornerSWRadius
        {
            get { return this._cornerFeats.SWRadius; }
            set
            {
                if (Assignable()) CheckCornerVersion().SWRadius = value;
            }
        }
        //------------------------------------------------------
        public CssLength MarginBottom
        {
            get { return this._marginFeats.Bottom; }
            set { if (Assignable()) CheckMarginVersion().Bottom = value; }
        }

        public CssLength MarginLeft
        {
            get { return this._marginFeats.Left; }
            set { if (Assignable())  CheckMarginVersion().Left = value; }
        }

        public CssLength MarginRight
        {
            get { return this._marginFeats.Right; }
            set { if (Assignable()) CheckMarginVersion().Right = value; }
        }

        public CssLength MarginTop
        {
            get { return this._marginFeats.Top; }
            set { if (Assignable()) CheckMarginVersion().Top = value; }
        }

        public CssLength PaddingBottom
        {
            get { return this._paddingFeats.Bottom; }
            set
            {
                if (Assignable()) CheckPaddingVersion().Bottom = value;
            }
        }

        public CssLength PaddingLeft
        {
            get { return this._paddingFeats.Left; }
            set
            {
                if (Assignable()) CheckPaddingVersion().Left = value;
            }
        }

        public CssLength PaddingRight
        {
            get { return this._paddingFeats.Right; }
            set
            {
                if (Assignable()) CheckPaddingVersion().Right = value;
            }
        }

        public CssLength PaddingTop
        {
            get
            {
                return this._paddingFeats.Top;
            }
            set
            {
                if (Assignable()) CheckPaddingVersion().Top = value;
            }
        }
        public CssLength Left
        {
            get { return _left; }
            set { if (Assignable()) _left = value; }
        }

        public CssLength Top
        {
            get { return _top; }
            set { if (Assignable()) _top = value; }
        }

        public CssLength Width
        {
            get { return this._width; }
            set { if (Assignable()) this._width = value; }
        }
        public CssLength MaxWidth
        {
            get { return _maxWidth; }
            set { if (Assignable()) _maxWidth = value; }
        }
        public CssLength Height
        {
            get { return this._height; }
            set { if (Assignable()) this._height = value; }
        }
        public Color BackgroundColor
        {
            get { return this._backgroundFeats.BackgroundColor; }
            set { if (Assignable()) this.CheckBgVersion().BackgroundColor = value; }

        }
        public ImageBinder BackgroundImageBinder
        {
            get { return this._backgroundFeats.BackgroundImageBinder; }
            set { if (Assignable()) CheckBgVersion().BackgroundImageBinder = value; }
        }
        public CssLength BackgroundPositionX
        {
            get { return this._backgroundFeats.BackgroundPosX; }
            set { if (Assignable()) CheckBgVersion().BackgroundPosX = value; }
        }
        public CssLength BackgroundPositionY
        {
            get { return this._backgroundFeats.BackgroundPosY; }
            set { if (Assignable()) CheckBgVersion().BackgroundPosY = value; }
        }
        public CssBackgroundRepeat BackgroundRepeat
        {
            get { return this._backgroundFeats.BackgroundRepeat; }
            set { if (Assignable()) CheckBgVersion().BackgroundRepeat = value; }
        }

        public Color BackgroundGradient
        {
            get { return this._backgroundFeats.BackgroundGradient; }
            set { if (Assignable()) CheckBgVersion().BackgroundGradient = value; }
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
            set { if (Assignable()) _actualColor = value; }
        }
        public CssEmptyCell EmptyCells
        {
            get { return _emptyCells; }
            set { if (Assignable()) _emptyCells = value; }
        }

        public CssFloat Float
        {
            get { return _float; }
            set { if (Assignable()) _float = value; }
        }
        public CssPosition Position
        {
            get { return this._position; }
            set { if (Assignable()) this._position = value; }
        }


        //----------------------------------------------------
        public CssLength LineHeight
        {
            get { return _lineHeight; }
            set
            {
                if (Assignable()) _lineHeight = value;
            }
        }
        public CssVerticalAlign VerticalAlign
        {
            get { return this._verticalAlign; }
            set { if (Assignable()) this._verticalAlign = value; }
        }
        public CssLength TextIndent
        {
            get { return _textIndent; }
            set { if (Assignable()) _textIndent = value; }
        }
        public CssTextAlign CssTextAlign
        {
            get { return this._textAlign; }
            set { if (Assignable()) this._textAlign = value; }
        }

        public CssTextDecoration TextDecoration
        {
            get { return _textDecoration; }
            set { if (Assignable()) _textDecoration = value; }
        }

        //-----------------------------------
        public CssWhiteSpace WhiteSpace
        {
            get { return this._whitespace; }
            set { if (Assignable()) this._whitespace = value; }
        }
        //----------------------------------- 
        public CssVisibility Visibility
        {
            get { return this._visibility; }
            set { if (Assignable()) this._visibility = value; }
        }
        public CssLength WordSpacing
        {
            get { return this._wordSpacing; }
            set { if (Assignable()) this._wordSpacing = value; }
        }

        public CssWordBreak WordBreak
        {
            get { return this._wordBreak; }
            set { if (Assignable()) this._wordBreak = value; }
        }

        public string FontFamily
        {
            get { return this._fontFeats.FontFamily; }
            set { if (Assignable()) CheckFontVersion().FontFamily = value; }
        }

        public CssLength FontSize
        {
            get { return this._fontFeats.FontSize; }
            set
            {
                if (Assignable()) CheckFontVersion().FontSize = value;
            }
        }

        public CssFontStyle FontStyle
        {
            get { return this._fontFeats.FontStyle; }
            set { if (Assignable()) CheckFontVersion().FontStyle = value; }
        }

        public CssFontVariant FontVariant
        {
            get { return this._fontFeats.FontVariant; }
            set { if (Assignable())  CheckFontVersion().FontVariant = value; }
        }

        public CssFontWeight FontWeight
        {
            get { return this._fontFeats.FontWeight; }
            set { if (Assignable()) CheckFontVersion().FontWeight = value; }
        }
        public CssOverflow Overflow
        {
            get { return this._overflow; }
            set { if (Assignable()) this._overflow = value; }
        }


        public CssListStylePosition ListStylePosition
        {
            get { return this._listFeats.ListStylePosition; }
            set { if (Assignable()) CheckListPropVersion().ListStylePosition = value; }
        }

        public string ListStyle
        {
            get { return this._listFeats.ListStyle; }
            set { if (Assignable()) CheckListPropVersion().ListStyle = value; }
        }
        public string ListStyleImage
        {
            get { return this._listFeats.ListStyleImage; }
            set { if (Assignable()) CheckListPropVersion().ListStyleImage = value; }
        }

        public CssListStyleType ListStyleType
        {
            get { return this._listFeats.ListStyleType; }
            set { if (Assignable()) CheckListPropVersion().ListStyleType = value; }
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
            set
            {
                if (Assignable()) this._backgroundFeats.BackgroundGradientAngle = value;
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
        internal FontInfo GetFontInfo(IFonts ifonts, float parentFontSize)
        {

            //---------------------------------------
            if (_actualFontInfo != null)
            {
                return this._actualFontInfo;
            }
            //---------------------------------------
            bool relateToParent = false;
            string fontFam = this.FontFamily;
            if (string.IsNullOrEmpty(FontFamily))
            {
                fontFam = FontDefaultConfig.DEFAULT_FONT_NAME;
            }

            //-----------------------------------------------------------------------------
            //style
            FontStyle st = PixelFarm.Drawing.FontStyle.Regular;
            switch (FontStyle)
            {
                case CssFontStyle.Italic:
                case CssFontStyle.Oblique:
                    st |= PixelFarm.Drawing.FontStyle.Italic;
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
                        st |= PixelFarm.Drawing.FontStyle.Bold;
                    } break;
            }

            CssLength fontsize = this.FontSize;
            if (fontsize.IsEmpty || fontsize.Number == 0)
            {
                fontsize = CssLength.MakeFontSizePtUnit(FontDefaultConfig.DEFAULT_FONT_SIZE);
            }

            float fsize = FontDefaultConfig.DEFAULT_FONT_SIZE;

            if (fontsize.IsFontSizeName)
            {
                switch (fontsize.UnitOrNames)
                {
                    case CssUnitOrNames.FONTSIZE_MEDIUM:
                        fsize = FontDefaultConfig.DEFAULT_FONT_SIZE; break;
                    case CssUnitOrNames.FONTSIZE_XX_SMALL:
                        fsize = FontDefaultConfig.DEFAULT_FONT_SIZE - 4; break;
                    case CssUnitOrNames.FONTSIZE_X_SMALL:
                        fsize = FontDefaultConfig.DEFAULT_FONT_SIZE - 3; break;
                    case CssUnitOrNames.FONTSIZE_LARGE:
                        fsize = FontDefaultConfig.DEFAULT_FONT_SIZE + 2; break;
                    case CssUnitOrNames.FONTSIZE_X_LARGE:
                        fsize = FontDefaultConfig.DEFAULT_FONT_SIZE + 3; break;
                    case CssUnitOrNames.FONTSIZE_XX_LARGE:
                        fsize = FontDefaultConfig.DEFAULT_FONT_SIZE + 4; break;
                    case CssUnitOrNames.FONTSIZE_SMALLER:
                        {
                            relateToParent = true;
                            //float parentFontSize = ConstConfig.DEFAULT_FONT_SIZE;
                            //if (parentBox != null)
                            //{
                            //    parentFontSize = parentBox._actualFont.Size;
                            //}
                            fsize = parentFontSize - 2;

                        } break;
                    case CssUnitOrNames.FONTSIZE_LARGER:
                        {
                            relateToParent = true;
                            //float parentFontSize = ConstConfig.DEFAULT_FONT_SIZE;
                            //if (parentBox != null)
                            //{
                            //    parentFontSize = parentBox._actualFont.Size;
                            //}
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
                fsize = fontsize.Number * FontDefaultConfig.DEFAULT_FONT_SIZE;
                relateToParent = true;
            }
            if (fsize <= 1f)
            {
                fsize = FontDefaultConfig.DEFAULT_FONT_SIZE;
            }

            FontInfo fontInfo = ifonts.GetFontInfo(fontFam, fsize, st);
            if (!relateToParent)
            {
                //cahce value
                this._actualFont = fontInfo.ResolvedFont;
                this._actualFontInfo = fontInfo;
            }
            return fontInfo;
        }


        //----------------------------------------------------------------------
        public bool HasBoxShadow
        {
            get { return this._boxShadow != CssBoxShadowFeature.Default; }
        }
        public CssLength BoxShadowHOffset
        {
            get { return this._boxShadow.HOffset; }
            set
            {
                if (Assignable()) CheckBoxShadowVersion().HOffset = value;
            }
        }
        public CssLength BoxShadowVOffset
        {
            get { return this._boxShadow.VOffset; }
            set
            {
                if (Assignable()) CheckBoxShadowVersion().VOffset = value;
            }
        }
        public CssLength BoxShadowBlurRadius
        {
            get { return this._boxShadow.BlurRadius; }
            set
            {
                if (Assignable()) CheckBoxShadowVersion().BlurRadius = value;
            }
        }
        public CssLength BoxShadowSpreadDistance
        {
            get { return this._boxShadow.SpreadDistance; }
            set
            {
                if (Assignable()) CheckBoxShadowVersion().SpreadDistance = value;
            }
        }
        public Color BoxShadowColor
        {
            get { return this._boxShadow.ShadowColor; }
            set
            {
                if (Assignable()) CheckBoxShadowVersion().ShadowColor = value;
            }
        }
        //----------------------------------------------------------------------
        public FlexFlowDirection FlexFlowDirection
        {
            get { return this._flexFeats.FlowDirection; }
            set
            {
                if (Assignable()) CheckFlexVersion().FlowDirection = value;
            }
        }
        public FlexJustifyContent FlexJustifyContent
        {
            get { return this._flexFeats.JustifyContent; }
            set
            {
                if (Assignable()) CheckFlexVersion().JustifyContent = value;
            }
        }
        public FlexWrap FlexWrap
        {
            get { return this._flexFeats.FlexWrap; }
            set
            {
                if (Assignable()) CheckFlexVersion().FlexWrap = value;
            }
        }
        public FlexAlignContent FlexAlignContent
        {
            get { return this._flexFeats.AlignContent; }
            set
            {
                if (Assignable()) CheckFlexVersion().AlignContent = value;
            }
        }
        public FlexAlignSelf FlexAlignSelf
        {
            get { return this._flexFeats.AlignSelf; }
            set
            {
                if (Assignable()) CheckFlexVersion().AlignSelf = value;
            }
        }
        public FlexAlignItem FlexAlignItem
        {
            get { return this._flexFeats.AlignItem; }
            set
            {
                if (Assignable()) CheckFlexVersion().AlignItem = value;
            }
        }
        //----------------------------------------------------------------------
        public int FlexGrow
        {
            get { return this._flexFeats.FlexGrow; }
            set
            {
                if (Assignable()) CheckFlexVersion().FlexGrow = value;
            }
        }
        public int FlexShrink
        {
            get { return this._flexFeats.FlexShrink; }
            set
            {
                if (Assignable()) CheckFlexVersion().FlexShrink = value;
            }
        }
        public int FlexOrder
        {
            get { return this._flexFeats.FlexOrder; }
            set
            {
                if (Assignable()) CheckFlexVersion().FlexOrder = value;
            }
        }
        public CssLength FlexBasis
        {
            get { return this._flexFeats.FlexBasis; }
            set
            {
                if (Assignable()) CheckFlexVersion().FlexBasis = value;
            }
        }
        //--------------------------------------------------------------
        public int FlexExtensionNum
        {
            get { return this._flexFeats.FlexExtensionNum; }
            set
            {
                if (Assignable()) CheckFlexVersion().FlexExtensionNum = value;
            }
        }
        public int FlexExtensionPart
        {
            get { return this._flexFeats.FlexExtensionPart; }
            set
            {
                if (Assignable()) CheckFlexVersion().FlexExtensionPart = value;
            }
        }
        //----------------------------------------------------------------------
#if DEBUG
        public static bool dbugCompare(dbugPropCheckReport dbugR, BoxSpec boxBase, BoxSpec spec)
        {

            int dd = boxBase.__aa_dbugId;
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


#if DEBUG
    public class dbugPropCheckReport
    {
        System.Collections.Generic.List<string> msgs = new System.Collections.Generic.List<string>();
        public void Check(string propName, bool testResult)
        {
            if (!testResult)
            {
                msgs.Add(propName);
            }
        }
        public int Count
        {
            get { return this.msgs.Count; }
        }
        public void ClearMsgs()
        {
            this.msgs.Clear();
        }
        public System.Collections.Generic.List<string> GetList()
        {
            return this.msgs;
        }
    }
#endif

}