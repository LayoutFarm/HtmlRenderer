//MIT, 2014-2016, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//----------------------------------- 

using System;
namespace PixelFarm.Drawing.Fonts
{
    public abstract class FontFace : IDisposable
    {
        public bool HasKerning { get; set; }
        protected abstract void OnDispose();
        public void Dispose()
        {
            OnDispose();
        }
        ~FontFace()
        {
            OnDispose();
        }
    }

    /// <summary>
    /// glyph ABC structure
    /// </summary>
    public struct FontABC
    {
        //see https://msdn.microsoft.com/en-us/library/windows/desktop/dd162454(v=vs.85).aspx
        //The ABC structure contains the width of a character in a TrueType font.
        public int a;
        public uint b;
        public int c;
        public FontABC(int a, uint b, int c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
        public int Sum
        {
            get
            {
                return a + (int)b + c;
            }
        }
    }
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
        public abstract FontABC GetCharABCWidth(char c);
        public abstract int GetStringWidth(char[] buffer);
        public abstract int GetStringWidth(char[] buffer, int length);
        public abstract Font ResolvedFont { get; }
        public object PlatformSpecificFont { get; set; }
    }
}