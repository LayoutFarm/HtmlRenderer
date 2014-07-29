//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace LayoutFarm.Presentation
{

    public class VisualPlainLayer : VisualLayer
    {

        public event EventHandler ReArrangeContentRequest;

        LinkedList<ArtVisualElement> myElements = new LinkedList<ArtVisualElement>();
        public VisualPlainLayer(ArtVisualContainerBase owner)
            : base(owner)
        {
        }
        public ArtVisualRootWindow GetWindowRoot()
        {
            if (this.ownerVisualElement == null)
            {
                return null;
            }
            if (this.ownerVisualElement.IsWindowRoot)
            {
                return (ArtVisualRootWindow)this.ownerVisualElement;
            }
            else
            {
                return this.WinRoot;
            }
        }

        public override IEnumerable<ArtVisualElement> GetVisualElementReverseIter()
        {
            LinkedListNode<ArtVisualElement> cur = myElements.Last;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.Previous;
            }
        }
        public override IEnumerable<ArtVisualElement> GetVisualElementIter()
        {
            LinkedListNode<ArtVisualElement> cur = myElements.First;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.Next;
            }
        }

        public override void AddTop(ArtVisualElement visualElement)
        {
#if DEBUG
            if (visualElement.ParentLink != null)
            {

            }
#endif

            LinkedListNode<ArtVisualElement> linkNode = myElements.AddLast(visualElement);
            ArtVisualElement.SetVisualElementAsChildOfSimpleContainer(visualElement,
                new SimpleLinkListParentLink(this, linkNode));


        }
        public override void Clear()
        {
            this.myElements.Clear();
        }


        public override IEnumerable<ArtVisualElement> GetDrawingIter()
        {

            LinkedListNode<ArtVisualElement> curNode = this.myElements.First;
            while (curNode != null)
            {
                yield return curNode.Value;
                curNode = curNode.Next;
            }

        }
        IEnumerable<ArtVisualElement> GetHitTestIter()
        {

            LinkedListNode<ArtVisualElement> curNode = this.myElements.Last;
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
        public override void DrawChildContent(ArtCanvas canvasPage, InternalRect updateArea)
        {
            if ((layerFlags & IS_LAYER_HIDDEN) != 0)
            {
                return;
            }

            this.BeginDrawingChildContent();
            foreach (ArtVisualElement child in this.GetDrawingIter())
            {

                if (child.IntersectsWith(updateArea))
                {

                    int x = child.X;
                    int y = child.Y;

                    canvasPage.OffsetCanvasOrigin(x, y); updateArea.Offset(-x, -y);
                    child.DrawToThisPage(canvasPage, updateArea);

                    canvasPage.OffsetCanvasOrigin(-x, -y); updateArea.Offset(x, y);
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

            foreach (ArtVisualElement child in this.GetDrawingIter())
            {
                child.dbug_DumpVisualProps(writer);
            }
            writer.LeaveCurrentLevel();

        }
#endif


        public override bool HitTestCore(ArtHitPointChain artHitResult)
        {
            if ((layerFlags & IS_LAYER_HIDDEN) == 0)
            {

                foreach (ArtVisualElement ui in this.GetHitTestIter())
                {

                    if (ui.HitTestCore(artHitResult))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        static Size ReCalculateContentSizeNoLayout(LinkedList<ArtVisualElement> velist, VisualElementArgs vinv)
        {
            int local_lineWidth = 0;
            int local_lineHeight = 17; LinkedListNode<ArtVisualElement> curNode = velist.First;

            while (curNode != null)
            {
                ArtVisualElement visualElement = curNode.Value;
                if (!visualElement.HasCalculatedSize)
                {
                    visualElement.TopDownReCalculateContentSize(vinv);
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


        public override void TopDownReArrangeContent(VisualElementArgs vinv)
        {
            vinv.IsInTopDownReArrangePhase = true;
#if DEBUG
            vinv.dbug_EnterLayerReArrangeContent(this);
#endif
            this.BeginLayerGraphicUpdate(vinv);



            this.EndLayerGraphicUpdate(vinv);
#if DEBUG
            vinv.dbug_ExitLayerReArrangeContent();
#endif
        }
        void PerformNoLayoutArrange(VisualElementArgs vinv)
        {


            Size ownerSize = this.ownerVisualElement.Size;
            this.ValidateArrangement();


            int j = this.myElements.Count;
            LinkedListNode<ArtVisualElement> curNode = this.myElements.First;
            while (curNode != null)
            {

                ArtVisualContainerBase containerBase = curNode.Value as ArtVisualContainerBase;
                if (containerBase == null)
                {
                    curNode = curNode.Next;
                    continue;
                }


                int v_desired_width = containerBase.ElementDesiredWidth;
                int v_desired_height = containerBase.ElementDesiredHeight;


                int x_pos = 0;
                int y_pos = 0;




                if (!ArtVisualElement.IsSizeEqualTo(containerBase, v_desired_width, v_desired_height))
                {

                    if (containerBase.CanbeFloatingWindow)
                    {
                        if (v_desired_width > 600)
                        {
                            v_desired_width = 600;
                        }
                        if (v_desired_height > 300)
                        {
                            v_desired_height = 300;
                        }
                    }
                    if (!containerBase.HasSpecificSize)
                    {
                        ArtVisualElement.DirectSetVisualElementSizeAndDoTopDownArrangement(containerBase,
                            v_desired_width, v_desired_height, vinv);
                    }

                }

                containerBase.TopDownReArrangeContentIfNeed(vinv);

                curNode = curNode.Next;
            }

            if (this.ReArrangeContentRequest != null)
            {
                this.ReArrangeContentRequest(this, EventArgs.Empty);
            }
        }



























































        public override void TopDownReArrangeContentToFit(VisualElementArgs vinv)
        {

        }
        void PreformVerticalStackReArrangeContentToFit(VisualElementArgs vinv)
        {



            int ecount = this.myElements.Count;
            if (ecount == 0)
            {
                return;
            }
            int heightdiff = this.ownerVisualElement.Height - this.PostCalculateContentSize.Height;
            if (heightdiff > 0)
            {
                List<ArtVisualElement> candidateList = new List<ArtVisualElement>();
                LinkedListNode<ArtVisualElement> cNode = this.myElements.First;
                while (cNode != null)
                {

                    ArtVisualElement ve = cNode.Value;
                    if (ve.IsStretchable)
                    {
                        candidateList.Add(ve);
                    }
                    cNode = cNode.Next;
                }

                int j = candidateList.Count;
                if (j > 0)
                {
                    int eachheightPart = heightdiff / j;
                    if (eachheightPart > 1)
                    {
                        for (int i = j - 1; i > -1; --i)
                        {
                            ArtVisualElement candidateVe = candidateList[i];
                            candidateVe.SetHeight(
                                candidateVe.Height + eachheightPart, vinv);
                        }
                    }
                }
            }
        }











































        public override void TopDownReCalculateContentSize(VisualElementArgs vinv)
        {


#if DEBUG

            vinv.dbug_EnterLayerReCalculateContent(this);
#endif

            SetPostCalculateLayerContentSize(ReCalculateContentSizeNoLayout(this.myElements, vinv));










#if DEBUG
            vinv.dbug_ExitLayerReCalculateContent();
#endif
        }




#if DEBUG
        public override string ToString()
        {

            return "flow layer " + "(L" + dbug_layer_id + this.dbugLayerState + ") postcal:" +
                this.PostCalculateContentSize.ToString() + " of " + ownerVisualElement.dbug_FullElementDescription();
        }
#endif


    }


}