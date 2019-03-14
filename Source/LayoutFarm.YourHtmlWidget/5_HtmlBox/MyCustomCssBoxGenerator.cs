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

        public override CssBox CreateCssBox(
            DomElement domE,
            CssBox parentBox,
            BoxSpec spec,
            HtmlHost host)
        {
            switch (domE.Name)
            {
                case "select":
                    {
                        CssBox selectedBox = CreateSelectBox(domE, parentBox, spec, myHost.RootGfx, host);
                        if (selectedBox != null)
                        {
                            return selectedBox;
                        }
                    }
                    break;
                case "input":
                    {
                        CssBox inputBox = CreateInputBox(domE, parentBox, spec, myHost.RootGfx, host);
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
                            host,
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
                               host,
                               simpleBox,
                               simpleBox.GetPrimaryRenderElement(myHost.RootGfx),
                               spec, false);
            parentBox.AppendChild(wrapperBox2);
            return wrapperBox2;
        }


        CssBox CreateSelectBox(DomElement domE,
            CssBox parentBox,
            BoxSpec spec,
            LayoutFarm.RootGraphic rootgfx, HtmlHost host)
        {
            //https://www.w3schools.com/html/html_form_elements.asp

            //1. as drop-down list
            //2. as list-box


            WebDom.Impl.HtmlElement htmlElem = ((WebDom.Impl.HtmlElement)domE);
            htmlElem.HasSpecialPresentation = true;
            //
            LayoutFarm.HtmlWidgets.HingeBox hingeBox = new LayoutFarm.HtmlWidgets.HingeBox(100, 30); //actual controller
            foreach (DomNode childNode in domE.GetChildNodeIterForward())
            {

                WebDom.Impl.HtmlElement childElem = childNode as WebDom.Impl.HtmlElement;
                if (childElem != null)
                {
                    //find a value 
                    if (childElem.WellknownElementName == WellKnownDomNodeName.option)
                    {
                        DomAttribute domAttr = childElem.FindAttribute("value");
                        if (domAttr != null)
                        {
                            childElem.Tag = domAttr.Value;
                        }
                    }
                    hingeBox.AddItem(childElem);
                }
            }

            LayoutFarm.WebDom.Impl.HtmlElement hingeBoxDom = (LayoutFarm.WebDom.Impl.HtmlElement)hingeBox.GetPresentationDomNode((WebDom.Impl.HtmlDocument)domE.OwnerDocument);
            CssBox cssHingeBox = host.CreateBox(parentBox, hingeBoxDom, true); //create and append to the parentBox 
            //
            hingeBoxDom.SetSubParentNode(domE);
            cssHingeBox.IsReplacement = true;
            htmlElem.SpecialPresentationUpdate = (o) =>
            {
                if (hingeBox.NeedUpdateDom)
                {
                    cssHingeBox.Clear();
                    host.UpdateChildBoxes(hingeBoxDom, false);
                }
            };
#if DEBUG
            //cssHingeBox.dbugMark1 = 1;
#endif
            return cssHingeBox;
        }

        CssBox CreateInputBox(DomElement domE,
            CssBox parentBox,
            BoxSpec spec,
            LayoutFarm.RootGraphic rootgfx, HtmlHost host)
        {



            //https://www.w3schools.com/tags/tag_input.asp
            //button
            //checkbox
            //color
            //date
            //datetime - local
            //email
            //file
            //hidden
            //image
            //month
            //number
            //password
            //radio
            //range
            //reset
            //search
            //submit
            //tel
            //text
            //time
            //url
            //week



            var typeAttr = domE.FindAttribute("type");
            if (typeAttr != null)
            {
                switch (typeAttr.Value)
                {

                    case "password":
                        {

                            var textbox = new LayoutFarm.CustomWidgets.TextBoxContainer(100, 20, false, true);
                            CssBox wrapperBox = CreateWrapper(
                                 host,
                                 textbox,
                                 textbox.GetPrimaryRenderElement(rootgfx),
                                 spec, true);
                            //place holder support
                            DomAttribute placeHolderAttr = domE.FindAttribute("placeholder");
                            if (placeHolderAttr != null)
                            {
                                textbox.PlaceHolderText = placeHolderAttr.Value;
                            }
                            parentBox.AppendChild(wrapperBox);
                            return wrapperBox;
                        }
                    case "text":
                        {
                            // user can specific width of textbox 
                            //var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 17, false);
                            var textbox = new LayoutFarm.CustomWidgets.TextBoxContainer(100, 20, false);
                            CssBox wrapperBox = CreateWrapper(
                                 host,
                                 textbox,
                                 textbox.GetPrimaryRenderElement(rootgfx),
                                 spec, true);
                            //place holder support
                            DomAttribute placeHolderAttr = domE.FindAttribute("placeholder");
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

                            WebDom.Impl.HtmlElement buttonDom = (WebDom.Impl.HtmlElement)button.GetPresentationDomNode((HtmlDocument)domE.OwnerDocument);
                            buttonDom.SetAttribute("style", "width:20px;height:20px;background-color:white;cursor:pointer");
                            CssBox buttonCssBox = host.CreateBox(parentBox, buttonDom, true);
                            parentBox.AppendChild(buttonCssBox);
                            return buttonCssBox;
                        }
                    case "checkbox":
                        {
                            //implement with choice box + multiple value
                            var button = new HtmlWidgets.ChoiceBox(18, 10);
                            button.OnlyOne = false; //*** show as checked box 
                            var ihtmlElement = domE as LayoutFarm.WebDom.IHtmlElement;
                            WebDom.Impl.HtmlElement buttonDom = (WebDom.Impl.HtmlElement)button.GetPresentationDomNode((HtmlDocument)domE.OwnerDocument);
                            //buttonDom.SetAttribute("style", "width:20px;height:20px;background-color:red;cursor:pointer");
                            CssBox buttonCssBox = host.CreateBox(parentBox, buttonDom, true); //create and append to the parentBox
#if DEBUG
                            buttonCssBox.dbugMark1 = 1;
#endif
                            return buttonCssBox;
                        }

                    case "radio":
                        {
                            var button = new HtmlWidgets.ChoiceBox(10, 10);
                            button.OnlyOne = true;// show as option box

                            var ihtmlElement = domE as LayoutFarm.WebDom.IHtmlElement;

                            //if (ihtmlElement != null)
                            //{
                            //    button.Text = ihtmlElement.innerHTML;
                            //}
                            //else
                            //{
                            //    button.Text = "testButton";
                            //}
                            //button.Text = "C";

                            WebDom.Impl.HtmlElement buttonDom = (WebDom.Impl.HtmlElement)button.GetPresentationDomNode((HtmlDocument)domE.OwnerDocument);
                            //buttonDom.SetAttribute("style", "width:20px;height:20px;background-color:red;cursor:pointer");
                            CssBox buttonCssBox = host.CreateBox(parentBox, buttonDom, true); //create and append to the parentBox
#if DEBUG
                            buttonCssBox.dbugMark1 = 1;
#endif
                            return buttonCssBox;
                        }
                    case "your_box":
                        {

                            //tempfix -> just copy the Button code,
                            //TODO: review here, use proper radio button 
                            var box = new LayoutFarm.CustomWidgets.Box(20, 20);
                            CssBox wrapperBox = CreateWrapper(
                                 host,
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
