using LayoutFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.Composers;
using LayoutFarm;

namespace LayoutFarm.CustomWidgets
{

    class MyCssBoxGenerator : CustomCssBoxGenerator
    {
        public override LayoutFarm.HtmlBoxes.CssBox CreateCssBox(object tag,
            LayoutFarm.HtmlBoxes.CssBox parentBox,
            BoxSpec spec,
            LayoutFarm.RootGraphic rootgfx)
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
                            //create wrapper CssBox 

                            var wrapperBox = this.CreateWrapper(
                                 textbox,
                                 textbox.GetPrimaryRenderElement(rootgfx),
                                 spec);

                            //leanTextBox.AcceptKeyboardFocus = true;
                            parentBox.AppendChild(wrapperBox);

                            return wrapperBox;
                        }
                }
            }
            var simpleBox = new LayoutFarm.CustomWidgets.EaseBox(100, 20);
            simpleBox.BackColor = LayoutFarm.Drawing.Color.LightGray;

            var wrapperBox2 = this.CreateWrapper(
                               simpleBox,
                               simpleBox.GetPrimaryRenderElement(rootgfx),
                               spec);
            parentBox.AppendChild(wrapperBox2);
            return wrapperBox2;
            //return leanBox;
        }

    }

}
