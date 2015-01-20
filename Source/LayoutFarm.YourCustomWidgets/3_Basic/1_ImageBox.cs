// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
    public class ImageBox : EaseBox
    {
        Image image;//image to draw   
        CustomImageRenderBox imgRenderBox;
        public ImageBox(int width, int height)
            : base(width, height)
        {

        }
        public Image Image
        {
            get { return this.image; }
            set
            {
                this.image = value;
                if (this.imgRenderBox != null)
                {
                    this.imgRenderBox.Image = value;
                    this.InvalidateGraphics();
                }
            }
        }
        protected override bool HasReadyRenderElement
        {
            get { return imgRenderBox != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (imgRenderBox == null)
            {
                var renderBox = new CustomImageRenderBox(rootgfx, this.Width, this.Height);
                renderBox.SetLocation(this.Left, this.Top);

                renderBox.Image = this.image;
                renderBox.SetController(this);
                renderBox.BackColor = this.BackColor;                                 
                SetPrimaryRenderElement(renderBox);
                this.imgRenderBox = renderBox;
            }
            return this.imgRenderBox;
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.imgRenderBox; }
        }

    }
}
