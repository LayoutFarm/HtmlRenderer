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

    public class UIButton : UIBox
    {
        CustomRenderElement primElement;
        public UIButton(int width, int height)
            : base(width, height)
        {

        }

        protected override bool HasReadyRenderElement
        {
            get { return this.primElement != null; }
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                var renderE = new CustomRenderElement(this.Width, this.Height);
                renderE.SetController(this);
                RenderElement.DirectSetVisualElementLocation(renderE, this.Left, this.Top);

                primElement = renderE;
            }
            return primElement;
        }

    }


}