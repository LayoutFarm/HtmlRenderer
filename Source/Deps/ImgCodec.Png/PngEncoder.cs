//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Globalization;
using System.IO; 
using ImageTools.Helpers;

namespace ImageTools.IO.Png
{
    /// <summary>
    /// Image encoder for writing image data to a stream in png format.
    /// </summary>
    public class PngEncoder : IImageEncoder
    {
        #region Constants

        private const int MaxBlockSize = 0xFFFF;

        #endregion

        #region Fields

        private Stream _stream;
        private ExtendedImage _image;

        #endregion

        #region IImageEncoder Members

        /// <summary>
        /// Gets or sets a value indicating whether this encoder
        /// will write the image uncompressed the stream.
        /// </summary>
        /// <value>
        /// <c>true</c> if the image should be written uncompressed to
        /// the stream; otherwise, <c>false</c>.
        /// </value>
        public bool IsWritingUncompressed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is writing
        /// gamma information to the stream. The default value is false.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is writing gamma 
        /// information to the stream.; otherwise, <c>false</c>.
        /// </value>
        public bool IsWritingGamma { get; set; }

        /// <summary>
        /// Gets or sets the gamma value, that will be written
        /// the the stream, when the <see cref="IsWritingGamma"/> property
        /// is set to true. The default value is 2.2f.
        /// </summary>
        /// <value>The gamma value of the image.</value>
        public double Gamma { get; set; }

        /// <summary>
        /// Gets the default file extension for this encoder.
        /// </summary>
        /// <value>The default file extension for this encoder.</value>
        public string Extension
        {
            get { return "PNG"; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PngEncoder"/> class.
        /// </summary>
        public PngEncoder()
        {
            Gamma = 2.2f;
        }

        /// <summary>
        /// Indicates if the image encoder supports the specified
        /// file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns><c>true</c>, if the encoder supports the specified
        /// extensions; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/>
        /// is null (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="extension"/> is a string
        /// of length zero or contains only blanks.</exception>
        public bool IsSupportedFileExtension(string extension)
        {
            Guard.NotNullOrEmpty(extension, "extension");

            string extensionAsUpper = extension.ToUpper(CultureInfo.CurrentCulture);
            return extensionAsUpper == "PNG";
        }

        /// <summary>
        /// Encodes the data of the specified image and writes the result to
        /// the specified stream.
        /// </summary>
        /// <param name="image">The image, where the data should be get from.
        /// Cannot be null (Nothing in Visual Basic).</param>
        /// <param name="stream">The stream, where the image data should be written to.
        /// Cannot be null (Nothing in Visual Basic).</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="image"/> is null (Nothing in Visual Basic).</para>
        /// <para>- or -</para>
        /// <para><paramref name="stream"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        public void Encode(ExtendedImage image, Stream stream)
        {
            Guard.NotNull(image, "image");
            Guard.NotNull(stream, "stream");

            _image = image;

            _stream = stream;

            // Write the png header.
            stream.Write(
                new byte[] 
                { 
                    0x89, 0x50, 0x4E, 0x47, 
                    0x0D, 0x0A, 0x1A, 0x0A 
                }, 0, 8);

            PngHeader header = new PngHeader();
            header.Width = image.PixelWidth;
            header.Height = image.PixelHeight;
            header.ColorType = 6;
            header.BitDepth = 8;
            header.FilterMethod = 0;
            header.CompressionMethod = 0;
            header.InterlaceMethod = 0;

            WriteHeaderChunk(header);

            WritePhysicsChunk();
            WriteGammaChunk();

            if (IsWritingUncompressed)
            {
                WriteDataChunksFast();
            }
            else
            {
                WriteDataChunks();
            }
            WriteEndChunk();

            stream.Flush();
        }

        private void WritePhysicsChunk()
        {
            if (_image.DensityXInt32 > 0 && _image.DensityYInt32 > 0)
            {
                int dpmX = _image.DensityXInt32;
                int dpmY = _image.DensityYInt32;

                byte[] chunkData = new byte[9];

                WriteInteger(chunkData, 0, dpmX);
                WriteInteger(chunkData, 4, dpmY);

                chunkData[8] = 1;

                WriteChunk(PngChunkTypes.Physical, chunkData);
            }
        }

        private void WriteGammaChunk()
        {
            if (IsWritingGamma)
            {
                int gammeValue = (int)(Gamma * 100000f);

                byte[] fourByteData = new byte[4];

                byte[] size = BitConverter.GetBytes(gammeValue);
                fourByteData[0] = size[3]; fourByteData[1] = size[2]; fourByteData[2] = size[1]; fourByteData[3] = size[0];

                WriteChunk(PngChunkTypes.Gamma, fourByteData);
            }
        }

        private void WriteDataChunksFast()
        {
            byte[] pixels = _image.Pixels;

            // Convert the pixel array to a new array for adding
            // the filter byte.
            // --------------------------------------------------
            byte[] data = new byte[_image.PixelWidth * _image.PixelHeight * 4 + _image.PixelHeight];

            int rowLength = _image.PixelWidth * 4 + 1;

            for (int y = 0; y < _image.PixelHeight; y++)
            {
                data[y * rowLength] = 0;

                Array.Copy(pixels, y * _image.PixelWidth * 4, data, y * rowLength + 1, _image.PixelWidth * 4);
            }
            // --------------------------------------------------

            //Adler32 adler32 = new Adler32();
            //adler32.Update(data);
            uint adler32Value = Ionic.Zlib.Adler.Adler32(0, data, 0, data.Length);
            using (MemoryStream tempStream = new MemoryStream())
            {
                int remainder = data.Length;

                int blockCount;
                if ((data.Length % MaxBlockSize) == 0)
                {
                    blockCount = data.Length / MaxBlockSize;
                }
                else
                {
                    blockCount = (data.Length / MaxBlockSize) + 1;
                }

                // Write headers
                tempStream.WriteByte(0x78);
                tempStream.WriteByte(0xDA);

                for (int i = 0; i < blockCount; i++)
                {
                    // Write the length
                    ushort length = (ushort)((remainder < MaxBlockSize) ? remainder : MaxBlockSize);

                    if (length == remainder)
                    {
                        tempStream.WriteByte(0x01);
                    }
                    else
                    {
                        tempStream.WriteByte(0x00);
                    }

                    tempStream.Write(BitConverter.GetBytes(length), 0, 2);

                    // Write one's compliment of length
                    tempStream.Write(BitConverter.GetBytes((ushort)~length), 0, 2);

                    // Write blocks
                    tempStream.Write(data, (int)(i * MaxBlockSize), length);

                    // Next block
                    remainder -= MaxBlockSize;
                }

                //WriteInteger(tempStream, (int)adler32.Value);
                WriteInteger(tempStream, (int)adler32Value);
                tempStream.Seek(0, SeekOrigin.Begin);

                byte[] zipData = new byte[tempStream.Length];
                tempStream.Read(zipData, 0, (int)tempStream.Length);

                WriteChunk(PngChunkTypes.Data, zipData);
            }
        }

        private void WriteDataChunks()
        {
            byte[] pixels = _image.Pixels;

            byte[] data = new byte[_image.PixelWidth * _image.PixelHeight * 4 + _image.PixelHeight];

            int rowLength = _image.PixelWidth * 4 + 1;

            for (int y = 0; y < _image.PixelHeight; y++)
            {
                byte compression = 0;
                if (y > 0)
                {
                    compression = 2;
                }
                data[y * rowLength] = compression;

                for (int x = 0; x < _image.PixelWidth; x++)
                {
                    // Calculate the offset for the new array.
                    int dataOffset = y * rowLength + x * 4 + 1;

                    // Calculate the offset for the original pixel array.
                    int pixelOffset = (y * _image.PixelWidth + x) * 4;

                    data[dataOffset + 0] = pixels[pixelOffset + 0];
                    data[dataOffset + 1] = pixels[pixelOffset + 1];
                    data[dataOffset + 2] = pixels[pixelOffset + 2];
                    data[dataOffset + 3] = pixels[pixelOffset + 3];

                    if (y > 0)
                    {
                        int lastOffset = ((y - 1) * _image.PixelWidth + x) * 4;

                        data[dataOffset + 0] -= pixels[lastOffset + 0];
                        data[dataOffset + 1] -= pixels[lastOffset + 1];
                        data[dataOffset + 2] -= pixels[lastOffset + 2];
                        data[dataOffset + 3] -= pixels[lastOffset + 3];
                    }
                }
            }

            byte[] buffer = null;
            int bufferLength = 0;

            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();

                //using (Ionic.Zlib.DeflateStream zStream = new Ionic.Zlib.DeflateStream(memoryStream, Ionic.Zlib.CompressionMode.Compress))
                using (System.IO.Compression.DeflateStream zStream = new System.IO.Compression.DeflateStream(memoryStream, System.IO.Compression.CompressionMode.Compress))
                {
                    zStream.Write(data, 0, data.Length);
                    zStream.Flush();

                    bufferLength = (int)memoryStream.Length;
                    buffer = memoryStream.GetBuffer();

                    zStream.Close();
                }

                //using (DeflaterOutputStream zStream = new DeflaterOutputStream(memoryStream))
                //{
                //    zStream.Write(data, 0, data.Length);
                //    zStream.Flush();
                //    zStream.Finish();

                //    bufferLength = (int)memoryStream.Length;
                //    buffer = memoryStream.GetBuffer();
                //}
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                }
            }

            int numChunks = bufferLength / MaxBlockSize;

            if (bufferLength % MaxBlockSize != 0)
            {
                numChunks++;
            }

            for (int i = 0; i < numChunks; i++)
            {
                int length = bufferLength - i * MaxBlockSize;

                if (length > MaxBlockSize)
                {
                    length = MaxBlockSize;
                }

                WriteChunk(PngChunkTypes.Data, buffer, i * MaxBlockSize, length);
            }
        }

        private void WriteEndChunk()
        {
            WriteChunk(PngChunkTypes.End, null);
        }

        private void WriteHeaderChunk(PngHeader header)
        {
            byte[] chunkData = new byte[13];

            WriteInteger(chunkData, 0, header.Width);
            WriteInteger(chunkData, 4, header.Height);

            chunkData[8] = header.BitDepth;
            chunkData[9] = header.ColorType;
            chunkData[10] = header.CompressionMethod;
            chunkData[11] = header.FilterMethod;
            chunkData[12] = header.InterlaceMethod;

            WriteChunk(PngChunkTypes.Header, chunkData);
        }

        private void WriteChunk(string type, byte[] data)
        {
            WriteChunk(type, data, 0, data != null ? data.Length : 0);
        }

        private void WriteChunk(string type, byte[] data, int offset, int length)
        {
            WriteInteger(_stream, length);

            byte[] typeArray = new byte[4];
            typeArray[0] = (byte)type[0];
            typeArray[1] = (byte)type[1];
            typeArray[2] = (byte)type[2];
            typeArray[3] = (byte)type[3];

            _stream.Write(typeArray, 0, 4);

            if (data != null)
            {
                _stream.Write(data, offset, length);
            }



            //Crc32 crc32 = new Crc32();
            //crc32.Update(typeArray);
            CRC32Calculator crc32Cal = new CRC32Calculator();
            crc32Cal.SlurpBlock(typeArray, 0, typeArray.Length);

            if (data != null)
            {
                // crc32.Update(data, offset, length);
                crc32Cal.SlurpBlock(data, offset, length);
            }

            //WriteInteger(_stream, (uint)crc32.Value);
            WriteInteger(_stream, (uint)crc32Cal.Crc32Result);
        }

        private static void WriteInteger(byte[] data, int offset, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Array.Reverse(buffer);
            Array.Copy(buffer, 0, data, offset, 4);
        }

        private static void WriteInteger(Stream stream, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Array.Reverse(buffer);

            stream.Write(buffer, 0, 4);
        }

        private static void WriteInteger(Stream stream, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Array.Reverse(buffer);

            stream.Write(buffer, 0, 4);
        }

        #endregion
    }
}
