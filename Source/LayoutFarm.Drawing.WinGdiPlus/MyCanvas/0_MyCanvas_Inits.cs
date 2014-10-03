//2014 BSD, WinterDev
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


namespace LayoutFarm
{

    partial class MyCanvas : Canvas, IGraphics
    {
       
         
        int left;
        int top;
        int right;
        int bottom;
        int pageNumFlags;
        int pageFlags;

        System.Drawing.Graphics gx; 
        //-------------------------------
        int internalCanvasOriginX = 0;
        int internalCanvasOriginY = 0;
        float canvasOriginX = 0;
        float canvasOriginY = 0;
        //-------------------------------
        IntPtr hRgn = IntPtr.Zero;
        IntPtr _hdc;
        IntPtr hbmp;
        IntPtr hFont = IntPtr.Zero;
        IntPtr originalHdc = IntPtr.Zero;
        //-------------------------------
        Stack<int> prevWin32Colors = new Stack<int>();
        Stack<IntPtr> prevHFonts = new Stack<IntPtr>();
        Stack<TextFontInfo> prevFonts = new Stack<TextFontInfo>();
        Stack<System.Drawing.Color> prevColor = new Stack<System.Drawing.Color>();
        Stack<System.Drawing.Rectangle> prevRegionRects = new Stack<System.Drawing.Rectangle>();
        Stack<System.Drawing.Rectangle> clipRectStack = new Stack<System.Drawing.Rectangle>();
        //-------------------------------
        InternalRect invalidateArea = InternalRect.CreateFromLTRB(0, 0, 0, 0);
        TextFontInfo currentTextFont = null;
        SolidBrush sharedSolidBrush;  
        //-------------------------------
        System.Drawing.Color currentTextColor = System.Drawing.Color.Black;    
        System.Drawing.Pen internalPen;
        System.Drawing.SolidBrush internalBrush;
        System.Drawing.Rectangle currentClipRect;
        //-------------------------------
        
        bool _avoidGeometryAntialias;
        bool _avoidTextAntialias;
        bool _useGdiPlusTextRendering;
        bool isFromPrinter = false;
        
        

        int canvasFlags = FIRSTTIME_INVALID;

 
    

        public MyCanvas(int horizontalPageNum, int verticalPageNum, int left, int top, int width, int height)
        {

            this.pageNumFlags = (horizontalPageNum << 8) | verticalPageNum;

            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;


            internalPen = new System.Drawing.Pen(System.Drawing.Color.Black);
            internalBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            originalHdc = MyWin32.CreateCompatibleDC(IntPtr.Zero);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            hbmp = bmp.GetHbitmap();
            MyWin32.SelectObject(originalHdc, hbmp);
            MyWin32.PatBlt(originalHdc, 0, 0, width, height, MyWin32.WHITENESS);
            MyWin32.SetBkMode(originalHdc, MyWin32._SetBkMode_TRANSPARENT);
            hFont = FontManager.CurrentFont.ToHfont();
            MyWin32.SelectObject(originalHdc, hFont);

            currentClipRect = new System.Drawing.Rectangle(0, 0, width, height);
            hRgn = MyWin32.CreateRectRgn(0, 0, width, height);
            MyWin32.SelectObject(originalHdc, hRgn);

            gx = System.Drawing.Graphics.FromHdc(originalHdc);

            PushFontInfoAndTextColor(FontManager.DefaultTextFontInfo, Color.Black);
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif
        }
        ~MyCanvas()
        {
            ReleaseUnManagedResource();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ReleaseHdc();
        }
        void ClearPreviousStoredValues()
        {
            internalCanvasOriginX = 0;
            internalCanvasOriginY = 0;
            this.clipRectStack.Clear();
            this.prevHFonts.Clear();
            this.prevRegionRects.Clear();
            this.prevFonts.Clear();
            this.prevWin32Colors.Clear();
        }

        public override void ReleaseUnManagedResource()
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

            if (sharedSolidBrush != null)
            {
                sharedSolidBrush.Dispose();
            }

#if DEBUG

            debug_releaseCount++;
#endif
        }

        public override void Reuse(int hPageNum, int vPageNum)
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
        public override void Reset(int hPageNum, int vPageNum, int newWidth, int newHeight)
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
            hFont = FontManager.CurrentFont.ToHfont();
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


#if DEBUG
        static class dbugCounter
        {
            public static int dbugDrawStringCount;
        }
#endif
    }
}