
using System;
using System.Drawing;
using System.Collections.Generic;
using HtmlRenderer.Css;
using HtmlRenderer.WebDom;
using HtmlRenderer.Boxes;
using HtmlRenderer.Composers.BridgeHtml;
namespace HtmlRenderer.Composers
{


    public struct EaseScriptableElement
    {
        BridgeHtmlElement elem;
        public EaseScriptableElement(HtmlElement elem)
        {
            this.elem = elem as BridgeHtmlElement;
        }
        public void ChangeFontColor(Color newcolor)
        {
            //change prop
            //then need to evaluate 
            if (elem == null)
            {
                return;
            }

            BoxSpec boxSpec = elem.Spec;
            if (boxSpec.ActualColor != newcolor)
            {

                BridgeHtmlElement.InvokeNotifyChangeOnIdleState(
                    elem,
                    ElementChangeKind.Spec);
                //-------------------------------------
                var existingRuleSet = elem.ElementRuleSet;
                if (existingRuleSet == null)
                {
                    //create new one                     
                    elem.ElementRuleSet = existingRuleSet = new CssRuleSet();
                    elem.IsStyleEvaluated = true;
                }

                existingRuleSet.AddCssCodeProperty(
                    new CssPropertyDeclaration(
                        WellknownCssPropertyName.Color,
                        new CssCodeColor(newcolor)));                 

                BridgeHtmlElement.InvokeNotifyChangeOnIdleState(elem, ElementChangeKind.Spec);

            }
        }
    }
}