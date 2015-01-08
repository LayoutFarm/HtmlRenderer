// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;


using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{

    public class CustomImageRenderBox : RenderBoxes.RenderBoxBase
    {

#if DEBUG
        public bool dbugBreak;
#endif
        Image image;
        public CustomImageRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.BackColor = Color.LightGray;
        }
        public override void ClearAllChildren()
        {

        }

        public Image Image
        {
            get { return this.image; }
            set { this.image = value; }
        }
        public Color BackColor
        {
            get;
            set;
        }
        protected override void DrawContent(Canvas canvas, Rectangle updateArea)
        {
            if (this.image != null)
            {
                canvas.DrawImage(this.image,
                    new RectangleF(0, 0, this.Width, this.Height));
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