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
    internal sealed class CssRectWord : CssRect
    {
        #region Fields and Consts

        /// <summary>
        /// The word text
        /// </summary>
        private readonly string _text;
        CssRectKind rectKind;

        /// <summary>
        /// was there a whitespace before the word chars (before trim)
        /// </summary>
        private readonly bool _hasSpaceBefore;

        /// <summary>
        /// was there a whitespace after the word chars (before trim)
        /// </summary>
        private readonly bool _hasSpaceAfter;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        /// <param name="text">the word chars </param>
        /// <param name="hasSpaceBefore">was there a whitespace before the word chars (before trim)</param>
        /// <param name="hasSpaceAfter">was there a whitespace after the word chars (before trim)</param>
        public CssRectWord(CssBox owner, string text, bool hasSpaceBefore, bool hasSpaceAfter)
            : base(owner)
        {
            _text = text;
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
                switch (this.rectKind)
                {   
                    case CssRectKind.Unknown:
                        return EvaluateText() == CssRectKind.Space;
                    case CssRectKind.Space:
                        return true;
                    default:
                        return false;
                }                 
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
                switch (this.rectKind)
                {
                    case CssRectKind.Unknown:
                        return EvaluateText() == CssRectKind.Space;
                    case CssRectKind.LineBreak:
                        return true;
                    default:
                        return false;
                }    
            }
        }


        CssRectKind EvaluateText()
        {
            char[] arr = this._text.ToCharArray();

            if (arr.Length == 1)
            {
                char c = arr[0];

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
                for (int i = arr.Length - 1; i >= 0; --i)
                {
                    if (!char.IsWhiteSpace(arr[i]))
                    {
                        //if only one is not space
                        is_space = false;
                        break;
                    }
                }
                return this.rectKind = is_space ? CssRectKind.Space : CssRectKind.Text;
            }
            //foreach (var c in Text)
            //{
            //    if (!char.IsWhiteSpace(c))
            //        return false;
            //}
            //return true; 
        }
        /// <summary>
        /// Gets the text of the word
        /// </summary>
        public override string Text
        {
            get { return _text; }
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