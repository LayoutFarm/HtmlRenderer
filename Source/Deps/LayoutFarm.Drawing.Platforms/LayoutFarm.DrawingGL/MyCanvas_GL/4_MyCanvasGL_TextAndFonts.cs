//2014,2015 BSD, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.DrawingGL;

namespace LayoutFarm.Drawing.DrawingGL
{
    partial class MyCanvasGL
    {
        GLTextPrinter myGLTextPrinter;
        Color textColor = Color.Black;
        //======================================
        //IFonts impl
        LayoutFarm.Drawing.FontInfo IFonts.GetFontInfo(string fontname, float fsize, FontStyle st)
        {
            //return FontsUtils.GetCachedFont(fontname, fsize, (System.Drawing.FontStyle)st);
            return this.platform.GetFont(fontname, fsize, st);
        }
        float IFonts.MeasureWhitespace(LayoutFarm.Drawing.Font f)
        {   
            //platform specific
            return LayoutFarm.Drawing.WinGdi.FontStore.MeasureWhitespace(this, f);

        }
        //======================================

        public Size MeasureString(char[] buff, int startAt, int len, Font font)
        {
            throw new NotImplementedException();

            //if (_useGdiPlusTextRendering)
            //{
            //    ReleaseHdc();
            //    _characterRanges[0] = new System.Drawing.CharacterRange(0, len);
            //    _stringFormat.SetMeasurableCharacterRanges(_characterRanges);
            //    System.Drawing.Font font2 = (System.Drawing.Font)font.InnerFont;

            //    var size = gx.MeasureCharacterRanges(
            //        new string(buff, startAt, len),
            //        font2,
            //        System.Drawing.RectangleF.Empty,
            //        _stringFormat)[0].GetBounds(gx).Size;
            //    return new LayoutFarm.Drawing.Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));
            //}
            //else
            //{

            //}

            //SetFont(font);
            //var size = new System.Drawing.Size();
            //unsafe
            //{
            //    fixed (char* startAddr = &buff[0])
            //    {
            //        DrawingBridge.Win32Utils.UnsafeGetTextExtentPoint32(_hdc, startAddr + startAt, len, ref size);
            //    }
            //}
            //return size.ToSize();
        }
        /// <summary>
        /// Measure the width and height of string <paramref name="str"/> when drawn on device context HDC
        /// using the given font <paramref name="font"/>.<br/>
        /// Restrict the width of the string and get the number of characters able to fit in the restriction and
        /// the width those characters take.
        /// </summary>
        /// <param name="str">the string to measure</param>
        /// <param name="font">the font to measure string with</param>
        /// <param name="maxWidth">the max width to render the string in</param>
        /// <param name="charFit">the number of characters that will fit under <see cref="maxWidth"/> restriction</param>
        /// <param name="charFitWidth"></param>
        /// <returns>the size of the string</returns>
        public Size MeasureString(char[] buff, int startAt, int len, Font font, float maxWidth, out int charFit, out int charFitWidth)
        {
            throw new NotImplementedException();
            //SetFont(font);

            //var size = new System.Drawing.Size();
            //unsafe
            //{
            //    fixed (char* startAddr = &buff[0])
            //    {
            //        DrawingBridge.Win32Utils.UnsafeGetTextExtentExPoint(
            //            _hdc, startAddr + startAt, len,
            //            (int)Math.Round(maxWidth), _charFit, _charFitWidth, ref size);
            //    }

            //}
            //charFit = _charFit[0];
            //charFitWidth = charFit > 0 ? _charFitWidth[charFit - 1] : 0;
            //return size.ToSize();

        }
        public override Color CurrentTextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;

            }
        }
        public override Font CurrentFont
        {
            get
            {
                return this.currentFont;
            }
            set
            {
                currentFont = value;
                if (this.myGLTextPrinter != null)
                {
                    //assign font        
                    this.myGLTextPrinter.CurrentFont = value.FontInfo.PlatformSpecificFont as PixelFarm.Agg.Fonts.Font;
                }
                //sample only ***  
                //canvasGL2d.CurrentFont = (PixelFarm.Agg.Fonts.Font)defaultFontInfo.PlatformSpecificFont;
            }
        }
        public override void DrawText(char[] buffer, int x, int y)
        {

            //handle draw canvas with 
            if (this.myGLTextPrinter == null)
            {
                this.myGLTextPrinter = new GLTextPrinter(canvasGL2d);
                this.myGLTextPrinter.CurrentFont = this.currentFont.FontInfo.PlatformSpecificFont as PixelFarm.Agg.Fonts.Font;
            }
            myGLTextPrinter.Print(this.textColor, buffer, 0, buffer.Length, x, y);

        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            if (this.myGLTextPrinter == null)
            {
                this.myGLTextPrinter = new GLTextPrinter(canvasGL2d);
                this.myGLTextPrinter.CurrentFont = this.currentFont.FontInfo.PlatformSpecificFont as PixelFarm.Agg.Fonts.Font;

            }
            myGLTextPrinter.Print(this.textColor, buffer, 0, buffer.Length, logicalTextBox.X, logicalTextBox.Y);
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            if (this.myGLTextPrinter == null)
            {
                this.myGLTextPrinter = new GLTextPrinter(canvasGL2d);
                this.myGLTextPrinter.CurrentFont = this.currentFont.FontInfo.PlatformSpecificFont as PixelFarm.Agg.Fonts.Font;
            }
            myGLTextPrinter.Print(this.textColor, str, startAt, len, logicalTextBox.X, logicalTextBox.Y);
        }
      
    }
}