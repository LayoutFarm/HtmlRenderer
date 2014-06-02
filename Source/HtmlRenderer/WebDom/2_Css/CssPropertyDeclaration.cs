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
        CssCodeValueExpression propertyValue;
        List<CssCodeValueExpression> moreValues;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public CssPropertyDeclaration(string propertyName)
        {

#if DEBUG
           
#endif
            this.PropertyName = propertyName.ToLower();
        }
        internal CssPropertyDeclaration(string propertyName, CssCodeValueExpression value)
        {

#if DEBUG
            
#endif
            //from another 
            this.PropertyName = propertyName.ToLower();
            this.propertyValue = value;
            this.markedAsInherit = value.ToString() == "inherit";
            //auto gen from another prop
            this.isAutoGen = true;
        }
        internal bool IsExpand
        {
            get { return this.isExpand; }
            set { this.isExpand = value; }
        }
        internal void AddUnitToLatestValue(string unit)
        {
            CssCodePrimitiveExpression latestValue = null;
            if (moreValues != null)
            {
                latestValue = moreValues[moreValues.Count - 1] as CssCodePrimitiveExpression;

            }
            else
            {
                latestValue = this.propertyValue as CssCodePrimitiveExpression;

            }
            if (latestValue != null)
            {
                latestValue.Unit = unit;
            }
        }
        public void AddValue(CssCodeValueExpression value)
        {
            if (propertyValue == null)
            {
                markedAsInherit = value.ToString() == "inherit";
                this.propertyValue = value;
            }
            else
            {
                if (moreValues == null)
                {
                    moreValues = new List<CssCodeValueExpression>();
                }
                moreValues.Add(value);
                markedAsInherit = false;
            }
        }
        public void ReplaceValue(int index, CssCodeValueExpression value)
        {
            if (index == 0)
            {
                this.propertyValue = value;
            }
            else
            {
                moreValues[index - 1] = value;
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
                    CssCodeValueExpression propV = moreValues[i];
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


        ///// <summary>
        ///// Check if the given string is a valid length value.
        ///// </summary>
        ///// <param name="value">the string value to check</param>
        ///// <returns>true - valid, false - invalid</returns>
        //public static bool IsValidLength(string value)
        //{
        //    if (value.Length > 1)
        //    {
        //        string number = string.Empty;
        //        if (value.EndsWith("%"))
        //        {
        //            number = value.Substring(0, value.Length - 1);
        //        }
        //        else if (value.Length > 2)
        //        {
        //            number = value.Substring(0, value.Length - 2);
        //        }
        //        float stub;
        //        return float.TryParse(number, out stub);
        //    }
        //    return false;
        //}

        //void PrepareProperty()
        //{
        //    return;
        //    //if (isReady)
        //    //{
        //    //    return;
        //    //}
        //    //isReady = true;

        //    //switch (this.PropertyName)
        //    //{
        //    //    case "width":
        //    //    case "height":
        //    //    case "lineheight":
        //    //        {
        //    //            ParseLengthProperty(this);
        //    //        } break;
        //    //    case "color":
        //    //    case "backgroundcolor":
        //    //    case "bordertopcolor":
        //    //    case "borderbottomcolor":
        //    //    case "borderleftcolor":
        //    //    case "borderrightcolor":
        //    //        {
        //    //            //check if valid color
        //    //            //if valid then parse ***
        //    //            ///CssValueParser.IsColorValid(propValue) check if valid color
        //    //            //ParseColorProperty(propName, propValue, properties);
        //    //        } break;
        //    //    case "font":
        //    //        {
        //    //            // ParseFontProperty(this);

        //    //        } break;
        //    //    //case "border":
        //    //    //    {
        //    //    //        ParseBorderProperty(propValue, null, properties);
        //    //    //    } break;
        //    //    //case "border-left":
        //    //    //    {
        //    //    //        ParseBorderProperty(propValue, "-left", properties);
        //    //    //    } break;
        //    //    //case "border-right":
        //    //    //    {
        //    //    //        ParseBorderProperty(propValue, "-right", properties);
        //    //    //    } break;
        //    //    //case "border-top":
        //    //    //    {
        //    //    //        ParseBorderProperty(propValue, "-top", properties);
        //    //    //    } break;
        //    //    //case "border-bottom":
        //    //    //    {
        //    //    //        ParseBorderProperty(propValue, "-bottom", properties);
        //    //    //    } break;
        //    //    //case "margin":
        //    //    //    {
        //    //    //        ParseMarginProperty(propValue, properties);
        //    //    //    } break;
        //    //    //case "border-style":
        //    //    //    {
        //    //    //        ParseBorderStyleProperty(propValue, properties);
        //    //    //    } break;
        //    //    //case "border-width":
        //    //    //    {
        //    //    //        ParseBorderWidthProperty(propValue, properties);
        //    //    //    } break;
        //    //    //case "border-color":
        //    //    //    {
        //    //    //        ParseBorderColorProperty(propValue, properties);
        //    //    //    } break;
        //    //    //case "padding":
        //    //    //    {
        //    //    //        ParsePaddingProperty(propValue, properties);
        //    //    //    } break;
        //    //    //case "background-image":
        //    //    //    {
        //    //    //        properties["background-image"] = new CssProperty(
        //    //    //            "background-image", ParseBackgroundImageProperty(propValue));
        //    //    //    } break;
        //    //    //case "font-family":
        //    //    //    {
        //    //    //        properties["font-family"] = new CssProperty(
        //    //    //            "font-family", ParseFontFamilyProperty(propValue));
        //    //    //    } break;
        //    //    default:
        //    //        {
        //    //            //properties[propName] = new CssProperty(propName, propValue);
        //    //        } break;
        //    //}
        //}


        public CssCodeValueExpression GetPropertyValue(int index)
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

    public class CssCodePrimitiveExpression : CssCodeValueExpression
    {
        string unit;
        readonly string _propertyValue;
        readonly CssValueHint hint;
        public CssCodePrimitiveExpression(string value, CssValueHint hint)
        {
            this._propertyValue = value;
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
    }
    public class CssCodeFunctionCallExpression : CssCodeValueExpression
    {

        List<CssCodeValueExpression> funcArgs = new List<CssCodeValueExpression>();
        public CssCodeFunctionCallExpression(string funcName)
        {
            this.FunctionName = funcName;
        }
        public string FunctionName
        {
            get;
            private set;
        }
        public void AddFuncArg(CssCodeValueExpression arg)
        {
            this.funcArgs.Add(arg);
        }
        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(this.FunctionName);
            sb.Append('(');
            int j = funcArgs.Count;

            for (int i = 0; i < j; ++i)
            {
                sb.Append(funcArgs[i].ToString());
                if (i < j - 1)
                {
                    sb.Append(',');
                }
            }
            sb.Append(')');


            return sb.ToString();

        }
        protected override string GetTranslatedValue()
        {
            switch (this.FunctionName)
            {
                case "rgb":
                    {
                        //each is number
                        //?
                        int r_value = int.Parse(funcArgs[0].ToString());
                        int g_value = int.Parse(funcArgs[1].ToString());
                        int b_value = int.Parse(funcArgs[2].ToString());

                        return "#" + r_value.ToString("X") + g_value.ToString("X") + b_value.ToString("X");
                    }
                default:
                    {
                        return this.ToString();
                    }
            }

        }
    }


    public abstract class CssCodeValueExpression
    {
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif

        public CssCodeValueExpression()
        {

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
                return this.cachedColor = Parse.CssValueParser.GetActualColor(this.GetTranslatedValue());
            }
            return this.cachedColor;
        }
        protected virtual string GetTranslatedValue()
        {
            return this.ToString();
        }

    }




}