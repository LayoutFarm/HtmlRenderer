// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.CustomWidgets;


namespace LayoutFarm.WebWidgets
{
    [DemoNote("5.9 DemoTabPage")]
    class DemoTabPage : HtmlDemoBase
    {
        protected override void OnHtmlHostCreated()
        {

            var tabContainer = new LayoutFarm.HtmlWidgets.TabPageContainer(300, 400);
            tabContainer.SetLocation(10, 10);
            AddToViewport(tabContainer);
            for (int i = 0; i < 10; ++i)
            {
                var tabPage = new LayoutFarm.HtmlWidgets.TabPage();
                tabPage.PageTitle = "page" + i;

                //test bridge 
                //var panel01 = new CustomWidgets.Panel(300, 200);
                //panel01.BackColor = Color.OrangeRed;
                //tabPage.ContentUI = panel01;

                var lightHtmlBox = new CustomWidgets.LightHtmlBox(this.myHtmlHost, 300, 200);
                lightHtmlBox.LoadHtmlDom(CreateSampleHtmlDoc(tabPage.PageTitle));
                tabPage.ContentUI = lightHtmlBox;


                tabContainer.AddItem(tabPage);
            }

            //tabContainer.SelectedIndex = 1; 
        }
        FragmentHtmlDocument CreateSampleHtmlDoc(string pageNote)
        {
            FragmentHtmlDocument htmldoc = this.myHtmlHost.CreateNewFragmentHtml();// new HtmlDocument();
            var rootNode = htmldoc.RootNode;
            //1. create body node             
            // and content  

            //style 2, lambda and adhoc attach event
            rootNode.AddChild("body", body =>
            {
                body.AddChild("div", div =>
                {
                    div.AddChild("span", span =>
                    {
                        span.SetAttribute("style", "font:16pt tahoma;background-color:red");
                        span.AddTextContent("ABCD:" + pageNote);
                        //3. attach event to specific span
//                        span.AttachMouseDownEvent(e =>
//                        {
//#if DEBUG
//                            // System.Diagnostics.Debugger.Break();                           
//                            //test change span property 
//                            //clear prev content and add new  text content 
//                            span.ClearAllElements();
//                            span.AddTextContent("XYZ0001");
//#endif

//                            e.StopPropagation();

//                        });
                    });

                    div.AddChild("span", span =>
                    {
                        span.AddTextContent("EFGHIJK");
                        //span.AttachMouseDownEvent(e =>
                        //{
                        //    span.ClearAllElements();
                        //    span.AddTextContent("LMNOP0003");
                        //});
                    });
                    //----------------------
                    div.AttachEvent(UIEventName.MouseDown, e =>
                    {
#if DEBUG
                        //this will not print 
                        //if e has been stop by its child
                        // System.Diagnostics.Debugger.Break();
                        //Console.WriteLine("div");
#endif

                    });
                });
            });


            return htmldoc;
        }
    }
}