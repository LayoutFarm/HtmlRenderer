//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation
{
    partial class RenderElement
    {

        public static void InvalidateGraphicLocalArea(RenderElement ve, Rectangle localArea)
        {
            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }
            ve.uiFlags &= ~IS_GRAPHIC_VALID;
            InternalRect internalRect = InternalRect.CreateFromRect(localArea);
            vinv_AddInvalidateRequest(ve, internalRect);
            InternalRect.FreeInternalRect(internalRect);
        }
        protected static void vinv_AddInvalidateRequest(RenderElement ve, InternalRect rect)
        {
            //throw new NotSupportedException();
            var winroot = ve.WinRoot;
            if (winroot != null)
            {
                winroot.InvalidateGraphicArea(ve, rect);
            }
        }

        protected bool vinv_ForceReArrange
        {
            get { return true; }
            set { }
        }
        protected static bool vinv_IsInTopDownReArrangePhase
        {
            get
            {
                return true;
            }
            set
            {

            }
        }
        public bool IsInvalidateGraphicBlocked
        {

            get
            {
#if DEBUG
                return (uiLayoutFlags & LY_SUSPEND_GRAPHIC) != 0;
#else
                return ((uiLayoutFlags & LY_SUSPEND_GRAPHIC) != 0) || ((uiFlags & HIDDEN) != 0);
#endif
            }
        }
        public void InvalidateGraphic()
        {
            uiFlags &= ~IS_GRAPHIC_VALID;

            if ((uiLayoutFlags & LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }


            InternalRect internalRect = InternalRect.CreateFromWH(b_width, b_Height);
            vinv_AddInvalidateRequest(this, internalRect);
            InternalRect.FreeInternalRect(internalRect);
        }


        public void BeginGraphicUpdate()
        {
            InvalidateGraphic();

            TopWindowRenderBox winroot = vinv_WinRoot;
            if (winroot != null)
            {
                winroot.RootBeginGraphicUpdate();
            }
            else
            {

            }
            this.uiLayoutFlags |= LY_SUSPEND_GRAPHIC;
        }
        public void EndGraphicUpdate()
        {
            this.uiLayoutFlags &= ~LY_SUSPEND_GRAPHIC;
            InvalidateGraphic();
            TopWindowRenderBox winroot = vinv_WinRoot;
            if (winroot != null)
            {
                winroot.RootEndGraphicUpdate();
            }
            else
            {

            }
        }

        void BeforeBoundChangedInvalidateGraphics()
        {

            InvalidateGraphic();

            TopWindowRenderBox winroot = vinv_WinRoot;
            if (winroot != null)
            {
                winroot.RootBeginGraphicUpdate();
            }
            else
            {

            }

            this.uiLayoutFlags |= LY_SUSPEND_GRAPHIC;
        }
        void AfterBoundChangedInvalidateGraphics()
        {
            this.uiLayoutFlags &= ~LY_SUSPEND_GRAPHIC;

            InvalidateGraphic();

            TopWindowRenderBox winroot = vinv_WinRoot;
            if (winroot != null)
            {
                winroot.RootEndGraphicUpdate();
            }
            else
            { 
            } 
        } 
        protected TopWindowRenderBox vinv_WinRoot
        {
            get
            {
                //find winroot
                if (this.IsWindowRoot)
                {
                    return (TopWindowRenderBox)this;
                }
                else if (this.ParentVisualElement != null)
                {
                    return this.ParentVisualElement.vinv_WinRoot;
                }
                else
                {
                    return null;
                }
            }
        } 
    }

}