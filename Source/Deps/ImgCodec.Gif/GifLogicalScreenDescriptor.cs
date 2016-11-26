//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.IO.Gif
{
    /// <summary>
    /// The Logical Screen Descriptor contains the parameters 
    /// necessary to define the area of the display device 
    /// within which the images will be rendered
    /// </summary>
    sealed class GifLogicalScreenDescriptor
    {
        /// <summary>
        /// Width, in pixels, of the Logical Screen where the images will 
        /// be rendered in the displaying device.
        /// </summary>
        public short Width;
        /// <summary>
        /// Height, in pixels, of the Logical Screen where the images will be 
        /// rendered in the displaying device.
        /// </summary>
        public short Height;
        /// <summary>
        /// Index into the Global Color Table for the Background Color. 
        /// The Background Color is the color used for those 
        /// pixels on the screen that are not covered by an image.
        /// </summary>
        public byte Background;
        /// <summary>
        /// Flag indicating the presence of a Global Color Table; 
        /// if the flag is set, the Global Color Table will immediately 
        /// follow the Logical Screen Descriptor.
        /// </summary>
        public bool GlobalColorTableFlag;
        /// <summary>
        /// If the Global Color Table Flag is set to 1, 
        /// the value in this field is used to calculate the number of 
        /// bytes contained in the Global Color Table.
        /// </summary>
        public int GlobalColorTableSize;
    }
}
