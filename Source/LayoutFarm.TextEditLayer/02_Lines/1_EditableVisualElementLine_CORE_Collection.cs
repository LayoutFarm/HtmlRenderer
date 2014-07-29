using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;




namespace LayoutFarm.Presentation.Text
{

        
    partial class EditableVisualElementLine
    {

                                        void AddNormalRunToLast(ArtEditableVisualTextRun v)
        {
                                    ArtEditableVisualTextRun.SetVisualElementAsChildOfOther(v, new VisualEditableLineParentLink(this, base.AddLast(v)));

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
                                        void AddNormalRunToFirst(ArtEditableVisualTextRun v)
        {
            ArtEditableVisualTextRun.SetVisualElementAsChildOfOther(v, new VisualEditableLineParentLink(this, base.AddFirst(v)));
            if ((this.lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0)
            {
                return;
            }
            v.StartBubbleUpLayoutInvalidState();

        }

        static LinkedListNode<ArtEditableVisualTextRun> GetLineLinkedNode(ArtEditableVisualTextRun ve)
        {
            return ((VisualEditableLineParentLink)ve.MyParentLink).internalLinkedNode;
                    }
                                                void AddNormalRunBefore(ArtEditableVisualTextRun beforeVisualElement, ArtEditableVisualTextRun v)
        {
            ArtEditableVisualTextRun.SetVisualElementAsChildOfOther(v,
               new VisualEditableLineParentLink(this, base.AddBefore(GetLineLinkedNode(beforeVisualElement), v)));
            if ((this.lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0)
            {
                                                                return;
            }
            v.StartBubbleUpLayoutInvalidState();

        }
                                                void AddNormalRunAfter(ArtEditableVisualTextRun afterVisualElement, ArtEditableVisualTextRun v)
        {
            ArtEditableVisualTextRun.SetVisualElementAsChildOfOther(v,
             new VisualEditableLineParentLink(this, base.AddAfter(GetLineLinkedNode(afterVisualElement), v)));

            if ((this.lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0)
            {
                return;
            }

            v.StartBubbleUpLayoutInvalidState();

        }
                                public new void Clear()
        {
            LinkedListNode<ArtEditableVisualTextRun> curNode = this.First;
            while (curNode != null)
            {
                ArtEditableVisualTextRun.ClearVisualElementInternalLinkedNode(curNode.Value);
                curNode = curNode.Next;
            }

                        base.Clear();
        }
        
                                        public new void Remove(ArtEditableVisualTextRun v)
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

            ArtEditableVisualTextRun.ClearVisualElementInternalLinkedNode(v);


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
                                                var ownerVe = editableFlowLayer.ownerVisualElement;
                if (ownerVe != null)
                {
                    ArtVisualElement.InnerInvalidateLayoutAndStartBubbleUp(ownerVe);                     
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}