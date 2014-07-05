//BSD 2014,WinterDev
using System.Collections.Generic;
namespace HtmlRenderer.Dom
{
    class RunCollection
    {
        List<TextSplitPart> originalSplitParts;
        char[] originalBuffer;

        List<CssRun> runList;
        bool isWhiteSpace;
        bool isSingleRun;

        public RunCollection(char[] originalBuffer)
        {
            this.runList = new List<CssRun>();
            this.originalBuffer = originalBuffer;
        }
        public RunCollection(CssRun singleRun)
        {
            this.runList = new List<CssRun>();
            singleRun.SetOwner(this);

            this.runList.Add(singleRun);

            isSingleRun = true;
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
        internal CssBox OwnerCssBox
        {
            get;
            set;
        }

        internal List<CssRun> GetInternalList()
        {
            return this.runList;
        }

        public int RunCount
        {
            get { return this.runList.Count; }
        }
        internal void ParseContent(ContentTextSplitter splitter,
            CssWhiteSpace whitespace, bool isBreakAll, bool forAnonBox)
        {
            if (!this.isSingleRun)
            {
                runList.Clear();
                splitter.ParseWordContent(
                   runList,
                   this.originalBuffer,
                   whitespace,
                   !isBreakAll,
                   forAnonBox);

                for (int i = runList.Count - 1; i >= 0; --i)
                {
                    runList[i].SetOwner(this);
                }
            }
        }
        internal void EvaluateWhitespace()
        {
            isWhiteSpace = false;

            char[] tmp = this.originalBuffer;
            if (tmp != null)
            {
                for (int i = tmp.Length - 1; i >= 0; --i)
                {
                    if (!char.IsWhiteSpace(tmp[i]))
                    {
                        //stop and return if found whitespace
                        return;
                    }
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

#if DEBUG
        public override string ToString()
        {
            return kind.ToString() + "(" + startIndex + "," + length + ")";
        }
#endif
    }

}