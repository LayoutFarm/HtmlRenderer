//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools
{
    /// <summary>
    /// Defines how a image should be flipped.
    /// </summary>
    public enum FlippingType
    {
        /// <summary>
        /// Dont flip the image.
        /// </summary>
        None,
        /// <summary>
        /// Flip the image at the x-axis which goes 
        /// through the middle of the image.
        /// </summary>
        FlipX,
        /// <summary>
        /// Flip the image at the y-axis, which goes through the 
        /// middle of the image.
        /// </summary>
        FlipY
    }
}
