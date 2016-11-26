//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools
{
    using System;

    /// <summary>
    /// The exception that is thrown when the library tries to load
    /// an iamge, which has an invalid format.
    /// </summary>
    public class ImageFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFormatException"/> class.
        /// </summary>
        public ImageFormatException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFormatException"/> class with the name of the 
        /// parameter that causes this exception.
        /// </summary>
        /// <param name="errorMessage">The error message that explains the reason for this exception.</param>
        public ImageFormatException(string errorMessage)
            : base(errorMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFormatException"/> class with a specified 
        /// error message and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorMessage">The error message that explains the reason for this exception.</param>
        /// <param name="innerEx">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) 
        /// if no inner exception is specified.</param>
        public ImageFormatException(string errorMessage, Exception innerEx)
            : base(errorMessage, innerEx) { }
    }
}
