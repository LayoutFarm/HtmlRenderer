//BSD  2014 ,WinterCore

using System.Collections.Generic;
using System.Text;
using System.IO;
 
namespace HtmlRenderer.WebDom
{
    /// <summary>
    /// css combinator expression
    /// </summary>
    public class CssCombinatorExpression : CssSelectorExpressionBase
    {
        public CssCombinatorOperator operatorName;
        public CssSelectorExpressionBase leftExpression;
        public CssSelectorExpressionBase rightExpression;
        public override string SelectorSignature
        {
            get
            {
                StringBuilder stBuilder = new StringBuilder();
                stBuilder.Append(leftExpression.SelectorSignature);
                    
                switch (operatorName)
                {
                    case CssCombinatorOperator.Descendant:
                        {
                            stBuilder.Append(' ');                            
                        } break;
                    case CssCombinatorOperator.AdjacentSibling:
                        {
                            stBuilder.Append('+');
                        } break;
                    case CssCombinatorOperator.Child:
                        {
                            stBuilder.Append('>');

                        } break;
                    case CssCombinatorOperator.GeneralSibling:
                        {
                            stBuilder.Append('~');
                        } break;
                    default:
                        {
                        } break;
                }

                stBuilder.Append(rightExpression.SelectorSignature);
                return stBuilder.ToString();
            }
        }
        public override string ToString()
        {
            return this.SelectorSignature; 
        }
    }


    
    public enum CssCombinatorOperator
    {
        /// <summary>
        /// Adjacent op Add
        /// </summary>
        AdjacentSibling,
        /// <summary>
        /// Child op GT
        /// </summary>
        Child,

        /// <summary>
        /// simple space
        /// </summary>
        Descendant,

        /// <summary>
        /// sibling operator tile
        /// </summary>
        GeneralSibling
    }
}