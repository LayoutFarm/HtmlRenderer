//MIT, 2014-2016, WinterDev

namespace PixelFarm.Drawing.Skia
{
    /// <summary>
    /// conversion helper
    /// </summary>
    static class Conv
    {
        public static SkiaSharp.SKPoint ToPointF(this PixelFarm.Drawing.PointF pointf)
        {
            return new SkiaSharp.SKPoint(pointf.X, pointf.Y);
        }
        public static PixelFarm.Drawing.PointF ToPointF(this SkiaSharp.SKPoint pointf)
        {
            return new PixelFarm.Drawing.PointF(pointf.X, pointf.Y);
        }
        //------------------------------------------------
        public static PixelFarm.Drawing.Point ToPoint(this SkiaSharp.SKPoint pointf)
        {
            return new PixelFarm.Drawing.Point((int)pointf.X, (int)pointf.Y);
        }
        public static SkiaSharp.SKPoint ToPoint(this PixelFarm.Drawing.Point point)
        {
            return new SkiaSharp.SKPoint(point.X, point.Y);
        }

        //------------------------------------------------
        public static SkiaSharp.SKSize ToSizeF(this PixelFarm.Drawing.SizeF size)
        {
            return new SkiaSharp.SKSize(size.Width, size.Height);
        }
        //public static PixelFarm.Drawing.SizeF ToSizeF(this System.Drawing.SizeF size)
        //{
        //    return new PixelFarm.Drawing.SizeF(size.Width, size.Height);
        //}

        //------------------------------------------------
        public static PixelFarm.Drawing.Size ToSize(this SkiaSharp.SKSizeI size)
        {
            return new PixelFarm.Drawing.Size(size.Width, size.Height);
        }
        public static SkiaSharp.SKSizeI ToSize(this PixelFarm.Drawing.Size size)
        {
            return new SkiaSharp.SKSizeI(size.Width, size.Height);
        }

        //------------------------------------------------
        public static SkiaSharp.SKRect ToRectF(this PixelFarm.Drawing.RectangleF rect)
        {
            return SkiaSharp.SKRect.Create(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static PixelFarm.Drawing.RectangleF ToRectF(this SkiaSharp.SKRect rect)
        {
            return new PixelFarm.Drawing.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height);
        }
        //------------------------------------------------
        public static SkiaSharp.SKRectI ToRect(this PixelFarm.Drawing.Rectangle rect)
        {
            return SkiaSharp.SKRectI.Create(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static PixelFarm.Drawing.Rectangle ToRect(this SkiaSharp.SKRect rect)
        {
            return new PixelFarm.Drawing.Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
        }
    }
}