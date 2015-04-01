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
            EditableTextSpan.SetParentLink(v, new VisualEditableLineParentLink(this, base.AddLast(v))); 
            //if ((this.lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0)
            //{
            //    return;
            //}


        }
        void AddNormalRunToFirst(EditableTextSpan v)
        {
            EditableTextSpan.SetParentLink(v, new VisualEditableLineParentLink(this, base.AddFirst(v)));
            
        }

        static LinkedListNode<EditableTextSpan> GetLineLinkedNode(EditableTextSpan ve)
        {
            return ((VisualEditableLineParentLink)ve.ParentLink).internalLinkedNode;
        }
        void AddNormalRunBefore(EditableTextSpan beforeVisualElement, EditableTextSpan v)
        {
            EditableTextSpan.SetParentLink(v,
               new VisualEditableLineParentLink(this, 
                   base.AddBefore(GetLineLinkedNode(beforeVisualElement), v)));
            

        }
        void AddNormalRunAfter(EditableTextSpan afterVisualElement, EditableTextSpan v)
        {
            EditableTextSpan.SetParentLink(v,
             new VisualEditableLineParentLink(this,
                 base.AddAfter(GetLineLinkedNode(afterVisualElement), v))); 
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