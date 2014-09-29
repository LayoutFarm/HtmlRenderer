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

            CssLeanBox leanBox = new CssLeanBox(tag, spec, null);
            parentBox.AppendChild(leanBox);
            return leanBox;
            //return leanBox;
        }

    }

}
