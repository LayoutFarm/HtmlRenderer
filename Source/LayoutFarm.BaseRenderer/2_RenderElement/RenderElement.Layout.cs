// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm
{

    

    partial class RenderElement
    {
        internal static int GetLayoutSpecificDimensionType(RenderElement visualElement)
        {
            return visualElement.uiLayoutFlags & 0x3;
        }

        public bool HasCalculatedSize
        {
            get
            {
                return ((uiLayoutFlags & RenderElementConst.LAY_HAS_CALCULATED_SIZE) != 0);
            }
        }
        protected void MarkHasValidCalculateSize()
        {

            uiLayoutFlags |= RenderElementConst.LAY_HAS_CALCULATED_SIZE;
#if DEBUG
            this.dbug_ValidateRecalculateSizeEpisode++;
#endif
        }
        public bool IsInLayoutQueue
        {
            get
            {
                return (uiLayoutFlags & RenderElementConst.LY_IN_LAYOUT_QUEUE) != 0;
            }
            set
            {
                uiLayoutFlags = value ?
                      uiLayoutFlags | RenderElementConst.LY_IN_LAYOUT_QUEUE :
                      uiLayoutFlags & ~RenderElementConst.LY_IN_LAYOUT_QUEUE;
            }
        }
        bool IsInLayoutQueueChainUp
        {
            get
            {
                return (uiLayoutFlags & RenderElementConst.LY_IN_LAYOUT_QCHAIN_UP) != 0;
            }
            set
            {
                uiLayoutFlags = value ?
                   uiLayoutFlags | RenderElementConst.LY_IN_LAYOUT_QCHAIN_UP :
                   uiLayoutFlags & ~RenderElementConst.LY_IN_LAYOUT_QCHAIN_UP;
            }
        }
        internal void InvalidateLayoutAndStartBubbleUp()
        {
            MarkInvalidContentSize();
            MarkInvalidContentArrangement();
            if (this.parentLink != null)
            {
                StartBubbleUpLayoutInvalidState();
            }
        }
        public static void InnerInvalidateLayoutAndStartBubbleUp(RenderElement ve)
        {
            ve.InvalidateLayoutAndStartBubbleUp();
        }
        static RenderElement BubbleUpInvalidLayoutToTopMost(RenderElement ve)
        {

#if DEBUG
            RootGraphic dbugVRoot = ve.dbugVRoot;
#endif

            ve.MarkInvalidContentSize();

            if (ve.parentLink == null)
            {
#if DEBUG
                if (ve.IsTopWindow)
                {
                }

                dbugVRoot.dbug_PushLayoutTraceMessage(RootGraphic.dbugMsg_NO_OWNER_LAY);
#endif
                return null;
            }


            if (ve.rootGfx.LayoutQueueClearing)
            {
                return null;
            }
            else if (ve.rootGfx.TopWindowRenderBox.IsInLayoutQueue)
            {
                ve.IsInLayoutQueueChainUp = true;
                ve.rootGfx.AddToLayoutQueue(ve);
            }

#if DEBUG
            dbugVRoot.dbug_LayoutTraceBeginContext(RootGraphic.dbugMsg_E_CHILD_LAYOUT_INV_BUB_enter, ve);
#endif

            bool goFinalExit;
            RenderElement parentRenderElement = ve.parentLink.NotifyParentToInvalidate(out goFinalExit
#if DEBUG
,
ve
#endif
);


            if (!goFinalExit)
            {
                //if (parentVisualElem.NeedSystemCaret)
                //{
                //    parentVisualElem.MarkInvalidContentArrangement();
                //    parentVisualElem.IsInLayoutQueueChainUp = true;
                //    goto finalExit;
                //}
                //else if (parentVisualElem.ActAsFloatingWindow)
                //{
                //    parentVisualElem.MarkInvalidContentArrangement();
                //    parentVisualElem.IsInLayoutQueueChainUp = true;
                //    goto finalExit;
                //}
                //if (parentVisualElem.ActAsFloatingWindow)
                //{
                //    parentVisualElem.MarkInvalidContentArrangement();
                //    parentVisualElem.IsInLayoutQueueChainUp = true;
                //    goto finalExit;
                //}


                parentRenderElement.MarkInvalidContentSize();
                parentRenderElement.MarkInvalidContentArrangement();

                if (!parentRenderElement.IsInLayoutQueueChainUp
                    && !parentRenderElement.IsInLayoutQueue
                    && !parentRenderElement.IsInLayoutSuspendMode)
                {

                    parentRenderElement.IsInLayoutQueueChainUp = true;

                    RenderElement upper = BubbleUpInvalidLayoutToTopMost(parentRenderElement);

                    if (upper != null)
                    {
                        upper.IsInLayoutQueueChainUp = true;
                        parentRenderElement = upper;
                    }
                }
                else
                {
                    parentRenderElement.IsInLayoutQueueChainUp = true;
                }
            }

        finalExit:
#if DEBUG
            dbugVRoot.dbug_LayoutTraceEndContext(RootGraphic.dbugMsg_E_CHILD_LAYOUT_INV_BUB_exit, ve);
#endif

            return parentRenderElement;
        }
         

        public void StartBubbleUpLayoutInvalidState()
        {

#if DEBUG
            dbugVRoot.dbug_LayoutTraceBeginContext(RootGraphic.dbugMsg_E_LAYOUT_INV_BUB_FIRST_enter, this);
#endif


            RenderElement tobeAddToLayoutQueue = BubbleUpInvalidLayoutToTopMost(this);

            if (tobeAddToLayoutQueue != null
                && !tobeAddToLayoutQueue.IsInLayoutQueue)
            {
                this.rootGfx.AddToLayoutQueue(tobeAddToLayoutQueue);
            }

#if DEBUG
            dbugVRoot.dbug_LayoutTraceEndContext(RootGraphic.dbugMsg_E_LAYOUT_INV_BUB_FIRST_exit, this);
#endif

        }

        public bool NeedReCalculateContentSize
        {
            get
            {
                return (uiLayoutFlags & RenderElementConst.LAY_HAS_CALCULATED_SIZE) == 0;
            }
        }

        public int GetReLayoutState()
        {
            return (uiLayoutFlags >> (7 - 1)) & 0x3;
        }


        internal void MarkInvalidContentArrangement()
        {
            uiLayoutFlags &= ~RenderElementConst.LY_HAS_ARRANGED_CONTENT;
#if DEBUG

            this.dbug_InvalidateContentArrEpisode++;
            dbug_totalInvalidateContentArrEpisode++;
#endif
        }
        public void MarkInvalidContentSize()
        {
            uiLayoutFlags &= ~RenderElementConst.LAY_HAS_CALCULATED_SIZE;
#if DEBUG
            this.dbug_InvalidateRecalculateSizeEpisode++;
#endif
        }
        public void MarkValidContentArrangement()
        {

#if DEBUG
            this.dbug_ValidateContentArrEpisode++;
#endif
            this.IsInLayoutQueueChainUp = false;
            uiLayoutFlags |= RenderElementConst.LY_HAS_ARRANGED_CONTENT;
        }
        public bool NeedContentArrangement
        {
            get
            {
                return (uiLayoutFlags & RenderElementConst.LY_HAS_ARRANGED_CONTENT) == 0;
            }
        }
        internal bool FirstArrangementPass
        {

            get
            {
                return (propFlags & RenderElementConst.FIRST_ARR_PASS) != 0;
            }
            set
            {
                propFlags = value ?
                   propFlags | RenderElementConst.FIRST_ARR_PASS :
                   propFlags & ~RenderElementConst.FIRST_ARR_PASS;
            }
        }
    }
}