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
        bool testToggle;

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
        }
        void SetupHtmlMenuBox()
        {
            //==================================================
            this.htmlMenuBox = new HtmlBox(htmlhost, 800, 40);
            htmlMenuBox.SetLocation(30, 0);
            viewport.AddContent(htmlMenuBox);
            string html = @"<html><head></head><body>
                    <div id='menubox'>
                        <span id='test_dom1'>click to toggle!</span>
                        <span id='test_dom2'>test document fragment</span>
                        <span id='test_dom3'>test showdow dom</span>
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
                var contextE = e.SourceHitElement as HtmlElement;
                switch (contextE.AttrElementId)
                {
                    case "test_dom1":
                        {
                            //test toggle with innerHTML
                            var testHtmlDoc = testHtmlBox.HtmlContainer.WebDocument as HtmlDocument;
                            HtmlElement div1 = testHtmlDoc.GetElementById("div1") as HtmlElement;

                            if (div1 != null)
                            {
                                //test set innerHTML
                                div1.SetInnerHtml(
                                    testToggle ?
                                        "<div>11111</div>" :
                                        "<div>22222</div>");
                                testToggle = !testToggle;
                            }

                        } break;
                    case "test_dom2":
                        {
                            //test toggle with DocumentFragment
                            var testHtmlDoc = testHtmlBox.HtmlContainer.WebDocument as HtmlDocument;
                            HtmlElement div1 = testHtmlDoc.GetElementById("div1") as HtmlElement;
                            if (div1 != null)
                            {
                                var docFragment =  testHtmlDoc.CreateDocumentFragment();
                                if (testToggle)
                                {
                                    var node1 = docFragment.CreateElement("div") as HtmlElement;
                                    node1.AddChild(
                                        docFragment.CreateTextNode("3333".ToCharArray()));//TODO: review this
                                    docFragment.RootNode.AddChild(node1);

                                }
                                else
                                {

                                    var node1 = docFragment.CreateElement("div") as HtmlElement;
                                    node1.AddChild(
                                        docFragment.CreateTextNode("4444".ToCharArray()));//TODO: review this
                                    docFragment.RootNode.AddChild(node1);
                                }
                                div1.ClearAllElements();
                                div1.AddChild(docFragment.RootNode);

                                testToggle = !testToggle;
                            }
                        } break;
                    case "test_dom3":
                        {
                            //shadow dom!
                            var testHtmlDoc = testHtmlBox.HtmlContainer.WebDocument as HtmlDocument;
                            HtmlElement div1 = testHtmlDoc.GetElementById("div1") as HtmlElement;

                            if (div1 != null)
                            {
                                HtmlElement shadowRoot = testHtmlDoc.CreateShadowRootElement() as HtmlElement;
                                if (testToggle)
                                {
                                    var node1 = testHtmlDoc.CreateElement("div") as HtmlElement;
                                    node1.AddChild(
                                        testHtmlDoc.CreateTextNode("XXX".ToCharArray()));//TODO: review this 
                                    //add node1 to shadow root element
                                    shadowRoot.AddChild(node1);
                                }
                                else
                                {
                                    var node1 = testHtmlDoc.CreateElement("div") as HtmlElement;
                                    node1.AddChild(
                                        testHtmlDoc.CreateTextNode("YYY".ToCharArray()));//TODO: review this 
                                    //add node1 to shadow root element
                                    shadowRoot.AddChild(node1);
                                }

                                div1.ClearAllElements();
                                div1.AddChild(shadowRoot);
                                testToggle = !testToggle;
                            }
                        } break;
                }
            });

        }

    }

}