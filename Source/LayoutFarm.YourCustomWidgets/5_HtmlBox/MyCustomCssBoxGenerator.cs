//BSD 2014-2015 ,WinterDev
//ArthurHub, Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;

using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;

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
            LayoutFarm.RootGraphic rootgfx,
            out bool alreadyHandleChildNodes)
        {
            //check if this is a proper tag  
            alreadyHandleChildNodes = true;

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
            ////------------------------------------------------
            //CreateCssBoxDelegate foundCreator;
            //if (generalCssBoxCreators.TryGetValue(domE.Name, out foundCreator))
            //{
            //    CssBox result = foundCreator(domE, parentBox, spec, rootgfx, out alreadyHandleChildNodes);
            //    return result;
            //} 

            var simpleBox = new LayoutFarm.CustomWidgets.SimpleBox(100, 20); //default unknown
            simpleBox.BackColor = PixelFarm.Drawing.Color.LightGray;
            var wrapperBox2 = CreateWrapper(
                               simpleBox,
                               simpleBox.GetPrimaryRenderElement(rootgfx),
                               spec, false);
            parentBox.AppendChild(wrapperBox2);
            return wrapperBox2;
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
