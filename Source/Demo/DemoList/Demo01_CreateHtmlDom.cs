using System;
using HtmlRenderer.Composers;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Demo
{

    class Demo01CreateHtmlDom : DemoBase
    {

        public Demo01CreateHtmlDom()
        {

        }
        protected override void OnStartDemo(HtmlPanel panel)
        {
            BridgeHtmlDocument htmldoc = new BridgeHtmlDocument();
            var rootNode = htmldoc.RootNode;
            //1. create body node             
            // and content 

            HtmlElement span;

            rootNode.AddChild("body")
                        .AddChild("div")
                            .AddChild("span", out span);

            //--------------------------------------------
            span.AddTextContent("ABCD");
            //2. add to view 
            panel.LoadHtmlDom(htmldoc,
               HtmlRenderer.Composers.CssDefaults.DefaultStyleSheet);

            //3. attach event to specific span
            span.AttachEvent(EventName.MouseDown, e =>
            {
                //-------------------------------
                //mousedown on specific span !
                //-------------------------------
#if DEBUG
                Console.WriteLine("span");
#endif

            });
        }
    }



}