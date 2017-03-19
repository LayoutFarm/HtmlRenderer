//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Agg;
using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class MiniAggCanvasRenderElement : RenderBoxBase, IDisposable
    {
        Graphics2D gfx2d;
        CanvasPainter painter;
        bool needUpdate;
        ActualImage actualImage;
        Image bmp;

        public MiniAggCanvasRenderElement(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {

            this.actualImage = new ActualImage(width, height, PixelFarm.Agg.PixelFormat.ARGB32);
            this.gfx2d = Graphics2D.CreateFromImage(actualImage);
            this.painter = new AggCanvasPainter((ImageGraphics2D)gfx2d);
            needUpdate = true;
            this.BackColor = Color.White;
        }
        public override void ClearAllChildren()
        {
        }
        public Color BackColor
        {
            get;
            set;
        }
        protected override void DrawBoxContent(Canvas canvas, Rectangle updateArea)
        {
            // canvas.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
            if (needUpdate)
            {
                //default bg => transparent !, 
                //gfx2d.Clear(ColorRGBA.White);//if want opaque bg
                ReleaseUnmanagedResources();
                if (bmp != null)
                {
                    bmp.Dispose();
                }

                this.bmp = this.actualImage;// new Bitmap(this.Width, this.Height, this.actualImage.GetBuffer(), false);
                // canvas.Platform.CreatePlatformBitmap(this.Width, this.Height, this.actualImage.GetBuffer(), false);
                Image.SetCacheInnerImage(bmp, null);
                needUpdate = false;
            }
            //canvas.FillRectangle(this.BackColor, 0, 0, this.Width, this.Height);

            if (bmp != null)
            {
                canvas.DrawImage(this.bmp, new RectangleF(0, 0, this.Width, this.Height));
            }
            //---------------------



#if DEBUG
            //canvasPage.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));
#endif
        }
        void ReleaseUnmanagedResources()
        {
            //-------------------------
            //TODO: review this again 
            //about resource mx
            //------------------------- 
            if (bmp != null)
            {
                bmp.Dispose();
                bmp = null;
            }
            //if (currentGdiPlusBmp != null)
            //{
            //    currentGdiPlusBmp.Dispose();
            //    currentGdiPlusBmp = null;
            //}
        }
        void IDisposable.Dispose()
        {
            ReleaseUnmanagedResources();
        }


        public CanvasPainter Painter
        {
            get
            {
                //context
                return this.painter;
            }
        }
        public void InvalidateCanvasContent()
        {
            this.needUpdate = true;
            this.InvalidateGraphics();
        }


    }
}