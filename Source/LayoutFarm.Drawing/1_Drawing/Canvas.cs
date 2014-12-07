//2014 Apache2, WinterDev


namespace LayoutFarm.Drawing
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
        public Canvas()
        {

        }
        public abstract GraphicsPlatform Platform { get; }
        public abstract SmoothingMode SmoothingMode { get; set; }
        //---------------------------------------------------------------------
        public abstract float StrokeWidth { get; set; }
        public abstract Color StrokeColor { get; set; }
        public abstract Color FillColor { get; set; }
        //states
        public abstract void Invalidate(Rect rect);

        public abstract Rect InvalidateArea { get; }
        public bool IsContentReady { get; set; }
        //---------------------------------------------------------------------
        // canvas dimension, canvas origin
        public abstract int Top { get; }
        public abstract int Left { get; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract int Bottom { get; }
        public abstract int Right { get; }
        public abstract Rectangle Rect { get; }
        public abstract void OffsetCanvasOrigin(int dx, int dy);
        public abstract void OffsetCanvasOriginX(int dx);
        public abstract void OffsetCanvasOriginY(int dy);
        public abstract bool IntersectsWith(Rect clientRect);
        public abstract float CanvasOriginX { get; }
        public abstract float CanvasOriginY { get; }
        public abstract void SetCanvasOrigin(float x, float y);
        //---------------------------------------------------------------------
        //clip area
        public abstract bool PushClipAreaForNativeScrollableElement(Rect updateArea);
        public abstract bool PushClipArea(int width, int height, Rect updateArea);
        public abstract void DisableClipArea();
        public abstract void EnableClipArea();
        public abstract void SetClip(RectangleF clip, CombineMode combineMode = CombineMode.Replace);
        public abstract RectangleF GetClip();
        public abstract Rectangle CurrentClipRect { get; }
        public abstract bool PushClipArea(int x, int y, int width, int height);
        public abstract void PopClipArea();
        //---------------------------------------
        //buffer
        public abstract void ClearSurface();
        public abstract void CopyFrom(Canvas sourceCanvas, int logicalSrcX, int logicalSrcY, Rectangle destArea);
        public abstract void RenderTo(System.IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea);
        //-------------------------------------------------------

        //region object
        public abstract RectangleF GetBound(Region rgn);
        public abstract void FillRegion(Region rgn);
        //---------------------------------------


        //text ,font, strings 
        public abstract Font CurrentFont { get; set; }
        public abstract Color CurrentTextColor { get; set; }

        public abstract FontInfo GetFontInfo(Font f);

        public abstract void DrawText(char[] buffer, int x, int y);
        public abstract void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment);
        public abstract void DrawText(char[] buffer, int startAt, int len, Rectangle logicalTextBox, int textAlignment);
        //-------------------------------------------------------

        //lines 
        public abstract void DrawLine(float x1, float y1, float x2, float y2);
        public abstract void DrawLine(PointF p1, PointF p2);
        public abstract void DrawLines(Point[] points);
        //-------------------------------------------------------
        //rects 
        public abstract void FillRectangle(Color color, float left, float top, float right, float bottom); 
        public abstract void FillRectangle(Brush brush, float left, float top, float width, float height); 
        public abstract void DrawRectangle(Color color, float left, float top, float width, float height);
      
        //------------------------------------------------------- 
        //path,  polygons,ellipse spline,contour,  
        public abstract void FillPath(GraphicsPath gfxPath);
        public abstract void FillPath(GraphicsPath gfxPath, Brush brush);
        public abstract void DrawPolygon(PointF[] points);

        public abstract void FillPolygon(PointF[] points);
        public abstract void FillEllipse(Point[] points);
        public abstract void FillEllipse(int x, int y, int width, int height);

        public abstract void DrawRoundRect(int x, int y, int w, int h, Size cornerSize);
        public abstract void DrawBezier(Point[] points);
        public abstract void DrawPath(GraphicsPath gfxPath);
        //------------------------------------------------------- 

        //images
        public abstract void DrawImage(Image image, RectangleF dest, RectangleF src);
        public abstract void DrawImage(Image image, RectangleF rect);
        //---------------------------------------------------------------------------
#if DEBUG
        public abstract void dbug_DrawRuler(int x);
        public abstract void dbug_DrawCrossRect(Color color, Rectangle rect);
#endif

    }
}
