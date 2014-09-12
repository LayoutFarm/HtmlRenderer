//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Presentation.Text;
using LayoutFarm.Presentation.UI;

namespace LayoutFarm.Presentation.SampleControls
{
    class CustomRenderElement : RenderElement
    {
        public CustomRenderElement(int w, int h)
            : base(w, h)
        {

        }
        public override void CustomDrawToThisPage(CanvasBase canvasPage, InternalRect updateArea)
        {

            canvasPage.FillRectangle(Brushes.Green, new Rectangle(0, 0, this.Width, this.Height));
        }
    }

}