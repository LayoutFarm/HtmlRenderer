//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{
    class NativeTextWin32
    {
        [DllImport("gdi32.dll")]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, char[] charBuffer, int cbstring);
        [DllImport("gdi32.dll")]
        public static extern bool GetTextExtentPoint32(IntPtr hdc, char[] charBuffer, int c, out WIN32SIZE size);

        [DllImport("gdi32.dll", EntryPoint = "GetTextExtentPoint32")]
        public static unsafe extern bool GetTextExtentPoint32Char(IntPtr hdc, char* ch, int c, out WIN32SIZE size);

        [DllImport("gdi32.dll")]
        public static unsafe extern bool ExtTextOut(IntPtr hdc, int x, int y, uint fuOptions,
            Rectangle* lpRect, char[] charBuffer, int cbCount, object arrayOfSpaceValues);

        public const int ETO_OPAQUE = 0x0002;
        public const int ETO_CLIPPED = 0x0004;


        [DllImport("gdi32.dll")]
        public static unsafe extern int GetGlyphIndices(IntPtr hdc, string text, int c, char* buffer, int fl);

        [DllImport("gdi32.dll")]
        public static unsafe extern bool GetCharABCWidths(IntPtr hdc, uint uFirstChar, uint uLastChar, void* lpabc);
        

        [StructLayout(LayoutKind.Sequential)]
        public struct FontABC
        {
            public int abcA;
            public uint abcB;
            public int abcC;
            public int Sum
            {
                get
                {
                    return abcA + (int)abcB + abcC;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WIN32SIZE
        {
            public int Width;
            public int Height;

            public WIN32SIZE(int w, int h)
            {
                this.Width = w;
                this.Height = h;
            }
        }
    }
}
