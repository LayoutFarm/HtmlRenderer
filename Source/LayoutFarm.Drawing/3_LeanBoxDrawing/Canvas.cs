//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{

    public abstract class Canvas
    {
#if DEBUG
        public static int dbug_canvasCount = 0;
        public int debug_resetCount = 0;
        public int debug_releaseCount = 0;
        public int debug_canvas_id = 0;
#endif
        const int CANVAS_UNUSED = 1 << (1 - 1);
        const int CANVAS_DIMEN_CHANGED = 1 << (2 - 1);

        public abstract bool AvoidGeometryAntialias
        {
            get;
            set;
        }
        public abstract bool AvoidTextAntialias
        {
            get;
            set;
        }
        public abstract IGraphics2 GetGfx();
        public abstract void MarkAsFirstTimeInvalidateAndUpdateContent();

        public abstract bool IsFromPrinter
        {
            get;
        }
        public abstract bool IsPageNumber(int hPageNum, int vPageNum);

        public abstract bool IsUnused
        {
            get;
            set;
        }
        public abstract bool DimensionInvalid
        {
            get;
            set;
        }

        public abstract SolidBrush GetSharedSolidBrush();

        public abstract void ReleaseUnManagedResource();


        public abstract bool IntersectsWith(InternalRect clientRect);
        public abstract bool PushClipAreaForNativeScrollableElement(InternalRect updateArea);
        public abstract bool PushClipArea(int width, int height, InternalRect updateArea);

        public abstract void DisableClipArea();
        public abstract void EnableClipArea();
        public abstract void SetClip(RectangleF clip, CombineMode combineMode);

        public abstract Rectangle CurrentClipRect
        {
            get;
        }

#if DEBUG

        public abstract int InternalOriginX
        {
            get;
        }
        public abstract int InternalOriginY
        {
            get;
        }

#endif
        public abstract bool PushClipArea(int x, int y, int width, int height);


        public abstract void PopClipArea();
        public abstract int Top { get; }
        public abstract int Left { get; }

        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract int Bottom { get; }

        public abstract int Right { get; }

        public abstract Rectangle Rect
        {
            get;
        }

        public abstract void DrawText(char[] buffer, int x, int y);
        public abstract void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment);

        public const int SAME_FONT_SAME_TEXT_COLOR = 0;
        public const int SAME_FONT_DIFF_TEXT_COLOR = 1;
        public const int DIFF_FONT_SAME_TEXT_COLOR = 2;
        public const int DIFF_FONT_DIFF_TEXT_COLOR = 3;
        public abstract int EvaluateFontAndTextColor(TextFontInfo textFontInfo, Color color);

        public abstract void PushFont(TextFontInfo textFontInfo);
        public abstract void PopFont();
        public abstract void PushFontInfoAndTextColor(TextFontInfo textFontInfo, Color color);

        public abstract void PopFontInfoAndTextColor();

        public abstract void PushTextColor(Color color);

        public abstract void PopTextColor();


        public abstract void CopyFrom(Canvas sourceCanvas, int logicalSrcX, int logicalSrcY, Rectangle destArea);

        public abstract void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea);



        public abstract void OffsetCanvasOrigin(int dx, int dy);

        public abstract void OffsetCanvasOriginX(int dx);

        public abstract void OffsetCanvasOriginY(int dy);


        public abstract void Reuse(int hPageNum, int vPageNum);

        public abstract void Reset(int hPageNum, int vPageNum, int newWidth, int newHeight);

        public abstract void ClearSurface(InternalRect rect);

        public abstract void ClearSurface();
        public abstract void FillPolygon(Brush brush, PointF[] points);

        public abstract void FillPolygon(Brush brush, Point[] points);


        public abstract void FillPolygon(ArtColorBrush colorBrush, Point[] points);

        public abstract void FillRegion(ArtColorBrush colorBrush, Region rgn);

        public abstract void FillPath(GraphicsPath gfxPath, Color solidColor);

        public abstract void FillPath(GraphicsPath gfxPath, Brush colorBrush);

        public abstract void FillPath(GraphicsPath gfxPath, ArtColorBrush colorBrush);

        public abstract void DrawPath(GraphicsPath gfxPath);

        public abstract void DrawPath(GraphicsPath gfxPath, Color color);

        public abstract void DrawPath(GraphicsPath gfxPath, Pen pen);

        public abstract void FillRectangle(Color color, Rectangle rect);

        public abstract void FillRectangle(Color color, RectangleF rectf);

        public abstract void FillRectangle(Brush brush, Rectangle rect);

        public abstract void FillRectangle(Brush brush, RectangleF rectf);

        public abstract void FillRectangle(ArtColorBrush brush, RectangleF rectf);

        public abstract void FillRectangle(Color color, int left, int top, int right, int bottom);

        public abstract float GetBoundWidth(Region rgn);

        public abstract RectangleF GetBound(Region rgn);

        public abstract float GetFontHeight(Font f);

        public abstract Region[] MeasureCharacterRanges(string text, Font f, RectangleF layoutRectF, StringFormat strFormat);

        public abstract Size MeasureString(string str, Font font, float maxWidth, out int charFit, out int charFitWidth);

        public abstract void FillRectangle(ArtColorBrush colorBrush, int left, int top, int right, int bottom);

        public abstract void DrawRectangle(Pen p, Rectangle rect);

        public abstract void DrawRectangle(Pen p, float x, float y, float width, float height);

        public abstract SmoothingMode SmoothingMode
        { get; set; }

        public abstract void DrawRectangle(Color color, int left, int top, int width, int height);

        public abstract void DrawString(string str, Font f, Brush brush, float x, float y);

        public abstract void DrawString(string str, Font f, Brush brush, float x, float y, float w, float h);

        public abstract void DrawRectangle(Color color, float left, float top, float width, float height);

        public abstract void DrawRectangle(Color color, Rectangle rect);

        public abstract void DrawImage(Image image, RectangleF dest, RectangleF src);

        public abstract void DrawImage(Image image, RectangleF rect);

        public abstract void DrawImage(Image image, Rectangle rect);

        public abstract void DrawImage(Bitmap image, int x, int y, int w, int h);

        public abstract void DrawImageUnScaled(Bitmap image, int x, int y);

#if DEBUG
        public abstract void dbug_DrawRuler(int x);

        public abstract void dbug_DrawCrossRect(Color color, Rectangle rect);


#endif
        public abstract void DrawBezire(Point[] points);

        public abstract void DrawLine(Pen pen, Point p1, Point p2);

        public abstract void DrawLine(Color c, int x1, int y1, int x2, int y2);

        public abstract void DrawLine(Color c, float x1, float y1, float x2, float y2);

        public abstract void DrawLine(Pen pen, float x1, float y1, float x2, float y2);

        public abstract void DrawLine(Color color, Point p1, Point p2);

        public abstract void DrawLine(Color color, Point p1, Point p2, DashStyle lineDashStyle);

        public abstract void DrawArc(Pen pen, Rectangle r, float startAngle, float sweepAngle);

        public abstract void DrawLines(Color color, Point[] points);

        public abstract void DrawPolygon(Point[] points);

        public abstract void FillPolygon(Point[] points);

        public abstract void FillEllipse(Point[] points);

        public abstract void FillEllipse(Color color, Rectangle rect);

        public abstract void FillEllipse(Color color, int x, int y, int width, int height);

        public abstract void DrawRoundRect(int x, int y, int w, int h, Size cornerSize);



        public abstract InternalRect InvalidateArea { get; }
        public abstract bool IsContentUpdated { get; }
        public abstract void Invalidate(InternalRect rect);


    }
}
