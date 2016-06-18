// 2015,2014 ,Apache2, WinterDev

using System;
using System.Runtime.InteropServices;
using PixelFarm.Drawing;
namespace Win32
{
    class NativeTextWin32
    {
        [DllImport("gdi32.dll")]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart,
            [MarshalAs(UnmanagedType.LPWStr)]string charBuffer, int cbstring);
        [DllImport("gdi32.dll")]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, char[] charBuffer, int cbstring);
        [DllImport("gdi32.dll", EntryPoint = "TextOutW")]
        public static unsafe extern bool TextOutUnsafe(IntPtr hdc, int x, int y, char* s, int len);
        [DllImport("gdi32.dll")]
        public static unsafe extern bool ExtTextOut(IntPtr hdc, int x, int y, uint fuOptions,
            Rectangle* lpRect, char[] charBuffer, int cbCount, object arrayOfSpaceValues);
        [DllImport("gdi32.dll")]
        public static extern bool GetTextExtentPoint32(IntPtr hdc, char[] charBuffer, int c, out WIN32SIZE size);
        [DllImport("gdi32.dll", EntryPoint = "GetTextExtentPoint32")]
        public static unsafe extern bool GetTextExtentPoint32Char(IntPtr hdc, char* ch, int c, out WIN32SIZE size);
        public const int ETO_OPAQUE = 0x0002;
        public const int ETO_CLIPPED = 0x0004;
        [DllImport("gdi32.dll", EntryPoint = "GetTextExtentPoint32W")]
        public static extern int GetTextExtentPoint32(IntPtr hdc, [MarshalAs(UnmanagedType.LPWStr)] string str, int len, ref Size size);
        [DllImport("gdi32.dll", EntryPoint = "GetTextExtentPoint32W")]
        public static unsafe extern int UnsafeGetTextExtentPoint32(
            IntPtr hdc, char* str, int len, ref Size size);
        [DllImport("gdi32.dll", EntryPoint = "GetTextExtentExPointW")]
        public static extern bool GetTextExtentExPoint(IntPtr hDc, [MarshalAs(UnmanagedType.LPWStr)]string str, int nLength, int nMaxExtent, int[] lpnFit, int[] alpDx, ref Size size);
        [DllImport("gdi32.dll", EntryPoint = "GetTextExtentExPointW")]
        public static unsafe extern bool UnsafeGetTextExtentExPoint(
            IntPtr hDc, char* str, int len, int nMaxExtent, int[] lpnFit, int[] alpDx, ref Size size);
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

#if DEBUG
        public static void dbugDrawTextOrigin(IntPtr hdc, int x, int y)
        {
            MyWin32.Rectangle(hdc, x, y, x + 20, y + 20);
            MyWin32.MoveToEx(hdc, x, y, 0);
            MyWin32.LineTo(hdc, x + 20, y + 20);
            MyWin32.MoveToEx(hdc, x, y + 20, 0);
            MyWin32.LineTo(hdc, x + 20, y);
        }
#endif


    }
}
