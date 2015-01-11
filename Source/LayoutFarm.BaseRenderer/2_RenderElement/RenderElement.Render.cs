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
        public abstract void CustomDrawToThisCanvas(Canvas canvas, Rectangle updateArea);
      
        public void DrawToThisCanvas(Canvas canvas, Rectangle updateArea)
        {

            if ((propFlags & RenderElementConst.HIDDEN) == RenderElementConst.HIDDEN)
            {
                return;
            }
#if DEBUG
            dbugVRoot.dbug_drawLevel++;
#endif

            if (canvas.PushClipAreaRect(b_width, b_height, ref updateArea))
            {
#if DEBUG
                if (dbugVRoot.dbug_RecordDrawingChain)
                {
                    dbugVRoot.dbug_AddDrawElement(this, canvas);
                }
#endif
                //------------------------------------------

                this.CustomDrawToThisCanvas(canvas, updateArea);

                //------------------------------------------
                propFlags |= RenderElementConst.IS_GRAPHIC_VALID;
#if DEBUG
                debug_RecordPostDrawInfo(canvas);
#endif
            }

            canvas.PopClipAreaRect();
#if DEBUG
            dbugVRoot.dbug_drawLevel--;
#endif
        } 
      

    }
}