//BSD, 2014-2016, WinterDev  

using System;
using System.Runtime.InteropServices;

namespace Win32
{

    class NativeWin32MemoryDc : IDisposable
    {
        int _width;
        int _height;
        IntPtr memHdc;
        IntPtr dib;
        IntPtr ppvBits;
        bool isDisposed;
        public NativeWin32MemoryDc(int w, int h)
        {
            this._width = w;
            this._height = h;

            memHdc = MyWin32.CreateMemoryHdc(
                IntPtr.Zero,
                w,
                h,
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
            MyWin32.ReleaseMemoryHdc(memHdc, dib);
            dib = IntPtr.Zero;
            memHdc = IntPtr.Zero;
            isDisposed = true;
        }

    }

}