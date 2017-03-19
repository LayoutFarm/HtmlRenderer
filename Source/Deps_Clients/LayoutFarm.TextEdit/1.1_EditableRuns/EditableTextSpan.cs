//Apache2, 2014-2017, WinterDev

using System;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    class EditableTextRun : EditableRun
    {
        TextSpanStyle spanStyle;
        char[] mybuffer;
        int[] glyphPositions = null;
        public EditableTextRun(RootGraphic gfx, char[] copyBuffer, TextSpanStyle style)
            : base(gfx)
        {   //check line break? 
            this.spanStyle = style;
            this.mybuffer = copyBuffer;
            UpdateRunWidth();
        }
        public EditableTextRun(RootGraphic gfx, char c, TextSpanStyle style)
            : base(gfx)
        {
            this.spanStyle = style;
            mybuffer = new char[] { c };
            if (c == '\n')
            {
                this.IsLineBreak = true;
            }
            //check line break?
            UpdateRunWidth();
        }
        public EditableTextRun(RootGraphic gfx, string str, TextSpanStyle style)
            : base(gfx)
        {
            this.spanStyle = style;
            if (str != null && str.Length > 0)
            {
                mybuffer = str.ToCharArray();
                if (mybuffer.Length == 1 && mybuffer[0] == '\n')
                {
                    this.IsLineBreak = true;
                }
                UpdateRunWidth();
            }
            else
            {
                throw new Exception("string must be null or zero length");
            }
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            DirectSetRootGraphics(this, rootgfx);
        }
        public override EditableRun Clone()
        {
            return new EditableTextRun(this.Root, this.Text, this.SpanStyle);
        }
        public override EditableRun Copy(int startIndex)
        {
            int length = mybuffer.Length - startIndex;
            if (startIndex > -1 && length > 0)
            {
                return MakeTextRun(startIndex, length);
            }
            else
            {
                return null;
            }
        }
        EditableRun MakeTextRun(int sourceIndex, int length)
        {
            if (length > 0)
            {
                EditableRun newTextRun = null;
                char[] newContent = new char[length];
                Array.Copy(this.mybuffer, sourceIndex, newContent, 0, length);
                newTextRun = new EditableTextRun(this.Root, newContent, this.SpanStyle);
                newTextRun.IsLineBreak = this.IsLineBreak;
                newTextRun.UpdateRunWidth();
                return newTextRun;
            }
            else
            {
                throw new Exception("string must be null or zero length");
            }
        }

        public override int GetRunWidth(int charOffset)
        {
            return CalculateDrawingStringSize(mybuffer, charOffset).Width;
        }
        public override string Text
        {
            get { return new string(mybuffer); }
        }

        static readonly char[] emptyline = new char[] { 'I' };

        internal override void UpdateRunWidth()
        {
            Size size;
            if (IsLineBreak)
            {
                size = CalculateDrawingStringSize(emptyline, 1);
                glyphPositions = new int[0];
            }
            else
            {
                //TODO: review here again
                int len = mybuffer.Length;
                size = CalculateDrawingStringSize(this.mybuffer, len);

                //when we update run width we should store
                //cache of each char x-advance?
                //or calculate it every time ?

                //TODO: review this,
                //if we have enough length, -> we don't need to alloc every time. 
                glyphPositions = new int[len];
                Root.IFonts.CalculateGlyphAdvancePos(mybuffer, 0, len, GetFont(), glyphPositions);
                //TextServices.IFonts.CalculateGlyphAdvancePos(mybuffer, 0, len, GetFont(), glyphPositions);
            }
            //---------
            this.SetSize(size.Width, size.Height);

            //---------
            MarkHasValidCalculateSize();
        }
        public override char GetChar(int index)
        {
            return mybuffer[index];
        }


        public override void CopyContentToStringBuilder(StringBuilder stBuilder)
        {
            if (IsLineBreak)
            {
                stBuilder.Append("\r\n");
            }
            else
            {
                stBuilder.Append(mybuffer);
            }
        }
        public override int CharacterCount
        {
            get
            {
                return mybuffer.Length;
            }
        }
        public override TextSpanStyle SpanStyle
        {
            get
            {
                return this.spanStyle;
            }
        }
        public override void SetStyle(TextSpanStyle spanStyle)
        {
            this.InvalidateGraphics();
            this.spanStyle = spanStyle;
            this.InvalidateGraphics();
            UpdateRunWidth();
        }
        Size CalculateDrawingStringSize(char[] buffer, int length)
        {
            PixelFarm.Drawing.RequestFont fontInfo = GetFont();
            return this.Root.IFonts.MeasureString(buffer, 0,
                 length, fontInfo);
        }
        protected PixelFarm.Drawing.RequestFont GetFont()
        {
            if (!HasStyle)
            {
                return this.Root.DefaultTextEditFontInfo;
            }
            else
            {
                TextSpanStyle spanStyle = this.SpanStyle;
                if (spanStyle.FontInfo != null)
                {
                    return spanStyle.FontInfo;
                }
                else
                {
                    return this.Root.DefaultTextEditFontInfo;
                }
            }
        }

        public override EditableRun Copy(int startIndex, int length)
        {
            if (startIndex > -1 && length > 0)
            {
                return MakeTextRun(startIndex, length);
            }
            else
            {
                return null;
            }
        }
        const int SAME_FONT_SAME_TEXT_COLOR = 0;
        const int SAME_FONT_DIFF_TEXT_COLOR = 1;
        const int DIFF_FONT_SAME_TEXT_COLOR = 2;
        const int DIFF_FONT_DIFF_TEXT_COLOR = 3;
        static int EvaluateFontAndTextColor(Canvas canvas, TextSpanStyle spanStyle)
        {
            var font = spanStyle.FontInfo;
            var color = spanStyle.FontColor;
            var currentTextFont = canvas.CurrentFont;
            var currentTextColor = canvas.CurrentTextColor;
            if (font != null && font != currentTextFont)
            {
                if (currentTextColor != color)
                {
                    return DIFF_FONT_DIFF_TEXT_COLOR;
                }
                else
                {
                    return DIFF_FONT_SAME_TEXT_COLOR;
                }
            }
            else
            {
                if (currentTextColor != color)
                {
                    return SAME_FONT_DIFF_TEXT_COLOR;
                }
                else
                {
                    return SAME_FONT_SAME_TEXT_COLOR;
                }
            }
        }
        protected bool HasStyle
        {
            get
            {
                return !this.SpanStyle.IsEmpty();
            }
        }
        public override void CustomDrawToThisCanvas(Canvas canvas, Rectangle updateArea)
        {
            int bWidth = this.Width;
            int bHeight = this.Height;
            if (!this.HasStyle)
            {
                canvas.DrawText(this.mybuffer, new Rectangle(0, 0, bWidth, bHeight), 0);
            }
            else
            {
                TextSpanStyle style = this.SpanStyle;
                switch (EvaluateFontAndTextColor(canvas, style))
                {
                    case DIFF_FONT_SAME_TEXT_COLOR:
                        {
                            var prevFont = canvas.CurrentFont;
                            canvas.CurrentFont = style.FontInfo;
                            canvas.DrawText(this.mybuffer,
                               new Rectangle(0, 0, bWidth, bHeight),
                               style.ContentHAlign);
                            canvas.CurrentFont = prevFont;
                        }
                        break;
                    case DIFF_FONT_DIFF_TEXT_COLOR:
                        {
                            var prevFont = canvas.CurrentFont;
                            var prevColor = canvas.CurrentTextColor;
                            canvas.CurrentFont = style.FontInfo;
                            canvas.CurrentTextColor = style.FontColor;
                            canvas.DrawText(this.mybuffer,
                               new Rectangle(0, 0, bWidth, bHeight),
                               style.ContentHAlign);
                            canvas.CurrentFont = prevFont;
                            canvas.CurrentTextColor = prevColor;
                        }
                        break;
                    case SAME_FONT_DIFF_TEXT_COLOR:
                        {
                            var prevColor = canvas.CurrentTextColor;
                            canvas.DrawText(this.mybuffer,
                            new Rectangle(0, 0, bWidth, bHeight),
                            style.ContentHAlign);
                            canvas.CurrentTextColor = prevColor;
                        }
                        break;
                    default:
                        {
                            canvas.DrawText(this.mybuffer,
                               new Rectangle(0, 0, bWidth, bHeight),
                               style.ContentHAlign);
                        }
                        break;
                }
            }
        }


        public override EditableRunCharLocation GetCharacterFromPixelOffset(int pixelOffset)
        {
            if (pixelOffset < Width)
            {

                int j = glyphPositions.Length;
                int accWidth = 0;
                for (int i = 0; i < j; i++)
                {
                    int charW = glyphPositions[i];
                    if (accWidth + charW > pixelOffset)
                    {
                        if (pixelOffset - accWidth > 3)
                        {
                            return new EditableRunCharLocation(accWidth + charW, i);
                        }
                        else
                        {
                            return new EditableRunCharLocation(accWidth, i - 1);
                        }
                    }
                    else
                    {
                        accWidth += charW;
                    }
                    //char c = myBuffer[i];
                    //int charW = GetCharacterWidth(c);
                    //if (accWidth + charW > pixelOffset)
                    //{
                    //    if (pixelOffset - accWidth > 3)
                    //    {
                    //        return new EditableRunCharLocation(accWidth + charW, i);
                    //    }
                    //    else
                    //    {
                    //        return new EditableRunCharLocation(accWidth, i - 1);
                    //    }
                    //}
                    //else
                    //{
                    //    accWidth += charW;
                    //}
                }
                return new EditableRunCharLocation(accWidth, j - 1);
            }
            else
            {
                return new EditableRunCharLocation(0, -1);
            }
        }
        //-------------------------------------------
        internal override bool IsInsertable
        {
            get
            {
                return true;
            }
        }
        public override EditableRun LeftCopy(int index)
        {
            if (index > -1)
            {
                return MakeTextRun(0, index + 1);
            }
            else
            {
                return null;
            }
        }
        internal override void InsertAfter(int index, char c)
        {
            int oldLexLength = mybuffer.Length;
            char[] newBuff = new char[oldLexLength + 1];
            if (index > -1 && index < mybuffer.Length - 1)
            {
                Array.Copy(mybuffer, newBuff, index + 1);
                newBuff[index + 1] = c;
                Array.Copy(mybuffer, index + 1, newBuff, index + 2, oldLexLength - index - 1);
            }
            else if (index == -1)
            {
                newBuff[0] = c;
                Array.Copy(mybuffer, 0, newBuff, 1, mybuffer.Length);
            }
            else if (index == oldLexLength - 1)
            {
                Array.Copy(mybuffer, newBuff, oldLexLength);
                newBuff[oldLexLength] = c;
            }
            else
            {
                throw new NotSupportedException();
            }
            this.mybuffer = newBuff;
            UpdateRunWidth();
        }
        internal override EditableRun Remove(int startIndex, int length, bool withFreeRun)
        {
            EditableRun freeRun = null;
            if (startIndex > -1 && length > 0)
            {
                int oldLexLength = mybuffer.Length;
                char[] newBuff = new char[oldLexLength - length];
                if (withFreeRun)
                {
                    freeRun = MakeTextRun(startIndex, length);
                }
                if (startIndex > 0)
                {
                    Array.Copy(mybuffer, 0, newBuff, 0, startIndex);
                }

                Array.Copy(mybuffer, startIndex + length, newBuff, startIndex, oldLexLength - startIndex - length);
                this.mybuffer = newBuff;
                UpdateRunWidth();
            }

            if (withFreeRun)
            {
                return freeRun;
            }
            else
            {
                return null;
            }
        }
    }
}
