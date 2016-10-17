//2015 MIT, WinterDev
// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

using System;
using System.Collections.Generic;
namespace Icu
{
    public class BreakIterator
    {
        /// <summary>
        /// The possible types of text boundaries.
        /// </summary>
        public enum UBreakIteratorType
        {
            /// <summary>Character breaks.</summary>
            CHARACTER = 0,
            /// <summary>Word breaks.</summary>
            WORD,
            /// <summary>Line breaks.</summary>
            LINE,
            /// <summary>Sentence breaks.</summary>
            SENTENCE,
            // <summary>Title Case breaks.</summary>
            // obsolete. Use WORD instead.
            //TITLE
        }

        public enum UWordBreak
        {
            /// <summary>
            /// Tag value for "words" that do not fit into any of other categories.
            /// Includes spaces and most punctuation.
            /// </summary>
            NONE = 0,
            /// <summary>
            /// Upper bound for tags for uncategorized words.
            /// </summary>
            NONE_LIMIT = 100,
            NUMBER = 100,
            NUMBER_LIMIT = 200,
            LETTER = 200,
            LETTER_LIMIT = 300,
            KANA = 300,
            KANA_LIMIT = 400,
            IDEO = 400,
            IDEO_LIMIT = 500,
        }

        public enum ULineBreakTag
        {
            SOFT = 0,
            SOFT_LIMIT = 100,
            HARD = 100,
            HARD_LIMIT = 200,
        }

        public enum USentenceBreakTag
        {
            TERM = 0,
            TERM_LIMIT = 100,
            SEP = 100,
            SEP_LIMIT = 200,
        }

        class InMemoryIcuDataHolder : IDisposable
        {
            IntPtr unmanagedICUMemData;
            public InMemoryIcuDataHolder()
            {
                byte[] inMemoryICUData = System.IO.File.ReadAllBytes(@"icudtl.dat");
                unmanagedICUMemData = System.Runtime.InteropServices.Marshal.AllocHGlobal(inMemoryICUData.Length);
                System.Runtime.InteropServices.Marshal.Copy(inMemoryICUData, 0, unmanagedICUMemData, inMemoryICUData.Length);
            }
            public void Use()
            {
                ErrorCode err;
                try
                {
                    NativeMethods.udata_setCommonData(unmanagedICUMemData, out err);
                }
                catch (Exception ex)
                {
                }
            }
            public void Dispose()
            {
                if (unmanagedICUMemData != IntPtr.Zero)
                {
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(unmanagedICUMemData);
                    unmanagedICUMemData = IntPtr.Zero;
                }
            }
        }


        static InMemoryIcuDataHolder inMemDataHolder;
        static BreakIterator()
        {
            inMemDataHolder = new InMemoryIcuDataHolder();
            inMemDataHolder.Use();
        }

        /// <summary>
        /// Value indicating all text boundaries have been returned.
        /// </summary>
        public const int DONE = -1;
        /// <summary>
        /// Splits the specified text along the specified type of boundaries. Spaces and punctuations
        /// are not returned.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="text">The text.</param>
        /// <returns>The tokens.</returns>
        public static IEnumerable<string> Split(UBreakIteratorType type, Locale locale, string text)
        {
            return Split(type, locale.Id, text);
        }

        public static IEnumerable<string> Split(UBreakIteratorType type, string locale, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new string[] { };
            }


            ErrorCode err;
            IntPtr bi = NativeMethods.ubrk_open(type, locale, text, text.Length, out err);
            if (err != ErrorCode.NoErrors)
                throw new Exception("BreakIterator.Split() failed with code " + err);
            var tokens = new List<string>();
            int cur = NativeMethods.ubrk_first(bi);
            while (cur != DONE)
            {
                int next = NativeMethods.ubrk_next(bi);
                int status = NativeMethods.ubrk_getRuleStatus(bi);
                if (next != DONE && AddToken(type, status))
                    tokens.Add(text.Substring(cur, next - cur));
                cur = next;
            }
            NativeMethods.ubrk_close(bi);
            return tokens;
        }
        //------------------------------------------
        private static bool AddToken(UBreakIteratorType type, int status)
        {
            switch (type)
            {
                case UBreakIteratorType.CHARACTER:
                    return true;
                case UBreakIteratorType.LINE:
                case UBreakIteratorType.SENTENCE:
                    return true;
                case UBreakIteratorType.WORD:
                    return status < (int)UWordBreak.NONE || status >= (int)UWordBreak.NONE_LIMIT;
            }
            return false;
        }
        //-------------------------------------------------------------------------------------------------
        //WinterDev
        public static IEnumerable<SplitBound> GetSplitBoundIter(UBreakIteratorType type,
            string locale,
            char[] charBuffer,
            int start,
            int len)
        {
            if (charBuffer == null || charBuffer.Length == 0)
            {
                return new SplitBound[] { };
            }

            ErrorCode err;
            var tokens = new List<SplitBound>();
            unsafe
            {
                fixed (char* head = &charBuffer[0])
                {
                    IntPtr bi = NativeMethods.ubrk_open_unsafe(type, locale, head + start, len, out err);
                    if (err == ErrorCode.USING_DEFAULT_WARNING)
                    {
                    }
                    else if (err != ErrorCode.NoErrors)
                    {
                        throw new Exception("BreakIterator.Split() failed with code " + err);
                    }
                    int cur = NativeMethods.ubrk_first(bi);
                    while (cur != DONE)
                    {
                        int next = NativeMethods.ubrk_next(bi);
                        int status = NativeMethods.ubrk_getRuleStatus(bi);
                        if (next != DONE && AddToken(type, status))
                            tokens.Add(new SplitBound(cur, next - cur));
                        cur = next;
                    }
                    NativeMethods.ubrk_close(bi);
                }
            }
            return tokens;
        }
    }

    public struct SplitBound
    {
        public readonly int startIndex;
        public readonly int length;
        public SplitBound(int startIndex, int length)
        {
            this.startIndex = startIndex;
            this.length = length;
        }
#if DEBUG
        public override string ToString()
        {
            return startIndex + ":" + length;
        }
#endif
    }
}
