//Apache2, 2014-2016, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using PixelFarm.Drawing.WinGdi;

namespace LayoutFarm.UI.Skia
{
    class SkiaCanvasViewport : CanvasViewport
    {
        
        PixelFarm.Drawing.Skia.MySkiaCanvas mySkCanvas;
        int internalSizeW = 800;
        int internalSizwH = 600;

        public SkiaCanvasViewport(RootGraphic rootgfx,
            Size viewportSize, int cachedPageNum)
            : base(rootgfx, viewportSize, cachedPageNum)
        {
            //quadPages = new QuadPages(cachedPageNum, viewportSize.Width, viewportSize.Height * 2);
            this.CalculateCanvasPages();

            mySkCanvas = new PixelFarm.Drawing.Skia.MySkiaCanvas(0, 0, 0, 0, internalSizeW, internalSizwH);
             
        }
       


        //static int dbugCount = 0;
        protected override void OnClosing()
        {
            //if (quadPages != null)
            //{
            //    quadPages.Dispose();
            //    quadPages = null;
            //}
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
        //// get the bitmap
        //CreateBitmap();
        //var data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

        //// create the surface
        //var info = new SKImageInfo(Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
        //using (var surface = SKSurface.Create(info, data.Scan0, data.Stride))
        //{
        //    // start drawing
        //    OnPaintSurface(new SKPaintSurfaceEventArgs(surface, info));

        //    surface.Canvas.Flush();
        //}

        //// write the bitmap to the graphics
        //bitmap.UnlockBits(data);
        //e.Graphics.DrawImage(bitmap, 0, 0);
         
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

            //CreateBitmap();

            //var data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            // create the surface
            //var info = new SkiaSharp.SKImageInfo(Width, Height, SkiaSharp.SKImageInfo.PlatformColorType, SkiaSharp.SKAlphaType.Premul);
            //SkiaSharp.SKBitmap bmp1 = new SkiaSharp.SKBitmap(800, 600);
            //SkiaSharp.SKCanvas ss = new SkiaSharp.SKCanvas(bmp1);
            mySkCanvas.ClearSurface(PixelFarm.Drawing.Color.White);

            UpdateAllArea(mySkCanvas, rootGraphics.TopWindowRenderBox);
            SkiaSharp.SKBitmap backBmp = mySkCanvas.BackBmp;
            // backBmp.LockPixels();
            IntPtr h = backBmp.GetPixels();
            int w1 = backBmp.Width;
            int h1 = backBmp.Height;
            int stride = backBmp.RowBytes;


            byte[] buffer1 = new byte[stride * h1];
            System.Runtime.InteropServices.Marshal.Copy(
                h, buffer1, 0, buffer1.Length
                );
            // backBmp.UnlockPixels();

            //
            System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(w1, h1);
            var bmpdata = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, w1, h1), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(buffer1, 0, bmpdata.Scan0, buffer1.Length);
            bmp1.UnlockBits(bmpdata);
            //if (RenderBoxBase.debugBreaK1)
            //{
            //    bmp1.Save("d:\\WImageTest\\t02.png");
            //}

            //using (var surface = SkiaSharp.SKSurface.Create(info, data.Scan0, data.Stride))
            //{
            //    // start drawing
            //    // OnPaintSurface(new SKPaintSurfaceEventArgs(surface, info));
            //    UpdateAllArea(mySkCanvas, rootGraphics.TopWindowRenderBox);
            //    surface.Canvas.Flush();
            //}

            // write the bitmap to the graphics
            //bitmap.UnlockBits(data);

            //using (System.Drawing.Graphics g2 = ViewControl.CreateGraphics())
            using (System.Drawing.Graphics g2 = System.Drawing.Graphics.FromHdc(hdc))
            {
                //   bitmap.Save("d:\\WImageTest\\t01.png");
                g2.DrawImage(bmp1, 0, 0);
            }


            //e.Graphics.DrawImage(bitmap, 0, 0);

            //if (this.FullMode)
            //{
            //    //quadPages.RenderToOutputWindowFullMode(
            //    //    rootGraphics.TopWindowRenderBox, hdc,
            //    //    this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            //}
            //else
            //{
            //    //temp to full mode
            //    //quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc, this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            //    //quadPages.RenderToOutputWindowPartialMode(
            //    //   rootGraphics.TopWindowRenderBox, hdc,
            //    //   this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
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