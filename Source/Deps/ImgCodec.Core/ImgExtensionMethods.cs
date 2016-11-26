//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
namespace ImageTools
{
    //TODO: review usage and performance
    public static class ImgExtensionMethods
    {
        /// <summary>
        /// Gets the ratio between the width and the height of this <see cref="SimpleImage"/> instance.
        /// </summary>
        /// <value>The ratio between the width and the height.</value>
        public static double GetPixelRatio(this SimpleImage img)
        {
            //Contract.Ensures(!IsFilled || Contract.Result<double>() > 0); 
            if (img.IsFilled)
            {
                return (double)img.PixelWidth / img.PixelHeight;
            }
            else
            {
                return 0;
            }
        }
        public static double GetInchWidth(this ExtendedImage img)
        {

            double densityX = img.DensityXInt32 / 39.3700787d;

            if (densityX <= 0)
            {
                densityX = ExtendedImage.DefaultDensityX;
            }

            return img.PixelWidth / densityX;
        }
        public static double GetInchHeight(this ExtendedImage img)
        {
            double densityY = img.DensityYInt32 / 39.3700787d;

            if (densityY <= 0)
            {
                densityY = ExtendedImage.DefaultDensityY;
            }
            return img.PixelHeight / densityY;
        }
        /// <summary>
        /// Transforms the specified image by flipping and rotating it. First the image
        /// will be rotated, then the image will be flipped. A new image will be returned. The original image
        /// will not be changed.
        /// </summary>
        /// <param name="source">The image, which should be transformed.</param>
        /// <param name="target">The new transformed image.</param>
        /// <param name="rotationType">Type of the rotation.</param>
        /// <param name="flippingType">Type of the flipping.</param>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="source"/> is null (Nothing in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="target"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        internal static void Transform(SimpleImage source, SimpleImage target, RotationType rotationType, FlippingType flippingType)
        {
            //Contract.Requires<ArgumentNullException>(source != null, "Source image cannot be null.");
            //Contract.Requires<ArgumentException>(source.IsFilled, "Source image has not been loaded.");
            //Contract.Requires<ArgumentNullException>(target != null, "Target image cannot be null.");

            switch (rotationType)
            {
                case RotationType.None:
                    {
                        byte[] targetPixels = source.Pixels;
                        byte[] sourcePixels = new byte[targetPixels.Length];

                        Array.Copy(targetPixels, sourcePixels, targetPixels.Length);

                        target.SetPixels(source.PixelWidth, source.PixelHeight, sourcePixels);
                    }
                    break;
                case RotationType.Rotate90:
                    {
                        Rotate90(source, target);
                    }
                    break;
                case RotationType.Rotate180:
                    {
                        Rotate180(source, target);
                    }
                    break;
                case RotationType.Rotate270:
                    {
                        Rotate270(source, target);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }

            switch (flippingType)
            {
                case FlippingType.FlipX:
                    FlipX(target);
                    break;
                case FlippingType.FlipY:
                    FlipY(target);
                    break;
            }
        }

        static void Rotate270(SimpleImage source, SimpleImage target)
        {
            //Contract.Requires(source != null);
            //Contract.Requires(source.IsFilled);
            //Contract.Requires(target != null);
            //Contract.Ensures(target.IsFilled);

            int oldIndex = 0, newIndex = 0;

            byte[] sourcePixels = source.Pixels;
            byte[] targetPixels = new byte[source.PixelWidth * source.PixelHeight * 4];

            for (int y = 0; y < source.PixelHeight; y++)
            {
                for (int x = 0; x < source.PixelWidth; x++)
                {
                    oldIndex = (y * source.PixelWidth + x) * 4;

                    // The new index will be calculated as if the image would be flipped
                    // at the x and the y axis and rotated about 90 degrees.
                    newIndex = ((source.PixelWidth - x - 1) * source.PixelHeight + y) * 4;

                    targetPixels[newIndex + 0] = sourcePixels[oldIndex + 0];
                    targetPixels[newIndex + 1] = sourcePixels[oldIndex + 1];
                    targetPixels[newIndex + 2] = sourcePixels[oldIndex + 2];
                    targetPixels[newIndex + 3] = sourcePixels[oldIndex + 3];
                }
            }
            target.SetPixels(source.PixelHeight, source.PixelWidth, targetPixels);
        }

        static void Rotate180(SimpleImage source, SimpleImage target)
        {
            //Contract.Requires(source != null);
            //Contract.Requires(source.IsFilled);
            //Contract.Requires(target != null);
            //Contract.Ensures(target.IsFilled);

            int oldIndex = 0, newIndex = 0;

            byte[] sourcePixels = source.Pixels;
            byte[] targetPixels = new byte[source.PixelWidth * source.PixelHeight * 4];

            for (int y = 0; y < source.PixelHeight; y++)
            {
                for (int x = 0; x < source.PixelWidth; x++)
                {
                    oldIndex = (y * source.PixelHeight + x) * 4;

                    // The new index will be calculated as if the image would be flipped
                    // at the x and the y axis.
                    newIndex = ((source.PixelHeight - y - 1) * source.PixelWidth + source.PixelWidth - x - 1) * 4;

                    targetPixels[newIndex + 0] = sourcePixels[oldIndex + 0];
                    targetPixels[newIndex + 1] = sourcePixels[oldIndex + 1];
                    targetPixels[newIndex + 2] = sourcePixels[oldIndex + 2];
                    targetPixels[newIndex + 3] = sourcePixels[oldIndex + 3];
                }
            }
            target.SetPixels(source.PixelWidth, source.PixelHeight, targetPixels);
        }

        static void Rotate90(SimpleImage source, SimpleImage target)
        {
            //Contract.Requires(source != null);
            //Contract.Requires(source.IsFilled);
            //Contract.Requires(target != null);
            //Contract.Ensures(target.IsFilled);

            int oldIndex = 0, newIndex = 0;

            byte[] sourcePixels = source.Pixels;
            byte[] targetPixels = new byte[source.PixelWidth * source.PixelHeight * 4];

            for (int y = 0; y < source.PixelHeight; y++)
            {
                for (int x = 0; x < source.PixelWidth; x++)
                {
                    oldIndex = (y * source.PixelWidth + x) * 4;

                    // The new index will just be calculated by swapping
                    // the x and the y value for the pixel.
                    newIndex = ((x + 1) * source.PixelHeight - y - 1) * 4;

                    targetPixels[newIndex + 0] = sourcePixels[oldIndex + 0];
                    targetPixels[newIndex + 1] = sourcePixels[oldIndex + 1];
                    targetPixels[newIndex + 2] = sourcePixels[oldIndex + 2];
                    targetPixels[newIndex + 3] = sourcePixels[oldIndex + 3];
                }
            }
            target.SetPixels(source.PixelHeight, source.PixelWidth, targetPixels);
        }

        /// <summary>
        /// Swaps the image at the X-axis, which goes throug the middle
        ///  of the height of the image.
        /// </summary>
        /// <param name="image">The image to swap at the x-axis. Cannot be null
        /// (Nothing in Visual Basic).</param>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> is null
        /// (Nothing in Visual Basic).</exception>
        static void FlipX(SimpleImage image)
        {
            //Contract.Requires<ArgumentNullException>(image != null, "Image cannot be null.");
            //Contract.Requires<ArgumentException>(image.IsFilled, "Other image has not been loaded.");

            int oldIndex = 0, newIndex = 0;

            byte[] sourcePixels = image.Pixels;

            byte r, g, b, a;

            for (int y = 0; y < image.PixelHeight / 2; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    oldIndex = (y * image.PixelWidth + x) * 4;

                    r = sourcePixels[oldIndex + 0];
                    g = sourcePixels[oldIndex + 1];
                    b = sourcePixels[oldIndex + 2];
                    a = sourcePixels[oldIndex + 3];

                    newIndex = ((image.PixelHeight - y - 1) * image.PixelWidth + x) * 4;

                    sourcePixels[oldIndex + 0] = sourcePixels[newIndex + 0];
                    sourcePixels[oldIndex + 1] = sourcePixels[newIndex + 1];
                    sourcePixels[oldIndex + 2] = sourcePixels[newIndex + 2];
                    sourcePixels[oldIndex + 3] = sourcePixels[newIndex + 3];

                    sourcePixels[newIndex + 0] = r;
                    sourcePixels[newIndex + 1] = g;
                    sourcePixels[newIndex + 2] = b;
                    sourcePixels[newIndex + 3] = a;
                }
            }
        }

        /// <summary>
        /// Swaps the image at the Y-axis, which goes throug the middle
        ///  of the width of the image.
        /// </summary>
        /// <param name="image">The image to swap at the y-axis. Cannot be null
        /// (Nothing in Visual Basic).</param>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> is null
        /// (Nothing in Visual Basic).</exception>
        static void FlipY(SimpleImage image)
        {
            //Contract.Requires<ArgumentNullException>(image != null, "Image cannot be null.");
            //Contract.Requires<ArgumentException>(image.IsFilled, "Other image has not been loaded.");

            int oldIndex = 0, newIndex = 0;

            byte[] sourcePixels = image.Pixels;

            byte r, g, b, a;

            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth / 2; x++)
                {
                    oldIndex = (y * image.PixelWidth + x) * 4;

                    r = sourcePixels[oldIndex + 0];
                    g = sourcePixels[oldIndex + 1];
                    b = sourcePixels[oldIndex + 2];
                    a = sourcePixels[oldIndex + 3];

                    newIndex = (y * image.PixelWidth + image.PixelWidth - x - 1) * 4;

                    sourcePixels[oldIndex + 0] = sourcePixels[newIndex + 0];
                    sourcePixels[oldIndex + 1] = sourcePixels[newIndex + 1];
                    sourcePixels[oldIndex + 2] = sourcePixels[newIndex + 2];
                    sourcePixels[oldIndex + 3] = sourcePixels[newIndex + 3];

                    sourcePixels[newIndex + 0] = r;
                    sourcePixels[newIndex + 1] = g;
                    sourcePixels[newIndex + 2] = b;
                    sourcePixels[newIndex + 3] = a;
                }
            }
        }

        /// <summary>
        /// Cuts the image with the specified rectangle and returns a new image.
        /// </summary>
        /// <param name="source">The image, where a cutted copy should be made from.</param>
        /// <param name="target">The new image.</param>
        /// <param name="bounds">The bounds of the new image.</param>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="source"/> is null (Nothing in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="target"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        //[ContractVerification(false)]
        //internal static void Crop(ImageBase source, ImageBase target, Rectangle bounds)
        //{
        //    //Contract.Requires<ArgumentNullException>(source != null, "Source image cannot be null.");
        //    //Contract.Requires<ArgumentException>(source.IsFilled, "Source image has not been loaded.");
        //    //Contract.Requires<ArgumentNullException>(target != null, "Target image cannot be null.");

        //    Guard.GreaterThan(bounds.Width, 0, "bounds",
        //        "Width of the rectangle must be greater than zero.");

        //    Guard.GreaterThan(bounds.Height, 0, "bounds",
        //        "Height of the rectangle must be greater than zero.");

        //    if (bounds.Right > source.PixelWidth || bounds.Bottom > source.PixelHeight)
        //    {
        //        throw new ArgumentException(
        //            "Rectangle must be in the range of the image's dimension.", "bounds");
        //    }

        //    byte[] sourcePixels = source.Pixels;
        //    byte[] targetPixels = new byte[bounds.Width * bounds.Height * 4];

        //    for (int y = bounds.Top, i = 0; y < bounds.Bottom; y++, i++)
        //    {
        //        Array.Copy(sourcePixels, (y * source.PixelWidth + bounds.Left) * 4, targetPixels, i * bounds.Width * 4, bounds.Width * 4);
        //    }

        //    target.SetPixels(bounds.Width, bounds.Height, targetPixels);
        //}
    }
}