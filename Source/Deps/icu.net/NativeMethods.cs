// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
using System.Runtime.InteropServices;
using Icu.Collation;
namespace Icu
{
    internal static class NativeMethods
    {
#if ICU_VER_40
		internal const string ICU_I18N_LIB = "icuin40.dll";
		internal const string ICU_COMMON_LIB = "icuuc40.dll";
		internal const string ICU_VERSION_SUFFIX = "_4_0";
#elif ICU_VER_48
		internal const string ICU_I18N_LIB = "icuin48.dll";
		internal const string ICU_COMMON_LIB = "icuuc48.dll";
		internal const string ICU_VERSION_SUFFIX = "_48";
#elif ICU_VER_49
		internal const string ICU_I18N_LIB = "icuin49.dll";
		internal const string ICU_COMMON_LIB = "icuuc49.dll";
		internal const string ICU_VERSION_SUFFIX = "_49";
#elif ICU_VER_50
		internal const string ICU_I18N_LIB = "icuin50.dll";
		internal const string ICU_COMMON_LIB = "icuuc50.dll";
		internal const string ICU_VERSION_SUFFIX = "_50";
#elif ICU_VER_51
		internal const string ICU_I18N_LIB = "icuin51.dll";
		internal const string ICU_COMMON_LIB = "icuuc51.dll";
		internal const string ICU_VERSION_SUFFIX = "_51";
#elif ICU_VER_52
		internal const string ICU_I18N_LIB = "icuin52.dll";
		internal const string ICU_COMMON_LIB = "icuuc52.dll";
		internal const string ICU_VERSION_SUFFIX = "_52";
		// Provide a bit of future proofing...
#elif ICU_VER_53
		internal const string ICU_I18N_LIB = "icuin53.dll";
		internal const string ICU_COMMON_LIB = "icuuc53.dll";
		internal const string ICU_VERSION_SUFFIX = "_53";
#elif ICU_VER_54
        internal const string ICU_I18N_LIB = "icuin54.dll";
        internal const string ICU_COMMON_LIB = "icuuc54.dll";
        internal const string ICU_VERSION_SUFFIX = "_54";
#elif ICU_VER_55
		internal const string ICU_I18N_LIB = "icuin55.dll";
		internal const string ICU_COMMON_LIB = "icuuc55.dll";
		internal const string ICU_VERSION_SUFFIX = "_55";
#elif ICU_VER_56
		internal const string ICU_I18N_LIB = "icuin56.dll";
		internal const string ICU_COMMON_LIB = "icuuc56.dll";
		internal const string ICU_VERSION_SUFFIX = "_56";
#elif ICU_VER_57
        internal const string ICU_I18N_LIB = "icuin57.dll";
        internal const string ICU_COMMON_LIB = "icuuc57.dll";
        internal const string ICU_VERSION_SUFFIX = "_57";
#else
#error We need to update the code for newer version of ICU after 56 (or older version before 4.8)
#endif

        /**
			 * Function type declaration for uenum_close().
			 *
			 * This function should cleanup the enumerator object
			 *
			 * @param en enumeration to be closed
			 */
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uenum_close" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uenum_close(IntPtr en);
        /**
             * Function type declaration for uenum_unext().
             *
             * This function returns the next element as a UChar *,
             * or NULL after all elements haven been enumerated.
             *
             * @param en enumeration
             * @param resultLength pointer to result length
             * @param status pointer to ErrorCode variable
             * @return next element as UChar *,
             *         or NULL after all elements haven been enumerated
             */
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uenum_unext" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uenum_unext(
            RuleBasedCollator.SafeEnumeratorHandle en,
            out int resultLength,
            out ErrorCode status);
        #region Unicode collator
        /// <summary>
        /// Open a Collator for comparing strings.
        /// Collator pointer is used in all the calls to the Collation
        /// service. After finished, collator must be disposed of by calling ucol_close
        /// </summary>
        /// <param name="loc">The locale containing the required collation rules.
        ///Special values for locales can be passed in -
        ///if NULL is passed for the locale, the default locale
        ///collation rules will be used. If empty string ("") or
        ///"root" are passed, UCA rules will be used.</param>
        /// <param name="status">A pointer to an ErrorCode to receive any errors
        ///</param>
        /// <returns>pointer to a Collator or 0 if an error occurred</returns>
        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_open" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_open(
            [MarshalAs(UnmanagedType.LPStr)] string loc,
            out ErrorCode status);
        /// <summary>
        /// Open a UCollator for comparing strings.
        /// </summary>
        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_open" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_open(
            byte[] loc,
            out ErrorCode err);
        ///<summary>
        /// Produce an Collator instance according to the rules supplied.
        /// The rules are used to change the default ordering, defined in the
        /// UCA in a process called tailoring. The resulting Collator pointer
        /// can be used in the same way as the one obtained by ucol_strcoll.
        /// </summary>
        /// <param name="rules">A string describing the collation rules. For the syntax
        ///    of the rules please see users guide.</param>
        /// <param name="rulesLength">The length of rules, or -1 if null-terminated.</param>
        /// <param name="normalizationMode">The normalization mode</param>
        /// <param name="strength">The default collation strength; can be also set in the rules</param>
        /// <param name="parseError">A pointer to ParseError to recieve information about errors
        /// occurred during parsing. This argument can currently be set
        /// to NULL, but at users own risk. Please provide a real structure.</param>
        /// <param name="status">A pointer to an ErrorCode to receive any errors</param>
        /// <returns>A pointer to a UCollator. It is not guaranteed that NULL be returned in case
        ///         of error - please use status argument to check for errors.</returns>
        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_openRules" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_openRules(
            [MarshalAs(UnmanagedType.LPWStr)] string rules,
            int rulesLength,
            NormalizationMode normalizationMode,
            CollationStrength strength,
            ref ParseError parseError,
            out ErrorCode status);
        /**
 * Open a collator defined by a short form string.
 * The structure and the syntax of the string is defined in the "Naming collators"
 * section of the users guide:
 * http://icu-project.org/userguide/Collate_Concepts.html#Naming_Collators
 * Attributes are overriden by the subsequent attributes. So, for "S2_S3", final
 * strength will be 3. 3066bis locale overrides individual locale parts.
 * The call to this function is equivalent to a call to ucol_open, followed by a
 * series of calls to ucol_setAttribute and ucol_setVariableTop.
 * @param definition A short string containing a locale and a set of attributes.
 *                   Attributes not explicitly mentioned are left at the default
 *                   state for a locale.
 * @param parseError if not NULL, structure that will get filled with error's pre
 *                   and post context in case of error.
 * @param forceDefaults if FALSE, the settings that are the same as the collator
 *                   default settings will not be applied (for example, setting
 *                   French secondary on a French collator would not be executed).
 *                   If TRUE, all the settings will be applied regardless of the
 *                   collator default value. If the definition
 *                   strings are to be cached, should be set to FALSE.
 * @param status     Error code. Apart from regular error conditions connected to
 *                   instantiating collators (like out of memory or similar), this
 *                   API will return an error if an invalid attribute or attribute/value
 *                   combination is specified.
 * @return           A pointer to a UCollator or 0 if an error occured (including an
 *                   invalid attribute).
 * @see ucol_open
 * @see ucol_setAttribute
 * @see ucol_setVariableTop
 * @see ucol_getShortDefinitionString
 * @see ucol_normalizeShortDefinitionString
 * @stable ICU 3.0
 *
 */

        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_openFromShortString"+ICU_VERSION_SUFFIX))]
            public static extern SafeRuleBasedCollatorHandle ucol_openFromShortString(
                    [MarshalAs(UnmanagedType.LPStr)] string definition,
                    [MarshalAs(UnmanagedType.I1)] bool forceDefaults,
                    ref ParseError parseError,
                    out ErrorCode status);
            */

        /**
 * Close a UCollator.
 * Once closed, a UCollator should not be used. Every open collator should
 * be closed. Otherwise, a memory leak will result.
 * @param coll The UCollator to close.
 * @stable ICU 2.0
 */


        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_close" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void ucol_close(IntPtr coll);
        /**
 * Compare two strings.
 * The strings will be compared using the options already specified.
 * @param coll The UCollator containing the comparison rules.
 * @param source The source string.
 * @param sourceLength The length of source, or -1 if null-terminated.
 * @param target The target string.
 * @param targetLength The length of target, or -1 if null-terminated.
 * @return The result of comparing the strings; one of UCOL_EQUAL,
 * UCOL_GREATER, UCOL_LESS
 * @see ucol_greater
 * @see ucol_greaterOrEqual
 * @see ucol_equal
 * @stable ICU 2.0
 */

        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_strcoll" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern CollationResult ucol_strcoll(RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
                                                          [MarshalAs(UnmanagedType.LPWStr)] string source,
                                                          Int32 sourceLength,
                                                          [MarshalAs(UnmanagedType.LPWStr)] string target,
                                                          Int32 targetLength);
        /**
 * Get the collation strength used in a UCollator.
 * The strength influences how strings are compared.
 * @param coll The UCollator to query.
 * @return The collation strength; one of UCOL_PRIMARY, UCOL_SECONDARY,
 * UCOL_TERTIARY, UCOL_QUATERNARY, UCOL_IDENTICAL
 * @see ucol_setStrength
 * @stable ICU 2.0
 */
        /*

[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getStrength"+ICU_VERSION_SUFFIX))]
public static extern CollationStrength ucol_getStrength(SafeRuleBasedCollatorHandle collator);
*/

        /**
* Set the collation strength used in a UCollator.
* The strength influences how strings are compared.
* @param coll The UCollator to set.
* @param strength The desired collation strength; one of UCOL_PRIMARY,
* UCOL_SECONDARY, UCOL_TERTIARY, UCOL_QUATERNARY, UCOL_IDENTICAL, UCOL_DEFAULT
* @see ucol_getStrength
* @stable ICU 2.0
*/
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_setStrength"+ICU_VERSION_SUFFIX))]
            public static extern void ucol_setStrength(SafeRuleBasedCollatorHandle collator,
                                                       CollationStrength strength);
            */

        /**
 * Get the display name for a UCollator.
 * The display name is suitable for presentation to a user.
 * @param objLoc The locale of the collator in question.
 * @param dispLoc The locale for display.
 * @param result A pointer to a buffer to receive the attribute.
 * @param resultLength The maximum size of result.
 * @param status A pointer to an ErrorCode to receive any errors
 * @return The total buffer size needed; if greater than resultLength,
 * the output was truncated.
 * @stable ICU 2.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getDisplayName"+ICU_VERSION_SUFFIX)]
            public static extern Int32 ucol_getDisplayName([MarshalAs(UnmanagedType.LPStr)] string objLoc,
                                                           [MarshalAs(UnmanagedType.LPStr)] string dispLoc,
                                                           [MarshalAs(UnmanagedType.LPWStr)] StringBuilder result,
                                                           Int32 resultLength,
                                                           out ErrorCode status);
            */
        /**
 * Get a locale for which collation rules are available.
 * A UCollator in a locale returned by this function will perform the correct
 * collation for the locale.
 * @param index The index of the desired locale.
 * @return A locale for which collation rules are available, or 0 if none.
 * @see ucol_countAvailable
 * @stable ICU 2.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getAvailable"+ICU_VERSION_SUFFIX)]
            [return : MarshalAs(UnmanagedType.LPStr)]
            public static extern string ucol_getAvailable(Int32 index);
            */

        /**
 * Determine how many locales have collation rules available.
 * This function is most useful as determining the loop ending condition for
 * calls to {@link #ucol_getAvailable }.
 * @return The number of locales for which collation rules are available.
 * @see ucol_getAvailable
 * @stable ICU 2.0
 */
        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_countAvailable" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 ucol_countAvailable();
        /**
 * Create a string enumerator of all locales for which a valid
 * collator may be opened.
 * @param status input-output error code
 * @return a string enumeration over locale strings. The caller is
 * responsible for closing the result.
 * @stable ICU 3.0
 */
        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_openAvailableLocales" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern RuleBasedCollator.SafeEnumeratorHandle ucol_openAvailableLocales(out ErrorCode status);
        /**
 * Create a string enumerator of all possible keywords that are relevant to
 * collation. At this point, the only recognized keyword for this
 * service is "collation".
 * @param status input-output error code
 * @return a string enumeration over locale strings. The caller is
 * responsible for closing the result.
 * @stable ICU 3.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getKeywords"+ICU_VERSION_SUFFIX))]
            public static extern IntPtr ucol_getKeywords(out ErrorCode status);
            */


        /**
 * Given a keyword, create a string enumeration of all values
 * for that keyword that are currently in use.
 * @param keyword a particular keyword as enumerated by
 * ucol_getKeywords. If any other keyword is passed in, *status is set
 * to U_ILLEGAL_ARGUMENT_ERROR.
 * @param status input-output error code
 * @return a string enumeration over collation keyword values, or NULL
 * upon error. The caller is responsible for closing the result.
 * @stable ICU 3.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getKeywordValues"+ICU_VERSION_SUFFIX))]
            public static extern IntPtr ucol_getKeywordValues([MarshalAs(UnmanagedType.LPStr)] string keyword,
                                                              out ErrorCode status);
            */



        /**
 * Return the functionally equivalent locale for the given
 * requested locale, with respect to given keyword, for the
 * collation service.  If two locales return the same result, then
 * collators instantiated for these locales will behave
 * equivalently.  The converse is not always true; two collators
 * may in fact be equivalent, but return different results, due to
 * internal details.  The return result has no other meaning than
 * that stated above, and implies nothing as to the relationship
 * between the two locales.  This is intended for use by
 * applications who wish to cache collators, or otherwise reuse
 * collators when possible.  The functional equivalent may change
 * over time.  For more information, please see the <a
 * href="http://icu-project.org/userguide/locale.html#services">
 * Locales and Services</a> section of the ICU User Guide.
 * @param result fillin for the functionally equivalent locale
 * @param resultCapacity capacity of the fillin buffer
 * @param keyword a particular keyword as enumerated by
 * ucol_getKeywords.
 * @param locale the requested locale
 * @param isAvailable if non-NULL, pointer to a fillin parameter that
 * indicates whether the requested locale was 'available' to the
 * collation service. A locale is defined as 'available' if it
 * physically exists within the collation locale data.
 * @param status pointer to input-output error code
 * @return the actual buffer size needed for the locale.  If greater
 * than resultCapacity, the returned full name will be truncated and
 * an error code will be returned.
 * @stable ICU 3.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getFunctionalEquivalent" + ICU_VERSION_SUFFIX)]
            public static extern Int32 ucol_getFunctionalEquivalent(
                    [MarshalAs(UnmanagedType.LPStr)] StringBuilder result,
                    Int32 resultCapacity,
                    [MarshalAs(UnmanagedType.LPStr)] string keyword,
                    [MarshalAs(UnmanagedType.LPStr)] string locale,
                    [MarshalAs(UnmanagedType.I1)] out bool isAvailable,
                    out ErrorCode status);
            */

        /**
 * Get the collation rules from a UCollator.
 * The rules will follow the rule syntax.
 * @param coll The UCollator to query.
 * @param length
 * @return The collation rules.
 * @stable ICU 2.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getRules" + ICU_VERSION_SUFFIX)]
            public static extern string ucol_getRules(SafeRuleBasedCollatorHandle collator,
                                                      out Int32 length);
            */

        /** Get the short definition string for a collator. This API harvests the collator's
 *  locale and the attribute set and produces a string that can be used for opening
 *  a collator with the same properties using the ucol_openFromShortString API.
 *  This string will be normalized.
 *  The structure and the syntax of the string is defined in the "Naming collators"
 *  section of the users guide:
 *  http://icu-project.org/userguide/Collate_Concepts.html#Naming_Collators
 *  This API supports preflighting.
 *  @param coll a collator
 *  @param locale a locale that will appear as a collators locale in the resulting
 *                short string definition. If NULL, the locale will be harvested
 *                from the collator.
 *  @param buffer space to hold the resulting string
 *  @param capacity capacity of the buffer
 *  @param status for returning errors. All the preflighting errors are featured
 *  @return length of the resulting string
 *  @see ucol_openFromShortString
 *  @see ucol_normalizeShortDefinitionString
 *  @stable ICU 3.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getShortDefinitionString" + ICU_VERSION_SUFFIX)]
            public static extern Int32 ucol_getShortDefinitionString(SafeRuleBasedCollatorHandle collator,
                                                                     [MarshalAs(UnmanagedType.LPStr)] string locale,
                                                                     [In,Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder
                                                                             buffer,
                                                                     Int32 capacity,
                                                                     out ErrorCode status);
            */
        /** Verifies and normalizes short definition string.
 *  Normalized short definition string has all the option sorted by the argument name,
 *  so that equivalent definition strings are the same.
 *  This API supports preflighting.
 *  @param source definition string
 *  @param destination space to hold the resulting string
 *  @param capacity capacity of the buffer
 *  @param parseError if not NULL, structure that will get filled with error's pre
 *                   and post context in case of error.
 *  @param status     Error code. This API will return an error if an invalid attribute
 *                    or attribute/value combination is specified. All the preflighting
 *                    errors are also featured
 *  @return length of the resulting normalized string.
 *
 *  @see ucol_openFromShortString
 *  @see ucol_getShortDefinitionString
 *
 *  @stable ICU 3.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_normalizeShortDefinitionString" + ICU_VERSION_SUFFIX)]
            public static extern Int32 ucol_normalizeShortDefinitionString(
                    [MarshalAs(UnmanagedType.LPStr)] string source,
                    [MarshalAs(UnmanagedType.LPStr)] StringBuilder destination,
                    Int32 capacity,
                    ref ParseError parseError,
                    out ErrorCode status);
            */
        /**
 * Get a sort key for a string from a UCollator.
 * Sort keys may be compared using <TT>strcmp</TT>.
 * @param coll The UCollator containing the collation rules.
 * @param source The string to transform.
 * @param sourceLength The length of source, or -1 if null-terminated.
 * @param result A pointer to a buffer to receive the attribute.
 * @param resultLength The maximum size of result.
 * @return The size needed to fully store the sort key..
 * @see ucol_keyHashCode
 * @stable ICU 2.0
 */

        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getSortKey" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 ucol_getSortKey(RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
                                                   [MarshalAs(UnmanagedType.LPWStr)] string source,
                                                   Int32 sourceLength,
                                                   [Out, MarshalAs(UnmanagedType.LPArray)] byte[]
                                                   result,
                                                   Int32 resultLength);
        /** Gets the next count bytes of a sort key. Caller needs
 *  to preserve state array between calls and to provide
 *  the same type of UCharIterator set with the same string.
 *  The destination buffer provided must be big enough to store
 *  the number of requested bytes. Generated sortkey is not
 *  compatible with sortkeys generated using ucol_getSortKey
 *  API, since we don't do any compression. If uncompressed
 *  sortkeys are required, this API can be used.
 *  @param coll The UCollator containing the collation rules.
 *  @param iter UCharIterator containing the string we need
 *              the sort key to be calculated for.
 *  @param state Opaque state of sortkey iteration.
 *  @param dest Buffer to hold the resulting sortkey part
 *  @param count number of sort key bytes required.
 *  @param status error code indicator.
 *  @return the actual number of bytes of a sortkey. It can be
 *          smaller than count if we have reached the end of
 *          the sort key.
 *  @stable ICU 2.6
 */
        /*
[DllImport(ICU_I18N_LIB, EntryPoint = "ucol_nextSortKeyPart"+ICU_VERSION_SUFFIX))]
        static public extern Int32
ucol_nextSortKeyPart(SafeRuleBasedCollatorHandle collator,
                     UCharIterator *iter,
                     [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] UInt32[] state,
                     [Out][MarshalAs(UnmanagedType.LPArray)] byte[]       dest,
                     Int32 count,
                     out ErrorCode status);
            */
        /*
            public enum CollationBoundMode
            {
                /// <summary>lower bound</summary>
                Lower = 0,
                /// <summary>upper bound that will match strings of exact size</summary>
                Upper = 1,
                /// <summary>upper bound that will match all the strings that have the same initial substring as the given string </summary>
                UpperLong = 2,
            }
            */
        /**
 * Produce a bound for a given sortkey and a number of levels.
 * Return value is always the number of bytes needed, regardless of
 * whether the result buffer was big enough or even valid.<br>
 * Resulting bounds can be used to produce a range of strings that are
 * between upper and lower bounds. For example, if bounds are produced
 * for a sortkey of string "smith", strings between upper and lower
 * bounds with one level would include "Smith", "SMITH", "sMiTh".<br>
 * There are two upper bounds that can be produced. If UCOL_BOUND_UPPER
 * is produced, strings matched would be as above. However, if bound
 * produced using UCOL_BOUND_UPPER_LONG is used, the above example will
 * also match "Smithsonian" and similar.<br>
 * For more on usage, see example in cintltst/capitst.c in procedure
 * TestBounds.
 * Sort keys may be compared using <TT>strcmp</TT>.
 * @param source The source sortkey.
 * @param sourceLength The length of source, or -1 if null-terminated.
 *                     (If an unmodified sortkey is passed, it is always null
 *                      terminated).
 * @param boundType Type of bound required. It can be UCOL_BOUND_LOWER, which
 *                  produces a lower inclusive bound, UCOL_BOUND_UPPER, that
 *                  produces upper bound that matches strings of the same length
 *                  or UCOL_BOUND_UPPER_LONG that matches strings that have the
 *                  same starting substring as the source string.
 * @param noOfLevels  Number of levels required in the resulting bound (for most
 *                    uses, the recommended value is 1). See users guide for
 *                    explanation on number of levels a sortkey can have.
 * @param result A pointer to a buffer to receive the resulting sortkey.
 * @param resultLength The maximum size of result.
 * @param status Used for returning error code if something went wrong. If the
 *               number of levels requested is higher than the number of levels
 *               in the source key, a warning (U_SORT_KEY_TOO_SHORT_WARNING) is
 *               issued.
 * @return The size needed to fully store the bound.
 * @see ucol_keyHashCode
 * @stable ICU 2.1
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getBound" + ICU_VERSION_SUFFIX)]
            public static extern Int32
                    ucol_getBound([MarshalAs(UnmanagedType.LPArray)] byte[] source,
                                  Int32 sourceLength,
                                  CollationBoundMode boundType,
                                  UInt32 noOfLevels,
                                  [Out][MarshalAs(UnmanagedType.LPArray)] byte[] result,
                                  Int32 resultLength,
                                  out ErrorCode status);
            */
        /**
 * Gets the version information for a Collator. Version is currently
 * an opaque 32-bit number which depends, among other things, on major
 * versions of the collator tailoring and UCA.
 * @param coll The UCollator to query.
 * @param info the version # information, the result will be filled in
 * @stable ICU 2.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getVersion" + ICU_VERSION_SUFFIX)]
            public static extern void ucol_getVersion(
                    SafeRuleBasedCollatorHandle collator,
                    out VersionInfo info);
            */
        /**
 * Gets the UCA version information for a Collator. Version is the
 * UCA version number (3.1.1, 4.0).
 * @param coll The UCollator to query.
 * @param info the version # information, the result will be filled in
 * @stable ICU 2.8
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getUCAVersion" + ICU_VERSION_SUFFIX)]
            public static extern void ucol_getUCAVersion(
                    SafeRuleBasedCollatorHandle collator,
                    out VersionInfo info);
            */
        /**
 * Merge two sort keys. The levels are merged with their corresponding counterparts
 * (primaries with primaries, secondaries with secondaries etc.). Between the values
 * from the same level a separator is inserted.
 * example (uncompressed):
 * 191B1D 01 050505 01 910505 00 and 1F2123 01 050505 01 910505 00
 * will be merged as
 * 191B1D 02 1F212301 050505 02 050505 01 910505 02 910505 00
 * This allows for concatenating of first and last names for sorting, among other things.
 * If the destination buffer is not big enough, the results are undefined.
 * If any of source lengths are zero or any of source pointers are NULL/undefined,
 * result is of size zero.
 * @param src1 pointer to the first sortkey
 * @param src1Length length of the first sortkey
 * @param src2 pointer to the second sortkey
 * @param src2Length length of the second sortkey
 * @param dest buffer to hold the result
 * @param destCapacity size of the buffer for the result
 * @return size of the result. If the buffer is big enough size is always
 *         src1Length+src2Length-1
 * @stable ICU 2.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_mergeSortkeys" + ICU_VERSION_SUFFIX)]
            public static extern Int32
                    ucol_mergeSortkeys([MarshalAs(UnmanagedType.LPArray)] byte[] src1,
                                       Int32 src1Length,
                                       [MarshalAs(UnmanagedType.LPArray)] byte[] src2,
                                       Int32 src2Length,
                                       [Out][MarshalAs(UnmanagedType.LPArray)] byte[] dest,
                                       Int32 destCapacity);
            */
        /**
 * Universal attribute setter
 * @param coll collator which attributes are to be changed
 * @param attr attribute type
 * @param value attribute value
 * @param status to indicate whether the operation went on smoothly or there were errors
 * @see UColAttribute
 * @see UColAttributeValue
 * @see ucol_getAttribute
 * @stable ICU 2.0
 */

        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_setAttribute" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ucol_setAttribute(
            RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
            CollationAttribute attr,
            CollationAttributeValue value,
            out ErrorCode status);
        /**
 * Universal attribute getter
 * @param coll collator which attributes are to be changed
 * @param attr attribute type
 * @return attribute value
 * @param status to indicate whether the operation went on smoothly or there were errors
 * @see UColAttribute
 * @see UColAttributeValue
 * @see ucol_setAttribute
 * @stable ICU 2.0
 */

        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getAttribute" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern CollationAttributeValue ucol_getAttribute(
            RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
            CollationAttribute attr,
            out ErrorCode status);
        /** Variable top
 * is a two byte primary value which causes all the codepoints with primary values that
 * are less or equal than the variable top to be shifted when alternate handling is set
 * to UCOL_SHIFTED.
 * Sets the variable top to a collation element value of a string supplied.
 * @param coll collator which variable top needs to be changed
 * @param varTop one or more (if contraction) UChars to which the variable top should be set
 * @param len length of variable top string. If -1 it is considered to be zero terminated.
 * @param status error code. If error code is set, the return value is undefined.
 *               Errors set by this function are: <br>
 *    U_CE_NOT_FOUND_ERROR if more than one character was passed and there is no such
 *    a contraction<br>
 *    U_PRIMARY_TOO_LONG_ERROR if the primary for the variable top has more than two bytes
 * @return a 32 bit value containing the value of the variable top in upper 16 bits.
 *         Lower 16 bits are undefined
 * @see ucol_getVariableTop
 * @see ucol_restoreVariableTop
 * @stable ICU 2.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_setVariableTop" + ICU_VERSION_SUFFIX)]
            public static extern UInt32 ucol_setVariableTop(
                    SafeRuleBasedCollatorHandle collator,
                    [MarshalAs(UnmanagedType.LPWStr)] string varTop,
                    Int32 len,
                    out ErrorCode status);
            */
        /**
 * Gets the variable top value of a Collator.
 * Lower 16 bits are undefined and should be ignored.
 * @param coll collator which variable top needs to be retrieved
 * @param status error code (not changed by function). If error code is set,
 *               the return value is undefined.
 * @return the variable top value of a Collator.
 * @see ucol_setVariableTop
 * @see ucol_restoreVariableTop
 * @stable ICU 2.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getVariableTop" + ICU_VERSION_SUFFIX)]
            public static extern UInt32 ucol_getVariableTop(
                    SafeRuleBasedCollatorHandle collator,
                    out ErrorCode status);
            */
        /**
 * Sets the variable top to a collation element value supplied. Variable top is
 * set to the upper 16 bits.
 * Lower 16 bits are ignored.
 * @param coll collator which variable top needs to be changed
 * @param varTop CE value, as returned by ucol_setVariableTop or ucol)getVariableTop
 * @param status error code (not changed by function)
 * @see ucol_getVariableTop
 * @see ucol_setVariableTop
 * @stable ICU 2.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_restoreVariableTop" + ICU_VERSION_SUFFIX)]
            public static extern void ucol_restoreVariableTop(
                    SafeRuleBasedCollatorHandle collator,
                    UInt32 varTop,
                    out ErrorCode status);
            */
        /**
 * Thread safe cloning operation. The result is a clone of a given collator.
 * @param coll collator to be cloned
 * @param stackBuffer user allocated space for the new clone.
 * If NULL new memory will be allocated.
 *  If buffer is not large enough, new memory will be allocated.
 *  Clients can use the U_COL_SAFECLONE_BUFFERSIZE.
 *  This will probably be enough to avoid memory allocations.
 * @param pBufferSize pointer to size of allocated space.
 *  If *pBufferSize == 0, a sufficient size for use in cloning will
 *  be returned ('pre-flighting')
 *  If *pBufferSize is not enough for a stack-based safe clone,
 *  new memory will be allocated.
 * @param status to indicate whether the operation went on smoothly or there were errors
 *    An informational status value, U_SAFECLONE_ALLOCATED_ERROR, is used if any
 * allocations were necessary.
 * @return pointer to the new clone
 * @see ucol_open
 * @see ucol_openRules
 * @see ucol_close
 * @stable ICU 2.0
 */

        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_safeClone" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern RuleBasedCollator.SafeRuleBasedCollatorHandle ucol_safeClone(
            RuleBasedCollator.SafeRuleBasedCollatorHandle collator,
            IntPtr stackBuffer,
            ref Int32 pBufferSize,
            out ErrorCode status);
        /**
 * Returns current rules. Delta defines whether full rules are returned or just the tailoring.
 * Returns number of UChars needed to store rules. If buffer is NULL or bufferLen is not enough
 * to store rules, will store up to available space.
 * @param coll collator to get the rules from
 * @param delta one of UCOL_TAILORING_ONLY, UCOL_FULL_RULES.
 * @param buffer buffer to store the result in. If NULL, you'll get no rules.
 * @param bufferLen lenght of buffer to store rules in. If less then needed you'll get only the part that fits in.
 * @return current rules
 * @stable ICU 2.0
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getRulesEx" + ICU_VERSION_SUFFIX)]
            public static extern Int32 ucol_getRulesEx(
                    SafeRuleBasedCollatorHandle collator,
                    CollationRuleOption delta,
                    [MarshalAs(UnmanagedType.LPWStr)] StringBuilder buffer,
                    Int32 bufferLen);
            */
        /**
 * gets the locale name of the collator. If the collator
 * is instantiated from the rules, then this function returns
 * NULL.
 * @param coll The UCollator for which the locale is needed
 * @param type You can choose between requested, valid and actual
 *             locale. For description see the definition of
 *             ULocDataLocaleType in uloc.h
 * @param status error code of the operation
 * @return real locale name from which the collation data comes.
 *         If the collator was instantiated from rules, returns
 *         NULL.
 * @stable ICU 2.8
 *
 */

        // Return IntPtr instead of marshalling string as unmanaged LPStr. By default, marshalling
        // creates a copy of the string and tries to de-allocate the C memory used by the
        // char*. Using IntPtr will not create a copy of any object and therefore will not
        // try to de-allocate memory. De-allocating memory from a string literal is not a
        // good Idea. To call the function use Marshal.PtrToString*(ucol_getLocaleByType(...));
        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getLocaleByType" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr
            ucol_getLocaleByType(RuleBasedCollator.SafeRuleBasedCollatorHandle collator, LocaleType type,
                                 out ErrorCode status);
        /**
 * Get an Unicode set that contains all the characters and sequences tailored in
 * this collator. The result must be disposed of by using uset_close.
 * @param coll        The UCollator for which we want to get tailored chars
 * @param status      error code of the operation
 * @return a pointer to newly created USet. Must be be disposed by using uset_close
 * @see ucol_openRules
 * @see uset_close
 * @stable ICU 2.4
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getTailoredSet" + ICU_VERSION_SUFFIX)]
            public static extern IntPtr
                    ucol_getTailoredSet(SafeRuleBasedCollatorHandle collator, out ErrorCode status);
            */

        /** Creates a binary image of a collator. This binary image can be stored and
 *  later used to instantiate a collator using ucol_openBinary.
 *  This API supports preflighting.
 *  @param coll Collator
 *  @param buffer a fill-in buffer to receive the binary image
 *  @param capacity capacity of the destination buffer
 *  @param status for catching errors
 *  @return size of the image
 *  @see ucol_openBinary
 *  @stable ICU 3.2
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_cloneBinary" + ICU_VERSION_SUFFIX)]
            public static extern Int32
                    ucol_cloneBinary(SafeRuleBasedCollatorHandle collator,
                                     [MarshalAs(UnmanagedType.U1)] out int buffer, Int32 capacity,
                                     out ErrorCode status);
            */
        /** Opens a collator from a collator binary image created using
 *  ucol_cloneBinary. Binary image used in instantiation of the
 *  collator remains owned by the user and should stay around for
 *  the lifetime of the collator. The API also takes a base collator
 *  which usualy should be UCA.
 *  @param bin binary image owned by the user and required through the
 *             lifetime of the collator
 *  @param length size of the image. If negative, the API will try to
 *                figure out the length of the image
 *  @param base fallback collator, usually UCA. Base is required to be
 *              present through the lifetime of the collator. Currently
 *              it cannot be NULL.
 *  @param status for catching errors
 *  @return newly created collator
 *  @see ucol_cloneBinary
 *  @stable ICU 3.2
 */
        /*
            [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_openBinary" + ICU_VERSION_SUFFIX)]
            public static extern SafeRuleBasedCollatorHandle
                    ucol_openBinary(byte[] bin, Int32 length,
                                    SafeRuleBasedCollatorHandle baseCollator,
                                    out ErrorCode status);
            */

        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getRulesEx" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int ucol_getRulesEx(RuleBasedCollator.SafeRuleBasedCollatorHandle coll, UColRuleOption delta, IntPtr buffer, int bufferLen);
        /// <summary>Test the rules to see if they are valid.</summary>
        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_openRules" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr ucol_openRules(string rules, int rulesLength, UColAttributeValue normalizationMode,
                                                    UColAttributeValue strength, out ParseError parseError, out ErrorCode status);
        [DllImport(ICU_I18N_LIB, EntryPoint = "ucol_getBound" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int ucol_getBound(byte[] source, int sourceLength, UColBoundMode boundType, int noOfLevels,
                                                byte[] result, int resultLength, out ErrorCode status);
        #endregion Unicode collator

        /*
			public enum CollationRuleOption
			{
				/// <summary>
				/// Retrieve tailoring only
				/// </summary>
				TailoringOnly,
				/// <summary>
				/// Retrieve UCA rules and tailoring
				/// </summary>
				FullRules
			}
		*/
        public enum LocaleType
        {
            /// <summary>
            /// This is locale the data actually comes from
            /// </summary>
            ActualLocale = 0,
            /// <summary>
            /// This is the most specific locale supported by ICU
            /// </summary>
            ValidLocale = 1,
        }

        public enum CollationAttributeValue
        {
            Default = -1, //accepted by most attributes
            Primary = 0, // primary collation strength
            Secondary = 1, // secondary collation strength
            Tertiary = 2, // tertiary collation strength
            Default_Strength = Tertiary,
            Quaternary = 3, //Quaternary collation strength
            Identical = 15, //Identical collation strength
            Off = 16,
            //Turn the feature off - works for FrenchCollation, CaseLevel, HiraganaQuaternaryMode, DecompositionMode
            On = 17,
            //Turn the feature on - works for FrenchCollation, CaseLevel, HiraganaQuaternaryMode, DecompositionMode

            Shifted = 20, // Valid for AlternateHandling. Alternate handling will be shifted
            NonIgnorable = 21, // Valid for AlternateHandling. Alternate handling will be non-ignorable
            LowerFirst = 24, // Valid for CaseFirst - lower case sorts before upper case
            UpperFirst = 25 // Valid for CaseFirst - upper case sorts before lower case
        }

        public enum CollationAttribute
        {
            FrenchCollation,
            AlternateHandling,
            CaseFirst,
            CaseLevel,
            NormalizationMode,
            DecompositionMode = NormalizationMode,
            Strength,
            HiraganaQuaternaryMode,
            NumericCollation,
            AttributeCount
        }

        public enum CollationResult
        {
            Equal = 0,
            Greater = 1,
            Less = -1
        }

        /// <summary>get the name of an ICU code point</summary>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_init" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void u_Init(out ErrorCode errorCode);
        /// <summary>Clean up the ICU files that could be locked</summary>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_cleanup" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void u_Cleanup();
        /// <summary>Return the ICU data directory</summary>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_getDataDirectory" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr u_GetDataDirectory();
        /// <summary>Set the ICU data directory</summary>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_setDataDirectory" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void u_SetDataDirectory(
            [MarshalAs(UnmanagedType.LPStr)]string directory);
        /// <summary>get the name of an ICU code point</summary>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_charName" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int u_CharName(
            int code,
            Character.UCharNameChoice nameChoice,
            IntPtr buffer,
            int bufferLength,
            out ErrorCode errorCode);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// get the numeric value for the Unicode digit
        /// </summary>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_digit" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int u_digit(
            int characterCode,
            byte radix);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// gets any of a variety of integer property values for the Unicode digit
        /// </summary>
        /// <param name="characterCode">The codepoint to look up</param>
        /// <param name="choice">The property value to look up</param>
        /// <remarks>DO NOT expose this method directly. Instead, make a specific implementation
        /// for each property needed. This not only makes it easier to use, but more importantly
        /// it prevents accidental use of the UCHAR_GENERAL_CATEGORY, which returns an
        /// enumeration that doesn't match the enumeration in FwKernel: LgGeneralCharCategory
        /// </remarks>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_getIntPropertyValue" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int u_getIntPropertyValue(
            int characterCode,
            Character.UProperty choice);
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_getUnicodeVersion" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void u_getUnicodeVersion(byte[] versionInfo);
        /// <summary>
        /// Get the general character type.
        /// </summary>
        /// <param name="characterCode"></param>
        /// <returns></returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_charType" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int u_charType(int characterCode);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        ///Get the numeric value for a Unicode code point as defined in the Unicode Character Database.
        ///A "double" return type is necessary because some numeric values are fractions, negative, or too large for int32_t.
        ///For characters without any numeric values in the Unicode Character Database,
        ///this function will return U_NO_NUMERIC_VALUE.
        ///
        ///Similar to java.lang.Character.getNumericValue(), but u_getNumericValue() also supports negative values,
        ///large values, and fractions, while Java's getNumericValue() returns values 10..35 for ASCII letters.
        ///</summary>
        ///<remarks>
        ///  See also:
        ///      U_NO_NUMERIC_VALUE
        ///  Stable:
        ///      ICU 2.2
        /// http://oss.software.ibm.com/icu/apiref/uchar_8h.html#a477
        /// </remarks>
        ///<param name="characterCode">Code point to get the numeric value for</param>
        ///<returns>Numeric value of c, or U_NO_NUMERIC_VALUE if none is defined.</returns>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_getNumericValue" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern double u_getNumericValue(
            int characterCode);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        ///	Determines whether the specified code point is a punctuation character.
        /// </summary>
        /// <param name="characterCode">the code point to be tested</param>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_ispunct" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        // Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
        // which are usually zero, especially in debug builds...but one day we will be sorry.
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool u_ispunct(
            int characterCode);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        ///	Determines whether the code point has the Bidi_Mirrored property.
        ///
        ///	This property is set for characters that are commonly used in Right-To-Left contexts
        ///	and need to be displayed with a "mirrored" glyph.
        ///
        ///	Same as java.lang.Character.isMirrored(). Same as UCHAR_BIDI_MIRRORED
        /// </summary>
        ///	<remarks>
        ///	See also:
        ///	    UCHAR_BIDI_MIRRORED
        ///
        ///	Stable:
        ///	    ICU 2.0
        ///	</remarks>
        /// <param name="characterCode">the code point to be tested</param>
        /// <returns><c>true</c> if the character has the Bidi_Mirrored property</returns>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_isMirrored" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        // Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
        // which are usually zero, especially in debug builds...but one day we will be sorry.
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool u_isMirrored(
            int characterCode);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        ///	Determines whether the specified code point is a control character. A control
        ///	character is one of the following:
        /// <list>
        ///	<item>ISO 8-bit control character (U+0000..U+001f and U+007f..U+009f)</item>
        ///	<item>U_CONTROL_CHAR (Cc)</item>
        ///	<item>U_FORMAT_CHAR (Cf)</item>
        ///	<item>U_LINE_SEPARATOR (Zl)</item>
        ///	<item>U_PARAGRAPH_SEPARATOR (Zp)</item>
        ///	</list>
        /// </summary>
        /// <param name="characterCode">the code point to be tested</param>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_iscntrl" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        // Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
        // which are usually zero, especially in debug builds...but one day we will be sorry.
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool u_iscntrl(
            int characterCode);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        ///	Determines whether the specified character is a space character.
        /// </summary>
        /// <remarks>
        ///	See also:
        ///	<list>
        ///	<item>u_isJavaSpaceChar</item>
        ///	<item>u_isWhitespace</item>
        /// <item>u_isUWhiteSpace</item>
        ///	</list>
        ///
        ///	Stable:
        ///	    ICU 2.0
        ///	</remarks>
        /// <param name="characterCode">the code point to be tested</param>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_isspace" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        // Required because ICU returns a one-byte boolean. Without this C# assumes 4, and picks up 3 more random bytes,
        // which are usually zero, especially in debug builds...but one day we will be sorry.
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool u_isspace(
            int characterCode);
        #region LCID
        /// ------------------------------------------------------------------------------------
        /// <summary>Get the ICU LCID for a locale</summary>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getLCID" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getLCID([MarshalAs(UnmanagedType.LPStr)]string localeID);
        /// ------------------------------------------------------------------------------------
        /// <summary>Gets the ICU locale ID for the specified Win32 LCID value. </summary>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getLocaleForLCID" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getLocaleForLCID(int lcid, IntPtr locale, int localeCapacity, out ErrorCode err);
        /// ------------------------------------------------------------------------------------
        /// <summary>Return the ISO 3 char value, if it exists</summary>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getISO3Country" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uloc_getISO3Country(
            [MarshalAs(UnmanagedType.LPStr)]string locale);
        /// ------------------------------------------------------------------------------------
        /// <summary>Return the ISO 3 char value, if it exists</summary>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getISO3Language" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uloc_getISO3Language(
            [MarshalAs(UnmanagedType.LPStr)]string locale);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the size of the all available locale list.
        /// </summary>
        /// <returns>the size of the locale list </returns>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_countAvailable" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_countAvailable();
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the specified locale from a list of all available locales.
        /// The return value is a pointer to an item of a locale name array. Both this array
        /// and the pointers it contains are owned by ICU and should not be deleted or written
        /// through by the caller. The locale name is terminated by a null pointer.
        /// </summary>
        /// <param name="n">n  the specific locale name index of the available locale list</param>
        /// <returns>a specified locale name of all available locales</returns>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getAvailable" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uloc_getAvailable(int n);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the language code for the specified locale.
        /// </summary>
        /// <param name="localeID">the locale to get the language code with </param>
        /// <param name="language">the language code for localeID </param>
        /// <param name="languageCapacity">the size of the language buffer to store the language
        /// code with </param>
        /// <param name="err">error information if retrieving the language code failed</param>
        /// <returns>the actual buffer size needed for the language code. If it's greater
        /// than languageCapacity, the returned language code will be truncated</returns>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getLanguage" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getLanguage(string localeID, IntPtr language,
                                                   int languageCapacity, out ErrorCode err);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the script code for the specified locale.
        /// </summary>
        /// <param name="localeID">the locale to get the script code with </param>
        /// <param name="script">the script code for localeID </param>
        /// <param name="scriptCapacity">the size of the script buffer to store the script
        /// code with </param>
        /// <param name="err">error information if retrieving the script code failed</param>
        /// <returns>the actual buffer size needed for the script code. If it's greater
        /// than scriptCapacity, the returned script code will be truncated</returns>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getScript" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getScript(string localeID, IntPtr script,
                                                 int scriptCapacity, out ErrorCode err);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the country code for the specified locale.
        /// </summary>
        /// <param name="localeID">the locale to get the country code with </param>
        /// <param name="country">the country code for localeID </param>
        /// <param name="countryCapacity">the size of the country buffer to store the country
        /// code with </param>
        /// <param name="err">error information if retrieving the country code failed</param>
        /// <returns>the actual buffer size needed for the country code. If it's greater
        /// than countryCapacity, the returned country code will be truncated</returns>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getCountry" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getCountry(string localeID, IntPtr country,
                                                  int countryCapacity, out ErrorCode err);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the variant code for the specified locale.
        /// </summary>
        /// <param name="localeID">the locale to get the variant code with </param>
        /// <param name="variant">the variant code for localeID </param>
        /// <param name="variantCapacity">the size of the variant buffer to store the variant
        /// code with </param>
        /// <param name="err">error information if retrieving the variant code failed</param>
        /// <returns>the actual buffer size needed for the variant code. If it's greater
        /// than variantCapacity, the returned variant code will be truncated</returns>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getVariant" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getVariant(string localeID, IntPtr variant,
                                                  int variantCapacity, out ErrorCode err);
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the full name suitable for display for the specified locale.
        /// </summary>
        /// <param name="localeID">the locale to get the displayable name with</param>
        /// <param name="inLocaleID">Specifies the locale to be used to display the name. In
        /// other words, if the locale's language code is "en", passing Locale::getFrench()
        /// for inLocale would result in "Anglais", while passing Locale::getGerman() for
        /// inLocale would result in "Englisch".  </param>
        /// <param name="result">the displayable name for localeID</param>
        /// <param name="maxResultSize">the size of the name buffer to store the displayable
        /// full name with</param>
        /// <param name="err">error information if retrieving the displayable name failed</param>
        /// <returns>the actual buffer size needed for the displayable name. If it's greater
        /// than variantCapacity, the returned displayable name will be truncated.</returns>
        /// ------------------------------------------------------------------------------------
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getDisplayName" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getDisplayName(string localeID, string inLocaleID,
            IntPtr result, int maxResultSize, out ErrorCode err);
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getDisplayLanguage" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getDisplayLanguage(string localeID, string displayLocaleID,
            IntPtr result, int maxResultSize, out ErrorCode err);
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getDisplayScript" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getDisplayScript(string localeID, string displayLocaleID,
            IntPtr result, int maxResultSize, out ErrorCode err);
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getDisplayCountry" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getDisplayCountry(string localeID, string displayLocaleID,
            IntPtr result, int maxResultSize, out ErrorCode err);
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getDisplayVariant" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getDisplayVariant(string localeID, string displayLocaleID,
            IntPtr result, int maxResultSize, out ErrorCode err);
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getName" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getName(string localeID, IntPtr name,
            int nameCapacity, out ErrorCode err);
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_getName" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_getBaseName(string localeID, IntPtr name,
            int nameCapacity, out ErrorCode err);
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uloc_canonicalize" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int uloc_canonicalize(string localeID, IntPtr name,
            int nameCapacity, out ErrorCode err);
        #endregion

        /// <summary>Return the lower case equivalent of the string.</summary>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_strToLower" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int u_strToLower(IntPtr dest,
            int destCapacity, string src, int srcLength,
            [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode);
        /// <summary>Return the title case equivalent of the string.</summary>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_strToTitle" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int u_strToTitle(IntPtr dest,
            int destCapacity, string src, int srcLength, IntPtr titleIter,
            [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode);
        /// <summary>Return the upper case equivalent of the string.</summary>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "u_strToUpper" + ICU_VERSION_SUFFIX,
            CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int u_strToUpper(IntPtr dest,
            int destCapacity, string src, int srcLength,
            [MarshalAs(UnmanagedType.LPStr)] string locale, out ErrorCode errorCode);
        #region normalize
        /// <summary>
        /// Normalize a string according to the given mode and options.
        /// </summary>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "unorm_normalize" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int unorm_normalize(string source, int sourceLength,
                                                  Normalizer.UNormalizationMode mode, int options,
                                                  IntPtr result, int resultLength, out ErrorCode errorCode);
        /// <summary>
        /// Check whether a string is normalized according to the given mode and options.
        /// </summary>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "unorm_isNormalized" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        // Note that ICU's UBool type is typedef to an 8-bit integer.
        public static extern byte unorm_isNormalized(string source, int sourceLength,
                                                     Normalizer.UNormalizationMode mode, out ErrorCode errorCode);
        #endregion normalize

        #region Break iterator

        /// <summary>
        /// Open a new UBreakIterator for locating text boundaries for a specified locale.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="text">The text.</param>
        /// <param name="textLength">Length of the text.</param>
        /// <param name="errorCode">The error code.</param>
        /// <returns></returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "ubrk_open" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr ubrk_open(BreakIterator.UBreakIteratorType type, string locale,
                                               string text, int textLength, out ErrorCode errorCode);
        /// <summary>
        ///(unsafe version) Open a new UBreakIterator for locating text boundaries for a specified locale.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="text">The text.</param>
        /// <param name="textLength">Length of the text.</param>
        /// <param name="errorCode">The error code.</param>
        /// <returns></returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "ubrk_open" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static unsafe extern IntPtr ubrk_open_unsafe(BreakIterator.UBreakIteratorType type, string locale,
               char* startChar, int length, out ErrorCode errorCode);
        /// <summary>
        /// Close a UBreakIterator.
        /// </summary>
        /// <param name="bi">The break iterator.</param>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "ubrk_close" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void ubrk_close(IntPtr bi);
        /// <summary>
        /// Determine the index of the first character in the text being scanned.
        /// </summary>
        /// <param name="bi">The break iterator.</param>
        /// <returns></returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "ubrk_first" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int ubrk_first(IntPtr bi);
        /// <summary>
        /// Determine the text boundary following the current text boundary.
        /// </summary>
        /// <param name="bi">The break iterator.</param>
        /// <returns></returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "ubrk_next" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int ubrk_next(IntPtr bi);
        /// <summary>
        /// Return the status from the break rule that determined the most recently returned break position.
        /// </summary>
        /// <param name="bi">The break iterator.</param>
        /// <returns></returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "ubrk_getRuleStatus" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int ubrk_getRuleStatus(IntPtr bi);
        #endregion Break iterator

        #region Unicode set

        /// <summary>
        /// Disposes of the storage used by Unicode set.  This function should be called exactly once for objects returned by uset_open()
        /// </summary>
        /// <param name="set">Unicode set to dispose of </param>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uset_close" + ICU_VERSION_SUFFIX, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uset_close(IntPtr set);
        /// <summary>
        /// Creates a Unicode set that contains the range of characters start..end, inclusive.
        /// If start > end then an empty set is created (same as using uset_openEmpty()).
        /// </summary>
        /// <param name="start">First character of the range, inclusive</param>
        /// <param name="end">Last character of the range, inclusive</param>
        /// <returns>Unicode set of characters.  The caller must call uset_close() on it when done</returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uset_open" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr uset_open(char start, char end);
        /// <summary>
        /// Creates a set from the given pattern.
        /// </summary>
        /// <param name="pattern">A string specifying what characters are in the set</param>
        /// <param name="patternLength">Length of the pattern, or -1 if null terminated</param>
        /// <param name="status">The error code</param>
        /// <returns>Unicode set</returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uset_openPattern" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr uset_openPattern(string pattern, int patternLength, ref ErrorCode status);
        /// <summary>
        /// Adds the given character to the given Unicode set.  After this call, uset_contains(set, c) will return TRUE.  A frozen set will not be modified.
        /// </summary>
        /// <param name="set">The object to which to add the character</param>
        /// <param name="c">The character to add</param>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uset_add" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void uset_add(IntPtr set, char c);
        /// <summary>
        /// Returns a string representation of this set.  If the result of calling this function is 
        /// passed to a uset_openPattern(), it will produce another set that is equal to this one.
        /// </summary>
        /// <param name="set">The Unicode set</param>
        /// <param name="result">The string to receive the rules, may be NULL</param>
        /// <param name="resultCapacity">The capacity of result, may be 0 if result is NULL</param>
        /// <param name="escapeUnprintable">if TRUE then convert unprintable characters to their hex escape representations,
        /// \uxxxx or \Uxxxxxxxx. Unprintable characters are those other than U+000A, U+0020..U+007E.</param>
        /// <param name="status">Error code</param>
        /// <returns>Length of string, possibly larger than resultCapacity</returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uset_toPattern" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int uset_toPattern(IntPtr set, IntPtr result, int resultCapacity,
            bool escapeUnprintable, ref ErrorCode status);
        /// <summary>
        /// Adds the given string to the given Unicode set
        /// </summary>
        /// <param name="set">The Unicode set to which to add the string</param>
        /// <param name="str">The string to add</param>
        /// <param name="strLen">The length of the string or -1 if null</param>
        /// 
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uset_addString" + ICU_VERSION_SUFFIX,
                CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void uset_addString(IntPtr set, string str, int strLen);
        /// <summary>
        /// Returns an item of this Unicode set.  An item is either a range of characters or a single multicharacter string.
        /// </summary>
        /// <param name="set">The Unicode set</param>
        /// <param name="itemIndex">A non-negative integer in the range 0..uset_getItemCount(set)-1</param>
        /// <param name="start">Pointer to variable to receive first character in range, inclusive</param>
        /// <param name="end">POinter to variable to receive the last character in range, inclusive</param>
        /// <param name="str">Buffer to receive the string, may be NULL</param>
        /// <param name="strCapacity">Capcacity of str, or 0 if str is NULL</param>
        /// <param name="ec">Error Code</param>
        /// <returns>The length of the string (>=2), or 0 if the item is a range, in which case it is the range *start..*end, or -1 if itemIndex is out of range</returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uset_getItem" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int uset_getItem(IntPtr set, int itemIndex, out int start, out int end, IntPtr str,
            int strCapacity, ref ErrorCode ec);
        /// <summary>
        /// Returns the number of items in this set.  An item is either a range of characters or a single multicharacter string
        /// </summary>
        /// <param name="set">The Unicode set</param>
        /// <returns>A non-negative integer counting the character ranges and/or strings contained in the set</returns>
        [DllImport(ICU_COMMON_LIB, EntryPoint = "uset_getItemCount" + ICU_VERSION_SUFFIX,
                   CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int uset_getItemCount(IntPtr set);
        #endregion Unicode set

        [DllImport(ICU_COMMON_LIB, EntryPoint = "udata_setCommonData" + ICU_VERSION_SUFFIX,
        CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void udata_setCommonData(IntPtr data, out ErrorCode ec);
    }
}
