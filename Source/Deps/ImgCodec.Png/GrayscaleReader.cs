//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using ImageTools.Helpers;

namespace ImageTools.IO.Png
{
    /// <summary>
    /// Color reader for reading grayscale colors from a PNG file.
    /// </summary>
    sealed class GrayscaleReader : IColorReader
    {
        #region Fields

        private int _row;
        private bool _useAlpha;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GrayscaleReader"/> class.
        /// </summary>
        /// <param name="useAlpha">if set to <c>true</c> the color reader will also read the
        /// alpha channel from the scanline.</param>
        public GrayscaleReader(bool useAlpha)
        {
            _useAlpha = useAlpha;
        }

        #endregion

        #region IPngFormatHandler Members

        /// <summary>
        /// Reads the specified scanline.
        /// </summary>
        /// <param name="scanline">The scanline.</param>
        /// <param name="pixels">The pixels, where the colors should be stored in RGBA format.</param>
        /// <param name="header">The header, which contains information about the png file, like
        /// the width of the image and the height.</param>
        public void ReadScanline(byte[] scanline, byte[] pixels, PngHeader header)
        {
            int offset = 0;

            //byte[] newScanline = scanline.ToArrayByBitsLength(header.BitDepth);
            byte[] newScanline = IEnum.ToArrayByBitsLength(scanline, header.BitDepth);
            if (_useAlpha)
            {
                for (int x = 0; x < header.Width / 2; x++)
                {
                    offset = (_row * header.Width + x) * 4;

                    pixels[offset + 0] = newScanline[x * 2];
                    pixels[offset + 1] = newScanline[x * 2];
                    pixels[offset + 2] = newScanline[x * 2];
                    pixels[offset + 3] = newScanline[x * 2 + 1];
                }
            }
            else
            {
                for (int x = 0; x < header.Width; x++)
                {
                    offset = (_row * header.Width + x) * 4;

                    pixels[offset + 0] = newScanline[x];
                    pixels[offset + 1] = newScanline[x];
                    pixels[offset + 2] = newScanline[x];
                    pixels[offset + 3] = (byte)255;
                }
            }

            _row++;
        }

        #endregion
    }
}
