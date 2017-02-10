//Apache2, 2014-2017, WinterDev

namespace LayoutFarm
{
    [DemoNote("2.1 SingleLineText")]
    class Demo_SingleLineText : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 30, false);
            viewport.AddContent(textbox);
            textbox.InvalidateGraphics();
        }
    }
}