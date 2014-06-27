//BSD 2014,WinterCore
//ArthurHub

using System;
using System.Globalization;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;

namespace HtmlRenderer.Dom
{
    public static class CssFontSizeConst
    {
        public const byte FONTSIZE_MEDIUM = 10;
        public const byte FONTSIZE_XX_SMALL = 11;
        public const byte FONTSIZE_X_SMALL = 12;
        public const byte FONTSIZE_SMALL = 13;
        public const byte FONTSIZE_LARGE = 14;
        public const byte FONTSIZE_X_LARGE = 15;
        public const byte FONTSIZE_XX_LARGE = 16;
        public const byte FONTSIZE_SMALLER = 17;
        public const byte FONTSIZE_LARGER = 18;
    }
    public static class CssBackgroundPositionConst
    {
        public const byte LEFT = 10;
        public const byte TOP = 11;
        public const byte RIGHT = 12;
        public const byte BOTTOM = 13;
        public const byte CENTER = 14;
    }
    public static class CssBorderThickName
    {
        public const byte MEDIUM = 10;
        public const byte THICK = 11;
        public const byte THIN = 12;
    }



    /// <summary>
    /// Represents and gets info about a CSS Length
    /// </summary>
    /// <remarks>
    /// http://www.w3.org/TR/CSS21/syndata.html#length-units
    /// </remarks>
    public struct CssLength
    {
        //has 2 instance fields
        //================================     

        //8 least sig bits (1 byte) : store CssUnit,or some predefined CssValue (like border-thickness,font name,background-pos)
        //24 upper bits (3 byte) : store other flags
        readonly int _flags;
        //for number
        readonly float _number;
        //================================  

        //for upper 24 bits of _flags
        public const int IS_ASSIGN = 1 << (11 - 1);
        public const int IS_AUTO = 1 << (12 - 1);
        public const int IS_RELATIVE = 1 << (13 - 1);
        public const int NORMAL = 1 << (14 - 1);
        public const int NONE_VALUE = 1 << (15 - 1);
        public const int HAS_ERROR = 1 << (16 - 1);
        //-------------------------------------
        //when used as border thickeness name
        public const int IS_BORDER_THICKNESS_NAME = 1 << (20 - 1);
        //when used as font size
        public const int IS_FONT_SIZE_NAME = 1 << (21 - 1);
        //------------------------------------- 
        //when used as background position
        public const int IS_BACKGROUND_POS_NAME = 1 << (22 - 1);
        //-------------------------------------   

        public static readonly CssLength AutoLength = new CssLength(IS_ASSIGN | IS_AUTO, CssUnit.AutoLength);
        public static readonly CssLength NotAssign = new CssLength(0);
        public static readonly CssLength NormalWordOrLine = new CssLength(IS_ASSIGN | NORMAL, CssUnit.NormalLength);


        public static readonly CssLength ZeroNoUnit = CssLength.MakeZeroLengthNoUnit();
        public static readonly CssLength ZeroPx = CssLength.MakePixelLength(0);

        //-----------------------------------------------------------------------------------------
        public static readonly CssLength Medium = new CssLength(IS_ASSIGN | IS_BORDER_THICKNESS_NAME | CssBorderThickName.MEDIUM, CssUnit.BorderMedium);
        public static readonly CssLength Thick = new CssLength(IS_ASSIGN | IS_BORDER_THICKNESS_NAME | CssBorderThickName.THICK, CssUnit.BorderThick);
        public static readonly CssLength Thin = new CssLength(IS_ASSIGN | IS_BORDER_THICKNESS_NAME | CssBorderThickName.THIN, CssUnit.BorderThin);
        //-----------------------------------------------------------------------------------------
        public static readonly CssLength FontSizeMedium = new CssLength(IS_ASSIGN | IS_FONT_SIZE_NAME | CssFontSizeConst.FONTSIZE_MEDIUM);//default
        public static readonly CssLength FontSizeXXSmall = new CssLength(IS_ASSIGN | IS_FONT_SIZE_NAME | CssFontSizeConst.FONTSIZE_XX_SMALL);
        public static readonly CssLength FontSizeXSmall = new CssLength(IS_ASSIGN | IS_FONT_SIZE_NAME | CssFontSizeConst.FONTSIZE_X_SMALL);
        public static readonly CssLength FontSizeSmall = new CssLength(IS_ASSIGN | IS_FONT_SIZE_NAME | CssFontSizeConst.FONTSIZE_SMALL);
        public static readonly CssLength FontSizeLarge = new CssLength(IS_ASSIGN | IS_FONT_SIZE_NAME | CssFontSizeConst.FONTSIZE_LARGE);
        public static readonly CssLength FontSizeXLarge = new CssLength(IS_ASSIGN | IS_FONT_SIZE_NAME | CssFontSizeConst.FONTSIZE_X_LARGE);
        public static readonly CssLength FontSizeXXLarge = new CssLength(IS_ASSIGN | IS_FONT_SIZE_NAME | CssFontSizeConst.FONTSIZE_XX_LARGE);

        public static readonly CssLength FontSizeSmaller = new CssLength(IS_ASSIGN | IS_FONT_SIZE_NAME | CssFontSizeConst.FONTSIZE_SMALLER);
        public static readonly CssLength FontSizeLarger = new CssLength(IS_ASSIGN | IS_FONT_SIZE_NAME | CssFontSizeConst.FONTSIZE_LARGE);
        //-----------------------------------------------------------------------------------------
        public static readonly CssLength BackgroundPosLeft = new CssLength(IS_ASSIGN | IS_BACKGROUND_POS_NAME | CssBackgroundPositionConst.LEFT);
        public static readonly CssLength BackgroundPosTop = new CssLength(IS_ASSIGN | IS_BACKGROUND_POS_NAME | CssBackgroundPositionConst.TOP);
        public static readonly CssLength BackgroundPosRight = new CssLength(IS_ASSIGN | IS_BACKGROUND_POS_NAME | CssBackgroundPositionConst.RIGHT);
        public static readonly CssLength BackgroundPosBottom = new CssLength(IS_ASSIGN | IS_BACKGROUND_POS_NAME | CssBackgroundPositionConst.BOTTOM);
        public static readonly CssLength BackgroundPosCenter = new CssLength(IS_ASSIGN | IS_BACKGROUND_POS_NAME | CssBackgroundPositionConst.CENTER);
        //-----------------------------------------------------------------------------------------


        #region Ctor


        public CssLength(float num, CssUnit unit)
        {
            this._number = num;
            this._flags = (int)unit | IS_ASSIGN;
            switch (unit)
            {
                case CssUnit.Pixels:
                case CssUnit.Ems:
                case CssUnit.Ex:
                case CssUnit.EmptyValue:
                    this._flags |= IS_RELATIVE;
                    break;
                case CssUnit.Unknown:
                    this._flags |= HAS_ERROR;
                    return;
                default:
                    break;
            }
        }
        private CssLength(int internalFlags)
        {
            this._number = 0;
            this._flags = internalFlags; // (int)CssUnit.Pixels | IS_ASSIGN; 
        }

        #endregion


        #region Props


        public static CssUnit GetCssUnit(string u)
        {
            switch (u)
            {
                case CssConstants.Em:
                    return CssUnit.Ems;
                case CssConstants.Ex:
                    return CssUnit.Ex;
                case CssConstants.Px:
                    return CssUnit.Pixels;
                case CssConstants.Mm:
                    return CssUnit.Milimeters;
                case CssConstants.Cm:
                    return CssUnit.Centimeters;
                case CssConstants.In:
                    return CssUnit.Inches;
                case CssConstants.Pt:
                    return CssUnit.Points;
                case CssConstants.Pc:
                    return CssUnit.Picas;
                default:
                    return CssUnit.Unknown;
            }
        }

        public static CssLength MakePixelLength(float pixel)
        {
            return new CssLength(pixel, CssUnit.Pixels);
        }
        public static CssLength MakeZeroLengthNoUnit()
        {
            return new CssLength(0, CssUnit.EmptyValue);
        }
        public static CssLength MakeFontSizePtUnit(float pointUnit)
        {
            return new CssLength(pointUnit, CssUnit.Points);
        }
        /// <summary>
        /// Gets the number in the length
        /// </summary>
        public float Number
        {
            get { return _number; }
        }

        /// <summary>
        /// Gets if the length has some parsing error
        /// </summary>
        public bool HasError
        {
            // get { return _hasError; }
            get { return (this._flags & HAS_ERROR) != 0; }
        }


        /// <summary>
        /// Gets if the length represents a precentage (not actually a length)
        /// </summary>
        public bool IsPercentage
        {

            get { return this.Unit == CssUnit.Percent; }
        }
        public bool IsAuto
        {
            get { return (this.Unit == CssUnit.AutoLength); }
        }
        public bool IsEmpty
        {
            get { return (this._flags & IS_ASSIGN) == 0; }
        }
        public bool IsEmptyOrAuto
        {
            //range usage *** 
            get { return this.Unit <= CssUnit.AutoLength; }
        }
        public bool IsNormalWordSpacing
        {
            get
            {
                return (this._flags & NORMAL) != 0;
            }
        }
        public bool IsNormalLineHeight
        {
            get
            {
                return (this._flags & NORMAL) != 0;
            }
        }
        /// <summary>
        /// Gets if the length is specified in relative units
        /// </summary>
        public bool IsRelative
        {
            get { return (this._flags & IS_RELATIVE) != 0; }

        }

        /// <summary>
        /// Gets the unit of the length
        /// </summary>
        public CssUnit Unit
        {
            get { return (CssUnit)(this._flags & 0xFF); }
        }

        //-------------------------------------------------
        public bool IsFontSizeName
        {
            get { return (this._flags & IS_FONT_SIZE_NAME) != 0; }
        }
        public int FontSizeName
        {
            get { return (int)(this._flags & 0xFF); }
        }
        //-------------------------------------------------
        public bool IsBackgroundPositionName
        {
            get { return (this._flags & IS_BACKGROUND_POS_NAME) != 0; }
        }
        public int BackgroundPositionName
        {
            get { return (int)(this._flags & 0xFF); }
        }
        //-------------------------------------------------
        public bool IsBorderThicknessName
        {
            get { return (this._flags & IS_BORDER_THICKNESS_NAME) != 0; }
        }
        public int BorderThicknessName
        {
            get { return (int)(this._flags & 0xFF); }
        }
        #endregion

        #region Methods

        /// <summary>
        /// If length is in Ems, returns its value in points
        /// </summary>
        /// <param name="emSize">Em size factor to multiply</param>
        /// <returns>Points size of this em</returns>
        /// <exception cref="InvalidOperationException">If length has an error or isn't in ems</exception>
        public CssLength ConvertEmToPoints(float emSize)
        {
            if (HasError) throw new InvalidOperationException("Invalid length");
            if (Unit != CssUnit.Ems) throw new InvalidOperationException("Length is not in ems");

            return new CssLength(Number * emSize, CssUnit.Points);
            //return new CssLength(string.Format("{0}pt", Convert.ToSingle(Number * emSize).ToString("0.0", NumberFormatInfo.InvariantInfo)));
        }

        /// <summary>
        /// If length is in Ems, returns its value in pixels
        /// </summary>
        /// <param name="pixelFactor">Pixel size factor to multiply</param>
        /// <returns>Pixels size of this em</returns>
        /// <exception cref="InvalidOperationException">If length has an error or isn't in ems</exception>
        public CssLength ConvertEmToPixels(float pixelFactor)
        {
            if (HasError) throw new InvalidOperationException("Invalid length");
            if (Unit != CssUnit.Ems) throw new InvalidOperationException("Length is not in ems");

            return new CssLength(Number * pixelFactor, CssUnit.Pixels);
            //return new CssLength(string.Format("{0}px", Convert.ToSingle(Number * pixelFactor).ToString("0.0", NumberFormatInfo.InvariantInfo)));
        }

        /// <summary>
        /// Returns the length formatted ready for CSS interpreting.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (HasError)
            {
                return string.Empty;
            }
            else
            {
                string u = string.Empty;

                switch (Unit)
                {
                    case CssUnit.Percent:
                        return string.Format(NumberFormatInfo.InvariantInfo, "{0}%", Number);

                    case CssUnit.EmptyValue:
                        break;
                    case CssUnit.Ems:
                        u = "em";
                        break;
                    case CssUnit.Pixels:
                        u = "px";
                        break;
                    case CssUnit.Ex:
                        u = "ex";
                        break;
                    case CssUnit.Inches:
                        u = "in";
                        break;
                    case CssUnit.Centimeters:
                        u = "cm";
                        break;
                    case CssUnit.Milimeters:
                        u = "mm";
                        break;
                    case CssUnit.Points:
                        u = "pt";
                        break;
                    case CssUnit.Picas:
                        u = "pc";
                        break;

                }

                return string.Format(NumberFormatInfo.InvariantInfo, "{0}{1}", Number, u);
            }
        }

        #endregion
    }
}