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
using System.Collections.Generic;
using Win32;
namespace PixelFarm.Drawing.WinGdi
{
    partial class MyScreenCanvas : Canvas, IFonts, IDisposable
    {
        int pageNumFlags;
        int pageFlags;
        bool isDisposed;
        IntPtr originalHdc = IntPtr.Zero;
        System.Drawing.Graphics gx;
        IntPtr hRgn = IntPtr.Zero;
        IntPtr tempDc;//temp dc from gx
        IntPtr hbmp;
        IntPtr hFont = IntPtr.Zero;
        Stack<System.Drawing.Rectangle> clipRectStack = new Stack<System.Drawing.Rectangle>();
        //-------------------------------
        System.Drawing.Bitmap bgBmp;
        System.Drawing.Color currentTextColor = System.Drawing.Color.Black;
        System.Drawing.Pen internalPen;
        System.Drawing.SolidBrush internalSolidBrush;
        System.Drawing.Rectangle currentClipRect;
        //-------------------------------

        GraphicsPlatform platform;
        public MyScreenCanvas(GraphicsPlatform platform,
            int horizontalPageNum,
            int verticalPageNum,
            int left, int top,
            int width,
            int height)
        {
            //platform specific Win32
            //1.
            this.platform = platform;
            this.pageNumFlags = (horizontalPageNum << 8) | verticalPageNum;
            //2. dimension
            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;
            CreateGraphicsFromNativeHdc(width, height);
            //-------------------------------------------------------
            currentClipRect = new System.Drawing.Rectangle(0, 0, width, height);
            var fontInfo = platform.GetFont("tahoma", 10, FontStyle.Regular);
            this.CurrentFont = defaultFont = fontInfo.ResolvedFont;
            this.CurrentTextColor = Color.Black;
            internalPen = new System.Drawing.Pen(System.Drawing.Color.Black);
            internalSolidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif
            this.StrokeWidth = 1;
        }
        void CreateGraphicsFromNativeHdc(int width, int height)
        {
            //3. create original dc from memory dc and prepare background
            var orgHdc = MyWin32.CreateCompatibleDC(IntPtr.Zero);
            bgBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            hbmp = bgBmp.GetHbitmap();
            MyWin32.SelectObject(orgHdc, hbmp);
            MyWin32.PatBlt(orgHdc, 0, 0, width, height, MyWin32.WHITENESS);
            MyWin32.SetBkMode(orgHdc, MyWin32._SetBkMode_TRANSPARENT);
            //4. font
            hFont = MyWin32.SelectObject(orgHdc, hFont);
            //5. region
            hRgn = MyWin32.CreateRectRgn(0, 0, width, height);
            MyWin32.SelectObject(orgHdc, hRgn);
            //6. create graphics from hdc***  

            gx = System.Drawing.Graphics.FromHdc(orgHdc);
            this.originalHdc = orgHdc;
        }
        public override string ToString()
        {
            return "visible_clip" + this.gx.VisibleClipBounds.ToString();
        }

        public override void CloseCanvas()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            ReleaseHdc();
            ReleaseUnManagedResource();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (isDisposed)
            {
                return;
            }
            this.CloseCanvas();
        }
        void IFonts.Dispose()
        {
            if (isDisposed)
            {
                return;
            }
            this.CloseCanvas();
        }

        void ClearPreviousStoredValues()
        {
            this.gx.RenderingOrigin = new System.Drawing.Point(0, 0);
            this.canvasOriginX = 0;
            this.canvasOriginY = 0;
            this.clipRectStack.Clear();
        }

        void ReleaseUnManagedResource()
        {
            if (hRgn != IntPtr.Zero)
            {
                MyWin32.DeleteObject(hRgn);
                hRgn = IntPtr.Zero;
            }

            MyWin32.DeleteDC(originalHdc);
            originalHdc = IntPtr.Zero;
            MyWin32.DeleteObject(hbmp);
            hbmp = IntPtr.Zero;
            clipRectStack.Clear();
            currentClipRect = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);
#if DEBUG

            debug_releaseCount++;
#endif
        }

        public void Reuse(int hPageNum, int vPageNum)
        {
            this.pageNumFlags = (hPageNum << 8) | vPageNum;
            int w = this.Width;
            int h = this.Height;
            this.ClearPreviousStoredValues();
            currentClipRect = new System.Drawing.Rectangle(0, 0, w, h);
            gx.Clear(System.Drawing.Color.White);
            MyWin32.SetRectRgn(hRgn, 0, 0, w, h);
            left = hPageNum * w;
            top = vPageNum * h;
            right = left + w;
            bottom = top + h;
        }
        public void Reset(int hPageNum, int vPageNum, int newWidth, int newHeight)
        {
            this.pageNumFlags = (hPageNum << 8) | vPageNum;
            this.ReleaseUnManagedResource();
            this.ClearPreviousStoredValues();
            var orgHdc = MyWin32.CreateCompatibleDC(IntPtr.Zero);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            hbmp = bmp.GetHbitmap();
            MyWin32.SelectObject(orgHdc, hbmp);
            MyWin32.PatBlt(orgHdc, 0, 0, newWidth, newHeight, MyWin32.WHITENESS);
            MyWin32.SetBkMode(orgHdc, MyWin32._SetBkMode_TRANSPARENT);
            hFont = defaultHFont;
            MyWin32.SelectObject(orgHdc, hFont);
            currentClipRect = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
            MyWin32.SelectObject(orgHdc, hRgn);
            gx = System.Drawing.Graphics.FromHdc(orgHdc);
            this.originalHdc = orgHdc;
            gx.Clear(System.Drawing.Color.White);
            MyWin32.SetRectRgn(hRgn, 0, 0, newWidth, newHeight);
            left = hPageNum * newWidth;
            top = vPageNum * newHeight;
            right = left + newWidth;
            bottom = top + newHeight;
#if DEBUG
            debug_resetCount++;
#endif
        }
        public bool IsPageNumber(int hPageNum, int vPageNum)
        {
            return pageNumFlags == ((hPageNum << 8) | vPageNum);
        }
        public bool IsUnused
        {
            get
            {
                return (pageFlags & CANVAS_UNUSED) != 0;
            }
            set
            {
                if (value)
                {
                    pageFlags |= CANVAS_UNUSED;
                }
                else
                {
                    pageFlags &= ~CANVAS_UNUSED;
                }
            }
        }
        int CanvasOrgX { get { return (int)this.canvasOriginX; } }
        int CanvasOrgY { get { return (int)this.canvasOriginY; } }
        public bool DimensionInvalid
        {
            get
            {
                return (pageFlags & CANVAS_DIMEN_CHANGED) != 0;
            }
            set
            {
                if (value)
                {
                    pageFlags |= CANVAS_DIMEN_CHANGED;
                }
                else
                {
                    pageFlags &= ~CANVAS_DIMEN_CHANGED;
                }
            }
        }
        /// <summary>
        /// Init HDC for the current graphics object to be used to call GDI directly.
        /// </summary>
        void InitHdc()
        {
            if (tempDc == IntPtr.Zero)
            {
                //var clip = _g.Clip.GetHrgn(_g);
                tempDc = gx.GetHdc();
                Win32Utils.SetBkMode(tempDc, 1);
                //Win32Utils.SelectClipRgn(_hdc, clip);
                //Win32Utils.DeleteObject(clip);
            }
        }

        /// <summary>
        /// Release current HDC to be able to use <see cref="Graphics"/> methods.
        /// </summary>
        void ReleaseHdc()
        {
            if (tempDc != IntPtr.Zero)
            {
                Win32Utils.SelectClipRgn(tempDc, IntPtr.Zero);
                gx.ReleaseHdc(tempDc);
                tempDc = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Set a resource (e.g. a font) for the specified device context.
        /// WARNING: Calling Font.ToHfont() many times without releasing the font handle crashes the app.
        /// </summary>
        void SetFont(Font font)
        {
            InitHdc();
            Win32Utils.SelectObject(tempDc, FontStore.GetCachedHFont(font.InnerFont as System.Drawing.Font));
        }

        /// <summary>
        /// Set the text color of the device context.
        /// </summary>
        void SetTextColor(Color color)
        {
            InitHdc();
            int rgb = (color.B & 0xFF) << 16 | (color.G & 0xFF) << 8 | color.R;
            Win32Utils.SetTextColor(tempDc, rgb);
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
                Win32Utils.SelectObject(memoryHdc, FontStore.GetCachedHFont(font.InnerFont as System.Drawing.Font));
                Win32Utils.SetTextColor(memoryHdc, (color.B & 0xFF) << 16 | (color.G & 0xFF) << 8 | color.R);
                // Draw text to memory HDC
                NativeTextWin32.TextOut(memoryHdc, 0, 0, str, str.Length);
                // copy from memory HDC to normal HDC with alpha blend so achieve the transparent text
                Win32Utils.AlphaBlend(hdc, point.X, point.Y, size.Width, size.Height, memoryHdc, 0, 0, size.Width, size.Height, new BlendFunction(color.A));
            }
            finally
            {
                Win32Utils.ReleaseMemoryHdc(memoryHdc, dib);
            }
        }



        //=====================================
        //static 
        static readonly int[] _charFit = new int[1];
        static readonly int[] _charFitWidth = new int[1000];
        /// <summary>
        /// Used for GDI+ measure string.
        /// </summary>
        static readonly System.Drawing.CharacterRange[] _characterRanges = new System.Drawing.CharacterRange[1];
        /// <summary>
        /// The string format to use for measuring strings for GDI+ text rendering
        /// </summary>
        static readonly System.Drawing.StringFormat _stringFormat;
        const int CANVAS_UNUSED = 1 << (1 - 1);
        const int CANVAS_DIMEN_CHANGED = 1 << (2 - 1);
        static IntPtr defaultHFont;
        Font defaultFont;
        static System.Drawing.Font defaultGdiFont;
        static MyScreenCanvas()
        {
            _stringFormat = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericDefault);
            _stringFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip | System.Drawing.StringFormatFlags.MeasureTrailingSpaces;
            //---------------------------
            defaultGdiFont = new System.Drawing.Font("tahoma", 10);
            defaultHFont = defaultGdiFont.ToHfont();
        }
        static System.Drawing.PointF[] ConvPointFArray(PointF[] points)
        {
            int j = points.Length;
            System.Drawing.PointF[] outputPoints = new System.Drawing.PointF[j];
            for (int i = j - 1; i >= 0; --i)
            {
                outputPoints[i] = points[i].ToPointF();
            }
            return outputPoints;
        }

        static System.Drawing.Color ConvColor(Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }



        //debug
#if DEBUG
        static class dbugCounter
        {
            public static int dbugDrawStringCount;
        }


        public override void dbug_DrawRuler(int x)
        {
            int canvas_top = this.top;
            int canvas_bottom = this.Bottom;
            for (int y = canvas_top; y < canvas_bottom; y += 10)
            {
                this.DrawText(y.ToString().ToCharArray(), x, y);
            }
        }
        public override void dbug_DrawCrossRect(Color color, Rectangle rect)
        {
            var prevColor = this.StrokeColor;
            this.StrokeColor = color;
            DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom);
            DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Top);
            this.StrokeColor = prevColor;
        }

#endif
        //public override bool PushClipAreaForNativeScrollableElement(Rect updateArea)
        //{

        //    clipRectStack.Push(currentClipRect);

        //    System.Drawing.Rectangle intersectResult = System.Drawing.Rectangle.Intersect(
        //        currentClipRect,
        //        updateArea.ToRectangle().ToRect());

        //    if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
        //    {
        //        currentClipRect = intersectResult;
        //        return false;
        //    }

        //    gx.SetClip(intersectResult);
        //    currentClipRect = intersectResult;
        //    return true;
        //}
    }
}