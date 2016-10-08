//BSD, 2014-2016, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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
using PixelFarm.Drawing.Fonts;
using Win32;

namespace PixelFarm.Drawing.GLES2
{
    partial class MyGLCanvas
    {

        Font currentTextFont = null;
        Color mycurrentTextColor = Color.Black;
        NativeFontStore nativeFontStore = new NativeFontStore();
        public ActualFont ResolveActualFont(Font f)
        {
            //TODO: review here
            return nativeFontStore.GetResolvedNativeFont(f);
        }
        public override float GetCharWidth(Font f, char c)
        {
            NativeFont font = nativeFontStore.GetResolvedNativeFont(f);
            return font.GetGlyph(c).horiz_adv_x >> 6;
        }
        //======================================
        public override ActualFont GetActualFont(Font f)
        {
            return nativeFontStore.GetResolvedNativeFont(f);
        }
        public Size MeasureString(char[] buff, int startAt, int len, Font font)
        {
            //throw new NotSupportedException();
            ////_characterRanges[0] = new System.Drawing.CharacterRange(0, len);
            ////_stringFormat.SetMeasurableCharacterRanges(_characterRanges);
            ////System.Drawing.Font font2 = (System.Drawing.Font)font.InnerFont;
            ////var size = gx.MeasureCharacterRanges(
            ////    new string(buff, startAt, len),
            ////    font2,
            ////    System.Drawing.RectangleF.Empty,
            ////    _stringFormat)[0].GetBounds(gx).Size;
            ////return new PixelFarm.Drawing.Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));


            this.currentTextFont = font;
            //TODO: review, select current font to windc TOO!
            var size = new PixelFarm.Drawing.Size();
            unsafe
            {
                fixed (char* startAddr = &buff[0])
                {
                    NativeTextWin32.UnsafeGetTextExtentExPoint(
                         win32MemDc.DC, startAddr + startAt, len,
                        int.MaxValue, _charFit, _charFitWidth, ref size);
                }
            }

            return size;


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
        public Size MeasureString(char[] buff, int startAt, int len,
            Font font, float maxWidth,
            out int charFit, out int charFitWidth)
        {

            this.currentTextFont = font;
            //TODO: review, select current font to windc TOO!
            var size = new PixelFarm.Drawing.Size();
            unsafe
            {
                fixed (char* startAddr = &buff[0])
                {

                    NativeTextWin32.UnsafeGetTextExtentExPoint(
                         win32MemDc.DC, startAddr + startAt, len,
                        (int)Math.Round(maxWidth), _charFit, _charFitWidth, ref size);
                }
            }
            charFit = _charFit[0];
            charFitWidth = charFit > 0 ? _charFitWidth[charFit - 1] : 0;
            return size;


        }
        //============================================== 

        public override Font CurrentFont
        {
            get
            {
                return currentTextFont;
            }
            set
            {
                this.currentTextFont = value;
            }
        }
        public override Color CurrentTextColor
        {
            get
            {
                return mycurrentTextColor;
            }
            set
            {
                this.currentTextColor = value;
            }
        }
        public override void DrawText(char[] buffer, int x, int y)
        {
            //var tmpColor = this.internalSolidBrush.Color;
            //internalSolidBrush.Color = this.currentTextColor;
            //gx.DrawString(new string(buffer),
            //    (System.Drawing.Font)this.currentTextFont.InnerFont,
            //    internalSolidBrush, new System.Drawing.PointF(x, y));
            //internalSolidBrush.Color = tmpColor;
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            //var tmpColor = this.internalSolidBrush.Color;
            //internalSolidBrush.Color = this.currentTextColor;
            //gx.DrawString(new string(buffer),
            //    (System.Drawing.Font)this.currentTextFont.InnerFont,
            //    internalSolidBrush,
            //    new System.Drawing.RectangleF(
            //        logicalTextBox.X,
            //        logicalTextBox.Y,
            //        logicalTextBox.Width,
            //        logicalTextBox.Height));
            //internalSolidBrush.Color = tmpColor;
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            ////var intersectRect = Rectangle.Intersect(logicalTextBox,
            ////    new Rectangle(currentClipRect.Left,
            ////        currentClipRect.Top,
            ////        currentClipRect.Width,
            ////        currentClipRect.Height));
            ////intersectRect.Offset(canvasOriginX, canvasOriginY);
            ////MyWin32.SetRectRgn(hRgn,
            //// intersectRect.Left,
            //// intersectRect.Top,
            //// intersectRect.Right,
            //// intersectRect.Bottom);
            ////MyWin32.SelectClipRgn(tempDc, hRgn);



            //var tmpColor = this.internalSolidBrush.Color;
            //internalSolidBrush.Color = this.currentTextColor;
            //gx.DrawString(new string(str, startAt, len),
            //    (System.Drawing.Font)this.currentTextFont.InnerFont,
            //    internalSolidBrush,
            //    logicalTextBox.X,
            //    logicalTextBox.Y);
            ////new System.Drawing.RectangleF(
            ////    logicalTextBox.X,
            ////    logicalTextBox.Y,
            ////    logicalTextBox.Width,
            ////    logicalTextBox.Height));
            //internalSolidBrush.Color = tmpColor;
            ////var str= new string(
            ////fixed (char* startAddr = &str[0])
            ////{
            ////    Win32.Win32Utils.TextOut2(tempDc,
            ////        (int)logicalTextBox.X + canvasOriginX,
            ////        (int)logicalTextBox.Y + canvasOriginY,
            ////        (startAddr + startAt), len);
            ////}


        }
    }
}