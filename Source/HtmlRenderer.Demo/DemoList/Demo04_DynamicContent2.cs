using System;
using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm;
using LayoutFarm.UI;
namespace LayoutFarm.Demo
{

    class Demo04_DynamicContent2 : DemoBase
    {

        public Demo04_DynamicContent2()
        {

        }
        protected override void OnStartDemo(HtmlPanel panel)
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
                        //Console.WriteLine("div");
#endif

                    });
                });
            });

            //2. add to view 
            panel.LoadHtmlDom(htmldoc,
               LayoutFarm.Composers.CssDefaults.DefaultStyleSheet);


        }
    }



}