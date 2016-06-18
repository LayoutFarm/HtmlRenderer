using LayoutFarm.WebDom;
using LayoutFarm.UI;
namespace LayoutFarm.Demo
{
    class Demo01_CreateHtmlDomStyle1 : DemoBase
    {
        public Demo01_CreateHtmlDomStyle1()
        {
        }
        protected override void OnStartDemo(HtmlPanel panel)
        {
            var htmldoc = panel.HtmlHost.CreatePresentationHtmlDoc();
            var rootNode = htmldoc.RootNode;
            //1. create body node             
            // and content 

            DomElement body, div, span;
            //style 1
            rootNode.AddChild("body")
                        .AddChild("div", out div)
                            .AddChild("span", out span);
            //-------------------------------------------- 
            span.AddTextContent("ABCD");
            //2. add to view 
            panel.LoadHtmlDom(htmldoc,
               LayoutFarm.Composers.CssDefaults.DefaultStyleSheet);
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
            div.AttachEvent(UIEventName.MouseDown, e =>
            {
#if DEBUG
                //this will not print 
                //if e has been stop by its child
                // System.Diagnostics.Debugger.Break();
                //Console.WriteLine("div");
#endif

            });
        }
    }
}