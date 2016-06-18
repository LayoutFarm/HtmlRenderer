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
namespace PixelFarm.Drawing.WinGdi
{
    partial class MyGdiPlusCanvas : Canvas, IFonts, IDisposable
    {
        bool isDisposed;
        System.Drawing.Graphics gx;
        Stack<System.Drawing.Rectangle> clipRectStack = new Stack<System.Drawing.Rectangle>();
        //-------------------------------

        System.Drawing.Color currentTextColor = System.Drawing.Color.Black;
        System.Drawing.Pen internalPen;
        System.Drawing.SolidBrush internalSolidBrush;
        System.Drawing.Rectangle currentClipRect;
        //-------------------------------
        System.Drawing.Graphics targetGfx;
        GraphicsPlatform platform;
        public MyGdiPlusCanvas(GraphicsPlatform platform,
            System.Drawing.Graphics targetGfx,
            int left, int top,
            int width,
            int height)
        {
            //platform specific Win32
            //1.
            this.platform = platform;
            this.targetGfx = this.gx = targetGfx;
            //2. dimension
            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;
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
            clipRectStack.Clear();
            currentClipRect = new System.Drawing.Rectangle(0, 0, this.Width, this.Height);
#if DEBUG

            debug_releaseCount++;
#endif
        }

        int CanvasOrgX { get { return (int)this.canvasOriginX; } }
        int CanvasOrgY { get { return (int)this.canvasOriginY; } }



        ///// <summary>
        ///// Special draw logic to draw transparent text using GDI.<br/>
        ///// 1. Create in-memory DC<br/>
        ///// 2. Copy background to in-memory DC<br/>
        ///// 3. Draw the text to in-memory DC<br/>
        ///// 4. Copy the in-memory DC to the proper location with alpha blend<br/>
        ///// </summary>
        //static void DrawTransparentText(IntPtr hdc, string str, Font font, Point point, Size size, Color color)
        //{
        //    IntPtr dib;
        //    var memoryHdc = Win32Utils.CreateMemoryHdc(hdc, size.Width, size.Height, out dib);

        //    try
        //    {
        //        // copy target background to memory HDC so when copied back it will have the proper background
        //        Win32Utils.BitBlt(memoryHdc, 0, 0, size.Width, size.Height, hdc, point.X, point.Y, Win32Utils.BitBltCopy);

        //        // Create and select font
        //        Win32Utils.SelectObject(memoryHdc, FontStore.GetCachedHFont(font.InnerFont as System.Drawing.Font));
        //        Win32Utils.SetTextColor(memoryHdc, (color.B & 0xFF) << 16 | (color.G & 0xFF) << 8 | color.R);

        //        // Draw text to memory HDC
        //        Win32Utils.TextOut(memoryHdc, 0, 0, str, str.Length);

        //        // copy from memory HDC to normal HDC with alpha blend so achieve the transparent text
        //        Win32Utils.AlphaBlend(hdc, point.X, point.Y, size.Width, size.Height, memoryHdc, 0, 0, size.Width, size.Height, new BlendFunction(color.A));
        //    }
        //    finally
        //    {
        //        Win32Utils.ReleaseMemoryHdc(memoryHdc, dib);
        //    }
        //}



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
        static System.Drawing.Font defaultGdiFont;
        static IntPtr defaultHFont;
        Font defaultFont;
        static MyGdiPlusCanvas()
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