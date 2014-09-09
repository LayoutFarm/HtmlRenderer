//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using LayoutFarm.Presentation;


namespace LayoutFarm.Presentation
{


    partial class ArtVisualElement
    {
        int uiTop;
        int uiLeft; 
        int uiWidth;
        int uiHeight;


        public bool IntersectsWith(InternalRect r)
        {
            int left = this.uiLeft;

            if (((left <= r._left) && (this.Right > r._left)) ||
                ((left >= r._left) && (left < r._right)))
            {
                int top = this.uiTop;
                if (((top <= r._top) && (this.Bottom > r._top)) ||
               ((top >= r._top) && (top < r._bottom)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IntersectsWith(Rectangle r)
        {
            int left = this.uiLeft;

            if (((left <= r.Left) && (this.Right > r.Left)) ||
                ((left >= r.Left) && (left < r.Right)))
            {
                int top = this.uiTop;
                if (((top <= r.Top) && (this.Bottom > r.Top)) ||
               ((top >= r.Top) && (top < r.Bottom)))
                {
                    return true;
                }
            }
            return false;
        }



        public bool IntersectOnHorizontalWith(InternalRect r)
        {
            int left = this.uiLeft;

            if (((left <= r._left) && (this.Right > r._left)) ||
                ((left >= r._left) && (left < r._right)))
            {
                return true;
            }
            return false;
        }





















        protected Rectangle GetLocalArea()
        {
            return new Rectangle(0, 0, uiWidth, uiHeight);
        }

        public Rectangle Rect
        {
            get
            {
                return new Rectangle(uiLeft, uiTop, uiWidth, uiHeight);

            }
        }
        public Size Size
        {
            get
            {
                return new Size(uiWidth, uiHeight);
            }
        }
        public int X
        {
            get
            {
                return uiLeft;
            }

        }
        public int Y
        {
            get
            {
                return uiTop;
            }

        }
        public int Right
        {
            get
            {
                return uiLeft + uiWidth;
            }
        }
        public int Bottom
        {
            get
            {
                return uiTop + uiHeight;
            }
        }
        public Point Location
        {
            get
            {
                return new Point(uiLeft, uiTop);
            }
        }
        public int Width
        {
            get
            {
                return uiWidth;
            }
        }
        public int Height
        {
            get
            {
                return uiHeight;
            }
        }





        public Point GetGlobalLocation()
        {

            return GetGlobalLocationStatic(this);
        }
        public Point GetLocationLimitTo(ArtVisualElement parentHint)
        {
            ArtVisualElement parentVisualElement = this.ParentVisualElement;
            if (parentVisualElement == parentHint)
            {
                return new Point(this.uiLeft, this.uiTop);
            }
            else
            {
                if (parentVisualElement != null)
                {
                    Point parentPos = parentVisualElement.GetLocationLimitTo(parentHint); return new Point(uiLeft + parentPos.X, uiTop + parentPos.Y);
                }
                else
                {
                    return new Point(uiLeft, uiTop);
                }
            }
        }
        public Point GetGlobalLocationRelativeTo(ArtVisualElement relativeElement)
        {

            Point relativeElemLoca = relativeElement.Location;
            Point relativeElementGlobalLocation = relativeElement.GetGlobalLocation();
            relativeElementGlobalLocation.Offset(
               uiLeft - relativeElemLoca.X, uiTop - relativeElemLoca.Y); return relativeElementGlobalLocation;
        }
        public Point GetLocationAsChildOf(ArtVisualElement relativeElement)
        {
            Point relativeElementGlobalLocation = relativeElement.GetGlobalLocation();
            Point thisGlobalLoca = GetGlobalLocation();
            return new Point(thisGlobalLoca.X - relativeElementGlobalLocation.X, thisGlobalLoca.Y - relativeElementGlobalLocation.Y);
        }
        public Point GetLocationAsSiblingOf(ArtVisualElement relativeElement)
        {
            ArtVisualElement parent = relativeElement.ParentVisualElement;
            return GetLocationAsChildOf(parent);
        }
        public Rectangle GetGlobalRect()
        {
            return new Rectangle(GetGlobalLocationStatic(this), Size);
        }

        static Point GetGlobalLocationStatic(ArtVisualElement ui)
        {

            ArtVisualElement parentVisualElement = ui.ParentVisualElement;
            if (parentVisualElement != null)
            {
                Point parentGlobalLocation = GetGlobalLocationStatic(parentVisualElement);
                ui.visualParentLink.AdjustParentLocation(ref parentGlobalLocation);

                if (parentVisualElement.IsVisualContainerBase)
                {
                    ArtVisualContainerBase parentAsContainerBase = (ArtVisualContainerBase)parentVisualElement;
                    return new Point(ui.uiLeft + parentGlobalLocation.X - parentAsContainerBase.ViewportX,
                        ui.uiTop + parentGlobalLocation.Y - parentAsContainerBase.ViewportY);
                }
                else
                {
                    return new Point(ui.uiLeft + parentGlobalLocation.X, ui.uiTop + parentGlobalLocation.Y);
                }
            }
            else
            {
                return ui.Location;
            }
        }

        public bool HitTestCoreNoRecursive(Point testPoint)
        {
            if ((uiFlags & HIDDEN) != 0)
            {
                return false;
            }
            return ContainPoint(testPoint.X, testPoint.Y);
        }
        public bool HitTestCore(ArtHitPointChain artHitResult)
        {

            if ((uiFlags & HIDDEN) != 0)
            {
                return false;
            }
            switch (this.ElementNature)
            {

                case VisualElementNature.CssBox:
                    {
                        throw new NotSupportedException();
                    }
                case VisualElementNature.Shapes: 
                case VisualElementNature.HtmlContainer:
                default:
                    {
                        int testX;
                        int testY;
                        artHitResult.GetTestPoint(out testX, out testY);
                        if ((testY >= uiTop && testY <= (uiTop + uiHeight)
                        && (testX >= uiLeft && testX <= (uiLeft + uiWidth))))
                        {
                            ArtVisualContainerBase scContainer = null;
                            if (this.IsScrollable)
                            {
                                scContainer = (ArtVisualContainerBase)this;
                                artHitResult.OffsetTestPoint(-uiLeft + scContainer.ViewportX,
                                    -uiTop + scContainer.ViewportY);
                            }
                            else
                            {
                                artHitResult.OffsetTestPoint(-uiLeft, -uiTop);
                            }

                            artHitResult.AddHit(this);

                            if (this.IsVisualContainerBase)
                            {
                                ((ArtVisualContainerBase)this).ChildrenHitTestCore(artHitResult);
                            }
                            if (this.IsScrollable)
                            {
                                artHitResult.OffsetTestPoint(
                                        uiLeft - scContainer.ViewportX,
                                        uiTop - scContainer.ViewportY);
                            }
                            else
                            {
                                artHitResult.OffsetTestPoint(uiLeft, uiTop);
                            }

                            if ((uiFlags & TRANSPARENT_FOR_ALL_EVENTS) != 0 && artHitResult.CurrentHitElement == this)
                            {
                                artHitResult.RemoveHit(artHitResult.CurrentHitNode);
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
            }

        }

        public bool FindUnderlingSibling(LinkedList<ArtVisualElement> foundElements)
        {
            throw new NotSupportedException();



        }
        public bool ContainPoint(int x, int y)
        {
            return ((x >= uiLeft && x < Right) && (y >= uiTop && y < Bottom));
        }
        public bool ContainPoint(Point p)
        {
            return ContainPoint(p.X, p.Y);
        }
        public bool ContainRect(InternalRect testRect)
        {
            return testRect._left >= uiLeft &&
testRect._top >= uiTop &&
testRect._right <= uiLeft + uiWidth &&
testRect._bottom <= uiTop + uiHeight;
        }
        public bool ContainRect(Rectangle r)
        {
            return r.Left >= uiLeft &&
r.Top >= uiTop &&
r.Right <= uiLeft + uiWidth &&
r.Bottom <= uiTop + uiHeight;
        }
        public bool ContainRect(int x, int y, int width, int height)
        {
            return x >= uiLeft &&
y >= uiTop &&
x + width <= uiLeft + uiWidth &&
y + height <= uiTop + uiHeight;
        }








        int uiLayoutFlags;
        public const int LY_HAS_SPC_WIDTH = 1 << (1 - 1);
        public const int LY_HAS_SPC_HEIGHT = 1 << (2 - 1);
        public const int LY_HAS_SPC_SIZE = LY_HAS_SPC_WIDTH | LY_HAS_SPC_HEIGHT;
        public const int LY_REACH_MIN_WIDTH = 1 << (3 - 1);
        public const int LY_REACH_MAX_WIDTH = 1 << (4 - 1);
        public const int LY_REACH_MIN_HEIGHT = 1 << (5 - 1);
        public const int LY_REACH_MAX_HEIGHT = 1 << (6 - 1);
        public const int LY_HAS_ARRANGED_CONTENT = 1 << (7 - 1);
        public const int LAY_HAS_CALCULATED_SIZE = 1 << (8 - 1);
        public const int LY_SUSPEND = 1 << (9 - 1);

        public const int LY_SUSPEND_GRAPHIC = 1 << (12 - 1);
        public const int LY_IN_LAYOUT_QUEUE = 1 << (13 - 1);
        public const int LY_IN_LAYOUT_QCHAIN_UP = 1 << (10 - 1);


        public int ElementDesiredWidth
        {
            get
            {
                return this.uiWidth;
            }
        }
        public int ElementDesiredRight
        {
            get
            {
                return uiLeft + this.ElementDesiredWidth;
            }
        }

        public int ElementDesiredBottom
        {
            get
            {
                return uiTop + this.ElementDesiredHeight;

            }
        }
        public int ElementDesiredHeight
        {
            get
            {
                return uiHeight; 
            }
        }


        public bool HasSpecificWidth
        {
            get
            {
                return ((uiLayoutFlags & LY_HAS_SPC_WIDTH) == LY_HAS_SPC_WIDTH);
            }
            set
            {

                if (value)
                {
                    uiLayoutFlags |= LY_HAS_SPC_WIDTH;
                }
                else
                {
                    uiLayoutFlags &= ~LY_HAS_SPC_WIDTH;
                }
            }
        }

        public bool HasSpecificHeight
        {
            get
            {
                return ((uiLayoutFlags & LY_HAS_SPC_HEIGHT) == LY_HAS_SPC_HEIGHT);
            }
            set
            {

                if (value)
                {
                    uiLayoutFlags |= LY_HAS_SPC_HEIGHT;
                }
                else
                {
                    uiLayoutFlags &= ~LY_HAS_SPC_HEIGHT;
                }
            }
        }
        public bool HasSpecificSize
        {
            get
            {
                return ((uiLayoutFlags & LY_HAS_SPC_SIZE) != 0);
            }
            set
            {
                if (value)
                {
                    uiLayoutFlags |= LY_HAS_SPC_SIZE;
                }
                else
                {
                    uiLayoutFlags &= ~LY_HAS_SPC_SIZE;
                }
            }
        }
        public static int GetLayoutSpecificDimensionType(ArtVisualElement visualElement)
        {
            return visualElement.uiLayoutFlags & 0x3;
        }





        public bool HasCalculatedSize
        {
            get
            {
                return ((uiLayoutFlags & LAY_HAS_CALCULATED_SIZE) != 0);
            }
        }
        protected void MarkHasValidCalculateSize()
        {

            uiLayoutFlags |= LAY_HAS_CALCULATED_SIZE;
#if DEBUG
            this.dbug_ValidateRecalculateSizeEpisode++;
#endif
        }



        public bool IsInLayoutQueue
        {
            get
            {
                return (uiLayoutFlags & LY_IN_LAYOUT_QUEUE) != 0;
            }
            set
            {

                if (value)
                {
                    uiLayoutFlags |= LY_IN_LAYOUT_QUEUE;
                }
                else
                {
                    uiLayoutFlags &= ~LY_IN_LAYOUT_QUEUE;
                }
            }
        }
        bool IsInLayoutQueueChainUp
        {
            get
            {
                return (uiLayoutFlags & LY_IN_LAYOUT_QCHAIN_UP) != 0;
            }
            set
            {

                if (value)
                {
                    uiLayoutFlags |= LY_IN_LAYOUT_QCHAIN_UP;
                }
                else
                {
                    uiLayoutFlags &= ~LY_IN_LAYOUT_QCHAIN_UP;
                }
            }
        }
        public void InvalidateLayoutAndStartBubbleUp()
        {
            MarkInvalidContentSize();
            MarkInvalidContentArrangement();
            if (this.visualParentLink != null)
            {
                StartBubbleUpLayoutInvalidState();
            }
        }
        public static void InnerInvalidateLayoutAndStartBubbleUp(ArtVisualElement ve)
        {
            ve.InvalidateLayoutAndStartBubbleUp();
        }
        static ArtVisualElement BubbleUpInvalidLayoutToTopMost(ArtVisualElement ve, ArtVisualRootWindow winroot)
        {
#if DEBUG
            VisualRoot dbugVRoot = ve.dbugVRoot;
#endif

            ve.MarkInvalidContentSize();

            if (ve.visualParentLink == null)
            {
#if DEBUG
                if (ve.IsWindowRoot)
                {
                }

                dbugVRoot.dbug_PushLayoutTraceMessage(VisualRoot.dbugMsg_NO_OWNER_LAY);
#endif
                return null;
            }
            if (winroot != null)
            {
                if (winroot.IsLayoutQueueClearing)
                {
                    return null;
                }
                else if (winroot.IsInLayoutQueue)
                {


                    ve.IsInLayoutQueueChainUp = true;
                    winroot.AddToLayoutQueue(ve);
                }
            }
#if DEBUG
            dbugVRoot.dbug_LayoutTraceBeginContext(VisualRoot.dbugMsg_E_CHILD_LAYOUT_INV_BUB_enter, ve);
#endif

            bool goFinalExit;
            ArtVisualElement parentVisualElem = ve.visualParentLink.NotifyParentToInvalidate(out goFinalExit
#if DEBUG
,
ve
#endif
);


            if (!goFinalExit)
            {
                if (parentVisualElem.IsTextEditContainer)
                {
                    parentVisualElem.MarkInvalidContentArrangement();
                    parentVisualElem.IsInLayoutQueueChainUp = true;
                    goto finalExit;
                }
                else if (parentVisualElem.ActAsFloatingWindow)
                {
                    parentVisualElem.MarkInvalidContentArrangement();
                    parentVisualElem.IsInLayoutQueueChainUp = true;
                    goto finalExit;
                }

                parentVisualElem.MarkInvalidContentSize();
                parentVisualElem.MarkInvalidContentArrangement();

                if (!parentVisualElem.IsInLayoutQueueChainUp
                    && !parentVisualElem.IsInLayoutQueue
                    && !parentVisualElem.IsInLayoutSuspendMode)
                {

                    parentVisualElem.IsInLayoutQueueChainUp = true;

                    ArtVisualElement upper = BubbleUpInvalidLayoutToTopMost(parentVisualElem, winroot);

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
            dbugVRoot.dbug_LayoutTraceEndContext(VisualRoot.dbugMsg_E_CHILD_LAYOUT_INV_BUB_exit, ve);
#endif

            return parentVisualElem;
        }

        ArtVisualRootWindow InternalGetWinRootElement()
        {
            if (visualParentLink == null)
            {
                if (this.IsWindowRoot)
                {
                    return (ArtVisualRootWindow)this;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return visualParentLink.GetWindowRoot();
            }
        }
        public void StartBubbleUpLayoutInvalidState()
        {

#if DEBUG
            dbugVRoot.dbug_LayoutTraceBeginContext(VisualRoot.dbugMsg_E_LAYOUT_INV_BUB_FIRST_enter, this);
#endif
            ArtVisualRootWindow winroot = this.InternalGetWinRootElement();
            ArtVisualElement tobeAddToLayoutQueue = BubbleUpInvalidLayoutToTopMost(this, winroot);
#if DEBUG

#endif

            if (tobeAddToLayoutQueue != null
                && winroot != null
                && !tobeAddToLayoutQueue.IsInLayoutQueue)
            {
                winroot.AddToLayoutQueue(tobeAddToLayoutQueue);
            }

#if DEBUG
            dbugVRoot.dbug_LayoutTraceEndContext(VisualRoot.dbugMsg_E_LAYOUT_INV_BUB_FIRST_exit, this);
#endif

        }

        public bool NeedReCalculateContentSize
        {
            get
            {
                return (uiLayoutFlags & LAY_HAS_CALCULATED_SIZE) == 0;
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
            uiLayoutFlags &= ~LY_HAS_ARRANGED_CONTENT;
#if DEBUG

            this.dbug_InvalidateContentArrEpisode++;
            dbug_totalInvalidateContentArrEpisode++;
#endif
        }
        public void MarkInvalidContentSize()
        {

            uiLayoutFlags &= ~LAY_HAS_CALCULATED_SIZE;
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
            uiLayoutFlags |= LY_HAS_ARRANGED_CONTENT;
        }
        public bool NeedContentArrangement
        {
            get
            {
                return (uiLayoutFlags & LY_HAS_ARRANGED_CONTENT) == 0;
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