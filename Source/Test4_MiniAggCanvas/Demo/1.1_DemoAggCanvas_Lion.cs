//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("1.1 DemoAggCanvas_Lion.cs")]
    class DemoAggCanvasLion : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            var bgRenderBox = new CustomRenderBox(viewport.Root, 800, 600);
            bgRenderBox.BackColor = Color.LightGray;
            viewport.AddContent(bgRenderBox);
            //---------------------------------------------------------------------
            var miniAggg = new MyMiniAggCanvasRenderElement(viewport.Root, 400, 600);
            var lionFill = new PixelFarm.Agg.LionFillSprite();
            miniAggg.AddSprite(lionFill);
            viewport.AddContent(miniAggg);
        }
    }
}