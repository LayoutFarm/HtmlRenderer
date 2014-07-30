//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm.Presentation
{


    partial class ArtVisualElement
    {


        public virtual void TopDownReCalculateContentSize(VisualElementArgs vinv)
        {

#if DEBUG
#endif
            MarkHasValidCalculateSize();
        }



        public static void SetCalculatedDesiredSize(ArtVisualContainerBase v, int desiredWidth, int desiredHeight)
        {
            v.uiDesiredWidth = desiredWidth;
            v.uiDesiredHeight = desiredHeight;
            v.MarkHasValidCalculateSize();
        }

        public void SuspendLayout()
        {
            uiLayoutFlags |= LY_SUSPEND;

        }
        public void ResumeLayout()
        {
            VisualElementArgs vinv = this.GetVInv();

#if DEBUG
            vinv.dbug_SetInitObject(this);
            vinv.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.ArtVisualElement_ResumeLayout);
#endif

            this.ResumeLayout(vinv);

#if DEBUG
            vinv.dbug_EndLayoutTrace();
#endif
            this.FreeVInv(vinv);

        }

        public bool IsLayoutSuspending
        {
            get
            {

                if (this.IsWindowRoot)
                {
                    return (this.uiLayoutFlags & LY_SUSPEND) != 0;
                }
                else
                {

                    if ((this.uiLayoutFlags & LY_SUSPEND) != 0)
                    {

                        return true;
                    }
                    else
                    {

                        ArtVisualElement parentElement = this.ParentVisualElement;
                        if (parentElement != null)
                        {
                            return parentElement.IsLayoutSuspending;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public bool IsInLayoutSuspendMode
        {
            get
            {
                return (uiLayoutFlags & LY_SUSPEND) != 0;
            }
        }

        public void ResumeLayout(VisualElementArgs vinv)
        {


            uiLayoutFlags &= ~LY_SUSPEND; if (this.IsVisualContainerBase)
            {
                if (this.HasOwner)
                {
                    if (!vinv.IsInTopDownReArrangePhase)
                    {
                        this.StartBubbleUpLayoutInvalidState();
                    }
                }
                else
                {
                    if (this.IsWindowRoot)
                    {
                        this.TopDownReCalculateContentSize(vinv);
                        ((ArtVisualContainerBase)this).TopDownReArrangeContentIfNeed(vinv);
                    }
                }
            }
        }

        public void SetWidth(int width, VisualElementArgs vinv)
        {
            this.SetSize(width, this.uiHeight, vinv); 
        } 
        public void SetHeight(int height, VisualElementArgs vinv)
        {
            this.SetSize(this.uiWidth, height, vinv);
        }
        public void SetSize(int width, int height, VisualElementArgs vinv)
        {

            if (visualParentLink == null)
            {
                this.uiWidth = width;
                this.uiHeight = height;
            }
            else
            {

                int prevWidth = this.uiWidth; int prevHeight = this.uiHeight;
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                PrivateSetSize(width, height, vinv);
                this.AfterBoundChangedInvalidateGraphics(vinv);
            }

        }
        void PrivateSetSize(int width, int height, VisualElementArgs vinv)
        {
            ArtVisualElement.DirectSetVisualElementSize(this, width, height);

            if (this.IsVisualContainerBase)
            {
                ArtVisualContainerBase vscont = (ArtVisualContainerBase)this;
                if (!vinv.IsInTopDownReArrangePhase)
                {
                    vscont.InvalidateContentArrangementFromContainerSizeChanged();
                    this.InvalidateLayoutAndStartBubbleUp();
                }
                else
                {
#if DEBUG
                    vinv.dbug_SetInitObject(this);
#endif
                    vscont.ForceTopDownReArrangeContent(vinv);
                }
            }
            else
            {
#if DEBUG
                this.dbug_BeginArr++;
#endif

                this.MarkValidContentArrangement();
#if DEBUG
                this.dbug_FinishArr++;
#endif
            }
        }
        public void SetRectBound(int x, int y, int width, int height, VisualElementArgs vinv)
        {
            if (visualParentLink == null)
            {
                uiLeft = x;
                uiTop = y;
                uiWidth = width;
                uiHeight = height;
            }
            else
            {

                int prevWidth = this.uiWidth; int prevHeight = this.uiHeight;
                bool locationChanged = (this.uiLeft != x) || (this.uiTop != y);
                bool sizeChanged = (prevWidth != width) || (prevHeight != height);

                if (!locationChanged && !sizeChanged)
                {
                    return;
                }
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                ArtVisualElement.DirectSetVisualElementLocation(this, x, y);
                if (sizeChanged)
                {
                    PrivateSetSize(width, height, vinv);

                }
                this.AfterBoundChangedInvalidateGraphics(vinv);
            }
        }
        public void SetRectBound(Rectangle r, VisualElementArgs vinv)
        {
            SetRectBound(r.X, r.Y, r.Width, r.Height, vinv);
        }


        public void ChangeWidthFromLeftSide(int widthDiff, VisualElementArgs vinv)
        {
            if (widthDiff == 0)
            {
                return;
            }
            if (visualParentLink == null)
            {
                uiWidth -= widthDiff;
                uiLeft += widthDiff;
            }
            else
            {
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                int prevWidth = this.uiWidth;
                uiLeft += widthDiff;
                PrivateSetSize(this.uiWidth - widthDiff, uiHeight, vinv);
                this.AfterBoundChangedInvalidateGraphics(vinv);












            }
        }

        public void ChangeHeightFromTopSide(int heightDiff, VisualElementArgs vinv)
        {

            if (heightDiff == 0)
            {
                return;
            }
            if (visualParentLink == null)
            {
                uiHeight -= heightDiff;
                uiTop += heightDiff;
            }
            else
            {
                int prevHeight = this.uiHeight;
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                uiTop += heightDiff;
                PrivateSetSize(uiWidth, uiHeight - heightDiff, vinv);



            }
        }
        public void ChangeWidthFromRightSide(int widthDiff, VisualElementArgs vinv)
        {
            if (widthDiff == 0)
            {
                return;
            }
            if (visualParentLink == null)
            {
                uiWidth += widthDiff;
            }
            else
            {

                int prevWidth = this.uiWidth;
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                PrivateSetSize(uiWidth + widthDiff, uiHeight, vinv);
                this.AfterBoundChangedInvalidateGraphics(vinv);




            }
        }
        public void ChangeHeightFromBottomSide(int heightDiff, VisualElementArgs vinv)
        {
            if (heightDiff == 0)
            {
                return;
            }
            if (visualParentLink == null)
            {
                uiHeight += heightDiff;
            }
            else
            {
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                PrivateSetSize(this.uiWidth, this.uiHeight + heightDiff, vinv);
                this.AfterBoundChangedInvalidateGraphics(vinv);



            }
        }
        public static bool IsSizeEqualTo(ArtVisualElement visualElement, int width, int height)
        {
            return visualElement.uiWidth == width && visualElement.uiHeight == height;
        }


    }
}