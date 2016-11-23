//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.IO.Gif
{
    /// <summary>
    /// The Graphic Control Extension contains parameters used when 
    /// processing a graphic rendering block.
    /// </summary>
    sealed class GifGraphicsControlExtension
    {        
        /// <summary>
        /// Indicates the way in which the graphic is to be treated after being displayed. 
        /// </summary>
        public DisposalMethod DisposalMethod;
        /// <summary>
        /// Indicates whether a transparency index is given in the Transparent Index field. 
        /// (This field is the least significant bit of the byte.) 
        /// </summary>
        public bool TransparencyFlag;
        /// <summary>
        /// The Transparency Index is such that when encountered, the corresponding pixel 
        /// of the display device is not modified and processing goes on to the next pixel.
        /// </summary>
        public int TransparencyIndex;
        /// <summary>
        /// If not 0, this field specifies the number of hundredths (1/100) of a second to 
        /// wait before continuing with the processing of the Data Stream. 
        /// The clock starts ticking immediately after the graphic is rendered. 
        /// This field may be used in conjunction with the User Input Flag field. 
        /// </summary>
        public int DelayTime;
    }
}
