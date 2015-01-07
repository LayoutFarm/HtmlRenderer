// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;

using PixelFarm.Drawing;

namespace LayoutFarm
{

    public interface IParentLink
    {
        bool MayHasOverlapChild { get; }
        RenderElement ParentVisualElement { get; } 
        void AdjustLocation(ref Point p);

        RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point);
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
         
        public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            var curnode = internalLinkedNode.Previous;
            while (curnode != null)
            {
                var element = curnode.Value;
                if (element.Contains(point))
                {
                    return element;
                }
                curnode = curnode.Previous;
            }
            return null;
        }


        public bool MayHasOverlapChild
        {
            get
            {
                return true;
            }
        }

        public RenderElement ParentVisualElement
        {
            get
            {
                return this.ownerLayer.OwnerRenderElement;
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
        public void AdjustLocation(ref  Point p)
        {

        }

    }
}