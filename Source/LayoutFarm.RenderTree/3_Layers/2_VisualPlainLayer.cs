//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;
using System.Text;
namespace LayoutFarm
{
   
    public class VisualPlainLayer : VisualLayer
    {
        LinkedList<RenderElement> myElements = new LinkedList<RenderElement>();
        public event EventHandler CustomRearrangeContent;

        public VisualPlainLayer(RenderElement owner)
        {
            this.OwnerRenderElement = owner;
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

        public void AddChild(RenderElement visualElement)
        {

#if DEBUG
            if (visualElement.ParentLink != null)
            {

            }
#endif

            LinkedListNode<RenderElement> linkNode = myElements.AddLast(visualElement);
            RenderElement.SetVisualElementAsChildOfOther(visualElement,
                new SimpleLinkListParentLink(this, linkNode));
            //position of new visual element

        }
        public override void Clear()
        {
            this.myElements.Clear();
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



        public override bool PrepareDrawingChain(VisualDrawingChain chain)
        {
            return false;
        }
        public override void DrawChildContent(Canvas canvasPage, InternalRect updateArea)
        {
            if ((layerFlags & IS_LAYER_HIDDEN) != 0)
            {
                return;
            }

            this.BeginDrawingChildContent();
            foreach (RenderElement child in this.GetDrawingIter())
            {
                if (child.IntersectsWith(updateArea))
                {

                    int x = child.X;
                    int y = child.Y;

                    canvasPage.OffsetCanvasOrigin(x, y);
                    updateArea.Offset(-x, -y);
                    child.DrawToThisPage(canvasPage, updateArea);

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


        public override bool HitTestCore(HitPointChain hitChain)
        {
            if ((layerFlags & IS_LAYER_HIDDEN) == 0)
            {
                foreach (RenderElement ui in this.GetHitTestIter())
                {
                    if (ui.HitTestCore(hitChain))
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
                int e_desiredRight = visualElement.ElementDesiredRight;

                if (local_lineWidth < e_desiredRight)
                {
                    local_lineWidth = e_desiredRight;
                }
                int e_desiredBottom = visualElement.ElementDesiredBottom;
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
            vinv_IsInTopDownReArrangePhase = true;
#if DEBUG
            vinv_dbug_EnterLayerReArrangeContent(this);
#endif
            //this.BeginLayerLayoutUpdate();
            if (CustomRearrangeContent != null)
            {
                CustomRearrangeContent(this, EventArgs.Empty);
            }

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