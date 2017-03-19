//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
namespace LayoutFarm.Text
{
    public abstract class EditableRun : RenderElement
    {
        EditableTextLine ownerTextLine;
        LinkedListNode<EditableRun> _editableRunInternalLinkedNode;
        public EditableRun(RootGraphic gfx)
            : base(gfx, 10, 10)
        {

        }
        internal bool IsLineBreak { get; set; }
        internal abstract bool IsInsertable { get; }
        public abstract string Text { get; }
        public abstract int CharacterCount { get; }
        public abstract TextSpanStyle SpanStyle { get; }
        //--------------------
        public abstract void SetStyle(TextSpanStyle spanStyle);
        public abstract char GetChar(int index);
        public abstract EditableRunCharLocation GetCharacterFromPixelOffset(int pixelOffset);
        /// <summary>
        /// get run width from start (left**) to charOffset
        /// </summary>
        /// <param name="charOffset"></param>
        /// <returns></returns>
        public abstract int GetRunWidth(int charOffset);

        ///////////////////////////////////////////////////////////////
        //edit funcs
        internal abstract void InsertAfter(int index, char c);
        internal abstract EditableRun Remove(int startIndex, int length, bool withFreeRun);
        internal static EditableRun InnerRemove(EditableRun tt, int startIndex, int length, bool withFreeRun)
        {
            return tt.Remove(startIndex, length, withFreeRun);
        }
        internal static EditableRun InnerRemove(EditableRun tt, int startIndex, bool withFreeRun)
        {
            return tt.Remove(startIndex, tt.CharacterCount - (startIndex), withFreeRun);
        }
        internal static EditableRunCharLocation InnerGetCharacterFromPixelOffset(EditableRun tt, int pixelOffset)
        {
            return tt.GetCharacterFromPixelOffset(pixelOffset);
        }
        internal abstract void UpdateRunWidth();
        ///////////////////////////////////////////////////////////////  
        public abstract EditableRun Clone();
        public abstract EditableRun LeftCopy(int index);
        public abstract EditableRun Copy(int startIndex, int length);
        public abstract EditableRun Copy(int startIndex);
        public abstract void CopyContentToStringBuilder(StringBuilder stBuilder);
        //------------------------------
        //owner, neighbor
        public EditableRun NextTextRun
        {
            get
            {
                if (this.LinkedNodeForEditableRun != null)
                {
                    if (LinkedNodeForEditableRun.Next != null)
                    {
                        return LinkedNodeForEditableRun.Next.Value;
                    }
                }
                return null;
            }
        }
        public EditableRun PrevTextRun
        {
            get
            {
                if (this.LinkedNodeForEditableRun != null)
                {
                    if (LinkedNodeForEditableRun.Previous != null)
                    {
                        return LinkedNodeForEditableRun.Previous.Value;
                    }
                }
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
        internal LinkedListNode<EditableRun> LinkedNodeForEditableRun
        {
            get { return this._editableRunInternalLinkedNode; }
        }
        internal void SetInternalLinkedNode(LinkedListNode<EditableRun> linkedNode, EditableTextLine ownerTextLine)
        {
            this.ownerTextLine = ownerTextLine;
            this._editableRunInternalLinkedNode = linkedNode;
            EditableRun.SetParentLink(this, ownerTextLine);
        }
        //----------------------------------------------------------------------
        public override void TopDownReCalculateContentSize()
        {
            InnerTextRunTopDownReCalculateContentSize(this);
        }

        public static void InnerTextRunTopDownReCalculateContentSize(EditableRun ve)
        {
#if DEBUG
            dbug_EnterTopDownReCalculateContent(ve);
#endif

            ve.UpdateRunWidth();
#if DEBUG
            dbug_ExitTopDownReCalculateContent(ve);
#endif
        }

#if DEBUG
        public override string dbug_FullElementDescription()
        {
            string user_elem_id = null;
            if (user_elem_id != null)
            {
                return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
                    + " i" + dbug_obj_id + "a " + ((EditableRun)this).Text + ",(ID " + user_elem_id + ") " + dbug_GetLayoutInfo();
            }
            else
            {
                return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
                 + " i" + dbug_obj_id + "a " + ((EditableRun)this).Text + " " + dbug_GetLayoutInfo();
            }
        }
        public override string ToString()
        {
            return "[" + this.dbug_obj_id + "]" + Text;
        }
#endif
    }
}