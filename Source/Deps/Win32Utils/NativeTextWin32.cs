//MIT, 2014-2017, WinterDev

using System;
using System.Runtime.InteropServices;
using PixelFarm.Drawing;
namespace Win32
{
    public class NativeTextWin32
    {

        const string GDI32 = "gdi32.dll";
        [DllImport(GDI32, CharSet = CharSet.Unicode)]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart,
            [MarshalAs(UnmanagedType.LPWStr)]string charBuffer, int cbstring);
        [DllImport(GDI32, CharSet = CharSet.Unicode)]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, char[] charBuffer, int cbstring);
        [DllImport(GDI32, EntryPoint = "TextOutW")]
        public static unsafe extern bool TextOutUnsafe(IntPtr hdc, int x, int y, char* s, int len);
        [DllImport(GDI32)]
        public static unsafe extern bool ExtTextOut(IntPtr hdc, int x, int y, uint fuOptions,
            Rectangle* lpRect, char[] charBuffer, int cbCount, object arrayOfSpaceValues);
        [DllImport(GDI32, CharSet = CharSet.Unicode)]
        public static extern bool GetTextExtentPoint32(IntPtr hdc, char[] charBuffer, int c, out WIN32SIZE size);
        [DllImport(GDI32, EntryPoint = "GetTextExtentPoint32", CharSet = CharSet.Unicode)]
        public static unsafe extern bool GetTextExtentPoint32Char(IntPtr hdc, char* ch, int c, out WIN32SIZE size);
        public const int ETO_OPAQUE = 0x0002;
        public const int ETO_CLIPPED = 0x0004;
        [DllImport(GDI32, EntryPoint = "GetTextExtentPoint32W", CharSet = CharSet.Unicode)]
        public static extern int GetTextExtentPoint32(IntPtr hdc, [MarshalAs(UnmanagedType.LPWStr)] string str, int len, ref Size size);
        [DllImport(GDI32, EntryPoint = "GetTextExtentPoint32W", CharSet = CharSet.Unicode)]
        public static unsafe extern int UnsafeGetTextExtentPoint32(
            IntPtr hdc, char* str, int len, ref Size size);
        [DllImport(GDI32, EntryPoint = "GetTextExtentExPointW", CharSet = CharSet.Unicode)]
        public static extern bool GetTextExtentExPoint(IntPtr hDc, [MarshalAs(UnmanagedType.LPWStr)]string str, int nLength, int nMaxExtent, int[] lpnFit, int[] alpDx, ref Size size);
        [DllImport(GDI32, EntryPoint = "GetTextExtentExPointW", CharSet = CharSet.Unicode)]
        public static unsafe extern bool UnsafeGetTextExtentExPoint(
            IntPtr hDc, char* str, int len, int nMaxExtent, int[] lpnFit, int[] alpDx, ref Size size);
        /// <summary>
        /// translates a string into an array of glyph indices. The function can be used to determine whether a glyph exists in a font.
        /// This function attempts to identify a single-glyph representation for each character in the string pointed to by lpstr. 
        /// While this is useful for certain low-level purposes (such as manipulating font files), higher-level applications that wish to map a string to glyphs will typically wish to use the Uniscribe functions.
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="text"></param>
        /// <param name="c">The length of both the length of the string pointed to by lpstr and the size (in WORDs) of the buffer pointed to by pgi.</param>
        /// <param name="buffer">This buffer must be of dimension c. On successful return, contains an array of glyph indices corresponding to the characters in the string</param>
        /// <param name="fl">(0 | GGI_MARK_NONEXISTING_GLYPHS) Specifies how glyphs should be handled if they are not supported. This parameter can be the following value.</param>
        /// <returns>If the function succeeds, it returns the number of bytes (for the ANSI function) or WORDs (for the Unicode function) converted.</returns>
        [DllImport(GDI32, CharSet = CharSet.Unicode)]
        public static extern unsafe int GetGlyphIndices(IntPtr hdc, char* text, int c, ushort* glyIndexBuffer, int fl);

        [DllImport(GDI32)]
        public static unsafe extern int GetCharABCWidths(IntPtr hdc, uint uFirstChar, uint uLastChar, void* lpabc);
        [DllImport(GDI32)]
        public static unsafe extern int GetCharABCWidthsFloat(IntPtr hdc, uint uFirstChar, uint uLastChar, void* lpabc);
        [DllImport(GDI32)]
        public static unsafe extern int GetOutlineTextMetrics(IntPtr hdc, uint cbData, uint uLastChar, void* lp_outlineTextMatrix);

        public const int GGI_MARK_NONEXISTING_GLYPHS = 0X0001;

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
        public struct ABCFloat
        {
            /// <summary>Specifies the A spacing of the character. The A spacing is the distance to add to the current
            /// position before drawing the character glyph.</summary>
            public float abcfA;
            /// <summary>Specifies the B spacing of the character. The B spacing is the width of the drawn portion of
            /// the character glyph.</summary>
            public float abcfB;
            /// <summary>Specifies the C spacing of the character. The C spacing is the distance to add to the current
            /// position to provide white space to the right of the character glyph.</summary>
            public float abcfC;
        }
        [DllImport(GDI32, CharSet = CharSet.Unicode)]
        public static unsafe extern int
            GetCharacterPlacement(IntPtr hdc, char* str, int nCount,
            int nMaxExtent, ref GCP_RESULTS lpResults, int dwFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct GCP_RESULTS
        {
            public int lStructSize;
            public char* lpOutString;
            public uint* lpOrder;
            public int* lpDx;
            public int* lpCaretPos;
            public char* lpClass;
            public char* lpGlyphs;
            public uint nGlyphs;
            public int nMaxFit;
        }
        //            DWORD GetCharacterPlacement(
        //  _In_    HDC           hdc,
        //  _In_    LPCTSTR       lpString,
        //  _In_    int           nCount,
        //  _In_    int           nMaxExtent,
        //  _Inout_ LPGCP_RESULTS lpResults,
        //  _In_    DWORD         dwFlags
        //);

        //    typedef struct _OUTLINETEXTMETRICW {
        //UINT    otmSize;
        //TEXTMETRICW otmTextMetrics;
        //BYTE    otmFiller;
        //PANOSE  otmPanoseNumber;
        //UINT    otmfsSelection;
        //UINT    otmfsType;
        // int    otmsCharSlopeRise;
        // int    otmsCharSlopeRun;
        // int    otmItalicAngle;
        //UINT    otmEMSquare;
        // int    otmAscent;
        // int    otmDescent;
        //UINT    otmLineGap;
        //UINT    otmsCapEmHeight;
        //UINT    otmsXHeight;
        //RECT    otmrcFontBox;
        // int    otmMacAscent;
        // int    otmMacDescent;
        //UINT    otmMacLineGap;
        //UINT    otmusMinimumPPEM;
        //POINT   otmptSubscriptSize;
        //POINT   otmptSubscriptOffset;
        //POINT   otmptSuperscriptSize;
        //POINT   otmptSuperscriptOffset;
        //UINT    otmsStrikeoutSize;
        // int    otmsStrikeoutPosition;
        // int    otmsUnderscoreSize;
        // int    otmsUnderscorePosition;
        //PSTR    otmpFamilyName;
        //PSTR    otmpFaceName;
        //PSTR    otmpStyleName;
        //PSTR    otmpFullName;
        //}
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PANOSE
        {
            byte bFamilyType;
            byte bSerifStyle;
            byte bWeight;
            byte bProportion;
            byte bContrast;
            byte bStrokeVariation;
            byte bArmStyle;
            byte bLetterform;
            byte bMidline;
            byte bXHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct _OUTLINETEXTMETRICW
        {
            uint otmSize;
            TEXTMETRICW otmTextMetrics;
            byte otmFiller;
            PANOSE otmPanoseNumber;
            uint otmfsSelection;
            uint otmfsType;
            int otmsCharSlopeRise;
            int otmsCharSlopeRun;
            int otmItalicAngle;
            uint otmEMSquare;
            int otmAscent;
            int otmDescent;
            uint otmLineGap;
            uint otmsCapEmHeight;
            uint otmsXHeight;
            RECT otmrcFontBox;
            int otmMacAscent;
            int otmMacDescent;
            uint otmMacLineGap;
            uint otmusMinimumPPEM;
            POINT otmptSubscriptSize;
            POINT otmptSubscriptOffset;
            POINT otmptSuperscriptSize;
            POINT otmptSuperscriptOffset;
            uint otmsStrikeoutSize;
            int otmsStrikeoutPosition;
            int otmsUnderscoreSize;
            int otmsUnderscorePosition;
            char* otmpFamilyName;
            char* otmpFaceName;
            char* otmpStyleName;
            char* otmpFullName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TEXTMETRICW
        {
            int tmHeight;
            int tmAscent;
            int tmDescent;
            int tmInternalLeading;
            int tmExternalLeading;
            int tmAveCharWidth;
            int tmMaxCharWidth;
            int tmWeight;
            int tmOverhang;
            int tmDigitizedAspectX;
            int tmDigitizedAspectY;
            char tmFirstChar;
            char tmLastChar;
            char tmDefaultChar;
            char tmBreakChar;
            byte tmItalic;
            byte tmUnderlined;
            byte tmStruckOut;
            byte tmPitchAndFamily;
            byte tmCharSet;
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
