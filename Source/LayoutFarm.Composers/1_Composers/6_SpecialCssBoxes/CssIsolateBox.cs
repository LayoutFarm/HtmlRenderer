// 2015,2014 ,BSD, WinterDev 
using System.Collections.Generic;
using System.Globalization;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.Composers;
namespace LayoutFarm.HtmlBoxes
{
    class CssIsolateBox : CssBox
    {
        public CssIsolateBox(BoxSpec spec, RootGraphic rootgfx)
            : base(spec, rootgfx)
        {
        }
        public override void InvalidateGraphics()
        {
        }
    }
    
    class RenderElementBridgeCssBox : CssBox
    {
        LayoutFarm.RenderElement containerElement;
        public RenderElementBridgeCssBox(BoxSpec spec,
            LayoutFarm.RenderElement containerElement,
            RootGraphic rootgfx)
            : base(spec, rootgfx)
        {
            this.containerElement = containerElement;
        }
        public override void InvalidateGraphics()
        {
            this.containerElement.InvalidateGraphics();
        }
        public LayoutFarm.RenderElement ContainerElement
        {
            get { return this.containerElement; }
        }
        protected override CssBox GetElementGlobalLocationImpl(out float globalX, out float globalY)
        {
            Point p = containerElement.GetGlobalLocation();
            globalX = p.X;
            globalY = p.Y;
            return this;
        }
    }



}
