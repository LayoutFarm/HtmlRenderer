using System;
using System.Collections.Generic;
using System.Globalization;

using System.Runtime.InteropServices;

namespace Icu
{
    public static class UnicodeSet
    {
        /// <summary>
        /// Returns a string representation of this Unicode set
        /// </summary>
        /// <param name="set">Unicode set to convert.  Null set throws an exception</param>
        /// <returns>pattern string</returns>
        public static string ToPattern(IEnumerable<string> set)
        {
            if (set == null)
            {
                throw new ArgumentNullException("set");
            }
            // uset_openEmpty unavailable, so this is equivalent
            IntPtr uset = NativeMethods.uset_open('1', '0');
            try
            {
                foreach (string str in set)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (str.Length == 1)
                            NativeMethods.uset_add(uset, str[0]);
                        else
                            NativeMethods.uset_addString(uset, str, str.Length);
                    }
                }

                var err = ErrorCode.ZERO_ERROR;
                int resultCapacity = NativeMethods.uset_toPattern(uset, IntPtr.Zero, 0, true, ref err);
                IntPtr buffer = Marshal.AllocCoTaskMem(resultCapacity * 2);
                try
                {
                    err = ErrorCode.ZERO_ERROR;
                    resultCapacity = NativeMethods.uset_toPattern(uset, buffer, resultCapacity, true, ref err);
                    if (err > ErrorCode.NoErrors)
                        throw new Exception("UnicodeSet.ToPattern() failed with code " + err);
                    return Marshal.PtrToStringUni(buffer, resultCapacity);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(buffer);
                }
            }
            finally
            {
                NativeMethods.uset_close(uset);
            }
        }

        /// <summary>
        /// Creates a Unicode set from the given pattern
        /// </summary>
        /// <param name="pattern">A string specifying what characters are in the set.  Null pattern returns an empty set</param>
        /// <returns>Unicode set of characters.</returns>
        public static IEnumerable<string> ToCharacters(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return new string[] { };
               // return Enumerable.Empty<string>();
            }

            var err = ErrorCode.ZERO_ERROR;
            IntPtr result = NativeMethods.uset_openPattern(pattern, -1, ref err);
            try
            {
                if (err != ErrorCode.NoErrors)
                    throw new ArgumentException("pattern");
                var output = new List<string>();

                // Parse the number of items in the Unicode set
                for (int i = 0; i < NativeMethods.uset_getItemCount(result); i++)
                {
                    int startChar, endChar;
                    int strLength = NativeMethods.uset_getItem(result, i, out startChar, out endChar, IntPtr.Zero, 0, ref err);
                    if (strLength == 0)
                    {
                        // Add a character range to the set
                        for (int j = startChar; j <= endChar; j++)
                        {
                            output.Add(((char)j).ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    else
                    {
                        // Add a multiple-character string to the set
                        IntPtr buffer = Marshal.AllocCoTaskMem(strLength * 2);
                        try
                        {
                            err = ErrorCode.ZERO_ERROR;
                            strLength = NativeMethods.uset_getItem(result, i, out startChar, out endChar, buffer, strLength, ref err);
                            if (err > ErrorCode.NoErrors)
                                throw new Exception("UnicodeSet.ToCharacters() failed with code " + err);
                            output.Add(Marshal.PtrToStringUni(buffer, strLength));
                        }
                        finally
                        {
                            Marshal.FreeCoTaskMem(buffer);
                        }
                    }
                }
                return output;
            }
            finally
            {
                NativeMethods.uset_close(result);
            }
        }
    }
}
