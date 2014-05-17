//BSD 2014,WinterCore

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
using System.Collections.Generic;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;
using System.Text;

namespace HtmlRenderer.Dom
{

    class CssTextSplitter
    {
        public static readonly CssTextSplitter DefaultSplitter = new CssTextSplitter();

        public CssTextSplitter()
        {
        }


        public void ParseWordContent(CssBox box)
        {

            bool preserveSpaces = box.WhiteSpace == CssWhiteSpace.Pre || box.WhiteSpace == CssWhiteSpace.PreWrap;
            bool respectNewline = preserveSpaces || box.WhiteSpace == CssWhiteSpace.PreLine;

            char[] textBuffer = CssBox.UnsafeGetTextBuffer(box);
            List<CssRect> boxWords = box.Words;

            int startIndex = 0;
            int buffLength = textBuffer.Length;
            bool boxIsNotBreakAll = box.WordBreak != CssWordBreak.BreakAll;

            while (startIndex < buffLength)
            {
                char cur = textBuffer[startIndex];
                if (cur == '\r')
                {
                    startIndex++;
                    continue;
                }
                if (startIndex < buffLength)
                {
                    var endIdx = startIndex;
                    while (endIdx < buffLength && char.IsWhiteSpace((cur = textBuffer[endIdx])) && cur != '\n')
                    {
                        endIdx++;
                    }

                    if (endIdx > startIndex)
                    {
                        if (preserveSpaces)
                        {
                            boxWords.Add(new CssRectWord(box, startIndex, endIdx - startIndex, false, false));
                        }
                    }
                    else
                    {
                        endIdx = startIndex;

                        if (boxIsNotBreakAll)
                        {
                            while (endIdx < buffLength && !char.IsWhiteSpace(cur = textBuffer[endIdx]) && cur != '-' &&
                                  !CommonUtils.IsAsianCharecter(cur))
                            {
                                endIdx++;
                            }
                        }
                         
                        if (endIdx < buffLength &&
                             ((cur = textBuffer[endIdx]) == '-' ||
                             !boxIsNotBreakAll ||
                             CommonUtils.IsAsianCharecter(cur)))
                        {
                            endIdx++;
                        }
                        
                        if (endIdx > startIndex)
                        {

                            var hasSpaceBefore = !preserveSpaces && (startIndex > 0 && boxWords.Count == 0 && char.IsWhiteSpace(textBuffer[startIndex - 1]));
                            var hasSpaceAfter = !preserveSpaces && (endIdx < buffLength && char.IsWhiteSpace(cur));

                            boxWords.Add(new CssRectWord(box, startIndex, endIdx - startIndex, hasSpaceBefore, hasSpaceAfter));
                        }
                    }

                    // create new-line word so it will effect the layout
                    if (endIdx < buffLength && cur == '\n')
                    {
                        endIdx++;
                        if (respectNewline)
                        {
                            boxWords.Add(new CssRectWord(box, startIndex, endIdx - startIndex, false, false));
                        }
                    }

                    startIndex = endIdx;
                }
            }
        }



    }
}