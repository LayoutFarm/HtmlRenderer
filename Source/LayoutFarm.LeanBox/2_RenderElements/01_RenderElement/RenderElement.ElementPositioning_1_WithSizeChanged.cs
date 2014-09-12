//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm.Presentation
{


    partial class RenderElement
    {


        public virtual void TopDownReCalculateContentSize()
        {   
            MarkHasValidCalculateSize();
        } 

        public static void SetCalculatedDesiredSize(RenderBoxBase v, int desiredWidth, int desiredHeight)
        {
 
            v.b_width = desiredWidth;
            v.b_Height = desiredHeight;
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

                        RenderElement parentElement = this.ParentVisualElement;
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

        public void ResumeLayout()
        {
            uiLayoutFlags &= ~LY_SUSPEND;

            if (this.MayHasChild)
            {
                if (this.HasOwner)
                {
                    if (!vinv_IsInTopDownReArrangePhase)
                    {
                        this.StartBubbleUpLayoutInvalidState();
                    }
                }
                else
                {
                    if (this.IsWindowRoot)
                    {
                        this.TopDownReCalculateContentSize();
                        ((RenderBoxBase)this).TopDownReArrangeContentIfNeed();
                    }
                }
            }
        }

        public void SetWidth(int width)
        {
            this.SetSize(width, this.b_Height);
        }
        public void SetHeight(int height)
        {
            this.SetSize(this.b_width, height);
        }
        public void SetSize(int width, int height)
        {

            if (visualParentLink == null)
            {
                this.b_width = width;
                this.b_Height = height;
            }
            else
            {

                int prevWidth = this.b_width;
                int prevHeight = this.b_Height;
                this.BeforeBoundChangedInvalidateGraphics();
                PrivateSetSize(width, height);
                this.AfterBoundChangedInvalidateGraphics();
            }

        }
        void PrivateSetSize(int width, int height)
        {
            RenderElement.DirectSetVisualElementSize(this, width, height);

            if (this.MayHasChild)
            {
                RenderBoxBase vscont = (RenderBoxBase)this;
                if (!vinv_IsInTopDownReArrangePhase)
                {
                    vscont.InvalidateContentArrangementFromContainerSizeChanged();
                    this.InvalidateLayoutAndStartBubbleUp();
                }
                else
                {
#if DEBUG
                    vinv_dbug_SetInitObject(this);
#endif
                    vscont.ForceTopDownReArrangeContent();
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