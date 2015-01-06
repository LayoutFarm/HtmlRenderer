using LayoutFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.Composers;
using LayoutFarm;

namespace LayoutFarm.CustomWidgets
{

    class MyCssBoxGenerator : CustomCssBoxGenerator
    {
        public override LayoutFarm.Boxes.CssBox CreateCssBox(object tag, LayoutFarm.Boxes.CssBox parentBox, BoxSpec spec, LayoutFarm.RootGraphic rootgfx)
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
                            var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 20, false);
                            LayoutFarm.Boxes.RenderElementInsideCssBox leanTextBox = new LayoutFarm.Boxes.RenderElementInsideCssBox(textbox,
                                spec,
                                textbox.GetPrimaryRenderElement(rootgfx));
                            //leanTextBox.AcceptKeyboardFocus = true;
                            parentBox.AppendChild(leanTextBox);

                            return leanTextBox;
                        }
                }
            }
            var simpleBox = new LayoutFarm.CustomWidgets.EaseBox(100, 20);
            simpleBox.BackColor = LayoutFarm.Drawing.Color.LightGray;

            LayoutFarm.Boxes.RenderElementInsideCssBox leanBox = new LayoutFarm.Boxes.RenderElementInsideCssBox(simpleBox, spec, simpleBox.GetPrimaryRenderElement(rootgfx));
            parentBox.AppendChild(leanBox);
            return leanBox;
            //return leanBox;
        }

    }

}
