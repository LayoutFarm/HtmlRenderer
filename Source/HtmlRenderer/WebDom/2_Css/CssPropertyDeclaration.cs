//BSD  2014 ,WinterCore


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace HtmlRenderer.WebDom
{

    public class CssPropertyDeclaration
    {
        bool isReady = false;
        bool isValid = false;
        bool markedAsInherit;
        bool isAutoGen = false;
        bool isExpand = false;
        CssCodePropertyValue propertyValue;
        List<CssCodePropertyValue> moreValues;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public CssPropertyDeclaration(string propertyName)
        {

#if DEBUG
#endif
            this.PropertyName = propertyName;
        }
        internal CssPropertyDeclaration(string propertyName, CssCodePropertyValue propvalue)
        {

#if DEBUG
#endif
            //from another 
            this.PropertyName = propertyName;
            this.propertyValue = propvalue;
            this.markedAsInherit = propvalue.Value == "inherit";
            //auto gen from another prop
            this.isAutoGen = true;
        }
        internal bool IsExpand
        {
            get { return this.isExpand; }
            set { this.isExpand = value; }
        }
        public void AddValue(CssCodePropertyValue value)
        {
            if (propertyValue == null)
            {
                markedAsInherit = value.Value == "inherit";
                this.propertyValue = value;
            }
            else
            {
                if (moreValues == null)
                {
                    moreValues = new List<CssCodePropertyValue>();
                }
                moreValues.Add(value);
                markedAsInherit = false;
            }
        }

        public string PropertyName
        {
            get;
            private set;
        }



        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(this.PropertyName);
            sb.Append(':');

            CollectValues(sb);

            return sb.ToString();
        }
        void CollectValues(StringBuilder stBuilder)
        {
            if (propertyValue != null)
            {
                stBuilder.Append(propertyValue.ToString());
            }
            if (moreValues != null)
            {
                int j = moreValues.Count;
                for (int i = 0; i < j; ++i)
                {
                    CssCodePropertyValue propV = moreValues[i];
                    stBuilder.Append(propV.ToString());
                    if (i < j - 1)
                    {
                        stBuilder.Append(' ');
                    }
                }
            }
        }

        public bool MarkedAsInherit
        {
            get
            {
                return this.markedAsInherit;
            }
        }
        public int ValueCount
        {
            get
            {
                if (moreValues == null)
                {
                    if (propertyValue == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return moreValues.Count + 1;
                }
            }
        }


        /// <summary>
        /// Check if the given string is a valid length value.
        /// </summary>
        /// <param name="value">the string value to check</param>
        /// <returns>true - valid, false - invalid</returns>
        public static bool IsValidLength(string value)
        {
            if (value.Length > 1)
            {
                string number = string.Empty;
                if (value.EndsWith("%"))
                {
                    number = value.Substring(0, value.Length - 1);
                }
                else if (value.Length > 2)
                {
                    number = value.Substring(0, value.Length - 2);
                }
                float stub;
                return float.TryParse(number, out stub);
            }
            return false;
        }

        void PrepareProperty()
        {
            return;
            //if (isReady)
            //{
            //    return;
            //}
            //isReady = true;

            //switch (this.PropertyName)
            //{
            //    case "width":
            //    case "height":
            //    case "lineheight":
            //        {
            //            ParseLengthProperty(this);
            //        } break;
            //    case "color":
            //    case "backgroundcolor":
            //    case "bordertopcolor":
            //    case "borderbottomcolor":
            //    case "borderleftcolor":
            //    case "borderrightcolor":
            //        {
            //            //check if valid color
            //            //if valid then parse ***
            //            ///CssValueParser.IsColorValid(propValue) check if valid color
            //            //ParseColorProperty(propName, propValue, properties);
            //        } break;
            //    case "font":
            //        {
            //            // ParseFontProperty(this);

            //        } break;
            //    //case "border":
            //    //    {
            //    //        ParseBorderProperty(propValue, null, properties);
            //    //    } break;
            //    //case "border-left":
            //    //    {
            //    //        ParseBorderProperty(propValue, "-left", properties);
            //    //    } break;
            //    //case "border-right":
            //    //    {
            //    //        ParseBorderProperty(propValue, "-right", properties);
            //    //    } break;
            //    //case "border-top":
            //    //    {
            //    //        ParseBorderProperty(propValue, "-top", properties);
            //    //    } break;
            //    //case "border-bottom":
            //    //    {
            //    //        ParseBorderProperty(propValue, "-bottom", properties);
            //    //    } break;
            //    //case "margin":
            //    //    {
            //    //        ParseMarginProperty(propValue, properties);
            //    //    } break;
            //    //case "border-style":
            //    //    {
            //    //        ParseBorderStyleProperty(propValue, properties);
            //    //    } break;
            //    //case "border-width":
            //    //    {
            //    //        ParseBorderWidthProperty(propValue, properties);
            //    //    } break;
            //    //case "border-color":
            //    //    {
            //    //        ParseBorderColorProperty(propValue, properties);
            //    //    } break;
            //    //case "padding":
            //    //    {
            //    //        ParsePaddingProperty(propValue, properties);
            //    //    } break;
            //    //case "background-image":
            //    //    {
            //    //        properties["background-image"] = new CssProperty(
            //    //            "background-image", ParseBackgroundImageProperty(propValue));
            //    //    } break;
            //    //case "font-family":
            //    //    {
            //    //        properties["font-family"] = new CssProperty(
            //    //            "font-family", ParseFontFamilyProperty(propValue));
            //    //    } break;
            //    default:
            //        {
            //            //properties[propName] = new CssProperty(propName, propValue);
            //        } break;
            //}
        }


        public CssCodePropertyValue GetPropertyValue(int index)
        {
            //if (!isReady)
            //{
            //    PrepareProperty();
            //}
            switch (index)
            {
                case 0:
                    {
                        return this.propertyValue;
                    }
                default:
                    {
                        if (moreValues != null)
                        {
                            return moreValues[index - 1];
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
            }
        }
    }

    public enum CssValueHint : byte
    {
        Unknown,
        Number,
        NumberWithUnit,
        HexColor,
        LiteralString,
        Iden,

    }
    public class CssCodePropertyValue
    {

        string unit;
        readonly string _propertyValue;
        readonly CssValueHint hint;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif

        public CssCodePropertyValue(string propertyValue, CssValueHint hint)
        {
            this._propertyValue = propertyValue;
            this.hint = hint;
        }
        public CssValueHint Hint
        {
            get { return this.hint; }
        }
        public string Unit
        {
            get { return unit; }
            set { this.unit = value; }
        }
        public string Value
        {
            get
            {
                return this._propertyValue;
            }
        }
        public override string ToString()
        {
            if (unit != null)
            {
                return Value + unit;
            }
            else
            {
                return Value;
            }
        }


        //--------------------------------------------
        bool isBorderLength;
        bool isLengthEval;
        bool isTranslatedLength;
        bool isColor;
        System.Drawing.Color cachedColor;
        //--------------------------------------------
        HtmlRenderer.Dom.CssLength cachedLength;
        internal HtmlRenderer.Dom.CssLength AsBorderLength()
        {
            if (!isBorderLength)
            {
                isBorderLength = true;
                return cachedLength = HtmlRenderer.Dom.CssLength.MakeBorderLength(this.ToString());
            }
            return cachedLength;
        }
        internal HtmlRenderer.Dom.CssLength AsLength()
        {
            if (!isLengthEval)
            {
                isLengthEval = true;
                return cachedLength = new Dom.CssLength(this.ToString());
            }
            return cachedLength;
        }
        internal HtmlRenderer.Dom.CssLength AsTranslatedLength()
        {
            if (!isTranslatedLength)
            {
                isTranslatedLength = true;
                return cachedLength = HtmlRenderer.Dom.BoxModelBuilder.TranslateLength(this.ToString());
            }
            return cachedLength;
        }

        internal System.Drawing.Color AsColor()
        {
            if (!isColor)
            {
                isColor = true;
                return this.cachedColor = Parse.CssValueParser.GetActualColor(this.ToString());
            }
            return this.cachedColor;
        }

    }

}