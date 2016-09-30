//MIT, 2014-2016, WinterDev  
using System.Collections.Generic;
namespace PixelFarm.Drawing.Fonts
{
    public class GdiPathFontStore
    {
        Dictionary<string, GdiPathFontFace> fontFaces = new Dictionary<string, GdiPathFontFace>();
        Dictionary<Font, GdiPathFont> registerFonts = new Dictionary<Font, GdiPathFont>();

        public Font LoadFont(string fontName, float fontPointSize)
        {
            //load font from specific file 
            GdiPathFontFace fontFace;
            if (!fontFaces.TryGetValue(fontName, out fontFace))
            {
                //create new font face               
                fontFace = new GdiPathFontFace(fontName);
                fontFaces.Add(fontName, fontFace);
            }
            if (fontFace == null)
            {
                return null;
            }

            Font font = new Drawing.Font(fontName, fontPointSize);
            GdiPathFont gdiPathFont = fontFace.GetFontAtSpecificSize((int)fontPointSize);
            registerFonts.Add(font, gdiPathFont);
            return font;
        } 
        public OutlineFont GetResolvedFont(Font f)
        {
            GdiPathFont found;
            registerFonts.TryGetValue(f, out found);
            return found;
        }
       
    }
}