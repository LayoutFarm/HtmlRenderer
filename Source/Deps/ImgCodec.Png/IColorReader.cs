//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================


namespace ImageTools.IO.Png
{
    /// <summary>
    /// Interface for color readers, which are responsible for reading 
    /// different color formats from a png file.
    /// </summary>
    interface IColorReader
    {
        /// <summary>
        /// Reads the specified scanline.
        /// </summary>
        /// <param name="scanline">The scanline.</param>
        /// <param name="pixels">The pixels, where the colors should be stored in RGBA format.</param>
        /// <param name="header">The header, which contains information about the png file, like
        /// the width of the image and the height.</param>
        void ReadScanline(byte[] scanline, byte[] pixels, PngHeader header);
    }
}
