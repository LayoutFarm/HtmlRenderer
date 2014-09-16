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

        protected static void RootInvalidateGraphicArea(RenderElement elem, ref Rectangle rect, out TopWindowRenderBox wintop)
        {
            //1.
            elem.uiFlags &= ~IS_GRAPHIC_VALID;
            //2.  

            elem.rootGfx.InvalidateGraphicArea(elem, ref rect, out wintop);

        }
        public static void InvalidateGraphicLocalArea(RenderElement ve, Rectangle localArea)
        {
            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }
            TopWindowRenderBox wintop;
            RootInvalidateGraphicArea(ve, ref localArea, out wintop);
        }
        public static void InvalidateGraphicLocalArea(RenderElement ve, Rectangle localArea, out TopWindowRenderBox wintop)
        {
            if (localArea.Height == 0 || localArea.Width == 0)
            {
                wintop = null;
                return;
            }
            RootInvalidateGraphicArea(ve, ref localArea, out wintop);
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
            TopWindowRenderBox wintop;
            InvalidateGraphic(out wintop);
        }
        
        public bool InvalidateGraphic(out TopWindowRenderBox wintop)
        {
            uiFlags &= ~IS_GRAPHIC_VALID;
            if ((uiLayoutFlags & LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                wintop = null;
                return false;
            }

            Rectangle rect = new Rectangle(0, 0, b_width, b_Height);
            RootInvalidateGraphicArea(this, ref rect, out wintop);
            return wintop != null;
        }
   
        public void BeginGraphicUpdate()
        {

            InvalidateGraphic();
            this.rootGfx.BeginGraphicUpdate(); 
            this.uiLayoutFlags |= LY_SUSPEND_GRAPHIC;
        }
        public void EndGraphicUpdate()
        {
            this.uiLayoutFlags &= ~LY_SUSPEND_GRAPHIC;
            TopWindowRenderBox wintop;
            if (InvalidateGraphic(out wintop))
            {
                this.rootGfx.EndGraphicUpdate(wintop);
            }
        }

        void BeforeBoundChangedInvalidateGraphics()
        {

            InvalidateGraphic();
            this.rootGfx.BeginGraphicUpdate();
            this.uiLayoutFlags |= LY_SUSPEND_GRAPHIC;
        }
        void AfterBoundChangedInvalidateGraphics()
        {
            this.uiLayoutFlags &= ~LY_SUSPEND_GRAPHIC;
            TopWindowRenderBox wintop;
            if (InvalidateGraphic(out wintop))
            {
                this.rootGfx.EndGraphicUpdate(wintop);
            }
        }

    }

}