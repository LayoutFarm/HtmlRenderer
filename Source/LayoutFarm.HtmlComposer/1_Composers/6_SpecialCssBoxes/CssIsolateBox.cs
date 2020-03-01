//BSD, 2014-present, WinterDev 

using PixelFarm.Drawing;
using LayoutFarm.Css;
namespace LayoutFarm.HtmlBoxes
{
    public class CssBoxRootGfxBridge : IRootGraphics
    {
        public CssBoxRootGfxBridge(LayoutFarm.RootGraphic rootgfx)
        {
            RootGfx = rootgfx;
        }
        public LayoutFarm.RootGraphic RootGfx { get; private set; }
    }
    static class CssBoxExtensions
    {
        public static RootGraphic GetInternalRootGfx(this CssBox cssbox)
        {
            return ((LayoutFarm.HtmlBoxes.CssBoxRootGfxBridge)cssbox.RootGfx).RootGfx;
        }
    }

    class CssIsolateBox : CssBox
    {
        public CssIsolateBox(BoxSpec spec, RootGraphic rootgfx)
            : base(spec, new CssBoxRootGfxBridge(rootgfx))
        {
        }
    }

    class RenderElementBridgeCssBox : CssBox
    {
        public RenderElementBridgeCssBox(BoxSpec spec,
            RenderElement containerElement,
            RootGraphic rootgfx)
            : base(spec, new CssBoxRootGfxBridge(rootgfx))
        {
            ContainerElement = containerElement;
        }
        public override void InvalidateGraphics(Rectangle clientArea)
        {
            ContainerElement.InvalidateGraphics(clientArea);
        }
        public LayoutFarm.RenderElement ContainerElement { get; private set; }
        protected override void GetGlobalLocationImpl(out float globalX, out float globalY)
        {
            Point p = ContainerElement.GetGlobalLocation();
            globalX = p.X;
            globalY = p.Y;
        }
    }
}
