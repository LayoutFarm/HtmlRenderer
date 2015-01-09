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
        //----------------------
        //rectangle boundary area 
        int b_top;
        int b_left;
        int b_width;
        int b_height;

        int uiLayoutFlags;

        public bool IntersectsWith(Rectangle r)
        {
            int left = this.b_left;

            if (((left <= r.Left) && (this.Right > r.Left)) ||
                ((left >= r.Left) && (left < r.Right)))
            {
                int top = this.b_top;
                if (((top <= r.Top) && (this.Bottom > r.Top)) ||
               ((top >= r.Top) && (top < r.Bottom)))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IntersectOnHorizontalWith(Rectangle r)
        {
            int left = this.b_left;

            if (((left <= r.Left) && (this.Right > r.Left)) ||
                ((left >= r.Left) && (left < r.Right)))
            {
                return true;
            }
            return false;
        }


        public Rectangle RectBounds
        {
            get
            {
                return new Rectangle(b_left, b_top, b_width, b_height);

            }
        }
        public Size Size
        {
            get
            {
                return new Size(b_width, b_height);
            }
        }
        public int X
        {
            get
            {
                return b_left;
            }
        }
        public virtual int BubbleUpX
        {
            get { return this.X; }
        }
        public virtual int BubbleUpY
        {
            get { return this.Y; }
        }
        public int Y
        {
            get
            {
                return b_top;
            }
        }
        public int Right
        {
            get
            {
                return b_left + b_width;
            }
        }
        public int Bottom
        {
            get
            {
                return b_top + b_height;
            }
        }
        public Point Location
        {
            get
            {
                return new Point(b_left, b_top);
            }
        }
        public int Width
        {
            get
            {
                return b_width;
            }
        }
        public int Height
        {
            get
            {
                return b_height;
            }
        }
        public Point GetGlobalLocation()
        {
            return GetGlobalLocationStatic(this);
        }
        static Point GetGlobalLocationStatic(RenderElement ui)
        {

            RenderElement parentVisualElement = ui.ParentVisualElement;
            if (parentVisualElement != null)
            {
                Point parentGlobalLocation = GetGlobalLocationStatic(parentVisualElement);
                ui.parentLink.AdjustLocation(ref parentGlobalLocation);

                if (parentVisualElement.MayHasViewport)
                {

                    return new Point(
                        ui.b_left + parentGlobalLocation.X - parentVisualElement.ViewportX,
                        ui.b_top + parentGlobalLocation.Y - parentVisualElement.ViewportY);
                }
                else
                {
                    return new Point(ui.b_left + parentGlobalLocation.X, ui.b_top + parentGlobalLocation.Y);
                }
            }
            else
            {
                return ui.Location;
            }
        }

        public bool Contains(Point testPoint)
        {
            return ((propFlags & RenderElementConst.HIDDEN) != 0) ?
                        false :
                        ContainPoint(testPoint.X, testPoint.Y);

        }
        public bool IsTestable
        {
            get { return ((this.propFlags & RenderElementConst.HIDDEN) == 0) && (this.parentLink != null); }
        }
        public bool HitTestCore(HitChain hitChain)
        {

            if ((propFlags & RenderElementConst.HIDDEN) != 0)
            {
                return false;
            }

            int testX;
            int testY;
            hitChain.GetTestPoint(out testX, out testY);
            if ((testY >= b_top && testY <= (b_top + b_height)
            && (testX >= b_left && testX <= (b_left + b_width))))
            {

                if (this.MayHasViewport)
                {
                    hitChain.OffsetTestPoint(
                        -b_left + this.ViewportX,
                        -b_top + this.ViewportY);
                }
                else
                {
                    hitChain.OffsetTestPoint(-b_left, -b_top);
                }

                hitChain.AddHitObject(this);

                if (this.MayHasChild)
                {
                    this.ChildrenHitTestCore(hitChain);
                }

                if (this.MayHasViewport)
                {
                    hitChain.OffsetTestPoint(
                            b_left - this.ViewportX,
                            b_top - this.ViewportY);
                }
                else
                {
                    hitChain.OffsetTestPoint(b_left, b_top);
                }

                if ((propFlags & RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS) != 0 &&
                    hitChain.TopMostElement == this)
                {
                    hitChain.RemoveCurrentHit();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {

                return false;
            }
        }

        public bool FindUnderlingSibling(LinkedList<RenderElement> foundElements)
        {
            //TODO: need?
            throw new NotSupportedException();
        }
        public bool ContainPoint(int x, int y)
        {
            return ((x >= b_left && x < Right) && (y >= b_top && y < Bottom));
        }

        public bool ContainRect(Rectangle r)
        {
            return r.Left >= b_left &&
                    r.Top >= b_top &&
                    r.Right <= b_left + b_width &&
                    r.Bottom <= b_top + b_height;
        }
        public bool ContainRect(int x, int y, int width, int height)
        {
            return x >= b_left &&
                    y >= b_top &&
                    x + width <= b_left + b_width &&
                    y + height <= b_top + b_height;
        }



        public int ElementDesiredWidth
        {
            get
            {
                return this.b_width;
            }
        }
        public int ElementDesiredRight
        {
            get
            {
                return b_left + this.ElementDesiredWidth;
            }
        }

        public int ElementDesiredBottom
        {
            get
            {
                return b_top + this.ElementDesiredHeight;

            }
        }
        public int ElementDesiredHeight
        {
            get
            {
                return b_height;
            }
        }


        public bool HasSpecificWidth
        {
            get
            {
                return ((uiLayoutFlags & RenderElementConst.LY_HAS_SPC_WIDTH) == RenderElementConst.LY_HAS_SPC_WIDTH);
            }
            set
            {
                uiLayoutFlags = value ?
                   uiLayoutFlags | RenderElementConst.LY_HAS_SPC_WIDTH :
                   uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_WIDTH;
            }
        }

        public bool HasSpecificHeight
        {
            get
            {
                return ((uiLayoutFlags & RenderElementConst.LY_HAS_SPC_HEIGHT) == RenderElementConst.LY_HAS_SPC_HEIGHT);
            }
            set
            {
                uiLayoutFlags = value ?
                    uiLayoutFlags | RenderElementConst.LY_HAS_SPC_HEIGHT :
                    uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_HEIGHT;
            }
        }
        public bool HasSpecificSize
        {
            get
            {
                return ((uiLayoutFlags & RenderElementConst.LY_HAS_SPC_SIZE) != 0);
            }
            set
            {
                uiLayoutFlags = value ?
                    uiLayoutFlags | RenderElementConst.LY_HAS_SPC_SIZE :
                    uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_SIZE;

            }
        }
        public static int GetLayoutSpecificDimensionType(RenderElement visualElement)
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
        public void InvalidateLayoutAndStartBubbleUp()
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
        static RenderElement BubbleUpInvalidLayoutToTopMost(RenderElement ve, TopWindowRenderBox topBox)
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

            if (topBox != null)
            {
                if (ve.rootGfx.LayoutQueueClearing)
                {
                    return null;
                }
                else if (topBox.IsInLayoutQueue)
                {
                    ve.IsInLayoutQueueChainUp = true;
                    topBox.AddToLayoutQueue(ve);
                }
            }
#if DEBUG
            dbugVRoot.dbug_LayoutTraceBeginContext(RootGraphic.dbugMsg_E_CHILD_LAYOUT_INV_BUB_enter, ve);
#endif

            bool goFinalExit;
            RenderElement parentVisualElem = ve.parentLink.NotifyParentToInvalidate(out goFinalExit
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


                parentVisualElem.MarkInvalidContentSize();
                parentVisualElem.MarkInvalidContentArrangement();

                if (!parentVisualElem.IsInLayoutQueueChainUp
                    && !parentVisualElem.IsInLayoutQueue
                    && !parentVisualElem.IsInLayoutSuspendMode)
                {

                    parentVisualElem.IsInLayoutQueueChainUp = true;

                    RenderElement upper = BubbleUpInvalidLayoutToTopMost(parentVisualElem, topBox);

                    if (upper != null)
                    {
                        upper.IsInLayoutQueueChainUp = true;
                        parentVisualElem = upper;
                    }
                }
                else
                {
                    parentVisualElem.IsInLayoutQueueChainUp = true;
                }
            }

        finalExit:
#if DEBUG
            dbugVRoot.dbug_LayoutTraceEndContext(RootGraphic.dbugMsg_E_CHILD_LAYOUT_INV_BUB_exit, ve);
#endif

            return parentVisualElem;
        }

        public TopWindowRenderBox GetTopWindowRenderBox()
        {
            if (parentLink == null)
            {
                if (this.IsTopWindow)
                {
                    return (TopWindowRenderBox)this;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return parentLink.ParentVisualElement.GetTopWindowRenderBox();
            }
        }

        public void StartBubbleUpLayoutInvalidState()
        {

#if DEBUG
            dbugVRoot.dbug_LayoutTraceBeginContext(RootGraphic.dbugMsg_E_LAYOUT_INV_BUB_FIRST_enter, this);
#endif

            TopWindowRenderBox topWinBox = this.GetTopWindowRenderBox();
            RenderElement tobeAddToLayoutQueue = BubbleUpInvalidLayoutToTopMost(this, topWinBox);

            if (tobeAddToLayoutQueue != null
                && topWinBox != null
                && !tobeAddToLayoutQueue.IsInLayoutQueue)
            {
                topWinBox.AddToLayoutQueue(tobeAddToLayoutQueue);
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
#if DEBUG
        public bool dbugNeedReCalculateContentSize
        {
            get
            {
                return this.NeedReCalculateContentSize;
            }
        }
#endif
        public int GetReLayoutState()
        {
            return (uiLayoutFlags >> (7 - 1)) & 0x3;
        }


        public void MarkInvalidContentArrangement()
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
#if DEBUG
        public bool dbugNeedContentArrangement
        {
            get
            {
                return this.NeedContentArrangement;
            }
        }
#endif
    }
}