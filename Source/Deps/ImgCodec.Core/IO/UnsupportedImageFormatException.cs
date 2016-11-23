//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================


namespace ImageTools.IO
{
    using System;

    /// <summary>
    /// The exception that is thrown when a image should be loaded which format is not supported.
    /// </summary>
    public class UnsupportedImageFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
        /// </summary>
        public UnsupportedImageFormatException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class with the name of the 
        /// parameter that causes this exception.
        /// </summary>
        /// <param name="errorMessage">The error message that explains the reason for this exception.</param>
        public UnsupportedImageFormatException(string errorMessage)
            : base(errorMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class with a specified 
        /// error message and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorMessage">The error message that explains the reason for this exception.</param>
        /// <param name="innerEx">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) 
        /// if no inner exception is specified.</param>
        public UnsupportedImageFormatException(string errorMessage, Exception innerEx)
            : base(errorMessage, innerEx) { }    }
}
