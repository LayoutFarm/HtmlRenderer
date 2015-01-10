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
        public static void InvalidateGraphicLocalArea(RenderElement ve, Rectangle localArea)
        {
            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }

            RootInvalidateGraphicArea(ve, ref localArea);
        }
        public void InvalidateGraphics()
        {

            InvalidateGraphic();
        }

        static void RootInvalidateGraphicArea(RenderElement elem, ref Rectangle rect)
        {
            //1.
            elem.propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            //2.  
            elem.rootGfx.InvalidateGraphicArea(elem, ref rect);

        }

        protected bool vinv_ForceReArrange
        {

            get { return true; }
            set { }
        }

        public static bool vinv_IsInTopDownReArrangePhase
        {
            //TODO: review this again !
            get
            {
                return true;
            }
            set
            {

            }
        }


        internal bool IsInvalidateGraphicBlocked
        {

            get
            {
#if DEBUG
                return (uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0;
#else
                return ((uiLayoutFlags & LY_SUSPEND_GRAPHIC) != 0) || ((uiFlags & HIDDEN) != 0);
#endif
            }
        }
        internal bool InvalidateGraphic()
        {
            propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            if ((uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif

                return false;
            }

            Rectangle rect = new Rectangle(0, 0, b_width, b_height);

            RootInvalidateGraphicArea(this, ref rect);
            return true;//TODO: review this 
        }

        internal void BeginGraphicUpdate()
        {
            InvalidateGraphics();
            this.rootGfx.BeginGraphicUpdate();
            this.uiLayoutFlags |= RenderElementConst.LY_SUSPEND_GRAPHIC;
        }
        internal void EndGraphicUpdate()
        {
            this.uiLayoutFlags &= ~RenderElementConst.LY_SUSPEND_GRAPHIC;

            if (InvalidateGraphic())
            {
                this.rootGfx.EndGraphicUpdate();
            }
        }
        void BeforeBoundChangedInvalidateGraphics()
        {
            InvalidateGraphics();
            this.rootGfx.BeginGraphicUpdate();
            this.uiLayoutFlags |= RenderElementConst.LY_SUSPEND_GRAPHIC;
        }
        void AfterBoundChangedInvalidateGraphics()
        {
            this.uiLayoutFlags &= ~RenderElementConst.LY_SUSPEND_GRAPHIC;

            if (InvalidateGraphic())
            {
                this.rootGfx.EndGraphicUpdate();
            }
        }

    }

}