//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using LayoutFarm.Composers;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
namespace LayoutFarm.CustomWidgets
{
    public class MyCustomCssBoxGenerator : CustomCssBoxGenerator
    {
        HtmlBoxes.HtmlHost myHost;
        public MyCustomCssBoxGenerator(HtmlBoxes.HtmlHost myHost)
        {
            this.myHost = myHost;
        }

        public override LayoutFarm.HtmlBoxes.CssBox CreateCssBox(
            DomElement domE,
            LayoutFarm.HtmlBoxes.CssBox parentBox,
            BoxSpec spec,
            HtmlHost host)
        {
            switch (domE.Name)
            {
                case "input":
                    {
                        var inputBox = CreateInputBox(domE, parentBox, spec, myHost.RootGfx, host);
                        if (inputBox != null)
                        {
                            return inputBox;
                        }
                    }
                    break;
                case "canvas":
                    {
                        //test only
                        //TODO: review here
                        var canvas = new LayoutFarm.CustomWidgets.MiniAggCanvasBox(400, 400);
                        var wrapperBox = CreateWrapper(
                             canvas,
                             canvas.GetPrimaryRenderElement(myHost.RootGfx),
                             spec, true);
                        parentBox.AppendChild(wrapperBox);
                        return wrapperBox;
                    }
            }

            //default unknown
            var simpleBox = new LayoutFarm.CustomWidgets.Box(100, 20);
            simpleBox.BackColor = PixelFarm.Drawing.Color.LightGray;
            var wrapperBox2 = CreateWrapper(
                               simpleBox,
                               simpleBox.GetPrimaryRenderElement(myHost.RootGfx),
                               spec, false);
            parentBox.AppendChild(wrapperBox2);
            return wrapperBox2;
        }

        LayoutFarm.HtmlBoxes.CssBox CreateInputBox(DomElement domE,
            LayoutFarm.HtmlBoxes.CssBox parentBox,
            BoxSpec spec,
            LayoutFarm.RootGraphic rootgfx, HtmlHost host)
        {
            var typeAttr = domE.FindAttribute("type");
            if (typeAttr != null)
            {
                switch (typeAttr.Value)
                {
                    case "textbox":
                        {
                            var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 17, false);
                            CssBox wrapperBox = CreateWrapper(
                                 textbox,
                                 textbox.GetPrimaryRenderElement(rootgfx),
                                 spec, true);
                            parentBox.AppendChild(wrapperBox);
                            return wrapperBox;
                        }
                    case "text":
                        {
                            // user can specific width of textbox 
                            //var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 17, false);
                            var textbox = new LayoutFarm.CustomWidgets.TextBoxContainer(100, 20, false);
                            var wrapperBox = CreateWrapper(
                                 textbox,
                                 textbox.GetPrimaryRenderElement(rootgfx),
                                 spec, true);
                            //place holder support
                            var placeHolderAttr = domE.FindAttribute("placeholder");
                            if (placeHolderAttr != null)
                            {
                                textbox.PlaceHolderText = placeHolderAttr.Value;
                            }
                            parentBox.AppendChild(wrapperBox);
                            return wrapperBox;
                        }
                    case "button":
                        {
                            //use subdom technique ***
                            //todo: review the technique here
                            var button = new HtmlWidgets.Button(60, 30);
                            var ihtmlElement = domE as LayoutFarm.WebDom.IHtmlElement;
                            if (ihtmlElement != null)
                            {
                                button.Text = ihtmlElement.innerHTML;
                            }
                            else
                            {
                                button.Text = "testButton";
                            }

                            DomElement buttonDom = button.GetPresentationDomNode((HtmlDocument)domE.OwnerDocument);
                            buttonDom.SetAttribute("style", "width:20px;height:20px;background-color:white;");
                            CssBox buttonCssBox = host.CreateBox2(parentBox, (WebDom.Impl.HtmlElement)buttonDom, true);
                            parentBox.AppendChild(buttonCssBox);
                            return buttonCssBox;
                            // CreateCssBox(buttonDom, parentBox, spec, host);
                            //var ui = button.GetPrimaryUIElement(this.myHost);

                            //var wrapperBox = CreateWrapper(
                            //    button,
                            //    ui.GetPrimaryRenderElement(rootgfx),
                            //    spec, true);
                            //parentBox.AppendChild(wrapperBox);
                            //return wrapperBox;

                            //parentBox.AppendChild(buttonCssBox);
                            //return buttonCssBox;
                        }
                    case "radio":
                        {
                            var button = new HtmlWidgets.Button(60, 30);
                            var ihtmlElement = domE as LayoutFarm.WebDom.IHtmlElement;
                            if (ihtmlElement != null)
                            {
                                button.Text = ihtmlElement.innerHTML;
                            }
                            else
                            {
                                button.Text = "testButton";
                            }
                            button.Text = "C";
                            DomElement buttonDom = button.GetPresentationDomNode((HtmlDocument)domE.OwnerDocument);
                            buttonDom.SetAttribute("style", "width:20px;height:20px;background-color:red;");


                            CssBox buttonCssBox = host.CreateBox2(parentBox, (WebDom.Impl.HtmlElement)buttonDom, true); //create and append to the parentBox
#if DEBUG
                            buttonCssBox.dbugMark1 = 1;
#endif
                            return buttonCssBox;

                            ////tempfix -> just copy the Button code,
                            ////TODO: review here, use proper radio button 
                            //var box = new LayoutFarm.CustomWidgets.Box(20, 20);
                            //CssBox wrapperBox = CreateWrapper(
                            //     box,
                            //     box.GetPrimaryRenderElement(rootgfx),
                            //     spec, true);
                            //parentBox.AppendChild(wrapperBox);
                            //return wrapperBox;


                            ////use subdom technique
                            ////todo: review the technique here
                            //var checkbox = new HtmlWidgets.CheckBox(20, 20);
                            //var ihtmlElement = domE as LayoutFarm.WebDom.IHtmlElement;
                            //if (ihtmlElement != null)
                            //{
                            //    checkbox.Text = ihtmlElement.innerHTML;
                            //}
                            //else
                            //{
                            //    checkbox.Text = "";
                            //}
                            //DomElement buttonDom = checkbox.GetPresentationDomNode((HtmlDocument)domE.OwnerDocument);
                            //CssBox buttonCssBox = host.CreateBox2(parentBox, (WebDom.Impl.HtmlElement)buttonDom, true);
                            //parentBox.AppendChild(buttonCssBox);
                            //return buttonCssBox;
                        }
                        break;
                    case "your_box":
                        {

                            //tempfix -> just copy the Button code,
                            //TODO: review here, use proper radio button 
                            var box = new LayoutFarm.CustomWidgets.Box(20, 20);
                            CssBox wrapperBox = CreateWrapper(
                                 box,
                                 box.GetPrimaryRenderElement(rootgfx),
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
