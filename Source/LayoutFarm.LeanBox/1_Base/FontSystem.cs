//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

namespace LayoutFarm
{



    public struct FontSignature
    {
        public static readonly FontSignature Empty = new FontSignature();
        string _fontName;
        float _fontSize; FontStyle style;
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


    public interface IFonts2
    {
        int[] MeasureCharWidths(IntPtr hFont);
        int MeasureStringWidth(IntPtr hFont, char[] buffer);
        int MeasureStringWidth(IntPtr hFont, char[] buffer, int length);
    }


    public class TextFontInfo
    {

        int[] charWidths;
        Font myFont;
        IntPtr hFont;
        int fontHeight;
        bool disposed;
        IFonts2 gdiFontHelper;

        public TextFontInfo(Font font, IFonts2 gdiFontHelper)
        {
            fontHeight = font.Height; myFont = font;
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
            Dispose();
        }
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
