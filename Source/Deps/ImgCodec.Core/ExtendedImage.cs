//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Diagnostics;

using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ImageTools.Helpers;
using ImageTools.IO;

namespace ImageTools
{
    /// <summary>
    /// Image class with stores the pixels and provides common functionality
    /// such as loading images from files and streams or operation like resizing or cutting.
    /// </summary>
    /// <remarks>The image data is alway stored in RGBA format, where the red, the blue, the
    /// alpha values are simple bytes.</remarks>
    [DebuggerDisplay("Image: {PixelWidth}x{PixelHeight}")]
    public sealed partial class ExtendedImage : SimpleImage
    {
        #region Constants

        /// <summary>
        /// The default density value (dots per inch) in x direction. The default value is 75 dots per inch.
        /// </summary>
        public const int DefaultDensityX = 75;
        /// <summary>
        /// The default density value (dots per inch) in y direction. The default value is 75 dots per inch.
        /// </summary>
        public const int DefaultDensityY = 75;

        #endregion

        #region Invariant

#if !WINDOWS_PHONE

        private void ImageInvariantMethod()
        {
            // Contract.Invariant(_frames != null);
            //Contract.Invariant(_properties != null);
        }
#endif

        #endregion

        #region Properties


        ///// <summary>
        ///// Gets or sets the resolution of the image in x direction. It is defined as 
        ///// number of dots per inch and should be an positive value.
        ///// </summary>
        ///// <value>The density of the image in x direction.</value>
        //public double DensityX { get;   }

        ///// <summary>
        ///// Gets or sets the resolution of the image in y direction. It is defined as 
        ///// number of dots per inch and should be an positive value.
        ///// </summary>
        ///// <value>The density of the image in y direction.</value>
        //public double DensityY { get;   }


        public int DensityXInt32 { get; set; }
        public int DensityYInt32 { get; set; }

        private ImageFrameCollection _frames = new ImageFrameCollection();
        /// <summary>
        /// Get the other frames for the animation.
        /// </summary>
        /// <value>The list of frame images.</value>
        public ImageFrameCollection Frames
        {
            get
            {
                //Contract.Ensures(Contract.Result<ImageFrameCollection>() != null);
                return _frames;
            }
        }

        private ImagePropertyCollection _properties = new ImagePropertyCollection();
        /// <summary>
        /// Gets the list of properties for storing meta information about this image.
        /// </summary>
        /// <value>A list of image properties.</value>
        public ImagePropertyCollection Properties
        {
            get
            {
                // Contract.Ensures(Contract.Result<ImagePropertyCollection>() != null);
                return _properties;
            }
        }


        #endregion



        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedImage"/> class
        /// with the height and the width of the image.
        /// </summary>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="height">The height of the image in pixels.</param>
        public ExtendedImage(int width, int height)
            : base(width, height)
        {
            //Contract.Requires<ArgumentException>(width >= 0, "Width must be greater or equals than zero.");
            //Contract.Requires<ArgumentException>(height >= 0, "Height must be greater or equals than zero.");
            //Contract.Ensures(IsFilled);

            DensityXInt32 = DefaultDensityX;
            DensityYInt32 = DefaultDensityY;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedImage"/> class
        /// by making a copy from another image.
        /// </summary>
        /// <param name="other">The other image, where the clone should be made from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null
        /// (Nothing in Visual Basic).</exception>
        public ExtendedImage(ExtendedImage other)
            : base(other)
        {
            //Contract.Requires<ArgumentNullException>(other != null, "Other image cannot be null.");
            // Contract.Requires<ArgumentException>(other.IsFilled, "Other image has not been loaded.");
            //Contract.Ensures(IsFilled);

            foreach (SimpleImage frame in other.Frames)
            {
                if (frame != null)
                {
                    if (!frame.IsFilled)
                    {
                        throw new ArgumentException("The image contains a frame that has not been loaded yet.");
                    }

                    Frames.Add(new SimpleImage(frame));
                }
            }

            DensityXInt32 = DefaultDensityX;
            DensityYInt32 = DefaultDensityY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedImage"/> class.
        /// </summary>
        public ExtendedImage()
        {
            DensityXInt32 = DefaultDensityX;
            DensityYInt32 = DefaultDensityY;
        }





        static int FindMax(List<IImageDecoder> imgCodecs)
        {
            int maxSize = 0;

            foreach (var imgCodec in imgCodecs)
            {
                if (imgCodec.HeaderSize > maxSize)
                {
                    maxSize = imgCodec.HeaderSize;
                }
            }
            return maxSize;
        }
        static IImageDecoder FindFirstSupport(List<IImageDecoder> imgCodecs, byte[] header)
        {
            foreach (var imgCodec in imgCodecs)
            {
                if (imgCodec.IsSupportedFileFormat(header))
                {
                    return imgCodec;
                }
            }
            return null;
        }
        public void Load(Stream stream, IImageDecoder decoder)
        {
            try
            {
                if (!stream.CanRead)
                {
                    throw new NotSupportedException("Cannot read from the stream.");
                }

                if (!stream.CanSeek)
                {
                    throw new NotSupportedException("The stream does not support seeking.");
                }

                int maxHeaderSize = decoder.HeaderSize;
                byte[] header = new byte[maxHeaderSize];

                stream.Read(header, 0, maxHeaderSize);
                stream.Position = 0;

                //var decoder = FindFirstSupport(decoders, header); //decoders.FirstOrDefault(x => x.IsSupportedFileFormat(header));
                if (decoder != null)
                {
                    decoder.Decode(this, stream);

                }



                //if (IsLoading)
                //{
                //    IsLoading = false;

                //    StringBuilder stringBuilder = new StringBuilder();
                //    stringBuilder.AppendLine("Image cannot be loaded. Available decoders:");

                //    foreach (IImageDecoder decoder in decoders)
                //    {
                //        stringBuilder.AppendLine("-" + decoder);
                //    }

                //    throw new UnsupportedImageFormatException(stringBuilder.ToString());
                //}
            }
            finally
            {
                stream.Dispose();
            }
        }
        private void Load(Stream stream)
        {
            //Contract.Requires(stream != null);

            try
            {
                if (!stream.CanRead)
                {
                    throw new NotSupportedException("Cannot read from the stream.");
                }

                if (!stream.CanSeek)
                {
                    throw new NotSupportedException("The stream does not support seeking.");
                }

                List<IImageDecoder> decoders = Decoders.GetAvailableDecoders();

                if (decoders.Count > 0)
                {
                    int maxHeaderSize = FindMax(decoders);
                    if (maxHeaderSize > 0)
                    {
                        byte[] header = new byte[maxHeaderSize];

                        stream.Read(header, 0, maxHeaderSize);
                        stream.Position = 0;

                        var decoder = FindFirstSupport(decoders, header); //decoders.FirstOrDefault(x => x.IsSupportedFileFormat(header));
                        if (decoder != null)
                        {
                            decoder.Decode(this, stream);
                        }
                    }
                }
            }
            finally
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public ExtendedImage Clone()
        {
            //Contract.Requires(IsFilled);
            //Contract.Ensures(Contract.Result<ExtendedImage>() != null);

            return new ExtendedImage(this);
        }


    }
}
