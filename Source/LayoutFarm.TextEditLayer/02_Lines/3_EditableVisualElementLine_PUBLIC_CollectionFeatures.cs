//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;




namespace LayoutFarm.Presentation.Text
{

    class VisualEditableLineParentLink : IVisualParentLink
    {
        internal readonly LinkedListNode<ArtEditableVisualTextRun> internalLinkedNode;
        EditableVisualElementLine ownerLine;
        internal VisualEditableLineParentLink(EditableVisualElementLine ownerLine, LinkedListNode<ArtEditableVisualTextRun> linkNode)
        {
            this.internalLinkedNode = linkNode;
            this.ownerLine = ownerLine;
        }
        public ArtVisualElement FindOverlapedChildElementAtPoint(ArtVisualElement afterThisChild, System.Drawing.Point point)
        {
            return null;
        }
        public void PerformLayout(VisualElementArgs vinv)
        {
        }
        
        public ArtVisualRootWindow GetWindowRoot()
        {
            return ownerLine.OwnerElement.WinRoot;
        }
        public bool MayHasOverlapChild
        {
            get
            {
                return false;
            }
        }
#if DEBUG
        public string dbugGetLinkInfo()
        {
            return "editable-link";
        }
      
#endif
        public ArtVisualElement NotifyParentToInvalidate(out bool goToFinalExit
#if DEBUG
, ArtVisualElement ve
#endif
)
        {
            ArtVisualElement parentVisualElem = null;
            goToFinalExit = false;

            EditableVisualElementLine line = this.OwnerLine;
#if DEBUG
            LayoutFarm.Presentation.dbugRootLog.dbug_PushLayoutTraceMessage(dbugRootLog.dbugMsg_VisualElementLine_INVALIDATE_enter, ve);
#endif
            line.InvalidateLineLayout();
#if DEBUG
            LayoutFarm.Presentation.dbugRootLog.dbug_PushLayoutTraceMessage(dbugRootLog.dbugMsg_VisualElementLine_INVALIDATE_exit, ve);
#endif

            if (!line.IsLocalSuspendLineRearrange)
            {
                parentVisualElem = line.editableFlowLayer.InvalidateArrangement();
            }
            else
            {
#if DEBUG
                LayoutFarm.Presentation.dbugRootLog.dbug_PushLayoutTraceMessage(dbugRootLog.dbugMsg_VisualElementLine_OwnerFlowElementIsIn_SUSPEND_MODE_enter, ve);
#endif
                goToFinalExit = true;
            }
            return parentVisualElem;
        }
        public void Unlink(ArtVisualElement ve)
        {
            this.OwnerLine.Remove((ArtEditableVisualTextRun)ve);
        }
        internal EditableTextFlowLayer OwnerFlowLayer
        {
            get
            {
                return this.ownerLine.editableFlowLayer;
            }
        }
        public EditableVisualElementLine OwnerLine
        {
            get
            {
                return (EditableVisualElementLine)(internalLinkedNode.List);
            }
        }
        public ArtVisualElement Next
        {
            get
            {
                LinkedListNode<ArtEditableVisualTextRun> next = this.internalLinkedNode.Next;
                if (next != null)
                {
                    return next.Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public ArtVisualElement Prev
        {
            get
            {
                LinkedListNode<ArtEditableVisualTextRun> prv = this.internalLinkedNode.Previous;
                if (prv != null)
                {
                    return prv.Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public ArtVisualElement ParentVisualElement
        {
            get
            {
                EditableTextFlowLayer ownerFlow = this.OwnerFlowLayer;
                if (ownerFlow != null)
                {
                    return ownerFlow.ownerVisualElement;
                }
                else
                {
                    return null;
                }
            }
        }
        
        public void AdjustParentLocation(ref System.Drawing.Point p)
        {
            p.Y += this.OwnerLine.LineTop;
        }
    }


    partial class EditableVisualElementLine
    {


        public new void AddLast(ArtEditableVisualTextRun v)
        {
            if (!v.IsLineBreak)
            {


                AddNormalRunToLast(v);

            }
            else
            {
                AddLineBreakAfter(this.LastRun);
            }
        }
        public new void AddFirst(ArtEditableVisualTextRun v)
        {
            if (!v.IsLineBreak)
            {
                AddNormalRunToFirst(v);
            }
            else
            {
                AddLineBreakBefore(this.FirstRun);
            }

        }
        public void AddBefore(ArtEditableVisualTextRun beforeVisualElement, ArtEditableVisualTextRun v)
        {
            if (!v.IsLineBreak)
            {
                AddNormalRunBefore(beforeVisualElement, v);
            }
            else
            {
                AddLineBreakBefore(beforeVisualElement);
            }

        }
        public void AddAfter(ArtEditableVisualTextRun afterVisualElement, ArtEditableVisualTextRun v)
        {
            if (!v.IsLineBreak)
            {
                AddNormalRunAfter(afterVisualElement, v);
            }
            else
            {
                AddLineBreakAfter(afterVisualElement);
            }
        }

        internal void UnsafeAddLast(ArtEditableVisualTextRun run)
        {
            ArtEditableVisualTextRun.SetVisualElementAsChildOfOther(run, new VisualEditableLineParentLink(this, base.AddLast(run)));
        }
        internal void UnsafeAddFirst(ArtEditableVisualTextRun run)
        {
            ArtEditableVisualTextRun.SetVisualElementAsChildOfOther(run, new VisualEditableLineParentLink(this, base.AddFirst(run)));
        }
        internal void UnsafeAddAfter(ArtEditableVisualTextRun after, ArtEditableVisualTextRun run)
        {
            ArtEditableVisualTextRun.SetVisualElementAsChildOfOther(run,
            new VisualEditableLineParentLink(this,
                base.AddAfter(GetLineLinkedNode(after), run)));
        }
        internal void UnsafeRemoveVisualElement(ArtEditableVisualTextRun v)
        {
            base.Remove(GetLineLinkedNode(v));
        }

    }

}