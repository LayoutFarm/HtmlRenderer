// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.Text
{

    public partial class EditableTextSpan : TextSpan
    {
       
        EditableTextLine ownerTextLine;
        LinkedListNode<EditableTextSpan> _internalLinkedNode;
        bool isInsertable = true;
      
        public EditableTextSpan(RootGraphic gfx, char[] myBuffer, TextSpanStyle style)
            : base(gfx, myBuffer, style)
        {

        }
        public EditableTextSpan(RootGraphic gfx, char c, TextSpanStyle style)
            : base(gfx, c, style)
        {

        }
        public EditableTextSpan(RootGraphic gfx, string str, TextSpanStyle style)
            : base(gfx, str, style)
        {

        }


        public int GetRunWidth(int charOffset)
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
        public bool IsInsertable
        {
            get
            {
                return isInsertable;
            }
        }

      
        public override void CustomDrawToThisCanvas(Canvas canvas, Rectangle updateArea)
        {
            
                this.DrawCharacters(canvas, updateArea, this.mybuffer);
             
        }
        //public override void UpdateRunWidth()
        //{
        //    if (this.isFreezed)
        //    {
        //        Size size;
        //        if (IsLineBreak)
        //        {
        //            size = CalculateDrawingStringSize(emptyline);
        //        }
        //        else
        //        {
        //            size = CalculateDrawingStringSize(this.freezeTextBuffer);
        //        }
        //        this.SetSize(size.Width, size.Height);
        //        MarkHasValidCalculateSize();
        //    }
        //    else
        //    {
        //        base.UpdateRunWidth();
        //    }
        //}
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

    }
}