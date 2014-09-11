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


        public virtual void TopDownReCalculateContentSize(VisualElementArgs vinv)
        {

#if DEBUG
#endif
            MarkHasValidCalculateSize();
        }



        public static void SetCalculatedDesiredSize(RenderBoxBase v, int desiredWidth, int desiredHeight)
        {
            //v.uiDesiredWidth = desiredWidth;
            //v.uiDesiredHeight = desiredHeight;
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

        public void ResumeLayout(VisualElementArgs vinv)
        {
            uiLayoutFlags &= ~LY_SUSPEND;

            if (this.IsVisualContainerBase)
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
                        ((RenderBoxBase)this).TopDownReArrangeContentIfNeed(vinv);
                    }
                }
            }
        }

        public void SetWidth(int width, VisualElementArgs vinv)
        {
            this.SetSize(width, this.b_Height, vinv);
        }
        public void SetHeight(int height, VisualElementArgs vinv)
        {
            this.SetSize(this.b_width, height, vinv);
        }
        public void SetSize(int width, int height, VisualElementArgs vinv)
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
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                PrivateSetSize(width, height, vinv);
                this.AfterBoundChangedInvalidateGraphics(vinv);
            }

        }
        void PrivateSetSize(int width, int height, VisualElementArgs vinv)
        {
            RenderElement.DirectSetVisualElementSize(this, width, height);

            if (this.IsVisualContainerBase)
            {
                RenderBoxBase vscont = (RenderBoxBase)this;
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