// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm
{
    partial class RenderElement
    {
        bool hasTransparentBg;


        public bool HasSolidBackground
        {
            get
            {
                return !hasTransparentBg;
            }
            set
            {
                this.hasTransparentBg = !value;
            }
        }


        public abstract void CustomDrawToThisPage(Canvas canvasPage, Rectangle updateArea);

        public bool PrepareDrawingChain(RenderBoxes.VisualDrawingChain drawingChain)
        {
            if ((uiFlags & RenderElementConst.HIDDEN) == RenderElementConst.HIDDEN)
            {
                return false;
            }

            if (this.IntersectsWith(drawingChain.CurrentClipRect))
            {
                bool containAll = this.ContainRect(drawingChain.CurrentClipRect);
                drawingChain.AddVisualElement(this, containAll);
                if (this.MayHasViewport)
                {
                    int x = this.b_left;
                    int y = this.b_top;
                    x -= this.ViewportX;
                    y -= this.ViewportY;

                    drawingChain.OffsetCanvasOrigin(x, y);
                    ((RenderBoxes.RenderBoxBase)this).PrepareOriginalChildContentDrawingChain(drawingChain);
                    drawingChain.OffsetCanvasOrigin(-x, -y);
                }
            }
            return false;
        }
        public void DrawToThisPage(Canvas canvasPage, Rectangle updateArea)
        {

            if ((uiFlags & RenderElementConst.HIDDEN) == RenderElementConst.HIDDEN)
            {
                return;
            }
#if DEBUG
            dbugVRoot.dbug_drawLevel++;
#endif

            if (canvasPage.PushClipAreaRect(b_width, b_Height, ref updateArea))
            {
#if DEBUG
                if (dbugVRoot.dbug_RecordDrawingChain)
                {
                    dbugVRoot.dbug_AddDrawElement(this, canvasPage);
                }
#endif
                //------------------------------------------

                this.CustomDrawToThisPage(canvasPage, updateArea);

                //------------------------------------------
                uiFlags |= RenderElementConst.IS_GRAPHIC_VALID;
#if DEBUG
                debug_RecordPostDrawInfo(canvasPage);
#endif
            }

            canvasPage.PopClipAreaRect();
#if DEBUG
            dbugVRoot.dbug_drawLevel--;
#endif
        }


        public bool IsTopWindow
        {
            get
            {
                return this.isWindowRoot;
            }
        }

        public bool HasDoubleScrollableSurface
        {
            get
            {
                return (this.uiFlags & RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE) != 0;
            }
            protected set
            {
                uiFlags = value ?
                      uiFlags | RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE :
                      uiFlags & ~RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE;
            }
        }



    }
}