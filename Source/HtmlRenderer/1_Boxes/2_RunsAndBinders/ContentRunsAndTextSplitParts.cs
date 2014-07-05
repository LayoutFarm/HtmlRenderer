//BSD 2014,WinterDev
using System.Collections.Generic;
namespace HtmlRenderer.Dom
{
    class ContentRuns
    {
        List<TextSplitPart> originalSplitParts;
        char[] originalBuffer;
        List<CssRun> runList;
        bool isWhiteSpace;
        public ContentRuns(char[] originalBuffer)
        {
            this.originalBuffer = originalBuffer;
        }

        internal char[] GetOriginalBuffer()
        {
            return this.originalBuffer;
        }
        internal List<TextSplitPart> OriginalSplitParts
        {
            get { return this.originalSplitParts; }
            set { this.originalSplitParts = value; }
        }
        internal List<CssRun> RunList
        {
            get { return this.runList; }
            set { this.runList = value; }
        }
        public int RunCount
        {
            get { return this.runList.Count; }
        }
        internal void ParseContent(ContentTextSplitter splitter, CssWhiteSpace whitespace, bool isBreakAll, bool forAnonBox)
        {

            //this._boxRuns = ContentTextSplitter.DefaultSplitter.ParseWordContent(
            //    _aa_textBuffer, this.WhiteSpace,
            //     this.WordBreak != CssWordBreak.BreakAll,
            //     this.HtmlElement == null);
        }
        internal void EvaluateWhitespace()
        {
            isWhiteSpace = false;
            char[] tmp = this.originalBuffer;
            for (int i = tmp.Length - 1; i >= 0; --i)
            {
                if (!char.IsWhiteSpace(tmp[i]))
                {
                    //stop and return if found whitespace
                    return;
                }
            }
            //exit here means all char is whitespace
            isWhiteSpace = true;
        }
        internal bool IsWhiteSpace
        {
            get { return this.isWhiteSpace; }
        }

    }
    enum TextSplitPartKind : byte
    {
        Text,
        Whitespace,
        SingleWhitespace,
        LineBreak,
    }
    struct TextSplitPart
    {
        public readonly int startIndex;
        public readonly int length;
        public readonly TextSplitPartKind kind;
        public TextSplitPart(int startIndex, int length, TextSplitPartKind kind)
        {
            this.startIndex = startIndex;
            this.length = length;
            this.kind = kind;
        }
    }

}