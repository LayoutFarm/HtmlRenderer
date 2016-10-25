//MIT 2014-2016, WinterDev
using PixelFarm.Drawing.Fonts;

namespace PixelFarm.DrawingGL
{
    public abstract class PlatformFontPrinterBase
    {
        public abstract void DrawString(CanvasGL2d canvas, string text, float x, float y);
    }

    class GLTextPrinter
    {

        Drawing.RequestFont currentFont;
        CanvasGL2d canvas2d;
        ProperGlyph[] properGlyphs = null;
        NativeFontStore nativeFontStore = new NativeFontStore();
        public GLTextPrinter(CanvasGL2d canvas2d)
        {
            this.canvas2d = canvas2d;
        }
        public Drawing.RequestFont CurrentFont
        {
            get { return this.currentFont; }
            set { this.currentFont = value; }
        }
        public void Print(string t, double x, double y)
        {
            Print(t.ToCharArray(), x, y);
        }
        public void Print(char[] buffer, double x, double y)
        {
            int j = buffer.Length;
            int buffsize = j * 2;
            //get kerning list
            ActualFont fontImp = nativeFontStore.GetResolvedNativeFont(currentFont);

            if (properGlyphs == null)
            {
                properGlyphs = new ProperGlyph[buffsize];
                fontImp.GetGlyphPos(buffer, 0, buffsize, properGlyphs);
            }

            double xpos = x;
            for (int i = 0; i < buffsize; ++i)
            {
                uint codepoint = properGlyphs[i].codepoint;
                if (codepoint == 0)
                {
                    break;
                }

                //-------------------------------------------------------------
                FontGlyph glyph = fontImp.GetGlyphByIndex(codepoint);
                //glyph image32 
                //-------------------------------------------------------------
                GLBitmap bmp = new GLBitmap(new LazyAggBitmapBufferProvider(glyph.glyphImage32));
                var left = glyph.glyphMatrix.img_horiBearingX;
                this.canvas2d.DrawImage(bmp,
                    (float)(xpos + (left >> 6)),
                    (float)(y + (glyph.glyphMatrix.bboxYmin >> 6)));
                int w = (glyph.glyphMatrix.advanceX) >> 6;
                xpos += (w);
                bmp.Dispose(); //temp here 
                //-------------------------------------------------------------                
            }
        }

        public void xPrint(char[] buffer, double x, double y)
        {
            //int j = buffer.Length;
            //int size = j * 2;
            ////get kerning list
            //if (properGlyphs == null)
            //{
            //    properGlyphs = new ProperGlyph[size];
            //    currentFont.GetGlyphPos(buffer, 0, size, properGlyphs);
            //}

            //double xpos = x;
            //for (int i = 0; i < j; ++i)
            //{
            //    char c = buffer[i];
            //    switch (c)
            //    {
            //        case ' ':
            //            {
            //            } break;
            //        case '\r':
            //            {
            //            } break;
            //        case '\n':
            //            {
            //            } break;
            //        default:
            //            {
            //                FontGlyph glyph = this.currentFont.GetGlyphByIndex(properGlyphs[i].codepoint);
            //                //FontGlyph glyph = this.currentFontFace.GetGlyph(c);
            //                //GLBitmapTexture bmp = new GLBitmapTexture();

            //                var left = glyph.exportGlyph.img_horiBearingX;

            //                GLBitmapTexture glbmp = new GLBitmapTexture();
            //                this.canvas2d.DrawImageInvert(glbmp,
            //                    (float)(xpos + (left >> 6)),
            //                    (float)(y + (glyph.exportGlyph.bboxYmin >> 6)));

            //                int w = (glyph.exportGlyph.advanceX) >> 6;
            //                xpos += (w);
            //            } break;
            //    }
            //}
        }
    }
}