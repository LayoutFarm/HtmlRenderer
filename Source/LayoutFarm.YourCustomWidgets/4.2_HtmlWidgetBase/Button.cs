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
        public Button(HtmlHost htmlhost, int w, int h)
            : base(htmlhost, w, h)
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

            FragmentHtmlDocument htmldoc = this.HtmlHost.CreateNewFragmentHtml();
            //TODO: use template engine, 
            //ideas:  AngularJS style ?

            //1. create body node             
            // and content   
            //style 2, lambda and adhoc attach event  
            var domElement =
                htmldoc.RootNode.AddChild("div", div =>
                {
                    div.SetAttribute("style", "font:10pt tahoma;");
                    div.AddChild("div", div2 =>
                    {
                        //init
                        div2.SetAttribute("style", "padding:5px;background-color:#dddddd;");
                        div2.AddChild("span", span =>
                        {
                            span.AddTextContent(this.buttonText);
                        });
                        //------------------------------

                        div2.AttachMouseDownEvent(e =>
                        {
                            div2.dbugMark = 1;
                            // div2.SetAttribute("style", "padding:5px;background-color:#aaaaaa;");
                            EaseScriptElement ee = new EaseScriptElement(div2);
                            ee.ChangeBackgroundColor(Color.FromArgb(0xaa, 0xaa, 0xaa));

                            e.StopPropagation();
                            this.InvalidateGraphics();

                        });
                        div2.AttachMouseUpEvent(e =>
                        {
                            div2.dbugMark = 2;
                            //div2.SetAttribute("style", "padding:5px;background-color:#dddddd;");
                            //this.InvalidateGraphics();
                            EaseScriptElement ee = new EaseScriptElement(div2);
                            ee.ChangeBackgroundColor(Color.FromArgb(0xdd, 0xdd, 0xdd));
                            e.StopPropagation();

                            this.InvalidateGraphics();
                        });

                    });
                });

            return htmldoc;
        }
    }
}