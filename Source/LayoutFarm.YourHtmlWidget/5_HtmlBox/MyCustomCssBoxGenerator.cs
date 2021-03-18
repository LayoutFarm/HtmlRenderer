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
        TextBoxSwitcher _textboxSwitcher;

        public MyCustomCssBoxGenerator(HtmlBoxes.HtmlHost myHost)
        {
            _myHost = myHost;

            _textboxSwitcher = new TextBoxSwitcher();

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
                        CssBox selectedBox = CreateSelectBox(domE, parentBox, spec, host);
                        if (selectedBox != null)
                        {
                            return selectedBox;
                        }
                    }
                    break;
                case "input":
                    {
                        CssBox inputBox = CreateInputBox(domE, parentBox, spec, host);
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
                             canvas.GetPrimaryRenderElement(),
                             spec,
                             null,
                             true);
                        parentBox.AppendChild(wrapperBox);
                        return wrapperBox;
                    }
                case "textarea":
                    {
                        CssBox textAreaCssBox = CreateTextAreaElement(domE, parentBox, spec, host);
                        if (textAreaCssBox != null)
                        {
                            return textAreaCssBox;
                        }
                    }
                    break;
            }

            //default unknown
            var simpleBox = new LayoutFarm.CustomWidgets.Box(100, 20);
            simpleBox.BackColor = PixelFarm.Drawing.KnownColors.LightGray;
            CssBox wrapperBox2 = CreateCssWrapper(
                               host,
                               simpleBox,
                               simpleBox.GetPrimaryRenderElement(),
                               spec,
                               null,
                               false);
            parentBox.AppendChild(wrapperBox2);
            return wrapperBox2;
        }


        CssBox CreateSelectBox(HtmlElement htmlElem,
            CssBox parentBox,
            BoxSpec spec,
            HtmlHost host)
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

        class TextAreaInputSubDomExtender : IHtmlTextAreaSubDomExtender
        {
            LayoutFarm.CustomWidgets.TextBoxContainer _textboxContainer;
            public TextAreaInputSubDomExtender(LayoutFarm.CustomWidgets.TextBoxContainer textboxContainer)
            {
                _textboxContainer = textboxContainer;
            }
            string IHtmlTextAreaSubDomExtender.GetInputValue() => _textboxContainer.GetText();
            void IHtmlTextAreaSubDomExtender.SetInputValue(string value) => _textboxContainer.SetText(value);
            void IHtmlTextAreaSubDomExtender.Focus() => _textboxContainer.Focus();
            void ISubDomExtender.Write(System.Text.StringBuilder stbuilder)
            {
                stbuilder.Append(_textboxContainer.GetText());
            }
        }

        CssBox CreateTextAreaElement(HtmlElement domE,
            CssBox parentBox,
            BoxSpec spec,
            HtmlHost host)
        {
            //mulitline
            //TODO: review default size of a textarea...

            HtmlTextAreaElement htmlTextAreaElem = (HtmlTextAreaElement)domE;
            var textbox = new LayoutFarm.CustomWidgets.TextBoxContainer(100, 60, true);
            var subdomExtender = new TextAreaInputSubDomExtender(textbox);

            CssBox wrapperBox = CreateCssWrapper(
                 host,
                 textbox,
                 textbox.GetPrimaryRenderElement(),
                 spec,
                 subdomExtender,
                 true);

            textbox.KeyDown += (s, e) =>
            {
                ((LayoutFarm.UI.IUIEventListener)domE).ListenKeyDown(e);
            };

            htmlTextAreaElem.SubDomExtender = subdomExtender;//connect 

            //place holder support
            DomAttribute placeHolderAttr = domE.FindAttribute("placeholder");
            if (placeHolderAttr != null)
            {
                textbox.PlaceHolderText = placeHolderAttr.Value;
            }
            parentBox.AppendChild(wrapperBox);

            //content of text area 
            HtmlTextNode textNode = null;
            foreach (DomNode child in domE.GetChildNodeIterForward())
            {
                switch (child.NodeKind)
                {
                    case HtmlNodeKind.TextNode:
                        {
                            textNode = (HtmlTextNode)child;
                        }
                        break;
                }
                if (textNode != null)
                {
                    break;
                }
            }
            if (textNode != null)
            {
                //if first line is blank line we skip
                //TODO: review here

                textbox.SetText(new string(textNode.GetOriginalBuffer()));

                //System.Collections.Generic.List<string> strList = new System.Collections.Generic.List<string>();
                //int lineCount = 0;
                //using (System.IO.StringReader strReader = new System.IO.StringReader())
                //{
                //    string line = strReader.ReadLine();
                //    while (line != null)
                //    {
                //        if (lineCount == 0)
                //        {
                //            if (line.Trim() != string.Empty)
                //            {
                //                strList.Add(line);
                //            }
                //        }
                //        else
                //        {
                //            strList.Add(line);
                //        }

                //        lineCount++;
                //        line = strReader.ReadLine();
                //    }

                //    if (strList.Count > 0)
                //    {
                //        //check last line
                //        line = strList[strList.Count - 1];
                //        if (line.Trim() == string.Empty)
                //        {
                //            strList.RemoveAt(strList.Count - 1);
                //        }
                //    }
                //}
                ////
                //if (strList.Count > 0)
                //{
                //    textbox.SetText(strList);
                //}
            }

            return wrapperBox;
        }

        CssBox CreateInputBox(HtmlElement domE,
            CssBox parentBox,
            BoxSpec spec,
            HtmlHost host)
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
                            textbox.TextBoxSwitcher = _textboxSwitcher;

                            var subdomExtender = new TextBoxInputSubDomExtender(textbox);

                            CssBox wrapperBox = CreateCssWrapper(
                                 host,
                                 textbox,
                                 textbox.GetPrimaryRenderElement(),
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
                            textbox.TextBoxSwitcher = _textboxSwitcher;

                            var subdomExtender = new TextBoxInputSubDomExtender(textbox);

                            CssBox wrapperBox = CreateCssWrapper(
                                 host,
                                 textbox,
                                 textbox.GetPrimaryRenderElement(),
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
                                 box.GetPrimaryRenderElement(),
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
