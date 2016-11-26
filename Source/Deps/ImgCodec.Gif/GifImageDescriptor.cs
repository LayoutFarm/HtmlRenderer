//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.IO.Gif
{
    /// <summary>
    /// Each image in the Data Stream is composed of an Image Descriptor, 
    /// an optional Local Color Table, and the image data. 
    /// Each image must fit within the boundaries of the 
    /// Logical Screen, as defined in the Logical Screen Descriptor. 
    /// </summary>
    sealed class GifImageDescriptor
    {
        /// <summary>
        /// Column number, in pixels, of the left edge of the image, 
        /// with respect to the left edge of the Logical Screen. 
        /// Leftmost column of the Logical Screen is 0.
        /// </summary>
        public short Left;
        /// <summary>
        /// Row number, in pixels, of the top edge of the image with 
        /// respect to the top edge of the Logical Screen. 
        /// Top row of the Logical Screen is 0.
        /// </summary>
        public short Top;
        /// <summary>
        /// Width of the image in pixels.
        /// </summary>
        public short Width;
        /// <summary>
        /// Height of the image in pixels.
        /// </summary>
        public short Height;
        /// <summary>
        /// Indicates the presence of a Local Color Table immediately 
        /// following this Image Descriptor.
        /// </summary>
        public bool LocalColorTableFlag;
        /// <summary>
        /// If the Local Color Table Flag is set to 1, the value in this field 
        /// is used to calculate the number of bytes contained in the Local Color Table.
        /// </summary>
        public int LocalColorTableSize;
        /// <summary>
        /// Indicates if the image is interlaced. An image is interlaced 
        /// in a four-pass interlace pattern.
        /// </summary>
        public bool InterlaceFlag;
    }
}
