//MIT, 2014-2017, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------
using System;
using System.Collections.Generic;
using PixelFarm.Drawing.Fonts;
using Typography.TextLayout;

namespace PixelFarm.Drawing.Text
{

    public sealed class HarfBuzzShapingService : TextShapingService
    {
        Dictionary<string, FontFace> nativeFontFaces = new Dictionary<string, FontFace>();
        Dictionary<FontKey, NativeFont> specificFontSize = new Dictionary<FontKey, NativeFont>();

        protected override void GetGlyphPosImpl(ActualFont actualFont, char[] buffer,
            int startAt, int len,
            List<GlyphPlan> glyphPlans)
        {
            NativeFont nativeFont = actualFont as NativeFont;
            if (nativeFont == null)
            {
                nativeFont = ResolveForNativeFont(actualFont);
            }

            unsafe
            {
                //TODO: review proper array size here
                int lim = len * 2;
                ProperGlyph* properGlyphArray = stackalloc ProperGlyph[lim];
                fixed (char* head = &buffer[0])
                {
                    //we use font shaping engine here
                    NativeMyFontsLib.MyFtShaping(
                       nativeFont.NativeFontFace.HBFont,
                        head,
                        buffer.Length,
                        properGlyphArray);
                }
                //copy from proper glyph to 
                //create glyph plan

                for (int i = 0; i < lim; ++i)
                {
                    ProperGlyph propGlyph = properGlyphArray[i];
                    if (propGlyph.codepoint == 0)
                    {
                        //finish , just return
                        return;
                    } 
                    glyphPlans.Add(new GlyphPlan((ushort)propGlyph.codepoint, 0, 0,(ushort) propGlyph.x_advance));
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
            nativeFont = (NativeFont)nativeFontFace.GetFontAtPointSize(actualFont.SizeInPoints);
            specificFontSize.Add(key, nativeFont);
            return nativeFont;
        }
    }
}