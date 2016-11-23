using System;

namespace BitMiracle.LibJpeg
{
    /// <summary>
    /// Represents a row of image - collection of samples.
    /// </summary>
#if EXPOSE_LIBJPEG
    public
#endif
    class SampleRow
    {
        byte[] m_bytes;
        short[] lineBuffer;
        int columnCount;
        int componentsPerSample;

        /// <summary>
        /// Creates a row from raw samples data.
        /// </summary>
        /// <param name="row">Raw description of samples.<br/>
        /// You can pass collection with more than sampleCount samples - only sampleCount samples 
        /// will be parsed and all remaining bytes will be ignored.</param>
        /// <param name="columnCount">The number of samples in row.</param>
        /// <param name="bitsPerComponent">The number of bits per component.</param>
        /// <param name="componentsPerSample">The number of components per sample.</param>
        public SampleRow(byte[] row, int columnCount, byte bitsPerComponent, byte componentsPerSample)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            if (row.Length == 0)
                throw new ArgumentException("row is empty");

            if (columnCount <= 0)
                throw new ArgumentOutOfRangeException("sampleCount");

            if (bitsPerComponent <= 0 || bitsPerComponent > 16)
                throw new ArgumentOutOfRangeException("bitsPerComponent");

            if (componentsPerSample <= 0 || componentsPerSample > 5) //1,2,3,4
                throw new ArgumentOutOfRangeException("componentsPerSample");

            this.componentsPerSample = componentsPerSample;
            this.m_bytes = row;
            this.columnCount = columnCount;
            using (BitStream bitStream = new BitStream(row))
            {
                //create long buffer for a single line                
                lineBuffer = new short[columnCount * componentsPerSample];
                int byteIndex = 0;
                for (int i = 0; i < columnCount; ++i)
                {
                    //each component
                    //eg. 1,2,3,4 
                    switch (componentsPerSample)
                    {
                        case 1:
                            lineBuffer[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                            byteIndex++;
                            break;
                        case 2:
                            lineBuffer[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                            lineBuffer[byteIndex + 1] = (short)bitStream.Read(bitsPerComponent);
                            byteIndex += 2;
                            break;
                        case 3:
                            lineBuffer[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                            lineBuffer[byteIndex + 1] = (short)bitStream.Read(bitsPerComponent);
                            lineBuffer[byteIndex + 2] = (short)bitStream.Read(bitsPerComponent);
                            byteIndex += 3;
                            break;
                        case 4:
                            lineBuffer[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                            lineBuffer[byteIndex + 1] = (short)bitStream.Read(bitsPerComponent);
                            lineBuffer[byteIndex + 2] = (short)bitStream.Read(bitsPerComponent);
                            lineBuffer[byteIndex + 4] = (short)bitStream.Read(bitsPerComponent);
                            byteIndex += 4;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    //for (short i = 0; i < componentCount; ++i)
                    //{
                    //    //bitPerSample may >8 bits                
                    //    m_components[i] = (short)bitStream.Read(bitsPerComponent);
                    //}
                }
                //m_samples = new Sample[sampleCount];
                //for (int i = 0; i < sampleCount; ++i)
                //    m_samples[i] = new Sample(bitStream, bitsPerComponent, componentsPerSample);
            }
        }


        public int ComponentsPerSample
        {
            get { return this.componentsPerSample; }
        }

        public void GetComponentsAt(int column, out byte r, out byte g, out byte b)
        {
            //no alpha channel for jpeg 
            switch (componentsPerSample)
            {
                case 1:
                    {
                        r = g = b = (byte)lineBuffer[column];
                    }
                    break;
                case 2:
                    {
                        //2 byte per sample?                        
                        throw new NotSupportedException(); //?
                    }
                case 3:
                    {
                        int pos = column * 3;
                        r = (byte)lineBuffer[pos];
                        g = (byte)lineBuffer[pos + 1];
                        b = (byte)lineBuffer[pos + 2];
                    }
                    break;
                case 4:
                    {
                        //should not occurs?
                        throw new NotSupportedException(); //?
                    }
                default:
                    throw new NotSupportedException();
            }
        }
        public void WriteToList(System.Collections.Generic.List<byte> outputBytes)
        {
            //write bytes of this row to output bytes 
            for (int i = 0; i < columnCount; ++i)
            {
                switch (componentsPerSample)
                {
                    case 1:
                        {
                            outputBytes.Add((byte)lineBuffer[i]);
                        }
                        break;
                    case 2:
                        {
                            //2 byte per sample?                        
                            throw new NotSupportedException(); //?
                        }
                    case 3:
                        {
                            int pos = i * 3;
                            outputBytes.Add((byte)lineBuffer[pos]);
                            outputBytes.Add((byte)lineBuffer[pos + 1]);
                            outputBytes.Add((byte)lineBuffer[pos + 2]);
                        }
                        break;
                    case 4:
                        {
                            //should not occurs?
                            throw new NotSupportedException(); //?
                        }
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        //public int ComponentsPerSample
        //{
        //    get { return this.componentsPerSample; }
        //}
        ///// <summary>
        ///// Creates row from an array of components.
        ///// </summary>
        ///// <param name="sampleComponents">Array of color components.</param>
        ///// <param name="bitsPerComponent">The number of bits per component.</param>
        ///// <param name="componentsPerSample">The number of components per sample.</param>
        ///// <remarks>The difference between this constructor and 
        ///// <see cref="M:BitMiracle.LibJpeg.SampleRow.#ctor(System.Byte[],System.Int32,System.Byte,System.Byte)">another one</see> -
        ///// this constructor accept an array of prepared color components whereas
        ///// another constructor accept raw bytes and parse them.
        ///// </remarks>
        //internal SampleRow(short[] sampleComponents, byte bitsPerComponent, byte componentsPerSample)
        //{
        //    if (sampleComponents == null)
        //        throw new ArgumentNullException("sampleComponents");

        //    if (sampleComponents.Length == 0)
        //        throw new ArgumentException("row is empty");

        //    if (bitsPerComponent <= 0 || bitsPerComponent > 16)
        //        throw new ArgumentOutOfRangeException("bitsPerComponent");

        //    if (componentsPerSample <= 0 || componentsPerSample > 5)
        //        throw new ArgumentOutOfRangeException("componentsPerSample");

        //    int sampleCount = sampleComponents.Length / componentsPerSample;

        //    m_samples = new Sample[sampleCount];
        //    for (int i = 0; i < sampleCount; ++i)
        //    {
        //        short[] components = new short[componentsPerSample];
        //        Buffer.BlockCopy(sampleComponents, i * componentsPerSample * sizeof(short), components, 0, componentsPerSample * sizeof(short));
        //        m_samples[i] = new Sample(components, bitsPerComponent);
        //    }

        //    using (BitStream bits = new BitStream())
        //    {
        //        for (int i = 0; i < sampleCount; ++i)
        //        {
        //            for (int j = 0; j < componentsPerSample; ++j)
        //                bits.Write(sampleComponents[i * componentsPerSample + j], bitsPerComponent);
        //        }

        //        m_bytes = new byte[bits.UnderlyingStream.Length];
        //        bits.UnderlyingStream.Seek(0, System.IO.SeekOrigin.Begin);
        //        bits.UnderlyingStream.Read(m_bytes, 0, (int)bits.UnderlyingStream.Length);
        //    }
        //}


        /// <summary>
        /// Gets the number of samples in this row.
        /// </summary>
        /// <value>The number of samples.</value>
        public int Length
        {
            get
            {
                return columnCount;
                //return m_samples.Length;
            }
        }


        ///// <summary>
        ///// Gets the sample at the specified index.
        ///// </summary>
        ///// <param name="sampleNumber">The number of sample.</param>
        ///// <returns>The required sample.</returns>
        //public Sample this[int sampleNumber]
        //{
        //    get
        //    {
        //        return GetAt(sampleNumber);
        //    }
        //}

        ///// <summary>
        ///// Gets the sample at the specified index.
        ///// </summary>
        ///// <param name="sampleNumber">The number of sample.</param>
        ///// <returns>The required sample.</returns>
        //public Sample GetAt(int sampleNumber)
        //{
        //    return m_samples[sampleNumber];
        //}

        ///// <summary>
        ///// Serializes this row to raw bytes.
        ///// </summary>
        ///// <returns>The row representation as array of bytes</returns>
        //public byte[] ToBytes()
        //{
        //    return m_bytes;
        //}
    }
}
