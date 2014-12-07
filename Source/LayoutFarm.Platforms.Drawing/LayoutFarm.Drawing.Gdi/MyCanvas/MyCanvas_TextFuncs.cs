//2014 BSD, WinterDev
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
using LayoutFarm.Drawing;

using DrawingBridge;

namespace LayoutFarm
{
    partial class MyCanvas
    {
        Color mycurrentTextColor = Color.Black;
        
        public override void DrawText(char[] buffer, int x, int y)
        {

            if (isFromPrinter)
            {
                //gx.DrawString(new string(buffer),
                //        ConvFont(prevFonts.Peek().Font),
                //        internalBrush,
                //        x,
                //        y);

            }
            else
            {
                IntPtr gxdc = gx.GetHdc();
                MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
                NativeTextWin32.TextOut(gxdc, x, y, buffer, buffer.Length);
                MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
                gx.ReleaseHdc(gxdc);
            }
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {

            if (isFromPrinter)
            {
                //gx.DrawString(
                //    new string(buffer),
                //    ConvFont(prevFonts.Peek().Font),
                //    internalBrush,
                //    logicalTextBox.ToRect());
            }
            else
            {
                IntPtr gxdc = gx.GetHdc();
                MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
                System.Drawing.Rectangle clipRect =
                    System.Drawing.Rectangle.Intersect(logicalTextBox.ToRect(), currentClipRect);
                clipRect.Offset(CanvasOrgX, CanvasOrgY);
                MyWin32.SetRectRgn(hRgn, clipRect.X, clipRect.Y, clipRect.Right, clipRect.Bottom);
                MyWin32.SelectClipRgn(gxdc, hRgn);
                NativeTextWin32.TextOut(gxdc, logicalTextBox.X, logicalTextBox.Y, buffer, buffer.Length);
                MyWin32.SelectClipRgn(gxdc, IntPtr.Zero);

                MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
                gx.ReleaseHdc();
            }
        }
        
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
        //public override void PushFont(FontInfo FontInfo)
        //{
        //    prevFonts.Push(currentTextFont);
        //    currentTextFont = FontInfo;
        //    IntPtr hdc = gx.GetHdc();
        //    prevHFonts.Push(MyWin32.SelectObject(hdc, FontInfo.HFont));
        //    gx.ReleaseHdc();
        //}
        //public override void PopFont()
        //{
        //    IntPtr hdc = gx.GetHdc();
        //    if (prevHFonts.Count > 0)
        //    {
        //        currentTextFont = prevFonts.Pop();
        //        MyWin32.SelectObject(hdc, prevHFonts.Pop());
        //    }
        //    gx.ReleaseHdc();
        //}

        //public override void PushTextColor(Color color)
        //{

        //    IntPtr hdc = gx.GetHdc();
        //    prevColor.Push(currentTextColor);
        //    this.currentTextColor = ConvColor(color);
        //    prevWin32Colors.Push(MyWin32.SetTextColor(hdc, MyWin32.ColorToWin32(color)));
        //    gx.ReleaseHdc();
        //}
        //public override void PopTextColor()
        //{
        //    IntPtr hdc = gx.GetHdc();
        //    if (prevColor.Count > 0)
        //    {
        //        currentTextColor = prevColor.Pop();
        //        MyWin32.SetTextColor(hdc, prevWin32Colors.Pop());
        //    }
        //    gx.ReleaseHdc();
        //}

        /// <summary>
        /// Measure the width and height of string <paramref name="str"/> when drawn on device context HDC
        /// using the given font <paramref name="font"/>.
        /// </summary>
        /// <param name="str">the string to measure</param>
        /// <param name="font">the font to measure string with</param>
        /// <returns>the size of the string</returns>
        public Size MeasureString(string str, Font font)
        {
            //if (_useGdiPlusTextRendering)
            //{
            //    ReleaseHdc();
            //    _characterRanges[0] = new System.Drawing.CharacterRange(0, str.Length);
            //    _stringFormat.SetMeasurableCharacterRanges(_characterRanges);

            //    var font2 = font.InnerFont as System.Drawing.Font;
            //    var size = gx.MeasureCharacterRanges(str,
            //        font2,
            //        System.Drawing.RectangleF.Empty,
            //        _stringFormat)[0].GetBounds(gx).Size;

            //    return new Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));
            //}
            //else
            //{

            SetFont(font);
            var size = new System.Drawing.Size();
            Win32Utils.GetTextExtentPoint32(_hdc, str, str.Length, ref size);
            return size.ToSize();

            //}
        }
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
            //    return new LayoutFarm.Drawing.Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));
            //}
            //else
            //{
            SetFont(font);
            var size = new System.Drawing.Size();
            unsafe
            {
                fixed (char* startAddr = &buff[0])
                {
                    Win32Utils.UnsafeGetTextExtentPoint32(_hdc, startAddr + startAt, len, ref size);
                }
            }
            return size.ToSize();
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

            var size = new System.Drawing.Size();
            unsafe
            {
                fixed (char* startAddr = &buff[0])
                {
                    Win32Utils.UnsafeGetTextExtentExPoint(
                        _hdc, startAddr + startAt, len,
                        (int)Math.Round(maxWidth), _charFit, _charFitWidth, ref size);
                }

            }
            charFit = _charFit[0];
            charFitWidth = charFit > 0 ? _charFitWidth[charFit - 1] : 0;
            return size.ToSize();
            //}
        }


        /// <summary>
        /// Init HDC for the current graphics object to be used to call GDI directly.
        /// </summary>
        void InitHdc()
        {
            if (_hdc == IntPtr.Zero)
            {
                //var clip = _g.Clip.GetHrgn(_g);
                _hdc = gx.GetHdc();
                Win32Utils.SetBkMode(_hdc, 1);
                //Win32Utils.SelectClipRgn(_hdc, clip);
                //Win32Utils.DeleteObject(clip);
            }
        }

        /// <summary>
        /// Release current HDC to be able to use <see cref="Graphics"/> methods.
        /// </summary>
        void ReleaseHdc()
        {
            if (_hdc != IntPtr.Zero)
            {
                Win32Utils.SelectClipRgn(_hdc, IntPtr.Zero);
                gx.ReleaseHdc(_hdc);
                _hdc = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Set a resource (e.g. a font) for the specified device context.
        /// WARNING: Calling Font.ToHfont() many times without releasing the font handle crashes the app.
        /// </summary>
        void SetFont(Font font)
        {
            InitHdc();
            Win32Utils.SelectObject(_hdc, FontsUtils.GetCachedHFont(font.InnerFont as System.Drawing.Font));
        }

        /// <summary>
        /// Set the text color of the device context.
        /// </summary>
        void SetTextColor(Color color)
        {
            InitHdc();
            int rgb = (color.B & 0xFF) << 16 | (color.G & 0xFF) << 8 | color.R;
            Win32Utils.SetTextColor(_hdc, rgb);
        }

        /// <summary>
        /// Special draw logic to draw transparent text using GDI.<br/>
        /// 1. Create in-memory DC<br/>
        /// 2. Copy background to in-memory DC<br/>
        /// 3. Draw the text to in-memory DC<br/>
        /// 4. Copy the in-memory DC to the proper location with alpha blend<br/>
        /// </summary>
        static void DrawTransparentText(IntPtr hdc, string str, Font font, Point point, Size size, Color color)
        {
            IntPtr dib;
            var memoryHdc = Win32Utils.CreateMemoryHdc(hdc, size.Width, size.Height, out dib);

            try
            {
                // copy target background to memory HDC so when copied back it will have the proper background
                Win32Utils.BitBlt(memoryHdc, 0, 0, size.Width, size.Height, hdc, point.X, point.Y, Win32Utils.BitBltCopy);

                // Create and select font
                Win32Utils.SelectObject(memoryHdc, FontsUtils.GetCachedHFont(font.InnerFont as System.Drawing.Font));
                Win32Utils.SetTextColor(memoryHdc, (color.B & 0xFF) << 16 | (color.G & 0xFF) << 8 | color.R);

                // Draw text to memory HDC
                Win32Utils.TextOut(memoryHdc, 0, 0, str, str.Length);

                // copy from memory HDC to normal HDC with alpha blend so achieve the transparent text
                Win32Utils.AlphaBlend(hdc, point.X, point.Y, size.Width, size.Height, memoryHdc, 0, 0, size.Width, size.Height, new BlendFunction(color.A));
            }
            finally
            {
                Win32Utils.ReleaseMemoryHdc(memoryHdc, dib);
            }
        }


    }

}