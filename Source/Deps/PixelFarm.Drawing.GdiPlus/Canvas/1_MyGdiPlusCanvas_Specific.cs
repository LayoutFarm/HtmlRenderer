//BSD, 2014-2017, WinterDev
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
    public partial class MyGdiPlusCanvas : Canvas, IDisposable
    {
        int pageNumFlags;
        int pageFlags;
        bool isDisposed;
        //-------------------------------
        NativeWin32MemoryDc win32MemDc;
        //-------------------------------

        IntPtr originalHdc = IntPtr.Zero;
        System.Drawing.Graphics gx;

        //-------------------------------
        Stack<System.Drawing.Rectangle> clipRectStack = new Stack<System.Drawing.Rectangle>();
        //-------------------------------

        System.Drawing.Color currentTextColor = System.Drawing.Color.Black;
        System.Drawing.Pen internalPen;
        System.Drawing.SolidBrush internalSolidBrush;
        System.Drawing.Rectangle currentClipRect;
        //-------------------------------

        public MyGdiPlusCanvas(int left, int top, int width, int height)
            : this(0, 0, left, top, width, height)
        {
        }
        internal MyGdiPlusCanvas(
            int horizontalPageNum,
            int verticalPageNum,
            int left, int top,
            int width,
            int height)
        {


#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif

            this.pageNumFlags = (horizontalPageNum << 8) | verticalPageNum;
            //2. dimension
            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;
            currentClipRect = new System.Drawing.Rectangle(0, 0, width, height);

            CreateGraphicsFromNativeHdc(width, height);
            this.gx = System.Drawing.Graphics.FromHdc(win32MemDc.DC);
            //-------------------------------------------------------     
            //managed object
            internalPen = new System.Drawing.Pen(System.Drawing.Color.Black);
            internalSolidBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            this.StrokeWidth = 1;
        }
        void CreateGraphicsFromNativeHdc(int width, int height)
        {
            win32MemDc = new NativeWin32MemoryDc(width, height, true);
            win32MemDc.PatBlt(NativeWin32MemoryDc.PatBltColor.White);
            win32MemDc.SetBackTransparent(true);
            win32MemDc.SetClipRect(0, 0, width, height);

            this.originalHdc = win32MemDc.DC;
            //--------------
            //set default font and default text color
            this.CurrentFont = new RequestFont("tahoma", 14);
            this.CurrentTextColor = Color.Black;
            //--------------

        }
#if DEBUG
        public override string ToString()
        {
            return "visible_clip" + this.gx.VisibleClipBounds.ToString();
        }
#endif

        public override void CloseCanvas()
        {
            if (isDisposed)
            {
                return;
            }
            if (win32MemDc != null)
            {
                win32MemDc.Dispose();
                win32MemDc = null;
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
        void ClearPreviousStoredValues()
        {
            this.gx.RenderingOrigin = new System.Drawing.Point(0, 0);
            this.canvasOriginX = 0;
            this.canvasOriginY = 0;
            this.clipRectStack.Clear();
        }

        void ReleaseUnManagedResource()
        {
            if (win32MemDc != null)
            {
                win32MemDc.Dispose();
                win32MemDc = null;
                originalHdc = IntPtr.Zero;
            }

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
            win32MemDc.PatBlt(NativeWin32MemoryDc.PatBltColor.White);
            win32MemDc.SetClipRect(0, 0, w, h);
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

            currentClipRect = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
            CreateGraphicsFromNativeHdc(newWidth, newHeight);
            this.gx = System.Drawing.Graphics.FromHdc(win32MemDc.DC);


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

        const int CANVAS_UNUSED = 1 << (1 - 1);
        const int CANVAS_DIMEN_CHANGED = 1 << (2 - 1);

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
        public static int dbugDrawStringCount;


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