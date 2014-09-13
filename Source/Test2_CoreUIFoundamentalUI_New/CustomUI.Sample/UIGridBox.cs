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
    public class UIGridBox : UIElement
    {
        CustomRenderElement simpleBox;
        int _width, _height;
        public UIGridBox(int width, int height)
        {
            this._width = width;
            this._height = height;
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (simpleBox == null)
            {
                simpleBox = new CustomRenderElement(_width, _height);
            }
            return simpleBox;
        }
        public override void InvalidateGraphic()
        {
            if (simpleBox != null)
            {
                simpleBox.InvalidateGraphic();
            }
        }
    }
}