//MIT, 2014-present, WinterDev
using LayoutFarm.Composers;
using LayoutFarm.UI;
using LayoutFarm.WebWidgets;
namespace LayoutFarm.Demo
{
    class Demo05_Dynamic_BoxSpec : HtmlDemoBase
    {
        public Demo05_Dynamic_BoxSpec()
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
//                        span.AttachMouseDownEvent(e =>
//                        {
//#if DEBUG

//                            var s_span = new EaseScriptElement(span);
//                            s_span.ChangeFontColor(PixelFarm.Drawing.Color.Blue);
//#endif

//                            e.StopPropagation();
//                        });
//                    });
//                    div.AddChild("span", span =>
//                    {
//                        span.AddTextContent("EFGHIJK");
//                        span.AttachMouseDownEvent(e =>
//                        {
//                            span.ClearAllElements();
//                            span.AddTextContent("LMNOP0003");
//                            var s_span = new EaseScriptElement(span);
//                            s_span.ChangeFontColor(PixelFarm.Drawing.Color.Red);
//                            s_span.ChangeBackgroundColor(PixelFarm.Drawing.Color.Yellow);
//                        });
//                        span.AttachMouseUpEvent(e =>
//                        {
//                            var s_span = new EaseScriptElement(span);
//                            s_span.ChangeFontColor(PixelFarm.Drawing.Color.Black);
//                            s_span.ChangeBackgroundColor(PixelFarm.Drawing.Color.White);
//                        });
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