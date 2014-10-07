//BSD 2014, WinterDev
using System;
namespace LayoutFarm.Drawing
{
     
    public abstract class FontInfo
    {
        
        public Font Font { get; protected set; }
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
        public abstract FontSignature GetFontSignature();
    }
}