//BSD, 2014-2017, WinterDev 

namespace PixelFarm.Drawing
{
    public interface IRootGraphics { }
    public class CssBoxRootGfxBridge : IRootGraphics
    {
        LayoutFarm.RootGraphic rootgfx;
        public CssBoxRootGfxBridge(LayoutFarm.RootGraphic rootgfx)
        {
            this.rootgfx = rootgfx;
        }
        public LayoutFarm.RootGraphic RootGfx { get { return rootgfx; } }
    }
}
