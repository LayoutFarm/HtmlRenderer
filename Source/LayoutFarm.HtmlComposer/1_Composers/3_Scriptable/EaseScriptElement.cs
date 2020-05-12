//MIT, 2014-present, WinterDev 

using PixelFarm.Drawing;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
namespace LayoutFarm.Composers
{
    public struct EaseScriptElement
    {
        HtmlElement _elem;
        public EaseScriptElement(DomElement elem)
        {
            _elem = elem as HtmlElement;
        }
        //
        public bool IsScriptable => _elem != null;

        //
        public void ChangeFontColor(Color newcolor)
        {
            //change prop
            //then need to evaluate 
            if (_elem == null)
            {
                return;
            }
            BoxSpec boxSpec = _elem.Spec;
            if (boxSpec.ActualColor == newcolor)
            {
                return;
            }

            //TODO: review 
            HtmlElement.InvokeNotifyChangeOnIdleState(
                _elem,
                ElementChangeKind.Spec, null);
            //-------------------------------------
            var existingRuleSet = _elem.ElementRuleSet;
            if (existingRuleSet == null)
            {
                //create new one                     
                _elem.ElementRuleSet = existingRuleSet = new CssRuleSet();
                _elem.IsStyleEvaluated = true;
            }
            existingRuleSet.AddCssCodeProperty(
                new CssPropertyDeclaration(
                    WellknownCssPropertyName.Color,
                    new CssCodeColor(newcolor)));
            HtmlElement.InvokeNotifyChangeOnIdleState(_elem, ElementChangeKind.Spec, null);
        }
        public void ChangeBackgroundColor(Color backgroundColor)
        {
            if (_elem == null)
            {
                return;
            }
            BoxSpec boxSpec = _elem.Spec;
            if (boxSpec.BackgroundColor == backgroundColor)
            {
                return;
            }


            var existingRuleSet = _elem.ElementRuleSet;
            if (existingRuleSet == null)
            {
                //create new one                     
                _elem.ElementRuleSet = existingRuleSet = new CssRuleSet();
                _elem.IsStyleEvaluated = false;
            }

            //-------------------------------------
            existingRuleSet.RemoveCssProperty(WellknownCssPropertyName.BackgroundColor);
            existingRuleSet.AddCssCodeProperty(
               new CssPropertyDeclaration(
                   WellknownCssPropertyName.BackgroundColor,
                   new CssCodeColor(backgroundColor)));
            _elem.SkipPrincipalBoxEvalulation = false;
            CssBox cssbox = HtmlElement.InternalGetPrincipalBox(_elem);
            if (cssbox != null)
            {
                //#if DEBUG
                //                cssbox.dbugMark1++;
                //#endif

                CssBox.InvalidateComputeValue(cssbox);
            }

            HtmlElement.InvokeNotifyChangeOnIdleState(
               _elem,
               ElementChangeKind.Spec, null);
            InvalidateCssBox(cssbox);
        }
        static void InvalidateCssBox(CssBox cssbox)
        {
            if (cssbox.GetTopRootCssBox() is RenderElementBridgeCssBox rootCssBox)
            {
                //----------------------------------------
                //TODO: fix here
                //we not need to update entire HtmlBox
                //we should update only invalidate area
                //----------------------------------------
                rootCssBox.ContainerElement.InvalidateGraphics();
            }
        }
    }
}