//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;

namespace LayoutFarm.Drawing
{
    class BasicGdi32FontHelper 
    {
        System.Drawing.Bitmap bmp;
        IntPtr hdc;
        bool isInit;
        public BasicGdi32FontHelper()
        {

        }
        ~BasicGdi32FontHelper()
        {
            MyWin32.DeleteDC(hdc);
            if (bmp != null)
            {
                bmp.Dispose();
                bmp = null;
            }
        }
        void Init()
        {

            bmp = new System.Drawing.Bitmap(2, 2);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            hdc = g.GetHdc();
            isInit = true;

        }
        public int[] MeasureCharWidths(IntPtr hFont)
        {
            if (!isInit) Init();


            int[] charWidths;
            MyWin32.SelectObject(hdc, hFont);

            charWidths = new int[256];
            unsafe
            {

                NativeTextWin32.FontABC[] abcSizes = new NativeTextWin32.FontABC[256];
                fixed (NativeTextWin32.FontABC* abc = abcSizes)
                {
                    NativeTextWin32.GetCharABCWidths(hdc, (uint)0, (uint)255, abc);

                }

                for (int i = 0; i < 161; i++)
                {
                    charWidths[i] = abcSizes[i].Sum;

                }
                for (int i = 161; i < 255; i++)
                {
                    charWidths[i] = abcSizes[i].Sum;

                }
            }
            return charWidths;
        }
        public int MeasureStringWidth(IntPtr hFont, char[] buffer)
        {
            if (!isInit) Init();

            MyWin32.SelectObject(this.hdc, hFont);
            NativeTextWin32.WIN32SIZE size;
            NativeTextWin32.GetTextExtentPoint32(hdc, buffer, buffer.Length, out size);
            return size.Width;
        }
        public int MeasureStringWidth(IntPtr hFont, char[] buffer, int length)
        {
            if (!isInit) Init();
            MyWin32.SelectObject(this.hdc, hFont);
            NativeTextWin32.WIN32SIZE size;
            NativeTextWin32.GetTextExtentPoint32(hdc, buffer, length, out size);
            return size.Width;
        }

    }

    class MyTextFontInfo : TextFontInfo
    {

        int[] charWidths;
        Font myFont;
        IntPtr hFont;
        int fontHeight;
        bool disposed;
        BasicGdi32FontHelper gdiFontHelper;

        public MyTextFontInfo(Font font, BasicGdi32FontHelper gdiFontHelper)
        {
            fontHeight = font.Height;
            myFont = font;
            hFont = myFont.ToHfont();
            this.gdiFontHelper = gdiFontHelper;

            charWidths = gdiFontHelper.MeasureCharWidths(hFont);
        }

        public void Dispose()
        {
            myFont.Dispose();
            disposed = true;
        }
        ~MyTextFontInfo()
        {
            Dispose();
        }
        public override int FontHeight
        {
            get
            {
                return fontHeight;
            }
        }
        public override Font Font
        {
            get
            {
                return myFont;
            }

        }
        public override IntPtr HFont
        {
            get
            {
                return hFont;
            }
        }
        public override int GetCharWidth(char c)
        {
            int converted = (int)c;
            if (converted > 160)
            {
                converted -= 3424;
            }
            if (converted < 256 && converted > -1)
            {
                return charWidths[converted];
            }
            else
            {
                return 0;
            }
        }
        public override int GetStringWidth(char[] buffer)
        {
            if (buffer == null)
            {
                return 0;
            }
            else
            {
                return gdiFontHelper.MeasureStringWidth(this.hFont, buffer);

            }
        }
        public override int GetStringWidth(char[] buffer, int length)
        {
            if (buffer == null)
            {
                return 0;
            }
            else
            {
                return this.gdiFontHelper.MeasureStringWidth(this.hFont, buffer, length);

            }
        }
        public override FontSignature GetFontSignature()
        {
            return new FontSignature(myFont.Name, myFont.Size, myFont.Style);

        }
    }

    static class FontManager
    {
        static Dictionary<FontSignature, TextFontInfo> fontDics = new Dictionary<FontSignature, TextFontInfo>();
        static TextFontInfo defaultTextFontInfo;
        static BasicGdi32FontHelper gdiFontHelper = new BasicGdi32FontHelper();

        static FontManager()
        {


            defaultTextFontInfo = new MyTextFontInfo(
                LayoutFarm.Drawing.CurrentGraphicPlatform.CreateFont(
                new System.Drawing.Font("Tahoma", 10)),
                gdiFontHelper);
            fontDics.Add(defaultTextFontInfo.GetFontSignature(), defaultTextFontInfo);


            RegisterFont(
                  LayoutFarm.Drawing.CurrentGraphicPlatform.CreateFont(
                 new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold)));

        }

        static void RegisterFont(Font fontInfo)
        {
            MyTextFontInfo textfontInfo = new MyTextFontInfo(fontInfo, gdiFontHelper);
            fontDics.Add(textfontInfo.GetFontSignature(), textfontInfo);

        }
        public static TextFontInfo GetTextFontInfo(string fontface, float size, bool bold, bool itatic)
        {


            FontStyle fontStyle = FontStyle.Regular;
            if (bold)
            {
                fontStyle |= FontStyle.Bold;
            }
            if (itatic)
            {
                fontStyle |= FontStyle.Italic;
            }

            FontSignature fontsig = new FontSignature(fontface, size, fontStyle);


            TextFontInfo textFontInfo;

            if (fontDics.TryGetValue(fontsig, out textFontInfo))
            {
                return textFontInfo;
            }
            else
            {

                textFontInfo = new MyTextFontInfo(
                    LayoutFarm.Drawing.CurrentGraphicPlatform.CreateFont(
                      new System.Drawing.Font(fontface,
                          size, (System.Drawing.FontStyle)fontStyle)),
                    gdiFontHelper);
                fontDics.Add(fontsig, textFontInfo);
                return textFontInfo;
            }
        }

        public static TextFontInfo GetTextFontInfo(FontSignature fontSig)
        {
            TextFontInfo textFontInfo;
            if (fontDics.TryGetValue(fontSig, out textFontInfo))
            {
                return textFontInfo;
            }
            else
            {

                textFontInfo = new MyTextFontInfo(
                     LayoutFarm.Drawing.CurrentGraphicPlatform.CreateFont(
                        new System.Drawing.Font(
                            fontSig.FontName,
                            fontSig.FontSize,
                            (System.Drawing.FontStyle)fontSig.FontStyle)),
                        gdiFontHelper);

                fontDics.Add(textFontInfo.GetFontSignature(), textFontInfo);
                return textFontInfo;
            }


        }
        public static TextFontInfo GetTextFontInfo(string fontface, float size)
        {
            TextFontInfo textFontInfo;
            FontSignature fontSig = new FontSignature(fontface, size, FontStyle.Regular);

            if (fontDics.TryGetValue(fontSig, out textFontInfo))
            {
                return textFontInfo;
            }
            else
            {
                textFontInfo = new MyTextFontInfo(
                     LayoutFarm.Drawing.CurrentGraphicPlatform.CreateFont(
                        new System.Drawing.Font(fontface, size)),
                    gdiFontHelper);
                fontDics.Add(fontSig, textFontInfo);
                return textFontInfo;
            }
        }
        public static TextFontInfo DefaultTextFontInfo
        {
            get
            {
                return defaultTextFontInfo;
            }
        }
        public static Font CurrentFont
        {
            get
            {
                return defaultTextFontInfo.Font;
            }
        }

        public static int GetStringWidth(char[] buffer)
        {
            return defaultTextFontInfo.GetStringWidth(buffer);
        }
    }
}