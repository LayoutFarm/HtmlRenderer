//2014,2015 BSD, WinterDev
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
using Win32;

namespace LayoutFarm.Drawing.WinGdi
{

    partial class MyCanvas : Canvas, IFonts
    {
        int pageNumFlags;
        int pageFlags;
        bool isDisposed;

        System.Drawing.Graphics gx;
        IntPtr hRgn = IntPtr.Zero;
        IntPtr _hdc;
        IntPtr hbmp;
        IntPtr hFont = IntPtr.Zero;
        IntPtr originalHdc = IntPtr.Zero;
        Stack<System.Drawing.Rectangle> clipRectStack = new Stack<System.Drawing.Rectangle>();
        //-------------------------------
        System.Drawing.Color currentTextColor = System.Drawing.Color.Black;
        System.Drawing.Pen internalPen;
        System.Drawing.SolidBrush internalSolidBrush;
        System.Drawing.Rectangle currentClipRect;
        //-------------------------------
        bool isFromPrinter = false;
        GraphicsPlatform platform;
        public MyCanvas(GraphicsPlatform platform,
            int horizontalPageNum,
            int verticalPageNum,
            int left, int top,
            int width,
            int height)
        {

            this.platform = platform;

            this.pageNumFlags = (horizontalPageNum << 8) | verticalPageNum;

            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;


            internalPen = new System.Drawing.Pen(System.Drawing.Color.Black);
            internalSolidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            //-------------------------------------------------------
            originalHdc = MyWin32.CreateCompatibleDC(IntPtr.Zero);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            hbmp = bmp.GetHbitmap();
            MyWin32.SelectObject(originalHdc, hbmp);
            MyWin32.PatBlt(originalHdc, 0, 0, width, height, MyWin32.WHITENESS);
            MyWin32.SetBkMode(originalHdc, MyWin32._SetBkMode_TRANSPARENT);
            hFont = MyWin32.SelectObject(originalHdc, hFont);
            currentClipRect = new System.Drawing.Rectangle(0, 0, width, height);
            hRgn = MyWin32.CreateRectRgn(0, 0, width, height);
            MyWin32.SelectObject(originalHdc, hRgn);
            gx = System.Drawing.Graphics.FromHdc(originalHdc);
            //-------------------------------------------------------

            //-------------------------------------------------------
            var fontInfo = platform.GetFont("tahoma", 10, FontStyle.Regular);
            defaultFont = fontInfo.ResolvedFont;
            this.CurrentFont = defaultFont;
            this.CurrentTextColor = Color.Black;
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif      
            this.StrokeWidth = 1;
        }

        //~MyCanvas()
        //{
        //    Dispose();
        //}
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            ReleaseHdc();
            ReleaseUnManagedResource();
        }

        void ClearPreviousStoredValues()
        {
            this.gx.RenderingOrigin = new System.Drawing.Point(0, 0);
            this.canvasOriginX = 0;
            this.canvasOriginY = 0;
            this.clipRectStack.Clear();
        }

        public void ReleaseUnManagedResource()
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

            originalHdc = MyWin32.CreateCompatibleDC(IntPtr.Zero);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            hbmp = bmp.GetHbitmap();
            MyWin32.SelectObject(originalHdc, hbmp);
            MyWin32.PatBlt(originalHdc, 0, 0, newWidth, newHeight, MyWin32.WHITENESS);
            MyWin32.SetBkMode(originalHdc, MyWin32._SetBkMode_TRANSPARENT);


            hFont = defaultHFont;

            MyWin32.SelectObject(originalHdc, hFont);
            currentClipRect = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
            MyWin32.SelectObject(originalHdc, hRgn);
            gx = System.Drawing.Graphics.FromHdc(originalHdc);

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
            
            Win32Utils.SelectObject(_hdc, FontStore.GetCachedHFont(font.InnerFont as System.Drawing.Font));
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
                Win32Utils.SelectObject(memoryHdc, FontStore.GetCachedHFont(font.InnerFont as System.Drawing.Font));
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

        static MyCanvas()
        {
            _stringFormat = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericDefault);
            _stringFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip | System.Drawing.StringFormatFlags.MeasureTrailingSpaces;
            //---------------------------
            defaultGdiFont = new System.Drawing.Font("tahoma", 10);
            defaultHFont = defaultGdiFont.ToHfont();

        }


        static System.Drawing.Point[] ConvPointArray(Point[] points)
        {
            int j = points.Length;
            System.Drawing.Point[] outputPoints = new System.Drawing.Point[j];
            for (int i = 0; i < j; ++i)
            {
                outputPoints[i] = points[i].ToPoint();
            }
            return outputPoints;
        }
        static System.Drawing.PointF[] ConvPointFArray(PointF[] points)
        {
            int j = points.Length;
            System.Drawing.PointF[] outputPoints = new System.Drawing.PointF[j];
            for (int i = 0; i < j; ++i)
            {
                outputPoints[i] = points[i].ToPointF();
            }
            return outputPoints;
        }

        static System.Drawing.Color ConvColor(Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }


        static System.Drawing.Bitmap ConvBitmap(Bitmap bmp)
        {
            return bmp.InnerImage as System.Drawing.Bitmap;
        }
        static System.Drawing.Image ConvBitmap(Image img)
        {
            return img.InnerImage as System.Drawing.Image;
        }
        static System.Drawing.Drawing2D.GraphicsPath ConvPath(GraphicsPath p)
        {
            return p.InnerPath as System.Drawing.Drawing2D.GraphicsPath;
        }
        static System.Drawing.Font ConvFont(Font f)
        {
            return f.InnerFont as System.Drawing.Font;
        }
        static System.Drawing.Region ConvRgn(Region rgn)
        {
            return rgn.InnerRegion as System.Drawing.Region;
        }
        static System.Drawing.Drawing2D.GraphicsPath ConvFont(GraphicsPath p)
        {
            return p.InnerPath as System.Drawing.Drawing2D.GraphicsPath;
        }
        //=========================================================





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