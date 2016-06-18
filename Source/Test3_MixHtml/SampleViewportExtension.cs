// 2015,2014 ,Apache2, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm
{
    public static class SampleViewportExtension
    {
        public static void AddContent(this SampleViewport viewport, UIElement ui)
        {
            viewport.ViewportControl.AddContent(
                ui.GetPrimaryRenderElement(viewport.ViewportControl.RootGfx),
                ui);
        }
    }
}