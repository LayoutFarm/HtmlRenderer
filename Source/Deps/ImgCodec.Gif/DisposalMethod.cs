//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================


namespace ImageTools.IO.Gif
{
    /// <summary>
    /// Specifies, what to do with the last image 
    /// in an animation sequence.
    /// </summary>
    public enum DisposalMethod
    {
        /// <summary>
        /// No disposal specified. The decoder is not 
        /// required to take any action. 
        /// </summary>
        Unspecified = 0,
        /// <summary>
        /// Do not dispose. The graphic is to be left in place. 
        /// </summary>
        NotDispose = 1,
        /// <summary>
        /// Restore to background color. 
        /// The area used by the graphic must be restored to
        /// the background color. 
        /// </summary>
        RestoreToBackground = 2,
        /// <summary>
        /// Restore to previous. The decoder is required to 
        /// restore the area overwritten by the 
        /// graphic with what was there prior to rendering the graphic. 
        /// </summary>
        RestoreToPrevious = 3
    }
}
