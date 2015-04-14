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
        public override LayoutFarm.HtmlBoxes.CssBox CreateCssBox(
            DomElement domE,
            LayoutFarm.HtmlBoxes.CssBox parentBox,
            BoxSpec spec,
            LayoutFarm.RootGraphic rootgfx)
        {
            //check if this is a proper tag  

            switch (domE.Name)
            {
                case "input":
                    {
                        var inputBox = CreateInputBox(domE, parentBox, spec, rootgfx);
                        if (inputBox != null)
                        {
                            return inputBox;
                        }
                    } break;
                case "canvas":
                    {
                        //test only
                        var canvas = new LayoutFarm.CustomWidgets.MiniAggCanvasBox(400, 400);
                        var wrapperBox = CreateWrapper(
                             canvas,
                             canvas.GetPrimaryRenderElement(rootgfx),
                             spec, true);
                        parentBox.AppendChild(wrapperBox);
                        return wrapperBox;
                    }

            }
            //------
            //else ...

            var simpleBox = new LayoutFarm.CustomWidgets.SimpleBox(100, 20);
            simpleBox.BackColor = PixelFarm.Drawing.Color.LightGray;
            var wrapperBox2 = CreateWrapper(
                               simpleBox,
                               simpleBox.GetPrimaryRenderElement(rootgfx),
                               spec, false);

            parentBox.AppendChild(wrapperBox2);
            return wrapperBox2;
            //return leanBox;
        }

        LayoutFarm.HtmlBoxes.CssBox CreateInputBox(DomElement domE,
            LayoutFarm.HtmlBoxes.CssBox parentBox,
            BoxSpec spec,
            LayoutFarm.RootGraphic rootgfx)
        {
            var typeAttr = domE.FindAttribute("type");

            if (typeAttr != null)
            {
                switch (typeAttr.Value)
                {
                    case "text":
                        {
                            // user can specific width of textbox 
                            var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 17, false);
                            var wrapperBox = CreateWrapper(
                                 textbox,
                                 textbox.GetPrimaryRenderElement(rootgfx),
                                 spec, true);
                            parentBox.AppendChild(wrapperBox);
                            return wrapperBox;
                        }
                    case "button":
                        {

                            //use subdom? technique
                            //todo: review the technique here
                            var button = new HtmlWidgets.Button(60, 30);
                            var ihtmlElement = domE as LayoutFarm.WebDom.IHtmlElement;
                            if (ihtmlElement != null)
                            {
                                button.Text = ihtmlElement.innerHTML;
                            }
                            else
                            {
                                button.Text = "";
                            }

                            var ui = button.GetPrimaryUIElement(this.myHost);
                            var wrapperBox = CreateWrapper(
                                button,
                                ui.GetPrimaryRenderElement(rootgfx),
                                spec, true);
                            parentBox.AppendChild(wrapperBox);
                            return wrapperBox;

                        }
                    case "textbox":
                        {
                            var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 17, false);
                            var wrapperBox = CreateWrapper(
                                 textbox,
                                 textbox.GetPrimaryRenderElement(rootgfx),
                                 spec, true);
                            parentBox.AppendChild(wrapperBox);
                            return wrapperBox;
                        }
                }
            }
            return null;
        }

    }

}
