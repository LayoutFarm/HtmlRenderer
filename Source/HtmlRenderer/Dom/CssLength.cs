using System;
using System.Globalization;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Represents and gets info about a CSS Length
    /// </summary>
    /// <remarks>
    /// http://www.w3.org/TR/CSS21/syndata.html#length-units
    /// </remarks>
    public struct CssLength
    {
        const int IS_AUTO = 1 << (10 - 1);
        const int IS_PERCENT = 1 << (11 - 1);
        const int IS_RELATIVE = 1 << (12 - 1);
        const int HAS_ERROR = 1 << (13 - 1);
        const int IS_ASSIGN = 1 << (14 - 1);
        const int NONE_VALUE = 1 << (15 - 1);
        const int NORMAL = 1 << (16 - 1);

        const int MEDIUM = 1 << (17 - 1);
        const int THICK = 1 << (18 - 1);
        const int THIN = 1 << (19 - 1);
         

        #region Fields
        private readonly float _number;
        readonly int _flags;


        //private readonly CssUnit _unit;
        //private readonly bool _isPercentage;
        //private readonly bool _hasError;
        //private readonly bool _isRelative;
        //readonly bool _isAuto;
        //readonly bool _isAssign;


        #endregion

        public static readonly CssLength AutoLength = new CssLength(0, IS_ASSIGN | IS_AUTO);
        public static readonly CssLength NotAssign = new CssLength(0, 0);
        public static readonly CssLength NormalWordOrLine = new CssLength(0, IS_ASSIGN | NORMAL);
        

        public static readonly CssLength Medium = new CssLength(0, IS_ASSIGN | MEDIUM);
        public static readonly CssLength Thick = new CssLength(0, IS_ASSIGN | THICK);
        public static readonly CssLength Thin = new CssLength(0, IS_ASSIGN | THIN);

        public static readonly CssLength ZeroNoUnit = CssLength.MakeZeroLengthNoUnit();
        public static readonly CssLength ZeroPx = CssLength.MakePixelLength(0);

        #region Ctor
        /// <summary>
        /// Creates a new CssLength from a length specified on a CSS style sheet or fragment
        /// </summary>
        /// <param name="length">Length as specified in the Style Sheet or style fragment</param>
        public CssLength(string length)
        {
            //_length = length;
            _number = 0f;
            //_unit = CssUnit.None; 
            this._flags = (int)CssUnit.None | IS_ASSIGN;

            if (length == "auto")
            {
                this._flags |= IS_AUTO;
                return;
            }
            else if (length == "normal")
            {
                this._flags |= NORMAL;
                return;
            } 

            //_isPercentage = false;
            //_isRelative = true;
            //_hasError = false;
            //_isAuto = length == "auto";
            //_isAssign = true;


            //Return zero if no length specified, zero specified
            if (string.IsNullOrEmpty(length) || length == "0")
            {
                return;
            }

            //If percentage, use ParseNumber
            if (length.EndsWith("%"))
            {
                //_number = CssValueParser.ParseNumber(length, 1);
                _number = float.Parse(length.Substring(0, length.Length - 1));
                this._flags |= IS_PERCENT;
                //_isPercentage = true;
                return;
            }

            //If no units, has error
            if (length.Length < 3)
            {
                float.TryParse(length, out _number);
                this._flags |= HAS_ERROR;
                //_hasError = true;
                return;
            }

            //Get units of the length
            string u = length.Substring(length.Length - 2, 2);

            //Number of the length
            string number = length.Substring(0, length.Length - 2);

            //TODO: Units behave different in paper and in screen!
            switch (u)
            {
                case CssConstants.Em:
                    //_unit = CssUnit.Ems;
                    //_isRelative = true;
                    this._flags |= (int)CssUnit.Ems | IS_RELATIVE;
                    break;
                case CssConstants.Ex:
                    //_unit = CssUnit.Ex;
                    //_isRelative = true;
                    this._flags |= (int)CssUnit.Ex | IS_RELATIVE;
                    break;
                case CssConstants.Px:
                    //_unit = CssUnit.Pixels;
                    //_isRelative = true;
                    this._flags |= (int)CssUnit.Pixels | IS_RELATIVE;
                    break;
                case CssConstants.Mm:
                    this._flags |= (int)CssUnit.Milimeters;
                    break;
                case CssConstants.Cm:
                    this._flags |= (int)CssUnit.Centimeters;
                    break;
                case CssConstants.In:
                    this._flags |= (int)CssUnit.Inches;
                    break;
                case CssConstants.Pt:
                    this._flags |= (int)CssUnit.Points;
                    break;
                case CssConstants.Pc:
                    this._flags |= (int)CssUnit.Picas;
                    break;
                default:
                    //_hasError = true;
                    this._flags |= HAS_ERROR;
                    return;
            }

            if (!float.TryParse(number, System.Globalization.NumberStyles.Number, NumberFormatInfo.InvariantInfo, out _number))
            {
                this._flags |= HAS_ERROR;
                //_hasError = true;
            }
        }
        public CssLength(float number, bool isPercent, CssUnit unit)
        {
            this._number = number;
            this._flags = (int)unit | IS_ASSIGN;
            if (isPercent)
            {
                this._flags |= IS_PERCENT;
            }
            //this._isAssign = true;            
            //this._isPercentage = isPercent;
            //this._hasError = false;
            //this._isAuto = false;

            switch (unit)
            {
                case CssUnit.Pixels:
                case CssUnit.Ems:
                case CssUnit.Ex:
                    this._flags |= IS_RELATIVE;
                    //this._isRelative = true;
                    break;
                default:
                    //this._isRelative = false;
                    break;
            }
        }

        public CssLength(CssLength another)
        {
            this._flags = another._flags;
            this._number = another._number;

        }
        private CssLength(float num, CssUnit unit, bool isRelative)
        {
            this._number = num;

            this._flags = (int)unit | IS_ASSIGN;
            if (isRelative)
            {
                this._flags |= IS_RELATIVE;
            }
            //this._isAuto = false; 
            //this._isRelative = isRelative;
            //this._isPercentage = false;
            //this._hasError = false;
            //this._isAssign = true;
        }
        private CssLength(int number, int internalFlags)
        {
            this._number = number;
            this._flags = internalFlags; // (int)CssUnit.Pixels | IS_ASSIGN;
            //if (isAuto)
            //{
            //    this._flags |= IS_AUTO;
            //}
            //this._isAuto = isAuto;
            //this._isRelative = false;
            //this._isPercentage = false;
            //this._hasError = false;
            //this._isAssign = true;
        }

        #endregion


        #region Props

        public static CssLength MakeBorderLength(string str)
        {
            switch (str)
            {
                case CssConstants.Medium:
                    return CssLength.Medium;
                case CssConstants.Thick:
                    return CssLength.Thick;
                case CssConstants.Thin:
                    return CssLength.Thin;
            }
            return new CssLength(str);
        }
        public static CssLength MakePixelLength(float pixel)
        {
            return new CssLength(pixel, CssUnit.Pixels, true);
        }
        public static CssLength MakeZeroLengthNoUnit()
        {
            return new CssLength(0, CssUnit.None, true);
        }

        /// <summary>
        /// Gets the number in the length
        /// </summary>
        public float Number
        {
            get { return _number; }
        }
        public bool IsMedium
        {
            get { return (this._flags & MEDIUM) != 0; }
        }
        public bool IsThick
        {
            get { return (this._flags & THICK) != 0; }
        }
        public bool IsThin
        {
            get { return (this._flags & THIN) != 0; }
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
            //get { return _isPercentage; }
            get { return (this._flags & IS_PERCENT) != 0; }
        }


        public bool IsAuto
        {
            get { return (this._flags & IS_AUTO) != 0; }
            //get
            //{

            //    return this._isAuto;
            //}
        }
        public bool IsEmpty
        {
            get { return (this._flags & IS_ASSIGN) == 0; }
            //get
            //{
            //    return !this._isAssign;
            //}
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
            //get { return _isRelative; }
        }

        /// <summary>
        /// Gets the unit of the length
        /// </summary>
        public CssUnit Unit
        {
            get { return (CssUnit)(this._flags & 0xFF); }
        }

        ///// <summary>
        ///// Gets the length as specified in the string
        ///// </summary>
        //public string Length
        //{
        //    get { return _length; }
        //}


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

            return new CssLength(Number * emSize, CssUnit.Points, false);
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

            return new CssLength(Number * pixelFactor, CssUnit.Pixels, true);
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
            else if (IsPercentage)
            {
                return string.Format(NumberFormatInfo.InvariantInfo, "{0}%", Number);
            }
            else
            {
                string u = string.Empty;

                switch (Unit)
                {
                    case CssUnit.None:
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