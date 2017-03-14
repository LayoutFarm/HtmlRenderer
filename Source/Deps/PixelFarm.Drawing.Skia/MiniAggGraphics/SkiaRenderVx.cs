//MIT, 2016-2017, WinterDev

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
    class SkiaRenerVxFormattedString : RenderVxFormattedString
    {
        public SkiaRenerVxFormattedString(string str)
        {
            this.OriginalString = str;
        }
    }
}