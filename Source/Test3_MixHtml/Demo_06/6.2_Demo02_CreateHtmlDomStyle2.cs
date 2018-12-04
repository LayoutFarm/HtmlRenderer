//MIT, 2014-present, WinterDev
using LayoutFarm.UI;
using LayoutFarm.WebWidgets;
namespace LayoutFarm.Demo
{
    [DemoNote("6.2 Demo02_CreateHtmlDomStyle2")]
    class Demo02_CreateHtmlDomStyle2 : HtmlDemoBase
    {
        public Demo02_CreateHtmlDomStyle2()
        {
        }
        protected override void OnStart(AppHost host)
        {
            base.OnStart(host);//setup

            //
            var htmldoc = _groundHtmlDoc;
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
                        span.AttachEvent(UIEventName.MouseDown, e =>
                        {
                            //-------------------------------
                            //mousedown on specific span !
                            //-------------------------------
#if DEBUG
                            // System.Diagnostics.Debugger.Break();
                            //Console.WriteLine("span");
#endif

                            //test stop propagation 
                            e.StopPropagation();
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
        }

    }
}