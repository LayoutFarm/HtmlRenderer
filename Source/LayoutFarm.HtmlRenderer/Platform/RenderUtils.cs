//BSD, 2014,
//ArthurHub, Jose Manuel Menendez Poo

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

//MIT, 2018-present, WinterDev

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
namespace LayoutFarm
{
    /// <summary>
    /// Provides some drawing functionallity
    /// </summary>
    static class RenderUtils
    {

        /// <summary>
        /// image used to draw loading image icon
        /// </summary>
        static Image _loadImage;
        /// <summary>
        /// image used to draw error image icon
        /// </summary>
        static Image _errorImage;
        /// <summary>
        /// Check if the given color is visible if painted (has alpha and color values)
        /// </summary>
        /// <param name="color">the color to check</param>
        /// <returns>true - visible, false - not visible</returns>
        public static bool IsColorVisible(Color color)
        {
            return color.A > 0;
        }

        /// <summary>
        /// Draw image loading icon.
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="r">the rectangle to draw icon in</param>
        public static void DrawImageLoadingIcon(DrawBoard g, RectangleF r)
        {
            g.DrawRectangle(KnownColors.LightGray, r.Left + 3, r.Top + 3, 13, 14);
            var image = GetLoadImage();
            g.DrawImage(image, new RectangleF(r.Left + 4, r.Top + 4, image.Width, image.Height));
        }
        public static void DrawImageLoadingIcon(LayoutFarm.HtmlBoxes.PaintVisitor p, RectangleF r)
        {
            p.DrawRectangle(KnownColors.LightGray, r.Left + 3, r.Top + 3, 13, 14);
            var image = GetLoadImage();
            p.DrawImage(image, new RectangleF(r.Left + 4, r.Top + 4, image.Width, image.Height));

        }
        /// <summary>
        /// Draw image failed to load icon.
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="r">the rectangle to draw icon in</param>
        public static void DrawImageErrorIcon(DrawBoard g, RectangleF r)
        {
            g.DrawRectangle(KnownColors.LightGray, r.Left + 2, r.Top + 2, 15, 15);
            var image = GetErrorImage();
            g.DrawImage(image, new RectangleF(r.Left + 3, r.Top + 3, image.Width, image.Height));
        }

        /// <summary>
        /// Draw image failed to load icon.
        /// </summary>
        /// <param name="p">the device to draw into</param>
        /// <param name="r">the rectangle to draw icon in</param>
        public static void DrawImageErrorIcon(LayoutFarm.HtmlBoxes.PaintVisitor p, RectangleF r)
        {
            p.DrawRectangle(KnownColors.LightGray, r.Left + 2, r.Top + 2, 15, 15);
            var image = GetErrorImage();
            p.DrawImage(image, new RectangleF(r.Left + 3, r.Top + 3, image.Width, image.Height));
        }

        public static void WriteRoundRect(PixelFarm.Drawing.VertexStore inputVxs,
            RectangleF rect, float nwRadius, float neRadius, float seRadius, float swRadius)
        {
            //  NW-----NE
            //  |       |
            //  |       |
            //  SW-----SE
            using (Tools.BorrowRoundedRect(out PixelFarm.CpuBlit.VertexProcessing.RoundedRect roundRect))
            {
                //TODO: review here again
                roundRect.SetRect(rect.Left, rect.Bottom, rect.Right, rect.Top);
                roundRect.SetRadius(swRadius, seRadius, neRadius, nwRadius);
                roundRect.MakeVxs(inputVxs);
            }

        }


        /// <summary>
        /// Get singleton instance of load image.
        /// </summary>
        /// <returns>image instance</returns>
        private static Image GetLoadImage()
        {
            //if (_loadImage == null)
            //    _loadImage = loadingImage;
            return _loadImage;
        }

        /// <summary>
        /// Get singleton instance of error image.
        /// </summary>
        /// <returns>image instance</returns>
        private static Image GetErrorImage()
        {
            //if (_errorImage == null)
            //    _errorImage = errorImage;
            return _errorImage;
        }


    }
}