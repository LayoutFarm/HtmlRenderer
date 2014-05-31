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


namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Represents a word inside an inline box
    /// </summary>
    sealed class CssTextRun : CssRun
    { 
        int _textStartIndex;
        int _textLength;

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        /// <param name="text">the word chars </param>
        /// <param name="hasSpaceBefore">was there a whitespace before the word chars (before trim)</param>
        /// <param name="hasSpaceAfter">was there a whitespace after the word chars (before trim)</param>
        private CssTextRun(CssBox owner, CssRunKind rectKind,
            int start, int len)
            : base(owner, rectKind)
        {

            this._textStartIndex = start;
            this._textLength = len;
        }

        private CssTextRun(CssBox owner, CssRunKind kind)
            : base(owner, kind)
        {
            //for single space only 
            this._textLength = 1;

        }
        private CssTextRun(CssBox owner, int whiteSpaceLength)
            : base(owner, CssRunKind.Space)
        {
            //for space only

            this._textLength = whiteSpaceLength;
        }

        //================================================================
        public static CssTextRun CreateTextRun(CssBox owner, int startIndex, int len)
        {
            return new CssTextRun(owner, CssRunKind.Text, startIndex, len);
        }
        public static CssTextRun CreateSingleWhitespace(CssBox owner)
        {
            return new CssTextRun(owner, CssRunKind.SingleSpace);
        }
        public static CssTextRun CreateLineBreak(CssBox owner)
        {
            return new CssTextRun(owner, CssRunKind.LineBreak);//, hasSpaceBefore, hasSpaceAfter);
        }
        public static CssTextRun CreateWhitespace(CssBox owner, int count)
        {
            if (count == 1)
            {
                return new CssTextRun(owner, CssRunKind.SingleSpace);
            }
            else
            {
                return new CssTextRun(owner, count);
            }
        }
        //================================================================


       


        /// <summary>
        /// Gets the text of the word
        /// </summary>
        public override string Text
        {
            get
            {
                switch (this.Kind)
                {
                    case CssRunKind.Space:
                        {
                            return new string(' ', this._textLength);
                        }
                    case CssRunKind.Text:
                        {
                            char[] ownerTextBuff = CssBox.UnsafeGetTextBuffer(this.OwnerBox);
                            return new string(ownerTextBuff, this._textStartIndex, this._textLength);
                        }
                    case CssRunKind.SingleSpace:
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