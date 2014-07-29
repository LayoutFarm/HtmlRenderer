//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;



namespace LayoutFarm.Presentation
{


    public sealed class ArtCanvas
    {
        private readonly static int[] _charFit = new int[1];

        private readonly static int[] _charFitWidth = new int[1000];

        bool _useGdiPlusTextRendering;



        int left; int top; int right; int bottom;

        int pageNumFlags;

        int pageFlags;

        const int CANVAS_UNUSED = 1 << (1 - 1);
        const int CANVAS_DIMEN_CHANGED = 1 << (2 - 1);
        IntPtr hFont = IntPtr.Zero;
        Graphics gx;
        IntPtr originalHdc = IntPtr.Zero;

        int internalCanvasOriginX = 0;
        int internalCanvasOriginY = 0;

        IntPtr hRgn = IntPtr.Zero;


        Stack<int> prevWin32Colors = new Stack<int>();


        Stack<IntPtr> prevHFonts = new Stack<IntPtr>();

        Stack<TextFontInfo> prevFonts = new Stack<TextFontInfo>();
        Stack<Color> prevColor = new Stack<Color>();


        Color currentTextColor = Color.Black; TextFontInfo currentTextFont = null;
        Stack<Rectangle> prevRegionRects = new Stack<Rectangle>();

        Pen internalPen;
        SolidBrush internalBrush;

        Rectangle currentClipRect;

        Stack<Rectangle> clipRectStack = new Stack<Rectangle>();

        IntPtr hbmp; bool _avoidGeometryAntialias;
        bool _avoidTextAntialias;
#if DEBUG
        static int dbug_canvasCount = 0;
        public int debug_resetCount = 0;
        public int debug_releaseCount = 0;
        public int debug_canvas_id = 0;
#endif
        bool isFromPrinter = false;
        SolidBrush sharedSolidBrush;

        public ArtCanvas(int horizontalPageNum, int verticalPageNum, int left, int top, int width, int height)
        {

            this.pageNumFlags = (horizontalPageNum << 8) | verticalPageNum;

            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;


            internalPen = new Pen(Color.Black);
            internalBrush = new SolidBrush(Color.Black);

            originalHdc = MyWin32.CreateCompatibleDC(IntPtr.Zero);
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            hbmp = bmp.GetHbitmap();
            MyWin32.SelectObject(originalHdc, hbmp);
            MyWin32.PatBlt(originalHdc, 0, 0, width, height, MyWin32.WHITENESS);
            MyWin32.SetBkMode(originalHdc, MyWin32._SetBkMode_TRANSPARENT);
            hFont = FontManager.CurrentFont.ToHfont();
            MyWin32.SelectObject(originalHdc, hFont);

            currentClipRect = new Rectangle(0, 0, width, height);
            hRgn = MyWin32.CreateRectRgn(0, 0, width, height);
            MyWin32.SelectObject(originalHdc, hRgn);

            gx = Graphics.FromHdc(originalHdc);

            PushFontInfoAndTextColor(FontManager.DefaultTextFontInfo, Color.Black);
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif
        }

        public ArtCanvas(Graphics gx, int verticalPageNum, int horizontalPageNum, int left, int top, int width, int height)
        {

            this.pageNumFlags = (horizontalPageNum << 8) | verticalPageNum;
            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;
            internalPen = new Pen(Color.Black);
            internalBrush = new SolidBrush(Color.Black);

            this.gx = gx;


            isFromPrinter = true;
            currentClipRect = new Rectangle(0, 0, width, height);

            PushFontInfoAndTextColor(FontManager.DefaultTextFontInfo, Color.Black);

#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif

        }

        public bool AvoidGeometryAntialias
        {
            get { return _avoidGeometryAntialias; }
            set { _avoidGeometryAntialias = value; }
        }
        public bool AvoidTextAntialias
        {
            get { return _avoidTextAntialias; }
            set { _avoidTextAntialias = value; }
        }

        public Graphics InternalGfx
        {
            get
            {
                return this.gx;
            }
        }
        public void MarkAsFirstTimeInvalidateAndUpdateContent()
        {
            canvasFlags = FIRSTTIME_INVALID_AND_UPDATED_CONTENT;
        }

        public bool IsFromPrinter
        {
            get
            {
                return isFromPrinter;
            }
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

        public SolidBrush GetSharedSolidBrush()
        {
            if (sharedSolidBrush == null)
            {
                sharedSolidBrush = new SolidBrush(Color.Black);
            }
            return sharedSolidBrush;
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

            currentClipRect = new Rectangle(0, 0, this.Width, this.Height);

            if (sharedSolidBrush != null)
            {
                sharedSolidBrush.Dispose();
            }

#if DEBUG

            debug_releaseCount++;
#endif
        }

        public bool IntersectsWith(InternalRect clientRect)
        {
            return clientRect.IntersectsWith(left, top, right, bottom);
        }
        public bool PushClipAreaForNativeScrollableElement(InternalRect updateArea)
        {

            clipRectStack.Push(currentClipRect);

            Rectangle intersectResult = Rectangle.Intersect(
                currentClipRect,
                updateArea.ToRectangle()
                );
            if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                currentClipRect = intersectResult; return false;
            }

            gx.SetClip(intersectResult); currentClipRect = intersectResult;
            return true;
        }


        public bool PushClipArea(int width, int height, InternalRect updateArea)
        {
            clipRectStack.Push(currentClipRect);

            Rectangle intersectResult =
                Rectangle.Intersect(
                    currentClipRect,
                    Rectangle.Intersect(updateArea.ToRectangle(), new Rectangle(0, 0, width, height)));


            currentClipRect = intersectResult; if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                return false;
            }
            else
            {
                gx.SetClip(intersectResult); return true;
            }
        }

        public void DisableClipArea()
        {
            gx.ResetClip();
        }
        public void EnableClipArea()
        {
            gx.SetClip(currentClipRect);
        }
        public void SetClip(RectangleF clip, System.Drawing.Drawing2D.CombineMode combineMode)
        {

            gx.SetClip(clip, combineMode);
        }

        public Rectangle CurrentClipRect
        {
            get
            {
                return currentClipRect;
            }
        }

#if DEBUG

        public int InternalOriginX
        {
            get
            {
                return internalCanvasOriginX;
            }
        }
        public int InternalOriginY
        {
            get
            {
                return internalCanvasOriginY;
            }
        }

#endif
        public bool PushClipArea(int x, int y, int width, int height)
        {
            clipRectStack.Push(currentClipRect);
            Rectangle intersectRect = Rectangle.Intersect(
    currentClipRect,
    new Rectangle(x, y, width, height));


            if (intersectRect.Width == 0 || intersectRect.Height == 0)
            {
                currentClipRect = intersectRect;
                return false;
            }
            else
            {
                gx.SetClip(intersectRect);
                currentClipRect = intersectRect;
                return true;
            }
        }

        public void PopClipArea()
        {
            if (clipRectStack.Count > 0)
            {
                currentClipRect = clipRectStack.Pop();

                gx.SetClip(currentClipRect);
            }







        }




        ~ArtCanvas()
        {
            ReleaseUnManagedResource();

        }


        public int Top
        {
            get
            {
                return top;
            }
        }
        public int Left
        {
            get
            {
                return left;
            }
        }

        public int Width
        {
            get
            {
                return right - left;
            }
        }
        public int Height
        {
            get
            {
                return bottom - top;
            }
        }
        public int Bottom
        {
            get
            {
                return bottom;
            }
        }
        public int Right
        {
            get
            {
                return right;
            }
        }
        public Rectangle Rect
        {
            get
            {
                return Rectangle.FromLTRB(left, top, right, bottom);
            }
        }

        public void DrawText(char[] buffer, int x, int y)
        {

            if (isFromPrinter)
            {
                gx.DrawString(new string(buffer),
    prevFonts.Peek().Font,
    internalBrush,
    x,
    y);

            }
            else
            {
                IntPtr gxdc = gx.GetHdc();
                MyWin32.SetViewportOrgEx(gxdc, internalCanvasOriginX, internalCanvasOriginY, IntPtr.Zero);
                NativeTextWin32.TextOut(gxdc, x, y, buffer, buffer.Length);
                MyWin32.SetViewportOrgEx(gxdc, -internalCanvasOriginX, -internalCanvasOriginY, IntPtr.Zero);
                gx.ReleaseHdc(gxdc);
            }
        }
        public void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {

            if (isFromPrinter)
            {
                gx.DrawString(
                    new string(buffer),
                    prevFonts.Peek().Font,
                    internalBrush,
                    logicalTextBox);
            }
            else
            {
                IntPtr gxdc = gx.GetHdc();
                MyWin32.SetViewportOrgEx(gxdc, internalCanvasOriginX, internalCanvasOriginY, IntPtr.Zero);
                Rectangle clipRect = Rectangle.Intersect(logicalTextBox, currentClipRect);
                clipRect.Offset(internalCanvasOriginX, internalCanvasOriginY);
                MyWin32.SetRectRgn(hRgn, clipRect.X, clipRect.Y, clipRect.Right, clipRect.Bottom);
                MyWin32.SelectClipRgn(gxdc, hRgn);
                NativeTextWin32.TextOut(gxdc, logicalTextBox.X, logicalTextBox.Y, buffer, buffer.Length);
                MyWin32.SelectClipRgn(gxdc, IntPtr.Zero);

                MyWin32.SetViewportOrgEx(gxdc, -internalCanvasOriginX, -internalCanvasOriginY, IntPtr.Zero);
                gx.ReleaseHdc();
            }
        }


        public const int SAME_FONT_SAME_TEXT_COLOR = 0;
        public const int SAME_FONT_DIFF_TEXT_COLOR = 1;
        public const int DIFF_FONT_SAME_TEXT_COLOR = 2;
        public const int DIFF_FONT_DIFF_TEXT_COLOR = 3;
        public int EvaluateFontAndTextColor(TextFontInfo textFontInfo, Color color)
        {

            if (textFontInfo != null && textFontInfo != currentTextFont)
            {
                if (color != currentTextColor)
                {
                    return DIFF_FONT_DIFF_TEXT_COLOR;
                }
                else
                {
                    return DIFF_FONT_SAME_TEXT_COLOR;
                }
            }
            else
            {
                if (color != currentTextColor)
                {
                    return SAME_FONT_DIFF_TEXT_COLOR;
                }
                else
                {
                    return SAME_FONT_SAME_TEXT_COLOR;
                }
            }
        }
        public void PushFont(TextFontInfo textFontInfo)
        {
            prevFonts.Push(currentTextFont); currentTextFont = textFontInfo;
            IntPtr hdc = gx.GetHdc();
            prevHFonts.Push(MyWin32.SelectObject(hdc, textFontInfo.HFont));
            gx.ReleaseHdc();
        }
        public void PopFont()
        {
            IntPtr hdc = gx.GetHdc();
            if (prevHFonts.Count > 0)
            {
                currentTextFont = prevFonts.Pop();
                MyWin32.SelectObject(hdc, prevHFonts.Pop());
            }
            gx.ReleaseHdc();
        }
        public void PushFontInfoAndTextColor(TextFontInfo textFontInfo, Color color)
        {
            prevFonts.Push(currentTextFont); currentTextFont = textFontInfo;
            IntPtr hdc = gx.GetHdc();
            prevHFonts.Push(MyWin32.SelectObject(hdc, textFontInfo.HFont));
            prevColor.Push(currentTextColor); this.currentTextColor = color;
            prevWin32Colors.Push(MyWin32.SetTextColor(hdc, GraphicWin32InterOp.ColorToWin32(color)));
            gx.ReleaseHdc();

        }
        public void PopFontInfoAndTextColor()
        {

            IntPtr hdc = gx.GetHdc();
            if (prevColor.Count > 0)
            {
                currentTextColor = prevColor.Pop();
                MyWin32.SetTextColor(hdc, prevWin32Colors.Pop());
            }
            if (prevHFonts.Count > 0)
            {
                currentTextFont = prevFonts.Pop();
                MyWin32.SelectObject(hdc, prevHFonts.Pop());
            }
            gx.ReleaseHdc();

        }
        public void PushTextColor(Color color)
        {

            IntPtr hdc = gx.GetHdc();
            prevColor.Push(currentTextColor); this.currentTextColor = color;
            prevWin32Colors.Push(MyWin32.SetTextColor(hdc, GraphicWin32InterOp.ColorToWin32(color)));
            gx.ReleaseHdc();
        }
        public void PopTextColor()
        {
            IntPtr hdc = gx.GetHdc();
            if (prevColor.Count > 0)
            {
                currentTextColor = prevColor.Pop();
                MyWin32.SetTextColor(hdc, prevWin32Colors.Pop());
            }
            gx.ReleaseHdc();
        }















        public void CopyFrom(ArtCanvas sourceCanvas, int logicalSrcX, int logicalSrcY, Rectangle destArea)
        {
            if (sourceCanvas.gx != null)
            {
                int phySrcX = logicalSrcX - sourceCanvas.left;
                int phySrcY = logicalSrcY - sourceCanvas.top;

                Rectangle postIntersect = Rectangle.Intersect(currentClipRect, destArea);
                phySrcX += postIntersect.X - destArea.X; phySrcY += postIntersect.Y - destArea.Y; destArea = postIntersect;
                IntPtr gxdc = gx.GetHdc();

                MyWin32.SetViewportOrgEx(gxdc, internalCanvasOriginX, internalCanvasOriginY, IntPtr.Zero);
                IntPtr source_gxdc = sourceCanvas.gx.GetHdc();
                MyWin32.SetViewportOrgEx(source_gxdc, sourceCanvas.internalCanvasOriginX, sourceCanvas.internalCanvasOriginY, IntPtr.Zero);


                MyWin32.BitBlt(gxdc, destArea.X, destArea.Y, destArea.Width,
destArea.Height, source_gxdc, phySrcX, phySrcY, MyWin32.SRCCOPY);


                MyWin32.SetViewportOrgEx(source_gxdc, -sourceCanvas.internalCanvasOriginX, -sourceCanvas.internalCanvasOriginY, IntPtr.Zero);

                sourceCanvas.gx.ReleaseHdc();

                MyWin32.SetViewportOrgEx(gxdc, -internalCanvasOriginX, -internalCanvasOriginY, IntPtr.Zero);
                gx.ReleaseHdc();



            }
        }
        public void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
        {
            IntPtr gxdc = gx.GetHdc();
            MyWin32.SetViewportOrgEx(gxdc, internalCanvasOriginX, internalCanvasOriginY, IntPtr.Zero);
            MyWin32.BitBlt(destHdc, destArea.X, destArea.Y,
    destArea.Width, destArea.Height, gxdc, sourceX, sourceY, MyWin32.SRCCOPY);
            MyWin32.SetViewportOrgEx(gxdc, -internalCanvasOriginX, -internalCanvasOriginY, IntPtr.Zero);
            gx.ReleaseHdc();

#if DEBUG
#endif

        }


        public void OffsetCanvasOrigin(int dx, int dy)
        {
            internalCanvasOriginX += dx;
            internalCanvasOriginY += dy;
            gx.TranslateTransform(dx, dy);
            currentClipRect.Offset(-dx, -dy);
        }
        public void OffsetCanvasOriginX(int dx)
        {
            internalCanvasOriginX += dx;
            gx.TranslateTransform(dx, 0);
            currentClipRect.Offset(-dx, 0);
        }
        public void OffsetCanvasOriginY(int dy)
        {
            internalCanvasOriginY += dy;
            gx.TranslateTransform(0, dy);
            currentClipRect.Offset(0, -dy);
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
        public void Reuse(int hPageNum, int vPageNum)
        {
            this.pageNumFlags = (hPageNum << 8) | vPageNum;

            int w = this.Width;
            int h = this.Height;

            this.ClearPreviousStoredValues();

            currentClipRect = new Rectangle(0, 0, w, h);
            gx.Clear(Color.White);
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
            Bitmap bmp = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            hbmp = bmp.GetHbitmap();
            MyWin32.SelectObject(originalHdc, hbmp);
            MyWin32.PatBlt(originalHdc, 0, 0, newWidth, newHeight, MyWin32.WHITENESS);
            MyWin32.SetBkMode(originalHdc, MyWin32._SetBkMode_TRANSPARENT);
            hFont = FontManager.CurrentFont.ToHfont();
            MyWin32.SelectObject(originalHdc, hFont);
            currentClipRect = new Rectangle(0, 0, newWidth, newHeight);
            MyWin32.SelectObject(originalHdc, hRgn);
            gx = Graphics.FromHdc(originalHdc);

            gx.Clear(Color.White);
            MyWin32.SetRectRgn(hRgn, 0, 0, newWidth, newHeight);


            left = hPageNum * newWidth;
            top = vPageNum * newHeight;
            right = left + newWidth;
            bottom = top + newHeight;
#if DEBUG
            debug_resetCount++;
#endif
        }

















        public void ClearSurface(InternalRect rect)
        {
            PushClipArea(rect._left, rect._top, rect.Width, rect.Height);
            gx.Clear(Color.White); PopClipArea();
        }
        public void ClearSurface()
        {
            gx.Clear(Color.White);
        }
        public void FillPolygon(Brush brush, PointF[] points)
        {
            gx.FillPolygon(brush, points);
        }
        public void FillPolygon(Brush brush, Point[] points)
        {
            gx.FillPolygon(brush, points);
        }

        public void FillPolygon(ArtColorBrush colorBrush, Point[] points)
        {
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;
                gx.FillPolygon(colorBrush.myBrush, points);

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;

                gx.FillPolygon(colorBrush.myBrush, points);


            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;


            }
        }
        public void FillRegion(ArtColorBrush colorBrush, Region rgn)
        {
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;
                gx.FillRegion(solidBrush.myBrush, rgn);

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;

                gx.FillRegion(colorBrush.myBrush, rgn);

            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;


            }
        }

        public void FillPath(GraphicsPath gfxPath, Color solidColor)
        {

            FillPath(gfxPath, new ArtSolidBrush(solidColor));
        }
        public void FillPath(GraphicsPath gfxPath, Brush colorBrush)
        {
            gx.FillPath(colorBrush, gfxPath);
        }
        public void FillPath(GraphicsPath gfxPath, ArtColorBrush colorBrush)
        {
            gx.SmoothingMode = SmoothingMode.HighSpeed;
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;
                if (solidBrush.myBrush == null)
                {
                    solidBrush.myBrush = new SolidBrush(solidBrush.Color);
                }
                gx.FillPath(solidBrush.myBrush, gfxPath);

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;
                gx.FillPath(gradientBrush.myBrush, gfxPath);

            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;
            }

        }
        public void DrawPath(GraphicsPath gfxPath)
        {
            gx.DrawPath(internalPen, gfxPath);
        }
        public void DrawPath(GraphicsPath gfxPath, Color color)
        {
            internalPen.Color = color;
            internalPen.Alignment = PenAlignment.Right;
            gx.DrawPath(internalPen, gfxPath);
        }
        public void DrawPath(GraphicsPath gfxPath, Pen pen)
        {
            gx.SmoothingMode = SmoothingMode.AntiAlias;
            gx.DrawPath(pen, gfxPath);
        }
        public void FillRectangle(Color color, Rectangle rect)
        {
            internalBrush.Color = color;
            gx.FillRectangle(internalBrush, rect);
        }
        public void FillRectangle(Color color, RectangleF rectf)
        {
            internalBrush.Color = color;
            gx.FillRectangle(internalBrush, rectf);
        }
        public void FillRectangle(Brush brush, Rectangle rect)
        {

            gx.FillRectangle(brush, rect);
        }
        public void FillRectangle(Brush brush, RectangleF rectf)
        {
            gx.FillRectangle(brush, rectf);
        }
        public void FillRectangle(ArtColorBrush brush, RectangleF rectf)
        {

            gx.FillRectangle(brush.myBrush, rectf);
        }
        public void FillRectangle(Color color, int left, int top, int right, int bottom)
        {
            internalBrush.Color = color;
            gx.FillRectangle(internalBrush, left, top, right - left, bottom - top);
        }
        public float GetBoundWidth(Region rgn)
        {
            return rgn.GetBounds(gx).Width;
        }
        public RectangleF GetBound(Region rgn)
        {
            return rgn.GetBounds(gx);
        }
        public float GetFontHeight(Font f)
        {
            return f.GetHeight(gx);
        }
        public Region[] MeasureCharacterRanges(string text, Font f, RectangleF layoutRectF, StringFormat strFormat)
        {
            return gx.MeasureCharacterRanges(text, f, layoutRectF, strFormat);
        }
        public Size MeasureString(string str, Font font, float maxWidth, out int charFit, out int charFitWidth)
        {

            if (_useGdiPlusTextRendering)
            {
                throw new NotSupportedException("Char fit string measuring is not supported for GDI+ text rendering");
            }
            else
            {


                throw new NotSupportedException();


            }
        }
        public void FillRectangle(ArtColorBrush colorBrush, int left, int top, int right, int bottom)
        {
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;

                if (solidBrush.myBrush == null)
                {
                    solidBrush.myBrush = new SolidBrush(solidBrush.Color);
                }
                gx.FillRectangle(solidBrush.myBrush, Rectangle.FromLTRB(left, top, right, bottom));

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;
                gx.FillRectangle(gradientBrush.myBrush, Rectangle.FromLTRB(left, top, right, bottom));

            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;

                if (imgBrush.MyImage != null)
                {
                    gx.DrawImageUnscaled(imgBrush.MyImage, 0, 0);
                }
            }
        }
        public void DrawRectangle(Pen p, Rectangle rect)
        {
            gx.DrawRectangle(p, rect);

        }
        public void DrawRectangle(Pen p, float x, float y, float width, float height)
        {
            gx.DrawRectangle(p, x, y, width, height);
        }
        public SmoothingMode SmoothingMode
        {
            get
            {
                return gx.SmoothingMode;
            }
            set
            {
                gx.SmoothingMode = value;
            }
        }

        public void DrawRectangle(Color color, int left, int top, int width, int height)
        {

            internalPen.Color = color;
            gx.DrawRectangle(internalPen, left, top, width, height);
        }
        public void DrawString(string str, Font f, Brush brush, float x, float y)
        {
            gx.DrawString(str, f, brush, x, y);

        }
        public void DrawString(string str, Font f, Brush brush, float x, float y, float w, float h)
        {
            gx.DrawString(str, f, brush, new RectangleF(x, y, w, h));

        }
        public void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            internalPen.Color = color;
            gx.DrawRectangle(internalPen, left, top, width, height);
        }
        public void DrawRectangle(Color color, Rectangle rect)
        {
            internalPen.Color = color;
            gx.DrawRectangle(internalPen, rect);

        }
        public void DrawImage(Image image, RectangleF dest, RectangleF src)
        {
            gx.DrawImage(image, dest, src, GraphicsUnit.Pixel);

        }
        public void DrawImage(Image image, RectangleF rect)
        {
            gx.DrawImage(image, rect);
        }
        public void DrawImage(Image image, Rectangle rect)
        {
            gx.DrawImage(image, rect);
        }
        public void DrawImage(Bitmap image, int x, int y, int w, int h)
        {
            gx.DrawImage(image, x, y, w, h);
        }
        public void DrawImageUnScaled(Bitmap image, int x, int y)
        {
            gx.DrawImageUnscaled(image, x, y);
        }
#if DEBUG
        public void dbug_DrawRuler(int x)
        {
            int canvas_top = this.top;
            int canvas_bottom = this.Bottom;
            for (int y = canvas_top; y < canvas_bottom; y += 10)
            {
                this.DrawText(y.ToString().ToCharArray(), x, y);
            }
        }
        public void dbug_DrawCrossRect(Color color, Rectangle rect)
        {
            DrawLine(color, rect.Location, new Point(rect.Right, rect.Bottom));
            DrawLine(color, new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Top));
        }

#endif
        public void DrawBezire(Point[] points)
        {
            gx.DrawBeziers(Pens.Red, points);
        }
        public void DrawLine(Pen pen, Point p1, Point p2)
        {
            gx.DrawLine(pen, p1, p2);
        }
        public void DrawLine(Color c, int x1, int y1, int x2, int y2)
        {
            Color prevColor = internalPen.Color; internalPen.Color = c;
            gx.DrawLine(internalPen, x1, y1, x2, y2);
            internalPen.Color = prevColor;
        }
        public void DrawLine(Color c, float x1, float y1, float x2, float y2)
        {
            Color prevColor = internalPen.Color; internalPen.Color = c;
            gx.DrawLine(internalPen, x1, y1, x2, y2);
            internalPen.Color = prevColor;
        }
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {

            gx.DrawLine(pen, x1, y1, x2, y2);
        }
        public void DrawLine(Color color, Point p1, Point p2)
        {
            Color prevColor = internalPen.Color; internalPen.Color = color;
            gx.DrawLine(internalPen, p1, p2);
            internalPen.Color = prevColor;
        }
        public void DrawLine(Color color, Point p1, Point p2, DashStyle lineDashStyle)
        {
            DashStyle prevLineDashStyle = internalPen.DashStyle;
            internalPen.DashStyle = lineDashStyle;
            internalPen.Color = color;
            gx.DrawLine(internalPen, p1, p2);
            internalPen.DashStyle = prevLineDashStyle;

        }
        public void DrawArc(Pen pen, Rectangle r, float startAngle, float sweepAngle)
        {
            gx.SmoothingMode = SmoothingMode.HighQuality;
            gx.DrawArc(pen, r, startAngle, sweepAngle);
        }


        public void DrawLines(Color color, Point[] points)
        {
            internalPen.Color = color;
            gx.DrawLines(internalPen, points);
        }
        public void DrawPolygon(Point[] points)
        {
            gx.DrawPolygon(Pens.Maroon, points);
        }
        public void FillPolygon(Point[] points)
        {
            gx.FillPolygon(Brushes.Maroon, points);
        }
        public void FillEllipse(Point[] points)
        {
            gx.FillEllipse(Brushes.Maroon, points[0].X, points[0].Y, points[2].X - points[0].X, points[2].Y - points[0].Y);

        }
        public void FillEllipse(Color color, Rectangle rect)
        {


            internalBrush.Color = color;
            gx.FillEllipse(internalBrush, rect);


        }
        public void FillEllipse(Color color, int x, int y, int width, int height)
        {

            internalBrush.Color = color;
            gx.FillEllipse(internalBrush, x, y, width, height);


        }
        public void DrawRoundRect(int x, int y, int w, int h, Size cornerSize)
        {

            int cornerSizeW = cornerSize.Width;
            int cornerSizeH = cornerSize.Height;

            GraphicsPath gxPath = new GraphicsPath();
            gxPath.AddArc(new Rectangle(x, y, cornerSizeW * 2, cornerSizeH * 2), 180, 90);
            gxPath.AddLine(new Point(x + cornerSizeW, y), new Point(x + w - cornerSizeW, y));

            gxPath.AddArc(new Rectangle(x + w - cornerSizeW * 2, y, cornerSizeW * 2, cornerSizeH * 2), -90, 90);
            gxPath.AddLine(new Point(x + w, y + cornerSizeH), new Point(x + w, y + h - cornerSizeH));

            gxPath.AddArc(new Rectangle(x + w - cornerSizeW * 2, y + h - cornerSizeH * 2, cornerSizeW * 2, cornerSizeH * 2), 0, 90);
            gxPath.AddLine(new Point(x + w - cornerSizeW, y + h), new Point(x + cornerSizeW, y + h));

            gxPath.AddArc(new Rectangle(x, y + h - cornerSizeH * 2, cornerSizeW * 2, cornerSizeH * 2), 90, 90);
            gxPath.AddLine(new Point(x, y + cornerSizeH), new Point(x, y + h - cornerSizeH));

            gx.FillPath(Brushes.Yellow, gxPath);
            gx.DrawPath(Pens.Red, gxPath);
            gxPath.Dispose();
        }

        InternalRect invalidateArea = InternalRect.CreateFromLTRB(0, 0, 0, 0); int canvasFlags = FIRSTTIME_INVALID;

        const int WAIT_FOR_UPDATE = 0x0;

        const int FIRSTTIME_INVALID = 0x1; const int UPDATED_CONTENT = 0x2;
        const int FIRSTTIME_INVALID_AND_UPDATED_CONTENT = 0x3;

        public InternalRect InvalidateArea
        {
            get
            {
                return invalidateArea;
            }
        }
        public bool IsContentUpdated
        {
            get
            {
                return ((canvasFlags & UPDATED_CONTENT) == UPDATED_CONTENT);
            }
        }

        public void Invalidate(InternalRect rect)
        {

            if ((canvasFlags & FIRSTTIME_INVALID) == FIRSTTIME_INVALID)
            {
                invalidateArea.LoadValues(rect);
            }
            else
            {
                invalidateArea.MergeRect(rect);
            }

            canvasFlags = WAIT_FOR_UPDATE;
        }

    }

}
