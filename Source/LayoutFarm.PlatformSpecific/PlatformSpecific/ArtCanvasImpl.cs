//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{


    public class CanvasImpl : Canvas,IGraphics2
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
        System.Drawing.Graphics gx;
        IntPtr originalHdc = IntPtr.Zero;

        int internalCanvasOriginX = 0;
        int internalCanvasOriginY = 0;

        IntPtr hRgn = IntPtr.Zero;
        Stack<int> prevWin32Colors = new Stack<int>();


        Stack<IntPtr> prevHFonts = new Stack<IntPtr>();

        Stack<TextFontInfo> prevFonts = new Stack<TextFontInfo>();
        Stack<System.Drawing.Color> prevColor = new Stack<System.Drawing.Color>();


        System.Drawing.Color currentTextColor = System.Drawing.Color.Black;
        TextFontInfo currentTextFont = null;
        Stack<System.Drawing.Rectangle> prevRegionRects = new Stack<System.Drawing.Rectangle>();

        System.Drawing.Pen internalPen;
        System.Drawing.SolidBrush internalBrush;

        System.Drawing.Rectangle currentClipRect;

        Stack<System.Drawing.Rectangle> clipRectStack = new Stack<System.Drawing.Rectangle>();

        IntPtr hbmp;
        bool _avoidGeometryAntialias;
        bool _avoidTextAntialias;

        bool isFromPrinter = false;
        SolidBrush sharedSolidBrush;

        public CanvasImpl(int horizontalPageNum, int verticalPageNum, int left, int top, int width, int height)
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

        public CanvasImpl(IGraphics2 gx, int verticalPageNum, int horizontalPageNum, int left, int top, int width, int height)
        {

            this.pageNumFlags = (horizontalPageNum << 8) | verticalPageNum;
            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;
            internalPen = new System.Drawing.Pen(System.Drawing.Color.Black);
            internalBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            this.gx = gx.GetInnerGraphic() as System.Drawing.Graphics;


            isFromPrinter = true;
            currentClipRect = new System.Drawing.Rectangle(0, 0, width, height);

            PushFontInfoAndTextColor(FontManager.DefaultTextFontInfo, Color.Black);

#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif

        }

        public override bool AvoidGeometryAntialias
        {
            get { return _avoidGeometryAntialias; }
            set { _avoidGeometryAntialias = value; }
        }
        public override bool AvoidTextAntialias
        {
            get { return _avoidTextAntialias; }
            set { _avoidTextAntialias = value; }
        }

        public System.Drawing.Graphics InternalGfx
        {
            get
            {
                return this.gx;
            }
        }
        public override void MarkAsFirstTimeInvalidateAndUpdateContent()
        {
            canvasFlags = FIRSTTIME_INVALID_AND_UPDATED_CONTENT;
        }
        public override IGraphics2 GetGfx()
        {
            return this;
        }
        public object GetInnerGraphic()
        {
            return this.gx;
        }
        public override bool IsFromPrinter
        {
            get
            {
                return isFromPrinter;
            }
        }
        public override bool IsPageNumber(int hPageNum, int vPageNum)
        {
            return pageNumFlags == ((hPageNum << 8) | vPageNum);
        }
        public override bool IsUnused
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
        public override bool DimensionInvalid
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

        public override SolidBrush GetSharedSolidBrush()
        {
            if (sharedSolidBrush == null)
            {
                sharedSolidBrush = CurrentGraphicPlatform.CreateSolidBrush(Color.Black);// new System.Drawing.SolidBrush(Color.Black);
            }
            return sharedSolidBrush;
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

        public override bool IntersectsWith(InternalRect clientRect)
        {
            return clientRect.IntersectsWith(left, top, right, bottom);
        }
        public override bool PushClipAreaForNativeScrollableElement(InternalRect updateArea)
        {

            clipRectStack.Push(currentClipRect);

            System.Drawing.Rectangle intersectResult = System.Drawing.Rectangle.Intersect(
                currentClipRect,
                Conv.ConvFromRect(updateArea.ToRectangle()));

            if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                currentClipRect = intersectResult;
                return false;
            }

            gx.SetClip(intersectResult);
            currentClipRect = intersectResult;
            return true;
        }


        public override bool PushClipArea(int width, int height, InternalRect updateArea)
        {
            clipRectStack.Push(currentClipRect);

            System.Drawing.Rectangle intersectResult =
                System.Drawing.Rectangle.Intersect(
                    currentClipRect,
                    System.Drawing.Rectangle.Intersect(
                    Conv.ConvFromRect(updateArea.ToRectangle()),
                    new System.Drawing.Rectangle(0, 0, width, height)));


            currentClipRect = intersectResult;
            if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                return false;
            }
            else
            {
                gx.SetClip(intersectResult); return true;
            }
        }

        public override void DisableClipArea()
        {
            gx.ResetClip();
        }
        public override void EnableClipArea()
        {
            gx.SetClip(currentClipRect);
        }
        public override void SetClip(RectangleF clip, CombineMode combineMode)
        {
            gx.SetClip(Conv.ConvFromRectF(clip), (System.Drawing.Drawing2D.CombineMode)combineMode);
        }

        public override Rectangle CurrentClipRect
        {
            get
            {
                return Conv.ConvToRect(currentClipRect);
            }
        }

#if DEBUG

        public override int InternalOriginX
        {
            get
            {
                return internalCanvasOriginX;
            }
        }
        public override int InternalOriginY
        {
            get
            {
                return internalCanvasOriginY;
            }
        }

#endif
        public override bool PushClipArea(int x, int y, int width, int height)
        {
            clipRectStack.Push(currentClipRect);
            System.Drawing.Rectangle intersectRect = System.Drawing.Rectangle.Intersect(
                currentClipRect,
                new System.Drawing.Rectangle(x, y, width, height));


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

        public override void PopClipArea()
        {
            if (clipRectStack.Count > 0)
            {
                currentClipRect = clipRectStack.Pop();

                gx.SetClip(currentClipRect);
            }
        }




        ~CanvasImpl()
        {
            ReleaseUnManagedResource();

        }


        public override int Top
        {
            get
            {
                return top;
            }
        }
        public override int Left
        {
            get
            {
                return left;
            }
        }

        public override int Width
        {
            get
            {
                return right - left;
            }
        }
        public override int Height
        {
            get
            {
                return bottom - top;
            }
        }
        public override int Bottom
        {
            get
            {
                return bottom;
            }
        }
        public override int Right
        {
            get
            {
                return right;
            }
        }
        public override Rectangle Rect
        {
            get
            {
                return Rectangle.FromLTRB(left, top, right, bottom);
            }
        }

        public override void DrawText(char[] buffer, int x, int y)
        {

            if (isFromPrinter)
            {
                gx.DrawString(new string(buffer),
                        ConvFont(prevFonts.Peek().Font),
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
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {

            if (isFromPrinter)
            {
                gx.DrawString(
                    new string(buffer),
                    ConvFont(prevFonts.Peek().Font),
                    internalBrush,
                   Conv.ConvFromRect(logicalTextBox));
            }
            else
            {
                IntPtr gxdc = gx.GetHdc();
                MyWin32.SetViewportOrgEx(gxdc, internalCanvasOriginX, internalCanvasOriginY, IntPtr.Zero);
                System.Drawing.Rectangle clipRect =
                    System.Drawing.Rectangle.Intersect(Conv.ConvFromRect(logicalTextBox), currentClipRect);
                clipRect.Offset(internalCanvasOriginX, internalCanvasOriginY);
                MyWin32.SetRectRgn(hRgn, clipRect.X, clipRect.Y, clipRect.Right, clipRect.Bottom);
                MyWin32.SelectClipRgn(gxdc, hRgn);
                NativeTextWin32.TextOut(gxdc, logicalTextBox.X, logicalTextBox.Y, buffer, buffer.Length);
                MyWin32.SelectClipRgn(gxdc, IntPtr.Zero);

                MyWin32.SetViewportOrgEx(gxdc, -internalCanvasOriginX, -internalCanvasOriginY, IntPtr.Zero);
                gx.ReleaseHdc();
            }
        }


        static bool IsEqColor(Color c1, System.Drawing.Color c2)
        {
            return c1.R == c2.R &&
                   c1.G == c2.G &&
                   c1.B == c2.B &&
                   c1.A == c2.A;
        }
        public override int EvaluateFontAndTextColor(TextFontInfo textFontInfo, Color color)
        {

            if (textFontInfo != null && textFontInfo != currentTextFont)
            {
                if (!IsEqColor(color, currentTextColor))
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
                if (!IsEqColor(color, currentTextColor))
                {
                    return SAME_FONT_DIFF_TEXT_COLOR;
                }
                else
                {
                    return SAME_FONT_SAME_TEXT_COLOR;
                }
            }
        }
        public override void PushFont(TextFontInfo textFontInfo)
        {
            prevFonts.Push(currentTextFont); currentTextFont = textFontInfo;
            IntPtr hdc = gx.GetHdc();
            prevHFonts.Push(MyWin32.SelectObject(hdc, textFontInfo.HFont));
            gx.ReleaseHdc();
        }
        public override void PopFont()
        {
            IntPtr hdc = gx.GetHdc();
            if (prevHFonts.Count > 0)
            {
                currentTextFont = prevFonts.Pop();
                MyWin32.SelectObject(hdc, prevHFonts.Pop());
            }
            gx.ReleaseHdc();
        }
        public override void PushFontInfoAndTextColor(TextFontInfo textFontInfo, Color color)
        {
            prevFonts.Push(currentTextFont);
            currentTextFont = textFontInfo;
            IntPtr hdc = gx.GetHdc();
            prevHFonts.Push(MyWin32.SelectObject(hdc, textFontInfo.HFont));
            prevColor.Push(currentTextColor);
            this.currentTextColor = ConvColor(color);
            prevWin32Colors.Push(MyWin32.SetTextColor(hdc, GraphicWin32InterOp.ColorToWin32(color)));
            gx.ReleaseHdc();

        }
        public override void PopFontInfoAndTextColor()
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
        public override void PushTextColor(Color color)
        {

            IntPtr hdc = gx.GetHdc();
            prevColor.Push(currentTextColor);
            this.currentTextColor = ConvColor(color);
            prevWin32Colors.Push(MyWin32.SetTextColor(hdc, GraphicWin32InterOp.ColorToWin32(color)));
            gx.ReleaseHdc();
        }
        public override void PopTextColor()
        {
            IntPtr hdc = gx.GetHdc();
            if (prevColor.Count > 0)
            {
                currentTextColor = prevColor.Pop();
                MyWin32.SetTextColor(hdc, prevWin32Colors.Pop());
            }
            gx.ReleaseHdc();
        }
        public override void CopyFrom(Canvas sourceCanvas, int logicalSrcX, int logicalSrcY, Rectangle destArea)
        {
            CanvasImpl s1 = (CanvasImpl)sourceCanvas;

            if (s1.gx != null)
            {
                int phySrcX = logicalSrcX - s1.left;
                int phySrcY = logicalSrcY - s1.top;

                System.Drawing.Rectangle postIntersect =
                    System.Drawing.Rectangle.Intersect(currentClipRect, Conv.ConvFromRect(destArea));
                phySrcX += postIntersect.X - destArea.X;
                phySrcY += postIntersect.Y - destArea.Y;
                destArea = Conv.ConvToRect(postIntersect);

                IntPtr gxdc = gx.GetHdc();

                MyWin32.SetViewportOrgEx(gxdc, internalCanvasOriginX, internalCanvasOriginY, IntPtr.Zero);
                IntPtr source_gxdc = s1.gx.GetHdc();
                MyWin32.SetViewportOrgEx(source_gxdc, s1.internalCanvasOriginX, s1.internalCanvasOriginY, IntPtr.Zero);


                MyWin32.BitBlt(gxdc, destArea.X, destArea.Y, destArea.Width, destArea.Height, source_gxdc, phySrcX, phySrcY, MyWin32.SRCCOPY);


                MyWin32.SetViewportOrgEx(source_gxdc, -s1.internalCanvasOriginX, -s1.internalCanvasOriginY, IntPtr.Zero);

                s1.gx.ReleaseHdc();

                MyWin32.SetViewportOrgEx(gxdc, -internalCanvasOriginX, -internalCanvasOriginY, IntPtr.Zero);
                gx.ReleaseHdc();



            }
        }
        public override void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
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


        public override void OffsetCanvasOrigin(int dx, int dy)
        {
            internalCanvasOriginX += dx;
            internalCanvasOriginY += dy;
            gx.TranslateTransform(dx, dy);
            currentClipRect.Offset(-dx, -dy);
        }
        public override void OffsetCanvasOriginX(int dx)
        {
            internalCanvasOriginX += dx;
            gx.TranslateTransform(dx, 0);
            currentClipRect.Offset(-dx, 0);
        }
        public override void OffsetCanvasOriginY(int dy)
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

        public override void ClearSurface(InternalRect rect)
        {
            PushClipArea(rect._left, rect._top, rect.Width, rect.Height);
            gx.Clear(System.Drawing.Color.White); PopClipArea();
        }
        public override void ClearSurface()
        {
            gx.Clear(System.Drawing.Color.White);
        }
        public override void FillPolygon(Brush brush, PointF[] points)
        {
            gx.FillPolygon(ConvBrush(brush), ConvPointFArray(points));
        }
        public override void FillPolygon(Brush brush, Point[] points)
        {
            gx.FillPolygon(ConvBrush(brush), ConvPointArray(points));
        }

        public override void FillPolygon(ArtColorBrush colorBrush, Point[] points)
        {
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;
                gx.FillPolygon(ConvBrush(colorBrush.myBrush), ConvPointArray(points));

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;

                gx.FillPolygon(ConvBrush(colorBrush.myBrush), ConvPointArray(points));


            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;


            }
        }
        public override void FillRegion(ArtColorBrush colorBrush, Region rgn)
        {
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;
                gx.FillRegion(ConvBrush(solidBrush.myBrush), ConvRgn(rgn));

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;
                gx.FillRegion(ConvBrush(colorBrush.myBrush), ConvRgn(rgn));

            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;


            }
        }

        public override void FillPath(GraphicsPath gfxPath, Color solidColor)
        {

            FillPath(gfxPath, new ArtSolidBrush(solidColor));
        }
        public override void FillPath(GraphicsPath gfxPath, Brush colorBrush)
        {
            gx.FillPath(ConvBrush(colorBrush), ConvPath(gfxPath));
        }
        public override void FillPath(GraphicsPath gfxPath, ArtColorBrush colorBrush)
        {
            gx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;
                if (solidBrush.myBrush == null)
                {
                    solidBrush.myBrush = CurrentGraphicPlatform.CreateSolidBrush(solidBrush.Color);
                }
                gx.FillPath(ConvBrush(solidBrush.myBrush), ConvPath(gfxPath));

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;
                gx.FillPath(ConvBrush(gradientBrush.myBrush), ConvPath(gfxPath));

            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;
            }

        }
        public override void DrawPath(GraphicsPath gfxPath)
        {
            gx.DrawPath(internalPen, gfxPath.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }
        public override void DrawPath(GraphicsPath gfxPath, Color color)
        {
            internalPen.Color = ConvColor(color);
            internalPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Right;
            gx.DrawPath(internalPen, ConvPath(gfxPath));
        }
        public override void DrawPath(GraphicsPath gfxPath, Pen pen)
        {
            gx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            gx.DrawPath(ConvPen(pen), ConvPath(gfxPath));
        }
        public override void FillRectangle(Color color, Rectangle rect)
        {
            internalBrush.Color = ConvColor(color);
            gx.FillRectangle(internalBrush, Conv.ConvFromRect(rect));
        }
        public override void FillRectangle(Color color, RectangleF rectf)
        {
            internalBrush.Color = ConvColor(color);
            gx.FillRectangle(internalBrush, Conv.ConvFromRectF(rectf));
        }
        public override void FillRectangle(Brush brush, Rectangle rect)
        {

            gx.FillRectangle(ConvBrush(brush), Conv.ConvFromRect(rect));
        }
        public override void FillRectangle(Brush brush, RectangleF rectf)
        {
            gx.FillRectangle(ConvBrush(brush), Conv.ConvFromRectF(rectf));
        }
        public override void FillRectangle(ArtColorBrush brush, RectangleF rectf)
        {

            gx.FillRectangle(ConvBrush(brush.myBrush), Conv.ConvFromRectF(rectf));
        }
        public override void FillRectangle(Color color, int left, int top, int right, int bottom)
        {
            internalBrush.Color = ConvColor(color);
            gx.FillRectangle(internalBrush, left, top, right - left, bottom - top);
        }
        public override float GetBoundWidth(Region rgn)
        {
            return ConvRgn(rgn).GetBounds(gx).Width;

        }
        public override RectangleF GetBound(Region rgn)
        {
            return Conv.ConvToRectF(ConvRgn(rgn).GetBounds(gx));

        }
        public override float GetFontHeight(Font f)
        {
            return ConvFont(f).GetHeight(gx);
        }
        public override Region[] MeasureCharacterRanges(string text, Font f, RectangleF layoutRectF, StringFormat strFormat)
        {
            throw new NotSupportedException();
            //return gx.MeasureCharacterRanges(
            //    text,
            //    ConvFont(f),
            //    Conv.ConvFromRectF(layoutRectF),
            //    strFormat.InnerFormat as System.Drawing.StringFormat);
        }
        public override Size MeasureString(string str, Font font, float maxWidth, out int charFit, out int charFitWidth)
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
        public override void FillRectangle(ArtColorBrush colorBrush, int left, int top, int right, int bottom)
        {
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;

                if (solidBrush.myBrush == null)
                {
                    solidBrush.myBrush = CurrentGraphicPlatform.CreateSolidBrush(solidBrush.Color);
                }
                gx.FillRectangle(solidBrush.myBrush.InnerBrush as System.Drawing.Brush,
                    System.Drawing.Rectangle.FromLTRB(left, top, right, bottom));

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;
                gx.FillRectangle(gradientBrush.myBrush.InnerBrush as System.Drawing.Brush,
                    System.Drawing.Rectangle.FromLTRB(left, top, right, bottom));

            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;

                if (imgBrush.MyImage != null)
                {

                    gx.DrawImageUnscaled(ConvBitmap(imgBrush.MyImage), 0, 0);
                }
            }
        }
        public override void DrawRectangle(Pen p, Rectangle rect)
        {
            gx.DrawRectangle(ConvPen(p), Conv.ConvFromRect(rect));

        }
        public override void DrawRectangle(Pen p, float x, float y, float width, float height)
        {
            gx.DrawRectangle(ConvPen(p), x, y, width, height);
        }
        public override SmoothingMode SmoothingMode
        {
            get
            {
                return (SmoothingMode)gx.SmoothingMode;
            }
            set
            {
                gx.SmoothingMode = (System.Drawing.Drawing2D.SmoothingMode)value;
            }
        }

        public override void DrawRectangle(Color color, int left, int top, int width, int height)
        {

            internalPen.Color = ConvColor(color);
            gx.DrawRectangle(internalPen, left, top, width, height);
        }
        public override void DrawString(string str, Font f, Brush brush, float x, float y)
        {
            gx.DrawString(str, ConvFont(f), ConvBrush(brush), x, y);

        }
        public override void DrawString(string str, Font f, Brush brush, float x, float y, float w, float h)
        {
            gx.DrawString(str, ConvFont(f), ConvBrush(brush), new System.Drawing.RectangleF(x, y, w, h));

        }
        public override void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            internalPen.Color = ConvColor(color);
            gx.DrawRectangle(internalPen, left, top, width, height);
        }
        public override void DrawRectangle(Color color, Rectangle rect)
        {
            internalPen.Color = ConvColor(color);
            gx.DrawRectangle(internalPen, Conv.ConvFromRect(rect));

        }
        public override void DrawImage(Image image, RectangleF dest, RectangleF src)
        {
            gx.DrawImage(image.InnerImage as System.Drawing.Bitmap,
               Conv.ConvFromRectF(dest),
               Conv.ConvFromRectF(src),
               System.Drawing.GraphicsUnit.Pixel);

        }
        public override void DrawImage(Image image, RectangleF rect)
        {
            gx.DrawImage(ConvBitmap(image), Conv.ConvFromRectF(rect));
        }
        public override void DrawImage(Image image, Rectangle rect)
        {
            gx.DrawImage(
                ConvBitmap(image),
                Conv.ConvFromRect(rect));
        }
        public override void DrawImage(Bitmap image, int x, int y, int w, int h)
        {
            gx.DrawImage(image.InnerImage as System.Drawing.Bitmap, x, y, w, h);
        }
        public override void DrawImageUnScaled(Bitmap image, int x, int y)
        {
            gx.DrawImageUnscaled(image.InnerImage as System.Drawing.Bitmap, x, y);
        }
#if DEBUG
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
            DrawLine(color, rect.Location, new Point(rect.Right, rect.Bottom));
            DrawLine(color, new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Top));
        }

#endif
        public override void DrawBezire(Point[] points)
        {
            gx.DrawBeziers(System.Drawing.Pens.Blue, ConvPointArray(points));
        }
        public override void DrawLine(Pen pen, Point p1, Point p2)
        {
            gx.DrawLine(ConvPen(pen), Conv.ConvFromPoint(p1), Conv.ConvFromPoint(p2));
        }
        public override void DrawLine(Color c, int x1, int y1, int x2, int y2)
        {
            System.Drawing.Color prevColor = internalPen.Color;
            internalPen.Color = ConvColor(c);
            gx.DrawLine(internalPen, x1, y1, x2, y2);
            internalPen.Color = prevColor;
        }
        public override void DrawLine(Color c, float x1, float y1, float x2, float y2)
        {
            System.Drawing.Color prevColor = internalPen.Color;
            internalPen.Color = ConvColor(c);
            gx.DrawLine(internalPen, x1, y1, x2, y2);
            internalPen.Color = prevColor;
        }
        public override void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {

            gx.DrawLine(ConvPen(pen), x1, y1, x2, y2);
        }
        public override void DrawLine(Color color, Point p1, Point p2)
        {
            System.Drawing.Color prevColor = internalPen.Color;
            internalPen.Color = ConvColor(color);
            gx.DrawLine(internalPen, Conv.ConvFromPoint(p1), Conv.ConvFromPoint(p2));
            internalPen.Color = prevColor;
        }
        public override void DrawLine(Color color, Point p1, Point p2, DashStyle lineDashStyle)
        {
            System.Drawing.Drawing2D.DashStyle prevLineDashStyle = (System.Drawing.Drawing2D.DashStyle)internalPen.DashStyle;
            internalPen.DashStyle = (System.Drawing.Drawing2D.DashStyle)lineDashStyle;

            internalPen.Color = ConvColor(color);
            gx.DrawLine(internalPen,
                Conv.ConvFromPoint(p1),
                Conv.ConvFromPoint(p2));
            internalPen.DashStyle = prevLineDashStyle;

        }
        public override void DrawArc(Pen pen, Rectangle r, float startAngle, float sweepAngle)
        {
            gx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gx.DrawArc(ConvPen(pen),
                Conv.ConvFromRect(r),
                startAngle,
                sweepAngle);
        }
        public override void DrawLines(Color color, Point[] points)
        {
            internalPen.Color = ConvColor(color);
            gx.DrawLines(internalPen,
               ConvPointArray(points));
        }
        public override void DrawPolygon(Point[] points)
        {
            gx.DrawPolygon(System.Drawing.Pens.Blue, ConvPointArray(points));
        }
        public override void FillPolygon(Point[] points)
        {
            gx.FillPolygon(System.Drawing.Brushes.Blue, ConvPointArray(points));
        }
        public override void FillEllipse(Point[] points)
        {
            gx.FillEllipse(System.Drawing.Brushes.Blue, points[0].X, points[0].Y, points[2].X - points[0].X, points[2].Y - points[0].Y);

        }
        public override void FillEllipse(Color color, Rectangle rect)
        {
            internalBrush.Color = ConvColor(color);
            gx.FillEllipse(internalBrush, Conv.ConvFromRect(rect));

        }
        static System.Drawing.Point[] ConvPointArray(Point[] points)
        {
            int j = points.Length;
            System.Drawing.Point[] outputPoints = new System.Drawing.Point[j];
            for (int i = 0; i < j; ++i)
            {
                outputPoints[i] = Conv.ConvFromPoint(points[i]);
            }
            return outputPoints;
        }
        static System.Drawing.PointF[] ConvPointFArray(PointF[] points)
        {
            int j = points.Length;
            System.Drawing.PointF[] outputPoints = new System.Drawing.PointF[j];
            for (int i = 0; i < j; ++i)
            {
                outputPoints[i] = Conv.ConvFromPointF(points[i]);
            }
            return outputPoints;
        }
        static System.Drawing.Color ConvColor(Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }
        static System.Drawing.Brush ConvBrush(Brush b)
        {
            return b.InnerBrush as System.Drawing.Brush;
        }
        static System.Drawing.Pen ConvPen(Pen p)
        {
            return p.InnerPen as System.Drawing.Pen;
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
        public override void FillEllipse(Color color, int x, int y, int width, int height)
        {
            internalBrush.Color = ConvColor(color);
            gx.FillEllipse(internalBrush, x, y, width, height);


        }
        public override void DrawRoundRect(int x, int y, int w, int h, Size cornerSize)
        {

            int cornerSizeW = cornerSize.Width;
            int cornerSizeH = cornerSize.Height;

            System.Drawing.Drawing2D.GraphicsPath gxPath = new System.Drawing.Drawing2D.GraphicsPath();
            gxPath.AddArc(new System.Drawing.Rectangle(x, y, cornerSizeW * 2, cornerSizeH * 2), 180, 90);
            gxPath.AddLine(new System.Drawing.Point(x + cornerSizeW, y), new System.Drawing.Point(x + w - cornerSizeW, y));

            gxPath.AddArc(new System.Drawing.Rectangle(x + w - cornerSizeW * 2, y, cornerSizeW * 2, cornerSizeH * 2), -90, 90);
            gxPath.AddLine(new System.Drawing.Point(x + w, y + cornerSizeH), new System.Drawing.Point(x + w, y + h - cornerSizeH));

            gxPath.AddArc(new System.Drawing.Rectangle(x + w - cornerSizeW * 2, y + h - cornerSizeH * 2, cornerSizeW * 2, cornerSizeH * 2), 0, 90);
            gxPath.AddLine(new System.Drawing.Point(x + w - cornerSizeW, y + h), new System.Drawing.Point(x + cornerSizeW, y + h));

            gxPath.AddArc(new System.Drawing.Rectangle(x, y + h - cornerSizeH * 2, cornerSizeW * 2, cornerSizeH * 2), 90, 90);
            gxPath.AddLine(new System.Drawing.Point(x, y + cornerSizeH), new System.Drawing.Point(x, y + h - cornerSizeH));

            gx.FillPath(System.Drawing.Brushes.Yellow, gxPath);
            gx.DrawPath(System.Drawing.Pens.Red, gxPath);
            gxPath.Dispose();
        }

        InternalRect invalidateArea = InternalRect.CreateFromLTRB(0, 0, 0, 0);
        int canvasFlags = FIRSTTIME_INVALID;

        const int WAIT_FOR_UPDATE = 0x0;

        const int FIRSTTIME_INVALID = 0x1; const int UPDATED_CONTENT = 0x2;
        const int FIRSTTIME_INVALID_AND_UPDATED_CONTENT = 0x3;

        public override InternalRect InvalidateArea
        {
            get
            {
                return invalidateArea;
            }
        }
        public override bool IsContentUpdated
        {
            get
            {
                return ((canvasFlags & UPDATED_CONTENT) == UPDATED_CONTENT);
            }
        }

        public override void Invalidate(InternalRect rect)
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