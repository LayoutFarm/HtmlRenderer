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

        #region CSS Fields

        private string _backgroundColor = "transparent";
        private string _backgroundGradient = "none";
        private string _backgroundGradientAngle = "90";
        private string _backgroundImage = "none";
        private string _backgroundPosition = "0% 0%";
        private string _backgroundRepeat = "repeat";

        CssBorderProp _borderProps = CssBorderProp.Default;
        CssPaddingProp _paddingProps = CssPaddingProp.Default;
        CssMarginProp _marginProps = CssMarginProp.Default;
        CssListProp _listProps = CssListProp.Default;
        CssCornerProp _cornerProps = CssCornerProp.Default;

        CssBorderCollapse _borderCollapse = CssBorderCollapse.Separate;
        CssLength _borderSpacingH = CssLength.ZeroNoUnit;
        CssLength _borderSpacingV = CssLength.ZeroNoUnit;

        CssLength _bottom = CssLength.NotAssign;

        private string _color = "black";



        CssEmptyCell _emptyCells = CssEmptyCell.Show;
        CssDirection _cssDirection = CssDirection.Ltl;
        CssDisplay _cssDisplay = CssDisplay.Inline;


        private string _fontFamily = "serif";
        private string _fontSize = "medium";
        private string _fontStyle = "normal";
        private string _fontVariant = "normal";
        private string _fontWeight = "normal";

        CssFloat _float = CssFloat.None;
        CssLength _height = CssLength.AutoLength;

        CssLength _left = CssLength.AutoLength;
        CssLength _lineHeight = CssLength.NormalWordOrLine;
        CssOverflow _overflow = CssOverflow.Visible;
        CssLength _right = CssLength.NotAssign;


        CssTextAlign _textAlign = CssTextAlign.NotAssign;
        CssTextDecoration _textDecoration = CssTextDecoration.NotAssign;

        CssLength _textIndent = CssLength.ZeroNoUnit;
        CssLength _top = CssLength.AutoLength;
        CssPosition _position = CssPosition.Static;

        CssVerticalAlign _verticalAlign = CssVerticalAlign.Baseline;

        CssLength _width = CssLength.AutoLength;
        CssLength _maxWidth = CssLength.NotAssign;
        CssLength _wordSpacing = CssLength.NormalWordOrLine;
        CssWordBreak _wordBreak = CssWordBreak.Normal;


        CssWhiteSpace _whitespace = CssWhiteSpace.Normal;
        CssVisibility _visibility = CssVisibility.Visible;


        #endregion


        #region Fields

        /// <summary>
        /// Gets or sets the location of the box
        /// </summary>
        private PointF _location;

        /// <summary>
        /// Gets or sets the size of the box
        /// </summary>
        //private SizeF _size;
        private float _sizeHeight;
        private float _sizeWidth;

        private float _actualCornerNW = float.NaN;
        private float _actualCornerNE = float.NaN;
        private float _actualCornerSW = float.NaN;
        private float _actualCornerSE = float.NaN;
        private Color _actualColor = System.Drawing.Color.Empty;
        private float _actualBackgroundGradientAngle = float.NaN;
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
        private float _fontAscent = float.NaN;
        private float _fontDescent = float.NaN;
        private float _fontLineSpacing = float.NaN;

        private Color _actualBackgroundGradient = System.Drawing.Color.Empty;
        private Color _actualBorderTopColor = System.Drawing.Color.Empty;
        private Color _actualBorderLeftColor = System.Drawing.Color.Empty;
        private Color _actualBorderBottomColor = System.Drawing.Color.Empty;
        private Color _actualBorderRightColor = System.Drawing.Color.Empty;
        private Color _actualBackgroundColor = System.Drawing.Color.Empty;
        private Font _actualFont;

        #endregion


        #region CSS Properties

        public CssLength BorderBottomWidth
        {
            get { return this._borderProps.BottomWidth; }
            set
            {
                CheckMyBorderVersion().BottomWidth = value;
                _actualBorderBottomWidth = Single.NaN;
            }
        }
        CssBorderProp CheckMyBorderVersion()
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
        CssListProp CheckListPropVersion()
        {
            return this._listProps = this._listProps.GetMyOwnVersion(this);
        }
        public CssLength BorderLeftWidth
        {
            get { return this._borderProps.LeftWidth; }
            set
            {

                CheckMyBorderVersion().LeftWidth = value;
                _actualBorderLeftWidth = Single.NaN;
            }
        }

        public CssLength BorderRightWidth
        {
            get { return this._borderProps.RightWidth; }
            set
            {

                CheckMyBorderVersion().RightWidth = value;
                _actualBorderRightWidth = Single.NaN;
            }
        }

        public CssLength BorderTopWidth
        {
            get { return this._borderProps.TopWidth; }
            set
            {

                CheckMyBorderVersion().TopWidth = value;
                _actualBorderTopWidth = Single.NaN;
            }
        }

        public CssBorderStyle BorderTopStyle
        {
            //get { return this._borderTopStyle; }
            //set { this._borderTopStyle = value; }
            get { return this._borderProps.TopStyle; }
            set
            {
                CheckMyBorderVersion().TopStyle = value;
            }

        }
        public CssBorderStyle BorderLeftStyle
        {
            //get { return this._borderLeftStyle; }
            //set { this._borderLeftStyle = value; }
            get { return this._borderProps.LeftStyle; }
            set
            {
                CheckMyBorderVersion().LeftStyle = value;
            }
        }
        public CssBorderStyle BorderRightStyle
        {
            //get { return this._borderRightStyle; }
            //set { this._borderRightStyle = value; }
            get { return this._borderProps.RightStyle; }
            set
            {
                CheckMyBorderVersion().RightStyle = value;
            }
        }

        public CssBorderStyle BorderBottomStyle
        {
            //get { return this._borderBottomStyle; }
            //set { this._borderBottomStyle = value; }
            get { return this._borderProps.BottomStyle; }
            set
            {
                CheckMyBorderVersion().BottomStyle = value;
            }
        }

        //--------------------------------------------
        public Color BorderBottomColor
        {
            get { return this._borderProps.BottomColor; }
            set
            {

                CheckMyBorderVersion().BottomColor = value;
                _actualBorderBottomColor = System.Drawing.Color.Empty;
            }
        }
        public Color BorderLeftColor
        {
            get { return this._borderProps.LeftColor; }
            set
            {
                CheckMyBorderVersion().LeftColor = value;
                _actualBorderLeftColor = System.Drawing.Color.Empty;
            }
        }
        //--------------------------------------------
        public Color BorderRightColor
        {
            get { return this._borderProps.RightColor; }
            set
            {

                CheckMyBorderVersion().RightColor = value;
                _actualBorderRightColor = System.Drawing.Color.Empty;
            }
        }

        public Color BorderTopColor
        {
            get { return this._borderProps.TopColor; }
            set
            {
                CheckMyBorderVersion().TopColor = value;
                _actualBorderTopColor = System.Drawing.Color.Empty;
            }
        }
        public CssLength BorderSpacingVertical
        {
            get { return _borderSpacingV; }
            set { _borderSpacingV = value; }
        }
        public CssLength BorderSpacingHorizontal
        {
            get { return _borderSpacingH; }
            set { _borderSpacingH = value; }
        }
        public CssBorderCollapse BorderCollapse
        {
            get { return this._borderCollapse; }
            set
            {
                this._borderCollapse = value;
            }
        }

        public bool IsBorderCollapse
        {
            get
            {
                return this._borderCollapse == CssBorderCollapse.Collapse;
            }
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
            get
            {
                return this._width;
            }
            set
            {
                this._width = value;
            }
        }
        //public string MaxWidth
        //{
        //    get { return _maxWidth; }
        //    set { _maxWidth = value; }
        //}
        public CssLength MaxWidth
        {
            get { return _maxWidth; }
            set { _maxWidth = value; }
        }
        //public string Height
        //{
        //    get { return _height; }
        //    set { _height = value; }
        //}
        public CssLength Height
        {
            get { return this._height; }
            set { this._height = value; }
        }
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        public string BackgroundImage
        {
            get { return _backgroundImage; }
            set { _backgroundImage = value; }
        }

        public string BackgroundPosition
        {
            get { return _backgroundPosition; }
            set { _backgroundPosition = value; }
        }

        public string BackgroundRepeat
        {
            get { return _backgroundRepeat; }
            set { _backgroundRepeat = value; }
        }

        public string BackgroundGradient
        {
            get { return _backgroundGradient; }
            set { _backgroundGradient = value; }
        }

        public string BackgroundGradientAngle
        {
            get { return _backgroundGradientAngle; }
            set { _backgroundGradientAngle = value; }
        }

        public string Color
        {
            get { return _color; }
            set { _color = value; _actualColor = System.Drawing.Color.Empty; }
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
            get
            {
                return this._position;
            }
            set
            {
                this._position = value;
            }
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
        //public string TextAlign
        //{
        //    get { return _textAlign; }
        //    set
        //    {
        //        _textAlign = value;
        //        switch (value)
        //        {
        //            case "":
        //            case CssConstants.Left:
        //                this.CssTextAlign = CssTextAlign.Left;
        //                break;
        //            case CssConstants.Right:
        //                this.CssTextAlign = CssTextAlign.Right;
        //                break;
        //            case CssConstants.Center:
        //                this.CssTextAlign = CssTextAlign.Center;
        //                break;
        //            case CssConstants.Justify:
        //                this.CssTextAlign = CssTextAlign.Justify;
        //                break;
        //            case CssConstants.Inherit:
        //                this.CssTextAlign = CssTextAlign.Inherit;
        //                break;
        //            default:
        //                {
        //                } break;
        //        }
        //    }
        //}

        //public string TextDecoration
        //{
        //    get { return _textDecoration; }
        //    set { _textDecoration = value; }
        //}
        public CssTextDecoration TextDecoration
        {
            get { return _textDecoration; }
            set { _textDecoration = value; }
        }

        //-----------------------------------
        public CssWhiteSpace WhiteSpace
        {
            get
            {
                return this._whitespace;
            }
            set
            {
                this._whitespace = value;
            }
        }
        //-----------------------------------

        public CssVisibility CssVisibility
        {
            get
            {
                return this._visibility;
            }
            set
            {
                this._visibility = value;
            }

        }

        //public string WordSpacing
        //{
        //    get { return _wordSpacing; }
        //    set { _wordSpacing = NoEms(value); }
        //}
        public CssLength WordSpacing
        {
            get { return this._wordSpacing; }
            set { this._wordSpacing = value; }
        }
        public void SetWordSpacing(string str)
        {
            this._wordSpacing = this.NoEms(new CssLength(str));
        }


        //public string WordBreak
        //{
        //    get { return _wordBreak; }
        //    set
        //    {
        //        _wordBreak = value;
        //        switch (value)
        //        {
        //            case CssConstants.Normal:
        //                this.myWordBreak = CssWordBreak.Normal;
        //                break;
        //            case CssConstants.BreakAll:
        //                this.myWordBreak = CssWordBreak.BreakAll;
        //                break;
        //            case CssConstants.KeepAll:
        //                this.myWordBreak = CssWordBreak.KeepAll;
        //                break;
        //            case CssConstants.Inherit:
        //                this.myWordBreak = CssWordBreak.Inherit;
        //                break;
        //            default:
        //                throw new NotSupportedException();
        //        }
        //    }
        //}
        public CssWordBreak WordBreak
        {
            get
            {
                return this._wordBreak;
            }
            set
            {
                this._wordBreak = value;
            }
        }

        public string FontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; }
        }

        public string FontSize
        {
            get { return _fontSize; }
            set
            {
                string length = RegexParserUtils.Search(RegexParserUtils.CssLength, value);

                if (length != null)
                {
                    string computedValue;
                    CssLength len = new CssLength(length);

                    if (len.HasError)
                    {
                        computedValue = "medium";
                    }
                    else if (len.Unit == CssUnit.Ems && GetParent() != null)
                    {
                        computedValue = len.ConvertEmToPoints(GetParent().ActualFont.SizeInPoints).ToString();
                    }
                    else
                    {
                        computedValue = len.ToString();
                    }

                    _fontSize = computedValue;
                }
                else
                {
                    _fontSize = value;
                }
            }
        }

        public string FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

        public string FontVariant
        {
            get { return _fontVariant; }
            set { _fontVariant = value; }
        }

        public string FontWeight
        {
            get { return _fontWeight; }
            set { _fontWeight = value; }
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

        public string ListStylePosition
        {
            get { return this._listProps.ListStylePosition; }
            set { CheckListPropVersion().ListStylePosition = value; }
        }

        public string ListStyleImage
        {
            get { return this._listProps.ListStyleImage; }
            set { CheckListPropVersion().ListStyleImage = value; }
        }

        public string ListStyleType
        {
            get { return this._listProps.ListStyleType; }
            set { CheckListPropVersion().ListStyleType = value; }
        }

        #endregion


        /// <summary>
        /// Gets or sets the location of the box
        /// </summary>
        public PointF Location
        {
            get { return _location; }
            set { _location = value; }
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
            get { return new RectangleF(Location, Size); }
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
            get { return Location.X + Size.Width; }
            set { Size = new SizeF(value - Location.X, Size.Height); }
        }

        /// <summary>
        /// Gets or sets the bottom of the box. 
        /// (When setting, alters only the Size.Height of the box)
        /// </summary>
        public float ActualBottom
        {
            get { return Location.Y + Size.Height; }
            set
            {
                Size = new SizeF(Size.Width, value - Location.Y);
            }
        }

        /// <summary>
        /// Gets the left of the client rectangle (Where content starts rendering)
        /// </summary>
        public float ClientLeft
        {
            get { return Location.X + ActualBorderLeftWidth + ActualPaddingLeft; }
        }

        /// <summary>
        /// Gets the top of the client rectangle (Where content starts rendering)
        /// </summary>
        public float ClientTop
        {
            get { return Location.Y + ActualBorderTopWidth + ActualPaddingTop; }
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

                    //if (MarginBottom == CssConstants.Auto)
                    //  MarginBottom = "0";
                    if (MarginBottom.IsAuto)
                    {
                        MarginBottom = CssLength.ZeroPx;
                    }
                    var actualMarginBottom = CssValueParser.ParseLength(MarginBottom, Size.Width, this);

                    if (MarginLeft.IsPercentage) //MarginLeft.EndsWith("%"))
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
                    //if (MarginRight == CssConstants.Auto)
                    if (MarginRight.IsAuto)
                    {
                        //MarginRight = "0";
                        MarginRight = CssLength.ZeroPx;
                    }
                    var actualMarginRight = CssValueParser.ParseLength(MarginRight, Size.Width, this);
                    if (MarginLeft.IsPercentage) //if (MarginLeft.EndsWith("%"))
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
                    ////if assign , default is none
                    //if (string.IsNullOrEmpty(BorderTopStyle) || BorderTopStyle == CssConstants.None)
                    //{
                    //    _actualBorderTopWidth = 0f;
                    //}
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
                    //if (string.IsNullOrEmpty(BorderLeftStyle) || BorderLeftStyle == CssConstants.None)
                    //{
                    //    _actualBorderLeftWidth = 0f;
                    //}
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
                    //if (string.IsNullOrEmpty(BorderBottomStyle) || BorderBottomStyle == CssConstants.None)
                    //{
                    //    _actualBorderBottomWidth = 0f;
                    //}
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
                    //if (string.IsNullOrEmpty(BorderRightStyle) || BorderRightStyle == CssConstants.None)
                    //{
                    //    _actualBorderRightWidth = 0f;
                    //}
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
                if (_actualBorderTopColor.IsEmpty)
                {
                    _actualBorderTopColor = BorderTopColor;// CssValueParser.GetActualColor(BorderTopColor);
                }
                return _actualBorderTopColor;
            }
        }

        /// <summary>
        /// Gets the actual Left border Color
        /// </summary>
        public Color ActualBorderLeftColor
        {
            get
            {
                if ((_actualBorderLeftColor.IsEmpty))
                {
                    _actualBorderLeftColor = BorderLeftColor;// CssValueParser.GetActualColor(BorderLeftColor);
                }
                return _actualBorderLeftColor;
            }
        }

        /// <summary>
        /// Gets the actual Bottom border Color
        /// </summary>
        public Color ActualBorderBottomColor
        {
            get
            {
                if ((_actualBorderBottomColor.IsEmpty))
                {
                    _actualBorderBottomColor = BorderBottomColor;// CssValueParser.GetActualColor(BorderBottomColor);
                }
                return _actualBorderBottomColor;
            }
        }

        /// <summary>
        /// Gets the actual Right border Color
        /// </summary>
        public Color ActualBorderRightColor
        {
            get
            {
                if ((_actualBorderRightColor.IsEmpty))
                {
                    _actualBorderRightColor = BorderRightColor;// CssValueParser.GetActualColor(BorderRightColor);
                }
                return _actualBorderRightColor;
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

                if (_actualColor.IsEmpty)
                {
                    _actualColor = CssValueParser.GetActualColor(Color);
                }

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

                if (_actualBackgroundColor.IsEmpty)
                {
                    _actualBackgroundColor = CssValueParser.GetActualColor(BackgroundColor);
                }

                return _actualBackgroundColor;
            }
        }

        /// <summary>
        /// Gets the second color that creates a gradient for the background
        /// </summary>
        public Color ActualBackgroundGradient
        {
            get
            {
                if (_actualBackgroundGradient.IsEmpty)
                {
                    _actualBackgroundGradient = CssValueParser.GetActualColor(BackgroundGradient);
                }
                return _actualBackgroundGradient;
            }
        }

        /// <summary>
        /// Gets the actual angle specified for the background gradient
        /// </summary>
        public float ActualBackgroundGradientAngle
        {
            get
            {
                if (float.IsNaN(_actualBackgroundGradientAngle))
                {
                    _actualBackgroundGradientAngle = CssValueParser.ParseNumber(BackgroundGradientAngle, 360f);
                }

                return _actualBackgroundGradientAngle;
            }
        }

        /// <summary>
        /// Gets the actual font of the parent
        /// </summary>
        public Font ActualParentFont
        {
            get { return GetParent() == null ? ActualFont : GetParent().ActualFont; }
        }

        /// <summary>
        /// Gets the font that should be actually used to paint the text of the box
        /// </summary>
        public Font ActualFont
        {
            get
            {
                if (_actualFont == null)
                {
                    if (string.IsNullOrEmpty(FontFamily)) { FontFamily = CssConstants.FontSerif; }
                    if (string.IsNullOrEmpty(FontSize)) { FontSize = CssConstants.FontSize.ToString(CultureInfo.InvariantCulture) + "pt"; }

                    FontStyle st = System.Drawing.FontStyle.Regular;

                    if (FontStyle == CssConstants.Italic || FontStyle == CssConstants.Oblique)
                    {
                        st |= System.Drawing.FontStyle.Italic;
                    }

                    if (FontWeight != CssConstants.Normal && FontWeight != CssConstants.Lighter && !string.IsNullOrEmpty(FontWeight) && FontWeight != CssConstants.Inherit)
                    {
                        st |= System.Drawing.FontStyle.Bold;
                    }

                    float fsize;
                    float parentSize = CssConstants.FontSize;

                    if (GetParent() != null)
                        parentSize = GetParent().ActualFont.Size;

                    switch (FontSize)
                    {
                        case CssConstants.Medium:
                            fsize = CssConstants.FontSize; break;
                        case CssConstants.XXSmall:
                            fsize = CssConstants.FontSize - 4; break;
                        case CssConstants.XSmall:
                            fsize = CssConstants.FontSize - 3; break;
                        case CssConstants.Small:
                            fsize = CssConstants.FontSize - 2; break;
                        case CssConstants.Large:
                            fsize = CssConstants.FontSize + 2; break;
                        case CssConstants.XLarge:
                            fsize = CssConstants.FontSize + 3; break;
                        case CssConstants.XXLarge:
                            fsize = CssConstants.FontSize + 4; break;
                        case CssConstants.Smaller:
                            fsize = parentSize - 2; break;
                        case CssConstants.Larger:
                            fsize = parentSize + 2; break;
                        default:
                            fsize = CssValueParser.ParseLength(FontSize, parentSize, parentSize, null, true, true);
                            break;
                    }

                    if (fsize <= 1f)
                    {
                        fsize = CssConstants.FontSize;
                    }

                    _actualFont = FontsUtils.GetCachedFont(FontFamily, fsize, st);
                }
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
                    //if (_actualLineHeight > 0)
                    //{
                    //}
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
                    //MatchCollection matches = RegexParserUtils.Match(RegexParserUtils.CssLength, BorderSpacing);

                    //if (matches.Count == 0)
                    //{
                    //    _actualBorderSpacingHorizontal = 0;
                    //}
                    //else if (matches.Count > 0)
                    //{
                    //    _actualBorderSpacingHorizontal = CssValueParser.ParseLength(matches[0].Value, 1, this);
                    //}
                    _actualBorderSpacingHorizontal = this._borderSpacingH.Number;
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
                    _actualBorderSpacingVertical = this._borderSpacingV.Number;

                    //MatchCollection matches = RegexParserUtils.Match(RegexParserUtils.CssLength, BorderSpacing);

                    //if (matches.Count == 0)
                    //{
                    //    _actualBorderSpacingVertical = 0;
                    //}
                    //else if (matches.Count == 1)
                    //{
                    //    _actualBorderSpacingVertical = CssValueParser.ParseLength(matches[0].Value, 1, this);
                    //}
                    //else
                    //{
                    //    _actualBorderSpacingVertical = CssValueParser.ParseLength(matches[1].Value, 1, this);
                    //}
                }
                return _actualBorderSpacingVertical;
            }
        }

        /// <summary>
        /// Get the parent of this css properties instance.
        /// </summary>
        /// <returns></returns>
        protected abstract CssBoxBase GetParent();

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
        protected string NoEms(string length)
        {
            var len = new CssLength(length);
            if (len.Unit == CssUnit.Ems)
            {
                length = len.ConvertEmToPixels(GetEmHeight()).ToString();
            }
            return length;
        }
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
        protected void SetAllBorders(string style = null, string width = null, string color = null)
        {
            //assign values
            if (style != null)
            {
                BorderLeftStyle = BorderTopStyle = BorderRightStyle = BorderBottomStyle = CssUtils.GetBorderStyle(style);
            }
            if (width != null)
            {
                BorderLeftWidth = BorderTopWidth = BorderRightWidth = BorderBottomWidth = CssLength.MakeBorderLength(width);
            }
            if (color != null)
            {
                BorderLeftColor = BorderTopColor = BorderRightColor = BorderBottomColor = CssValueParser.GetActualColor(color);
            }
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
                //if (WordSpacing != CssConstants.Normal)
                //{
                //    string len = RegexParserUtils.Search(RegexParserUtils.CssLength, WordSpacing);
                //    _actualWordSpacing += CssValueParser.ParseLength(len, 1, this);
                //}
            }
        }

        /// <summary>
        /// Inherits inheritable values from specified box.
        /// </summary>
        /// <param name="everything">Set to true to inherit all CSS properties instead of only the ineritables</param>
        /// <param name="p">Box to inherit the properties</param>
        protected void InheritStyle(CssBox p, bool everything)
        {
            if (p != null)
            {
                //_borderSpacing = p._borderSpacing;
                //_borderCollapse = p._borderCollapse;
                this._borderSpacingH = p._borderSpacingH;
                this._borderSpacingV = p._borderSpacingV;
                this.BorderCollapse = p.BorderCollapse;
                _color = p._color;
                _emptyCells = p._emptyCells;
                this.WhiteSpace = p.WhiteSpace;
                this.CssVisibility = p.CssVisibility;
                //_textIndent = p._textIndent;
                this._textIndent = p._textIndent;
                //this.TextAlign = p._textAlign;
                this.CssTextAlign = p.CssTextAlign;
                this.VerticalAlign = p.VerticalAlign;
                _fontFamily = p._fontFamily;
                _fontSize = p._fontSize;
                _fontStyle = p._fontStyle;
                _fontVariant = p._fontVariant;
                _fontWeight = p._fontWeight;

                _listProps = p._listProps;

                _lineHeight = p._lineHeight;
                //_wordBreak = p.WordBreak;
                this.WordBreak = p.WordBreak;

                // _direction = p._direction;
                this.CssDirection = p.CssDirection;
                this.CssDirection = p.CssDirection;

                if (everything)
                {
                    _backgroundColor = p._backgroundColor;
                    _backgroundGradient = p._backgroundGradient;
                    _backgroundGradientAngle = p._backgroundGradientAngle;
                    _backgroundImage = p._backgroundImage;
                    _backgroundPosition = p._backgroundPosition;
                    _backgroundRepeat = p._backgroundRepeat;

                    //direct share border feature from parent
                    _borderProps = p._borderProps;//***

                    //_borderTopWidth = p._borderTopWidth;
                    //_borderRightWidth = p._borderRightWidth;
                    //_borderBottomWidth = p._borderBottomWidth;
                    //_borderLeftWidth = p._borderLeftWidth;

                    //_borderTopColor = p._borderTopColor;
                    //_borderRightColor = p._borderRightColor;
                    //_borderBottomColor = p._borderBottomColor;
                    //_borderLeftColor = p._borderLeftColor;
                    this.BorderTopStyle = p.BorderTopStyle;
                    this.BorderLeftStyle = p.BorderLeftStyle;
                    this.BorderBottomStyle = p.BorderBottomStyle;
                    this.BorderRightStyle = p.BorderRightStyle;

                    //_borderTopStyle = p._borderTopStyle;
                    //_borderRightStyle = p._borderRightStyle;
                    //_borderBottomStyle = p._borderBottomStyle;
                    //_borderLeftStyle = p._borderLeftStyle;

                    _bottom = p._bottom;
                    _cornerProps = p._cornerProps;

                    //_cornerRadius = p._cornerRadius;
                    // _display = p._display;
                    this.CssDisplay = p.CssDisplay;
                    _float = p._float;
                    //_height = p._height;
                    this.Height = p.Height;
                    //_marginBottom = p._marginBottom;
                    //_marginLeft = p._marginLeft;
                    //_marginRight = p._marginRight;
                    //_marginTop = p._marginTop;
                    _left = p._left;
                    _lineHeight = p._lineHeight;
                    this.Overflow = p.Overflow;
                    //_paddingLeft = p._paddingLeft;
                    //_paddingBottom = p._paddingBottom;
                    //_paddingRight = p._paddingRight;
                    //_paddingTop = p._paddingTop;
                    _right = p._right;
                    //_textDecoration = p._textDecoration;
                    this.TextDecoration = p.TextDecoration;
                    _top = p._top;
                    //_position = p._position;
                    this.Position = p.Position;
                    //_width = p._width;
                    this.Width = p.Width;
                    this.MaxWidth = p.MaxWidth;
                    //_maxWidth = p._maxWidth;
                    this._wordSpacing = p._wordSpacing;
                    //_wordSpacing = p._wordSpacing;
                }
            }
        }
    }
}