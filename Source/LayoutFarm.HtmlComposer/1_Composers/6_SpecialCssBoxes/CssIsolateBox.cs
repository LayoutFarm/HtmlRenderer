//BSD, 2014-2017, WinterDev 

using PixelFarm.Drawing;
using LayoutFarm.Css;
namespace LayoutFarm.HtmlBoxes
{
    class IsolateBoxRootGfx : IRootGraphics
    {
        RootGraphic rootgfx;
        public IsolateBoxRootGfx(RootGraphic rootgfx)
        {
            this.rootgfx = rootgfx;
        }
    }
    class CssIsolateBox : CssBox
    {

        public CssIsolateBox(BoxSpec spec, RootGraphic rootgfx)
            : base(spec, new IsolateBoxRootGfx(rootgfx))
        {
        }
    }

    class RenderElementBridgeCssBox : CssBox
    {
        RenderElement containerElement;
        public RenderElementBridgeCssBox(BoxSpec spec,
            RenderElement containerElement,
            RootGraphic rootgfx)
            : base(spec, new IsolateBoxRootGfx(rootgfx))
        {
            this.containerElement = containerElement;
        }
        public override void InvalidateGraphics(Rectangle clientArea)
        {
            //send to container element
            this.containerElement.InvalidateGraphicBounds(clientArea);
        }
        public LayoutFarm.RenderElement ContainerElement
        {
            get { return this.containerElement; }
        }
        protected override CssBox GetGlobalLocationImpl(out float globalX, out float globalY)
        {
            Point p = containerElement.GetGlobalLocation();
            globalX = p.X;
            globalY = p.Y;
            return this;
        }
    }
}
