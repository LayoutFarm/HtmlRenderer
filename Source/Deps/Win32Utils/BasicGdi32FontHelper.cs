//MIT, 2014-2016, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing.Fonts;
namespace Win32
{
    class BasicGdi32FontHelper : IDisposable
    {

        NativeWin32MemoryDc nativeWinDc;
        bool isInit;
        public BasicGdi32FontHelper()
        {
            Init();
        }
        public void Dispose()
        {
            if (nativeWinDc != null)
            {
                nativeWinDc.Dispose();
                nativeWinDc = null;
            }
        }
         
        void Init()
        {
            //bmp = new System.Drawing.Bitmap(2, 2);
            //System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            //hdc = g.GetHdc();
            nativeWinDc = new NativeWin32MemoryDc(2, 2);
            isInit = true;
        }
        const int MAX_CODEPOINT_NO = 255;
        public void MeasureCharWidths(IntPtr hFont, out int[] charWidths, out NativeTextWin32.FontABC[] abcSizes)
        {
            if (!isInit) Init();
            //only in ascii range
            //current version
            charWidths = new int[MAX_CODEPOINT_NO + 1]; // 
            MyWin32.SelectObject(nativeWinDc.DC, hFont);
            unsafe
            {
                //see: https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx
                //A code page contains 256 code points and is zero-based.
                //In most code pages, code points 0 through 127 represent the ASCII character set,
                //and code points 128 through 255 differ significantly between code pages
                abcSizes = new NativeTextWin32.FontABC[MAX_CODEPOINT_NO + 1];
                fixed (NativeTextWin32.FontABC* abc = abcSizes)
                {
                    NativeTextWin32.GetCharABCWidths(nativeWinDc.DC, (uint)0, (uint)MAX_CODEPOINT_NO, abc);
                }
                for (int i = 0; i < (MAX_CODEPOINT_NO + 1); ++i)
                {
                    charWidths[i] = abcSizes[i].Sum;
                }
            }
        }
        public int MeasureStringWidth(IntPtr hFont, char[] buffer)
        {
            if (!isInit) Init();
            MyWin32.SelectObject(nativeWinDc.DC, hFont);
            NativeTextWin32.WIN32SIZE size;
            NativeTextWin32.GetTextExtentPoint32(nativeWinDc.DC, buffer, buffer.Length, out size);
            return size.Width;
        }
        public int MeasureStringWidth(IntPtr hFont, char[] buffer, int length)
        {
            if (!isInit) Init();
            MyWin32.SelectObject(nativeWinDc.DC, hFont);
            NativeTextWin32.WIN32SIZE size;
            NativeTextWin32.GetTextExtentPoint32(nativeWinDc.DC, buffer, length, out size);
            return size.Width;
        }
    }
}