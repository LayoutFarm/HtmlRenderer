// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

namespace Icu.Collation
{
    /// <summary>Use this to set the strength of a Collator object.
    ///  This is also used to determine the strength of sort keys
    ///  generated from Collator objects
    /// The usual strength for most locales (except Japanese) is tertiary.
    /// Quaternary strength is useful when combined with shifted setting
    /// for alternate handling attribute and for JIS x 4061 collation,
    /// when it is used to distinguish between Katakana  and Hiragana
    /// (this is achieved by setting the HiraganaQuaternary mode to on.
    /// Otherwise, quaternary level is affected only by the number of
    /// non ignorable code points in the string.
    /// </summary>
    public enum CollationStrength
    {
        /// <summary>
        /// Use the strength set in the locale or rules
        /// </summary>
        Default = -1,
        /// <summary>
        /// Base letter represents a primary difference.  Set comparison
        /// level to Primary to ignore secondary and tertiary differences.
        /// Example of primary difference, "abc" &lt; "abd"
        /// </summary>
        Primary = 0,
        /// <summary>
        /// Diacritical differences on the same base letter represent a secondary
        /// difference.  Set comparison level to Secondary to ignore tertiary
        /// differences.
        /// Example of secondary difference, "a&#x308;" >> "a".
        /// </summary>
        Secondary = 1,
        /// <summary>
        ///  Uppercase and lowercase versions of the same character represents a
        /// tertiary difference.  Set comparison level to Tertiary to include
        ///  all comparison differences.
        ///  Example of tertiary difference, "abc" &lt;&lt;&lt; "ABC".
        /// </summary>
        Tertiary = 2,
        /// <summary>
        /// Quaternary level is usually only affected by the number of
        /// non-ignorable code points in the string.
        /// </summary>
        Quaternary = 3,
        /// <summary>
        ///  Two characters are considered "identical" when they have the same
        /// unicode spellings.
        /// For example, "a&#x308;" == "a&#x308;".
        /// </summary>
        /// <remarks>Identical strength is rarely useful, as it amounts to
        /// codepoints of the NFD form of the string</remarks>
        Identical
    }
}