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
            //v.uiDesiredWidth = desiredWidth;
            //v.uiDesiredHeight = desiredHeight;
            v.uiWidth = desiredWidth;
            v.uiHeight = desiredHeight;
            v.MarkHasValidCalculateSize();
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



    }
}