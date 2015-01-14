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
using PixelFarm.Drawing;
using PixelFarm.DrawingGL;

using Win32;

namespace PixelFarm.Drawing.DrawingGL
{

    partial class MyCanvasGL : Canvas, IFonts, IDisposable
    {


        GraphicsPlatform platform;
        int pageFlags;
        Font currentFont;
        CanvasGL2d canvasGL2d;
        PixelFarm.Agg.VertexSource.CurveFlattener flattener = new PixelFarm.Agg.VertexSource.CurveFlattener();
        //-------
        Stack<System.Drawing.Rectangle> clipRectStack = new Stack<System.Drawing.Rectangle>();
        System.Drawing.Rectangle currentClipRect;

        public MyCanvasGL(GraphicsPlatform platform, int hPageNum, int vPageNum, int left, int top, int width, int height)
        {
            canvasGL2d = new CanvasGL2d(width, height);
            //--------------------------------------------
            this.platform = platform;
            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;
            //--------------------------------------------

            this.CurrentFont = defaultFontInfo.ResolvedFont;
            this.CurrentTextColor = Color.Black;
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif
            this.StrokeWidth = 1;
            this.currentClipRect = new System.Drawing.Rectangle(0, 0, width, height);


        }
        //-------------------------------------------
        bool isDisposed;

        ~MyCanvasGL()
        {
            CloseCanvas();
        }
        public override void CloseCanvas()
        {
            if (isDisposed)
            {
                return;
            }
            this.isDisposed = true;
            this.canvasGL2d.Dispose();
            ReleaseUnManagedResource();
            this.canvasGL2d = null;
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
            //this.gx.RenderingOrigin = new System.Drawing.Point(0, 0);
            //this.canvasOriginX = 0;
            //this.canvasOriginY = 0;
            this.clipRectStack.Clear();
        }

        public void ReleaseUnManagedResource()
        {

            //if (hRgn != IntPtr.Zero)
            //{
            //    MyWin32.DeleteObject(hRgn);
            //    hRgn = IntPtr.Zero;
            //}

            //MyWin32.DeleteDC(originalHdc);
            //originalHdc = IntPtr.Zero;
            //MyWin32.DeleteObject(hbmp);
            //hbmp = IntPtr.Zero;

            clipRectStack.Clear();

            //currentClipRect = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);



#if DEBUG

            debug_releaseCount++;
#endif
        }


        public void Reset(int hPageNum, int vPageNum, int newWidth, int newHeight)
        {

            this.ReleaseUnManagedResource();
            this.ClearPreviousStoredValues();

            //originalHdc = MyWin32.CreateCompatibleDC(IntPtr.Zero);
            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //hbmp = bmp.GetHbitmap();
            //MyWin32.SelectObject(originalHdc, hbmp);
            //MyWin32.PatBlt(originalHdc, 0, 0, newWidth, newHeight, MyWin32.WHITENESS);
            //MyWin32.SetBkMode(originalHdc, MyWin32._SetBkMode_TRANSPARENT); 
            //hFont = defaultHFont;

            //MyWin32.SelectObject(originalHdc, hFont);
            //currentClipRect = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
            //MyWin32.SelectObject(originalHdc, hRgn);
            //gx = System.Drawing.Graphics.FromHdc(originalHdc); 
            //gx.Clear(System.Drawing.Color.White);
            //MyWin32.SetRectRgn(hRgn, 0, 0, newWidth, newHeight);


            left = hPageNum * newWidth;
            top = vPageNum * newHeight;
            right = left + newWidth;
            bottom = top + newHeight;
#if DEBUG
            debug_resetCount++;
#endif
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
        //int CanvasOrgX { get { return (int)this.canvasOriginX; } }
        //int CanvasOrgY { get { return (int)this.canvasOriginY; } }
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

        }

        /// <summary>
        /// Release current HDC to be able to use <see cref="Graphics"/> methods.
        /// </summary>
        void ReleaseHdc()
        {
            //if (_hdc != IntPtr.Zero)
            //{
            //    Win32Utils.SelectClipRgn(_hdc, IntPtr.Zero);
            //    gx.ReleaseHdc(_hdc);
            //    _hdc = IntPtr.Zero;
            //}
        }

        /// <summary>
        /// Set a resource (e.g. a font) for the specified device context.
        /// WARNING: Calling Font.ToHfont() many times without releasing the font handle crashes the app.
        /// </summary>
        void SetFont(Font font)
        {
            throw new NotImplementedException();
            //InitHdc();
            //Win32Utils.SelectObject(_hdc, FontsUtils.GetCachedHFont(font.InnerFont as System.Drawing.Font));
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
            throw new NotImplementedException();
            //IntPtr dib;
            //var memoryHdc = Win32Utils.CreateMemoryHdc(hdc, size.Width, size.Height, out dib);

            //try
            //{
            //    // copy target background to memory HDC so when copied back it will have the proper background
            //    Win32Utils.BitBlt(memoryHdc, 0, 0, size.Width, size.Height, hdc, point.X, point.Y, Win32Utils.BitBltCopy);

            //    // Create and select font
            //    Win32Utils.SelectObject(memoryHdc, FontsUtils.GetCachedHFont(font.InnerFont as System.Drawing.Font));
            //    Win32Utils.SetTextColor(memoryHdc, (color.B & 0xFF) << 16 | (color.G & 0xFF) << 8 | color.R);

            //    // Draw text to memory HDC
            //    Win32Utils.TextOut(memoryHdc, 0, 0, str, str.Length);

            //    // copy from memory HDC to normal HDC with alpha blend so achieve the transparent text
            //    Win32Utils.AlphaBlend(hdc, point.X, point.Y, size.Width, size.Height, memoryHdc, 0, 0, size.Width, size.Height, new BlendFunction(color.A));
            //}
            //finally
            //{
            //    Win32Utils.ReleaseMemoryHdc(memoryHdc, dib);
            //}
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

        static FontInfo defaultFontInfo;

        static MyCanvasGL()
        {
            _stringFormat = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericDefault);
            _stringFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip | System.Drawing.StringFormatFlags.MeasureTrailingSpaces;
            //---------------------------
            defaultFontInfo = CanvasGLPlatform.PlatformGetFont("Tahoma", 10, FontLoadTechnique.GdiBitmapFont);
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