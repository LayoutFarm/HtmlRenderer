//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm.SampleControls
{
     
    class CustomRenderBox : RenderBoxBase
    {

        public CustomRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.BackColor = Color.LightGray;
        }
        public override void ClearAllChildren()
        {
        }
        public Color BackColor
        {
            get;
            set;
        }
        public override void CustomDrawToThisPage(Canvas canvasPage, InternalRect updateArea)
        {
            //sample bg
            using (Brush brush = new SolidBrush(BackColor))
            {
                canvasPage.FillRectangle(brush, new Rectangle(0, 0, this.Width, this.Height));
                if (this.Layers != null)
                {
                    this.Layers.LayersDrawContent(canvasPage, updateArea);
                }
            }
        }
    }



}