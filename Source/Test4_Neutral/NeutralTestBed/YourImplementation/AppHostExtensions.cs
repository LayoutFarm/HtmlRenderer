//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm
{
    public static class AppHostExtensions
    {
        public static void AddChild(this AppHost appHost, UIElement ui)
        {
#if DEBUG
            if (ui.ParentUI != null)
            {
                throw new System.NotSupportedException();
            }
#endif
            appHost.AddChild(ui.GetPrimaryRenderElement(appHost.RootGfx), ui);
        }
    }
}