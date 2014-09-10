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

        public static void InvalidateGraphicLocalArea(RenderElement ve, Rectangle localArea, VisualElementArgs vinv)
        {
            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }
            ve.uiFlags &= ~IS_GRAPHIC_VALID;
            InternalRect internalRect = InternalRect.CreateFromRect(localArea);
            vinv.AddInvalidateRequest(ve, internalRect);
            InternalRect.FreeInternalRect(internalRect);


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
        public void InvalidateGraphic(VisualElementArgs vinv)
        {
            uiFlags &= ~IS_GRAPHIC_VALID;

            if ((uiLayoutFlags & LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(dbugRootElement.dbugMsg_BLOCKED, this);
#endif
                return;
            }


            InternalRect internalRect = InternalRect.CreateFromWH(b_width, b_Height);
            vinv.AddInvalidateRequest(this, internalRect);
            InternalRect.FreeInternalRect(internalRect);
        }
        public void BeginGraphicUpdate(VisualElementArgs vinv)
        {
            InvalidateGraphic(vinv);

            RootWindowRenderBox winroot = vinv.WinRoot;
            if (winroot != null)
            {
                winroot.RootBeginGraphicUpdate();
            }
            else
            {

            }
            this.uiLayoutFlags |= LY_SUSPEND_GRAPHIC;
        }

        public void EndGraphicUpdate(VisualElementArgs vinv)
        {
            this.uiLayoutFlags &= ~LY_SUSPEND_GRAPHIC; InvalidateGraphic(vinv);
            RootWindowRenderBox winroot = vinv.WinRoot;
            if (winroot != null)
            {
                winroot.RootEndGraphicUpdate();
            }
            else
            {

            }
        }
        void BeforeBoundChangedInvalidateGraphics(VisualElementArgs vinv)
        {
            if (vinv != null)
            {
                InvalidateGraphic(vinv);

                RootWindowRenderBox winroot = vinv.WinRoot;
                if (winroot != null)
                {
                    winroot.RootBeginGraphicUpdate();
                }
                else
                {

                }
            }
            this.uiLayoutFlags |= LY_SUSPEND_GRAPHIC;
        }
        void AfterBoundChangedInvalidateGraphics(VisualElementArgs vinv)
        {
            this.uiLayoutFlags &= ~LY_SUSPEND_GRAPHIC; if (vinv != null)
            {
                InvalidateGraphic(vinv);

                RootWindowRenderBox winroot = vinv.WinRoot;
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

}