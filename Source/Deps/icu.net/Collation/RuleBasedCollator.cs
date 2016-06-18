// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if !PCL
using System.Globalization;
using Icu;
using System.Runtime.ConstrainedExecution;
#endif

namespace Icu.Collation
{
    public sealed class RuleBasedCollator : Collator
    {
        internal sealed class SafeRuleBasedCollatorHandle : SafeHandle
        {
            public SafeRuleBasedCollatorHandle() :
                    base(IntPtr.Zero, true)
            { }

            ///<summary>
            ///When overridden in a derived class, executes the code required to free the handle.
            ///</summary>
            ///<returns>
            ///true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
            ///</returns>
#if !PCL
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
            protected override bool ReleaseHandle()
            {
                NativeMethods.ucol_close(handle);
                handle = IntPtr.Zero;
                return true;
            }

            ///<summary>
            ///When overridden in a derived class, gets a value indicating whether the handle value is invalid.
            ///</summary>
            ///<returns>
            ///true if the handle is valid; otherwise, false.
            ///</returns>
            public override bool IsInvalid
            {
                get { return (handle == IntPtr.Zero); }
            }
        }

        private SafeRuleBasedCollatorHandle collatorHandle;
        readonly ParseError parseError = new ParseError();
        /// <summary>
        /// RuleBasedCollator constructor.
        /// This takes the table rules and builds a collation table out of them.
        /// </summary>
        /// <param name="rules">the collation rules to build the collation table from</param>
        public RuleBasedCollator(string rules) :
                this(rules, CollationStrength.Default)
        { }

        /// <summary>
        /// RuleBasedCollator constructor.
        /// This takes the table rules and builds a collation table out of them.
        /// </summary>
        /// <param name="rules">the collation rules to build the collation table from</param>
        /// <param name="collationStrength">the collation strength to use</param>
        public RuleBasedCollator(string rules, CollationStrength collationStrength)
            : this(rules, NormalizationMode.Default, collationStrength)
        { }

        /// <summary>
        /// RuleBasedCollator constructor.
        /// This takes the table rules and builds a collation table out of them.
        /// </summary>
        /// <param name="rules">the collation rules to build the collation table from</param>
        /// <param name="normalizationMode">the normalization mode to use</param>
        /// <param name="collationStrength">the collation strength to use</param>
        public RuleBasedCollator(string rules,
                                 NormalizationMode normalizationMode,
                                 CollationStrength collationStrength)
        {
            ErrorCode status;
            collatorHandle = NativeMethods.ucol_openRules(rules,
                                                          rules.Length,
                                                          normalizationMode,
                                                          collationStrength,
                                                          ref parseError,
                                                          out status);
            ExceptionFromErrorCode.ThrowIfError(status, parseError.ToString(rules));
        }

        /// <summary>The collation strength.
        /// The usual strength for most locales (except Japanese) is tertiary.
        /// Quaternary strength is useful when combined with shifted setting
        /// for alternate handling attribute and for JIS x 4061 collation,
        /// when it is used to distinguish between Katakana and Hiragana
        /// (this is achieved by setting the HiraganaQuaternary mode to on.
        /// Otherwise, quaternary level is affected only by the number of
        /// non ignorable code points in the string.
        /// </summary>
        public override CollationStrength Strength
        {
            get { return (CollationStrength)GetAttribute(NativeMethods.CollationAttribute.Strength); }
            set
            {
                SetAttribute(NativeMethods.CollationAttribute.Strength,
                             (NativeMethods.CollationAttributeValue)value);
            }
        }

        /// <summary>
        /// Controls whether the normalization check and necessary normalizations
        /// are performed.
        /// </summary>
        public override NormalizationMode NormalizationMode
        {
            get { return (NormalizationMode)GetAttribute(NativeMethods.CollationAttribute.NormalizationMode); }
            set
            {
                SetAttribute(NativeMethods.CollationAttribute.NormalizationMode,
                             (NativeMethods.CollationAttributeValue)value);
            }
        }

        /// <summary>
        /// Direction of secondary weights - Necessary for French to make secondary weights be considered from back to front.
        /// </summary>
        public override FrenchCollation FrenchCollation
        {
            get { return (FrenchCollation)GetAttribute(NativeMethods.CollationAttribute.FrenchCollation); }
            set
            {
                SetAttribute(NativeMethods.CollationAttribute.FrenchCollation,
                             (NativeMethods.CollationAttributeValue)value);
            }
        }

        /// <summary>
        /// Controls whether an extra case level (positioned before the third
        /// level) is generated or not. Contents of the case level are affected by
        /// the value of CaseFirst attribute. A simple way to ignore
        /// accent differences in a string is to set the strength to Primary
        /// and enable case level.
        /// </summary>
        public override CaseLevel CaseLevel
        {
            get { return (CaseLevel)GetAttribute(NativeMethods.CollationAttribute.CaseLevel); }
            set
            {
                SetAttribute(NativeMethods.CollationAttribute.CaseLevel,
                             (NativeMethods.CollationAttributeValue)value);
            }
        }

        /// <summary>
        /// When turned on, this attribute positions Hiragana before all
        /// non-ignorables on quaternary level This is a sneaky way to produce JIS
        /// sort order
        /// </summary>
        public override HiraganaQuaternary HiraganaQuaternary
        {
            get { return (HiraganaQuaternary)GetAttribute(NativeMethods.CollationAttribute.HiraganaQuaternaryMode); }
            set
            {
                SetAttribute(NativeMethods.CollationAttribute.HiraganaQuaternaryMode,
                             (NativeMethods.CollationAttributeValue)value);
            }
        }

        /// <summary>
        /// When turned on, this attribute generates a collation key
        /// for the numeric value of substrings of digits.
        /// This is a way to get '100' to sort AFTER '2'.
        /// </summary>
        public override NumericCollation NumericCollation
        {
            get { return (NumericCollation)GetAttribute(NativeMethods.CollationAttribute.NumericCollation); }
            set
            {
                SetAttribute(NativeMethods.CollationAttribute.NumericCollation,
                             (NativeMethods.CollationAttributeValue)value);
            }
        }

        /// <summary>
        /// Controls the ordering of upper and lower case letters.
        /// </summary>
        public override CaseFirst CaseFirst
        {
            get { return (CaseFirst)GetAttribute(NativeMethods.CollationAttribute.CaseFirst); }
            set
            {
                SetAttribute(NativeMethods.CollationAttribute.CaseFirst,
                             (NativeMethods.CollationAttributeValue)value);
            }
        }

        /// <summary>
        /// Attribute for handling variable elements.
        /// </summary>
        public override AlternateHandling AlternateHandling
        {
            get { return (AlternateHandling)GetAttribute(NativeMethods.CollationAttribute.AlternateHandling); }
            set
            {
                SetAttribute(NativeMethods.CollationAttribute.AlternateHandling,
                             (NativeMethods.CollationAttributeValue)value);
            }
        }

        private byte[] keyData = new byte[1024];
#if !PCL
        /// <summary>
        /// Get a sort key for the argument string.
        /// Sort keys may be compared using SortKey.Compare
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public override SortKey GetSortKey(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            int actualLength;
            for (;;)
            {
                actualLength = NativeMethods.ucol_getSortKey(collatorHandle,
                                                             source,
                                                             source.Length,
                                                             keyData,
                                                             keyData.Length);
                if (actualLength > keyData.Length)
                {
                    keyData = new byte[keyData.Length * 2];
                    continue;
                }
                break;
            }
            return CreateSortKey(source, keyData, actualLength);
        }
#endif
        private NativeMethods.CollationAttributeValue GetAttribute(NativeMethods.CollationAttribute attr)
        {
            ErrorCode e;
            NativeMethods.CollationAttributeValue value = NativeMethods.ucol_getAttribute(collatorHandle, attr, out e);
            ExceptionFromErrorCode.ThrowIfError(e);
            return value;
        }

        private void SetAttribute(NativeMethods.CollationAttribute attr, NativeMethods.CollationAttributeValue value)
        {
            ErrorCode e;
            NativeMethods.ucol_setAttribute(collatorHandle, attr, value, out e);
            ExceptionFromErrorCode.ThrowIfError(e);
        }

        public static IList<string> GetAvailableCollationLocales()
        {
            List<string> locales = new List<string>();
            // The ucol_openAvailableLocales call failes when there are no locales available, so check first.
            if (NativeMethods.ucol_countAvailable() == 0)
            {
                return locales;
            }
            ErrorCode ec;
            SafeEnumeratorHandle en = NativeMethods.ucol_openAvailableLocales(out ec);
            ExceptionFromErrorCode.ThrowIfError(ec);
            try
            {
                string str = en.Next();
                while (str != null)
                {
                    locales.Add(str);
                    str = en.Next();
                }
            }
            finally
            {
#if !PCL
                en.Close();
#endif
            }
            return locales;
        }

        internal sealed class SafeEnumeratorHandle : SafeHandle
        {
            public SafeEnumeratorHandle()
                :
                    base(IntPtr.Zero, true)
            { }

            ///<summary>
            ///When overridden in a derived class, executes the code required to free the handle.
            ///</summary>
            ///<returns>
            ///true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
            ///</returns>
#if !PCL
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
            protected override bool ReleaseHandle()
            {
                NativeMethods.uenum_close(handle);
                handle = IntPtr.Zero;
                return true;
            }

            ///<summary>
            ///When overridden in a derived class, gets a value indicating whether the handle value is invalid.
            ///</summary>
            ///<returns>
            ///true if the handle is valid; otherwise, false.
            ///</returns>
            public override bool IsInvalid
            {
                get { return (handle == IntPtr.Zero); }
            }

            public string Next()
            {
                ErrorCode e;
                int length;
                IntPtr str = NativeMethods.uenum_unext(this, out length, out e);
                if (str == IntPtr.Zero)
                {
                    return null;
                }
                string result = Marshal.PtrToStringUni(str, length);
                ExceptionFromErrorCode.ThrowIfError(e);
                return result;
            }
        }

        #region ICloneable Members

        ///<summary>
        ///Creates a new object that is a copy of the current instance.
        ///</summary>
        ///
        ///<returns>
        ///A new object that is a copy of this instance.
        ///</returns>
        public override object Clone()
        {
            RuleBasedCollator copy = new RuleBasedCollator();
            ErrorCode status;
            int buffersize = 512;
            copy.collatorHandle = NativeMethods.ucol_safeClone(collatorHandle,
                                                               IntPtr.Zero,
                                                               ref buffersize,
                                                               out status);
            ExceptionFromErrorCode.ThrowIfError(status);
            return copy;
        }

        public static new Collator Create(string localeId)
        {
            return Create(localeId, Fallback.NoFallback);
        }

        public static new Collator Create(string localeId, Fallback fallback)
        {
            RuleBasedCollator instance = new RuleBasedCollator();
            ErrorCode status;
            instance.collatorHandle = NativeMethods.ucol_open(localeId, out status);
            if (status == ErrorCode.USING_FALLBACK_WARNING && fallback == Fallback.NoFallback)
            {
                throw new ArgumentException("Could only create Collator by falling back to '" +
                                            instance.Id +
                                            "'. You can use the fallback option to create this.");
            }
            if (status == ErrorCode.INTERNAL_PROGRAM_ERROR && fallback == Fallback.FallbackAllowed)
            {
                instance = new RuleBasedCollator(string.Empty); // fallback to UCA
            }
            else
            {
                try
                {
                    ExceptionFromErrorCode.ThrowIfError(status);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(
                            "Unable to create a collator using the given localeId.\nThis is likely because the ICU data file was created without collation rules for this locale. You can provide the rules yourself or replace the data dll.",
                            e);
                }
            }
            return instance;
        }

        private RuleBasedCollator() { }

        private string Id
        {
            get
            {
                ErrorCode status;
                if (collatorHandle.IsInvalid)
                    return string.Empty;
                // See NativeMethods.ucol_getLocaleByType for marshal information.
                string result = Marshal.PtrToStringAnsi(NativeMethods.ucol_getLocaleByType(
                    collatorHandle, NativeMethods.LocaleType.ValidLocale, out status));
                if (status != ErrorCode.NoErrors)
                {
                    return string.Empty;
                }
                return result;
            }
        }

        #endregion

        #region IComparer<string> Members
        /// <summary>
        /// Compares two strings based on the rules of this RuleBasedCollator
        /// </summary>
        /// <param name="string1">The first string to compare</param>
        /// <param name="string2">The second string to compare</param>
        /// <returns></returns>
        /// <remarks>Comparing a null reference is allowed and does not generate an exception.
        /// A null reference is considered to be less than any reference that is not null.</remarks>
        public override int Compare(string string1, string string2)
        {
            if (string1 == null)
            {
                if (string2 == null)
                {
                    return 0;
                }
                return -1;
            }
            if (string2 == null)
            {
                return 1;
            }
            return (int)NativeMethods.ucol_strcoll(collatorHandle,
                                                    string1,
                                                    string1.Length,
                                                    string2,
                                                    string2.Length);
        }

        #endregion
    }
}
