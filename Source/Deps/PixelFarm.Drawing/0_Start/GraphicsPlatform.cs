//MIT, 2014-2016, WinterDev
using System.Collections.Generic;
using PixelFarm.Drawing.Fonts;

namespace PixelFarm.Drawing
{
    public abstract class GraphicsPlatform
    {
        public abstract Font GetFont(string fontfaceName, float emsize, FontStyle st);
        public abstract ActualFont GetActualFont(Font f);
        public abstract GraphicsPath CreateGraphicsPath();
        public abstract Canvas CreateCanvas(
            int left,
            int top,
            int width,
            int height);
        public abstract Canvas CreateCanvas(
            object platformCanvas,
            int left,
            int top,
            int width,
            int height
         );
        public abstract IFonts SampleIFonts { get; }
        public Font TextEditFontInfo { get; set; }
        public static string GenericSerifFontName
        {
            get;
            set;
        }
        public abstract Bitmap CreatePlatformBitmap(int w, int h, byte[] rawBuffer, bool isBottomUp);
        
    }
}