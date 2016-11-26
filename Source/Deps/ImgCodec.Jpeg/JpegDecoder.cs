//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================


using System.Globalization;
using System.IO;
using BitMiracle.LibJpeg;
using FluxJpeg.Core;
using ImageTools.Helpers;

namespace ImageTools.IO.Jpeg
{
    using FluxCoreJpegDecoder = FluxJpeg.Core.Decoder.JpegDecoder;

    /// <summary>
    /// Image decoder for generating an image out of an jpg stream.
    /// </summary>
    public class JpegDecoder : IImageDecoder
    {
        #region IImageDecoder Members

        /// <summary>
        /// Gets or sets a value indicating whether FJCore should be used to decode the images.
        /// </summary>
        /// <value>A value indicating whether FJCore should be used to decode the images.</value>
        public bool UseLegacyLibrary { get; set; }

        /// <summary>
        /// Gets the size of the header for this image type.
        /// </summary>
        /// <value>The size of the header.</value>
        public int HeaderSize
        {
            get { return 11; }
        }

        /// <summary>
        /// Indicates if the image decoder supports the specified
        /// file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>
        /// <c>true</c>, if the decoder supports the specified
        /// extensions; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="extension"/>
        /// is null (Nothing in Visual Basic).</exception>
        /// <exception cref="System.ArgumentException"><paramref name="extension"/> is a string
        /// of length zero or contains only blanks.</exception>
        public bool IsSupportedFileExtension(string extension)
        {
            Guard.NotNullOrEmpty(extension, "extension");

            string extensionAsUpper = extension.ToUpper(CultureInfo.CurrentCulture);
            return extensionAsUpper == "JPG" ||
                   extensionAsUpper == "JPEG" ||
                   extensionAsUpper == "JFIF";
        }

        /// <summary>
        /// Indicates if the image decoder supports the specified
        /// file header.
        /// </summary>
        /// <param name="header">The file header.</param>
        /// <returns>
        /// <c>true</c>, if the decoder supports the specified
        /// file header; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="header"/>
        /// is null (Nothing in Visual Basic).</exception>
        public bool IsSupportedFileFormat(byte[] header)
        {
            Guard.NotNull(header, "header");

            bool isSupported = false;

            if (header.Length >= 11)
            {
                bool isJpeg = IsJpeg(header);
                bool isExif = IsExif(header);

                isSupported = isJpeg || isExif;
            }

            return isSupported;
        }

        private bool IsExif(byte[] header)
        {
            bool isExif =
                header[6] == 0x45 && // E
                header[7] == 0x78 && // x
                header[8] == 0x69 && // i
                header[9] == 0x66 && // f
                header[10] == 0x00;

            return isExif;
        }

        private static bool IsJpeg(byte[] header)
        {
            bool isJpg =
                header[6] == 0x4A && // J
                header[7] == 0x46 && // F
                header[8] == 0x49 && // I
                header[9] == 0x46 && // F
                header[10] == 0x00;

            return isJpg;
        }

        /// <summary>
        /// Decodes the image from the specified stream and sets
        /// the data to image.
        /// </summary>
        /// <param name="image">The image, where the data should be set to.
        /// Cannot be null (Nothing in Visual Basic).</param>
        /// <param name="stream">The stream, where the image should be
        /// decoded from. Cannot be null (Nothing in Visual Basic).</param>
        /// <exception cref="System.ArgumentNullException">
        /// 	<para><paramref name="image"/> is null (Nothing in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="stream"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        public void Decode(ExtendedImage image, Stream stream)
        {
            Guard.NotNull(image, "image");
            Guard.NotNull(stream, "stream");

            if (UseLegacyLibrary)
            {
                FluxCoreJpegDecoder fluxCoreJpegDecoder = new FluxCoreJpegDecoder(stream);

                DecodedJpeg jpg = fluxCoreJpegDecoder.Decode();

                jpg.Image.ChangeColorSpace(ColorSpace.RGB);

                int pixelWidth = jpg.Image.Width;
                int pixelHeight = jpg.Image.Height;

                byte[] pixels = new byte[pixelWidth * pixelHeight * 4];

                byte[][,] sourcePixels = jpg.Image.Raster;

                for (int y = 0; y < pixelHeight; y++)
                {
                    for (int x = 0; x < pixelWidth; x++)
                    {
                        int offset = (y * pixelWidth + x) * 4;

                        pixels[offset + 0] = sourcePixels[0][x, y];
                        pixels[offset + 1] = sourcePixels[1][x, y];
                        pixels[offset + 2] = sourcePixels[2][x, y];
                        pixels[offset + 3] = (byte)255;

                    }
                }

                //-------

                //
                image.DensityXInt32 = jpg.Image.DensityX;
                image.DensityYInt32 = jpg.Image.DensityY;

                image.SetPixels(pixelWidth, pixelHeight, pixels);
            }
            else
            {
                JpegImage jpg = new JpegImage(stream);

                int pixelWidth = jpg.Width;
                int pixelHeight = jpg.Height;

                byte[] pixels = new byte[pixelWidth * pixelHeight * 4];

                if (!(jpg.Colorspace == Colorspace.RGB && jpg.BitsPerComponent == 8))
                {
                    throw new UnsupportedImageFormatException();
                }

                for (int y = 0; y < pixelHeight; y++)
                {
                    SampleRow row = jpg.GetRow(y);
                    for (int x = 0; x < pixelWidth; x++)
                    {
                        //Sample sample = row.GetAt(x);
                        int offset = (y * pixelWidth + x) * 4;
                        row.GetComponentsAt(x, out pixels[offset + 0], out pixels[offset + 1], out pixels[offset + 2]);
                        //r = (byte)sample[0];
                        //g = (byte)sample[1];
                        //b = (byte)sample[2];  
                        //pixels[offset + 0] = r;
                        //pixels[offset + 1] = g;
                        //pixels[offset + 2] = b;
                        pixels[offset + 3] = (byte)255;
                    }
                }

                image.SetPixels(pixelWidth, pixelHeight, pixels);
            }
        }

        #endregion
    }
}
