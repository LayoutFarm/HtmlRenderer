//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Presentation.Text;

namespace LayoutFarm.Presentation.SampleControls
{


    public class UIButton : UIElement
    {
        RenderElement primaryVisualElement;
        public UIButton(int width, int height)
        {
            primaryVisualElement = new CustomVisualBox(width, height);
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
    }

    class CustomVisualBox : RenderElement
    {

        public CustomVisualBox(int w, int h)
            : base(w, h, ElementNature.Shapes)
        {

        }

        public override void ClearAllChildren()
        {

        }

        public override void CustomDrawToThisPage(CanvasBase canvasPage, InternalRect updateArea)
        {
            canvasPage.FillRectangle(Brushes.Green, new Rectangle(0, 0, this.Width, this.Height));
        }
    }

}