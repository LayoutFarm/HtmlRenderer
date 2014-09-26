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
        public static System.Drawing.PointF ToPointF(this LayoutFarm.Drawing.PointF pointf)
        {
            return new System.Drawing.PointF(pointf.X, pointf.Y);
        }
        public static LayoutFarm.Drawing.PointF ToPointF(this System.Drawing.PointF pointf)
        {
            return new LayoutFarm.Drawing.PointF(pointf.X, pointf.Y);
        }
        //------------------------------------------------
        public static LayoutFarm.Drawing.Point ToPoint(this System.Drawing.Point pointf)
        {
            return new LayoutFarm.Drawing.Point(pointf.X, pointf.Y);
        }       
        public static System.Drawing.Point ToPoint(this LayoutFarm.Drawing.Point point)
        {
            return new System.Drawing.Point(point.X, point.Y);
        }
        
        //------------------------------------------------
        public static System.Drawing.SizeF ToSizeF(this LayoutFarm.Drawing.SizeF size)
        {
            return new System.Drawing.SizeF(size.Width, size.Height);
        }
        public static LayoutFarm.Drawing.SizeF ToSizeF(this System.Drawing.SizeF size)
        {
            return new LayoutFarm.Drawing.SizeF(size.Width, size.Height);
        }
        
        //------------------------------------------------
        public static LayoutFarm.Drawing.Size ToSize(this System.Drawing.Size size)
        {
            return new LayoutFarm.Drawing.Size(size.Width, size.Height);
        }
        public static System.Drawing.Size ToSize(this LayoutFarm.Drawing.Size size)
        {
            return new System.Drawing.Size(size.Width, size.Height);
        }

        //------------------------------------------------
        public static System.Drawing.RectangleF ToRectF(this LayoutFarm.Drawing.RectangleF rect)
        {
            return new System.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static LayoutFarm.Drawing.RectangleF ToRectF(this System.Drawing.RectangleF rect)
        {
            return new LayoutFarm.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
        //------------------------------------------------
        public static System.Drawing.Rectangle ToRect(this LayoutFarm.Drawing.Rectangle rect)
        {
            return new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static LayoutFarm.Drawing.Rectangle ToRect(this System.Drawing.Rectangle rect)
        {
            return new LayoutFarm.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
         

    }
}