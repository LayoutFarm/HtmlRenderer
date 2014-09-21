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
#if DEBUG
        public bool dbugBreak;
#endif
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
        protected override void BoxDrawContent(Canvas canvasPage, InternalRect updateArea)
        {

            //sample bg
            using (Brush brush = new SolidBrush(BackColor))
            {
                canvasPage.FillRectangle(brush, updateArea.ToRectangle());
                if (this.Layers != null)
                {
                    this.Layers.LayersDrawContent(canvasPage, updateArea);
                }
            }
        }


    }



}