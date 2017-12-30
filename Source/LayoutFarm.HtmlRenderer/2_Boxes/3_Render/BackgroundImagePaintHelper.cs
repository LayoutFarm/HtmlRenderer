//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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
using PixelFarm.Drawing;
using LayoutFarm.Css;
namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// Contains all the paint code to paint different background images.
    /// </summary>
    static class BackgroundImagePaintHelper
    {
        /// <summary>
        /// Draw the background image of the given box in the given rectangle.<br/>
        /// Handle background-repeat and background-position values.
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="box">the box to draw its background image</param>
        /// <param name="imageLoadHandler">the handler that loads image to draw</param>
        /// <param name="rectangle">the rectangle to draw image in</param>
        public static void DrawBackgroundImage(DrawBoard g, CssBox box, ImageBinder imageBinder, RectangleF rectangle)
        {
            var image = imageBinder.Image;
            //temporary comment image scale code 
            var imgSize = image.Size;
            //new Size(imageLoadHandler.Rectangle == Rectangle.Empty ? imageLoadHandler.Image.Width : imageLoadHandler.Rectangle.Width,
            //                 imageLoadHandler.Rectangle == Rectangle.Empty ? imageLoadHandler.Image.Height : imageLoadHandler.Rectangle.Height);

            // get the location by BackgroundPosition value
            var location = GetLocation(box.BackgroundPositionX, box.BackgroundPositionY, rectangle, imgSize);
            //var srcRect = imageLoadHandler.Rectangle == Rectangle.Empty
            //                  ? new Rectangle(0, 0, imgSize.Width, imgSize.Height)
            //                  : new Rectangle(imageLoadHandler.Rectangle.Left, imageLoadHandler.Rectangle.Top, imgSize.Width, imgSize.Height);
            var srcRect = new Rectangle(0, 0, image.Width, image.Height);
            // initial image destination rectangle
            var destRect = new Rectangle(location, imgSize);
            // need to clip so repeated image will be cut on rectangle

            var prevClip = g.CurrentClipRect;
            PixelFarm.Drawing.Rectangle copyRect = new PixelFarm.Drawing.Rectangle(
               (int)rectangle.X,
               (int)rectangle.Y,
               (int)rectangle.Width,
               (int)rectangle.Height);
            copyRect.Intersect(prevClip);
            g.SetClipRect(copyRect);
            switch (box.BackgroundRepeat)
            {
                case CssBackgroundRepeat.NoRepeat:
                    g.DrawImage(image, new RectangleF(location, imgSize), new RectangleF(0, 0, image.Width, image.Height));
                    break;
                case CssBackgroundRepeat.RepeatX:
                    DrawRepeatX(g, image, rectangle, srcRect, destRect, imgSize);
                    break;
                case CssBackgroundRepeat.RepeatY:
                    DrawRepeatY(g, image, rectangle, srcRect, destRect, imgSize);
                    break;
                default:
                    DrawRepeat(g, image, rectangle, srcRect, destRect, imgSize);
                    break;
            }

            g.SetClipRect(prevClip);
        }


        #region Private methods

        /// <summary>
        /// Get top-left location to start drawing the image at depending on background-position value.
        /// </summary>
        /// <param name="backgroundPosition">the background-position value</param>
        /// <param name="rectangle">the rectangle to position image in</param>
        /// <param name="imgSize">the size of the image</param>
        /// <returns>the top-left location</returns>
        static Point GetLocation(CssLength posX, CssLength posY, RectangleF rectangle, Size imgSize)
        {
            int left = (int)rectangle.Left;
            int top = (int)rectangle.Top;
            if (posX.IsBackgroundPositionName)
            {
                switch (posX.UnitOrNames)
                {
                    case CssUnitOrNames.LEFT:
                        {
                            left = (int)(rectangle.Left + .5f);
                        }
                        break;
                    case CssUnitOrNames.RIGHT:
                        {
                            left = (int)rectangle.Right - imgSize.Width;
                        }
                        break;
                }
            }
            else
            {
                //not complete !
                left = (int)(rectangle.Left + (rectangle.Width - imgSize.Width) / 2 + .5f);
            }
            if (posY.IsBackgroundPositionName)
            {
                switch (posY.UnitOrNames)
                {
                    case CssUnitOrNames.TOP:
                        {
                            top = (int)rectangle.Top;
                        }
                        break;
                    case CssUnitOrNames.BOTTOM:
                        {
                            top = (int)rectangle.Bottom - imgSize.Height;
                        }
                        break;
                }
            }
            else
            {   //not complete !
                top = (int)(rectangle.Top + (rectangle.Height - imgSize.Height) / 2 + .5f);
            }
            return new Point(left, top);
        }

        /// <summary>
        /// Draw the background image at the required location repeating it over the X axis.<br/>
        /// Adjust location to left if starting location doesn't include all the range (adjusted to center or right).
        /// </summary>
        static void DrawRepeatX(DrawBoard g, Image img, RectangleF rectangle, Rectangle srcRect, Rectangle destRect, Size imgSize)
        {
            while (destRect.X > rectangle.X)
                destRect.X -= imgSize.Width;
            //TODO: replement texture brysg again;

            //using (var brush = g.Platform.CreateTextureBrush(img, srcRect))
            //{
            //    brush.TranslateTransform(destRect.X, destRect.Y);
            //    g.FillRectangle(brush, rectangle.X, destRect.Y, rectangle.Width, srcRect.Height);
            //}
        }

        /// <summary>
        /// Draw the background image at the required location repeating it over the Y axis.<br/>
        /// Adjust location to top if starting location doesn't include all the range (adjusted to center or bottom).
        /// </summary>
        private static void DrawRepeatY(DrawBoard g, Image img, RectangleF rectangle, Rectangle srcRect, Rectangle destRect, Size imgSize)
        {
            while (destRect.Y > rectangle.Y)
                destRect.Y -= imgSize.Height;
            //TODO: reimplement this again
            //using (var brush = g.Platform.CreateTextureBrush(img, srcRect))
            //{
            //    brush.TranslateTransform(destRect.X, destRect.Y);
            //    g.FillRectangle(brush, destRect.X, rectangle.Y, srcRect.Width, rectangle.Height);
            //}
        }

        /// <summary>
        /// Draw the background image at the required location repeating it over the X and Y axis.<br/>
        /// Adjust location to left-top if starting location doesn't include all the range (adjusted to center or bottom/right).
        /// </summary>
        private static void DrawRepeat(DrawBoard g, Image img, RectangleF rectangle, Rectangle srcRect, Rectangle destRect, Size imgSize)
        {
            while (destRect.X > rectangle.X)
                destRect.X -= imgSize.Width;
            while (destRect.Y > rectangle.Y)
                destRect.Y -= imgSize.Height;
            //TODO: reimplement this again
            //using (var brush = g.Platform.CreateTextureBrush(img, srcRect))
            //{
            //    brush.TranslateTransform(destRect.X, destRect.Y);
            //    g.FillRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            //}
        }

        #endregion
    }
}