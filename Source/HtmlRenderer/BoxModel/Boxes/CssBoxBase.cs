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

        internal int cssClassVersion;

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

        WellknownHtmlTagName wellKnownTagName;
        #endregion
        //==========================================================


        #region Fields

<<<<<<< HEAD
        //location, size
        //float _globlalX;
        //float _globalY;

        float _localX;
        float _localY;
=======
        //location, size 
>>>>>>> v1.7errs2
        float _sizeHeight;
        float _sizeWidth;

      


        //corner
        float _actualCornerNW;
        float _actualCornerNE;
        float _actualCornerSW;
        float _actualCornerSE;

        /// <summary>
        /// user's expected height
        /// </summary>
        float _expectedHight;
        /// <summary>
        /// user's expected width 
        /// </summary>
        float _expectedWidth;

        float _actualPaddingTop;
        float _actualPaddingBottom;
        float _actualPaddingRight;
        float _actualPaddingLeft;

        float _actualMarginTop;
        float _actualMarginBottom;
        float _actualMarginRight;
        float _actualMarginLeft;



        float _actualBorderTopWidth;
        float _actualBorderLeftWidth;
        float _actualBorderBottomWidth;
        float _actualBorderRightWidth;


        float _actualLineHeight;
        float _actualWordSpacing;
        float _actualTextIndent;

        float _actualBorderSpacingHorizontal;
        float _actualBorderSpacingVertical;

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
            set { this._cssDisplay = value; }
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
                this._prop_pass_eval &= ~CssBoxBaseAssignments.BORDER_WIDTH_LEFT;
            }
        }

        public CssLength BorderRightWidth
        {
            get { return this._borderProps.RightWidth; }
            set
            {
                CheckBorderVersion().RightWidth = value;
                this._prop_pass_eval &= ~CssBoxBaseAssignments.BORDER_WIDTH_RIGHT;
            }
        }

        public CssLength BorderBottomWidth
        {
            get { return this._borderProps.BottomWidth; }
            set
            {
                CheckBorderVersion().BottomWidth = value;
                this._prop_pass_eval &= ~CssBoxBaseAssignments.BORDER_WIDTH_BOTTOM;
            }
        }

        public CssLength BorderTopWidth
        {
            get { return this._borderProps.TopWidth; }
            set
            {
                CheckBorderVersion().TopWidth = value;
                this._prop_pass_eval &= ~CssBoxBaseAssignments.BORDER_WIDTH_TOP;
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
                this._prop_wait_eval |= CssBoxBaseAssignments.CORNER_NE;
            }
        }
        public CssLength CornerNWRadius
        {
            get { return this._cornerProps.NWRadius; }
            set
            {
                CheckCornerVersion().NWRadius = value;
                this._prop_wait_eval |= CssBoxBaseAssignments.CORNER_NW;
            }
        }
        public CssLength CornerSERadius
        {
            get { return this._cornerProps.SERadius; }
            set
            {
                CheckCornerVersion().SERadius = value;
                this._prop_wait_eval |= CssBoxBaseAssignments.CORNER_SE;
            }
        }
        public CssLength CornerSWRadius
        {
            get { return this._cornerProps.SWRadius; }
            set
            {
                CheckCornerVersion().SWRadius = value;
                this._prop_wait_eval |= CssBoxBaseAssignments.CORNER_SW;
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
                this._prop_wait_eval |= CssBoxBaseAssignments.PADDING_BOTTOM;
            }
        }

        public CssLength PaddingLeft
        {
            get { return this._paddingProps.Left; }
            set
            {
                CheckPaddingVersion().Left = value;
                this._prop_wait_eval |= CssBoxBaseAssignments.PADDING_LEFT;
            }
        }

        public CssLength PaddingRight
        {
            get { return this._paddingProps.Right; }
            set
            {
                CheckPaddingVersion().Right = value;
                this._prop_wait_eval |= CssBoxBaseAssignments.PADDING_RIGHT;
            }
        }

        public CssLength PaddingTop
        {
            get { return this._paddingProps.Top; }
            set
            {
                CheckPaddingVersion().Top = value;
                this._prop_wait_eval |= CssBoxBaseAssignments.PADDING_TOP;
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
      

        //----------------------------------------------------
        public CssLength LineHeight
        {
            get { return _lineHeight; }
            set
            {
                _lineHeight = value;
                this._prop_pass_eval &= ~CssBoxBaseAssignments.LINE_HEIGHT;
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
<<<<<<< HEAD
        }
        public float LocalX
        {
            get { return this._localX; }
        }
        public float LocalY
        {
            get { return this._localY; }
        }
        //public float GlobalX
        //{
        //    get { return this._globlalX; }
        //}
        //public float GlobalY
        //{
        //    get { return this._globalY; }
        //}
        public void Offset(float dx, float dy)
        {
            //this._globlalX += dx;
            //this._globalY += dy;
            this._localX += dx;
            this._localY += dy;

            this._baseCompactFlags |= CssBoxFlagsConst.HAS_ASSIGNED_LOCATION;
        }

        /// <summary>
        /// set location related to container box
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetLocation(float x, float y)
        {
            //this._globlalX = x;
            //this._globalY = y;

            this._localX = x;
            this._localY = y;

            this._baseCompactFlags |= CssBoxFlagsConst.HAS_ASSIGNED_LOCATION;
        }
        /// <summary>
        /// Gets or sets the size of the box
        /// </summary>
        public SizeF Size
        {
            get { return new SizeF(this._sizeWidth, this._sizeHeight); }
        }
        public void SetSize(float width, float height)
        {
            this._sizeWidth = width;
            this._sizeHeight = height;
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

        ///// <summary>
        ///// Gets the bounds of the box
        ///// </summary>
        //public RectangleF GlobalBound
        //{
        //    get { return new RectangleF(new PointF(this.GlobalX, this.GlobalY), Size); }
        //}
        public RectangleF LocalBound
        {
            get { return new RectangleF(new PointF(this.LocalX, this.LocalY), Size); }
        }
        /// <summary>
        /// Gets the width available on the box, counting padding and margin.
        /// </summary>
        public float AvailableWidth
        {
            get { return this.SizeWidth - ActualBorderLeftWidth - ActualPaddingLeft - ActualPaddingRight - ActualBorderRightWidth; }
        }

        ///// <summary>
        ///// Gets the right of the box. When setting, it will affect only the width of the box.
        ///// </summary>
        //public float GlobalActualRight
        //{
        //    get { return GlobalX + this.SizeWidth; }
        //}
        public float LocalActualRight
        {
            get { return this.LocalX + this.SizeWidth; }
        }

        //public void SetGlobalActualRight(float value)
        //{
        //    this._sizeWidth = value - GlobalX;
        //}
        public void SetLocalActualRight(float value)
        {
            this._sizeWidth = value - this.LocalX;
        }

        ///// <summary>
        ///// Gets or sets the bottom of the box. 
        ///// (When setting, alters only the Size.Height of the box)
        ///// </summary>
        //public float GlobalActualBottom
        //{
        //    get { return this.GlobalY + this._sizeHeight; }
        //}
        public float LocalActualBottom
        {
            get { return this.LocalY + this._sizeHeight; }
        }
        //public void SetGlobalActualBottom(float value)
        //{
        //    this._sizeHeight = value - this._globalY;
        //}
        public void SetLocalActualBottom(float value)
        {
            this._sizeHeight = value - this._localY;
        }
        protected void SetHeight(float height)
        {
            this._sizeHeight = height;
        }
        internal void UpdateIfHigher(float newHeight)
        {
            if (newHeight > this._sizeHeight)
            {
                this._sizeHeight = newHeight;
            }
        }


        ///// <summary>
        ///// Gets the left of the client rectangle (Where content starts rendering)
        ///// </summary>
        //public float GlobalClientLeft
        //{
        //    get { return this.GlobalX + ActualBorderLeftWidth + ActualPaddingLeft; }
        //}
        public float LocalClientLeft
        {
            get { return ActualBorderLeftWidth + ActualPaddingLeft; }
        }
        ///// <summary>
        ///// Gets the right of the client rectangle
        ///// </summary>
        //public float GlobalClientRight
        //{
        //    get { return GlobalActualRight - ActualPaddingRight - ActualBorderRightWidth; }
        //}
        public float LocalClientRight
        {
            get { return ActualPaddingRight - ActualBorderRightWidth; }
        }
        //public float GlobalClientTop
        //{
        //    get { return this.GlobalY + this.LocalClientTop; }
        //}
        public float LocalClientTop
        {
            get { return ActualBorderTopWidth + ActualPaddingTop; }
        }
        /// <summary>
        /// Gets the bottom of the client rectangle
        /// </summary>
        //public float GlobalClientBottom
        //{
        //    get { return this.GlobalActualBottom - ActualPaddingBottom - ActualBorderBottomWidth; }
        //}
        public float LocalClientBottom
        {
            get { return this.LocalActualBottom - ActualPaddingBottom - ActualBorderBottomWidth; }
        }
        /// <summary>
        /// Gets the client rectangle
        /// </summary>
        public RectangleF ClientRectangle
        {
            get
            {
                return RectangleF.FromLTRB(this.LocalClientLeft,
                     this.LocalClientTop, this.LocalClientRight,
                     this.LocalClientBottom);

            }
        }

        public float ClientWidth
        {
            get { return this.LocalClientRight - this.LocalClientLeft; }
=======
>>>>>>> v1.7errs2
        }
       
        /// <summary>
        /// Gets the actual height
        /// </summary>
        public float ExpectedHeight
        {
            get
            {
                if ((this._prop_pass_eval & CssBoxBaseAssignments.HEIGHT) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.HEIGHT;
                    return _expectedHight = CssValueParser.ParseLength(Height, this.SizeHeight, this);
                }
                return _expectedHight;
            }
        }

        /// <summary>
        /// Gets the actual height
        /// </summary>
        public float ExpectedWidth
        {
            get
            {
                if ((this._prop_pass_eval & CssBoxBaseAssignments.WIDTH) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.WIDTH;
                    return _expectedWidth = CssValueParser.ParseLength(Width, this.SizeWidth, this);
                }
                return _expectedWidth;
            }
        }

        /// <summary>
        /// Gets the actual top's padding
        /// </summary>
        public float ActualPaddingTop
        {
            get
            {
                if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_TOP) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_TOP;
                    return _actualPaddingTop = CssValueParser.ParseLength(PaddingTop, this.SizeWidth, this);
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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_LEFT) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_LEFT;
                    return this._actualPaddingLeft = CssValueParser.ParseLength(PaddingLeft, this.SizeWidth, this);
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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_BOTTOM) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_BOTTOM;
                    return _actualPaddingBottom = CssValueParser.ParseLength(PaddingBottom, this.SizeWidth, this);
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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.PADDING_RIGHT) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.PADDING_RIGHT;
                    return _actualPaddingRight = CssValueParser.ParseLength(PaddingRight, SizeWidth, this);
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


                if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_TOP) == 0)
                {
                    if (this.MarginTop.IsAuto)
                    {
                        MarginTop = CssLength.ZeroPx;
                    }

                    var value = CssValueParser.ParseLength(MarginTop, this.SizeWidth, this);

                    if (this.MarginLeft.IsPercentage)
                    {
                        return value;
                    }

                    this._prop_pass_eval = CssBoxBaseAssignments.MARGIN_TOP;
                    return this._actualMarginTop = value;
                }
                return _actualMarginTop;

            }
        }

        /// <summary>
        /// Gets the actual Margin on the left
        /// </summary>
        public float ActualMarginLeft
        {
            get
            {
                if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_LEFT) == 0)
                {
                    if (MarginLeft.IsAuto)
                    {
                        MarginLeft = CssLength.ZeroPx;
                    }
                    var value = CssValueParser.ParseLength(MarginLeft, this.SizeWidth, this);
                    if (this.MarginLeft.IsPercentage)
                    {
                        return value;
                    }

                    this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_LEFT;
                    return _actualMarginLeft = value;
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

                if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_BOTTOM) == 0)
                {
                    if (MarginBottom.IsAuto)
                    {
                        MarginBottom = CssLength.ZeroPx;
                    }

                    var value = CssValueParser.ParseLength(MarginBottom, this.SizeWidth, this);

                    if (MarginLeft.IsPercentage)
                    {
                        return value;
                    }
                    this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_BOTTOM;
                    return this._actualMarginBottom = value;
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.MARGIN_RIGHT) == 0)
                {
                    if (MarginRight.IsAuto)
                    {
                        MarginRight = CssLength.ZeroPx;
                    }
                    var value = CssValueParser.ParseLength(MarginRight, this.SizeWidth, this);
                    if (MarginLeft.IsPercentage)
                    {
                        return value;
                    }
                    this._prop_pass_eval |= CssBoxBaseAssignments.MARGIN_RIGHT;
                    return this._actualMarginRight = value;
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_TOP) == 0)
                {
                    //need evaluate
                    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_TOP;
                    return (this.BorderTopStyle == CssBorderStyle.None) ?
                        _actualBorderTopWidth = 0f :
                        _actualBorderTopWidth = CssValueParser.GetActualBorderWidth(BorderTopWidth, this);
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

                if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_LEFT) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_LEFT;
                    return (this.BorderLeftStyle == CssBorderStyle.None) ?
                        _actualBorderLeftWidth = 0f :
                        _actualBorderLeftWidth = CssValueParser.GetActualBorderWidth(BorderLeftWidth, this);
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_BOTTOM) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_BOTTOM;
                    return (this.BorderBottomStyle == CssBorderStyle.None) ?
                        _actualBorderBottomWidth = 0f :
                        _actualBorderBottomWidth = CssValueParser.GetActualBorderWidth(BorderBottomWidth, this);

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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_WIDTH_RIGHT) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_WIDTH_RIGHT;
                    return (this.BorderRightStyle == CssBorderStyle.None) ?
                        _actualBorderRightWidth = 0f :
                        _actualBorderRightWidth = CssValueParser.GetActualBorderWidth(BorderRightWidth, this);

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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_NW) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_NW;
                    return _actualCornerNW = CssValueParser.ParseLength(CornerNWRadius, 0, this);
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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_NE) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_NE;
                    return _actualCornerNE = CssValueParser.ParseLength(CornerNERadius, 0, this);
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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_SE) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_SE;
                    return _actualCornerSE = CssValueParser.ParseLength(CornerSERadius, 0, this);
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
                if ((this._prop_wait_eval & CssBoxBaseAssignments.CORNER_SW) != 0)
                {
                    this._prop_wait_eval &= ~CssBoxBaseAssignments.CORNER_SW;
                    return _actualCornerSW = CssValueParser.ParseLength(CornerSWRadius, 0, this);
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.LINE_HEIGHT) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.LINE_HEIGHT;
                    _actualLineHeight = .9f * CssValueParser.ParseLength(LineHeight, this.SizeHeight, this);
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.TEXT_INDENT) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.TEXT_INDENT;
                    _actualTextIndent = CssValueParser.ParseLength(TextIndent, this.SizeWidth, this);
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_SPACING_H) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_SPACING_H;
                    _actualBorderSpacingHorizontal = this.BorderSpacingHorizontal.Number;
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
                if ((this._prop_pass_eval & CssBoxBaseAssignments.BORDER_SPACING_V) == 0)
                {
                    this._prop_pass_eval |= CssBoxBaseAssignments.BORDER_SPACING_V;
                    _actualBorderSpacingVertical = this.BorderSpacingVertical.Number;
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

        protected void MeasureWordSpacing(IGraphics g)
        {
            if ((this._prop_pass_eval & CssBoxBaseAssignments.WORD_SPACING) == 0)
            {
                this._prop_pass_eval |= CssBoxBaseAssignments.WORD_SPACING;

                _actualWordSpacing = CssUtils.MeasureWhiteSpace(g, this);
                if (!this.WordSpacing.IsNormalWordSpacing)
                {
                    _actualWordSpacing += CssValueParser.ParseLength(this.WordSpacing, 1, this);
                }
            }
        }
        /// <summary>
        /// Gets or sets the size of the box
        /// </summary>
        public SizeF Size
        {
            get { return new SizeF(this._sizeWidth, this._sizeHeight); }
        }
        public void SetSize(float width, float height)
        {
            this._sizeWidth = width;
            this._sizeHeight = height;
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
        protected void SetHeight(float height)
        {
            this._sizeHeight = height;
        }
        internal void UpdateIfHigher(float newHeight)
        {
            if (newHeight > this._sizeHeight)
            {
                this._sizeHeight = newHeight;
            }
        }
    }
}