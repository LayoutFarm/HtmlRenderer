//Apache2, 2014-2016, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Text;
using LayoutFarm.Composers;
using LayoutFarm.Text;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{

    public class ContentTextSplitter
    {
        //configure icu's locale here 
        string icuLocal = "th-TH";
        public ContentTextSplitter()
        {

        }
        public IEnumerable<TextSplitBound> ParseWordContent(char[] textBuffer, int startIndex, int appendLength)
        {

            int s_index = startIndex;
            TextBreaker textBreaker = RootGraphic.GetTextBreaker(icuLocal);
            List<SplitBound> runlist = new List<SplitBound>();
            textBreaker.DoBreak(textBuffer, startIndex, appendLength, bounds =>
            {
                //iterate new split
                runlist.Add(bounds);
            });

            foreach (var splitBound in runlist)
            {
                //need consecutive bound
                if (splitBound.startIndex != s_index)
                {
                    yield return new TextSplitBound(s_index, splitBound.startIndex - s_index);
                    s_index = splitBound.startIndex;
                }
                s_index += splitBound.length;
                yield return new TextSplitBound(splitBound.startIndex, splitBound.length);
            }
            if (s_index < textBuffer.Length)
            {
                yield return new TextSplitBound(s_index, textBuffer.Length - s_index);
            }
        }
    }
}