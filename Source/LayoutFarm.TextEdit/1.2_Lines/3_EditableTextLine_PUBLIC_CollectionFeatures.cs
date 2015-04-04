// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{

    partial class EditableTextLine : LayoutFarm.RenderBoxes.IParentLink
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
            run.SetInternalLinkedNode(base.AddLast(run), this);
        }
        internal void UnsafeAddFirst(EditableTextSpan run)
        {
            run.SetInternalLinkedNode(base.AddFirst(run), this);
        }
        internal void UnsafeAddAfter(EditableTextSpan after, EditableTextSpan run)
        {
            run.SetInternalLinkedNode(base.AddAfter(GetLineLinkedNode(after), run), this);
        }
        internal void UnsafeRemoveVisualElement(EditableTextSpan v)
        {
            base.Remove(GetLineLinkedNode(v));
        } 
        RenderElement RenderBoxes.IParentLink.ParentRenderElement
        {
            get { return this.OwnerFlowLayer.OwnerRenderElement; }
        } 
        void RenderBoxes.IParentLink.AdjustLocation(ref Point p)
        {
            p.Y += this.LineTop;
        }

        RenderElement RenderBoxes.IParentLink.FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            return null;
        }
#if DEBUG
        string RenderBoxes.IParentLink.dbugGetLinkInfo()
        {
            return "editable-link";
        }
#endif
    }

}