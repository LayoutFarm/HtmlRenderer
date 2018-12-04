//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

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


namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// Represents a word inside an inline box
    /// </summary>
    public sealed class CssTextRun : CssRun
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
        private CssTextRun(CssRunKind rectKind,
            int start, int len)
            : base(rectKind)
        {
            _textStartIndex = start;
            _textLength = len;
        }

        private CssTextRun(CssRunKind kind)
            : base(kind)
        {
            //for single space only 
            _textLength = 1;
        }

        private CssTextRun(int whiteSpaceLength)
            : base(CssRunKind.Space)
        {
            //for space only
            _textLength = whiteSpaceLength;
        }

        //================================================================
        public static CssTextRun CreateTextRun(int startIndex, int len)
        {
            return new CssTextRun(CssRunKind.Text, startIndex, len);
        }
        public static CssTextRun CreateSingleWhitespace()
        {
            return new CssTextRun(CssRunKind.SingleSpace);
        }
        public static CssTextRun CreateLineBreak()
        {
            return new CssTextRun(CssRunKind.LineBreak);
        }
        public static CssTextRun CreateWhitespace(int count)
        {
            if (count == 1)
            {
                return new CssTextRun(CssRunKind.SingleSpace);
            }
            else
            {
                return new CssTextRun(count);
            }
        }
        //================================================================

        public void MakeLength1()
        {
            _textLength = 1;
        }


        public int TextLength
        {
            get
            {
                return _textLength;
            }
        }
        internal int TextStartIndex
        {
            get
            {
                return _textStartIndex;
            }
        }
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
                            return new string(' ', _textLength);
                        }
                    case CssRunKind.Text:
                        {
                            char[] ownerTextBuff = CssBox.UnsafeGetTextBuffer(this.OwnerBox);
                            return new string(ownerTextBuff, _textStartIndex, _textLength);
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
        public override void WriteContent(System.Text.StringBuilder stbuilder, int start, int length)
        {
#if DEBUG
            if (start < -1)
            {
                throw new System.NotSupportedException();
            }
#endif

            switch (this.Kind)
            {
                case CssRunKind.Space:
                    {
                        stbuilder.Append(new string(' ', _textLength));
                    }
                    break;
                case CssRunKind.Text:
                    {
                        char[] ownerTextBuff = CssBox.UnsafeGetTextBuffer(this.OwnerBox);
                        stbuilder.Append(ownerTextBuff, _textStartIndex, _textLength);
                    }
                    break;
                case CssRunKind.SingleSpace:
                    {
                        stbuilder.Append(' ');
                    }
                    break;
            }
        }


        /// <summary>
        /// Represents this word for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string txt = this.Text;
            return string.Format("{0} ({1} char{2})", txt.Replace(' ', '-').Replace("\n", "\\n"), txt.Length, txt.Length != 1 ? "s" : string.Empty);
        }
    }
}