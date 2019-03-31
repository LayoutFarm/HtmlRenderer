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
        HtmlHost _myHost;
        public MyCustomCssBoxGenerator(HtmlBoxes.HtmlHost myHost)
        {
            _myHost = myHost;
        }

        public override CssBox CreateCssBox(
            HtmlElement domE,
            CssBox parentBox,
            BoxSpec spec,
            HtmlHost host)
        {
            switch (domE.Name)
            {
                case "select":
                    {
                        CssBox selectedBox = CreateSelectBox(domE, parentBox, spec, _myHost.RootGfx, host);
                        if (selectedBox != null)
                        {
                            return selectedBox;
                        }
                    }
                    break;
                case "input":
                    {
                        CssBox inputBox = CreateInputBox(domE, parentBox, spec, _myHost.RootGfx, host);
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
                        //software canvas ?
                        var canvas = new LayoutFarm.CustomWidgets.MiniAggCanvasBox(400, 400);
                        CssBox wrapperBox = CreateCssWrapper(
                             host,
                             canvas,
                             canvas.GetPrimaryRenderElement(_myHost.RootGfx),
                             spec,
                             null,
                             true);
                        parentBox.AppendChild(wrapperBox);
                        return wrapperBox;
                    }
            }

            //default unknown
            var simpleBox = new LayoutFarm.CustomWidgets.Box(100, 20);
            simpleBox.BackColor = PixelFarm.Drawing.Color.LightGray;
            CssBox wrapperBox2 = CreateCssWrapper(
                               host,
                               simpleBox,
                               simpleBox.GetPrimaryRenderElement(_myHost.RootGfx),
                               spec,
                               null,
                               false);
            parentBox.AppendChild(wrapperBox2);
            return wrapperBox2;
        }


        CssBox CreateSelectBox(HtmlElement htmlElem,
            CssBox parentBox,
            BoxSpec spec,
            LayoutFarm.RootGraphic rootgfx, HtmlHost host)
        {
            //https://www.w3schools.com/html/html_form_elements.asp

            //1. as drop-down list
            //2. as list-box 


            htmlElem.HasSpecialPresentation = true;
            //
            LayoutFarm.HtmlWidgets.HingeBox hingeBox = new LayoutFarm.HtmlWidgets.HingeBox(100, 30); //actual controller
            foreach (DomNode childNode in htmlElem.GetChildNodeIterForward())
            {

                WebDom.Impl.HtmlElement childElem = childNode as WebDom.Impl.HtmlElement;
                if (childElem != null)
                {
                    //find a value 
                    //if (childElem.WellknownElementName == WellKnownDomNodeName.option)
                    //{
                    //    WebDom.IHtmlOptionElement option = childElem as WebDom.IHtmlOptionElement;
                    //    if (option != null)
                    //    {

                    //    }
                    //    //DomAttribute domAttr = childElem.FindAttribute("value");
                    //    //if (domAttr != null)
                    //    //{
                    //    //    childElem.Tag = domAttr.Value;
                    //    //}
                    //}
                    hingeBox.AddItem(childElem);
                }
            }

            HtmlElement hingeBoxDom = hingeBox.GetPresentationDomNode(htmlElem);
            CssBox cssHingeBox = host.CreateCssBox(parentBox, hingeBoxDom, true); //create and append to the parentBox 
            //
            hingeBoxDom.SetSubParentNode(htmlElem);
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


        class TextBoxInputSubDomExtender : IHtmlInputSubDomExtender
        {
            LayoutFarm.CustomWidgets.TextBoxContainer _textboxContainer;
            public TextBoxInputSubDomExtender(LayoutFarm.CustomWidgets.TextBoxContainer textboxContainer)
            {
                _textboxContainer = textboxContainer;
            }
            string IHtmlInputSubDomExtender.GetInputValue() => _textboxContainer.GetText();
            void IHtmlInputSubDomExtender.SetInputValue(string value) => _textboxContainer.SetText(value);
            void IHtmlInputSubDomExtender.Focus() => _textboxContainer.Focus();
            void ISubDomExtender.Write(System.Text.StringBuilder stbuilder)
            {
                stbuilder.Append(_textboxContainer.GetText());
            }
        }



        CssBox CreateInputBox(HtmlElement domE,
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
            HtmlInputElement htmlInputElem = (HtmlInputElement)domE;
            var typeAttr = domE.FindAttribute("type");
            if (typeAttr != null)
            {
                switch (typeAttr.Value)
                {

                    case "password":
                        {
                            var textbox = new LayoutFarm.CustomWidgets.TextBoxContainer(100, 20, false, true);
                            var subdomExtender = new TextBoxInputSubDomExtender(textbox);

                            CssBox wrapperBox = CreateCssWrapper(
                                 host,
                                 textbox,
                                 textbox.GetPrimaryRenderElement(rootgfx),
                                 spec,
                                 subdomExtender,
                                 true);

                            textbox.KeyDown += (s, e) =>
                            {
                                ((LayoutFarm.UI.IUIEventListener)htmlInputElem).ListenKeyDown(e);
                            };

                         
                            htmlInputElem.SubDomExtender = subdomExtender;//connect 

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
                            //TODO: user can specific width of textbox 
                            var textbox = new LayoutFarm.CustomWidgets.TextBoxContainer(100, 20, false);
                            var subdomExtender = new TextBoxInputSubDomExtender(textbox);

                            CssBox wrapperBox = CreateCssWrapper(
                                 host,
                                 textbox,
                                 textbox.GetPrimaryRenderElement(rootgfx),
                                 spec,
                                 subdomExtender,
                                 true);

                            textbox.KeyDown += (s, e) =>
                            {
                                ((LayoutFarm.UI.IUIEventListener)htmlInputElem).ListenKeyDown(e);
                            };

                            htmlInputElem.SubDomExtender = subdomExtender;//connect 

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

                            HtmlElement buttonDom = button.GetPresentationDomNode(domE);
                            buttonDom.SetAttribute("style", "width:20px;height:20px;background-color:white;cursor:pointer");
                            CssBox buttonCssBox = host.CreateCssBox(parentBox, buttonDom, true);
                            parentBox.AppendChild(buttonCssBox);
                            return buttonCssBox;
                        }
                    case "checkbox":
                        {
                            //implement with choice box + multiple value

                            var chkbox = new HtmlWidgets.ChoiceBox(18, 10);
                            chkbox.SetHtmlInputBox(htmlInputElem);
                            chkbox.OnlyOne = false; //*** show as checked box 

                            HtmlElement chkBoxElem = chkbox.GetPresentationDomNode(domE);
                            //buttonDom.SetAttribute("style", "width:20px;height:20px;background-color:red;cursor:pointer");

                            CssBox chkCssBox = host.CreateCssBox(parentBox, chkBoxElem, true); //create and append to the parentBox
                            htmlInputElem.SubDomExtender = chkbox;//connect 

#if DEBUG
                            chkCssBox.dbugMark1 = 1;
#endif
                            return chkCssBox;
                        }

                    case "radio":
                        {

                            var radio = new HtmlWidgets.ChoiceBox(10, 10);
                            radio.OnlyOne = true;// show as option box  
                            HtmlElement radioElem = radio.GetPresentationDomNode(domE);
                            //buttonDom.SetAttribute("style", "width:20px;height:20px;background-color:red;cursor:pointer");

                            CssBox buttonCssBox = host.CreateCssBox(parentBox, radioElem, true); //create and append to the parentBox
                            htmlInputElem.SubDomExtender = radio;//connect 

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
                            CssBox wrapperBox = CreateCssWrapper(
                                 host,
                                 box,
                                 box.GetPrimaryRenderElement(rootgfx),
                                 spec,
                                 null,
                                 true);
                            parentBox.AppendChild(wrapperBox);
                            return wrapperBox;
                        }
                }
            }
            return null;
        }
    }
}
