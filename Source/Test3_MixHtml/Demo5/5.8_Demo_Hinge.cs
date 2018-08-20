//Apache2, 2014-present, WinterDev
using LayoutFarm.WebDom.Impl;
using LayoutFarm.WebDom.Extension;

namespace LayoutFarm.WebWidgets
{
    [DemoNote("5.8 Hinge")]
    class Demo_Hinge : HtmlDemoBase
    {
        protected override void OnHtmlHostCreated()
        {
            //-------------------------------
            int boxX = 0;
            for (int i = 0; i < 1; ++i)
            {
                var hingeBox = CreateHingeBox(100, 30);
                for (int m = 0; m < 10; ++m)
                {
                    var div = (HtmlElement)_groundHtmlDoc.CreateElement("div");
                    div.AddChild("div", div2 =>
                    {
                        div2.AddTextContent("HELLO!" + m);
                        div2.Tag = m.ToString();
                    });
                    

                    hingeBox.AddItem(div);
                }

                hingeBox.SetLocation(boxX, 20);
                boxX += 100 + 2;
                AddToViewport(hingeBox);
            }
        }
        LayoutFarm.HtmlWidgets.HingeBox CreateHingeBox(int w, int h)
        {
            var hingeBox = new LayoutFarm.HtmlWidgets.HingeBox(w, h);
            //1. set land part detail
            //2. set float part detail 
            return hingeBox;
        }
    }
}