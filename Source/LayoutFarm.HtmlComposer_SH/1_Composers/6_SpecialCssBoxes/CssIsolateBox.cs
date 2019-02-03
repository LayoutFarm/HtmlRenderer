//BSD, 2014-present, WinterDev 

using PixelFarm.Drawing;
using LayoutFarm.Css;
namespace LayoutFarm.HtmlBoxes
{
    public class CssBoxRootGfxBridge : IRootGraphics
    {
        LayoutFarm.RootGraphic _rootgfx;
        public CssBoxRootGfxBridge(LayoutFarm.RootGraphic rootgfx)
        {
            _rootgfx = rootgfx;
        }
        public LayoutFarm.RootGraphic RootGfx => _rootgfx;
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
        RenderElement _containerElement;
        public RenderElementBridgeCssBox(BoxSpec spec,
            RenderElement containerElement,
            RootGraphic rootgfx)
            : base(spec, new CssBoxRootGfxBridge(rootgfx))
        {
            _containerElement = containerElement;
        }
        public override void InvalidateGraphics(Rectangle clientArea)
        {
            //send to container element
            clientArea.Offset(_containerElement.X, _containerElement.Y);
            _containerElement.InvalidateParentGraphics(clientArea);
        }
        public LayoutFarm.RenderElement ContainerElement => _containerElement;

        protected override void GetGlobalLocationImpl(out float globalX, out float globalY)
        {
            Point p = _containerElement.GetGlobalLocation();
            globalX = p.X;
            globalY = p.Y;
            //return this;
        }
    }
}
