//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;

namespace LayoutFarm.Presentation
{
    public enum ParentLinkRelocatePosition
    {
        TopMost,
        First,
        Last,
        After,
        Before
    }
  

    public interface IParentLink
    {

        void Unlink(RenderElement ve);
       
        bool MayHasOverlapChild { get; }
        RenderElement ParentVisualElement { get; }

        
        void AdjustParentLocation(ref System.Drawing.Point p);
        void PerformLayout(VisualElementArgs vinv);
        TopWindowRenderBox GetWindowRoot();
        RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, System.Drawing.Point point);
        RenderElement NotifyParentToInvalidate(out bool goToFinalExit

#if DEBUG
, RenderElement ve
#endif
);

#if DEBUG
        string dbugGetLinkInfo();
#endif

    }


    class SimpleLinkListParentLink : IParentLink
    {
        public readonly LinkedListNode<RenderElement> internalLinkedNode;
        VisualPlainLayer ownerLayer;
        public SimpleLinkListParentLink(VisualPlainLayer ownerLayer,
            LinkedListNode<RenderElement> internalLinkedNode)
        {
            this.ownerLayer = ownerLayer;
            this.internalLinkedNode = internalLinkedNode;
        }
        public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, System.Drawing.Point point)
        {
            var curnode = internalLinkedNode.Previous;
            while (curnode != null)
            {
                var element = curnode.Value;
                if (element.HitTestCoreNoRecursive(point))
                {
                    return element;
                }
                curnode = curnode.Previous;
            }
            return null;
        }
        public TopWindowRenderBox GetWindowRoot()
        {
            return this.ownerLayer.GetWindowRoot();
        }
        public void PerformLayout(VisualElementArgs vinv)
        {
            ownerLayer.TopDownReArrangeContent(vinv); 
        } 
        public bool MayHasOverlapChild
        {
            get
            {
                return true;
            }
        }
        public void Unlink(RenderElement ve)
        {
            (internalLinkedNode.List).Remove(ve);
        }
        
        public RenderElement ParentVisualElement
        {
            get
            {
                return this.ownerLayer.ownerVisualElement;
            }
        }

#if DEBUG
        public string dbugGetLinkInfo()
        {
            return ownerLayer.ToString();
        }
#endif
        public RenderElement NotifyParentToInvalidate(out bool goToFinalExit

#if DEBUG
, RenderElement ve
#endif
)
        {
            goToFinalExit = false;
            return ownerLayer.InvalidateArrangement();
        }
        public void AdjustParentLocation(ref System.Drawing.Point p)
        {

        }
        
    }
}