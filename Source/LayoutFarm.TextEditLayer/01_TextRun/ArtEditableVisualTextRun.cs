//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;


namespace LayoutFarm.Presentation.Text
{

    public partial class ArtEditableVisualTextRun : ArtVisualTextRun
    {


        private ArtEditableVisualTextRun(char[] myBuffer)
            : base(myBuffer)
        {

        }

        public ArtEditableVisualTextRun(char c)
            : base(c)
        {

        }
        public ArtEditableVisualTextRun(string str)
            : base(str)
        {

        }

        public int GetRunWidth(int charCount)
        {

            return CalculateDrawingStringSize(mybuffer, charCount).Width;
        }
        internal ArtEditableVisualTextRun Clone()
        {
            return new ArtEditableVisualTextRun(this.Text);
        }
        Size CalculateDrawingStringSize(char[] buffer, int length)
        {
            TextFontInfo textFontInfo = GetTextFontInfo();
            return new Size(
                 textFontInfo.GetStringWidth(buffer, length),
                 textFontInfo.FontHeight);
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

        public ArtEditableVisualTextRun Copy(int startIndex, int length)
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
        public override ArtVisualElement ParentVisualElement
        {
            get
            {
                EditableVisualElementLine ownerline = this.OwnerEditableLine;
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
        ArtEditableVisualTextRun MakeTextRun(int sourceIndex, int length)
        {

            if (length > 0)
            {
                char[] newContent = new char[length];
                Array.Copy(this.mybuffer, sourceIndex, newContent, 0, length);
                ArtEditableVisualTextRun newTextRun = new ArtEditableVisualTextRun(newContent);
                BoxStyle bah = this.Beh;
                if (bah != null)
                {
                    newTextRun.SetBehavior(bah, null);
                }

                newTextRun.IsLineBreak = this.IsLineBreak;
                newTextRun.UpdateRunWidth(null);
                return newTextRun;
            }
            else
            {
                throw new Exception("string must be null or zero length");
            }

        }
        public ArtEditableVisualTextRun Copy(int startIndex)
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

         
        public EditableVisualElementLine OwnerEditableLine
        {
            get
            {
                var parentLink = this.MyParentLink as VisualEditableLineParentLink;
                if (parentLink != null)
                {
                    return (EditableVisualElementLine)(parentLink.internalLinkedNode.List);
                }
                return null;
            }
        }
        public ArtEditableVisualTextRun NextTextRun
        {
            get
            {
                VisualEditableLineParentLink parent = (VisualEditableLineParentLink)this.ParentLink;
                return parent.Next as ArtEditableVisualTextRun; 
            }
        }
        public ArtEditableVisualTextRun PrevTextRun
        {
            get
            {
                VisualEditableLineParentLink parent = (VisualEditableLineParentLink)this.ParentLink;
                return parent.Prev as ArtEditableVisualTextRun;
            }
        }

    }
}