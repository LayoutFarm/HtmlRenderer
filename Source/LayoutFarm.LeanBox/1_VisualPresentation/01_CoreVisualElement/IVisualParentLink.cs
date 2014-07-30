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
  

    public interface IVisualParentLink
    {

        void Unlink(ArtVisualElement ve);
       
        bool MayHasOverlapChild { get; }
        ArtVisualElement ParentVisualElement { get; }

        
        void AdjustParentLocation(ref System.Drawing.Point p);
        void PerformLayout(VisualElementArgs vinv);
        ArtVisualRootWindow GetWindowRoot();
        ArtVisualElement FindOverlapedChildElementAtPoint(ArtVisualElement afterThisChild, System.Drawing.Point point);
        ArtVisualElement NotifyParentToInvalidate(out bool goToFinalExit

#if DEBUG
, ArtVisualElement ve
#endif
);

#if DEBUG
        string dbugGetLinkInfo();
#endif

    }


    class SimpleLinkListParentLink : IVisualParentLink
    {
        public readonly LinkedListNode<ArtVisualElement> internalLinkedNode;
        VisualPlainLayer ownerLayer;
        public SimpleLinkListParentLink(VisualPlainLayer ownerLayer,
            LinkedListNode<ArtVisualElement> internalLinkedNode)
        {
            this.ownerLayer = ownerLayer;
            this.internalLinkedNode = internalLinkedNode;
        }
        public ArtVisualElement FindOverlapedChildElementAtPoint(ArtVisualElement afterThisChild, System.Drawing.Point point)
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
        public ArtVisualRootWindow GetWindowRoot()
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
        public void Unlink(ArtVisualElement ve)
        {
            (internalLinkedNode.List).Remove(ve);
        }
        
        public ArtVisualElement ParentVisualElement
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
        public ArtVisualElement NotifyParentToInvalidate(out bool goToFinalExit

#if DEBUG
, ArtVisualElement ve
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