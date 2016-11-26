//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================


 
using System.IO;

namespace ImageTools.IO
{
 
    public interface IImageEncoder
    {
        /// <summary>
        /// Gets the default file extension for this encoder.
        /// </summary>
        /// <value>The default file extension for this encoder.</value>
        string Extension { get; }

        /// <summary>
        /// Indicates if the image encoder supports the specified
        /// file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>
        /// <c>true</c>, if the encoder supports the specified
        /// extensions; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="extension"/>
        /// is null (Nothing in Visual Basic).</exception>
        /// <exception cref="System.ArgumentException"><paramref name="extension"/> is a string
        /// of length zero or contains only blanks.</exception>
        bool IsSupportedFileExtension(string extension);

        /// <summary>
        /// Encodes the data of the specified image and writes the result to
        /// the specified stream.
        /// </summary>
        /// <param name="image">The image, where the data should be get from.
        /// Cannot be null (Nothing in Visual Basic).</param>
        /// <param name="stream">The stream, where the image data should be written to. 
        /// Cannot be null (Nothing in Visual Basic).</param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="image"/> is null (Nothing in Visual Basic).</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="stream"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        void Encode(ExtendedImage image, Stream stream);
    }
}
