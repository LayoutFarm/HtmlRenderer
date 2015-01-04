//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
    public class UIImageBox : UIEaseBox
    {
        Image image;//image to draw   
        CustomImageRenderBox imgRenderBox;
        public UIImageBox(int width, int height)
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
                renderBox.Image = this.image;
                renderBox.SetController(this);
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
