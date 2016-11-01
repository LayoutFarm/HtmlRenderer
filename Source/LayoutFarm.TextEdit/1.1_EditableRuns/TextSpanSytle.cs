//Apache2, 2014-2016, WinterDev
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
namespace LayoutFarm.Text
{
    public struct TextSpanStyle
    {
        public Color FontColor
        {
            get;
            set;
        }
        public RequestFont FontInfo;
        public byte ContentHAlign;
        public static readonly TextSpanStyle Empty = new TextSpanStyle();
        public bool IsEmpty()
        {
            return this.FontInfo == null;
        }
    }
}