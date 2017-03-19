//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    partial class EditableTextLine : LayoutFarm.RenderBoxes.IParentLink
    {
        public new void AddLast(EditableRun v)
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
        public new void AddFirst(EditableRun v)
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
        public void AddBefore(EditableRun beforeVisualElement, EditableRun v)
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
        public void AddAfter(EditableRun afterVisualElement, EditableRun v)
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

        internal void UnsafeAddLast(EditableRun run)
        {
            run.SetInternalLinkedNode(base.AddLast(run), this);
        }
        internal void UnsafeAddFirst(EditableRun run)
        {
            run.SetInternalLinkedNode(base.AddFirst(run), this);
        }
        internal void UnsafeAddAfter(EditableRun after, EditableRun run)
        {
            run.SetInternalLinkedNode(base.AddAfter(GetLineLinkedNode(after), run), this);
        }
        internal void UnsafeRemoveVisualElement(EditableRun v)
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