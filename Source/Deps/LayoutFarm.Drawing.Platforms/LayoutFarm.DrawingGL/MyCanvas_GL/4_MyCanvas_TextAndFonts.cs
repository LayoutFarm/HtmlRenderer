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



namespace LayoutFarm.Drawing.DrawingGL
{
    partial class MyCanvasGL
    {
        Brush currentBrush;
        Font currentTextFont = null;
        Color mycurrentTextColor = Color.Black;
        //======================================
        //IFonts impl
        LayoutFarm.Drawing.FontInfo IFonts.GetFontInfo(string fontname, float fsize, FontStyle st)
        {
            return FontsUtils.GetCachedFont(fontname, fsize, (System.Drawing.FontStyle)st);
        }
        float IFonts.MeasureWhitespace(LayoutFarm.Drawing.Font f)
        {
            return FontsUtils.MeasureWhitespace(this, f);
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
        //==============================================



        public override void DrawText(char[] buffer, int x, int y)
        {
            throw new NotImplementedException();
            //if (isFromPrinter)
            //{
            //    //gx.DrawString(new string(buffer),
            //    //        ConvFont(prevFonts.Peek().Font),
            //    //        internalBrush,
            //    //        x,
            //    //        y);

            //}
            //else
            //{
            //    IntPtr gxdc = gx.GetHdc();
            //    MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
            //    NativeTextWin32.TextOut(gxdc, x, y, buffer, buffer.Length);
            //    MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
            //    gx.ReleaseHdc(gxdc);
            //}
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            throw new NotImplementedException();
            //if (isFromPrinter)
            //{
            //    //gx.DrawString(
            //    //    new string(buffer),
            //    //    ConvFont(prevFonts.Peek().Font),
            //    //    internalBrush,
            //    //    logicalTextBox.ToRect());
            //}
            //else
            //{
            //    IntPtr gxdc = gx.GetHdc();
            //    MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
            //    System.Drawing.Rectangle clipRect =
            //        System.Drawing.Rectangle.Intersect(logicalTextBox.ToRect(), currentClipRect);
            //    clipRect.Offset(CanvasOrgX, CanvasOrgY);
            //    MyWin32.SetRectRgn(hRgn, clipRect.X, clipRect.Y, clipRect.Right, clipRect.Bottom);
            //    MyWin32.SelectClipRgn(gxdc, hRgn);
            //    NativeTextWin32.TextOut(gxdc, logicalTextBox.X, logicalTextBox.Y, buffer, buffer.Length);
            //    MyWin32.SelectClipRgn(gxdc, IntPtr.Zero);

            //    MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
            //    gx.ReleaseHdc();
            //}
        }

        //public override Font CurrentFont
        //{
        //    get
        //    {
        //        return currentTextFont;
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //        //ReleaseHdc();
        //        //this.currentTextFont = value;

        //        //MyFont myFont = value as MyFont;
        //        //IntPtr hdc = gx.GetHdc();
        //        //MyWin32.SelectObject(hdc, myFont.ToHfont());

        //        //gx.ReleaseHdc();
        //    }
        //}
        public override Color CurrentTextColor
        {
            get
            {
                return mycurrentTextColor;
            }
            set
            {
                mycurrentTextColor = value;
            
            }
        }

        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {

#if DEBUG
            dbugCounter.dbugDrawStringCount++;
#endif
            throw new NotImplementedException();
            //var color = this.CurrentTextColor;
            //if (color.A == 255)
            //{
            //    unsafe
            //    {
            //        fixed (char* startAddr = &str[0])
            //        {
            //            //DrawingBridge.Win32Utils.TextOut2(_hdc, 
            //            //    (int)Math.Round(logicalTextBox.X + canvasOriginX),
            //            //    (int)Math.Round(logicalTextBox.Y + canvasOriginY),
            //            //    (startAddr + startAt), len);
            //            DrawingBridge.Win32Utils.TextOut2(_hdc,
            //                (int)logicalTextBox.X + canvasOriginX,
            //                (int)logicalTextBox.Y + canvasOriginY,
            //                (startAddr + startAt), len);
            //        }
            //    }
            //}
            //else
            //{
            //    //translucent / transparent text
            //    InitHdc();
            //    unsafe
            //    {
            //        fixed (char* startAddr = &str[0])
            //        {
            //            DrawingBridge.Win32Utils.TextOut2(_hdc,
            //                 logicalTextBox.X + canvasOriginX,
            //                 logicalTextBox.Y + canvasOriginY,
            //                (startAddr + startAt), len);
            //        }
            //    }

            //    //DrawTransparentText(_hdc, str, font, new Point((int)Math.Round(point.X), (int)Math.Round(point.Y)), Size.Round(size), color);
            //}
        }
        //====================================================
    }
}