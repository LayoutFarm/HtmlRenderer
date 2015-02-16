//2014,2015 ,BSD, WinterDev

using System;
namespace LayoutFarm
{
    public static class CssColorConv
    {
        public static PixelFarm.Drawing.Color ConvertToActualColor(LayoutFarm.WebDom.CssColor color)
        {
            return new PixelFarm.Drawing.Color(
                color.A,
                color.R,
                color.G,
                color.B);

        }
        public static LayoutFarm.WebDom.CssColor ConvertToCssColor(PixelFarm.Drawing.Color color)
        {
            return new LayoutFarm.WebDom.CssColor(
                color.A,
                color.R,
                color.G,
                color.B);

        }
    }
}