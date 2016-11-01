//MIT, 2016, WinterDev

using PixelFarm.Agg;
namespace PixelFarm.Drawing.WinGdi
{
    class WinGdiRenderVx : RenderVx
    {
        internal VertexStoreSnap snap;
        internal System.Drawing.Drawing2D.GraphicsPath path;
        public WinGdiRenderVx(VertexStoreSnap snap)
        {
            this.snap = snap;
        }
    }
}