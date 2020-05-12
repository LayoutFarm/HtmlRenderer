//BSD, 2014-present, WinterDev 

using PixelFarm.Drawing;
using LayoutFarm.Css;
namespace LayoutFarm.HtmlBoxes
{
     

    class CssIsolateBox : CssBox
    {
        public CssIsolateBox(BoxSpec spec)
            : base(spec)
        {
        }
    }

    class RenderElementBridgeCssBox : CssBox
    {
        public RenderElementBridgeCssBox(BoxSpec spec,
            RenderElement containerElement)
            : base(spec)
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
