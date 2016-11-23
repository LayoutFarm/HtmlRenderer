//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.IO.Png
{
    /// <summary>
    /// Contains a list of possible chunk type identifier.
    /// </summary>
    static class PngChunkTypes
    {
        /// <summary>
        /// The first chunk in a png file. Can only exists once. Contains 
        /// common information like the width and the height of the image or
        /// the used compression method.
        /// </summary>
        public const string Header = "IHDR";
        /// <summary>
        /// The PLTE chunk contains from 1 to 256 palette entries, each a three byte
        /// series in the RGB format.
        /// </summary>
        public const string Palette = "PLTE";
        /// <summary>
        /// The IDAT chunk contains the actual image data. The image can contains more
        /// than one chunk of this type. All chunks together are the whole image.
        /// </summary>
        public const string Data = "IDAT";
        /// <summary>
        /// This chunk must appear last. It marks the end of the PNG datastream. 
        /// The chunk's data field is empty. 
        /// </summary>
        public const string End = "IEND";
        /// <summary>
        /// This chunk specifies that the image uses simple transparency: 
        /// either alpha values associated with palette entries (for indexed-color images) 
        /// or a single transparent color (for grayscale and truecolor images). 
        /// </summary>
        public const string PaletteAlpha = "tRNS";
        /// <summary>
        /// Textual information that the encoder wishes to record with the image can be stored in 
        /// tEXt chunks. Each tEXt chunk contains a keyword and a text string.
        /// </summary>
        public const string Text = "tEXt";
        /// <summary>
        /// This chunk specifies the relationship between the image samples and the desired 
        /// display output intensity.
        /// </summary>
        public const string Gamma = "gAMA";
        /// <summary>
        /// The pHYs chunk specifies the intended pixel size or aspect ratio for display of the image. 
        /// </summary>
        public const string Physical = "pHYs";
    }

    /// <summary>
    /// Stores header information about a chunk.
    /// </summary>
    sealed class PngChunk
    {
        /// <summary>
        /// An unsigned integer giving the number of bytes in the chunk's 
        /// data field. The length counts only the data field, not itself, 
        /// the chunk type code, or the CRC. Zero is a valid length
        /// </summary>
        public int Length;
        /// <summary>
        /// A chunk type as string with 4 chars.
        /// </summary>
        public string Type;
        /// <summary>
        /// The data bytes appropriate to the chunk type, if any. 
        /// This field can be of zero length. 
        /// </summary>
        public byte[] Data;
        /// <summary>
        /// A CRC (Cyclic Redundancy Check) calculated on the preceding bytes in the chunk, 
        /// including the chunk type code and chunk data fields, but not including the length field. 
        /// The CRC is always present, even for chunks containing no data
        /// </summary>
        public uint Crc;
    }
}
