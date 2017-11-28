//MIT, 2014-2017, WinterDev 

using PixelFarm.Drawing;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
namespace LayoutFarm.Composers
{
    public struct EaseScriptElement
    {
        HtmlElement elem;
        public EaseScriptElement(DomElement elem)
        {
            this.elem = elem as HtmlElement;
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

            HtmlElement.InvokeNotifyChangeOnIdleState(
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
                    new CssCodeColor(
                        CssColorConv.ConvertToCssColor(newcolor))));
            HtmlElement.InvokeNotifyChangeOnIdleState(elem, ElementChangeKind.Spec);
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
                elem.IsStyleEvaluated = false;
            }

            //-------------------------------------
            existingRuleSet.RemoveCssProperty(WellknownCssPropertyName.BackgroundColor);
            existingRuleSet.AddCssCodeProperty(
               new CssPropertyDeclaration(
                   WellknownCssPropertyName.BackgroundColor,
                   new CssCodeColor(CssColorConv.ConvertToCssColor(backgroundColor))));
            elem.SkipPrincipalBoxEvalulation = false;
            CssBox cssbox = HtmlElement.InternalGetPrincipalBox(elem);
            if (cssbox != null)
            {
#if DEBUG
                cssbox.dbugMark1++;
#endif

                CssBox.InvalidateComputeValue(cssbox);
            }

            HtmlElement.InvokeNotifyChangeOnIdleState(
               elem,
               ElementChangeKind.Spec);
            InvalidateCssBox(cssbox);
        }
        static void InvalidateCssBox(CssBox cssbox)
        {
            var rootGfx = cssbox.GetInternalRootGfx();
            var rootCssBox = cssbox.GetTopRootCssBox() as RenderElementBridgeCssBox;
            if (rootCssBox != null)
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