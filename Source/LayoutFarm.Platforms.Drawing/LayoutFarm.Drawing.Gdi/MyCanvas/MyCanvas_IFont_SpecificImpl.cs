//2014 BSD, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;


namespace LayoutFarm
{
    partial class MyCanvas
    {

        public override LayoutFarm.Drawing.FontInfo GetFontInfo(Font f)
        {
            return FontsUtils.GetCachedFont(f.InnerFont as System.Drawing.Font);
        }
        LayoutFarm.Drawing.FontInfo IFonts.GetFontInfo(string fontname, float fsize, FontStyle st)
        {
            return FontsUtils.GetCachedFont(fontname, fsize, (System.Drawing.FontStyle)st);
        }
        float IFonts.MeasureWhitespace(LayoutFarm.Drawing.Font f)
        {
            return FontsUtils.MeasureWhitespace(this, f);
        }
    }
}