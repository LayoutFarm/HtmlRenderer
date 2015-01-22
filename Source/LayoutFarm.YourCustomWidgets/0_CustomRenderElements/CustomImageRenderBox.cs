// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.CustomWidgets
{

    public class CustomImageRenderBox : CustomRenderBox
    {

#if DEBUG
        public bool dbugBreak;
#endif
        ImageBinder imageBinder;
        public CustomImageRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.BackColor = Color.LightGray;
        }
        public override void ClearAllChildren()
        {

        }

        public ImageBinder ImageBinder
        {
            get { return this.imageBinder; }
            set { this.imageBinder = value; }
        }
        protected override void DrawContent(Canvas canvas, Rectangle updateArea)
        {
            if (this.imageBinder != null)
            {
                switch (imageBinder.State)
                {
                    case ImageBinderState.Loaded:
                        {
                            canvas.DrawImage(imageBinder.Image,
                                new RectangleF(0, 0, this.Width, this.Height));
                        } break;

                }

            }
            else
            {
                //when no image
                //canvasPage.FillRectangle(BackColor, updateArea._left, updateArea._top, updateArea.Width, updateArea.Height);
            }
#if DEBUG
            //canvasPage.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));
#endif
        }
    }



}