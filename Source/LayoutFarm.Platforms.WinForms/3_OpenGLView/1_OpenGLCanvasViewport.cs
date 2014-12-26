//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI.OpenGLView
{
    class OpenGLCanvasViewport : CanvasViewport
    {
        Canvas canvas;
        public OpenGLCanvasViewport(TopWindowRenderBox wintop,
            Size viewportSize, int cachedPageNum)
            : base(wintop, viewportSize, cachedPageNum)
        {
            
        }
        public void NotifyWindowControlBinding()
        {
            this.canvas = LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, this.ViewportWidth, this.ViewportHeight);
        }
        public void PaintMe()
        {

            //----------------------------------
            //gl paint here
            canvas.ClearSurface(Color.White);
            
            ////test draw rect
            //canvas.StrokeColor = LayoutFarm.Drawing.Color.Blue;
            //canvas.DrawRectangle(Color.Blue, 20, 20, 200, 200);
            ////------------------------

            if (this.IsClosed) { return; }
            //------------------------------------ 
            topWindowBox.PrepareRender();
            //---------------
            this.rootGraphics.IsInRenderPhase = true;
#if DEBUG
            this.rootGraphics.dbug_rootDrawingMsg.Clear();
            this.rootGraphics.dbug_drawLevel = 0;
#endif
            UpdateAllArea(this.canvas, this.topWindowBox);
             
            this.rootGraphics.IsInRenderPhase = false;
#if DEBUG

            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
                List<dbugLayoutMsg> outputMsgs = dbugOutputWindow.dbug_rootDocDebugMsgs;
                outputMsgs.Clear();
                outputMsgs.Add(new dbugLayoutMsg(null as RenderElement, "[" + debug_render_to_output_count + "]"));
                visualroot.dbug_DumpRootDrawingMsg(outputMsgs);
                dbugOutputWindow.dbug_InvokeVisualRootDrawMsg();
                debug_render_to_output_count++;
            }


            if (dbugHelper01.dbugVE_HighlightMe != null)
            {
                dbugOutputWindow.dbug_HighlightMeNow(dbugHelper01.dbugVE_HighlightMe.dbugGetGlobalRect());

            }
#endif
        }

        static void UpdateAllArea(Canvas mycanvas, ITopWindowRenderBox topWindowRenderBox)
        {

            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rect rect = Rect.CreateFromRect(mycanvas.Rect);
            topWindowRenderBox.DrawToThisPage(mycanvas, rect);

#if DEBUG
            topWindowRenderBox.dbugShowRenderPart(mycanvas, rect);
#endif
#if DEBUG
#endif

            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
        }
    }

}