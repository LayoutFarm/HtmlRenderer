// 2015,2014 ,Apache2, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm
{
    public static class RenderElementExtension
    {
        public static void AddChild(this RenderElement renderBox, UIElement ui)
        {
            renderBox.AddChild(ui.GetPrimaryRenderElement(renderBox.Root));
        }
    }
}