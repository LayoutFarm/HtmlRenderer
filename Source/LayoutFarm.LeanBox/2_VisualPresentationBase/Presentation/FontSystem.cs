using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

namespace LayoutFarm.Presentation
{

    public static class FontManager
    {
        static Dictionary<FontSignature, TextFontInfo> fontDics = new Dictionary<FontSignature, TextFontInfo>();
        static TextFontInfo defaultTextFontInfo;
        static BasicGdi32FontHelper gdiFontHelper = new BasicGdi32FontHelper();

        static FontManager()
        {
                        defaultTextFontInfo = new TextFontInfo(new Font("Tahoma", 10), gdiFontHelper);
                        fontDics.Add(defaultTextFontInfo.GetFontSignature(), defaultTextFontInfo);

                                    RegisterFont(new Font("Tahoma", 14, FontStyle.Bold));

        }

        static void RegisterFont(Font fontInfo)
        {
            TextFontInfo textfontInfo = new TextFontInfo(fontInfo, gdiFontHelper);
            fontDics.Add(textfontInfo.GetFontSignature(), new TextFontInfo(fontInfo, gdiFontHelper));

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


                textFontInfo = new TextFontInfo(new Font(fontface, size, fontStyle), gdiFontHelper);
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

                textFontInfo = new TextFontInfo(new Font(fontSig.FontName, fontSig.FontSize, fontSig.FontStyle), gdiFontHelper);

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
                textFontInfo = new TextFontInfo(new Font(fontface, size), gdiFontHelper);
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

                public struct FontSignature
    {
        public static readonly FontSignature Empty = new FontSignature();
                string _fontName;
                float _fontSize;        FontStyle style;
        public FontSignature(string fontName, float fontSize, FontStyle style)
        {
            this._fontName = fontName;
            this._fontSize = fontSize;
            this.style = style;
        }
        public FontStyle FontStyle
        {
            get
            {
                return this.style;
            }
        }

        public string FontName
        {
            get
            {
                return this._fontName;
            }
        }
        public float FontSize
        {
            get
            {
                return this._fontSize;
            }
        }

    }


    public class BasicGdi32FontHelper
    {
        Bitmap bmp;
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

            bmp = new Bitmap(2, 2);
            Graphics g = Graphics.FromImage(bmp);
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
                {                       charWidths[i] = abcSizes[i].Sum;

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
            return size.Width;        }
        public int MeasureStringWidth(IntPtr hFont, char[] buffer, int length)
        {
            if (!isInit) Init();
            MyWin32.SelectObject(this.hdc, hFont);
            NativeTextWin32.WIN32SIZE size;
            NativeTextWin32.GetTextExtentPoint32(hdc, buffer, length, out size);
            return size.Width;        }
    }
                public class TextFontInfo
    {
        
        int[] charWidths; 
        Font myFont;
        IntPtr hFont;

        int fontHeight;
        bool disposed;        BasicGdi32FontHelper gdiFontHelper;

                public TextFontInfo(Font font, BasicGdi32FontHelper gdiFontHelper)
        {
                                                fontHeight = font.Height;            myFont = font;
            hFont = myFont.ToHfont();
            this.gdiFontHelper = gdiFontHelper;
            charWidths = gdiFontHelper.MeasureCharWidths(hFont);

        }

        public void Dispose()
        {
            myFont.Dispose();
            disposed = true;
        }
        ~TextFontInfo()
        {
            Dispose();        }
        public int FontHeight
        {
            get
            {
                return fontHeight;
            }
        }
        public Font Font
        {
            get
            {
                return myFont;
            }

        }
        public IntPtr HFont
        {
            get
            {
                return hFont;
            }
        }
                                                public int GetCharWidth(char c)
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
        public int GetStringWidth(char[] buffer)
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
        public int GetStringWidth(char[] buffer, int length)
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
        public FontSignature GetFontSignature()
        {
            return new FontSignature(myFont.Name, myFont.Size, myFont.Style);

        }
    }
}
