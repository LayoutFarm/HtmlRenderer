//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;




namespace LayoutFarm.Text
{


    partial class EditableVisualElementLine
    {

        void AddNormalRunToLast(EditableTextSpan v)
        {
            EditableTextSpan.SetVisualElementAsChildOfOther(v, new VisualEditableLineParentLink(this, base.AddLast(v)));

#if DEBUG
#endif
            if ((this.lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0)
            {
                return;
            }

            v.StartBubbleUpLayoutInvalidState();

#if DEBUG
#endif

        }
        void AddNormalRunToFirst(EditableTextSpan v)
        {
            EditableTextSpan.SetVisualElementAsChildOfOther(v, new VisualEditableLineParentLink(this, base.AddFirst(v)));
            if ((this.lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0)
            {
                return;
            }
            v.StartBubbleUpLayoutInvalidState();

        }

        static LinkedListNode<EditableTextSpan> GetLineLinkedNode(EditableTextSpan ve)
        {
            return ((VisualEditableLineParentLink)ve.ParentLink).internalLinkedNode;
        }
        void AddNormalRunBefore(EditableTextSpan beforeVisualElement, EditableTextSpan v)
        {
            EditableTextSpan.SetVisualElementAsChildOfOther(v,
               new VisualEditableLineParentLink(this, base.AddBefore(GetLineLinkedNode(beforeVisualElement), v)));
            if ((this.lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0)
            {
                return;
            }
            v.StartBubbleUpLayoutInvalidState();

        }
        void AddNormalRunAfter(EditableTextSpan afterVisualElement, EditableTextSpan v)
        {
            EditableTextSpan.SetVisualElementAsChildOfOther(v,
             new VisualEditableLineParentLink(this,
                 base.AddAfter(GetLineLinkedNode(afterVisualElement), v)));

            if ((this.lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0)
            {
                return;
            }

            v.StartBubbleUpLayoutInvalidState();

        }
        public new void Clear()
        {
            LinkedListNode<EditableTextSpan> curNode = this.First;
            while (curNode != null)
            {
                EditableTextSpan.ClearVisualElementInternalLinkedNode(curNode.Value);
                curNode = curNode.Next;
            }

            base.Clear();
        }

        public new void Remove(EditableTextSpan v)
        {
#if DEBUG
            if (v.IsLineBreak)
            {
                throw new NotSupportedException("not support line break");
            }
#endif

            if ((lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) == 0)
            {
                v.StartBubbleUpLayoutInvalidState();
            }
            base.Remove(GetLineLinkedNode(v));

            EditableTextSpan.ClearVisualElementInternalLinkedNode(v);


            if ((this.lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0)
            {
                return;
            }

            if (!this.EndWithLineBreak && this.Count == 0 && this.currentLineNumber > 0)
            {
                if (!editableFlowLayer.GetTextLine(currentLineNumber - 1).EndWithLineBreak)
                {
                    editableFlowLayer.Remove(currentLineNumber);
                }
            }
            else
            {
                var ownerVe = editableFlowLayer.OwnerRenderElement;
                if (ownerVe != null)
                {
                    RenderElement.InnerInvalidateLayoutAndStartBubbleUp(ownerVe);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}