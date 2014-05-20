//BSD  2014 ,WinterCore


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO; 

namespace HtmlRenderer.WebDom
{
    public abstract class CssExpression
    { 
    } 
    public abstract class CssSelectorExpressionBase : CssExpression
    {
        public abstract string SelectorSignature { get; }
    }


    public enum PrimitiveSelectorType
    {
        TagName,
        ClassName,

        Id,
        Universal
    }


    public class CssPseudoClassExpression : CssSelectorExpressionBase
    {
        public string ClassName;
        public CssSelectorExpressionBase targetSelector;
        public override string SelectorSignature
        {
            get
            {
                if (targetSelector != null)
                {
                    return targetSelector.ToString() + ":" + ClassName;
                }
                else
                {
                    return ":" + ClassName;
                }
            }
        }

        public override string ToString()
        {
            return this.SelectorSignature;
        }
    }
    /// <summary>
    /// primitive selector expression
    /// </summary>
    public class CssPrimitiveSelectorExpression : CssSelectorExpressionBase
    {
        public string TagName;
        public PrimitiveSelectorType selectorType;

        public override string SelectorSignature
        {
            get
            {
                switch (selectorType)
                {
                    case PrimitiveSelectorType.ClassName:
                        {
                            return "." + TagName;
                        }
                    case PrimitiveSelectorType.Id:
                        {
                            return "#" + TagName;
                        }

                    case PrimitiveSelectorType.Universal:
                        {
                            return "*";
                        }
                    default:
                        {
                            return TagName;
                        }
                }

            }
        }

        public override string ToString()
        {
            return SelectorSignature;
        }

    }




   
}