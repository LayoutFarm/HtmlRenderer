//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

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
        protected override void BoxDrawContent(Canvas canvasPage, Rect updateArea)
        {

            //sample bg
            using (var brush = LayoutFarm.Drawing.CurrentGraphicPlatform.CreateSolidBrush(BackColor))
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