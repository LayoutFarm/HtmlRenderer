//BSD, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
namespace LayoutFarm.WebDom
{
    public class CssPropertyDeclaration
    {
        bool isAutoGen;
        bool markedAsInherit;
        CssCodeValueExpression firstValue;
        List<CssCodeValueExpression> moreValues;
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public CssPropertyDeclaration(string unknownName)
        {
            //convert from name to wellknown property name; 
            this.UnknownRawName = unknownName;
        }
        public CssPropertyDeclaration(WellknownCssPropertyName wellNamePropertyName)
        {
            //convert from name to wellknown property name; 
            this.WellknownPropertyName = wellNamePropertyName;
        }
        public CssPropertyDeclaration(WellknownCssPropertyName wellNamePropertyName, CssCodeValueExpression value)
        {
            //if (this.dbugId == 221)
            //{
            //}

            //from another 
            this.WellknownPropertyName = wellNamePropertyName;
            this.firstValue = value;
            this.markedAsInherit = value.IsInherit;
            //auto gen from another prop
            this.isAutoGen = true;
        }
        public bool IsExpand { get; set; }
        public string UnknownRawName { get; private set; }

        public void AddValue(CssCodeValueExpression value)
        {
            if (firstValue == null)
            {
                this.markedAsInherit = value.IsInherit;
                this.firstValue = value;
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
                this.firstValue = value;
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
            if (firstValue != null)
            {
                stBuilder.Append(firstValue.ToString());
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
                    if (firstValue == null)
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
                        return this.firstValue;
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
        Func,
        BinaryExpression,
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
        BackgroundRepeat,
        BoxSizing,
    }
    public enum CssValueOpName
    {
        Unknown,
        Divide,
    }
}