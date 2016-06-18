// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
namespace Icu.Collation
{
    /// <summary>
    /// Collation constants
    /// </summary>
    public enum UColAttributeValue
    {
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_DEFAULT = -1,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_PRIMARY = 0,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_SECONDARY = 1,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_TERTIARY = 2,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_DEFAULT_STRENGTH = UCOL_TERTIARY,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_CE_STRENGTH_LIMIT,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_QUATERNARY = 3,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_IDENTICAL = 15,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_STRENGTH_LIMIT,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_OFF = 16,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_ON = 17,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_SHIFTED = 20,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_NON_IGNORABLE = 21,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_LOWER_FIRST = 24,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_UPPER_FIRST = 25,
        /// <summary>http://oss.software.ibm.com/icu/apiref/uchar_8h-source.html</summary>
        UCOL_ATTRIBUTE_VALUE_COUNT
    }
}
