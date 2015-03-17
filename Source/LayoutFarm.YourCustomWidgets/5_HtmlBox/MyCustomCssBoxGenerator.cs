using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.Composers;
using LayoutFarm;

namespace LayoutFarm.CustomWidgets
{

    public class MyCustomCssBoxGenerator : CustomCssBoxGenerator
    {
        HtmlBoxes.HtmlHost myHost;
        public MyCustomCssBoxGenerator(HtmlBoxes.HtmlHost myHost)
        {
            this.myHost = myHost;
        }
        protected override HtmlBoxes.HtmlHost MyHost
        {
            get { return this.myHost; }
        }
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
                    case "text":
                        {
                            //user can specific width of textbox
                            var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 17, false);
                            //create wrapper CssBox 
                            var wrapperBox = this.CreateWrapper(
                                 textbox,
                                 textbox.GetPrimaryRenderElement(rootgfx),
                                 spec);

                            //leanTextBox.AcceptKeyboardFocus = true;
                            parentBox.AppendChild(wrapperBox);
                            return wrapperBox;
                        }
                    case "button":
                        {

                            //use subdom? technique
                            //todo: review the technique here
                            var button = new HtmlWidgets.Button(60, 30);
                            //use content of button
                            var ihtmlElement = tag as LayoutFarm.WebDom.IHtmlElement;
                            if (ihtmlElement != null)
                            {
                                button.Text = ihtmlElement.innerHTML;
                            }
                            else
                            {
                                button.Text = "";
                            }

                            var ui = button.GetPrimaryUIElement(this.myHost);
                            var wrapperBox = this.CreateWrapper(
                                button,
                                ui.GetPrimaryRenderElement(rootgfx),
                                spec);

                            //leanTextBox.AcceptKeyboardFocus = true;
                            parentBox.AppendChild(wrapperBox);
                            return wrapperBox;

                        }
                    case "textbox":
                        {
                            //user can specific width of textbox
                            var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 17, false);
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
            simpleBox.BackColor = PixelFarm.Drawing.Color.LightGray;

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
