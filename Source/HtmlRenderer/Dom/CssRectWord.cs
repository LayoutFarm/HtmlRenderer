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


namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Represents a word inside an inline box
    /// </summary>
    sealed class CssRectWord : CssRect
    {

        /// <summary>
        /// was there a whitespace before the word chars (before trim)
        /// </summary>
        readonly bool _hasSpaceBefore;
        /// <summary>
        /// was there a whitespace after the word chars (before trim)
        /// </summary>
        readonly bool _hasSpaceAfter;

        int _textStartIndex;
        int _textLength;

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        /// <param name="text">the word chars </param>
        /// <param name="hasSpaceBefore">was there a whitespace before the word chars (before trim)</param>
        /// <param name="hasSpaceAfter">was there a whitespace after the word chars (before trim)</param>
        private CssRectWord(CssBox owner, CssRectKind rectKind,
            int start, int len,
            bool hasSpaceBefore,
            bool hasSpaceAfter)
            : base(owner, rectKind)
        {

            this._textStartIndex = start;
            this._textLength = len;
            _hasSpaceBefore = hasSpaceBefore;
            _hasSpaceAfter = hasSpaceAfter;
        }

        private CssRectWord(CssBox owner, CssRectKind kind, bool hasSpaceBefore, bool hasSpaceAfter)
            : base(owner, kind)
        {
            //for single space only

            this._textLength = 1;
            _hasSpaceBefore = hasSpaceBefore;
            _hasSpaceAfter = hasSpaceAfter;
        }
        private CssRectWord(CssBox owner, int whiteSpaceLength, bool hasSpaceBefore, bool hasSpaceAfter)
            : base(owner, CssRectKind.Space)
        {
            //for space only
           
            this._textLength = whiteSpaceLength;
            _hasSpaceBefore = hasSpaceBefore;
            _hasSpaceAfter = hasSpaceAfter;
        }

        //================================================================
        public static CssRectWord CreateRefText(CssBox owner, int startIndex, int len, bool hasSpaceBefore, bool hasSpaceAfter)
        {
            return new CssRectWord(owner, CssRectKind.Text, startIndex, len, hasSpaceBefore, hasSpaceAfter);
        }
        public static CssRectWord CreateSingleWhitespace(CssBox owner, bool hasSpaceBefore, bool hasSpaceAfter)
        {
            return new CssRectWord(owner, CssRectKind.SingleSpace, hasSpaceBefore, hasSpaceAfter);
        }
        public static CssRectWord CreateLineBreak(CssBox owner, bool hasSpaceBefore, bool hasSpaceAfter)
        {
            return new CssRectWord(owner, CssRectKind.LineBreak, hasSpaceBefore, hasSpaceAfter);
        }
        public static CssRectWord CreateWhitespace(CssBox owner, int count, bool hasSpaceBefore, bool hasSpaceAfter)
        {
            if (count == 1)
            {
                return new CssRectWord(owner, CssRectKind.SingleSpace, hasSpaceBefore, hasSpaceAfter);
            }
            else
            {
                return new CssRectWord(owner, count, hasSpaceBefore, hasSpaceAfter);
            }
        }
        //================================================================

       
        /// <summary>
        /// was there a whitespace before the word chars (before trim)
        /// </summary>
        public override bool HasSpaceBefore
        {
            get { return _hasSpaceBefore; }
        }

        /// <summary>
        /// was there a whitespace after the word chars (before trim)
        /// </summary>
        public override bool HasSpaceAfter
        {
            get { return _hasSpaceAfter; }
        }

        
        /// <summary>
        /// Gets the text of the word
        /// </summary>
        public override string Text
        {
            get
            {
                switch (this.RectKind)
                {
                    case CssRectKind.Space:
                        {
                            return new string(' ', this._textLength);
                        }
                    case CssRectKind.Text:
                        {
                            char[] ownerTextBuff = CssBox.UnsafeGetTextBuffer(this.OwnerBox);
                            return new string(ownerTextBuff, this._textStartIndex, this._textLength);
                        }
                    case CssRectKind.SingleSpace:
                        {
                            return " ";
                        }
                    default:
                        {
                            return "";
                        }
                }

            }
        }

        /// <summary>
        /// Represents this word for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1} char{2})", Text.Replace(' ', '-').Replace("\n", "\\n"), Text.Length, Text.Length != 1 ? "s" : string.Empty);
        }
    }
}