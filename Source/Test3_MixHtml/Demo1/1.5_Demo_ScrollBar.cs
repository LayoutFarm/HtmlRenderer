//Apache2, 2014-2017, WinterDev

namespace LayoutFarm
{
    [DemoNote("1.5 ScrollBar")]
    class Demo_ScrollBar : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            //----------------------------------------------------------------
            {
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                scbar.SetLocation(10, 10);
                scbar.MinValue = 0;
                scbar.MaxValue = 100;
                scbar.SmallChange = 50;
                viewport.AddContent(scbar);
            }
            //----------------------------------------------------------------
            {
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                scbar.SetLocation(30, 10);
                scbar.MinValue = 0;
                scbar.MaxValue = 100;
                scbar.SmallChange = 25;
                viewport.AddContent(scbar);
            }
            //----------------------------------------------------------------
            {
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                scbar.SetLocation(50, 10);
                scbar.MinValue = 0;
                scbar.MaxValue = 1000;
                scbar.SmallChange = 100;
                viewport.AddContent(scbar);
            }
            //-------------------------------------
            {
                //horizontal scrollbar
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(200, 15);
                scbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                scbar.SetLocation(80, 10);
                scbar.MinValue = 0;
                scbar.MaxValue = 100;
                scbar.SmallChange = 50;
                viewport.AddContent(scbar);
            }
            {
                //horizontal scrollbar
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(200, 15);
                scbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                scbar.SetLocation(80, 40);
                scbar.MinValue = 0;
                scbar.MaxValue = 100;
                scbar.SmallChange = 25;
                viewport.AddContent(scbar);
            }

            {
                //horizontal scrollbar
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(200, 15);
                scbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                scbar.SetLocation(80, 80);
                scbar.MinValue = 0;
                scbar.MaxValue = 1000;
                scbar.SmallChange = 100;
                viewport.AddContent(scbar);
            }
        }
    }
}