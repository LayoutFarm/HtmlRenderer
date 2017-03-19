//Apache2, 2014-2017, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
{
    class PlainLayer : RenderElementLayer
    {
        LinkedList<RenderElement> myElements = new LinkedList<RenderElement>();
        public PlainLayer(RenderElement owner)
            : base(owner)
        {
        }
        public override IEnumerable<RenderElement> GetRenderElementReverseIter()
        {
            LinkedListNode<RenderElement> cur = myElements.Last;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.Previous;
            }
        }
        public override IEnumerable<RenderElement> GetRenderElementIter()
        {
            LinkedListNode<RenderElement> cur = myElements.First;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.Next;
            }
        }


        public void AddChild(RenderElement re)
        {
            re.internalLinkedNode = myElements.AddLast(re);
            RenderElement.SetParentLink(re, this.owner);
            re.InvalidateGraphics();
        }
        public void RemoveChild(RenderElement re)
        {
            myElements.Remove(re.internalLinkedNode);
            re.internalLinkedNode = null;
            var bounds = re.RectBounds;
            RenderElement.SetParentLink(re, null);
            RenderElement.InvalidateGraphicLocalArea(this.OwnerRenderElement, bounds);
        }
        public override void Clear()
        {
            //todo: clear all parent link 
            this.myElements.Clear();
            this.OwnerRenderElement.InvalidateGraphics();
        }
        IEnumerable<RenderElement> GetDrawingIter()
        {
            LinkedListNode<RenderElement> curNode = this.myElements.First;
            while (curNode != null)
            {
                yield return curNode.Value;
                curNode = curNode.Next;
            }
        }
        IEnumerable<RenderElement> GetHitTestIter()
        {
            LinkedListNode<RenderElement> curNode = this.myElements.Last;
            while (curNode != null)
            {
                yield return curNode.Value;
                curNode = curNode.Previous;
            }
        }

        public override void DrawChildContent(Canvas canvasPage, Rectangle updateArea)
        {
            if ((layerFlags & IS_LAYER_HIDDEN) != 0)
            {
                return;
            }

            this.BeginDrawingChildContent();
            foreach (RenderElement child in this.GetDrawingIter())
            {
                if (child.IntersectsWith(ref updateArea))
                {
                    int x = child.X;
                    int y = child.Y;
                    canvasPage.OffsetCanvasOrigin(x, y);
                    updateArea.Offset(-x, -y);
                    child.DrawToThisCanvas(canvasPage, updateArea);
                    canvasPage.OffsetCanvasOrigin(-x, -y);
                    updateArea.Offset(x, y);
                }
            }

            this.FinishDrawingChildContent();
        }
#if DEBUG
        public override void dbug_DumpElementProps(dbugLayoutMsgWriter writer)
        {
            writer.Add(new dbugLayoutMsg(
                this, this.ToString()));
            writer.EnterNewLevel();
            foreach (RenderElement child in this.GetDrawingIter())
            {
                child.dbug_DumpVisualProps(writer);
            }
            writer.LeaveCurrentLevel();
        }
#endif
        public override bool HitTestCore(HitChain hitChain)
        {
            if ((layerFlags & IS_LAYER_HIDDEN) == 0)
            {
                foreach (RenderElement renderE in this.GetHitTestIter())
                {
                    if (renderE.HitTestCore(hitChain))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        static Size ReCalculateContentSizeNoLayout(LinkedList<RenderElement> velist)
        {
            int local_lineWidth = 0;
            int local_lineHeight = 17;
            LinkedListNode<RenderElement> curNode = velist.First;
            while (curNode != null)
            {
                RenderElement visualElement = curNode.Value;
                if (!visualElement.HasCalculatedSize)
                {
                    visualElement.TopDownReCalculateContentSize();
                }
                int e_desiredRight = visualElement.Right;
                if (local_lineWidth < e_desiredRight)
                {
                    local_lineWidth = e_desiredRight;
                }
                int e_desiredBottom = visualElement.Bottom;
                if (local_lineHeight < e_desiredBottom)
                {
                    local_lineHeight = e_desiredBottom;
                }
                curNode = curNode.Next;
            }

            return new Size(local_lineWidth, local_lineHeight);
        }


        public override void TopDownReArrangeContent()
        {
            //vinv_IsInTopDownReArrangePhase = true;
#if DEBUG
            vinv_dbug_EnterLayerReArrangeContent(this);
#endif
            //this.BeginLayerLayoutUpdate();
            //if (CustomRearrangeContent != null)
            //{
            //    CustomRearrangeContent(this, EventArgs.Empty);
            //}

            //this.EndLayerLayoutUpdate();
#if DEBUG
            vinv_dbug_ExitLayerReArrangeContent();
#endif
        }
        public override void TopDownReCalculateContentSize()
        {
#if DEBUG

            vinv_dbug_EnterLayerReCalculateContent(this);
#endif

            SetPostCalculateLayerContentSize(ReCalculateContentSizeNoLayout(this.myElements));
#if DEBUG
            vinv_dbug_ExitLayerReCalculateContent();
#endif
        }
#if DEBUG
        public override string ToString()
        {
            return "plain layer " + "(L" + dbug_layer_id + this.dbugLayerState + ") postcal:" +
                this.PostCalculateContentSize.ToString() + " of " + this.OwnerRenderElement.dbug_FullElementDescription();
        }
#endif
    }
}