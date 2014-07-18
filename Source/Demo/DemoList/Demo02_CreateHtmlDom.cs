using System;
using HtmlRenderer.Composers;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Demo
{

    class Demo02CreateHtmlDom : DemoBase
    {


        public Demo02CreateHtmlDom()
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
                        span.AttachEvent(EventName.MouseDown, e =>
                        {

                            //-------------------------------
                            //mousedown on specific span !
                            //-------------------------------
#if DEBUG
                            // System.Diagnostics.Debugger.Break();
                            Console.WriteLine("span");
#endif
                            //test stop propagation 
                            e.StopPropagation();

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