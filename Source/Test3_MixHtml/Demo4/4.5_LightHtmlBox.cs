//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
using LayoutFarm.InternalHtmlDom;
using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.HtmlBoxes;
namespace LayoutFarm
{
    [DemoNote("4.5 LightHtmlBox")]
    class Demo_LightHtmlBox : DemoBase
    {
        HtmlIslandHost islandHost;
        LightHtmlBoxHost lightBoxHost;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.islandHost = new HtmlIslandHost();
            this.islandHost.BaseStylesheet = LayoutFarm.Composers.CssParserHelper.ParseStyleSheet(null, true);

            lightBoxHost = new LightHtmlBoxHost(islandHost, viewport.P); 
            lightBoxHost.SetRootGraphic(viewport.ViewportControl.WinTopRootGfx);

            ////==================================================
            //html box
            {
                LightHtmlBox lightHtmlBox = lightBoxHost.CreateLightBox(800, 50);
                lightHtmlBox.SetLocation(50, 450);
                viewport.AddContent(lightHtmlBox);
                //light box can't load full html
                //all light boxs of the same lightbox host share resource with the host
                string html = @"<div>OK1</div><div>OK2</div>";
                //if you want to use full html-> use HtmlBox instead  
                lightHtmlBox.LoadHtmlFragmentText(html);
            }
            //==================================================  
            {
                LightHtmlBox lightHtmlBox2 = lightBoxHost.CreateLightBox(800, 50);
                lightHtmlBox2.SetLocation(0, 60);
                viewport.AddContent(lightHtmlBox2);
                //light box can't load full html
                //all light boxs of the same lightbox host share resource with the host
                string html2 = @"<div>OK3</div><div>OK4</div>";
                //if you want to use ful l html-> use HtmlBox instead  
                lightHtmlBox2.LoadHtmlFragmentText(html2);
            }
            //==================================================  
            {
                LightHtmlBox lightHtmlBox3 = lightBoxHost.CreateLightBox(800, 50);
                lightHtmlBox3.SetLocation(0, 100);
                viewport.AddContent(lightHtmlBox3);
                //fragment dom 
                //create dom then to thie light box
                lightHtmlBox3.LoadHtmlFragmentDom(CreateBridgeDoc());

            }
            //================================================== 
            //textbox
            var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 150, true);
            textbox.SetLocation(0, 200);
            viewport.AddContent(textbox);
            textbox.Focus();
        }
        HtmlDocument CreateBridgeDoc()
        {
            HtmlDocument htmldoc = new HtmlDocument();
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
                        span.AddTextContent("ABCD");
                        //3. attach event to specific span
                        span.AttachMouseDownEvent(e =>
                        {
#if DEBUG
                            // System.Diagnostics.Debugger.Break();                           
                            //test change span property 
                            //clear prev content and add new  text content 
                            span.ClearAllElements();
                            span.AddTextContent("XYZ0001");
#endif

                            e.StopPropagation();

                        });
                    });

                    div.AddChild("span", span =>
                    {
                        span.AddTextContent("EFGHIJK");
                        span.AttachMouseDownEvent(e =>
                        {
                            span.ClearAllElements();
                            span.AddTextContent("LMNOP0003");
                        });
                    });
                    //----------------------
                    div.AttachEvent(UIEventName.MouseDown, e =>
                    {
#if DEBUG
                        //this will not print 
                        //if e has been stop by its child
                        // System.Diagnostics.Debugger.Break();
                        Console.WriteLine("div");
#endif

                    });
                });
            });


            return htmldoc;
        }
    }
}