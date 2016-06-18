// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

namespace Icu.Collation
{
    /// <summary>
    /// Controls the ordering of upper and lower case letters.
    /// </summary>
    public enum CaseFirst
    {
        Default = -1,
        /// <summary>
        /// orders upper and lower case letters in accordance to their tertiary weights
        /// </summary>
        Off = 16,
        /// <summary>
        /// forces lower case letters to sort before upper case letters
        /// </summary>
        LowerFirst = 24,
        /// <summary>
        /// forces upper case letters to sort before lower case letters
        /// </summary>
        UpperFirst = 25
    }
}
