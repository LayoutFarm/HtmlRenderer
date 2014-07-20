using System;
using HtmlRenderer.Composers;
using HtmlRenderer.WebDom;


namespace HtmlRenderer.Demo
{

    class Demo05_Dynamic_BoxSpec : DemoBase
    {

        public Demo05_Dynamic_BoxSpec()
        {

        }
        protected override void OnStartDemo(HtmlPanel panel)
        {

            BridgeHtmlDocument htmldoc = new BridgeHtmlDocument();
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
                             
                            var s_span = new EaseScriptableElement(span);
                            s_span.ChangeFontColor(System.Drawing.Color.Blue);
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
                            var s_span = new EaseScriptableElement(span);
                            s_span.ChangeFontColor(System.Drawing.Color.Red);
                        });
                    });
                    //----------------------
                    div.AttachEvent(EventName.MouseDown, e =>
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

            //2. add to view 
            panel.LoadHtmlDom(htmldoc,
               HtmlRenderer.Composers.CssDefaults.DefaultStyleSheet);


        }
    }



}