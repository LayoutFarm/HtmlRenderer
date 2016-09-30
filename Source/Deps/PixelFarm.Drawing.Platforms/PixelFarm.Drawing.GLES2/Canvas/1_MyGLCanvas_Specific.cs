//BSD, 2014-2016, WinterDev
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
using PixelFarm.DrawingGL;
namespace PixelFarm.Drawing.GLES2
{
    partial class MyGLCanvas : Canvas, IFonts, IDisposable
    {
        CanvasGL2d canvasGL2;
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
        public MyGLCanvas(
            GraphicsPlatform platform,
            CanvasGL2d canvasGL2d,
            System.Drawing.Graphics targetGfx,
            int left, int top,
            int width,
            int height)
        {
            this.canvasGL2 = canvasGL2d;
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
            Font font = platform.GetFont("tahoma", 10, FontStyle.Regular);
            this.CurrentFont = defaultFont = font;
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
        static MyGLCanvas()
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