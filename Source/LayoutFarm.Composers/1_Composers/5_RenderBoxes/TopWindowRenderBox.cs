// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm
{

    sealed class TopWindowRenderBox : RenderBoxBase
    {
        public TopWindowRenderBox(RootGraphic rootGfx, int width, int height)
            : base(rootGfx, width, height)
        {
            this.IsTopWindow = true;
            this.HasSpecificSize = true;
        }
        protected override void DrawContent(Canvas canvas, Rectangle updateArea)
        {
            canvas.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
            this.DrawDefaultLayer(canvas, ref updateArea);
        }
    }

}