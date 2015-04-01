// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.Text
{


    partial class EditableTextLine
    {

        void AddNormalRunToLast(EditableTextSpan v)
        {

            v.SetInternalLinkedNode(base.AddLast(v), this);

        }
        void AddNormalRunToFirst(EditableTextSpan v)
        {
            v.SetInternalLinkedNode(base.AddFirst(v), this);
        }

        static LinkedListNode<EditableTextSpan> GetLineLinkedNode(EditableTextSpan ve)
        {
            return ve.internalLinkedNode;
        }
        void AddNormalRunBefore(EditableTextSpan beforeVisualElement, EditableTextSpan v)
        {
            v.SetInternalLinkedNode(base.AddBefore(GetLineLinkedNode(beforeVisualElement), v), this);
        }
        void AddNormalRunAfter(EditableTextSpan afterVisualElement, EditableTextSpan v)
        {
            v.SetInternalLinkedNode(base.AddAfter(GetLineLinkedNode(afterVisualElement), v), this);
        }
        public new void Clear()
        {
            LinkedListNode<EditableTextSpan> curNode = this.First;
            while (curNode != null)
            {
                EditableTextSpan.RemoveParentLink(curNode.Value);
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


            base.Remove(GetLineLinkedNode(v));

            EditableTextSpan.RemoveParentLink(v);


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
                //var ownerVe = editableFlowLayer.OwnerRenderElement;
                //if (ownerVe != null)
                //{
                //    RenderElement.InnerInvalidateLayoutAndStartBubbleUp(ownerVe);
                //}
                //else
                //{
                //    throw new NotSupportedException();
                //}
            }
        }
    }
}