// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.Text
{

    public partial class EditableTextSpan : EditableRun
    {
        char[] mybuffer;
        EditableTextLine ownerTextLine;
        LinkedListNode<EditableTextSpan> _internalLinkedNode;
        bool isInsertable = true;
        public EditableTextSpan(RootGraphic gfx, char[] copyBuffer, TextSpanStyle style)
            : base(gfx, style)
        {   //check line break?
            this.mybuffer = copyBuffer;
            UpdateRunWidth();
        }
        public EditableTextSpan(RootGraphic gfx, char c, TextSpanStyle style)
            : base(gfx, style)
        {

            mybuffer = new char[] { c };
            if (c == '\n')
            {
                this.IsLineBreak = true;
            }
            //check line break?
            UpdateRunWidth();
        }
        public EditableTextSpan(RootGraphic gfx, string str, TextSpanStyle style)
            : base(gfx, style)
        {

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

        public override int GetRunWidth(int charOffset)
        {
            return CalculateDrawingStringSize(mybuffer, charOffset).Width;
        }

        internal EditableTextSpan Clone()
        {
            return new EditableTextSpan(this.Root, this.Text, this.SpanStyle);
        }
        internal LinkedListNode<EditableTextSpan> internalLinkedNode
        {
            get { return this._internalLinkedNode; }

        }
        internal void SetInternalLinkedNode(LinkedListNode<EditableTextSpan> linkedNode, EditableTextLine ownerTextLine)
        {
            this.ownerTextLine = ownerTextLine;
            this._internalLinkedNode = linkedNode;
            EditableTextSpan.SetParentLink(this, ownerTextLine);
        }
        Size CalculateDrawingStringSize(char[] buffer, int length)
        {
            FontInfo FontInfo = GetFontInfo();
            return new Size(
                 FontInfo.GetStringWidth(buffer, length),
                 FontInfo.FontHeight);
        }
        FontInfo GetFontInfo()
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
        public bool IsInsertable
        {
            get
            {
                return isInsertable;
            }
        }

        const int SAME_FONT_SAME_TEXT_COLOR = 0;
        const int SAME_FONT_DIFF_TEXT_COLOR = 1;
        const int DIFF_FONT_SAME_TEXT_COLOR = 2;
        const int DIFF_FONT_DIFF_TEXT_COLOR = 3;

        static int EvaluateFontAndTextColor(Canvas canvas, TextSpanStyle spanStyle)
        {
            var font = spanStyle.FontInfo.ResolvedFont;
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
                            canvas.CurrentFont = style.FontInfo.ResolvedFont;
                            canvas.DrawText(this.mybuffer,
                               new Rectangle(0, 0, bWidth, bHeight),
                               style.ContentHAlign);

                            canvas.CurrentFont = prevFont;
                        } break;
                    case DIFF_FONT_DIFF_TEXT_COLOR:
                        {
                            var prevFont = canvas.CurrentFont;
                            var prevColor = canvas.CurrentTextColor;

                            canvas.CurrentFont = style.FontInfo.ResolvedFont;
                            canvas.CurrentTextColor = style.FontColor;
                            canvas.DrawText(this.mybuffer,
                               new Rectangle(0, 0, bWidth, bHeight),
                               style.ContentHAlign);

                            canvas.CurrentFont = prevFont;
                            canvas.CurrentTextColor = prevColor;

                        } break;
                    case SAME_FONT_DIFF_TEXT_COLOR:
                        {
                            var prevColor = canvas.CurrentTextColor;
                            canvas.DrawText(this.mybuffer,
                            new Rectangle(0, 0, bWidth, bHeight),
                            style.ContentHAlign);
                            canvas.CurrentTextColor = prevColor;
                        } break;
                    default:
                        {
                            canvas.DrawText(this.mybuffer,
                               new Rectangle(0, 0, bWidth, bHeight),
                               style.ContentHAlign);
                        } break;
                }
            }


        }
        public override void UpdateRunWidth()
        {
            Size size;
            if (IsLineBreak)
            {
                size = CalculateDrawingStringSize(emptyline);
            }
            else
            {
                size = CalculateDrawingStringSize(this.mybuffer);
            }
            this.SetSize(size.Width, size.Height);
            MarkHasValidCalculateSize();

        }
        Size CalculateDrawingStringSize(char[] buffer)
        {
            FontInfo fontInfo = GetFontInfo();
            return new Size(
                fontInfo.GetStringWidth(buffer),
                fontInfo.FontHeight
                );
        }
        public override string Text
        {
            get { return new string(mybuffer); }
        }
        public void CopyContentToStringBuilder(StringBuilder stBuilder)
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
        public char this[int index]
        {
            get
            {

                return mybuffer[index];
            }
        }
        public int CharacterCount
        {
            get
            {
                return mybuffer.Length;
            }
        }

        public EditableTextSpan Copy(int startIndex, int length)
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
        EditableTextSpan MakeTextRun(int sourceIndex, int length)
        {

            if (length > 0)
            {

                EditableTextSpan newTextRun = null;
                char[] newContent = new char[length];
                Array.Copy(this.mybuffer, sourceIndex, newContent, 0, length);
                newTextRun = new EditableTextSpan(this.Root, newContent, this.SpanStyle);
                newTextRun.IsLineBreak = this.IsLineBreak;
                newTextRun.UpdateRunWidth();
                return newTextRun;
            }
            else
            {
                throw new Exception("string must be null or zero length");
            }

        }
        public EditableTextSpan Copy(int startIndex)
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


        internal EditableTextLine OwnerEditableLine
        {
            get
            {
                return this.ownerTextLine;
            }
        }
        public EditableTextSpan NextTextRun
        {
            get
            {
                if (this.internalLinkedNode != null)
                {
                    if (internalLinkedNode.Next != null)
                    {
                        return internalLinkedNode.Next.Value;
                    }
                }
                return null;
            }
        }
        public EditableTextSpan PrevTextRun
        {
            get
            {

                if (this.internalLinkedNode != null)
                {
                    if (internalLinkedNode.Previous != null)
                    {
                        return internalLinkedNode.Previous.Value;
                    }
                }
                return null;
            }
        }
        internal static readonly char[] emptyline = new char[] { 'I' };
    }
}