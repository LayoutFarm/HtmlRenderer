//Apache2, 2014-2018, WinterDev

using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
using LayoutFarm.Composers;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.HtmlBoxes;
namespace LayoutFarm
{
    [DemoNote("4.5 LightHtmlBox")]
    class Demo_LightHtmlBox : DemoBase
    {
        HtmlHost htmlHost;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport, null, null);
            ////==================================================
            //html box
            {
                HtmlBox lightHtmlBox = new HtmlBox(htmlHost, 800, 50);
                lightHtmlBox.SetLocation(50, 450);
                viewport.AddChild(lightHtmlBox);
                //light box can't load full html
                //all light boxs of the same lightbox host share resource with the host
                string html = @"<div>OK1</div><div>OK2</div>";
                //if you want to use full html-> use HtmlBox instead  
                lightHtmlBox.LoadHtmlFragmentString(html);
            }
            //==================================================  
            {
                HtmlBox lightHtmlBox2 = new HtmlBox(htmlHost, 800, 50);
                lightHtmlBox2.SetLocation(0, 60);
                viewport.AddChild(lightHtmlBox2);
                //light box can't load full html
                //all light boxs of the same lightbox host share resource with the host
                string html2 = @"<div>OK3</div><div>OK4</div>";
                //if you want to use ful l html-> use HtmlBox instead  
                lightHtmlBox2.LoadHtmlFragmentString(html2);
            }
            //==================================================  
            {
                HtmlBox lightHtmlBox3 = new HtmlBox(htmlHost, 800, 50);
                lightHtmlBox3.SetLocation(0, 100);
                viewport.AddChild(lightHtmlBox3);
                //fragment dom 
                //create dom then to thie light box
                lightHtmlBox3.LoadHtmlDom(CreateSampleHtmlDoc());
            }
            //================================================== 
            //textbox
            var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 150, true);
            textbox.SetLocation(0, 200);
            viewport.AddChild(textbox);
            textbox.Focus();
        }
        HtmlDocument CreateSampleHtmlDoc()
        {
            HtmlDocument htmldoc = htmlHost.CreateNewSharedHtmlDoc();// new HtmlDocument();
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
                        //Console.WriteLine("div");
#endif

                    });
                });
            });
            return htmldoc;
        }
    }
}