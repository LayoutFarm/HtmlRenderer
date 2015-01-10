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

        public virtual void TopDownReCalculateContentSize()
        {
            MarkHasValidCalculateSize();
        }

        internal static void SetCalculatedDesiredSize(RenderBoxBase v, int desiredWidth, int desiredHeight)
        {
            v.b_width = desiredWidth;
            v.b_height = desiredHeight;
            v.MarkHasValidCalculateSize();
        }
        public static bool IsLayoutSuspending(RenderBoxBase re)
        {
            //recursive
            if (re.IsTopWindow)
            {
                return (re.uiLayoutFlags & RenderElementConst.LY_SUSPEND) != 0;
            }
            else
            {

                if ((re.uiLayoutFlags & RenderElementConst.LY_SUSPEND) != 0)
                {

                    return true;
                }
                else
                {

                    var parentElement = re.ParentRenderElement as RenderBoxBase;
                    if (parentElement != null)
                    {
                        return IsLayoutSuspending(parentElement);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        bool IsInLayoutSuspendMode
        {
            get
            {
                return (uiLayoutFlags & RenderElementConst.LY_SUSPEND) != 0;
            }
        }



        public void ResumeLayout()
        {
            uiLayoutFlags &= ~RenderElementConst.LY_SUSPEND;

            if (this.MayHasChild)
            {
                if (this.HasParent)
                {
                    if (!IsInTopDownReArrangePhase)
                    {
                        this.StartBubbleUpLayoutInvalidState();
                    }
                }
                else
                {
                    if (this.IsTopWindow)
                    {
                        this.TopDownReCalculateContentSize();
                        ((RenderBoxBase)this).TopDownReArrangeContentIfNeed();
                    }
                }
            }
        }

        public void SetWidth(int width)
        {
            this.SetSize(width, this.b_height);
        }
        public void SetHeight(int height)
        {
            this.SetSize(this.b_width, height);
        }
        public void SetSize(int width, int height)
        {
            if (parentLink == null)
            {
                //direct set size
                this.b_width = width;
                this.b_height = height;
            }
            else
            {
#if DEBUG
                int dbug_prevWidth = this.b_width;
                int dbug_prevHeight = this.b_height;
#endif

                this.BeforeBoundChangedInvalidateGraphics();
               
                PrivateSetSize(width, height);
                
                this.AfterBoundChangedInvalidateGraphics();
            }
        }
        public void SetLocation(int left, int top)
        {
            if (parentLink == null)
            {
                this.b_left = left;
                this.b_top = top;
            }
            else
            {
#if DEBUG
                int dbug_prevLeft = this.b_left;
                int dbug_prevTop = this.b_top;
#endif
                this.BeginGraphicUpdate();
                DirectSetVisualElementLocation(this, left, top);
                this.EndGraphicUpdate();
            }
        }

        void PrivateSetSize(int width, int height)
        {
            RenderElement.DirectSetVisualElementSize(this, width, height);

            if (this.MayHasChild)
            {
                RenderBoxBase vscont = (RenderBoxBase)this;
                if (!IsInTopDownReArrangePhase)
                {
                    vscont.InvalidateContentArrangementFromContainerSizeChanged();
                    this.InvalidateLayoutAndStartBubbleUp();
                }
                else
                {
#if DEBUG
                    dbug_SetInitObject(this);
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