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
        
        public bool InvalidateGraphics()
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

        static void RootInvalidateGraphicArea(RenderElement elem, ref Rectangle rect)
        {
            //1.
            elem.propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            //2.  
            elem.rootGfx.InvalidateGraphicArea(elem, ref rect); 
        } 
        protected static void InvalidateGraphicLocalArea(RenderElement ve, Rectangle localArea)
        {
            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }

            RootInvalidateGraphicArea(ve, ref localArea);
        }
        //TODO: review this again
        protected bool ForceReArrange
        {
            get { return true; }
            set { }
        }

        public static bool IsInTopDownReArrangePhase
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
         

        internal void BeginGraphicUpdate()
        {
            InvalidateGraphics();
            this.rootGfx.BeginGraphicUpdate();
            this.uiLayoutFlags |= RenderElementConst.LY_SUSPEND_GRAPHIC;
        }
        internal void EndGraphicUpdate()
        {
            this.uiLayoutFlags &= ~RenderElementConst.LY_SUSPEND_GRAPHIC;

            if (InvalidateGraphics())
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

            if (InvalidateGraphics())
            {
                this.rootGfx.EndGraphicUpdate();
            }
        }
        internal bool IsInRenderChain
        {
            get
            {
                return (propFlags & RenderElementConst.IS_IN_RENDER_CHAIN) != 0;
            }
            set
            {
                propFlags = value ?
                   propFlags | RenderElementConst.IS_IN_RENDER_CHAIN :
                   propFlags & ~RenderElementConst.FIRST_ARR_PASS;

            }
        }
    }

}