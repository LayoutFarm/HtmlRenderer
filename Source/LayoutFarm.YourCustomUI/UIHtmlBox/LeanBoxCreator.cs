using LayoutFarm.Drawing;
using HtmlRenderer.WebDom;
using HtmlRenderer.Css;
using HtmlRenderer.Composers;

namespace HtmlRenderer.Boxes.LeanBox
{

    class LeanBoxCreator : CustomCssBoxGenerator
    {
        public override CssBox CreateCssBox(object tag, CssBox parentBox, BoxSpec spec)
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

                            CssLeanBox leanTextBox = new CssLeanBox(tag, spec, null);
                            parentBox.AppendChild(leanTextBox);

                            return leanTextBox;
                        }
                }
            }
            CssLeanBox leanBox = new CssLeanBox(tag, spec, null);
            parentBox.AppendChild(leanBox);
            return leanBox;
            //return leanBox;
        }

    }

}
