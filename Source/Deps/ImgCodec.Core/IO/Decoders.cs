//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.IO
{
    using System;
    using System.Collections.Generic; 

    /// <summary>
    /// Helper methods for decoders.
    /// </summary>
    public static class Decoders
    {
        private static List<Type> _decoderTypes = new List<Type>();

        /// <summary>
        /// Adds the type of the decoder to the list of available image decoders.
        /// </summary>
        /// <typeparam name="TDecoder">The type of the decoder.</typeparam>
        public static void AddDecoder<TDecoder>() where TDecoder : IImageDecoder
        {
            if (!_decoderTypes.Contains(typeof(TDecoder)))
            {
                _decoderTypes.Add(typeof(TDecoder));
            }
        }

        /// <summary>
        /// Gets a list of all available decoders.
        /// </summary>
        /// <returns>A list of all available decoders.</returns>
        public static List<IImageDecoder> GetAvailableDecoders()
        {
            List<IImageDecoder> decoders = new List<IImageDecoder>();

            foreach (Type decorderType in _decoderTypes)
            {
                if (decorderType != null)
                {
                    decoders.Add(Activator.CreateInstance(decorderType) as IImageDecoder);
                }
            }

            return decoders;
        }
    }
}
