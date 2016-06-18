//MIT 2014, WinterDev

using PixelFarm.Drawing;
namespace OpenTK.Graphics.ES20
{
    public static class GLHelper
    {
        public static void ClearColor(Color c)
        {
            GL.ClearColor(
                (float)c.R / 255f,
                (float)c.G / 255f,
                (float)c.B / 255f,
                (float)c.A / 255f);
        }

        public static PixelFarm.Drawing.Rectangle ConvToRect(Rectangle openTkRect)
        {
            return new PixelFarm.Drawing.Rectangle(
                openTkRect.X,
                openTkRect.Y,
                openTkRect.Width,
                openTkRect.Height);
        }
    }
}