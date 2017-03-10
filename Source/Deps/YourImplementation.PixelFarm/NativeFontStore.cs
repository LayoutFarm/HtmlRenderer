//MIT, 2016-2017, WinterDev

using System;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
using System.Collections.Generic;


namespace PixelFarm.DrawingGL
{

#if DEBUG
    class NativeFontStore
    {
        //TODO: review here again ***

        Dictionary<InstalledFont, FontFace> fonts = new Dictionary<InstalledFont, FontFace>();
        Dictionary<FontKey, ActualFont> registerFonts = new Dictionary<FontKey, ActualFont>();
        //--------------------------------------------------


        //public override float GetCharWidth(RequestFont f, char c)
        //{
        //    return GLES2PlatformFontMx.Default.ResolveForGdiFont(f).GetGlyph(c).horiz_adv_x >> 6;
        //    //NativeFont font = nativeFontStore.GetResolvedNativeFont(f);
        //    //return font.GetGlyph(c).horiz_adv_x >> 6;
        //} 
        //============================================== 

        public NativeFontStore()
        {

        }
        public ActualFont LoadFont(string fontName, float fontSizeInPoints)
        {
            //find install font from fontname
            InstalledFont found = PixelFarm.Drawing.GLES2.GLES2Platform.GetInstalledFont(fontName, InstalledFontStyle.Regular);
            if (found == null)
            {
                return null;
            }

            FontFace fontFace;
            if (!fonts.TryGetValue(found, out fontFace))
            {
                throw new NotSupportedException("revisit freetype impl");
                //convert to freetype data

                //TODO: review here
                //fontFace = FreeTypeFontLoader.LoadFont(found,
                //    GLES2PlatformFontMx.defaultScriptLang
                //    GLES2PlatformFontMx.defaultHbDirection,
                //    GLES2PlatformFontMx.defaultScriptCode);
                //fontFace = FreeTypeFontLoader.LoadFont(found,
                //     "en",
                //     HBDirection.HB_DIRECTION_RTL);

                if (fontFace == null)
                {
                    throw new NotSupportedException();
                }
                fonts.Add(found, fontFace);//register
            }
            //-----------
            //create font at specific size from this fontface
            FontKey fontKey = new FontKey(fontName, fontSizeInPoints, FontStyle.Regular);
            ActualFont createdFont;
            if (!registerFonts.TryGetValue(fontKey, out createdFont))
            {
                createdFont = fontFace.GetFontAtPointSize(fontSizeInPoints);
            }
            //-----------
            return createdFont;
        }

        public ActualFont GetResolvedNativeFont(RequestFont reqFont)
        {
            ActualFont found;
            registerFonts.TryGetValue(reqFont.FontKey, out found);
            return found;
        }
    }

#endif
}