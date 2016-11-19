//MIT, 2014-2016, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PixelFarm.Drawing.Fonts;

namespace PixelFarm.Drawing.Text
{

    public sealed class HarfBuzzShapingService : TextShapingService
    {
        Dictionary<string, FontFace> nativeFontFaces = new Dictionary<string, FontFace>();
        Dictionary<FontKey, NativeFont> specificFontSize = new Dictionary<FontKey, NativeFont>();

        protected override void GetGlyphPosImpl(ActualFont actualFont, char[] buffer,
            int startAt, int len,
            Fonts.ProperGlyph[] properGlyphs)
        {
            NativeFont nativeFont = actualFont as NativeFont;
            if (nativeFont == null)
            {
                nativeFont = ResolveForNativeFont(actualFont);
            }

            unsafe
            {
                fixed (ProperGlyph* propGlyphH = &properGlyphs[0])
                fixed (char* head = &buffer[0])
                {
                    //we use font shaping engine here
                    NativeMyFontsLib.MyFtShaping(
                       nativeFont.NativeFontFace.HBFont,
                        head,
                        buffer.Length,
                        propGlyphH);
                }
            }

        }
        NativeFont ResolveForNativeFont(ActualFont actualFont)
        {
            NativeFont nativeFont;
            FontFace fontface = actualFont.FontFace;
            FontKey key = new FontKey(fontface.Name, actualFont.SizeInPoints, FontStyle.Regular);
            if (specificFontSize.TryGetValue(key, out nativeFont))
            {
                return nativeFont;
            }
            //-----------------------------
            //not native font
            //if we need to use hardbuzz then 
            //create a native one for use 
            FontFace nativeFontFace;
            if (!nativeFontFaces.TryGetValue(fontface.Name, out nativeFontFace))
            {
                //create new
                nativeFontFace = FreeTypeFontLoader.LoadFont(fontface.FontPath, "en", HBDirection.HB_DIRECTION_LTR);
                nativeFontFaces.Add(fontface.Name, nativeFontFace);
            }

            //check if we have native fontface for this font?                
            nativeFont = (NativeFont)nativeFontFace.GetFontAtPointsSize(actualFont.SizeInPoints);
            specificFontSize.Add(key, nativeFont);
            return nativeFont;
        }
    }
}