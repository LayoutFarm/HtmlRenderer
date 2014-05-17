//BSD  2014 ,WinterCore
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
 
namespace HtmlRenderer.WebDom
{

    /// <summary>
    /// css attr selector
    /// </summary>
    public class CssAttributeSelectorExpression : CssSelectorExpressionBase
    {

        public CssSelectorExpressionBase targetExpression;
        public CssAttributeNameExpression attributeExpression;
        public CssAttributeSelectorOperator operatorName;
        public CssInstructionValue valueExpression;

        public override string SelectorSignature
        {
            get
            {
                
                StringBuilder stBuilder = new StringBuilder(); 
                if (targetExpression != null)
                {
                    stBuilder.Append(targetExpression.SelectorSignature);
                }

                stBuilder.Append('[');
                stBuilder.Append(attributeExpression.ToString());
                
                switch (operatorName)
                {
                    case CssAttributeSelectorOperator.Equalily:
                        {
                            stBuilder.Append('=');
                        } break;
                    case CssAttributeSelectorOperator.Existance:
                        {
                            
                        } break;
                    case CssAttributeSelectorOperator.Hyphen:
                        {
                            stBuilder.Append("|=");
                        } break;
                    case CssAttributeSelectorOperator.Prefix:
                        {
                            stBuilder.Append("^=");
                        } break;
                    case CssAttributeSelectorOperator.WhiteSpace:
                        {
                            stBuilder.Append("~=");
                        } break;
                    case CssAttributeSelectorOperator.Substring:
                        {
                            stBuilder.Append("*=");
                        } break;
                    case CssAttributeSelectorOperator.Suffix:
                        {
                            stBuilder.Append("$=");
                        } break;
                   
                }
                if (valueExpression != null)
                {
                    stBuilder.Append(valueExpression.ToString());
                }
                stBuilder.Append(']');
                return stBuilder.ToString();

            }
        }
    }
    public class CssAttributeNameExpression : CssExpression
    {
        public string AttributeName;
        public override string ToString()
        {
            return AttributeName;
        }
    } 
    public enum CssAttributeSelectorOperator
    {
        Equalily,
        Existance,
        Hyphen,
        Prefix,
        Substring,
        Suffix,
        WhiteSpace
    }

}