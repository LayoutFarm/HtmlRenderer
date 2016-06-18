//2014,2015 BSD, WinterDev
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
using Win32;
namespace PixelFarm.Drawing.WinGdi
{
    partial class MyScreenCanvas
    {
        Font currentTextFont = null;
        Color mycurrentTextColor = Color.Black;
        //======================================
        //IFonts impl
        PixelFarm.Drawing.FontInfo IFonts.GetFontInfo(string fontname, float fsize, FontStyle st)
        {
            return this.platform.GetFont(fontname, fsize, st);
        }
        float IFonts.MeasureWhitespace(PixelFarm.Drawing.Font f)
        {
            return FontStore.MeasureWhitespace(this, f);
        }
        //======================================

        public Size MeasureString(char[] buff, int startAt, int len, Font font)
        {
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
            //    return new PixelFarm.Drawing.Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));
            //}
            //else
            //{
            SetFont(font);
            PixelFarm.Drawing.Size size = new Size();
            unsafe
            {
                fixed (char* startAddr = &buff[0])
                {
                    NativeTextWin32.UnsafeGetTextExtentPoint32(tempDc, startAddr + startAt, len, ref size);
                }
            }
            return size;
            //}
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
            //if (_useGdiPlusTextRendering)
            //{
            //    ReleaseHdc();
            //    throw new NotSupportedException("Char fit string measuring is not supported for GDI+ text rendering");
            //}
            //else
            //{
            SetFont(font);
            var size = new PixelFarm.Drawing.Size();
            unsafe
            {
                fixed (char* startAddr = &buff[0])
                {
                    NativeTextWin32.UnsafeGetTextExtentExPoint(
                        tempDc, startAddr + startAt, len,
                        (int)Math.Round(maxWidth), _charFit, _charFitWidth, ref size);
                }
            }
            charFit = _charFit[0];
            charFitWidth = charFit > 0 ? _charFitWidth[charFit - 1] : 0;
            return size;
            //}
        }
        //==============================================



        public override void DrawText(char[] buffer, int x, int y)
        {
            ReleaseHdc();
            IntPtr gxdc = gx.GetHdc();
            var clipRect = currentClipRect;
            clipRect.Offset(canvasOriginX, canvasOriginY);
            MyWin32.SetRectRgn(hRgn,
             clipRect.Left,
             clipRect.Top,
             clipRect.Right,
             clipRect.Bottom);
            MyWin32.SelectClipRgn(gxdc, hRgn);
            NativeTextWin32.TextOut(gxdc, CanvasOrgX + x, CanvasOrgY + y, buffer, buffer.Length);
            MyWin32.SelectClipRgn(gxdc, IntPtr.Zero);
            gx.ReleaseHdc();
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            ReleaseHdc();
            IntPtr gxdc = gx.GetHdc();
            var clipRect = System.Drawing.Rectangle.Intersect(logicalTextBox.ToRect(), currentClipRect);
            clipRect.Offset(canvasOriginX, canvasOriginY);
            MyWin32.SetRectRgn(hRgn,
             clipRect.Left,
             clipRect.Top,
             clipRect.Right,
             clipRect.Bottom);
            MyWin32.SelectClipRgn(gxdc, hRgn);
            NativeTextWin32.TextOut(gxdc, CanvasOrgX + logicalTextBox.X, CanvasOrgY + logicalTextBox.Y, buffer, buffer.Length);
            MyWin32.SelectClipRgn(gxdc, IntPtr.Zero);
            gx.ReleaseHdc();
            //ReleaseHdc();
            //IntPtr gxdc = gx.GetHdc();
            //MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
            //System.Drawing.Rectangle clipRect =
            //    System.Drawing.Rectangle.Intersect(logicalTextBox.ToRect(), currentClipRect);
            //clipRect.Offset(CanvasOrgX, CanvasOrgY);
            //MyWin32.SetRectRgn(hRgn, clipRect.X, clipRect.Y, clipRect.Right, clipRect.Bottom);
            //MyWin32.SelectClipRgn(gxdc, hRgn);
            //NativeTextWin32.TextOut(gxdc, logicalTextBox.X, logicalTextBox.Y, buffer, buffer.Length); 
            //MyWin32.SelectClipRgn(gxdc, IntPtr.Zero); 
            //MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero); 
            //gx.ReleaseHdc();

        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
#if DEBUG
            dbugCounter.dbugDrawStringCount++;
#endif
            var color = this.CurrentTextColor;
            if (color.A == 255)
            {
                var clipRect = Rectangle.Intersect(logicalTextBox,
                    new Rectangle(currentClipRect.Left,
                        currentClipRect.Top,
                        currentClipRect.Width,
                        currentClipRect.Height));
                clipRect.Offset(canvasOriginX, canvasOriginY);
                MyWin32.SetRectRgn(hRgn,
                 clipRect.Left,
                 clipRect.Top,
                 clipRect.Right,
                 clipRect.Bottom);
                MyWin32.SelectClipRgn(tempDc, hRgn);
                unsafe
                {
                    fixed (char* startAddr = &str[0])
                    {
                        NativeTextWin32.TextOutUnsafe(tempDc,
                            (int)logicalTextBox.X + canvasOriginX,
                            (int)logicalTextBox.Y + canvasOriginY,
                            (startAddr + startAt), len);
                    }
                }
                MyWin32.SelectClipRgn(tempDc, IntPtr.Zero);
#if DEBUG
                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
                //        logicalTextBox.X + canvasOriginX,
                //        logicalTextBox.Y + canvasOriginY);
#endif

            }
            else
            {
                //translucent / transparent text
                InitHdc();
                var intersectRect = Rectangle.Intersect(logicalTextBox,
                        new Rectangle(currentClipRect.Left,
                            currentClipRect.Top,
                            currentClipRect.Width,
                            currentClipRect.Height));
                intersectRect.Offset(canvasOriginX, canvasOriginY);
                MyWin32.SetRectRgn(hRgn,
                 intersectRect.Left,
                 intersectRect.Top,
                 intersectRect.Right,
                 intersectRect.Bottom);
                MyWin32.SelectClipRgn(tempDc, hRgn);
                unsafe
                {
                    fixed (char* startAddr = &str[0])
                    {
                        NativeTextWin32.TextOutUnsafe(tempDc,
                             logicalTextBox.X + canvasOriginX,
                             logicalTextBox.Y + canvasOriginY,
                            (startAddr + startAt), len);
                    }
                }
#if DEBUG
                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
                //    logicalTextBox.X + canvasOriginX,
                //    logicalTextBox.Y + canvasOriginY);
#endif

            }
        }
        //====================================================
        public override Font CurrentFont
        {
            get
            {
                return currentTextFont;
            }
            set
            {
                ReleaseHdc();
                this.currentTextFont = value;
                MyFont myFont = value as MyFont;
                IntPtr hdc = gx.GetHdc();
                MyWin32.SelectObject(hdc, myFont.ToHfont());
                gx.ReleaseHdc();
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
                mycurrentTextColor = value;
                SetTextColor(value);
                //this.currentTextColor = ConvColor(value);
                //IntPtr hdc = gx.GetHdc();
                //MyWin32.SetTextColor(hdc, MyWin32.ColorToWin32(value));
                //gx.ReleaseHdc();
            }
        }
    }
}