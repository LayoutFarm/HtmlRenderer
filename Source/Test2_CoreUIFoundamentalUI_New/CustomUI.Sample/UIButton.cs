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
        public UIButton(int width, int height)
        {
            primaryVisualElement = new CustomVisualBox(width, height);
            primaryVisualElement.SetController(this);
        }
        public RenderElement PrimaryVisualElement
        {
            get
            {
                return primaryVisualElement;
            }
        }
        public int Left
        {
            get { return this.primaryVisualElement.Location.X; }
        }
        public int Top
        {
            get { return this.primaryVisualElement.Location.Y; }
        }
        public void SetLocation(int left, int top)
        {
            RenderElement.DirectSetVisualElementLocation(this.primaryVisualElement, left, top);
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (MouseDown != null)
            {
                MouseDown(this, e);
            }
            base.OnMouseDown(e);
        }
    }

    class CustomVisualBox : RenderElement
    {

        public CustomVisualBox(int w, int h)
            : base(w, h, ElementNature.Shapes)
        {
        }
        public override void CustomDrawToThisPage(CanvasBase canvasPage, InternalRect updateArea)
        {

            canvasPage.FillRectangle(Brushes.Green, new Rectangle(0, 0, this.Width, this.Height));
        }

    }

}