// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.UI;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.CustomWidgets;

namespace LayoutFarm.HtmlWidgets
{

    public class Button : LightHtmlWidgetBase
    {
        string buttonText = "";
        public Button(HtmlIslandHost htmlIslandHost, int w, int h)
            : base(htmlIslandHost, w, h)
        {
        }
        //---------------------------------------------------------------------------
        public string Text
        {
            get { return this.buttonText; }
            set
            {
                this.buttonText = value;
            }
        } 
        protected override FragmentHtmlDocument CreatePresentationDom()
        {

            FragmentHtmlDocument htmldoc = this.HtmlIslandHost.CreateNewFragmentHtml();
            //TODO: use template engine, 
            //ideas:  AngularJS style ?

            //1. create body node             
            // and content   
            //style 2, lambda and adhoc attach event  
            var domElement =
                htmldoc.RootNode.AddChild("div", div =>
                {
                    div.SetAttribute("style", "font:10pt tahoma;");
                    div.AddChild("span", span =>
                    {
                        span.AddTextContent(this.buttonText);
                    });
                });

            return htmldoc;
        }
    }
}