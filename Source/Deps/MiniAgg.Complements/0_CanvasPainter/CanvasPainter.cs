// 2015,2014 ,BSD, WinterDev

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007-2011
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
//
// Class StringPrinter.cs
// 
// Class to output the vertex source of a string as a run of glyphs.
//----------------------------------------------------------------------------

using PixelFarm.Agg.Fonts;
namespace PixelFarm.Agg
{
    public abstract class CanvasPainter
    {
        public abstract void Clear(ColorRGBA color);
        public abstract RectInt ClipBox { get; set; }

        public abstract void SetClipBox(int x1, int y1, int x2, int y2);
        /// <summary>
        /// draw circle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public abstract void FillCircle(double x, double y, double radius, ColorRGBA color);
        public abstract void FillCircle(double x, double y, double radius);
        public abstract void FillEllipse(double left, double bottom, double right, double top, int nsteps);
        public abstract void DrawEllipse();
        /// <summary>
        /// draw line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public abstract void Line(double x1, double y1, double x2, double y2, ColorRGBA color);
        /// <summary>
        /// draw line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public abstract void Line(double x1, double y1, double x2, double y2);
        public abstract double StrokeWidth { get; set; }
        public abstract void Draw(VertexStore vxs);
        /// <summary>
        /// draw rectangle
        /// </summary>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="color"></param>
        /// <param name="strokeWidth"></param>
        public abstract void Rectangle(double left, double bottom, double right, double top, ColorRGBA color);
        public abstract void Rectangle(double left, double bottom, double right, double top);
        public abstract void FillRectangle(double left, double bottom, double right, double top, ColorRGBA fillColor);
        public abstract void FillRectangle(double left, double bottom, double right, double top);
        public abstract void FillRectLBWH(double left, double bottom, double width, double height);
        public abstract void FillRoundRectangle(double left, double bottom, double right, double top, double radius);
        public abstract void DrawRoundRect(double left, double bottom, double right, double top, double radius);
        //-------------------------------------------------------
        public abstract Font CurrentFont { get; set; }

        public abstract void DrawString(
           string text,
           double x,
           double y);
        /// <summary>
        /// fill vertex store
        /// </summary>
        /// <param name="vxs"></param>
        /// <param name="c"></param>
        public abstract void Fill(VertexStoreSnap snap);
        public abstract void Fill(VertexStore vxs);
        public abstract bool UseSubPixelRendering { get; set; }
        public abstract ColorRGBA FillColor { get; set; }
        public abstract ColorRGBA StrokeColor { get; set; }
        public abstract void PaintSeries(VertexStore vxs, ColorRGBA[] colors, int[] pathIndexs, int numPath);
        public abstract void Fill(VertexStore vxs, ISpanGenerator spanGen);
        public abstract void DrawImage(ActualImage actualImage, double x, double y);
        public abstract void DrawImage(ActualImage actualImage, params Transform.AffinePlan[] affinePlans);
        //----------------------
        /// <summary>
        /// do filter at specific area
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="area"></param>
        public abstract void DoFilterBlurStack(RectInt area, int r);
        public abstract void DoFilterBlurRecursive(RectInt area, int r);
        //---------------- 
        public abstract void DrawBezierCurve(float startX, float startY, float endX, float endY,
           float controlX1, float controlY1,
           float controlX2, float controlY2);
        //---------------- 
        public abstract VertexStore FlattenCurves(VertexStore srcVxs);
        //---------------- 
        public abstract int Width { get; }
        public abstract int Height { get; }
    }
}