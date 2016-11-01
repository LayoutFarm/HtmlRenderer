//BSD, 2014-2016, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing.Text;
namespace PixelFarm.Drawing.Fonts
{

    class FontSystem  
    {
        NativeFontStore fontStore = new NativeFontStore();
        static FontSystem()
        {

        }
        public void Dispose()
        {
        }
        public ActualFont ResolveActualFont(RequestFont f)
        {
            return fontStore.GetResolvedNativeFont(f);
        }
        public ActualFont GetActualFont(RequestFont f)
        {
            return fontStore.GetResolvedNativeFont(f);
        }
        public Size MeasureString(char[] str, int startAt, int len, RequestFont font)
        {
            //measure in horizontal alignment ***
            //use native method to measure string
            ProperGlyph[] properGlyphs = new ProperGlyph[len * 2];
            ActualFont fontImpl = fontStore.GetResolvedNativeFont(font);
            TextShapingService.GetGlyphPos(fontImpl, str, startAt, len, properGlyphs);

            int j = properGlyphs.Length;
            float total_width = 0;
            float total_height = 0;
            for (int i = 0; i < j; ++i)
            {
                ProperGlyph glyph = properGlyphs[i];
                if (glyph.codepoint == 0)
                {
                    break;
                }
                FontGlyph g2 = fontImpl.GetGlyphByIndex(glyph.codepoint);
                total_width += glyph.x_advance; //we use x_advance from shaping engine ***
                if (total_height < g2.glyphMatrix.img_height)
                {
                    total_height = g2.glyphMatrix.img_height;
                }
            }
            return new Size((int)total_width, (int)total_height);

        }
        public Size MeasureString(char[] str, int startAt, int len, RequestFont font, float maxWidth, out int charFit, out int charFitWidth)
        {
            ProperGlyph[] properGlyphs = new ProperGlyph[len * 2];
            ActualFont fontImpl = fontStore.GetResolvedNativeFont(font);

            TextShapingService.GetGlyphPos(fontImpl, str, startAt, len, properGlyphs);
            int j = properGlyphs.Length;
            float total_width = 0;
            float total_height = 0;
            for (int i = 0; i < j; ++i)
            {
                ProperGlyph glyph = properGlyphs[i];
                if (glyph.codepoint == 0)
                {
                    break;
                }

                FontGlyph g2 = fontImpl.GetGlyphByIndex(glyph.codepoint);
                //TODO: review here
                //char may not map 1:1 to glyphindex 
                if (total_width + glyph.x_advance >= maxWidth)
                {
                    //just stop here
                    charFitWidth = (int)total_width;
                    charFit = i;
                    break;
                }
                total_width += glyph.x_advance; //we use x_advance from shaping engine ***
                if (total_height < g2.glyphMatrix.img_height)
                {
                    total_height = g2.glyphMatrix.img_height;
                }
            }
            charFitWidth = (int)total_width;
            charFit = j - 1;
            return new Size((int)total_width, (int)total_height);
        }
        public float MeasureWhitespace(RequestFont f)
        {
            ActualFont fontImpl = fontStore.GetResolvedNativeFont(f);
            FontGlyph whitespaceGlyph = fontImpl.GetGlyph(' ');
            return whitespaceGlyph.horiz_adv_x;
        }
    }
}