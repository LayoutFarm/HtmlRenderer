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
    /// Base class for css box to handle the css properties.<br/>
    /// Has field and property for every css property that can be set, the properties add additional parsing like
    /// setting the correct border depending what border value was set (single, two , all four).<br/>
    /// Has additional fields to control the location and size of the box and 'actual' css values for some properties
    /// that require additional calculations and parsing.<br/>
    /// </summary>
    public abstract partial class CssBoxBase
    {


        //==========================================================
        #region css values Inherit From Parent (by default)
        //inherit from parent by default
        CssFontProp _fontProps = CssFontProp.Default;
        CssListProp _listProps = CssListProp.Default;
        CssLength _lineHeight = CssLength.NormalWordOrLine;
        CssLength _textIndent = CssLength.ZeroNoUnit;
        Color _actualColor = System.Drawing.Color.Empty;
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
        CssBorderProp _borderProps = CssBorderProp.Default;
        CssPaddingProp _paddingProps = CssPaddingProp.Default;
        CssMarginProp _marginProps = CssMarginProp.Default;
        CssCornerProp _cornerProps = CssCornerProp.Default;
        Font _actualFont;
        CssBackgroundProp _backgroundProps = CssBackgroundProp.Default; 

        CssDisplay _cssDisplay = CssDisplay.Inline;
        CssFloat _float = CssFloat.None;

        CssLength _left = CssLength.AutoLength;
        CssLength _top = CssLength.AutoLength; 
        CssLength _right = CssLength.NotAssign;
        CssLength _bottom = CssLength.NotAssign; 
        CssLength _width = CssLength.AutoLength;
        CssLength _height = CssLength.AutoLength;

        CssLength _maxWidth = CssLength.NotAssign; 

        CssOverflow _overflow = CssOverflow.Visible;
        CssTextDecoration _textDecoration = CssTextDecoration.NotAssign;
        CssPosition _position = CssPosition.Static;
        CssLength _wordSpacing = CssLength.NormalWordOrLine;
        #endregion
        //==========================================================


        #region Fields 

        float _locationX;
        float _locationY;
        private float _sizeHeight;
        private float _sizeWidth;

        private float _actualCornerNW = float.NaN;
        private float _actualCornerNE = float.NaN;
        private float _actualCornerSW = float.NaN;
        private float _actualCornerSE = float.NaN;


        private float _actualHeight = float.NaN;
        private float _actualWidth = float.NaN;

        private float _actualPaddingTop = float.NaN;
        private float _actualPaddingBottom = float.NaN;
        private float _actualPaddingRight = float.NaN;
        private float _actualPaddingLeft = float.NaN;
        private float _actualMarginTop = float.NaN;
        private float _collapsedMarginTop = float.NaN;
        private float _actualMarginBottom = float.NaN;
        private float _actualMarginRight = float.NaN;
        private float _actualMarginLeft = float.NaN;

        private float _actualBorderTopWidth = float.NaN;
        private float _actualBorderLeftWidth = float.NaN;
        private float _actualBorderBottomWidth = float.NaN;
        private float _actualBorderRightWidth = float.NaN;

        /// <summary>
        /// the width of whitespace between words
        /// </summary>
        private float _actualLineHeight = float.NaN;
        private float _actualWordSpacing = float.NaN;
        private float _actualTextIndent = float.NaN;

        private float _actualBorderSpacingHorizontal = float.NaN;
        private float _actualBorderSpacingVertical = float.NaN;

        #endregion



#if DEBUG
        public readonly int dbugId = dbugTotalId++;
        static int dbugTotalId;
#endif
        public CssBoxBase()
        {
            _actualColor = System.Drawing.Color.Black;
#if DEBUG
            
#endif 

        } 
        #region CSS Properties

        public CssLength BorderBottomWidth
        {
            get { return this._borderProps.BottomWidth; }
            set
            {
                CheckBorderVersion().BottomWidth = value;
                _actualBorderBottomWidth = Single.NaN;
            }
        }
        public CssDisplay CssDisplay
        {
            get { return this._cssDisplay; }
            set
            {
                this._cssDisplay = value;
                this.PassTestInlineOnlyDeep = this.PassTestInlineOnly = false;
            }
        }
        public CssDirection CssDirection
        {
            get { return this._cssDirection; }
            set { this._cssDirection = value; }
        }

        public CssLength BorderLeftWidth
        {
            get { return this._borderProps.LeftWidth; }
            set
            {
                CheckBorderVersion().LeftWidth = value;
                _actualBorderLeftWidth = Single.NaN;
            }
        }

        public CssLength BorderRightWidth
        {
            get { return this._borderProps.RightWidth; }
            set
            {

                CheckBorderVersion().RightWidth = value;
                _actualBorderRightWidth = Single.NaN;
            }
        }

        public CssLength BorderTopWidth
        {
            get { return this._borderProps.TopWidth; }
            set
            {
                CheckBorderVersion().TopWidth = value;
                _actualBorderTopWidth = Single.NaN;
            }
        }

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
            set
            {
                CheckBorderVersion().BottomColor = value;
            }
        }
        public Color BorderLeftColor
        {
            get { return this._borderProps.LeftColor; }
            set
            {
                CheckBorderVersion().LeftColor = value;
            }
        }
        //--------------------------------------------
        public Color BorderRightColor
        {
            get { return this._borderProps.RightColor; }
            set
            {

                CheckBorderVersion().RightColor = value;
            }
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
            set { CheckCornerVersion().NERadius = value; }
        }
        public CssLength CornerNWRadius
        {
            get { return this._cornerProps.NWRadius; }
            set { CheckCornerVersion().NWRadius = value; }
        }
        public CssLength CornerSERadius
        {
            get { return this._cornerProps.SERadius; }
            set { CheckCornerVersion().SERadius = value; }
        }
        public CssLength CornerSWRadius
        {
            get { return this._cornerProps.SWRadius; }
            set { CheckCornerVersion().SWRadius = value; }
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
                _actualPaddingBottom = float.NaN;
            }
        }

        public CssLength PaddingLeft
        {
            get { return this._paddingProps.Left; }
            set
            {
                CheckPaddingVersion().Left = value;
                _actualPaddingLeft = float.NaN;
            }
        }

        public CssLength PaddingRight
        {
            get { return this._paddingProps.Right; }
            set
            {
                CheckPaddingVersion().Right = value;
                _actualPaddingRight = float.NaN;
            }
        }

        public CssLength PaddingTop
        {
            get { return this._paddingProps.Top; }
            set
            {
                CheckPaddingVersion().Top = value;
                _actualPaddingTop = float.NaN;
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

        public string BackgroundImage
        {
            get { return this._backgroundProps.BackgroundImage; }
            set { CheckBgVersion().BackgroundImage = value; }
        }

        public string BackgroundPosition
        {
            get { return this._backgroundProps.BackgroundPosition; }
            set { CheckBgVersion().BackgroundPosition = value; }
        }

        public string BackgroundRepeat
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
        public bool IsAbsolutePosition
        {
            get
            {
                return this.Position == CssPosition.Absolute;
            }
        }

        //----------------------------------------------------
        public CssLength LineHeight
        {
            get { return _lineHeight; }
            set
            {
                _lineHeight = value;
                _actualLineHeight = float.NaN;
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
            set { _textIndent = NoEms(value); }
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
            set { this._wordSpacing = this.NoEms(value); }
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
        public float LocationX
        {
            get { return this._locationX; }
            set
            {
                this._locationX = value;
                this._compactFlags |= CssBoxFlagsConst.HAS_ASSIGNED_LOCATION;
            }
        }
        public float LocationY
        {
            get { return this._locationY; }
            set
            {
                this._locationY = value;
                this._compactFlags |= CssBoxFlagsConst.HAS_ASSIGNED_LOCATION;
            }
        }
        public void SetLocation(float x, float y)
        {
            this._locationX = x;
            this._locationY = y;
            this._compactFlags |= CssBoxFlagsConst.HAS_ASSIGNED_LOCATION;
        }
        /// <summary>
        /// Gets or sets the size of the box
        /// </summary>
        public SizeF Size
        {
            get { return new SizeF(this._sizeWidth, this._sizeHeight); }
            set
            {
                this._sizeWidth = value.Width;
                this._sizeHeight = value.Height;
            }
        }
        public float SizeWidth
        {
            get
            {
                return this._sizeWidth;
            }

        }
        public float SizeHeight
        {
            get
            {
                return this._sizeHeight;
            }
        }

        /// <summary>
        /// Gets the bounds of the box
        /// </summary>
        public RectangleF Bounds
        {
            get { return new RectangleF(new PointF(this.LocationX, this.LocationY), Size); }
        }

        /// <summary>
        /// Gets the width available on the box, counting padding and margin.
        /// </summary>
        public float AvailableWidth
        {
            get { return Size.Width - ActualBorderLeftWidth - ActualPaddingLeft - ActualPaddingRight - ActualBorderRightWidth; }
        }

        /// <summary>
        /// Gets the right of the box. When setting, it will affect only the width of the box.
        /// </summary>
        public float ActualRight
        {
            get { return LocationX + Size.Width; }
            set { Size = new SizeF(value - LocationX, Size.Height); }
        }

        /// <summary>
        /// Gets or sets the bottom of the box. 
        /// (When setting, alters only the Size.Height of the box)
        /// </summary>
        public float ActualBottom
        {
            get { return this.LocationY + Size.Height; }
            set
            {
                Size = new SizeF(Size.Width, value - this.LocationY);
            }
        }

        /// <summary>
        /// Gets the left of the client rectangle (Where content starts rendering)
        /// </summary>
        public float ClientLeft
        {
            get { return this.LocationX + ActualBorderLeftWidth + ActualPaddingLeft; }
        }

        /// <summary>
        /// Gets the top of the client rectangle (Where content starts rendering)
        /// </summary>
        public float ClientTop
        {
            get { return this.LocationY + ActualBorderTopWidth + ActualPaddingTop; }
        }

        /// <summary>
        /// Gets the right of the client rectangle
        /// </summary>
        public float ClientRight
        {
            get { return ActualRight - ActualPaddingRight - ActualBorderRightWidth; }
        }

        /// <summary>
        /// Gets the bottom of the client rectangle
        /// </summary>
        public float ClientBottom
        {
            get { return ActualBottom - ActualPaddingBottom - ActualBorderBottomWidth; }
        }

        /// <summary>
        /// Gets the client rectangle
        /// </summary>
        public RectangleF ClientRectangle
        {
            get { return RectangleF.FromLTRB(ClientLeft, ClientTop, ClientRight, ClientBottom); }
        }

        /// <summary>
        /// Gets the actual height
        /// </summary>
        public float ActualHeight
        {
            get
            {
                if (float.IsNaN(_actualHeight))
                {
                    _actualHeight = CssValueParser.ParseLength(Height, Size.Height, this);
                }
                return _actualHeight;
            }
        }

        /// <summary>
        /// Gets the actual height
        /// </summary>
        public float ActualWidth
        {
            get
            {
                if (float.IsNaN(_actualWidth))
                {
                    _actualWidth = CssValueParser.ParseLength(Width, Size.Width, this);
                }
                return _actualWidth;
            }
        }

        /// <summary>
        /// Gets the actual top's padding
        /// </summary>
        public float ActualPaddingTop
        {
            get
            {
                if (float.IsNaN(_actualPaddingTop))
                {
                    _actualPaddingTop = CssValueParser.ParseLength(PaddingTop, Size.Width, this);
                }
                return _actualPaddingTop;
            }
        }

        /// <summary>
        /// Gets the actual padding on the left
        /// </summary>
        public float ActualPaddingLeft
        {
            get
            {
                if (float.IsNaN(_actualPaddingLeft))
                {
                    _actualPaddingLeft = CssValueParser.ParseLength(PaddingLeft, Size.Width, this);
                }
                return _actualPaddingLeft;
            }
        }

        /// <summary>
        /// Gets the actual Padding of the bottom
        /// </summary>
        public float ActualPaddingBottom
        {
            get
            {
                if (float.IsNaN(_actualPaddingBottom))
                {
                    _actualPaddingBottom = CssValueParser.ParseLength(PaddingBottom, Size.Width, this);
                }
                return _actualPaddingBottom;
            }
        }

        /// <summary>
        /// Gets the actual padding on the right
        /// </summary>
        public float ActualPaddingRight
        {
            get
            {
                if (float.IsNaN(_actualPaddingRight))
                {
                    _actualPaddingRight = CssValueParser.ParseLength(PaddingRight, Size.Width, this);
                }
                return _actualPaddingRight;
            }
        }

        /// <summary>
        /// Gets the actual top's Margin
        /// </summary>
        public float ActualMarginTop
        {
            get
            {
                if (float.IsNaN(_actualMarginTop))
                {
                    if (this.MarginTop.IsAuto)  //(MarginTop == CssConstants.Auto)
                    {
                        MarginTop = CssLength.ZeroPx;
                    }
                    var actualMarginTop = CssValueParser.ParseLength(MarginTop, Size.Width, this);
                    if (this.MarginLeft.IsPercentage)// (MarginLeft.EndsWith("%"))
                    {
                        return actualMarginTop;
                    }
                    _actualMarginTop = actualMarginTop;
                }
                return _actualMarginTop;
            }
        }

        /// <summary>
        /// The margin top value if was effected by margin collapse.
        /// </summary>
        public float CollapsedMarginTop
        {
            get { return float.IsNaN(_collapsedMarginTop) ? 0 : _collapsedMarginTop; }
            set { _collapsedMarginTop = value; }
        }

        /// <summary>
        /// Gets the actual Margin on the left
        /// </summary>
        public float ActualMarginLeft
        {
            get
            {

                if (float.IsNaN(_actualMarginLeft))
                {
                    if (MarginLeft.IsAuto) //if (MarginLeft == CssConstants.Auto)
                    { MarginLeft = CssLength.ZeroPx; }
                    //"0"; }

                    var actualMarginLeft = CssValueParser.ParseLength(MarginLeft, Size.Width, this);
                    if (this.MarginLeft.IsPercentage) // (MarginLeft.EndsWith("%"))
                    {
                        return actualMarginLeft;
                    }
                    _actualMarginLeft = actualMarginLeft;
                }
                return _actualMarginLeft;
            }
        }

        /// <summary>
        /// Gets the actual Margin of the bottom
        /// </summary>
        public float ActualMarginBottom
        {
            get
            {
                if (float.IsNaN(_actualMarginBottom))
                {
                    if (MarginBottom.IsAuto)
                    {
                        MarginBottom = CssLength.ZeroPx;
                    }

                    var actualMarginBottom = CssValueParser.ParseLength(MarginBottom, Size.Width, this);

                    if (MarginLeft.IsPercentage)
                    {
                        return actualMarginBottom;
                    }

                    _actualMarginBottom = actualMarginBottom;
                }
                return _actualMarginBottom;
            }
        }

        /// <summary>
        /// Gets the actual Margin on the right
        /// </summary>
        public float ActualMarginRight
        {
            get
            {
                if (float.IsNaN(_actualMarginRight))
                {

                    if (MarginRight.IsAuto)
                    {

                        MarginRight = CssLength.ZeroPx;
                    }
                    var actualMarginRight = CssValueParser.ParseLength(MarginRight, Size.Width, this);
                    if (MarginLeft.IsPercentage)
                    {
                        return actualMarginRight;
                    }
                    _actualMarginRight = actualMarginRight;
                }
                return _actualMarginRight;
            }
        }

        /// <summary>
        /// Gets the actual top border width
        /// </summary>
        public float ActualBorderTopWidth
        {
            get
            {
                if (float.IsNaN(_actualBorderTopWidth))
                {
                    _actualBorderTopWidth = CssValueParser.GetActualBorderWidth(BorderTopWidth, this);
                    if (this.BorderTopStyle == CssBorderStyle.None)
                    {
                        _actualBorderTopWidth = 0f;
                    }
                }
                return _actualBorderTopWidth;
            }
        }

        /// <summary>
        /// Gets the actual Left border width
        /// </summary>
        public float ActualBorderLeftWidth
        {
            get
            {
                if (float.IsNaN(_actualBorderLeftWidth))
                {
                    _actualBorderLeftWidth = CssValueParser.GetActualBorderWidth(BorderLeftWidth, this);
                    if (this.BorderLeftStyle == CssBorderStyle.None)
                    {
                        _actualBorderLeftWidth = 0f;
                    }
                }
                return _actualBorderLeftWidth;
            }
        }

        /// <summary>
        /// Gets the actual Bottom border width
        /// </summary>
        public float ActualBorderBottomWidth
        {
            get
            {
                if (float.IsNaN(_actualBorderBottomWidth))
                {
                    _actualBorderBottomWidth = CssValueParser.GetActualBorderWidth(BorderBottomWidth, this);
                    if (this.BorderBottomStyle == CssBorderStyle.None)
                    {
                        _actualBorderBottomWidth = 0f;

                    }
                }
                return _actualBorderBottomWidth;
            }
        }

        /// <summary>
        /// Gets the actual Right border width
        /// </summary>
        public float ActualBorderRightWidth
        {
            get
            {
                if (float.IsNaN(_actualBorderRightWidth))
                {
                    _actualBorderRightWidth = CssValueParser.GetActualBorderWidth(BorderRightWidth, this);
                    if (this.BorderRightStyle == CssBorderStyle.None)
                    {
                        _actualBorderRightWidth = 0f;
                    }
                }
                return _actualBorderRightWidth;
            }
        }

        /// <summary>
        /// Gets the actual top border Color
        /// </summary>
        public Color ActualBorderTopColor
        {
            get
            {
                return this.BorderTopColor;
            }
        }

        /// <summary>
        /// Gets the actual Left border Color
        /// </summary>
        public Color ActualBorderLeftColor
        {
            get
            {
                return this.BorderLeftColor;
            }
        }

        /// <summary>
        /// Gets the actual Bottom border Color
        /// </summary>
        public Color ActualBorderBottomColor
        {
            get
            {
                return this.BorderBottomColor;
            }
        }

        /// <summary>
        /// Gets the actual Right border Color
        /// </summary>
        public Color ActualBorderRightColor
        {
            get
            {
                return this.BorderRightColor;
            }
        }

        /// <summary>
        /// Gets the actual lenght of the north west corner
        /// </summary>
        public float ActualCornerNW
        {
            get
            {
                if (float.IsNaN(_actualCornerNW))
                {
                    _actualCornerNW = CssValueParser.ParseLength(CornerNWRadius, 0, this);
                }
                return _actualCornerNW;
            }
        }

        /// <summary>
        /// Gets the actual lenght of the north east corner
        /// </summary>
        public float ActualCornerNE
        {
            get
            {
                if (float.IsNaN(_actualCornerNE))
                {
                    _actualCornerNE = CssValueParser.ParseLength(CornerNERadius, 0, this);
                }
                return _actualCornerNE;
            }
        }

        /// <summary>
        /// Gets the actual lenght of the south east corner
        /// </summary>
        public float ActualCornerSE
        {
            get
            {
                if (float.IsNaN(_actualCornerSE))
                {
                    _actualCornerSE = CssValueParser.ParseLength(CornerSERadius, 0, this);
                }
                return _actualCornerSE;
            }
        }

        /// <summary>
        /// Gets the actual lenght of the south west corner
        /// </summary>
        public float ActualCornerSW
        {
            get
            {
                if (float.IsNaN(_actualCornerSW))
                {
                    _actualCornerSW = CssValueParser.ParseLength(CornerSWRadius, 0, this);
                }
                return _actualCornerSW;
            }
        }

        /// <summary>
        /// Gets a value indicating if at least one of the corners of the box is rounded
        /// </summary>
        public bool IsRounded
        {
            get { return ActualCornerNE > 0f || ActualCornerNW > 0f || ActualCornerSE > 0f || ActualCornerSW > 0f; }
        }

        /// <summary>
        /// Gets the actual width of whitespace between words.
        /// </summary>
        public float ActualWordSpacing
        {
            get { return _actualWordSpacing; }
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
        /// Gets the font that should be actually used to paint the text of the box
        /// </summary>
        public Font ActualFont
        {
            get
            {
                if (_actualFont != null)
                {
                    return _actualFont;
                }
                //-----------------------------------------------------------------------------                
                _actualFont = this._fontProps.GetCacheFont(this.GetParent());
                return _actualFont;
            }
        }

        /// <summary>
        /// Gets the line height
        /// </summary>
        public float ActualLineHeight
        {
            get
            {
                if (float.IsNaN(_actualLineHeight))
                {
                    //if not calculate yet then calculate it
                    _actualLineHeight = .9f * CssValueParser.ParseLength(LineHeight, Size.Height, this);

                }
                return _actualLineHeight;
            }
        }

        /// <summary>
        /// Gets the text indentation (on first line only)
        /// </summary>
        public float ActualTextIndent
        {
            get
            {
                if (float.IsNaN(_actualTextIndent))
                {
                    _actualTextIndent = CssValueParser.ParseLength(TextIndent, Size.Width, this);
                }

                return _actualTextIndent;
            }
        }

        /// <summary>
        /// Gets the actual horizontal border spacing for tables
        /// </summary>
        public float ActualBorderSpacingHorizontal
        {
            get
            {
                if (float.IsNaN(_actualBorderSpacingHorizontal))
                {
                    _actualBorderSpacingHorizontal = this.BorderSpacingHorizontal.Number; // this._borderSpacingH.Number;
                }
                return _actualBorderSpacingHorizontal;
            }
        }

        /// <summary>
        /// Gets the actual vertical border spacing for tables
        /// </summary>
        public float ActualBorderSpacingVertical
        {
            get
            {
                if (float.IsNaN(_actualBorderSpacingVertical))
                {
                    _actualBorderSpacingVertical = this.BorderSpacingVertical.Number;// this._borderSpacingV.Number;

                }
                return _actualBorderSpacingVertical;
            }
        }

        /// <summary>
        /// Get the parent of this css properties instance.
        /// </summary>
        /// <returns></returns>
        public abstract CssBoxBase GetParent();

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
            if (length.Unit == CssUnit.Ems)
            {
                return length.ConvertEmToPixels(GetEmHeight());
            }
            return length;
        }

        /// <summary>
        /// Set the style/width/color for all 4 borders on the box.<br/>
        /// if null is given for a value it will not be set.
        /// </summary>
        /// <param name="style">optional: the style to set</param>
        /// <param name="width">optional: the width to set</param>
        /// <param name="color">optional: the color to set</param>
        protected void SetAllBorders(CssBorderStyle borderStyle, CssLength length, Color color)
        {
            //assign values

            BorderLeftStyle = BorderTopStyle = BorderRightStyle = BorderBottomStyle = borderStyle;

            BorderLeftWidth = BorderTopWidth = BorderRightWidth = BorderBottomWidth = length;

            BorderLeftColor = BorderTopColor = BorderRightColor = BorderBottomColor = color;

        }

        /// <summary>
        /// Measures the width of whitespace between words (set <see cref="ActualWordSpacing"/>).
        /// </summary>
        protected void MeasureWordSpacing(IGraphics g)
        {
            if (float.IsNaN(ActualWordSpacing))
            {
                _actualWordSpacing = CssUtils.WhiteSpace(g, this);
                if (!this.WordSpacing.IsNormalWordSpacing)
                {
                    _actualWordSpacing += CssValueParser.ParseLength(this.WordSpacing, 1, this);
                }
            }
        }

        /// <summary>
        /// Inherits inheritable values from specified box.
        /// </summary>
        /// <param name="clone">Set to true to inherit all CSS properties instead of only the ineritables</param>
        /// <param name="p">Box to inherit the properties</param>
        protected void InheritStyles(CssBox p, bool clone)
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
                this.CssTextAlign = p.CssTextAlign;
                this.VerticalAlign = p.VerticalAlign;
                this.CssVisibility = p.CssVisibility;
                this.WhiteSpace = p.WhiteSpace;
                this.WordBreak = p.WordBreak;
                this.CssDirection = p.CssDirection;
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
                    this._wordSpacing = p._wordSpacing;
                    this._lineHeight = p._lineHeight;
                    this._float = p._float;

                    this.CssDisplay = p.CssDisplay;
                    this.Overflow = p.Overflow;
                    this.TextDecoration = p.TextDecoration;
                    this.Position = p.Position;
                    this.Width = p.Width;
                    this.Height = p.Height;
                    this.MaxWidth = p.MaxWidth;

                }
            }
        }
    }
}