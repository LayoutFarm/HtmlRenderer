//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Agg;
namespace LayoutFarm.CustomWidgets
{
    public class MyMiniAggCanvasRenderElement : RenderBoxBase, IDisposable
    {
        Graphics2D gfx2d;
        bool needUpdate;
        List<BasicSprite> sprites = new List<BasicSprite>();
        ActualImage actualImage;

        public MyMiniAggCanvasRenderElement(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.actualImage = new ActualImage(width, height, PixelFarm.Agg.PixelFormat.ARGB32);
            this.gfx2d = Graphics2D.CreateFromImage(actualImage);
            needUpdate = true;
        }
        public override void ClearAllChildren()
        {
        }
        public Color BackColor
        {
            get;
            set;
        }

        public void AddSprite(BasicSprite sprite)
        {
            this.sprites.Add(sprite);
            needUpdate = true;
        }
        protected override void DrawBoxContent(Canvas canvas, Rectangle updateArea)
        {
            //if (this.image != null)
            //{
            //    canvas.DrawImage(this.image,
            //        new RectangleF(0, 0, this.Width, this.Height));
            //}
            //else
            //{
            //when no image
            //---------------------

            // canvas.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
            if (needUpdate)
            {
                //default bg => transparent !,

                //gfx2d.Clear(ColorRGBA.White);//if want opaque bg
                ReleaseUnmanagedResources();
                int j = sprites.Count;
                for (int i = 0; i < j; ++i)
                {
                    sprites[i].OnDraw(gfx2d);
                }

                //this.bmp = new Bitmap(this.Width, this.Height, this.actualImage.GetBuffer(), true);
                needUpdate = false;
            }
            canvas.DrawImage(actualImage, new RectangleF(0, 0, this.Width, this.Height));
            //---------------------
            //copy data from actual image to canvas 

            //}
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
            //if (bmp != null)
            //{
            //    bmp.Dispose();
            //    bmp = null;
            //}
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

        //static void CopyFromAggActualImageToGdiPlusBitmap(ActualImage aggActualImage, System.Drawing.Bitmap bitmap)
        //{
        //    //platform specific
        //    var bmpdata = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //         System.Drawing.Imaging.ImageLockMode.ReadOnly,
        //         System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        //    var aggBuffer = aggActualImage.GetBuffer();
        //    System.Runtime.InteropServices.Marshal.Copy(aggBuffer, 0,
        //        bmpdata.Scan0, aggBuffer.Length);

        //    bitmap.UnlockBits(bmpdata);
        //}
    }
}