//MIT, 2014-2016, WinterDev

using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.Text
{

    public enum TextBreakKind
    {
        Word,
        Sentence,
    }

    public delegate void OnBreak(SplitBound splitBound);
    public abstract class TextBreaker
    {
        public abstract void DoBreak(char[] input, int start, int len, OnBreak onbreak);
        public TextBreakKind BreakKind
        {
            get;
            set;
        }
        public void DoBreak(string input, OnBreak onbreak)
        {
            IsCanceled = false;//reset
            char[] charBuff = input.ToCharArray();
            //to end
            DoBreak(charBuff, 0, charBuff.Length, onbreak);
        }
        public void DoBreak(string input, int start, int len, OnBreak onbreak)
        {
            IsCanceled = false;//reset
            char[] charBuff = input.ToCharArray();
            //
            DoBreak(charBuff, start, len, onbreak);
        }
        protected bool IsCanceled { get; set; }
        /// <summary>
        /// cancel current breaking task
        /// </summary>
        public void Cancel() { IsCanceled = true; }
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