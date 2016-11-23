//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================


using ImageTools.Helpers;

namespace ImageTools.IO.Png
{
    /// <summary>
    /// A color reader for reading palette indices from the PNG file.
    /// </summary>
    sealed class PaletteIndexReader : IColorReader
    {
        #region Fields

        private int _row;
        private byte[] _palette;
        private byte[] _paletteAlpha;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteIndexReader"/> class.
        /// </summary>
        /// <param name="palette">The palette as simple byte array. It will contains 3 values for each
        /// color, which represents the red-, the green- and the blue channel.</param>
        /// <param name="paletteAlpha">The alpha palette. Can be null, if the image does not have an
        /// alpha channel and can contain less entries than the number of colors in the palette.</param>
        public PaletteIndexReader(byte[] palette, byte[] paletteAlpha)
        {
            _palette = palette;

            _paletteAlpha = paletteAlpha;
        }

        #endregion

        #region IColorReader Members

        /// <summary>
        /// Reads the specified scanline.
        /// </summary>
        /// <param name="scanline">The scanline.</param>
        /// <param name="pixels">The pixels, where the colors should be stored in RGBA format.</param>
        /// <param name="header">The header, which contains information about the png file, like
        /// the width of the image and the height.</param>
        public void ReadScanline(byte[] scanline, byte[] pixels, PngHeader header)
        {
            //byte[] newScanline = scanline.ToArrayByBitsLength(header.BitDepth);
            byte[] newScanline = IEnum.ToArrayByBitsLength(scanline, header.BitDepth);
            int offset = 0, index = 0;

            if (_paletteAlpha != null && _paletteAlpha.Length > 0)
            {
                // If the alpha palette is not null and does one or
                // more entries, this means, that the image contains and alpha
                // channel and we should try to read it.
                for (int i = 0; i < header.Width; i++)
                {
                    index = newScanline[i];

                    offset = (_row * header.Width + i) * 4;

                    pixels[offset + 0] = _palette[index * 3];
                    pixels[offset + 1] = _palette[index * 3 + 1];
                    pixels[offset + 2] = _palette[index * 3 + 2];
                    pixels[offset + 3] = _paletteAlpha.Length > index ? _paletteAlpha[index] : (byte)255;
                }
            }
            else
            {
                for (int i = 0; i < header.Width; i++)
                {
                    index = newScanline[i];

                    offset = (_row * header.Width + i) * 4;

                    pixels[offset + 0] = _palette[index * 3];
                    pixels[offset + 1] = _palette[index * 3 + 1];
                    pixels[offset + 2] = _palette[index * 3 + 2];
                    pixels[offset + 3] = (byte)255;
                }
            }

            _row++;
        }

        #endregion
    }
}
