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

    public class UIButton : UIElement
    {
        public event EventHandler<UIMouseEventArgs> MouseDown;
        RenderElement primaryVisualElement;
        int _left;
        int _top;
        int _width;
        int _height;

        public UIButton(int width, int height)
        {
            this._width = width;
            this._height = height;
        }
        public int Left
        {
            get
            {
                if (this.primaryVisualElement != null)
                {
                    return this.primaryVisualElement.Location.X;
                }
                else
                {
                    return this._left;
                }
            }
        }
        public int Top
        {
            get
            {
                if (this.primaryVisualElement != null)
                {
                    return this.primaryVisualElement.Location.Y;
                }
                else
                {
                    return this._top;
                }
            }
        }
        public void SetLocation(int left, int top)
        {
            this._left = left;
            this._top = top;

            if (this.primaryVisualElement != null)
            {
                RenderElement.DirectSetVisualElementLocation(this.primaryVisualElement, left, top);
            }
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (MouseDown != null)
            {
                MouseDown(this, e);
            }
            base.OnMouseDown(e);
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primaryVisualElement == null)
            {
                primaryVisualElement = new CustomRenderElement(this._width, this._height);
                primaryVisualElement.SetController(this);
                RenderElement.DirectSetVisualElementLocation(primaryVisualElement, _left, _top);
            }
            return primaryVisualElement;
        }
        public override void InvalidateGraphic()
        {
            if (primaryVisualElement != null)
                primaryVisualElement.InvalidateGraphic();
        }
    }


}