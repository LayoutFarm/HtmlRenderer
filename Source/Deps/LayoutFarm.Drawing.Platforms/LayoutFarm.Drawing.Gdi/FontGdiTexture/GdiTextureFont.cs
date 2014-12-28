//MIT 2014, WinterDev   
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using PixelFarm;
using PixelFarm.Agg;
using LayoutFarm;
using LayoutFarm.DrawingGL;
using LayoutFarm.Drawing;

namespace PixelFarm.Agg.Fonts
{
    //platform specific
    public class GdiTextureFont : Font
    {
        System.Drawing.Bitmap textBoardBmp;
        System.Drawing.Graphics gx;
        int textBoardW = 800;
        int textBoardH = 100;
        TextureAtlas textureAtlas;
        int width;
        int height;
        Dictionary<char, LayoutFarm.Drawing.RectangleF> charMap = new Dictionary<char, LayoutFarm.Drawing.RectangleF>();
        //LayoutFarm.Drawing.Bitmap myTextBoardBmp;
        IntPtr hFont;
        LayoutFarm.Drawing.FontInfo fontInfo;
        GLBitmap innerGLbmp;

        public GdiTextureFont(int width, int height, System.Drawing.Font font)
        {
            this.width = width;
            this.height = height;
            this.textureAtlas = new TextureAtlas(width, height);
            this.hFont = font.ToHfont();
            //32 bits bitmap
            textBoardBmp = new System.Drawing.Bitmap(textBoardW, textBoardH, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gx = System.Drawing.Graphics.FromImage(textBoardBmp);
            //draw character map
            //basic eng 

            fontInfo = FontsUtils.GetCachedFont(font);

            char[] chars = new char[255];
            for (int i = 0; i < 255; ++i)
            {
                chars[i] = (char)i;
            }

            //PrepareCharacterMap("ญู".ToCharArray());
            PrepareCharacterMap(chars);
        }
        void PrepareCharacterMap(char[] buffer)
        {
            //-----------------------------------------------------------------------
            IntPtr gxdc = gx.GetHdc();
            int len = buffer.Length;
            //draw each character
            int curX = 0;
            int curY = 0;

            //1. clear with white color, 
            MyWin32.PatBlt(gxdc, 0, 0, width, height, MyWin32.WHITENESS);
            //2. transparent background
            MyWin32.SetBkMode(gxdc, MyWin32._SetBkMode_TRANSPARENT);

            //set user font to dc
            MyWin32.SelectObject(gxdc, this.hFont);
            int fontHeight = fontInfo.FontHeight;
            int maxLineHeight = fontHeight;
            for (int i = 0; i < len; ++i)
            {
                //-----------------------------------------------------------------------
                //measure string 
                //and make simple character map
                //----------------------------------------------------------------------- 
                //measure each character ***
                //not adjust kerning*** 

                char c = buffer[i];
                FontABC abcWidth = fontInfo.GetCharABCWidth(c);
                int glyphBoxWidth = Math.Abs(abcWidth.a) + (int)abcWidth.b + abcWidth.c;
                if (abcWidth.Sum + curX > this.width)
                {
                    //start newline
                    curX = 0;
                    curY += maxLineHeight;
                    maxLineHeight = 0;
                }

                NativeTextWin32.TextOut(gxdc, curX, curY, new char[] { c }, 1);
                charMap.Add(c, new LayoutFarm.Drawing.RectangleF(curX, curY, glyphBoxWidth, fontHeight));
                curX += glyphBoxWidth; //move next 

            }

            gx.ReleaseHdc(gxdc);
            //myTextBoardBmp = new Bitmap(width, height, new LazyGdiBitmapBufferProvider(this.textBoardBmp));
            //myTextBoardBmp.InnerImage = GLBitmapTextureHelper.CreateBitmapTexture(this.textBoardBmp);

        }
        public LayoutFarm.Drawing.RectangleF[] GetGlyphPos(char[] buffer, int x, int y)
        {
            if (innerGLbmp == null)
            {
                innerGLbmp = GLBitmapTextureHelper.CreateBitmapTexture(this.textBoardBmp);
            }

            //create reference bmp
            int len = buffer.Length;
            float curX = x;
            float curY = y;
            //create destAndSrcArray
            LayoutFarm.Drawing.RectangleF[] destAndSrcPairs = new LayoutFarm.Drawing.RectangleF[len * 2];

            int pp = 0;
            for (int i = 0; i < len; ++i)
            {
                //find map glyph
                LayoutFarm.Drawing.RectangleF found;
                if (charMap.TryGetValue(buffer[i], out found))
                {
                    //found
                    //canvasGL2d.DrawImage(myTextBoardBmp,
                    //    new RectangleF(curX, curY,
                    //        found.Width, found.Height),
                    //    found);
                    //dest
                    destAndSrcPairs[pp] = new LayoutFarm.Drawing.RectangleF(curX, curY, found.Width, found.Height);
                    //src
                    destAndSrcPairs[pp + 1] = found;
                    curX += found.Width;
                }
                else
                {
                    //draw missing glyph

                }
                pp += 2;
            }
            return destAndSrcPairs;

        }
         
        public GLBitmap BmpBoard
        {
            get { return this.innerGLbmp; }
        }
        protected override void OnDispose()
        {
            if (innerGLbmp != null)
            {
                innerGLbmp.Dispose();
                innerGLbmp = null;
            }
        }
        //-----------
        public override double AscentInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override double CapHeightInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override double DescentInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override int EmSizeInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override FontFace FontFace
        {
            get { throw new NotImplementedException(); }
        }
        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            throw new NotImplementedException();
        }
        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            throw new NotImplementedException();
        }
        public override int GetAdvanceForCharacter(char c)
        {
            throw new NotImplementedException();
        }
        public override int GetAdvanceForCharacter(char c, char next_c)
        {
            throw new NotImplementedException();
        }
        public override double XHeightInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override FontGlyph GetGlyph(char c)
        {
            throw new NotImplementedException();
        }

        public override bool IsAtlasFont
        {
            get { return true; }
        }
    }

}