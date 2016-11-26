//MIT, 2016, WinterDev

using PixelFarm.Agg;
namespace PixelFarm.Drawing.Skia
{
    class WinGdiRenderVx : RenderVx
    {
        internal VertexStoreSnap snap;
        internal SkiaSharp.SKPath path;
        public WinGdiRenderVx(VertexStoreSnap snap)
        {
            this.snap = snap;
        }
    }
}