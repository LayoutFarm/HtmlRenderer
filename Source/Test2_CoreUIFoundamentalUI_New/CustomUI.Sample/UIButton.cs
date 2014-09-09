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
        ArtVisualElement primaryVisualElement;
        public UIButton(int width, int height)
        {
            primaryVisualElement = new ArtVisualBox(width, height);
        }
        public ArtVisualElement PrimaryVisualElement
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
            ArtVisualElement.DirectSetVisualElementLocation(this.primaryVisualElement, left, top);
        }


    }

    class ArtVisualBox : ArtVisualElement
    {

        public ArtVisualBox(int w, int h)
            : base(w, h, VisualElementNature.Shapes)
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