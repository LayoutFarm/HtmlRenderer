//2014 ,BSD, WinterCore

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Parse
{
    partial class HtmlParser
    {
        /// <summary>
        /// Represents sub-string of a full string starting at specific location with a specific length.
        /// </summary>
        struct SubString
        {
            /// <summary>
            /// text source
            /// </summary>
            readonly TextSnapshot _source;
            /// <summary>
            /// the start index of the sub-string
            /// </summary>
            readonly int _startIdx;
            /// <summary>
            /// the length of the sub-string starting at <see cref="_startIdx"/>
            /// </summary>
            readonly int _length;

            bool hasEvaluteWhitespace;
            bool isWhiteSpaceCacheValue;

            public static readonly SubString EmptyTextSpan = new SubString();


            /// <summary>
            /// Init sub-string that is the full string.
            /// </summary>
            /// <param name="fullString">the full string that this sub-string is part of</param>
            public SubString(TextSnapshot snapSource)
            {
                ArgChecker.AssertArgNotNull(snapSource, "fullString");
                _source = snapSource;
                _startIdx = 0;
                _length = snapSource.Length;
                hasEvaluteWhitespace = isWhiteSpaceCacheValue = false;
            }


            /// <summary>
            /// Init.
            /// </summary>
            /// <param name="fullString">the full string that this sub-string is part of</param>
            /// <param name="startIdx">the start index of the sub-string</param>
            /// <param name="length">the length of the sub-string starting at <paramref name="startIdx"/></param>
            /// <exception cref="ArgumentNullException"><paramref name="fullString"/> is null</exception>
            public SubString(TextSnapshot snapSource, int startIdx, int length)
            {
                ArgChecker.AssertArgNotNull(snapSource, "fullString");
                if (startIdx < 0 || startIdx >= snapSource.Length)
                    throw new ArgumentOutOfRangeException("startIdx", "Must within fullString boundries");
                if (length < 0 || startIdx + length > snapSource.Length)
                    throw new ArgumentOutOfRangeException("length", "Must within fullString boundries");

                _source = snapSource;
                _startIdx = startIdx;
                _length = length;
                hasEvaluteWhitespace = isWhiteSpaceCacheValue = false;
            }

            public bool IsNull
            {
                get
                {
                    return this._source == null;
                }
            }

            /// <summary>
            /// the length of the sub-string starting at _startIdx
            /// </summary>
            public int Length
            {
                get { return _length; }
            }

            /// <summary>
            /// Get string char at specific index.
            /// </summary>
            /// <param name="idx">the idx to get the char at</param>
            /// <returns>char at index</returns>
            public char this[int idx]
            {
                get
                {
                    if (idx < 0 || idx > _length)
                    {
                        throw new ArgumentOutOfRangeException("idx", "must be within the string range");
                    }
                    return _source[_startIdx + idx];
                }
            }
            /// <summary>
            /// Is the sub-string is empty string.
            /// </summary>
            /// <returns>true - empty string, false - otherwise</returns>
            public bool IsEmpty()
            {
                return _length < 1;
            }
            void EvaluateWhitespace()
            {
                //init 
                this.hasEvaluteWhitespace = true;
                this.isWhiteSpaceCacheValue = true;
                for (int i = 0; i < _length; i++)
                {
                    if (!char.IsWhiteSpace(_source[_startIdx + i]))
                    {
                        this.isWhiteSpaceCacheValue = false;
                        break;
                    }
                }
            }
            /// <summary>
            /// Is the sub-string is empty string or contains only whitespaces.
            /// </summary>
            /// <returns>true - empty or whitespace string, false - otherwise</returns>
            public bool IsEmptyOrWhitespace()
            {
                if (!hasEvaluteWhitespace)
                {
                    EvaluateWhitespace();
                }
                return this.isWhiteSpaceCacheValue;
            }

            /// <summary>
            /// Is the sub-string contains only whitespaces (at least one).
            /// </summary>
            /// <returns>true - empty or whitespace string, false - otherwise</returns>
            public bool IsWhitespace()
            {
                //----------------------
                if (_length < 1)
                {
                    return false;
                }
                //----------------------
                if (!hasEvaluteWhitespace)
                {
                    EvaluateWhitespace();
                }
                return this.isWhiteSpaceCacheValue;

            }

            /// <summary>
            /// Get a string of the sub-string.<br/>
            /// This will create a new string object!
            /// </summary>
            /// <returns>new string that is the sub-string represented by this instance</returns>
            public string CutSubstring()
            {
                return _length > 0 ? _source.Substring(_startIdx, _length) : string.Empty;
            }

            /// <summary>
            /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length. 
            /// </summary>
            /// <param name="startIdx">The zero-based starting character position of a substring in this instance.</param>
            /// <param name="length">The number of characters in the substring. </param>
            /// <returns>A String equivalent to the substring of length length that begins at startIndex in this instance, or 
            /// Empty if startIndex is equal to the length of this instance and length is zero. </returns>
            public string Substring(int startIdx, int length)
            {
                if (startIdx < 0 || startIdx > _length)
                    throw new ArgumentOutOfRangeException("startIdx");
                if (length > _length)
                    throw new ArgumentOutOfRangeException("length");
                if (startIdx + length > _length)
                    throw new ArgumentOutOfRangeException("length");

                return _source.Substring(_startIdx + startIdx, length);
            }

            public override string ToString()
            {
                return string.Format("Sub-string: {0}", _length > 0 ? _source.Substring(_startIdx, _length) : string.Empty);
            }
        }
    }
}
