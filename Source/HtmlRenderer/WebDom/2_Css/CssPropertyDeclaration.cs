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
            //if (this.dbugId == 199)
            //{

            //}
#endif
            this.PropertyName = propertyName;
        }
        internal CssPropertyDeclaration(string propertyName, CssCodePropertyValue propvalue)
        {
#if DEBUG
            //if (this.dbugId == 199)
            //{

            //}
#endif
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
        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(this.PropertyName);
            sb.Append(':');

            CollectValues(sb);

            return sb.ToString();
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
        /// <summary>
        /// Parse length property to add only valid lengths.
        /// </summary>
        /// <param name="propName">the name of the css property to add</param>
        /// <param name="propValue">the value of the css property to add</param>
        /// <param name="properties">the properties collection to add to</param>
        static void ParseLengthProperty(CssPropertyDeclaration property)
        {

            //then parse to css length
            if (IsValidLength(property.propertyValue.Value) ||
                property.propertyValue.Value.Equals(HtmlRenderer.Entities.CssConstants.Auto, StringComparison.OrdinalIgnoreCase))
            {
                property.isValid = true;
            }

            //CssProperty property = new CssProperty(propName, propValue);
            //if (CssValueParser.IsValidLength(propValue) ||
            //    propValue.Equals(CssConstants.Auto, StringComparison.OrdinalIgnoreCase))
            //{
            //    property.IsValidValue = true;
            //}
            //properties.Add(propName, property);

        }



        /// <summary>
        /// Parse a complex font property value that contains multiple css properties into specific css properties.
        /// </summary>
        /// <param name="propValue">the value of the property to parse to specific values</param>
        /// <param name="properties">the properties collection to add the specific properties to</param>
        static void ParseFontProperty(CssPropertyDeclaration property)
        {
            //find each value meaning
            int j = property.ValueCount;
            for (int i = 0; i < j; ++i)
            {
                CssCodePropertyValue propValue = property.GetPropertyValue(i);
                switch (propValue.Hint)
                {
                    default:
                        {
                        } break;
                    case CssValueHint.Number:
                        {
                        } break;
                }

            }



            //int mustBePos;
            //string mustBe = RegexParserUtils.Search(RegexParserUtils.CssFontSizeAndLineHeight, propValue, out mustBePos);

            //if (!string.IsNullOrEmpty(mustBe))
            //{
            //    mustBe = mustBe.Trim();
            //    //Check for style||variant||weight on the left
            //    string leftSide = propValue.Substring(0, mustBePos);
            //    string fontStyle = RegexParserUtils.Search(RegexParserUtils.CssFontStyle, leftSide);
            //    string fontVariant = RegexParserUtils.Search(RegexParserUtils.CssFontVariant, leftSide);
            //    string fontWeight = RegexParserUtils.Search(RegexParserUtils.CssFontWeight, leftSide);

            //    //Check for family on the right
            //    string rightSide = propValue.Substring(mustBePos + mustBe.Length);
            //    string fontFamily = rightSide.Trim(); //Parser.Search(Parser.CssFontFamily, rightSide); //TODO: Would this be right?

            //    //Check for font-size and line-height
            //    string fontSize = mustBe;
            //    string lineHeight = string.Empty;

            //    if (mustBe.Contains("/") && mustBe.Length > mustBe.IndexOf("/", StringComparison.Ordinal) + 1)
            //    {
            //        int slashPos = mustBe.IndexOf("/", StringComparison.Ordinal);
            //        fontSize = mustBe.Substring(0, slashPos);
            //        lineHeight = mustBe.Substring(slashPos + 1);
            //    }

            //    SetPropIfValueNotNull(properties, "font-family", ParseFontFamilyProperty(fontFamily));
            //    SetPropIfValueNotNull(properties, "font-style", fontStyle);
            //    SetPropIfValueNotNull(properties, "font-variant", fontVariant);
            //    SetPropIfValueNotNull(properties, "font-weight", fontWeight);
            //    SetPropIfValueNotNull(properties, "font-size", fontSize);
            //    SetPropIfValueNotNull(properties, "line-height", lineHeight);
            //}
            //else
            //{
            //    // Check for: caption | icon | menu | message-box | small-caption | status-bar
            //    //TODO: Interpret font values of: caption | icon | menu | message-box | small-caption | status-bar
            //}
        }


        public void PrepareProperty()
        {
            if (isReady)
            {
                return;
            }
            isReady = true;

            switch (this.PropertyName)
            {
                case "width":
                case "height":
                case "lineheight":
                    {
                        ParseLengthProperty(this);
                    } break;
                case "color":
                case "backgroundcolor":
                case "bordertopcolor":
                case "borderbottomcolor":
                case "borderleftcolor":
                case "borderrightcolor":
                    {
                        //check if valid color
                        //if valid then parse ***
                        ///CssValueParser.IsColorValid(propValue) check if valid color
                        //ParseColorProperty(propName, propValue, properties);
                    } break;
                case "font":
                    {
                        // ParseFontProperty(this);

                    } break;
                //case "border":
                //    {
                //        ParseBorderProperty(propValue, null, properties);
                //    } break;
                //case "border-left":
                //    {
                //        ParseBorderProperty(propValue, "-left", properties);
                //    } break;
                //case "border-right":
                //    {
                //        ParseBorderProperty(propValue, "-right", properties);
                //    } break;
                //case "border-top":
                //    {
                //        ParseBorderProperty(propValue, "-top", properties);
                //    } break;
                //case "border-bottom":
                //    {
                //        ParseBorderProperty(propValue, "-bottom", properties);
                //    } break;
                //case "margin":
                //    {
                //        ParseMarginProperty(propValue, properties);
                //    } break;
                //case "border-style":
                //    {
                //        ParseBorderStyleProperty(propValue, properties);
                //    } break;
                //case "border-width":
                //    {
                //        ParseBorderWidthProperty(propValue, properties);
                //    } break;
                //case "border-color":
                //    {
                //        ParseBorderColorProperty(propValue, properties);
                //    } break;
                //case "padding":
                //    {
                //        ParsePaddingProperty(propValue, properties);
                //    } break;
                //case "background-image":
                //    {
                //        properties["background-image"] = new CssProperty(
                //            "background-image", ParseBackgroundImageProperty(propValue));
                //    } break;
                //case "font-family":
                //    {
                //        properties["font-family"] = new CssProperty(
                //            "font-family", ParseFontFamilyProperty(propValue));
                //    } break;
                default:
                    {
                        //properties[propName] = new CssProperty(propName, propValue);
                    } break;
            }
        }


        public CssCodePropertyValue GetPropertyValue(int index)
        {
            if (!isReady)
            {
                PrepareProperty();
            }
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

    public enum CssValueHint
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
        public string unit;
        string _propertyValue;
        CssValueHint hint;

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
            get
            {
                return this.hint;
            }
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
    }

}