//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D; 
namespace LayoutFarm.Presentation
{
    public class BasicGdi32FontHelper : IFonts2
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
}