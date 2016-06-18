// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
using System.Globalization;
using System.Runtime.InteropServices;
namespace Icu
{
    public static class Character
    {
        /// <summary>
        /// Defined in ICU uchar.h
        /// http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html#l00158
        /// </summary>
        public enum UProperty
        {
            /*  See note !!.  Comments of the form "Binary property Dash",
			"Enumerated property Script", "Double property Numeric_Value",
			and "String property Age" are read by genpname. */

            /*  Note: Place ALPHABETIC before BINARY_START so that
			debuggers display ALPHABETIC as the symbolic name for 0,
			rather than BINARY_START.  Likewise for other *_START
			identifiers. */

            ALPHABETIC = 0,
            BINARY_START = ALPHABETIC,
            ASCII_HEX_DIGIT = 1,
            BIDI_CONTROL = 2,
            BIDI_MIRRORED = 3,
            DASH = 4,
            DEFAULT_IGNORABLE_CODE_POINT = 5,
            DEPRECATED = 6,
            DIACRITIC = 7,
            EXTENDER = 8,
            FULL_COMPOSITION_EXCLUSION = 9,
            GRAPHEME_BASE = 10,
            GRAPHEME_EXTEND = 11,
            GRAPHEME_LINK = 12,
            HEX_DIGIT = 13,
            HYPHEN = 14,
            ID_CONTINUE = 15,
            ID_START = 16,
            IDEOGRAPHIC = 17,
            IDS_BINARY_OPERATOR = 18,
            IDS_TRINARY_OPERATOR = 19,
            JOIN_CONTROL = 20,
            LOGICAL_ORDER_EXCEPTION = 21,
            LOWERCASE = 22,
            MATH = 23,
            NONCHARACTER_CODE_POINT = 24,
            QUOTATION_MARK = 25,
            RADICAL = 26,
            SOFT_DOTTED = 27,
            TERMINAL_PUNCTUATION = 28,
            UNIFIED_IDEOGRAPH = 29,
            UPPERCASE = 30,
            WHITE_SPACE = 31,
            XID_CONTINUE = 32,
            XID_START = 33,
            CASE_SENSITIVE = 34,
            S_TERM = 35,
            VARIATION_SELECTOR = 36,
            NFD_INERT = 37,
            NFKD_INERT = 38,
            NFC_INERT = 39,
            NFKC_INERT = 40,
            SEGMENT_STARTER = 41,
            PATTERN_SYNTAX = 42,
            PATTERN_WHITE_SPACE = 43,
            POSIX_ALNUM = 44,
            POSIX_BLANK = 45,
            POSIX_GRAPH = 46,
            POSIX_PRINT = 47,
            POSIX_XDIGIT = 48,
            CASED = 49,
            CASE_IGNORABLE = 50,
            CHANGES_WHEN_LOWERCASED = 51,
            CHANGES_WHEN_UPPERCASED = 52,
            CHANGES_WHEN_TITLECASED = 53,
            CHANGES_WHEN_CASEFOLDED = 54,
            CHANGES_WHEN_CASEMAPPED = 55,
            CHANGES_WHEN_NFKC_CASEFOLDED = 56,
            BINARY_LIMIT = 57,
            BIDI_CLASS = 0x1000,
            INT_START = BIDI_CLASS,
            BLOCK = 0x1001,
            CANONICAL_COMBINING_CLASS = 0x1002,
            DECOMPOSITION_TYPE = 0x1003,
            EAST_ASIAN_WIDTH = 0x1004,
            GENERAL_CATEGORY = 0x1005,
            JOINING_GROUP = 0x1006,
            JOINING_TYPE = 0x1007,
            LINE_BREAK = 0x1008,
            NUMERIC_TYPE = 0x1009,
            SCRIPT = 0x100A,
            HANGUL_SYLLABLE_TYPE = 0x100B,
            NFD_QUICK_CHECK = 0x100C,
            NFKD_QUICK_CHECK = 0x100D,
            NFC_QUICK_CHECK = 0x100E,
            NFKC_QUICK_CHECK = 0x100F,
            LEAD_CANONICAL_COMBINING_CLASS = 0x1010,
            TRAIL_CANONICAL_COMBINING_CLASS = 0x1011,
            GRAPHEME_CLUSTER_BREAK = 0x1012,
            SENTENCE_BREAK = 0x1013,
            WORD_BREAK = 0x1014,
            INT_LIMIT = 0x1015,
            GENERAL_CATEGORY_MASK = 0x2000,
            MASK_START = GENERAL_CATEGORY_MASK,
            MASK_LIMIT = 0x2001,
            NUMERIC_VALUE = 0x3000,
            DOUBLE_START = NUMERIC_VALUE,
            DOUBLE_LIMIT = 0x3001,
            AGE = 0x4000,
            STRING_START = AGE,
            BIDI_MIRRORING_GLYPH = 0x4001,
            CASE_FOLDING = 0x4002,
            ISO_COMMENT = 0x4003,
            LOWERCASE_MAPPING = 0x4004,
            NAME = 0x4005,
            SIMPLE_CASE_FOLDING = 0x4006,
            SIMPLE_LOWERCASE_MAPPING = 0x4007,
            SIMPLE_TITLECASE_MAPPING = 0x4008,
            SIMPLE_UPPERCASE_MAPPING = 0x4009,
            TITLECASE_MAPPING = 0x400A,
            UNICODE_1_NAME = 0x400B,
            UPPERCASE_MAPPING = 0x400C,
            STRING_LIMIT = 0x400D,
            SCRIPT_EXTENSIONS = 0x7000,
            OTHER_PROPERTY_START = SCRIPT_EXTENSIONS,
            OTHER_PROPERTY_LIMIT = 0x7001,
            INVALID_CODE = -1
        }

        ///<summary>
        /// enumerated Unicode general category types.
        /// See http://www.unicode.org/Public/UNIDATA/UnicodeData.html .
        /// ///</summary>
        public enum UCharCategory
        {
            ///<summary>Non-category for unassigned and non-character code points.</summary>
            UNASSIGNED = 0,
            ///<summary>Cn "Other, Not Assigned (no characters in [UnicodeData.txt] have this property)" (same as U_UNASSIGNED!)</summary>
            GENERAL_OTHER_TYPES = 0,
            ///<summary>Lu</summary>
            UPPERCASE_LETTER = 1,
            ///<summary>Ll</summary>
            LOWERCASE_LETTER = 2,
            ///<summary>Lt</summary>
            TITLECASE_LETTER = 3,
            ///<summary>Lm</summary>
            MODIFIER_LETTER = 4,
            ///<summary>Lo</summary>
            OTHER_LETTER = 5,
            ///<summary>Mn</summary>
            NON_SPACING_MARK = 6,
            ///<summary>Me</summary>
            ENCLOSING_MARK = 7,
            ///<summary>Mc</summary>
            COMBINING_SPACING_MARK = 8,
            ///<summary>Nd</summary>
            DECIMAL_DIGIT_NUMBER = 9,
            ///<summary>Nl</summary>
            LETTER_NUMBER = 10,
            ///<summary>No</summary>
            OTHER_NUMBER = 11,
            ///<summary>Zs</summary>
            SPACE_SEPARATOR = 12,
            ///<summary>Zl</summary>
            LINE_SEPARATOR = 13,
            ///<summary>Zp</summary>
            PARAGRAPH_SEPARATOR = 14,
            ///<summary>Cc</summary>
            CONTROL_CHAR = 15,
            ///<summary>Cf</summary>
            FORMAT_CHAR = 16,
            ///<summary>Co</summary>
            PRIVATE_USE_CHAR = 17,
            ///<summary>Cs</summary>
            SURROGATE = 18,
            ///<summary>Pd</summary>
            DASH_PUNCTUATION = 19,
            ///<summary>Ps</summary>
            START_PUNCTUATION = 20,
            ///<summary>Pe</summary>
            END_PUNCTUATION = 21,
            ///<summary>Pc</summary>
            CONNECTOR_PUNCTUATION = 22,
            ///<summary>Po</summary>
            OTHER_PUNCTUATION = 23,
            ///<summary>Sm</summary>
            MATH_SYMBOL = 24,
            ///<summary>Sc</summary>
            CURRENCY_SYMBOL = 25,
            ///<summary>Sk</summary>
            MODIFIER_SYMBOL = 26,
            ///<summary>So</summary>
            OTHER_SYMBOL = 27,
            ///<summary>Pi</summary>
            INITIAL_PUNCTUATION = 28,
            ///<summary>Pf</summary>
            FINAL_PUNCTUATION = 29,
            ///<summary>One higher than the last enum UCharCategory constant.</summary>
            CHAR_CATEGORY_COUNT
        }

        /// <summary>
        /// Selector constants for u_charName().
        /// u_charName() returns the "modern" name of a Unicode character; or the name that was
        /// defined in Unicode version 1.0, before the Unicode standard merged with ISO-10646; or
        /// an "extended" name that gives each Unicode code point a unique name.
        /// </summary>
        public enum UCharNameChoice
        {
            /// <summary></summary>
            UNICODE_CHAR_NAME,
            /// <summary></summary>
            UNICODE_10_CHAR_NAME,
            /// <summary></summary>
            EXTENDED_CHAR_NAME,
            /// <summary></summary>
            CHAR_NAME_CHOICE_COUNT
        }

        /// <summary>
        /// Decomposition Type constants.
        /// </summary>
        public enum UDecompositionType
        {
            NONE,
            CANONICAL,
            COMPAT,
            CIRCLE,
            FINAL,
            FONT,
            FRACTION,
            INITIAL,
            ISOLATED,
            MEDIAL,
            NARROW,
            NOBREAK,
            SMALL,
            SQUARE,
            SUB,
            SUPER,
            VERTICAL,
            WIDE,
            COUNT
        }

        /// <summary>
        /// Numeric Type constants
        /// </summary>
        public enum UNumericType
        {
            NONE,
            DECIMAL,
            DIGIT,
            NUMERIC,
            COUNT
        }

        public const double NO_NUMERIC_VALUE = (double)-123456789;
        /// <summary></summary>
        public static int Digit(int characterCode, byte radix)
        {
            return NativeMethods.u_digit(characterCode, radix);
        }

        /// <summary>
        /// Determines whether the specified character code is alphabetic, based on the
        /// UProperty.ALPHABETIC property.
        /// </summary>
        /// <param name="characterCode">The character code.</param>
        public static bool IsAlphabetic(int characterCode)
        {
            return NativeMethods.u_getIntPropertyValue(characterCode, UProperty.ALPHABETIC) != 0;
        }

        /// <summary>
        /// Determines whether the specified character code is ideographic, based on the
        /// UProperty.IDEOGRAPHIC property.
        /// </summary>
        /// <param name="characterCode">The character code.</param>
        public static bool IsIdeographic(int characterCode)
        {
            return NativeMethods.u_getIntPropertyValue(characterCode, UProperty.IDEOGRAPHIC) != 0;
        }

        /// <summary>
        /// Determines whether the specified character code is alphabetic, based on the
        /// UProperty.DIACRITIC property.
        /// </summary>
        /// <param name="characterCode">The character code.</param>
        public static bool IsDiacritic(int characterCode)
        {
            return NativeMethods.u_getIntPropertyValue(characterCode, UProperty.DIACRITIC) != 0;
        }

        /// <summary>
        ///	Determines whether the specified code point is a symbol character
        /// </summary>
        /// <param name="characterCode">the code point to be tested</param>
        public static bool IsSymbol(int characterCode)
        {
            var nAns = NativeMethods.u_charType(characterCode);
            return nAns == (int)UCharCategory.MATH_SYMBOL ||
                nAns == (int)UCharCategory.CURRENCY_SYMBOL ||
                    nAns == (int)UCharCategory.MODIFIER_SYMBOL ||
                    nAns == (int)UCharCategory.OTHER_SYMBOL;
        }

        ///<summary>
        /// Get the general character category value for the given code point.
        ///</summary>
        ///<param name="ch">the code point to be checked</param>
        ///<returns></returns>
        public static UCharCategory GetCharType(int ch)
        {
            return (UCharCategory)NativeMethods.u_charType(ch);
        }

        /// <summary>
        /// Determines whether the specified character code is numeric, based on the
        /// UProperty.NUMERIC_TYPE property.
        /// </summary>
        /// <param name="characterCode">The character code.</param>
        public static bool IsNumeric(int characterCode)
        {
            return NativeMethods.u_getIntPropertyValue(characterCode, UProperty.NUMERIC_TYPE) != 0;
        }

        /// <summary></summary>
        public static double GetNumericValue(int characterCode)
        {
            return NativeMethods.u_getNumericValue(characterCode);
        }

        /// <summary>Determines whether the specified code point is a punctuation character, as
        /// defined by the ICU NativeMethods.u_ispunct function.</summary>
        public static bool IsPunct(int characterCode)
        {
            return NativeMethods.u_ispunct(characterCode);
        }

        /// <summary>Determines whether the code point has the Bidi_Mirrored property. </summary>
        public static bool IsMirrored(int characterCode)
        {
            return NativeMethods.u_isMirrored(characterCode);
        }

        /// <summary>Determines whether the specified code point is a control character, as
        /// defined by the ICU NativeMethods.u_iscntrl function.</summary>
        public static bool IsControl(int characterCode)
        {
            return NativeMethods.u_iscntrl(characterCode);
        }

        /// <summary>
        ///	Determines whether the specified character is a control character. A control
        ///	character is one of the following:
        /// <list>
        ///	<item>ISO 8-bit control character (U+0000..U+001f and U+007f..U+009f)</item>
        ///	<item>U_CONTROL_CHAR (Cc)</item>
        ///	<item>U_FORMAT_CHAR (Cf)</item>
        ///	<item>U_LINE_SEPARATOR (Zl)</item>
        ///	<item>U_PARAGRAPH_SEPARATOR (Zp)</item>
        ///	</list>
        /// </summary>
        public static bool IsControl(string chr)
        {
            return (string.IsNullOrEmpty(chr) || chr.Length != 1 ? false : IsControl(chr[0]));
        }

        /// <summary>Determines whether the specified character is a space character, as
        /// defined by the ICU NativeMethods.u_isspace function.</summary>
        public static bool IsSpace(int characterCode)
        {
            return NativeMethods.u_isspace(characterCode);
        }

        /// <summary>
        ///	Determines whether the specified character is a space character.
        /// </summary>
        public static bool IsSpace(string chr)
        {
            return (string.IsNullOrEmpty(chr) || chr.Length != 1 ? false : IsSpace(chr[0]));
        }

        /// <summary>
        /// Get the description for a given ICU code point.
        /// </summary>
        /// <param name="code">the code point to get description/name of</param>
        /// <param name="nameChoice">what type of information to retrieve</param>
        /// <param name="name">return string</param>
        /// <returns>length of string</returns>
        private static int CharName(int code, UCharNameChoice nameChoice, out string name)
        {
            const int nSize = 255;
            IntPtr resPtr = Marshal.AllocCoTaskMem(nSize);
            try
            {
                ErrorCode error;
                int nResult = NativeMethods.u_CharName(code, nameChoice, resPtr, nSize, out error);
                if (error != ErrorCode.NoErrors)
                {
                    nResult = -1;
                    name = null;
                }
                else
                    name = Marshal.PtrToStringAnsi(resPtr);
                return nResult;
            }
            finally
            {
                Marshal.FreeCoTaskMem(resPtr);
            }
        }

        /// <summary>
        /// Gets the ICU display name of the specified character.
        /// </summary>
        public static string GetPrettyICUCharName(string chr)
        {
            if (!string.IsNullOrEmpty(chr) && chr.Length == 1)
            {
                string name;
                if (CharName(chr[0], UCharNameChoice.UNICODE_CHAR_NAME, out name) > 0)
                {
                    name = name.ToLower();
#if !PCL
                    return CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(name);
#else
                    //TODO: review here
                    throw new NotSupportedException();
#endif
                }
            }
            return null;
        }



        /// <summary>
        /// Gets the raw ICU display name of the specified character code.
        /// </summary>
        public static string GetCharName(int code)
        {
            string name;
            if (CharName(code, UCharNameChoice.UNICODE_CHAR_NAME, out name) > 0)
            {
                return name;
            }
            return null;
        }
    }
}
