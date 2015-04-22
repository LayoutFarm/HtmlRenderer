// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
using LayoutFarm.WebDom;

namespace LayoutFarm
{
    [DemoNote("4.6 dynamic dom1")]
    class Demo_DynamicDom1 : DemoBase
    {
        HtmlBox htmlMenuBox;
        HtmlBox testHtmlBox;
        HtmlBoxes.HtmlHost htmlhost;
        SampleViewport viewport;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.viewport = viewport;
            this.htmlhost = HtmlHostCreatorHelper.CreateHtmlHost(viewport, null, null);
            SetupHtmlMenuBox();
            //==================================================
            //box1 test area
            //html box
            this.testHtmlBox = new HtmlBox(htmlhost, 800, 400);
            testHtmlBox.SetLocation(30, 50);
            viewport.AddContent(testHtmlBox);
            string html = @"<html><head></head><body><div id='div1'>OK1</div><div>OK2</div></body></html>";
            testHtmlBox.LoadHtmlString(html);
            //================================================== 

            var htmldoc = testHtmlBox.HtmlContainer.WebDocument as HtmlDocument;
            HtmlElement div1 = htmldoc.GetElementById("div1") as HtmlElement;
            if (div1 != null)
            {
                //test set innerHTML
                div1.SetInnerHtml("<div>OKOKOK</div>");
            }
        }
        void SetupHtmlMenuBox()
        {
            //==================================================
            this.htmlMenuBox = new HtmlBox(htmlhost, 800, 40);
            htmlMenuBox.SetLocation(30, 0);
            viewport.AddContent(htmlMenuBox);
            string html = @"<html><head></head><body>
                    <div id='menubox'>
                        <span id='test_dom1'>test dom1</span>
                        <span id='test_dom2'>test dom2</span>
                    </div>
            </body></html>";
            htmlMenuBox.LoadHtmlString(html);

            //set event handlers
            var htmldoc = htmlMenuBox.HtmlContainer.WebDocument as HtmlDocument;
            HtmlElement div_menuBox = htmldoc.GetElementById("menubox") as HtmlElement;
            if (div_menuBox == null) return;

            //test set innerHTML
            div_menuBox.AttachEvent(UIEventName.MouseDown, (e) =>
            {
                var contextE = e.CurrentContextElement as HtmlElement;

            });

        }

    }

}