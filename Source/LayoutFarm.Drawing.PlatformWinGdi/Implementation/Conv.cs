//BSD 2014, WinterDev
using System;
using System.Collections.Generic;


namespace LayoutFarm.Drawing
{
    /// <summary>
    /// conversion helper
    /// </summary>
    public static class Conv
    {
        public static System.Drawing.PointF ConvFromPointF(LayoutFarm.Drawing.PointF pointf)
        {
            return new System.Drawing.PointF(pointf.X, pointf.Y);
        }
        public static LayoutFarm.Drawing.PointF ConvToPointF(System.Drawing.PointF pointf)
        {
            return new LayoutFarm.Drawing.PointF(pointf.X, pointf.Y);
        }
        //------------------------------------------------
        public static System.Drawing.Point ConvFromPoint(LayoutFarm.Drawing.Point point)
        {
            return new System.Drawing.Point(point.X, point.Y);
        }
        public static LayoutFarm.Drawing.Point ConvToPoint(System.Drawing.Point point)
        {
            return new LayoutFarm.Drawing.Point(point.X, point.Y);
        }
        //------------------------------------------------

        public static System.Drawing.SizeF ConvFromSizeF(LayoutFarm.Drawing.SizeF size)
        {
            return new System.Drawing.SizeF(size.Width, size.Height);
        }
        public static LayoutFarm.Drawing.SizeF ConvToSizeF(System.Drawing.SizeF size)
        {
            return new LayoutFarm.Drawing.SizeF(size.Width, size.Height);
        }
        public static LayoutFarm.Drawing.Size ConvToSize(System.Drawing.Size size)
        {
            return new LayoutFarm.Drawing.Size(size.Width, size.Height);
        }
        public static System.Drawing.Size ConvFromSize(LayoutFarm.Drawing.Size size)
        {
            return new System.Drawing.Size(size.Width, size.Height);
        }
        //------------------------------------------------
        public static System.Drawing.RectangleF ConvFromRectF(LayoutFarm.Drawing.RectangleF rect)
        {
            return new System.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static LayoutFarm.Drawing.RectangleF ConvToRectF(System.Drawing.RectangleF rect)
        {
            return new LayoutFarm.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
        //------------------------------------------------
        public static System.Drawing.Rectangle ConvFromRect(LayoutFarm.Drawing.Rectangle rect)
        {
            return new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static LayoutFarm.Drawing.Rectangle ConvToRect(System.Drawing.Rectangle rect)
        {
            return new LayoutFarm.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
        //------------------------------------------------
        public static System.Drawing.Drawing2D.CombineMode FromCombineMode(LayoutFarm.Drawing.CombineMode mode)
        {
            return (System.Drawing.Drawing2D.CombineMode)mode;
        }
        public static LayoutFarm.Drawing.CombineMode ToCombineMode(System.Drawing.Drawing2D.CombineMode mode)
        {
            return (LayoutFarm.Drawing.CombineMode)mode;
        }
        //------------------------------------------------
        public static System.Drawing.Drawing2D.SmoothingMode FromSmoothMode(LayoutFarm.Drawing.SmoothingMode mode)
        {
            return (System.Drawing.Drawing2D.SmoothingMode)mode;
        }
        public static LayoutFarm.Drawing.SmoothingMode ToCombineMode(System.Drawing.Drawing2D.SmoothingMode mode)
        {
            return (LayoutFarm.Drawing.SmoothingMode)mode;
        }
        public static System.Drawing.FontStyle FromFonStyle(LayoutFarm.Drawing.FontStyle fontstyle)
        {
            return (System.Drawing.FontStyle)fontstyle;
        }

    }
}