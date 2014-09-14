//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using LayoutFarm;

namespace LayoutFarm
{
    partial class RenderElement
    {

        protected static void RootInvalidateGraphicArea(RenderElement elem, ref Rectangle rect)
        {
            //1.
            elem.uiFlags &= ~IS_GRAPHIC_VALID;
            //2.

            var winroot = elem.WinRoot;
            if (winroot != null)
            {
                winroot.InvalidateGraphicArea(elem, ref rect);
            }
        }
        public static void InvalidateGraphicLocalArea(RenderElement ve, Rectangle localArea)
        {
            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }
            RootInvalidateGraphicArea(ve, ref localArea);
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

            Rectangle rect = new Rectangle(0, 0, b_width, b_Height);
            RootInvalidateGraphicArea(this, ref rect);
        }
        public void BeginGraphicUpdate()
        {
            InvalidateGraphic();


            TopWindowRenderBox winroot = this.InternalGetTopWindowRenderBox();
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
            TopWindowRenderBox winroot = this.InternalGetTopWindowRenderBox();
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

            TopWindowRenderBox winroot = this.InternalGetTopWindowRenderBox();
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

            TopWindowRenderBox winroot = this.InternalGetTopWindowRenderBox();
            if (winroot != null)
            {
                winroot.RootEndGraphicUpdate();
            }
            else
            {
            }
        }
    
    }

}