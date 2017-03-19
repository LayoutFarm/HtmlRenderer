//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
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
        public void InvalidateGraphicBounds()
        {
            this.InvalidateGraphicBounds(this.RectBounds);
        }
        public void InvalidateGraphicBounds(Rectangle totalBounds)
        {
            propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            var parent = this.ParentRenderElement; //start at parent ****
            //--------------------------------------- 
            if (parent == null)
            {
                return;
            }
            this.rootGfx.InvalidateGraphicArea(parent, ref totalBounds);
        }

        static void RootInvalidateGraphicArea(RenderElement re, ref Rectangle rect)
        {
            //1.
            re.propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            //2.  
            re.rootGfx.InvalidateGraphicArea(re, ref rect);
        }

        public static void InvalidateGraphicLocalArea(RenderElement re, Rectangle localArea)
        {
            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }
            RootInvalidateGraphicArea(re, ref localArea);
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


        internal bool BlockGraphicUpdateBubble
        {
            get
            {
#if DEBUG
                return (uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0;
#else
                return (uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0;
#endif
            }
        }
    }
}