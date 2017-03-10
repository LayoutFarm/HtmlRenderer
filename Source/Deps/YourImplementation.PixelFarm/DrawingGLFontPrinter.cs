//MIT, 2016-2017, WinterDev

using System;
using System.Collections.Generic;
//
using PixelFarm.Agg;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
using Typography.TextLayout;


namespace PixelFarm.DrawingGL
{

    public class AggTextSpanPrinter : ITextPrinter
    {
        ActualImage actualImage;
        ImageGraphics2D imgGfx2d;
        AggCanvasPainter aggPainter;
        VxsTextPrinter textPrinter;
        int bmpWidth;
        int bmpHeight;
        CanvasGL2d canvas;
        GLCanvasPainter canvasPainter;

        public AggTextSpanPrinter(GLCanvasPainter canvasPainter, int w, int h)
        {
            //this class print long text into agg canvas
            //then copy pixel buffer from aff canvas to gl-bmp
            //then draw the  gl-bmp into target gl canvas


            //TODO: review here
            this.canvasPainter = canvasPainter;
            this.canvas = canvasPainter.Canvas;
            bmpWidth = w;
            bmpHeight = h;
            actualImage = new ActualImage(bmpWidth, bmpHeight, PixelFormat.ARGB32);

            imgGfx2d = new ImageGraphics2D(actualImage);
            aggPainter = new AggCanvasPainter(imgGfx2d);
            aggPainter.FillColor = Color.Black;
            aggPainter.StrokeColor = Color.Black;

            //set default1
            aggPainter.CurrentFont = canvasPainter.CurrentFont;

            textPrinter = new VxsTextPrinter(aggPainter, YourImplementation.BootStrapOpenGLES2.myFontLoader);
            aggPainter.TextPrinter = textPrinter;
        }
        public Typography.Rendering.HintTechnique HintTechnique
        {
            get { return textPrinter.HintTechnique; }
            set { textPrinter.HintTechnique = value; }
        }
        public bool UseSubPixelRendering
        {
            get { return aggPainter.UseSubPixelRendering; }
            set
            {
                aggPainter.UseSubPixelRendering = value;
            }
        }

        public void DrawString(char[] text, int startAt, int len, double x, double y)
        {

            //1. clear prev drawing result
            aggPainter.Clear(Drawing.Color.Transparent);
            //2. print text span into Agg Canvas
            textPrinter.DrawString(text, startAt, len, 0, 0);
            //3.copy to gl bitmap
            byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(actualImage);
            //------------------------------------------------------
            GLBitmap glBmp = new GLBitmap(bmpWidth, bmpHeight, buffer, true);
            glBmp.IsInvert = false;
            //TODO: review font height
            canvas.DrawImage(glBmp, (float)x, (float)y + 40);
            glBmp.Dispose();
        }

        public void ChangeFont(RequestFont font)
        {
            aggPainter.CurrentFont = font;
        }
        public void ChangeFontColor(Color fontColor)
        {
            aggPainter.FillColor = Color.Black;
        }
    }




    public class GLBmpGlyphTextPrinter : ITextPrinter
    {
        GlyphLayout _glyphLayout = new GlyphLayout();
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        SimpleFontAtlas simpleFontAtlas;
        IFontLoader _fontLoader;
        RequestFont font;

        public GLBmpGlyphTextPrinter(GLCanvasPainter painter, IFontLoader fontLoader)
        {
            //create text printer for use with canvas painter
            this.painter = painter;
            this.canvas2d = painter.Canvas;
            _fontLoader = fontLoader;
            //------
            ChangeFont(painter.CurrentFont);
            this._glyphLayout.ScriptLang = painter.CurrentFont.GetOpenFontScriptLang();

        }
        public void ChangeFontColor(Color color)
        {
            //called by owner painter  

        }
        public void ChangeFont(RequestFont font)
        {
            //from request font
            //we resolve it to actual font

            this.font = font;
            this._glyphLayout.ScriptLang = font.GetOpenFontScriptLang();
            ActualFont fontImp = ActiveFontAtlasService.GetTextureFontAtlasOrCreateNew(_fontLoader, font, out simpleFontAtlas);
            _typeface = (Typography.OpenFont.Typeface)fontImp.FontFace.GetInternalTypeface();
            float srcTextureScale = _typeface.CalculateFromPointToPixelScale(simpleFontAtlas.OriginalFontSizePts);
            //scale at request
            float targetTextureScale = _typeface.CalculateFromPointToPixelScale(font.SizeInPoints);
            _finalTextureScale = targetTextureScale / srcTextureScale;
        }

        //-----------
        List<GlyphPlan> glyphPlans = new List<GlyphPlan>();
        Typography.OpenFont.Typeface _typeface;
        float _finalTextureScale = 1;
        //-----------
        public void DrawString(char[] buffer, int startAt, int len, double x, double y)
        {
            int j = buffer.Length;

            //resolve font from painter?  
            glyphPlans.Clear();
            _glyphLayout.Layout(_typeface, font.SizeInPoints, buffer, startAt, len, glyphPlans);


            int n = glyphPlans.Count;

            Typography.Rendering.GlyphImage totoalGlyphImg = simpleFontAtlas.TotalGlyph;

            //PERF:
            //TODO: review here, can we cache the glbmp for later use
            //not to create it every time
            using (GLBitmap glBmp = new GLBitmap(totoalGlyphImg.Width, totoalGlyphImg.Height, totoalGlyphImg.GetImageBuffer(), false))
            {
                glBmp.IsInvert = false;

                float scaleFromTexture = _finalTextureScale;

                Typography.Rendering.TextureKind textureKind = simpleFontAtlas.TextureKind;

                for (int i = 0; i < n; ++i)
                {
                    GlyphPlan glyph = glyphPlans[i];
                    Typography.Rendering.TextureFontGlyphData glyphData;
                    if (!simpleFontAtlas.TryGetGlyphDataByCodePoint(glyph.glyphIndex, out glyphData))
                    {
                        continue;
                    }
                    PixelFarm.Drawing.Rectangle srcRect = ConvToRect(glyphData.Rect);

                    switch (textureKind)
                    {
                        case Typography.Rendering.TextureKind.Msdf:
                            {
                                canvas2d.DrawSubImageWithMsdf(glBmp,
                                    ref srcRect,
                                    (float)(x + (glyph.x - glyphData.TextureXOffset) * scaleFromTexture), // -glyphData.TextureXOffset => restore to original pos
                                    (float)(y + (glyph.y - glyphData.TextureYOffset + srcRect.Height) * scaleFromTexture),// -glyphData.TextureYOffset => restore to original pos
                                    scaleFromTexture);
                            }
                            break;
                        case Typography.Rendering.TextureKind.AggGrayScale:
                            {
                                canvas2d.DrawSubImage(glBmp,
                                  ref srcRect,
                                  (float)(x + (glyph.x - glyphData.TextureXOffset) * scaleFromTexture), // -glyphData.TextureXOffset => restore to original pos
                                  (float)(y + (glyph.y - glyphData.TextureYOffset + srcRect.Height) * scaleFromTexture),// -glyphData.TextureYOffset => restore to original pos
                                  scaleFromTexture);

                            }
                            break;
                        case Typography.Rendering.TextureKind.AggSubPixel:
                            throw new NotSupportedException();
                    }
                }
            }
            //temp here 
            //draw with texture printer ***
            //char[] chars = text.ToCharArray();
            //int j = chars.Length;
            //int buffsize = j * 2;
            ////get kerning list 

            ////get actual font for this canvas 
            //TextureFont currentFont = _currentTextureFont;
            //SimpleFontAtlas fontAtlas = currentFont.FontAtlas;
            //ProperGlyph[] properGlyphs = new ProperGlyph[buffsize];
            //TextShapingService.GetGlyphPos(currentFont, chars, 0, buffsize, properGlyphs);
            //GLBitmap glBmp = (GLBitmap)currentFont.GLBmp;
            //if (glBmp == null)
            //{
            //    //create glbmp
            //    GlyphImage glyphImage = fontAtlas.TotalGlyph;
            //    int[] buffer = glyphImage.GetImageBuffer();
            //    glBmp = new GLBitmap(glyphImage.Width, glyphImage.Height, buffer, false);
            //}
            ////int j = chars.Length;
            ////
            //float c_x = (float)x;
            //float c_y = (float)y;

            ////TODO: review here ***
            ////-----------------
            ////1. layout each glyph before render *** 
            ////
            //float baseline = c_y - 24;//eg line height= 24 //create a list

            ////--------------
            //List<float> coords = new List<float>();
            //float scale = 1f;
            //for (int i = 0; i < buffsize; ++i)
            //{
            //    ProperGlyph glyph1 = properGlyphs[i];
            //    uint codepoint = properGlyphs[i].codepoint;
            //    if (codepoint == 0)
            //    {
            //        break;
            //    }
            //    //--------------------------------
            //    //if (codepoint == 1173 && i > 1)
            //    //{
            //    //    //check prev code point 
            //    //    codepoint = 1168;
            //    //}
            //    //--------------------------------
            //    TextureFontGlyphData glyphData;
            //    if (!fontAtlas.GetRectByCodePoint((int)codepoint, out glyphData))
            //    {
            //        //Rectangle r = glyphData.Rect;
            //        //float x_min = glyphData.BBoxXMin / 64;
            //        ////draw each glyph at specific position                          
            //        ////_canvas.DrawSubImageWithMsdf(glBmp, ref r, c_x + x_min, (float)(baseline + r.Height));
            //        //_canvas.DrawSubImageWithMsdf(glBmp, ref r, c_x + x_min, (float)(baseline + r.Height)); 
            //        ////c_x += r.Width - 10;
            //        //c_x += (glyphData.AdvanceX / 64);
            //        continue;
            //    }



            //    FontGlyph glyph = currentFont.GetGlyphByIndex(codepoint);
            //    int left = ((int)(glyph.glyphMatrix.img_horiBearingX * scale) >> 6);
            //    Rectangle r = glyphData.Rect;
            //    int adjustX = 0;
            //    int bboxYMin = glyph.glyphMatrix.bboxYmin >> 6;
            //    if (bboxYMin > 1 || bboxYMin < -1)
            //    {
            //        //  adjustX = 3;
            //    }
            //    //scale down 0.8; 
            //    //_canvas.DrawSubImageWithMsdf(glBmp, ref r, adjustX + c_x + left,
            //    //    (float)(baseline + ((int)(glyphData.ImgHeight + glyph.glyphMatrix.bboxYmin) >> 6)), 1.1f);

            //    coords.Add(r.Left);
            //    coords.Add(r.Top);
            //    coords.Add(r.Width);
            //    coords.Add(r.Height);
            //    //-------------------------
            //    coords.Add(adjustX + c_x + left);
            //    //coords.Add(baseline + ((int)((glyphData.ImgHeight + glyph.glyphMatrix.bboxYmin) * scale) >> 6));
            //    coords.Add(baseline + ((int)((glyphData.ImgHeight + glyphData.BBoxYMin) * scale) >> 6));
            //    //int w = (int)(glyph.glyphMatrix.advanceX * scale) >> 6;
            //    int w = (int)(glyph.horiz_adv_x * scale) >> 6;
            //    c_x += w;
            //}
            //_canvas.DrawSubImageWithMsdf(glBmp, coords.ToArray(), scale);

            //-----------------------
            //public override void DrawString(string text, double x, double y)
            //{

            //    char[] chars = text.ToCharArray();
            //    int j = chars.Length;
            //    int buffsize = j * 2;
            //    //get kerning list 
            //    TextureFont currentFont = this.CurrentFont as TextureFont;
            //    SimpleFontAtlas fontAtlas = currentFont.FontAtlas;
            //    ProperGlyph[] properGlyphs = new ProperGlyph[buffsize];
            //    currentFont.GetGlyphPos(chars, 0, buffsize, properGlyphs);
            //    GLBitmap glBmp = currentFont.GLBmp;
            //    if (glBmp == null)
            //    {
            //        //create glbmp
            //        GlyphImage glyphImage = fontAtlas.TotalGlyph;
            //        int[] buffer = glyphImage.GetImageBuffer();
            //        glBmp = new GLBitmap(glyphImage.Width, glyphImage.Height, buffer, false);
            //    }
            //    //int j = chars.Length;
            //    //
            //    float c_x = (float)x;
            //    float c_y = (float)y;

            //    //TODO: review here 
            //    //-----------------
            //    //1. layout each glyph before render *** 
            //    float baseline = c_y - 24;//eg line height= 24 
            //                              //create a list

            //    for (int i = 0; i < buffsize; ++i)
            //    {
            //        ProperGlyph glyph1 = properGlyphs[i];
            //        uint codepoint = properGlyphs[i].codepoint;
            //        if (codepoint == 0)
            //        {
            //            break;
            //        }
            //        if (codepoint == 1173 && i > 1)
            //        {
            //            //check prev code point 
            //            codepoint = 1168;
            //        }
            //        TextureFontGlyphData glyphData;
            //        if (!fontAtlas.GetRect((int)codepoint, out glyphData))
            //        {
            //            //Rectangle r = glyphData.Rect;
            //            //float x_min = glyphData.BBoxXMin / 64;
            //            ////draw each glyph at specific position                          
            //            ////_canvas.DrawSubImageWithMsdf(glBmp, ref r, c_x + x_min, (float)(baseline + r.Height));
            //            //_canvas.DrawSubImageWithMsdf(glBmp, ref r, c_x + x_min, (float)(baseline + r.Height)); 
            //            ////c_x += r.Width - 10;
            //            //c_x += (glyphData.AdvanceX / 64);
            //            continue;
            //        }

            //        //-------------------------------------------------------------
            //        //FontGlyph glyph = this.currentFont.GetGlyphByIndex(codepoint);
            //        FontGlyph glyph = currentFont.GetGlyphByIndex(codepoint);
            //        int left = (glyph.glyphMatrix.img_horiBearingX >> 6);
            //        Rectangle r = glyphData.Rect;
            //        int adjustX = 0;
            //        int bboxYMin = glyph.glyphMatrix.bboxYmin >> 6;
            //        if (bboxYMin > 1 || bboxYMin < -1)
            //        {
            //            //  adjustX = 3;
            //        }
            //        //scale down 0.8; 
            //        _canvas.DrawSubImageWithMsdf(glBmp, ref r, adjustX + c_x + left,
            //            (float)(baseline + ((int)(glyphData.ImgHeight + glyph.glyphMatrix.bboxYmin) >> 6)), 1.1f);
            //        int w = (glyph.glyphMatrix.advanceX) >> 6;
            //        c_x += (w);
            //    }
            //}
            //        public override void DrawString(string text, double x, double y)
            //        {
            //            ////in this version we draw string to image
            //            ////and the write the image back to gl surface
            //            //_winGfx.Clear(System.Drawing.Color.White);
            //            //_winGfx.DrawString(text, _winFont, _winGfxBrush, 0, 0);
            //            ////_winGfxBackBmp.Save("d:\\WImageTest\\a00123.png"); 

            //            //System.Drawing.SizeF textAreaSize = _winGfx.MeasureString(text, _winFont);
            //            //var bmpData = _winGfxBackBmp.LockBits(new System.Drawing.Rectangle(0, 0, _winGfxBackBmp.Width, _winGfxBackBmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, _winGfxBackBmp.PixelFormat);
            //            //int width = (int)textAreaSize.Width;
            //            //int height = (int)textAreaSize.Height;

            //            //ActualImage actualImg = new ActualImage(width, height, Agg.Image.PixelFormat.ARGB32);
            //            ////------------------------------------------------------
            //            ////copy bmp from specific bmp area 
            //            ////and convert to GLBmp  
            //            //int stride = bmpData.Stride;
            //            //byte[] buffer = actualImg.GetBuffer();
            //            //unsafe
            //            //{
            //            //    byte* header = (byte*)bmpData.Scan0;
            //            //    fixed (byte* dest0 = &buffer[0])
            //            //    {
            //            //        byte* dest = dest0;
            //            //        byte* rowHead = header;
            //            //        int rowLen = width * 4;
            //            //        for (int h = 0; h < height; ++h)
            //            //        {

            //            //            header = rowHead;
            //            //            for (int n = 0; n < rowLen;)
            //            //            {
            //            //                //move next
            //            //                *(dest + 0) = *(header + 0);
            //            //                *(dest + 1) = *(header + 1);
            //            //                *(dest + 2) = *(header + 2);
            //            //                *(dest + 3) = *(header + 3);
            //            //                header += 4;
            //            //                dest += 4;
            //            //                n += 4;
            //            //            }
            //            //            //finish one row
            //            //            rowHead += stride;
            //            //        }
            //            //    }
            //            //}
            //            //_winGfxBackBmp.UnlockBits(bmpData);
            //            ////------------------------------------------------------
            //            //GLBitmap glBmp = new GLBitmap(width, height, buffer, false);
            //            //_canvas.DrawImageWithWhiteTransparent(glBmp, (float)x, (float)y);
            //            //glBmp.Dispose();
            //        }
        }


        static PixelFarm.Drawing.Rectangle ConvToRect(Typography.Rendering.Rectangle r)
        {
            return Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
        }
    }

}