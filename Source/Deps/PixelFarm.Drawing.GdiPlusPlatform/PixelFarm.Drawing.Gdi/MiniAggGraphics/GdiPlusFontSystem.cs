//2016 MIT, WinterDev

using System;
using System.Collections.Generic;
 
namespace PixelFarm.Drawing.WinGdi
{
    class WinGdiPlusFontSystem
    {
        Font latestFont;
        WinGdiPlusFont latestWinFont;
        Dictionary<Font, WinGdiPlusFont> registerFonts = new Dictionary<Font, WinGdiPlusFont>();
        public WinGdiPlusFont GetWinGdiFont(Font f)
        {
            if (f == null)
            {
                throw new NotSupportedException();
            }
            if (f == latestFont)
            {
                return latestWinFont;
            }
            //-----
            //get register font or create the new one
            WinGdiPlusFont found;
            if (!registerFonts.TryGetValue(f, out found))
            {
                //create the new one and register                  
                found = new WinGdiPlusFont(new System.Drawing.Font(f.Name, f.EmSize));
            }
            latestFont = f;
            return latestWinFont = found;
        }
    }
}