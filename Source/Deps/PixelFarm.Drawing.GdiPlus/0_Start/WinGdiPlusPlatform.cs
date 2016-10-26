//BSD, 2014-2016, WinterDev 
using System;
using Win32;
using System.Runtime.InteropServices;

namespace PixelFarm.Drawing.WinGdi
{
    public class WinGdiPlusPlatform : GraphicsPlatform
    {

        GdiPlusIFonts ifonts = new GdiPlusIFonts();
        public WinGdiPlusPlatform()
        {
            PixelFarm.Agg.AggBuffMx.SetNaiveBufferImpl(new Win32AggBuffMx());

        }
        ~WinGdiPlusPlatform()
        {

        }
        public override Canvas CreateCanvas(int left, int top, int width, int height, CanvasInitParameters canvasInitPars)
        {
            return new MyGdiPlusCanvas(this, 0, 0, left, top, width, height);
        }
        
        public override GraphicsPath CreateGraphicsPath()
        {
            return new WinGdiGraphicsPath();
        }

        public override IFonts Fonts
        {
            get
            {
                return ifonts;
            }
        }
        public override Bitmap CreatePlatformBitmap(int w, int h, byte[] rawBuffer, bool isBottomUp)
        {
            //create platform bitmap
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            CopyFromAggActualImageToGdiPlusBitmap(rawBuffer, bmp);
            if (isBottomUp)
            {
                bmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            }
            return new Bitmap(w, h, bmp);
        }
        static void CopyFromAggActualImageToGdiPlusBitmap(byte[] rawBuffer, System.Drawing.Bitmap bitmap)
        {
            //platform specific
            var bmpdata = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                 System.Drawing.Imaging.ImageLockMode.ReadOnly,
                 System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(rawBuffer, 0,
                bmpdata.Scan0, rawBuffer.Length);
            bitmap.UnlockBits(bmpdata);
        }
    }

    class GdiPlusIFonts : IFonts
    {
        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(2, 2);
        NativeWin32MemoryDc win32MemDc;
        WinGdiPlusFontStore fontStore = new WinGdiPlusFontStore();

        //=====================================
        //static 
        static readonly int[] _charFit = new int[1];
        static readonly int[] _charFitWidth = new int[1000];

        public GdiPlusIFonts()
        {
            win32MemDc = new NativeWin32MemoryDc(2, 2);
        }
        public float MeasureWhitespace(RequestFont f)
        {
            return fontStore.MeasureWhitespace(this, f);
        }
        void SetFont(RequestFont font)
        {
            WinGdiPlusFont winFont = fontStore.ResolveFont(font);
            Win32Utils.SelectObject(win32MemDc.DC, winFont.ToHfont());
        }
        public PixelFarm.Drawing.Fonts.ActualFont ResolveActualFont(RequestFont f)
        {
            return fontStore.ResolveFont(f);
        }
        public Size MeasureString(char[] buff, int startAt, int len, RequestFont font)
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
            if (buff.Length > 0)
            {
                unsafe
                {
                    fixed (char* startAddr = &buff[0])
                    {
                        NativeTextWin32.UnsafeGetTextExtentPoint32(win32MemDc.DC, startAddr + startAt, len, ref size);
                    }
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
        public Size MeasureString(char[] buff, int startAt, int len, RequestFont font, float maxWidth, out int charFit, out int charFitWidth)
        {
            //if (_useGdiPlusTextRendering)
            //{
            //    ReleaseHdc();
            //    throw new NotSupportedException("Char fit string measuring is not supported for GDI+ text rendering");
            //}
            //else
            //{
            SetFont(font);
            if (buff.Length == 0)
            {
                charFit = 0;
                charFitWidth = 0;
                return Size.Empty;
            }
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
            //}
        }
        //==============================================


        public void Dispose()
        {
            if (bmp != null)
            {
                bmp.Dispose();
                bmp = null;
            }

            win32MemDc.Dispose();
            win32MemDc = null;
        }
    }


    class Win32AggBuffMx : PixelFarm.Agg.AggBuffMx
    {

        protected override void InnerMemCopy(byte[] dest_buffer, int dest_startAt, byte[] src_buffer, int src_StartAt, int len)
        {
            unsafe
            {
                fixed (byte* head_dest = &dest_buffer[dest_startAt])
                fixed (byte* head_src = &src_buffer[src_StartAt])
                {
                    memcpy(head_dest, head_src, len);
                }
            }
        }
        protected override void InnerMemSet(byte[] dest, int startAt, byte value, int count)
        {
            unsafe
            {
                fixed (byte* head = &dest[0])
                {
                    memset(head, 0, 100);
                }
            }
        }
        //this is platform specific ***
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void memset(byte* dest, byte c, int byteCount);
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void memcpy(byte* dest, byte* src, int byteCount);
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern int memcmp(byte* dest, byte* src, int byteCount);
    }
}