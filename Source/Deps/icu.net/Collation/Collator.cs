// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
namespace Icu.Collation
{
    public abstract class Collator : IComparer<string>
    {
        public abstract CollationStrength Strength { get; set; }

        public abstract NormalizationMode NormalizationMode { get; set; }

        public abstract FrenchCollation FrenchCollation { get; set; }

        public abstract CaseLevel CaseLevel { get; set; }

        public abstract HiraganaQuaternary HiraganaQuaternary { get; set; }

        public abstract NumericCollation NumericCollation { get; set; }

        public abstract CaseFirst CaseFirst { get; set; }

        public abstract AlternateHandling AlternateHandling { get; set; }

#if !PCL
        public abstract SortKey GetSortKey(string source);
#endif
        public abstract int Compare(string source, string target);
        public abstract object Clone();
        public enum Fallback
        {
            NoFallback,
            FallbackAllowed
        }

        public static Collator Create()
        {
            return Create(CultureInfo.CurrentCulture);
        }

        public static Collator Create(string localeId)
        {
            return Create(localeId, Fallback.NoFallback);
        }

        public static Collator Create(string localeId, Fallback fallback)
        {
            if (localeId == null)
            {
                throw new ArgumentNullException();
            }
            return RuleBasedCollator.Create(localeId, fallback);
        }

        public static Collator Create(CultureInfo cultureInfo)
        {
#if PCL
            return Create(cultureInfo.Name, Fallback.NoFallback);
#else
            return Create(cultureInfo, Fallback.NoFallback);
#endif
        }
#if !PCL
        public static Collator Create(CultureInfo cultureInfo, Fallback fallback)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException();
            }
            return Create(cultureInfo.IetfLanguageTag, fallback);
        }
        static public SortKey CreateSortKey(string originalString, byte[] keyData)
        {
            if (keyData == null)
            {
                throw new ArgumentNullException("keyData");
            }
            return CreateSortKey(originalString, keyData, keyData.Length);
        }
        static public SortKey CreateSortKey(string originalString, byte[] keyData, int keyDataLength)
        {
            if (originalString == null)
            {
                throw new ArgumentNullException("originalString");
            }
            if (keyData == null)
            {
                throw new ArgumentNullException("keyData");
            }
            if (0 > keyDataLength || keyDataLength > keyData.Length)
            {
                throw new ArgumentOutOfRangeException("keyDataLength");
            }


            SortKey sortKey = CultureInfo.InvariantCulture.CompareInfo.GetSortKey(string.Empty);
            SetInternalOriginalStringField(sortKey, originalString);
            SetInternalKeyDataField(sortKey, keyData, keyDataLength);
            return sortKey;
        }

        private static void SetInternalKeyDataField(SortKey sortKey, byte[] keyData, int keyDataLength)
        {
            byte[] keyDataCopy = new byte[keyDataLength];
            Array.Copy(keyData, keyDataCopy, keyDataLength);
            string propertyName = "SortKey.KeyData";
            string monoInternalFieldName = "key";
            string netInternalFieldName = "m_KeyData";
            SetInternalFieldForPublicProperty(sortKey,
                                              propertyName,
                                              netInternalFieldName,
                                              monoInternalFieldName,
                                              keyDataCopy);
        }

        private static void SetInternalOriginalStringField(SortKey sortKey, string originalString)
        {
            string propertyName = "SortKey.OriginalString";
            string monoInternalFieldName = "source";
            string netInternalFieldName = "m_String";
            SetInternalFieldForPublicProperty(sortKey,
                                              propertyName,
                                              netInternalFieldName,
                                              monoInternalFieldName,
                                              originalString);
        }
#endif
#if PCL
        static FieldInfo GetField(Type type, string fieldname, bool isStatic, bool isPublic)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            foreach (FieldInfo f in typeInfo.DeclaredFields)
            {
                if (f.IsStatic == isStatic &&
                    f.IsPublic == isPublic &&
                    f.Name == fieldname)
                {
                    return f;
                }
            }
            return null;
        }
#endif
        private static void SetInternalFieldForPublicProperty<T, P>(
            T instance,
            string propertyName,
            string netInternalFieldName,
            string monoInternalFieldName,
            P value)
        {
            Type type = instance.GetType();
            FieldInfo fieldInfo;
            if (IsRunningOnMono())
            {
#if PCL
                fieldInfo = GetField(type, monoInternalFieldName, false, false);
#else
                fieldInfo = type.GetField(monoInternalFieldName,
                                          BindingFlags.Instance
                 | BindingFlags.NonPublic);
#endif
            }
            else //Is Running On .Net
            {
#if PCL
                fieldInfo = GetField(type, monoInternalFieldName, false, false);
#else
                fieldInfo = type.GetField(netInternalFieldName,
                                          BindingFlags.Instance
                    | BindingFlags.NonPublic);
#endif
            }

            //Debug.Assert(fieldInfo != null,
            //             "Unsupported runtime",
            //             "Could not figure out an internal field for" + propertyName);

            if (fieldInfo == null)
            {
                throw new NotImplementedException("Not implemented for this runtime");
            }

            fieldInfo.SetValue(instance, value);
        }

        private static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        /// <summary>
        /// Simple class to allow passing collation error info back to the caller of CheckRules.
        /// </summary>
        public class CollationRuleErrorInfo
        {
            /// <summary>Line number (1-based) containing the error</summary>
            public int Line;
            /// <summary>Character offset (1-based) on Line where the error was detected</summary>
            public int Offset;
            /// <summary>Characters preceding the the error</summary>
            public String PreContext;
            /// <summary>Characters following the the error</summary>
            public String PostContext;
        }

        // REVIEW: We might want to integrate the methods below in a better way.

        /// <summary>
        /// Test collation rules and return an object with error information if it fails.
        /// </summary>
        /// <param name="rules">String containing the collation rules to check</param>
        /// <returns>A CollationRuleErrorInfo object with error information; or <c>null</c> if
        /// no errors are found.</returns>
        public static CollationRuleErrorInfo CheckRules(string rules)
        {
            if (rules == null)
                return null;
            ErrorCode err;
            ParseError parseError;
            IntPtr col = NativeMethods.ucol_openRules(rules, rules.Length, UColAttributeValue.UCOL_DEFAULT,
                UColAttributeValue.UCOL_DEFAULT_STRENGTH, out parseError, out err);
            try
            {
                if (err == ErrorCode.NoErrors)
                    return null;
                return new CollationRuleErrorInfo
                {
                    Line = parseError.Line + 1,
                    Offset = parseError.Offset + 1,
                    PreContext = parseError.PreContext,
                    PostContext = parseError.PostContext
                };
            }
            finally
            {
                NativeMethods.ucol_close(col);
            }
        }

        /// <summary>
        /// Gets the collation rules for the specified locale.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <returns></returns>
        public static string GetCollationRules(string locale)
        {
            string sortRules = null;
            ErrorCode err;
            RuleBasedCollator.SafeRuleBasedCollatorHandle coll = NativeMethods.ucol_open(locale, out err);
            if (!coll.IsInvalid && err == ErrorCode.NoErrors)
            {
                const int len = 1000;
                IntPtr buffer = Marshal.AllocCoTaskMem(len * 2);
                try
                {
                    int actualLen = NativeMethods.ucol_getRulesEx(coll, UColRuleOption.UCOL_TAILORING_ONLY, buffer, len);
                    if (actualLen > len)
                    {
                        Marshal.FreeCoTaskMem(buffer);
                        buffer = Marshal.AllocCoTaskMem(actualLen * 2);
                        NativeMethods.ucol_getRulesEx(coll, UColRuleOption.UCOL_TAILORING_ONLY, buffer, actualLen);
                    }
                    sortRules = Marshal.PtrToStringUni(buffer);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(buffer);
                }
            }
            return sortRules;
        }

        /// <summary>
        /// Produces a bound for a given sort key.
        /// </summary>
        /// <param name="sortKey">The sort key.</param>
        /// <param name="boundType">Type of the bound.</param>
        /// <param name="result">The result.</param>
        public static void GetSortKeyBound(byte[] sortKey, UColBoundMode boundType, ref byte[] result)
        {
            ErrorCode err;
            int size = NativeMethods.ucol_getBound(sortKey, sortKey.Length, boundType, 1, result, result.Length, out err);
            if (err > 0 && err != ErrorCode.BUFFER_OVERFLOW_ERROR)
                throw new Exception("Collator.GetSortKeyBound() failed with code " + err);
            if (size > result.Length)
            {
                result = new byte[size + 1];
                NativeMethods.ucol_getBound(sortKey, sortKey.Length, boundType, 1, result, result.Length, out err);
                if (err != ErrorCode.NoErrors)
                    throw new Exception("Collator.GetSortKeyBound() failed with code " + err);
            }
        }
    }
}
