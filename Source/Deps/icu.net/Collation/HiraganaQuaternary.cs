// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

namespace Icu.Collation
{
    /// <summary>
    /// When turned on, this attribute positions Hiragana before all
    /// non-ignorables on quaternary level This is a sneaky way to produce JIS
    /// sort order
    /// </summary>
    public enum HiraganaQuaternary
    {
        Default = -1,
        Off = 16,
        On = 17
    }
}
