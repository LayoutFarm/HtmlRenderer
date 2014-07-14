//BSD 2014, WinterDev
using System.Drawing;
namespace HtmlRenderer.Drawing
{

    public class FontInfo
    {
        Font font;
        public FontInfo(Font f, int lineHeight, int ws)
        {
            this.font = f;
            this.LineHeight = lineHeight; 
        }
        public Font Font
        {
            get { return this.font; }
        }

        public int LineHeight { get; private set; }
        
    }
}