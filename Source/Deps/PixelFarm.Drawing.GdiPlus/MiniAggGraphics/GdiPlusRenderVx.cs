//MIT, 2016-2017, WinterDev

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
    class WinGdiRenderVxFormattedString : RenderVxFormattedString
    {
        public WinGdiRenderVxFormattedString(string str)
        {
            this.OriginalString = str;
        }
    }
}