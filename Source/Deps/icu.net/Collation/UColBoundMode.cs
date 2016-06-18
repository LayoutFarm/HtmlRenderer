// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
namespace Icu.Collation
{
    /// <summary>
    /// The sort key bound mode
    /// </summary>
    public enum UColBoundMode
    {
        /// <summary>
        /// lower bound
        /// </summary>
        UCOL_BOUND_LOWER = 0,
        /// <summary>
        /// upper bound that will match strings of exact size
        /// </summary>
        UCOL_BOUND_UPPER,
        /// <summary>
        /// upper bound that will match all strings that have the same initial substring as the given string
        /// </summary>
        UCOL_BOUND_UPPER_LONG
    }
}
