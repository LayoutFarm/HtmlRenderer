// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{

    partial class RenderElement : IParentLink
    {
        internal LinkedListNode<RenderElement> internalLinkedNode;
        internal RenderElement myParentRenderElement;
        protected virtual bool _MayHasOverlapChild()
        {
            return true;
        }
        bool IParentLink.MayHasOverlapChild
        {
            get { return _MayHasOverlapChild(); }
        }

        RenderElement IParentLink.ParentRenderElement
        {
            get { return this; }
        }

        void IParentLink.AdjustLocation(ref Point p)
        {
            //nothing
        }

        RenderElement IParentLink.FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            var child_internalLinkedNode = afterThisChild.internalLinkedNode;
            if (child_internalLinkedNode == null)
            {
                return null;
            }
            var curnode = child_internalLinkedNode.Previous;
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

        string IParentLink.dbugGetLinkInfo()
        {
            return "";
        }
    }
}