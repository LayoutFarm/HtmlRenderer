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
        #region Fields and Consts 
        int _textStartIndex;
        int _textLength;

        CssRectKind rectKind;
        /// <summary>
        /// was there a whitespace before the word chars (before trim)
        /// </summary>
        readonly bool _hasSpaceBefore;
        /// <summary>
        /// was there a whitespace after the word chars (before trim)
        /// </summary>
        readonly bool _hasSpaceAfter;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        /// <param name="text">the word chars </param>
        /// <param name="hasSpaceBefore">was there a whitespace before the word chars (before trim)</param>
        /// <param name="hasSpaceAfter">was there a whitespace after the word chars (before trim)</param>
        public CssRectWord(CssBox owner,
            int start, int len,
            bool hasSpaceBefore,
            bool hasSpaceAfter)
            : base(owner)
        {
            this._textStartIndex = start;
            this._textLength = len;
            _hasSpaceBefore = hasSpaceBefore;
            _hasSpaceAfter = hasSpaceAfter;
        }
        public override CssRectKind RectKind
        {
            get
            {
                switch (this.rectKind)
                {
                    case CssRectKind.Unknown:
                        {
                            EvaluateText();
                            return this.rectKind;
                        }
                    default:
                        {
                            return this.rectKind;
                        }
                }
            }
        }
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
        /// Gets a bool indicating if this word is composed only by spaces.
        /// Spaces include tabs and line breaks
        /// </summary>
        public override bool IsSpaces
        {
            get
            {
                //eval once
                return this.rectKind == CssRectKind.Space;
            }
        }
        /// <summary>
        /// Gets if the word is composed by only a line break
        /// </summary>
        public override bool IsLineBreak
        {
            get
            {
                //eval once
                return this.rectKind == CssRectKind.LineBreak;
            }
        }
        CssRectKind EvaluateText()
        {
            char[] ownerTextBuff = CssBox.UnsafeGetTextBuffer(this.OwnerBox);
  
            if (this._textLength == 1)
            {
                char c = ownerTextBuff[this._textStartIndex];
                if (c == '\n')
                {
                    return this.rectKind = CssRectKind.LineBreak;
                }
                else if (char.IsWhiteSpace(c))
                {
                    return this.rectKind = CssRectKind.Space;
                }
                else
                {
                    return this.rectKind = CssRectKind.Text;
                }
            }
            else
            {
                bool is_space = true;
                int realIndex = this._textStartIndex + this._textLength - 1;

                for (int i = this._textLength - 1; i >= 0; --i)
                {
                    if (!char.IsWhiteSpace(ownerTextBuff[realIndex]))
                    {
                        //if only one is not space
                        is_space = false;
                        break;
                    }
                    realIndex--;

                }
                return this.rectKind = is_space ? CssRectKind.Space : CssRectKind.Text;
            }
        }
        /// <summary>
        /// Gets the text of the word
        /// </summary>
        public override string Text
        {
            get
            {
                char[] ownerTextBuff = CssBox.UnsafeGetTextBuffer(this.OwnerBox);
                return new string(ownerTextBuff, this._textStartIndex, this._textLength);
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