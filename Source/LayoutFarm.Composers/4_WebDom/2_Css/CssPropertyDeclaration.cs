//BSD  2014 ,WinterDev 
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Css;
using PixelFarm.Drawing;
namespace LayoutFarm.WebDom
{


    public class CssPropertyDeclaration
    {
        bool isReady = false;
        bool isValid = false;
        bool isAutoGen = false;


        bool markedAsInherit;
        bool isExpand = false;

        CssCodeValueExpression propertyValue;
        List<CssCodeValueExpression> moreValues;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public CssPropertyDeclaration(WellknownCssPropertyName wellNamePropertyName)
        {
            //convert from name to wellknown property name; 
            this.WellknownPropertyName = wellNamePropertyName;
        }
        public CssPropertyDeclaration(WellknownCssPropertyName wellNamePropertyName, CssCodeValueExpression value)
        {
            //from another 
            this.WellknownPropertyName = wellNamePropertyName;
#if DEBUG

#endif
            this.propertyValue = value;
            this.markedAsInherit = value.IsInherit;
            //auto gen from another prop
            this.isAutoGen = true;
        }
        public bool IsExpand
        {
            get { return this.isExpand; }
            set { this.isExpand = value; }
        }
        public void AddUnitToLatestValue(string unit)
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

                this.markedAsInherit = value.IsInherit;
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

        public WellknownCssPropertyName WellknownPropertyName
        {
            get;
            private set;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.WellknownPropertyName.ToString());
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


        public CssCodeValueExpression GetPropertyValue(int index)
        {

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
        HexColor,
        LiteralString,
        Iden,
        Func
    }
    public class CssCodeColor : CssCodeValueExpression
    {
        Color color;
        public CssCodeColor(Color color)
            : base(CssValueHint.HexColor)
        {
            this.color = color;
            SetColorValue(color);
        }
        public Color ActualColor
        {
            get { return this.color; }
        }
    }
    public class CssCodePrimitiveExpression : CssCodeValueExpression
    {
        string unit;
        readonly string _propertyValue;

        public CssCodePrimitiveExpression(string value, CssValueHint hint)
            : base(hint)
        {
            this._propertyValue = value;
            switch (hint)
            {
                case CssValueHint.Iden:
                    {
                        //check value  
                        this.IsInherit = value == "inherit";
                    } break;
                case CssValueHint.Number:
                    {
                        this.number = float.Parse(value);
                    } break;
            }
        }
        public CssCodePrimitiveExpression(float number)
            : base(CssValueHint.Number)
        {
            //number             
            this.number = number;
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
            switch (this.Hint)
            {
                case CssValueHint.Number:
                    {
                        if (unit != null)
                        {
                            return number.ToString() + unit;
                        }
                        else
                        {
                            return number.ToString();
                        }
                    }
                default:
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



    public class CssCodeFunctionCallExpression : CssCodeValueExpression
    {

        List<CssCodeValueExpression> funcArgs = new List<CssCodeValueExpression>();
        public CssCodeFunctionCallExpression(string funcName)
            : base(CssValueHint.Func)
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

        string evaluatedStringValue;
        bool isEval;
        public override string GetTranslatedStringValue()
        {
            if (isEval)
            {
                return this.evaluatedStringValue;
            }
            else
            {
                isEval = true;
                switch (this.FunctionName)
                {
                    case "rgb":
                        {
                            //each is number 
                            int r_value = (int)funcArgs[0].AsNumber();
                            int g_value = (int)funcArgs[1].AsNumber();
                            int b_value = (int)funcArgs[2].AsNumber();

                            return this.evaluatedStringValue = "#" + r_value.ToString("X") + g_value.ToString("X") + b_value.ToString("X");
                        }
                    case "url":
                        {
                            return this.evaluatedStringValue = this.funcArgs[0].ToString();
                        }
                    default:
                        {
                            return this.evaluatedStringValue = this.ToString();
                        }
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

        public CssCodeValueExpression(CssValueHint hint)
        {
            this.Hint = hint;
        }
        public CssValueHint Hint
        {
            get;
            private set;
        }

        CssValueEvaluatedAs evaluatedAs;
        Color cachedColor;
        LayoutFarm.Css.CssLength cachedLength;
        int cachedInt;
        protected float number;

        public bool IsInherit
        {
            get;
            internal set;
        }

        //------------------------------------------------------
        public float AsNumber()
        {
            return this.number;
        }

        public void SetIntValue(int intValue, CssValueEvaluatedAs evaluatedAs)
        {
            this.evaluatedAs = evaluatedAs;
            this.cachedInt = intValue;
        }
        public void SetColorValue(Color color)
        {
            this.evaluatedAs = CssValueEvaluatedAs.Color;
            this.cachedColor = color;
        }
        public void SetCssLength(CssLength len, WebDom.CssValueEvaluatedAs evalAs)
        {
            this.cachedLength = len;
            this.evaluatedAs = evalAs;
        }

        public CssValueEvaluatedAs EvaluatedAs
        {
            get
            {
                return this.evaluatedAs;
            }
        }

        public Color GetCacheColor()
        {
            return this.cachedColor;
        }
        public CssLength GetCacheCssLength()
        {
            return this.cachedLength;
        }
        public virtual string GetTranslatedStringValue()
        {
            return this.ToString();
        }
        public int GetCacheIntValue()
        {
            return this.cachedInt;
        }
    }

    public enum CssValueEvaluatedAs : byte
    {
        UnEvaluate,
        Unknown,
        BorderLength,
        Length,
        TranslatedLength,
        Color,
        TranslatedString,

        BorderStyle,
        BorderCollapse,
        WhiteSpace,
        Visibility,
        VerticalAlign,
        TextAlign,
        Overflow,
        TextDecoration,
        WordBreak,
        Position,
        Direction,
        Display,
        Float,
        EmptyCell,
        FontWeight,
        FontStyle,
        FontVariant,

        ListStylePosition,
        ListStyleType,
        BackgroundRepeat
    }


}