//BSD 2014, WinterDev  

using System;



namespace LayoutFarm.Drawing
{
    public interface IFonts
    {   
        FontInfo GetFontInfo(Font f);
        FontInfo GetFontInfo(string fontname, float fsize, FontStyle st);
        float MeasureWhitespace(Font f); 
    }

}