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
        RenderElement primaryVisualElement;
        ArtVisualTextEditBox visualTextSurface;
        public UITextBox(int width, int height)
        {

            visualTextSurface = new ArtVisualTextEditBox(width, height, false);
            visualTextSurface.SetStyleDefinition(textBoxRole, null);
            visualTextSurface.HasSpecificSize = true;
            visualTextSurface.SetController(this);
            SetPrimaryVisualElement(visualTextSurface);
        }

        public void SetPrimaryVisualElement(RenderElement visualElement)
        {
            this.primaryVisualElement = visualElement;
        }
        public RenderElement PrimaryVisualElement
        {
            get
            {
                return primaryVisualElement;
            }
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
        public ArtVisualTextEditBox VisualTextSurface
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
        static BoxStyle textBoxRole;

        static UITextBox()
        {
            textBoxRole = InternalVisualRoleHelper.CreateSimpleRole(Color.White);
        }
    }





}