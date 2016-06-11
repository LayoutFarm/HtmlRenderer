//MIT 2014, WinterDev

using System.Text;
using System;
using PixelFarm.Agg.Fonts;
namespace PixelFarm.DrawingGL
{
    class GLTextPrinter
    {
        Font currentFont;
        CanvasGL2d canvas2d;
        ProperGlyph[] properGlyphs = null;
        public GLTextPrinter(CanvasGL2d canvas2d)
        {
            this.canvas2d = canvas2d;
        }
        public Font CurrentFont
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
            if (properGlyphs == null)
            {
                properGlyphs = new ProperGlyph[buffsize];
                currentFont.GetGlyphPos(buffer, 0, buffsize, properGlyphs);
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
                FontGlyph glyph = this.currentFont.GetGlyphByIndex(codepoint);
                //glyph image32 
                //-------------------------------------------------------------
                GLBitmap bmp = new GLBitmap(new LazyAggBitmapBufferProvider(glyph.glyphImage32));
                var left = glyph.exportGlyph.img_horiBearingX;
                this.canvas2d.DrawImage(bmp,
                    (float)(xpos + (left >> 6)),
                    (float)(y + (glyph.exportGlyph.bboxYmin >> 6)));
                int w = (glyph.exportGlyph.advanceX) >> 6;
                xpos += (w);
                bmp.Dispose(); //temp here 
                //-------------------------------------------------------------                
            }
        }
    }
}