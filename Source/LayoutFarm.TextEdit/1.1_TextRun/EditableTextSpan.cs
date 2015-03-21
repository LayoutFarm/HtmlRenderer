// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.Text
{

    public partial class EditableTextSpan : TextSpan
    {
        private EditableTextSpan(RootGraphic gfx, char[] myBuffer)
            : base(gfx, myBuffer)
        {
        }
        public EditableTextSpan(RootGraphic gfx, char c)
            : base(gfx, c)
        {

        }
        public EditableTextSpan(RootGraphic gfx, string str)
            : base(gfx, str)
        {

        }

        public int GetRunWidth(int charCount)
        {

            return CalculateDrawingStringSize(mybuffer, charCount).Width;
        }
        internal EditableTextSpan Clone()
        {
            return new EditableTextSpan(this.Root, this.Text);
        }
        Size CalculateDrawingStringSize(char[] buffer, int length)
        {
            FontInfo FontInfo = GetFontInfo();
            return new Size(
                 FontInfo.GetStringWidth(buffer, length),
                 FontInfo.FontHeight);
        }
        public bool IsInsertable
        {
            get
            {
                return true;
            }
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
        public override RenderElement ParentRenderElement
        {
            get
            {
                EditableTextLine ownerline = this.OwnerEditableLine;
                if (ownerline != null)
                {
                    return ownerline.OwnerElement;
                }
                else
                {
                    return null;
                }
            }
        }
        EditableTextSpan MakeTextRun(int sourceIndex, int length)
        {

            if (length > 0)
            {
                char[] newContent = new char[length];
                Array.Copy(this.mybuffer, sourceIndex, newContent, 0, length);
                EditableTextSpan newTextRun = new EditableTextSpan(this.Root, newContent);

                if (this.SpanStyle != null)
                {
                    newTextRun.SetStyle(this.SpanStyle);
                }

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
                var parentLink = this.ParentLink as VisualEditableLineParentLink;
                if (parentLink != null)
                {
                    return (EditableTextLine)(parentLink.internalLinkedNode.List);
                }
                return null;
            }
        }
        public EditableTextSpan NextTextRun
        {
            get
            {
                VisualEditableLineParentLink parent = (VisualEditableLineParentLink)this.ParentLink;
                return parent.Next as EditableTextSpan;
            }
        }
        public EditableTextSpan PrevTextRun
        {
            get
            {
                VisualEditableLineParentLink parent = (VisualEditableLineParentLink)this.ParentLink;
                return parent.Prev as EditableTextSpan;
            }
        }

    }
}