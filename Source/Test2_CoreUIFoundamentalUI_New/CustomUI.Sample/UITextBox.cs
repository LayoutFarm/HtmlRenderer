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


    public class UITextBox : UIElement
    {
         
        TextEditRenderBox visualTextSurface;
        public UITextBox(int width, int height)
        {

            visualTextSurface = new TextEditRenderBox(width, height, false);

            visualTextSurface.HasSpecificSize = true;
            visualTextSurface.SetController(this);

            
        }
        public override RenderElement PrimaryRenderElement
        {
            get { return this.visualTextSurface; }
        }
        
        public VisualTextRun CurrentTextRun
        {
            get
            {
                return visualTextSurface.CurrentTextRun;
            }
        }
        public TextSurfaceEventListener TextDomListener
        {
            get
            {
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
    }
}