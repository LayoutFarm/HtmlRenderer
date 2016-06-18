// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
using System.Runtime.InteropServices;
namespace Icu
{
    public class Normalizer
    {
        /// <summary>
        /// Normalization mode constants.
        /// </summary>
        public enum UNormalizationMode
        {
            /// <summary>No decomposition/composition.</summary>
            UNORM_NONE = 1,
            /// <summary>Canonical decomposition.</summary>
            UNORM_NFD = 2,
            /// <summary>Compatibility decomposition.</summary>
            UNORM_NFKD = 3,
            /// <summary>Canonical decomposition followed by canonical composition.</summary>
            UNORM_NFC = 4,
            /// <summary>Default normalization.</summary>
            UNORM_DEFAULT = UNORM_NFC,
            ///<summary>Compatibility decomposition followed by canonical composition.</summary>
            UNORM_NFKC = 5,
            /// <summary>"Fast C or D" form.</summary>
            UNORM_FCD = 6
        }

        /// <summary>
        /// Normalize the string according to the given mode.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string Normalize(string src, UNormalizationMode mode)
        {
            if (string.IsNullOrEmpty(src))
                return string.Empty;
            int length = src.Length + 10;
            IntPtr resPtr = Marshal.AllocCoTaskMem(length * 2);
            try
            {
                ErrorCode err;
                int outLength = NativeMethods.unorm_normalize(src, src.Length, mode, 0, resPtr, length, out err);
                if (err > 0 && err != ErrorCode.BUFFER_OVERFLOW_ERROR)
                    throw new Exception("Normalizer.Normalize() failed with code " + err);
                if (outLength >= length)
                {
                    Marshal.FreeCoTaskMem(resPtr);
                    length = outLength + 1;     // allow room for the terminating NUL (FWR-505)
                    resPtr = Marshal.AllocCoTaskMem(length * 2);
                    NativeMethods.unorm_normalize(src, src.Length, mode, 0, resPtr, length, out err);
                }
                if (err > 0)
                    throw new Exception("Normalizer.Normalize() failed with code " + err);
                string result = Marshal.PtrToStringUni(resPtr);
                // Strip any garbage left over at the end of the string.
                if (err == ErrorCode.STRING_NOT_TERMINATED_WARNING && result != null)
                    return result.Substring(0, outLength);
                return result;
            }
            finally
            {
                Marshal.FreeCoTaskMem(resPtr);
            }
        }

        /// <summary>
        /// Check whether the string is normalized according to the given mode.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool IsNormalized(string src, UNormalizationMode mode)
        {
            if (string.IsNullOrEmpty(src))
                return true;
            ErrorCode err;
            byte fIsNorm = NativeMethods.unorm_isNormalized(src, src.Length, mode, out err);
            if (err != ErrorCode.NoErrors)
                throw new Exception("Normalizer.IsNormalized() failed with code " + err);
            return fIsNorm != 0;
        }
    }
}
