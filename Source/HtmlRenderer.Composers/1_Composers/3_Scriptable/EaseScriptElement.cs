
using System;
using System.Drawing;
using System.Collections.Generic;
using HtmlRenderer.Css;
using HtmlRenderer.WebDom;
using HtmlRenderer.Boxes;
using HtmlRenderer.Composers.BridgeHtml;
namespace HtmlRenderer.Composers
{


    public struct EaseScriptElement
    {
        BridgeHtmlElement elem;
        public EaseScriptElement(HtmlElement elem)
        {
            this.elem = elem as BridgeHtmlElement;
        }
        public bool IsScriptable
        {
            get { return this.elem != null; }
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
            if (boxSpec.ActualColor == newcolor)
            {
                return;
            }

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
        public void ChangeBackgroundColor(Color backgroundColor)
        {
            if (elem == null)
            {
                return;
            }
            BoxSpec boxSpec = elem.Spec;
            if (boxSpec.BackgroundColor == backgroundColor)
            {
                return;
            }
            var existingRuleSet = elem.ElementRuleSet;
            if (existingRuleSet == null)
            {
                //create new one                     
                elem.ElementRuleSet = existingRuleSet = new CssRuleSet();
                elem.IsStyleEvaluated = true;
            }
            existingRuleSet.AddCssCodeProperty(
               new CssPropertyDeclaration(
                   WellknownCssPropertyName.BackgroundColor,
                   new CssCodeColor(backgroundColor)));
            BridgeHtmlElement.InvokeNotifyChangeOnIdleState(elem, ElementChangeKind.Spec);
            elem.SkipPrincipalBoxEvalulation = false;

            var cssbox = BridgeHtmlElement.InternalGetPrincipalBox(elem);
            if (cssbox != null)
            {
                CssBox.InvalidateComputeValue(cssbox);
            }
        }


    }
}