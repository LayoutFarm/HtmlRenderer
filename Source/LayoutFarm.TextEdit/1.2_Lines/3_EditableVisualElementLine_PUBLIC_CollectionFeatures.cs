//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
namespace LayoutFarm.Text
{

    class VisualEditableLineParentLink : IParentLink
    {
        internal readonly LinkedListNode<EditableTextSpan> internalLinkedNode;
        EditableVisualElementLine ownerLine;
        internal VisualEditableLineParentLink(EditableVisualElementLine ownerLine, LinkedListNode<EditableTextSpan> linkNode)
        {
            this.internalLinkedNode = linkNode;
            this.ownerLine = ownerLine;
        }
        public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            return null;
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
        RootGraphic dbugVRoot
        {
            get
            {
                return RootGraphic.dbugCurrentGlobalVRoot;
            }
        }
#endif
        public RenderElement NotifyParentToInvalidate(out bool goToFinalExit
#if DEBUG
, RenderElement ve
#endif
)
        {
            RenderElement parentVisualElem = null;
            goToFinalExit = false;

            EditableVisualElementLine line = this.OwnerLine;
#if DEBUG
            dbugVRoot.dbug_PushLayoutTraceMessage(RootGraphic.dbugMsg_VisualElementLine_INVALIDATE_enter, ve);
#endif
            line.InvalidateLineLayout();
#if DEBUG
            dbugVRoot.dbug_PushLayoutTraceMessage(RootGraphic.dbugMsg_VisualElementLine_INVALIDATE_exit, ve);
#endif

            if (!line.IsLocalSuspendLineRearrange)
            {
                parentVisualElem = line.editableFlowLayer.InvalidateArrangement();
            }
            else
            {
#if DEBUG
                dbugVRoot.dbug_PushLayoutTraceMessage(RootGraphic.dbugMsg_VisualElementLine_OwnerFlowElementIsIn_SUSPEND_MODE_enter, ve);
#endif
                goToFinalExit = true;
            }
            return parentVisualElem;
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
        public RenderElement Next
        {
            get
            {
                LinkedListNode<EditableTextSpan> next = this.internalLinkedNode.Next;
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
        public RenderElement Prev
        {
            get
            {
                LinkedListNode<EditableTextSpan> prv = this.internalLinkedNode.Previous;
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
        public RenderElement ParentVisualElement
        {
            get
            {
                EditableTextFlowLayer ownerFlow = this.OwnerFlowLayer;
                if (ownerFlow != null)
                {
                    return ownerFlow.OwnerRenderElement;
                }
                else
                {
                    return null;
                }
            }
        }

        public void AdjustLocation(ref Point p)
        {
            p.Y += this.OwnerLine.LineTop;
        }
    }


    partial class EditableVisualElementLine
    {


        public new void AddLast(EditableTextSpan v)
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
        public new void AddFirst(EditableTextSpan v)
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
        public void AddBefore(EditableTextSpan beforeVisualElement, EditableTextSpan v)
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
        public void AddAfter(EditableTextSpan afterVisualElement, EditableTextSpan v)
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

        internal void UnsafeAddLast(EditableTextSpan run)
        {
            EditableTextSpan.SetParentLink(run, new VisualEditableLineParentLink(this, base.AddLast(run)));
        }
        internal void UnsafeAddFirst(EditableTextSpan run)
        {
            EditableTextSpan.SetParentLink(run, new VisualEditableLineParentLink(this, base.AddFirst(run)));
        }
        internal void UnsafeAddAfter(EditableTextSpan after, EditableTextSpan run)
        {
            EditableTextSpan.SetParentLink(run,
            new VisualEditableLineParentLink(this,
                base.AddAfter(GetLineLinkedNode(after), run)));
        }
        internal void UnsafeRemoveVisualElement(EditableTextSpan v)
        {
            base.Remove(GetLineLinkedNode(v));
        }

    }

}