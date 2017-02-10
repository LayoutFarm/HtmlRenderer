//BSD, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Css;
namespace LayoutFarm.WebDom
{
    public abstract class CssCodeValueExpression
    {
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif

        public CssCodeValueExpression(CssValueHint hint)
        {
#if DEBUG
            //if (this.dbugId == 111)
            //{

            //}
#endif
            this.Hint = hint;
        }


        CssValueEvaluatedAs evaluatedAs;
        CssColor cachedColor;
        LayoutFarm.Css.CssLength cachedLength;
        int cachedInt;
        protected float number;
        public bool IsInherit
        {
            get;
            internal set;
        }
        public CssValueHint Hint
        {
            get;
            private set;
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
        public void SetColorValue(CssColor color)
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

        public CssColor GetCacheColor()
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
    public class CssCodeColor : CssCodeValueExpression
    {
        CssColor color;
        public CssCodeColor(CssColor color)
            : base(CssValueHint.HexColor)
        {
            this.color = color;
            SetColorValue(color);
        }
        public CssColor ActualColor
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
                    }
                    break;
                case CssValueHint.Number:
                    {
                        this.number = float.Parse(value);
                    }
                    break;
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
        string evaluatedStringValue;
        bool isEval;
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
                            //css color function rgb
                            //each is number 
                            byte r_value = (byte)funcArgs[0].AsNumber();
                            byte g_value = (byte)funcArgs[1].AsNumber();
                            byte b_value = (byte)funcArgs[2].AsNumber();
                            return this.evaluatedStringValue = string.Concat("#",
                                ConvertByteToStringWithPadding(r_value),
                                ConvertByteToStringWithPadding(g_value),
                                ConvertByteToStringWithPadding(b_value));
                        }
                    //TODO: implement rgba here
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
        static string ConvertByteToStringWithPadding(byte colorByte)
        {
            string hex = colorByte.ToString("X");
            if (hex.Length < 2)
            {
                return "0" + hex;
            }
            else
            {
                return hex;
            }
        }
    }

    public class CssCodeBinaryExpression : CssCodeValueExpression
    {
        public CssCodeBinaryExpression()
            : base(CssValueHint.BinaryExpression)
        {
        }
        public CssValueOpName OpName
        {
            get;
            set;
        }
        public CssCodeValueExpression Left
        {
            get;
            set;
        }
        public CssCodeValueExpression Right
        {
            get;
            set;
        }
        public override string ToString()
        {
            StringBuilder stbuilder = new StringBuilder();
            if (Left != null)
            {
                stbuilder.Append(Left.ToString());
            }
            else
            {
                throw new NotSupportedException();
            }
            switch (this.OpName)
            {
                case CssValueOpName.Unknown:
                    {
                        throw new NotSupportedException();
                    }
                case CssValueOpName.Divide:
                    {
                        stbuilder.Append('/');
                    }
                    break;
            }
            if (Right != null)
            {
                stbuilder.Append(Right.ToString());
            }
            else
            {
                throw new NotSupportedException();
            }
            return stbuilder.ToString();
        }
        public override string GetTranslatedStringValue()
        {
            throw new NotImplementedException();
        }
    }
}