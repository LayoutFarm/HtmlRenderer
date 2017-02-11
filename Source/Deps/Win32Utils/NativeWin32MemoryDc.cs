//BSD, 2014-2017, WinterDev  

using System;

namespace Win32
{

    public class NativeWin32MemoryDc : IDisposable
    {
        int _width;
        int _height;
        IntPtr memHdc;
        IntPtr dib;
        IntPtr ppvBits;
        IntPtr hRgn = IntPtr.Zero;

        bool isDisposed;
        public NativeWin32MemoryDc(int w, int h, bool invertImage = false)
        {
            this._width = w;
            this._height = h;

            memHdc = MyWin32.CreateMemoryHdc(
                IntPtr.Zero,
                w,
                invertImage ? -h : h, //***
                out dib,
                out ppvBits);
        }
        public IntPtr DC
        {
            get { return this.memHdc; }
        }
        public IntPtr PPVBits
        {
            get { return this.ppvBits; }
        }
        public void SetTextColor(int win32Color)
        {
            Win32.MyWin32.SetTextColor(memHdc, win32Color);
        }
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            if (hRgn != IntPtr.Zero)
            {
                MyWin32.DeleteObject(hRgn);
                hRgn = IntPtr.Zero;
            }


            MyWin32.ReleaseMemoryHdc(memHdc, dib);
            dib = IntPtr.Zero;
            memHdc = IntPtr.Zero;
            isDisposed = true;
        }
        public void PatBlt(PatBltColor color)
        {
            MyWin32.PatBlt(memHdc, 0, 0, _width, _height, (int)color);
        }
        public void SetBackTransparent(bool value)
        {
            //public const int _SetBkMode_TRANSPARENT = 1;
            //public const int _SetBkMode_OPAQUE = 2;
            MyWin32.SetBkMode(memHdc, value ? 1 : 2);
        }
        public enum PatBltColor
        {
            Black = MyWin32.BLACKNESS,
            White = MyWin32.WHITENESS
        }
        public IntPtr SetFont(IntPtr hFont)
        {
            return MyWin32.SelectObject(memHdc, hFont);
        }
        /// <summary>
        /// set solid text color
        /// </summary>
        /// <param name="r">0-255</param>
        /// <param name="g">0-255</param>
        /// <param name="b">0-255</param>
        public void SetSolidTextColor(byte r, byte g, byte b)
        {
            //convert to win32 colorv
            MyWin32.SetTextColor(memHdc, (b & 0xFF) << 16 | (g & 0xFF) << 8 | r);
        }
        public void SetClipRect(PixelFarm.Drawing.Rectangle r)
        {
            SetClipRect(r.Left, r.Top, r.Width, r.Height);
        }
        public void SetClipRect(int x, int y, int w, int h)
        {
            if (hRgn == IntPtr.Zero)
            {
                //create
                hRgn = MyWin32.CreateRectRgn(0, 0, w, h);
            }
            MyWin32.SetRectRgn(hRgn,
            x,
            y,
            x + w,
            y + h);
            MyWin32.SelectObject(memHdc, hRgn);
        }
        public void ClearClipRect()
        {
            MyWin32.SelectClipRgn(memHdc, IntPtr.Zero);

        }
    }

}