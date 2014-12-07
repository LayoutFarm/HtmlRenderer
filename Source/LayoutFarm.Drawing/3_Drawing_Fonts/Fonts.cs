using System;

namespace LayoutFarm.Drawing
{
    public abstract class Font : System.IDisposable
    {
        public abstract string Name { get; }
        public abstract float Size { get; }
        public abstract FontStyle Style { get; }
        public abstract object InnerFont { get; }
        public abstract void Dispose();
        public abstract int Height { get; }
        public abstract System.IntPtr ToHfont();
     
    }

    public abstract class FontFamily
    {
       
        public abstract string Name { get; }
    }

    public abstract class StringFormat
    {
        public abstract object InnerFormat { get; }
    }

    //------------------------------------------
    public abstract class FontInfo
    {

       
        public float AscentPx { get; protected set; }
        public float DescentPx { get; protected set; }
        public float BaseLine { get; protected set; }
        public int LineHeight { get; protected set; }
        //--------------------------------------------------
        public int FontHeight { get; protected set; }
        public int FontSize { get; protected set; }
        public abstract IntPtr HFont { get; }
        public abstract int GetCharWidth(char c);
        public abstract int GetStringWidth(char[] buffer);
        public abstract int GetStringWidth(char[] buffer, int length);

        public abstract Font ResolvedFont { get; }

    }
    public interface IFonts
    {
        FontInfo GetFontInfo(Font f);
        FontInfo GetFontInfo(string fontname, float fsize, FontStyle st);
        float MeasureWhitespace(Font f);

        Size MeasureString(string str, Font font); 
        Size MeasureString(char[] str, int startAt, int len, Font font);
        Size MeasureString(char[] str, int startAt, int len, Font font, float maxWidth, out int charFit, out int charFitWidth);
        void Dispose();
    }

}