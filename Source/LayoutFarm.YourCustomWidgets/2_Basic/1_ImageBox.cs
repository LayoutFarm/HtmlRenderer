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

        CustomImageRenderBox imgRenderBox;
        ImageBinder imageBinder;
        public ImageBox(int width, int height)
            : base(width, height)
        {

        }
        public ImageBinder ImageBinder
        {
            get { return this.imageBinder; }
            set
            {
                this.imageBinder = value;
                if (this.imgRenderBox != null)
                {
                    this.imgRenderBox.ImageBinder = value;
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
                renderBox.ImageBinder = imageBinder;
                renderBox.SetController(this);
                renderBox.BackColor = this.BackColor;
                SetPrimaryRenderElement(renderBox);
                this.imgRenderBox = renderBox;
            }
            return this.imgRenderBox;
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.imgRenderBox; }
        }
        protected override void OnContentUpdate()
        {
            if (imageBinder.State == ImageBinderState.Loaded)
            {
                this.SetSize(this.imageBinder.ImageWidth, this.imageBinder.ImageHeight);
                this.ParentUI.InvalidateLayout();
            }
        }
    }
}
