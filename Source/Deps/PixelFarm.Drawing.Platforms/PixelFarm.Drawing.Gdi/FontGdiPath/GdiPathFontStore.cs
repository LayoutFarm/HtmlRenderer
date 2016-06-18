// 2015,2014 ,MIT, WinterDev 


using System.Collections.Generic;
namespace PixelFarm.Agg.Fonts
{
    public static class GdiPathFontStore
    {
        static Dictionary<string, GdiPathFontFace> fontFaces = new Dictionary<string, GdiPathFontFace>();
        public static Font LoadFont(string filename, int fontPointSize)
        {
            //load font from specific file 
            GdiPathFontFace fontFace;
            if (!fontFaces.TryGetValue(filename, out fontFace))
            {
                //create new font face               
                fontFace = new GdiPathFontFace(filename);
                fontFaces.Add(filename, fontFace);
            }
            if (fontFace == null)
            {
                return null;
            }
            return fontFace.GetFontAtSpecificSize(fontPointSize);
        }


        //---------------------------------------------------
        //helper function
        public static int ConvertFromPointUnitToPixelUnit(float point)
        {
            //from FreeType Documenetation
            //pixel_size = (pointsize * (resolution/72);
            return (int)(point * 96 / 72);
        }
    }
}