// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

using PixelFarm.Agg;

namespace LayoutFarm.CustomWidgets
{

   
    public class MiniAggCanvasRenderElement : RenderBoxBase, IDisposable
    {


<<<<<<< HEAD
 
        Graphics2D gfx2d;

        bool needUpdate;

 
        Graphics2D gfx2d; 
        bool needUpdate; 
 
=======
        Graphics2D gfx2d; 
        bool needUpdate; 
>>>>>>> v_dev
        List<BasicSprite> sprites = new List<BasicSprite>();

        ActualImage actualImage;
        Bitmap bmp;
        System.Drawing.Bitmap currentGdiPlusBmp;

        public MiniAggCanvasRenderElement(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.actualImage = new ActualImage(width, height, PixelFarm.Agg.Image.PixelFormat.Rgba32);
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
        protected override void DrawContent(Canvas canvas, Rectangle updateArea)
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
                 
                //---------------------------------
                var buffer = actualImage.GetBuffer();
                if (currentGdiPlusBmp != null)
                {
                    this.currentGdiPlusBmp.Dispose();
                }

                this.currentGdiPlusBmp = new System.Drawing.Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                CopyFromAggActualImageToGdiPlusBitmap(this.actualImage, this.currentGdiPlusBmp);
                this.currentGdiPlusBmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
                this.bmp = new Bitmap(this.Width, this.Height, currentGdiPlusBmp);

                needUpdate = false;
            }
            canvas.DrawImage(this.bmp, new RectangleF(0, 0, this.Width, this.Height));
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
            if (bmp != null)
            {
                bmp.Dispose();
                bmp = null;
            }
            if (currentGdiPlusBmp != null)
            {
                currentGdiPlusBmp.Dispose();
                currentGdiPlusBmp = null;
            }
        }
        void IDisposable.Dispose()
        {
            ReleaseUnmanagedResources();
        }

        static void CopyFromAggActualImageToGdiPlusBitmap(ActualImage aggActualImage, System.Drawing.Bitmap bitmap)
        {
            //platform specific
            var bmpdata = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                 System.Drawing.Imaging.ImageLockMode.ReadOnly,
                 System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var aggBuffer = aggActualImage.GetBuffer();
            System.Runtime.InteropServices.Marshal.Copy(aggBuffer, 0,
                bmpdata.Scan0, aggBuffer.Length);

            bitmap.UnlockBits(bmpdata);
        }


        
    }



}