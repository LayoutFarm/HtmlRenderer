using LayoutFarm.Drawing;
using HtmlRenderer.WebDom;
using HtmlRenderer.Css;
using HtmlRenderer.Composers;

namespace HtmlRenderer.Boxes.LeanBox
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
                            CssLeanBox leanTextBox = new CssLeanBox(tag, spec, textbox.GetPrimaryRenderElement(rootgfx));
                            parentBox.AppendChild(leanTextBox);
                            return leanTextBox;
                        }
                }
            }
            var simpleBox = new LayoutFarm.SampleControls.UIButton(100, 20);
            simpleBox.BackColor = LayoutFarm.Drawing.Color.LightGray;
            CssLeanBox leanBox = new CssLeanBox(tag, spec, simpleBox.GetPrimaryRenderElement(rootgfx));
            parentBox.AppendChild(leanBox);
            return leanBox;
            //return leanBox;
        }

    }

}
