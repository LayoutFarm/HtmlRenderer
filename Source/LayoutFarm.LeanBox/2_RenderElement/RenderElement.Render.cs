//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;
using System.Drawing.Drawing2D;



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


        public abstract void CustomDrawToThisPage(Canvas canvasPage, InternalRect updateArea);

        public bool PrepareDrawingChain(VisualDrawingChain drawingChain)
        {
            if ((uiFlags & HIDDEN) == HIDDEN)
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
                    ((RenderBoxBase)this).PrepareOriginalChildContentDrawingChain(drawingChain);
                    drawingChain.OffsetCanvasOrigin(-x, -y);

                }
               
            }
            return false;
        }
        public void DrawToThisPage(Canvas canvasPage, InternalRect updateArea)
        {

            if ((uiFlags & HIDDEN) == HIDDEN)
            {
                return;
            }
#if DEBUG
            dbugVRoot.dbug_drawLevel++;
#endif

            if (canvasPage.PushClipArea(b_width, b_Height, updateArea))
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
                uiFlags |= IS_GRAPHIC_VALID;
#if DEBUG
                debug_RecordPostDrawInfo(canvasPage);
#endif
            }

            canvasPage.PopClipArea();
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
                return (this.uiFlags & HAS_DOUBLE_SCROLL_SURFACE) != 0;
            }
            protected set
            {
                if (value)
                {
                    this.uiFlags |= HAS_DOUBLE_SCROLL_SURFACE;
                }
                else
                {
                    this.uiFlags &= ~HAS_DOUBLE_SCROLL_SURFACE;
                }
            }
        }

        
        
    }
}