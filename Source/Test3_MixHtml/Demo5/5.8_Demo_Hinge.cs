//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.WebWidgets
{
    [DemoNote("5.3 Hinge")]
    class Demo_Hinge : HtmlDemoBase
    {
        protected override void OnHtmlHostCreated()
        {
            //-------------------------------
            int boxX = 0;
            for (int i = 0; i < 1; ++i)
            {
                var hingeBox = CreateHingeBox(100, 30);
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