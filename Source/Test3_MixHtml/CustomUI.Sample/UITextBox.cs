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


    public class UITextBox : UIElement
    {
        int _width, _height;
        TextEditRenderBox visualTextSurface;
        public UITextBox(int width, int height)
        {

            this._width = width;
            this._height = height;
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (this.visualTextSurface == null)
            {
                visualTextSurface = new TextEditRenderBox(rootgfx, _width, _height, false);
                visualTextSurface.HasSpecificSize = true;
                visualTextSurface.SetController(this);
                //-------------------------------------- 
            }
            return visualTextSurface;
        }


        public TextSurfaceEventListener TextDomListener
        {
            get
            {
                if (this.visualTextSurface == null)
                {
                    return null;
                }
                return this.visualTextSurface.TextDomListener;
            }
        }
        public TextEditRenderBox VisualTextSurface
        {
            get
            {

                return this.visualTextSurface;
            }
        }

        public int CurrentLineId
        {
            get
            {
                return visualTextSurface.CurrentLineNumber;
            }
        }
        public int CurrentLineCharIndex
        {
            get
            {

                return visualTextSurface.CurrentLineCharIndex;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return visualTextSurface.CurrentTextRunCharIndex;
            }
        }
        public override void InvalidateGraphic()
        {
            if (visualTextSurface != null)
                visualTextSurface.InvalidateGraphic();
        }
    }
}