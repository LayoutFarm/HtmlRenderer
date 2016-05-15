// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using PixelFarm.Drawing.WinGdi;

namespace LayoutFarm.UI.GdiPlus
{
    class GdiPlusCanvasViewport : CanvasViewport
    {
        QuadPages quadPages = null;
        public GdiPlusCanvasViewport(RootGraphic rootgfx,
            Size viewportSize, int cachedPageNum)
            : base(rootgfx, viewportSize, cachedPageNum)
        {
            quadPages = new QuadPages(rootgfx.P, cachedPageNum, viewportSize.Width, viewportSize.Height * 2);
            this.CalculateCanvasPages();
        }
        ~GdiPlusCanvasViewport()
        {
            if (quadPages != null)
            {
                quadPages.Dispose();
            }
        }

        //static int dbugCount = 0;
        protected override void OnClosing()
        {
            if (quadPages != null)
            {
                quadPages.Dispose();
                quadPages = null;
            }
            base.OnClosing();
        }

        public override void CanvasInvlidateArea(Rectangle r)
        {
            quadPages.CanvasInvalidate(r);
            //Console.WriteLine((dbugCount++).ToString() + " " + r.ToString());
        }
        public override bool IsQuadPageValid
        {
            get
            {
                return this.quadPages.IsValid;
            }
        }
        protected override void ResetQuadPages(int viewportWidth, int viewportHeight)
        {
            quadPages.ResizeAllPages(viewportWidth, viewportHeight);
        }
        protected override void CalculateCanvasPages()
        {
            quadPages.CalculateCanvasPages(this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            this.FullMode = true;
        }


        public void PaintMe(IntPtr hdc)
        {
            if (this.IsClosed) { return; }
            //------------------------------------ 

            this.rootGraphics.PrepareRender();
            //---------------
            this.rootGraphics.IsInRenderPhase = true;
#if DEBUG
            this.rootGraphics.dbug_rootDrawingMsg.Clear();
            this.rootGraphics.dbug_drawLevel = 0;
#endif
            if (this.FullMode)
            {
                quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc,
                    this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            }
            else
            {
                //temp to full mode
                //quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc, this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
                quadPages.RenderToOutputWindowPartialMode(rootGraphics.TopWindowRenderBox, hdc,
                   this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);

            }
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


        public void PaintMe(Canvas mycanvas)
        {
            if (this.IsClosed) { return; }
            //------------------------------------ 

            this.rootGraphics.PrepareRender();
            //---------------
            this.rootGraphics.IsInRenderPhase = true;
#if DEBUG
            this.rootGraphics.dbug_rootDrawingMsg.Clear();
            this.rootGraphics.dbug_drawLevel = 0;
#endif

            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rectangle rect = mycanvas.Rect;
            this.rootGraphics.TopWindowRenderBox.DrawToThisCanvas(mycanvas, rect);
#if DEBUG
            this.rootGraphics.TopWindowRenderBox.dbugShowRenderPart(mycanvas, rect);
#endif

            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);

            //if (this.FullMode)
            //{
            //    quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc,
            //        this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            //}
            //else
            //{
            //    //temp to full mode
            //    //quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc, this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            //    quadPages.RenderToOutputWindowPartialMode(rootGraphics.TopWindowRenderBox, hdc,
            //        this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);

            //}
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
    }

}