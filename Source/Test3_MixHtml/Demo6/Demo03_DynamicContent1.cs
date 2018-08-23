//MIT, 2014-present, WinterDev
using LayoutFarm.UI;
using LayoutFarm.WebWidgets;
namespace LayoutFarm.Demo
{
    class Demo03_DynamicContent1 : HtmlDemoBase
    {
        public Demo03_DynamicContent1()
        {
        }
//        protected override void OnStartDemo(HtmlPanel panel)
//        {
//            var htmldoc = panel.HtmlHost.CreatePresentationHtmlDoc();
//            var rootNode = htmldoc.RootNode;
//            //1. create body node             
//            // and content  

//            //style 2, lambda and adhoc attach event
//            rootNode.AddChild("body", body =>
//            {
//                body.AddChild("div", div =>
//                {
//                    div.AddChild("span", span =>
//                    {
//                        span.AddTextContent("ABCD");
//                        //3. attach event to specific span
//                        span.AttachEvent(UIEventName.MouseDown, e =>
//                        {
//#if DEBUG
//                            // System.Diagnostics.Debugger.Break();                           
//                            //test change span property

//                            //clear prev content and add new  text content 
//                            span.ClearAllElements();
//                            span.AddTextContent("XYZ0001");
//                            //affect layout of html dom
//                            panel.ForceRefreshHtmlDomChange(htmldoc);
//#endif

//                            e.StopPropagation();
//                        });
//                    });
//                    div.AddChild("span", span =>
//                    {
//                        span.AddTextContent("EFGHIJK");
//                    });
//                    //----------------------

//                    div.AttachEvent(UIEventName.MouseDown, e =>
//                    {
//#if DEBUG
//                        //this will not print 
//                        //if e has been stop by its child
//                        // System.Diagnostics.Debugger.Break();
//                        //Console.WriteLine("div");
//#endif

//                    });
//                });
//            });
//            //2. add to view 
//            panel.LoadHtmlDom(htmldoc,
//               LayoutFarm.Composers.CssDefaults.DefaultStyleSheet);
//        }
    }
}