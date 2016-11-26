//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================


namespace ImageTools
{
    /// <summary>
    /// Defines how the image should be rotated.
    /// </summary>
    public enum RotationType
    {
        /// <summary>
        /// Do not rotate the image.
        /// </summary>
        None,
        /// <summary>
        /// Rotate the image by 90 degrees clockwise.
        /// </summary>
        Rotate90,
        /// <summary>
        /// Rotate the image by 180 degrees clockwise.
        /// </summary>
        Rotate180,
        /// <summary>
        /// Rotate the image by 270 degrees clockwise.
        /// </summary>
        Rotate270
    }
}
