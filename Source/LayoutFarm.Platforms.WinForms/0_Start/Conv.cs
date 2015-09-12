// 2015,2014 ,BSD, WinterDev

namespace LayoutFarm.UI
{
    /// <summary>
    /// conversion helper
    /// </summary>
    public static class Conv
    {
        public static System.Drawing.PointF ToPointF(this PixelFarm.Drawing.PointF pointf)
        {
            return new System.Drawing.PointF(pointf.X, pointf.Y);
        }
        public static PixelFarm.Drawing.PointF ToPointF(this System.Drawing.PointF pointf)
        {
            return new PixelFarm.Drawing.PointF(pointf.X, pointf.Y);
        }
        //------------------------------------------------
        public static PixelFarm.Drawing.Point ToPoint(this System.Drawing.Point pointf)
        {
            return new PixelFarm.Drawing.Point(pointf.X, pointf.Y);
        }
        public static System.Drawing.Point ToPoint(this PixelFarm.Drawing.Point point)
        {
            return new System.Drawing.Point(point.X, point.Y);
        }

        //------------------------------------------------
        public static System.Drawing.SizeF ToSizeF(this PixelFarm.Drawing.SizeF size)
        {
            return new System.Drawing.SizeF(size.Width, size.Height);
        }
        public static PixelFarm.Drawing.SizeF ToSizeF(this System.Drawing.SizeF size)
        {
            return new PixelFarm.Drawing.SizeF(size.Width, size.Height);
        }

        //------------------------------------------------
        public static PixelFarm.Drawing.Size ToSize(this System.Drawing.Size size)
        {
            return new PixelFarm.Drawing.Size(size.Width, size.Height);
        }
        public static System.Drawing.Size ToSize(this PixelFarm.Drawing.Size size)
        {
            return new System.Drawing.Size(size.Width, size.Height);
        }

        //------------------------------------------------
        public static System.Drawing.RectangleF ToRectF(this PixelFarm.Drawing.RectangleF rect)
        {
            return new System.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static PixelFarm.Drawing.RectangleF ToRectF(this System.Drawing.RectangleF rect)
        {
            return new PixelFarm.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
        //------------------------------------------------
        public static System.Drawing.Rectangle ToRect(this PixelFarm.Drawing.Rectangle rect)
        {
            return new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static PixelFarm.Drawing.Rectangle ToRect(this System.Drawing.Rectangle rect)
        {
            return new PixelFarm.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }


    }
}