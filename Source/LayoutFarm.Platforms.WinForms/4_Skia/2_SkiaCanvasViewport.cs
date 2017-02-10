//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.WinGdi;

#if __SKIA__
namespace LayoutFarm.UI.Skia
{
    class SkiaCanvasViewport : CanvasViewport
    {

        PixelFarm.Drawing.Skia.MySkiaCanvas mySkCanvas;
        int internalSizeW = 800;
        int internalSizwH = 600;
        //-
        //TODO: review here again
        System.Drawing.Bitmap tmpBmp;
        byte[] tmpBuffer;
        public SkiaCanvasViewport(RootGraphic rootgfx,
            Size viewportSize, int cachedPageNum)
            : base(rootgfx, viewportSize, cachedPageNum)
        {

            this.CalculateCanvasPages();
            mySkCanvas = new PixelFarm.Drawing.Skia.MySkiaCanvas(0, 0, 0, 0, internalSizeW, internalSizwH);
            //TODO: review performance here
            //review how to move data from unmanged(skia) to unmanaged(hdc's bitmap)
            tmpBmp = new System.Drawing.Bitmap(internalSizeW, internalSizwH);
            var bmpdata = tmpBmp.LockBits(
                new System.Drawing.Rectangle(0, 0, internalSizeW, internalSizwH),
                 System.Drawing.Imaging.ImageLockMode.ReadOnly, tmpBmp.PixelFormat);
            tmpBuffer = new byte[bmpdata.Stride * bmpdata.Height];
            tmpBmp.UnlockBits(bmpdata);
        }
        protected override void OnClosing()
        {
            if (tmpBuffer != null)
            {
                tmpBuffer = null;
            }
            if (tmpBmp != null)
            {
                tmpBmp.Dispose();
                tmpBmp = null;
            }

            base.OnClosing();
        }
        public override void CanvasInvlidateArea(Rectangle r)
        {
            //quadPages.CanvasInvalidate(r);
            //Console.WriteLine((dbugCount++).ToString() + " " + r.ToString());
        }
        public override bool IsQuadPageValid
        {
            get
            {
                return true;
                //return this.quadPages.IsValid;
            }
        }
        protected override void ResetQuadPages(int viewportWidth, int viewportHeight)
        {
            //  quadPages.ResizeAllPages(viewportWidth, viewportHeight);
        }
        protected override void CalculateCanvasPages()
        {
            //quadPages.CalculateCanvasPages(this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            this.FullMode = true;
        }

        static void UpdateAllArea(PixelFarm.Drawing.Skia.MySkiaCanvas mycanvas, IRenderElement topWindowRenderBox)
        {
            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rectangle rect = mycanvas.Rect;
            topWindowRenderBox.DrawToThisCanvas(mycanvas, rect);
#if DEBUG
            topWindowRenderBox.dbugShowRenderPart(mycanvas, rect);
#endif

            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
        }
        public void PaintMe(IntPtr hdc)
        {
            if (this.IsClosed) { return; }
            this.rootGraphics.PrepareRender();
            //---------------
            this.rootGraphics.IsInRenderPhase = true;
#if DEBUG
            this.rootGraphics.dbug_rootDrawingMsg.Clear();
            this.rootGraphics.dbug_drawLevel = 0;
#endif

            //1. clear sk surface
            mySkCanvas.ClearSurface(PixelFarm.Drawing.Color.White);
            //2. render to the surface
            UpdateAllArea(mySkCanvas, rootGraphics.TopWindowRenderBox);
            //3. copy bitmap buffer from the surface and render to final hdc

            //-----------------------------------------------
            //TODO: review performance here 
            //we should copy from unmanaged (skia bitmap) 
            //and write to unmanaged (hdc'bitmap)

            SkiaSharp.SKBitmap backBmp = mySkCanvas.BackBmp;
            backBmp.LockPixels();
            IntPtr h = backBmp.GetPixels();

            int w1 = backBmp.Width;
            int h1 = backBmp.Height;
            int stride = backBmp.RowBytes;

            System.Runtime.InteropServices.Marshal.Copy(
                h, tmpBuffer, 0, tmpBuffer.Length
                );
            backBmp.UnlockPixels();


            var bmpdata = tmpBmp.LockBits(new System.Drawing.Rectangle(0, 0, w1, h1),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(tmpBuffer, 0, bmpdata.Scan0, tmpBuffer.Length);
            tmpBmp.UnlockBits(bmpdata);

            using (System.Drawing.Graphics g2 = System.Drawing.Graphics.FromHdc(hdc))
            {
                g2.DrawImage(tmpBmp, 0, 0);
            }
            //-----------------------------------------------------------------------------
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


        public void PaintMe(MyGdiPlusCanvas mycanvas)
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

#endif