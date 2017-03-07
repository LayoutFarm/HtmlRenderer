//MIT, 2016-2017, WinterDev

using System;
using PixelFarm.Agg;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

using Typography.TextLayout;

using System.Collections.Generic;


namespace PixelFarm.DrawingGL
{

    //this provides 3 ITextPrinter for GLES2-based Canvas

    public class AggFontPrinter : ITextPrinter
    {
        ActualImage actualImage;
        ImageGraphics2D imgGfx2d;
        AggCanvasPainter aggPainter;
        VxsTextPrinter textPrinter;
        int bmpWidth;
        int bmpHeight;
        CanvasGL2d canvas;
        GLCanvasPainter canvasPainter;

        public AggFontPrinter(GLCanvasPainter canvasPainter, int w, int h)
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
        public void DrawString(char[] text, int startAt, int len, double x, double y)
        {
            aggPainter.Clear(Drawing.Color.Transparent);
            //draw text 
            textPrinter.DrawString(text, startAt, len, 0, 0);

            byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(actualImage);
            //------------------------------------------------------
            GLBitmap glBmp = new GLBitmap(bmpWidth, bmpHeight, buffer, true);
            glBmp.IsInvert = false;
            canvas.DrawImage(glBmp, (float)x, (float)y + 40);

            //bool isYFliped = canvas.FlipY;
            //if (isYFliped)
            //{

            //}
            //else
            //{
            //    canvas.FlipY = true;
            //    canvas.DrawImage(glBmp, (float)x, (float)y);
            //    canvas.FlipY = false;
            //}

            glBmp.Dispose();
        }
        public void DrawString(string text, double x, double y)
        {
            DrawString(text.ToCharArray(), 0, text.Length, x, y);
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


    /// <summary>
    /// this use win gdi only
    /// </summary>
    public class WinGdiFontPrinter : ITextPrinter, IDisposable
    {

        int _width;
        int _height;
        Win32.NativeWin32MemoryDc memdc;
        IntPtr hfont;
        int bmpWidth = 200;
        int bmpHeight = 50;
        CanvasGL2d canvas;
        public WinGdiFontPrinter(CanvasGL2d canvas, int w, int h)
        {
            this.canvas = canvas;
            _width = w;
            _height = h;
            bmpWidth = w;
            bmpHeight = h;

            memdc = new Win32.NativeWin32MemoryDc(bmpWidth, bmpHeight);
            //TODO: review here
            //use default font from current platform
            InitFont("tahoma", 14);
            memdc.SetTextColor(0);
        }
        public void ChangeFont(RequestFont font)
        {

        }
        public void ChangeFontColor(Color fontColor)
        {

        }
        public void Dispose()
        {
            //TODO: review here 
            Win32.MyWin32.DeleteObject(hfont);
            hfont = IntPtr.Zero;
            memdc.Dispose();
        }
        void InitFont(string fontName, int emHeight)
        {
            Win32.MyWin32.LOGFONT logFont = new Win32.MyWin32.LOGFONT();
            Win32.MyWin32.SetFontName(ref logFont, fontName);
            logFont.lfHeight = emHeight;
            logFont.lfCharSet = 1;//default
            logFont.lfQuality = 0;//default
            hfont = Win32.MyWin32.CreateFontIndirect(ref logFont);
            Win32.MyWin32.SelectObject(memdc.DC, hfont);
        }
        public void DrawString(char[] textBuffer, int startAt, int len, double x, double y)
        {
            //TODO: review performan 
            Win32.MyWin32.PatBlt(memdc.DC, 0, 0, bmpWidth, bmpHeight, Win32.MyWin32.WHITENESS);
            Win32.NativeTextWin32.TextOut(memdc.DC, 0, 0, textBuffer, textBuffer.Length);
            // Win32.Win32Utils.BitBlt(hdc, 0, 0, bmpWidth, 50, memHdc, 0, 0, Win32.MyWin32.SRCCOPY);
            //---------------
            int stride = 4 * ((bmpWidth * 32 + 31) / 32);

            //Bitmap newBmp = new Bitmap(bmpWidth, 50, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //var bmpData = newBmp.LockBits(new Rectangle(0, 0, bmpWidth, 50), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] tmp1 = new byte[stride * 50];
            System.Runtime.InteropServices.Marshal.Copy(memdc.PPVBits, tmp1, 0, tmp1.Length);
            //---------------
            int pos = 3;
            for (int r = 0; r < 50; ++r)
            {
                for (int c = 0; c < stride; ++c)
                {
                    tmp1[pos] = 255;
                    pos += 4;
                    c += 4;
                }
            }

            Win32.NativeTextWin32.WIN32SIZE win32Size;
            unsafe
            {
                fixed (char* bufferHead = &textBuffer[0])
                {
                    Win32.NativeTextWin32.GetTextExtentPoint32Char(memdc.DC, bufferHead, textBuffer.Length, out win32Size);
                }
            }
            bmpWidth = win32Size.Width;
            bmpHeight = win32Size.Height;

            var actualImg = new Agg.ActualImage(bmpWidth, bmpHeight, Agg.PixelFormat.ARGB32);
            //------------------------------------------------------
            //copy bmp from specific bmp area 
            //and convert to GLBmp   
            byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(actualImg);
            unsafe
            {
                byte* header = (byte*)memdc.PPVBits;
                fixed (byte* dest0 = &buffer[0])
                {
                    byte* dest = dest0;
                    byte* rowHead = header;
                    int rowLen = bmpWidth * 4;
                    for (int h = 0; h < bmpHeight; ++h)
                    {

                        header = rowHead;
                        for (int n = 0; n < rowLen;)
                        {
                            //move next
                            *(dest + 0) = *(header + 0);
                            *(dest + 1) = *(header + 1);
                            *(dest + 2) = *(header + 2);
                            //*(dest + 3) = *(header + 3);
                            *(dest + 3) = 255;
                            header += 4;
                            dest += 4;
                            n += 4;
                        }
                        //finish one row
                        rowHead += stride;
                    }
                }
            }

            //------------------------------------------------------
            GLBitmap glBmp = new GLBitmap(bmpWidth, bmpHeight, buffer, false);
            canvas.DrawImage(glBmp, (float)x, (float)y);
            glBmp.Dispose();

        }

        public void DrawString(string text, double x, double y)
        {
            DrawString(text.ToCharArray(), 0, text.Length, x, y);
        }
    }


    public class NativeFontStore
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
                createdFont = fontFace.GetFontAtPointsSize(fontSizeInPoints);
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



    public class GLBmpGlyphTextPrinter : ITextPrinter
    {
        GlyphLayout _glyphLayout = new GlyphLayout();
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        SimpleFontAtlas simpleFontAtlas;
        IFontLoader _fontLoader;
        FontFace ff;
        RequestFont font;
        NativeFontStore nativeFontStore = new NativeFontStore();


        public GLBmpGlyphTextPrinter(GLCanvasPainter painter, IFontLoader fontLoader)
        {
            //create text printer for use with canvas painter
            this.painter = painter;
            this.canvas2d = painter.Canvas;
            _fontLoader = fontLoader;
            //------
            ChangeFont(painter.CurrentFont);
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
            //TODO: each font should be loaded once ...
            //resolve
            string fontfile = _fontLoader.GetFont(font.Name, InstalledFontStyle.Regular).FontPath;
            ff = TextureFontLoader.LoadFont(fontfile, ScriptLangs.Latin, WriteDirection.LTR, out simpleFontAtlas);
            //resolve typeface**
            ActualFont fontImp = ff.GetFontAtPointsSize(font.SizeInPoints);
            _typeface = (Typography.OpenFont.Typeface)ff.GetInternalTypeface();

            float srcTextureScale = _typeface.CalculateFromPointToPixelScale(14);
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
            //int buffsize = j * 2;

            //resolve font from painter?  
            glyphPlans.Clear();
            _glyphLayout.Layout(_typeface, font.SizeInPoints, buffer, startAt, len, glyphPlans);

            //
            //un-test version
            //ActualFont fontImp = nativeFontStore.GetResolvedNativeFont(painter.CurrentFont);
            //if (properGlyphs == null)
            //{
            //    properGlyphs = new ProperGlyph[buffsize];
            //    TextShapingService.GetGlyphPos(fontImp, buffer, 0, buffsize, properGlyphs);
            //} 
            //TODO: implement msdf texture  
            //double xpos = x;
            //for (int i = 0; i < buffsize; ++i)
            //{
            //    uint codepoint = properGlyphs[i].codepoint;
            //    if (codepoint == 0)
            //    {
            //        break;
            //    }

            //    //-------------------------------------------------------------
            //    FontGlyph glyph = fontImp.GetGlyphByIndex(codepoint);
            //    //glyph image32 
            //    //-------------------------------------------------------------
            //    GLBitmap bmp = new GLBitmap(new LazyAggBitmapBufferProvider(glyph.glyphImage32));
            //    var left = glyph.glyphMatrix.img_horiBearingX;
            //    this.canvas2d.DrawImage(bmp,
            //        (float)(xpos + (left >> 6)),
            //        (float)(y + (glyph.glyphMatrix.bboxYmin >> 6)));
            //    int w = (glyph.glyphMatrix.advanceX) >> 6;
            //    xpos += (w);
            //    bmp.Dispose(); //temp here 
            //    //-------------------------------------------------------------                
            //}
            //-------------------------------------
            //msdf texture version

            int n = glyphPlans.Count;

            Typography.Rendering.GlyphImage glyphImage = simpleFontAtlas.TotalGlyph;
            using (GLBitmap glBmp = new GLBitmap(glyphImage.Width, glyphImage.Height, glyphImage.GetImageBuffer(), false))
            {
                glBmp.IsInvert = false;

                float c_x = (float)x;
                //int left = ((int)(glyph.glyphMatrix.img_horiBearingX * scale) >> 6);
                int x_adjust = 0; //minor offset
                                  //float baseline = c_y - 24;//eg line height= 24 //create a list
                                  //TODO: review here
                float baseline = (float)y;//eg line height= 24 //create a list
                                          //bool isFlipY = canvas2d.FlipY;
                                          //if (!isFlipY)
                                          //{
                                          //    canvas2d.FlipY = true;
                                          //}
                float scaleFromTexture = _finalTextureScale;

                for (int i = 0; i < n; ++i)
                {
                    GlyphPlan glyph = glyphPlans[i];
                    Typography.Rendering.TextureFontGlyphData glyphData;
                    if (!simpleFontAtlas.GetRectByCodePoint(glyph.glyphIndex, out glyphData))
                    {
                        //
                        //Rectangle r = glyphData.Rect;
                        //float x_min = glyphData.BBoxXMin / 64;
                        ////draw each glyph at specific position                          
                        ////_canvas.DrawSubImageWithMsdf(glBmp, ref r, c_x + x_min, (float)(baseline + r.Height));
                        //_canvas.DrawSubImageWithMsdf(glBmp, ref r, c_x + x_min, (float)(baseline + r.Height)); 
                        ////c_x += r.Width - 10;
                        //c_x += (glyphData.AdvanceX / 64);
                        continue;
                    }
                    //found

                    PixelFarm.Drawing.Rectangle srcRect = ConvToRect(glyphData.Rect);
                    //test draw full msdf gen img
                    //canvas2d.DrawImage(glBmp, c_x + left, (float)(baseline + ((int)(glyphData.ImgHeight))));

                    //canvas2d.DrawSubImageWithMsdf(glBmp, ref r, c_x + left,
                    //    (float)(baseline + ((int)(glyphData.ImgHeight))), 1.0f);
                    //

                    canvas2d.DrawSubImageWithMsdf(glBmp,
                        ref srcRect, c_x + x_adjust,
                        (float)(baseline + ((int)(srcRect.Height))), scaleFromTexture);

                    c_x += (glyph.advX * scaleFromTexture);
                }

                //canvas2d.FlipY = isFlipY;
                //glBmp.Dispose();
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
        public void DrawString(string t, double x, double y)
        {
            DrawString(t.ToCharArray(), 0, t.Length, x, y);
        }

        static PixelFarm.Drawing.Rectangle ConvToRect(Typography.Rendering.Rectangle r)
        {
            return Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
        }
    }

}