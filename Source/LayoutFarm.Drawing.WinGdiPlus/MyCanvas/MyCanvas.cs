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
using HtmlRenderer.Drawing;

namespace LayoutFarm
{

    class MyCanvas : Canvas, IGraphics
    {

        readonly static int[] _charFit = new int[1];
        readonly static int[] _charFitWidth = new int[1000];
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
        float canvasOriginX = 0;
        float canvasOriginY = 0;



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


        InternalRect invalidateArea = InternalRect.CreateFromLTRB(0, 0, 0, 0);
        int canvasFlags = FIRSTTIME_INVALID;

        const int WAIT_FOR_UPDATE = 0x0;

        const int FIRSTTIME_INVALID = 0x1; const int UPDATED_CONTENT = 0x2;
        const int FIRSTTIME_INVALID_AND_UPDATED_CONTENT = 0x3;


        /// <summary>
        /// Used for GDI+ measure string.
        /// </summary>
        static readonly System.Drawing.CharacterRange[] _characterRanges = new System.Drawing.CharacterRange[1];

        /// <summary>
        /// The string format to use for measuring strings for GDI+ text rendering
        /// </summary>
        static readonly System.Drawing.StringFormat _stringFormat;
        /// </summary>
        IntPtr _hdc;
        /// <summary>
        /// Init static resources.
        /// </summary>
        static MyCanvas()
        {
            _stringFormat = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericDefault);
            _stringFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip | System.Drawing.StringFormatFlags.MeasureTrailingSpaces;
        } 
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

        public override IGraphics GetIGraphics()
        {
            return this;
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
        public override object GetGfx()
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
                updateArea.ToRectangle().ToRect());

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
                    updateArea.ToRectangle().ToRect(),
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
       
        public override Rectangle CurrentClipRect
        {
            get
            {
                return currentClipRect.ToRect();
            }
        }
         

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
                    logicalTextBox.ToRect());
            }
            else
            {
                IntPtr gxdc = gx.GetHdc();
                MyWin32.SetViewportOrgEx(gxdc, internalCanvasOriginX, internalCanvasOriginY, IntPtr.Zero);
                System.Drawing.Rectangle clipRect =
                    System.Drawing.Rectangle.Intersect(logicalTextBox.ToRect(), currentClipRect);
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
            MyCanvas s1 = (MyCanvas)sourceCanvas;

            if (s1.gx != null)
            {
                int phySrcX = logicalSrcX - s1.left;
                int phySrcY = logicalSrcY - s1.top;

                System.Drawing.Rectangle postIntersect =
                    System.Drawing.Rectangle.Intersect(currentClipRect, destArea.ToRect());
                phySrcX += postIntersect.X - destArea.X;
                phySrcY += postIntersect.Y - destArea.Y;
                destArea = postIntersect.ToRect();

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

            gx.FillRectangle(internalBrush, rect.ToRect());
        }
        public override void FillRectangle(Color color, RectangleF rectf)
        {
            internalBrush.Color = ConvColor(color);
            gx.FillRectangle(internalBrush, rectf.ToRectF());
        }
        public override void FillRectangle(Brush brush, Rectangle rect)
        {
            gx.FillRectangle(ConvBrush(brush), rect.ToRect());
        }
        public override void FillRectangle(Brush brush, RectangleF rectf)
        {
            gx.FillRectangle(ConvBrush(brush), rectf.ToRectF());
        }
        public override void FillRectangle(ArtColorBrush brush, RectangleF rectf)
        {

            gx.FillRectangle(ConvBrush(brush.myBrush), rectf.ToRectF());
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
            return (ConvRgn(rgn).GetBounds(gx)).ToRectF();

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
            gx.DrawRectangle(ConvPen(p), rect.ToRect());

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
            gx.DrawRectangle(internalPen, rect.ToRect());

        }
        
        public override void DrawImage(Image image, Rectangle rect)
        {
            gx.DrawImage(
                ConvBitmap(image),
                rect.ToRect());
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
            gx.DrawLine(ConvPen(pen), p1.ToPoint(), p2.ToPoint());
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
       
        public override void DrawLine(Color color, Point p1, Point p2)
        {
            System.Drawing.Color prevColor = internalPen.Color;
            internalPen.Color = ConvColor(color);
            gx.DrawLine(internalPen, p1.ToPoint(), p2.ToPoint());
            internalPen.Color = prevColor;
        }
        public override void DrawLine(Color color, Point p1, Point p2, DashStyle lineDashStyle)
        {
            System.Drawing.Drawing2D.DashStyle prevLineDashStyle = (System.Drawing.Drawing2D.DashStyle)internalPen.DashStyle;
            internalPen.DashStyle = (System.Drawing.Drawing2D.DashStyle)lineDashStyle;

            internalPen.Color = ConvColor(color);
            gx.DrawLine(internalPen,
                p1.ToPoint(),
                p2.ToPoint());
            internalPen.DashStyle = prevLineDashStyle;

        }
        public override void DrawArc(Pen pen, Rectangle r, float startAngle, float sweepAngle)
        {
            gx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gx.DrawArc(ConvPen(pen),
                r.ToRect(),
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
            gx.FillEllipse(internalBrush, rect.ToRect());

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


       
        public Canvas CurrentCanvas
        {
            get { return this; }
        }
        public GraphicPlatform Platform
        {
            get { return LayoutFarm.Drawing.CurrentGraphicPlatform.P; }
        }

        public object GetInnerGraphic()
        {
            return this.gx;
        }
        public void SetCanvasOrigin(float x, float y)
        {
            ReleaseHdc();
            //-----------
            this.gx.TranslateTransform(-this.canvasOriginX, -this.canvasOriginY);
            this.gx.TranslateTransform(x, y);

            this.canvasOriginX = x;
            this.canvasOriginY = y;
        }
        public float CanvasOriginX
        {
            get { return this.canvasOriginX; }

        }
        public float CanvasOriginY
        {
            get { return this.canvasOriginY; }
        }
        /// <summary>
        /// Gets the bounding clipping region of this graphics.
        /// </summary>
        /// <returns>The bounding rectangle for the clipping region</returns>
        public LayoutFarm.Drawing.RectangleF GetClip()
        {
            if (_hdc == IntPtr.Zero)
            {
                var clip1 = gx.ClipBounds;
                return new LayoutFarm.Drawing.RectangleF(
                    clip1.X, clip1.Y,
                    clip1.Width, clip1.Height);
            }
            else
            {
                System.Drawing.Rectangle lprc;
                Win32Utils.GetClipBox(_hdc, out lprc);


                return new LayoutFarm.Drawing.RectangleF(
                    lprc.X, lprc.Y,
                    lprc.Width, lprc.Height);
            }
        }

        /// <summary>
        /// Sets the clipping region of this <see cref="T:System.Drawing.Graphics"/> to the result of the specified operation combining the current clip region and the rectangle specified by a <see cref="T:System.Drawing.RectangleF"/> structure.
        /// </summary>
        /// <param name="rect"><see cref="T:System.Drawing.RectangleF"/> structure to combine. </param>
        /// <param name="combineMode">Member of the <see cref="T:System.Drawing.Drawing2D.CombineMode"/> enumeration that specifies the combining operation to use. </param>
        public override void SetClip(RectangleF rect, CombineMode combineMode = CombineMode.Replace)
        {
            ReleaseHdc();
            gx.SetClip(rect.ToRectF(), (System.Drawing.Drawing2D.CombineMode)combineMode);
        }

        /// <summary>
        /// Measure the width and height of string <paramref name="str"/> when drawn on device context HDC
        /// using the given font <paramref name="font"/>.
        /// </summary>
        /// <param name="str">the string to measure</param>
        /// <param name="font">the font to measure string with</param>
        /// <returns>the size of the string</returns>
        public Size MeasureString(string str, Font font)
        {
            if (_useGdiPlusTextRendering)
            {
                ReleaseHdc();
                _characterRanges[0] = new System.Drawing.CharacterRange(0, str.Length);
                _stringFormat.SetMeasurableCharacterRanges(_characterRanges);

                var font2 = font.InnerFont as System.Drawing.Font;
                var size = gx.MeasureCharacterRanges(str,
                    font2,
                    System.Drawing.RectangleF.Empty,
                    _stringFormat)[0].GetBounds(gx).Size;

                return new Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));
            }
            else
            {
                SetFont(font);

                var size = new System.Drawing.Size();
                Win32Utils.GetTextExtentPoint32(_hdc, str, str.Length, ref size);
                return size.ToSize();

            }
        }
        public Size MeasureString2(char[] buff, int startAt, int len, Font font)
        {
            if (_useGdiPlusTextRendering)
            {
                ReleaseHdc();
                _characterRanges[0] = new System.Drawing.CharacterRange(0, len);
                _stringFormat.SetMeasurableCharacterRanges(_characterRanges);
                System.Drawing.Font font2 = (System.Drawing.Font)font.InnerFont;

                var size = gx.MeasureCharacterRanges(
                    new string(buff, startAt, len),
                    font2,
                    System.Drawing.RectangleF.Empty,
                    _stringFormat)[0].GetBounds(gx).Size;
                return new LayoutFarm.Drawing.Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));
            }
            else
            {
                SetFont(font);
                var size = new System.Drawing.Size();
                unsafe
                {
                    fixed (char* startAddr = &buff[0])
                    {
                        Win32Utils.UnsafeGetTextExtentPoint32(_hdc, startAddr + startAt, len, ref size);
                    }
                }
                return size.ToSize();
            }
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
        public Size MeasureString2(char[] buff, int startAt, int len, Font font, float maxWidth, out int charFit, out int charFitWidth)
        {
            if (_useGdiPlusTextRendering)
            {
                ReleaseHdc();
                throw new NotSupportedException("Char fit string measuring is not supported for GDI+ text rendering");
            }
            else
            {
                SetFont(font);

                var size = new System.Drawing.Size();
                unsafe
                {
                    fixed (char* startAddr = &buff[0])
                    {
                        Win32Utils.UnsafeGetTextExtentExPoint(
                            _hdc, startAddr + startAt, len,
                            (int)Math.Round(maxWidth), _charFit, _charFitWidth, ref size);
                    }

                }
                charFit = _charFit[0];
                charFitWidth = charFit > 0 ? _charFitWidth[charFit - 1] : 0;
                return size.ToSize();
            }
        }
        public override Size MeasureString(string str, LayoutFarm.Drawing.Font font,
            float maxWidth, out int charFit, out int charFitWidth)
        {
            if (_useGdiPlusTextRendering)
            {
                ReleaseHdc();
                throw new NotSupportedException("Char fit string measuring is not supported for GDI+ text rendering");
            }
            else
            {
                SetFont(font);

                var size = new System.Drawing.Size();

                Win32Utils.GetTextExtentExPoint(
                    _hdc, str, str.Length,
                    (int)Math.Round(maxWidth), _charFit, _charFitWidth, ref size);
                charFit = _charFit[0];
                charFitWidth = charFit > 0 ? _charFitWidth[charFit - 1] : 0;
                return size.ToSize();
            }
        }
#if DEBUG
        static class dbugCounter
        {
            public static int dbugDrawStringCount;
        }
#endif
        public void DrawString(char[] str, int startAt, int len, Font font, Color color, PointF point, SizeF size)
        {

#if DEBUG
            dbugCounter.dbugDrawStringCount++;
#endif
            if (_useGdiPlusTextRendering)
            {
                //ReleaseHdc();
                //_g.DrawString(
                //    new string(str, startAt, len),
                //    font,
                //    RenderUtils.GetSolidBrush(color),
                //    (int)Math.Round(point.X + canvasOriginX - FontsUtils.GetFontLeftPadding(font) * .8f),
                //    (int)Math.Round(point.Y + canvasOriginY));

            }
            else
            {
                if (color.A == 255)
                {
                    SetFont(font);
                    SetTextColor(color);
                    unsafe
                    {
                        fixed (char* startAddr = &str[0])
                        {
                            Win32Utils.TextOut2(_hdc, (int)Math.Round(point.X + canvasOriginX),
                                (int)Math.Round(point.Y + canvasOriginY), (startAddr + startAt), len);
                        }
                    }
                }
                else
                {
                    //translucent / transparent text
                    InitHdc();
                    unsafe
                    {
                        fixed (char* startAddr = &str[0])
                        {
                            Win32Utils.TextOut2(_hdc, (int)Math.Round(point.X + canvasOriginX),
                                (int)Math.Round(point.Y + canvasOriginY), (startAddr + startAt), len);
                        }
                    }

                    //DrawTransparentText(_hdc, str, font, new Point((int)Math.Round(point.X), (int)Math.Round(point.Y)), Size.Round(size), color);
                }
            }
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ReleaseHdc();
        }


        #region Delegate graphics methods

        /// <summary>
        /// Gets or sets the rendering quality for this <see cref="T:System.Drawing.Graphics"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Drawing.Drawing2D.SmoothingMode"/> values.
        /// </returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override SmoothingMode SmoothingMode
        {
            get
            {
                ReleaseHdc();
                return (SmoothingMode)(gx.SmoothingMode);
            }
            set
            {
                ReleaseHdc();
                gx.SmoothingMode = (System.Drawing.Drawing2D.SmoothingMode)value;
            }
        }

        /// <summary>
        /// Draws a line connecting the two points specified by the coordinate pairs.
        /// </summary>
        /// <param name="pen"><see cref="T:System.Drawing.Pen"/> that determines the color, width, and style of the line. </param><param name="x1">The x-coordinate of the first point. </param><param name="y1">The y-coordinate of the first point. </param><param name="x2">The x-coordinate of the second point. </param><param name="y2">The y-coordinate of the second point. </param><exception cref="T:System.ArgumentNullException"><paramref name="pen"/> is null.</exception>
        public override void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            ReleaseHdc();
            gx.DrawLine(pen.InnerPen as System.Drawing.Pen, x1, y1, x2, y2);

            //System.Drawing.Color prevColor = internalPen.Color;
            //internalPen.Color = ConvColor(c);
            //gx.DrawLine(internalPen, x1, y1, x2, y2);
            //internalPen.Color = prevColor;

        }

        /// <summary>
        /// Draws a rectangle specified by a coordinate pair, a width, and a height.
        /// </summary>
        /// <param name="pen">A <see cref="T:System.Drawing.Pen"/> that determines the color, width, and style of the rectangle. </param><param name="x">The x-coordinate of the upper-left corner of the rectangle to draw. </param><param name="y">The y-coordinate of the upper-left corner of the rectangle to draw. </param><param name="width">The width of the rectangle to draw. </param><param name="height">The height of the rectangle to draw. </param><exception cref="T:System.ArgumentNullException"><paramref name="pen"/> is null.</exception>
        public override void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            ReleaseHdc();
            gx.DrawRectangle((System.Drawing.Pen)pen.InnerPen, x, y, width, height);
        }

        public void FillRectangle(Brush getSolidBrush, float left, float top, float width, float height)
        {
            ReleaseHdc();
            gx.FillRectangle((System.Drawing.Brush)getSolidBrush.InnerBrush, left, top, width, height);
        }

        /// <summary>
        /// Draws the specified portion of the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param>
        /// <param name="destRect"><see cref="T:System.Drawing.RectangleF"/> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle. </param>
        /// <param name="srcRect"><see cref="T:System.Drawing.RectangleF"/> structure that specifies the portion of the <paramref name="image"/> object to draw. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception>
        public override void DrawImage(Image image, RectangleF destRect, RectangleF srcRect)
        {
            ReleaseHdc();
            gx.DrawImage(image.InnerImage as System.Drawing.Image,
                destRect.ToRectF(),
                srcRect.ToRectF(),
                System.Drawing.GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Draws the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param><param name="destRect"><see cref="T:System.Drawing.Rectangle"/> structure that specifies the location and size of the drawn image. </param><exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override void DrawImage(Image image, RectangleF destRect)
        {
            ReleaseHdc();
            gx.DrawImage(image.InnerImage as System.Drawing.Image, destRect.ToRectF());
        }

        /// <summary>
        /// Draws a <see cref="T:System.Drawing.Drawing2D.GraphicsPath"/>.
        /// </summary>
        /// <param name="pen"><see cref="T:System.Drawing.Pen"/> that determines the color, width, and style of the path. </param><param name="path"><see cref="T:System.Drawing.Drawing2D.GraphicsPath"/> to draw. </param><exception cref="T:System.ArgumentNullException"><paramref name="pen"/> is null.-or-<paramref name="path"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public void DrawPath(Pen pen, GraphicsPath path)
        {
            gx.DrawPath(pen.InnerPen as System.Drawing.Pen,
                path.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }

        /// <summary>
        /// Fills the interior of a <see cref="T:System.Drawing.Drawing2D.GraphicsPath"/>.
        /// </summary>
        /// <param name="brush"><see cref="T:System.Drawing.Brush"/> that determines the characteristics of the fill. </param><param name="path"><see cref="T:System.Drawing.Drawing2D.GraphicsPath"/> that represents the path to fill. </param><exception cref="T:System.ArgumentNullException"><paramref name="brush"/> is null.-or-<paramref name="path"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public void FillPath(Brush brush, GraphicsPath path)
        {
            ReleaseHdc();
            gx.FillPath(brush.InnerBrush as System.Drawing.Brush,
                path.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }

        /// <summary>
        /// Fills the interior of a polygon defined by an array of points specified by <see cref="T:System.Drawing.PointF"/> structures.
        /// </summary>
        /// <param name="brush"><see cref="T:System.Drawing.Brush"/> that determines the characteristics of the fill. </param><param name="points">Array of <see cref="T:System.Drawing.PointF"/> structures that represent the vertices of the polygon to fill. </param><exception cref="T:System.ArgumentNullException"><paramref name="brush"/> is null.-or-<paramref name="points"/> is null.</exception>
        public override void FillPolygon(Brush brush, PointF[] points)
        {
            ReleaseHdc();
            //create Point
            var pps = ConvPointFArray(points);
            //System.Drawing.PointF[] pps = new System.Drawing.PointF[points.Length];
            ////?
            //int j = points.Length;
            //for (int i = 0; i < j; ++i)
            //{
            //    pps[i] = points[i].ToPointF();
            //}
            gx.FillPolygon(brush.InnerBrush as System.Drawing.Brush, pps);

        }

        #endregion


        #region Private methods

        /// <summary>
        /// Init HDC for the current graphics object to be used to call GDI directly.
        /// </summary>
        private void InitHdc()
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
        private void SetFont(Font font)
        {
            InitHdc();
            Win32Utils.SelectObject(_hdc, HtmlRenderer.Drawing.FontsUtils.GetCachedHFont(font.InnerFont as System.Drawing.Font));
        }

        /// <summary>
        /// Set the text color of the device context.
        /// </summary>
        private void SetTextColor(Color color)
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
        private static void DrawTransparentText(IntPtr hdc, string str, Font font, Point point, Size size, Color color)
        {
            IntPtr dib;
            var memoryHdc = Win32Utils.CreateMemoryHdc(hdc, size.Width, size.Height, out dib);

            try
            {
                // copy target background to memory HDC so when copied back it will have the proper background
                Win32Utils.BitBlt(memoryHdc, 0, 0, size.Width, size.Height, hdc, point.X, point.Y, Win32Utils.BitBltCopy);

                // Create and select font
                Win32Utils.SelectObject(memoryHdc, HtmlRenderer.Drawing.FontsUtils.GetCachedHFont(font.InnerFont as System.Drawing.Font));
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

        //=====================================================
        public LayoutFarm.Drawing.FontInfo GetFontInfo(Font f)
        {
            return HtmlRenderer.Drawing.FontsUtils.GetCachedFont(f.InnerFont as System.Drawing.Font);
        }
        public LayoutFarm.Drawing.FontInfo GetFontInfo(string fontname, float fsize, FontStyle st)
        {
            return HtmlRenderer.Drawing.FontsUtils.GetCachedFont(fontname, fsize, (System.Drawing.FontStyle)st);
        }
        public float MeasureWhitespace(LayoutFarm.Drawing.Font f)
        {
            return HtmlRenderer.Drawing.FontsUtils.MeasureWhitespace(this, f);
        }

        #endregion
    }

}