//Apache2, 2014-2017, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{
    partial class RenderElement : IParentLink
    {
        internal LinkedListNode<RenderElement> internalLinkedNode;
        protected virtual bool _MayHasOverlapChild()
        {
            return true;
        }

        public IParentLink MyParentLink
        {
            get { return this.parentLink; }
        }


        RenderElement IParentLink.ParentRenderElement
        {
            //yes, because when this renderElement act as parentlink
            //it return itself as parent
            get { return this; }
        }
        void IParentLink.AdjustLocation(ref Point p)
        {
            //nothing
        }
        RenderElement IParentLink.FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            //called from child node
            if (this._MayHasOverlapChild())
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
            }
            return null;
        }

#if DEBUG
        string IParentLink.dbugGetLinkInfo()
        {
            return "";
        }
#endif
    }
}