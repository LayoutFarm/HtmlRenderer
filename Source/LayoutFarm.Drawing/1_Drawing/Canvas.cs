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
        //const int CANVAS_UNUSED = 1 << (1 - 1);
        //const int CANVAS_DIMEN_CHANGED = 1 << (2 - 1);
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

        public abstract int CanvasOriginX { get; }
        public abstract int CanvasOriginY { get; }
        public abstract void SetCanvasOrigin(int x, int y);
        public abstract bool IntersectsWith(Rect clientRect);
        //---------------------------------------------------------------------
        //clip area

        public abstract bool PushClipArea(int width, int height, ref Rect updateArea);
        public abstract void PopClipArea();

        public abstract void SetClip(RectangleF clip, CombineMode combineMode = CombineMode.Replace);
        public abstract Rectangle CurrentClipRect { get; }
        //---------------------------------------
        //buffer
        public abstract void ClearSurface(LayoutFarm.Drawing.Color c);
        public abstract void CopyFrom(Canvas sourceCanvas, int logicalSrcX, int logicalSrcY, Rectangle destArea);
        public abstract void RenderTo(System.IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea);
        //-------------------------------------------------------



        //--------------------------------------- 
        //text ,font, strings 
        public abstract Font CurrentFont { get; set; }
        public abstract Color CurrentTextColor { get; set; }

        public abstract void DrawText(char[] buffer, int x, int y);
        public abstract void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment);
        public abstract void DrawText(char[] buffer, int startAt, int len, Rectangle logicalTextBox, int textAlignment);
        //-------------------------------------------------------

        //lines 
        public abstract void DrawLine(float x1, float y1, float x2, float y2);
        //-------------------------------------------------------
        //rects 
        public abstract void FillRectangle(Color color, float left, float top, float width, float height);
        public abstract void FillRectangle(Brush brush, float left, float top, float width, float height);
        public abstract void DrawRectangle(Color color, float left, float top, float width, float height);

        //------------------------------------------------------- 
        //path,  polygons,ellipse spline,contour,  
        public abstract void FillPath(GraphicsPath gfxPath);
        public abstract void FillPath(GraphicsPath gfxPath, Brush brush);
        public abstract void DrawPath(GraphicsPath gfxPath);

        public abstract void FillPolygon(PointF[] points);
        //-------------------------------------------------------  
        //images
        public abstract void DrawImage(Image image, RectangleF dest, RectangleF src);
        public abstract void DrawImage(Image image, RectangleF rect);
        //---------------------------------------------------------------------------
#if DEBUG
        public abstract void dbug_DrawRuler(int x);
        public abstract void dbug_DrawCrossRect(Color color, Rectangle rect);
#endif
        //-------------------------------------------------------  
        public void OffsetCanvasOrigin(int dx, int dy)
        {
            this.SetCanvasOrigin(this.CanvasOriginX + dx, this.CanvasOriginY + dy);
        }
        public void OffsetCanvasOriginX(int dx)
        {
            this.OffsetCanvasOrigin(dx, 0);
        }
        public void OffsetCanvasOriginY(int dy)
        {
            this.OffsetCanvasOrigin(0, dy);
        }
    }
}
