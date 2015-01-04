using LayoutFarm.Drawing;
using HtmlRenderer.WebDom;
using HtmlRenderer.Css;
using HtmlRenderer.Composers;
using LayoutFarm;

namespace HtmlRenderer.Boxes
{

    class LeanBoxCreator : CustomCssBoxGenerator
    { 
        public override CssBox CreateCssBox(object tag, CssBox parentBox, BoxSpec spec, LayoutFarm.RootGraphic rootgfx)
        {
            //check if this is a proper tag 
            DomElement domE = tag as DomElement;
            if (domE == null) return null;
            //------
            var typeAttr = domE.FindAttribute("type"); 

            if (typeAttr != null)
            {
                switch (typeAttr.Value)
                {
                    case "textbox":
                        {
                            var textbox = new LayoutFarm.SampleControls.UITextBox(100, 20, false);
                            LayoutFarm.Boxes.RenderElementInsideCssBox leanTextBox = new LayoutFarm.Boxes.RenderElementInsideCssBox(textbox,
                                spec, 
                                textbox.GetPrimaryRenderElement(rootgfx));
                            //leanTextBox.AcceptKeyboardFocus = true;
                            parentBox.AppendChild(leanTextBox);
                            
                            return leanTextBox;
                        }
                }
            }
            var simpleBox = new LayoutFarm.SampleControls.UIEaseBox(100, 20);
            simpleBox.BackColor = LayoutFarm.Drawing.Color.LightGray;

            LayoutFarm.Boxes.RenderElementInsideCssBox leanBox = new LayoutFarm.Boxes.RenderElementInsideCssBox(simpleBox, spec, simpleBox.GetPrimaryRenderElement(rootgfx));
            parentBox.AppendChild(leanBox);
            return leanBox;
            //return leanBox;
        }

    }

}
