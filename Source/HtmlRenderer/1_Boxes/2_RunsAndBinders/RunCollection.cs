//BSD 2014,WinterDev
using System.Collections.Generic;
namespace HtmlRenderer.Dom
{
    class ContentRuns
    {
        List<TextSplitPart> originalSplitParts;
        char[] originalBuffer;
        List<CssRun> runList;
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