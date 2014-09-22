//BSD 2014, WinterDev  

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LayoutFarm.Drawing
{
    public interface IFonts
    {   
        FontInfo GetFontInfo(Font f);
        FontInfo GetFontInfo(string fontname, float fsize, FontStyle st);
        float MeasureWhitespace(Font f); 
    }

}