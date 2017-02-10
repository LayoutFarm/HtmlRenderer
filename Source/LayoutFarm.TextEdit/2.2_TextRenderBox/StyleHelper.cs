//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    static class StyleHelper
    {
        public static TextSpanStyle CreateNewStyle(Color color)
        {
            if (color != Color.Empty)
            {
                TextSpanStyle simpleBeh = new TextSpanStyle();
                // simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
                return simpleBeh;
            }
            else
            {
                TextSpanStyle simpleBeh = new TextSpanStyle();
                //simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
                return simpleBeh;
            }
        }
    }
}