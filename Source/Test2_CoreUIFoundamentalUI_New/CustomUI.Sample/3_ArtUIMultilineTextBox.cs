//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LayoutFarm.Presentation;
using LayoutFarm.Presentation.Text;


namespace LayoutFarm.Presentation.SampleControls
{

    public class ArtUIMultiLineTextBox : ArtUIElement
    {
        ArtVisualElement primaryVisualElement;
        ArtVisualTextEditBox visualTextEdit; 
        public ArtUIMultiLineTextBox(int width, int height, bool multiline)
        {
                 
            visualTextEdit = new ArtVisualTextEditBox(width, height, multiline);
            visualTextEdit.SetStyleDefinition(textBoxRole, null);
            visualTextEdit.HasSpecificSize = true;
            this.SetPrimaryVisualElement(visualTextEdit);
            visualTextEdit.SetController(this);
            RegisterNativeEvent(
              1 << ArtEventIdentifier.NE_MOUSE_DOWN
              | 1 << ArtEventIdentifier.NE_LOST_FOCUS
              | 1 << ArtEventIdentifier.NE_SIZE_CHANGED
              );
        }
        public ArtVisualElement PrimaryVisualElement
        {
            get
            {
                return primaryVisualElement;
            }
        } 
        public void SetPrimaryVisualElement(ArtVisualElement visualElement)
        {
            this.primaryVisualElement = visualElement;
        }
        protected override void OnKeyPress(ArtKeyPressEventArgs e)
        {
            visualTextEdit.OnKeyPress(e);
        }
        protected override void OnKeyDown(ArtKeyEventArgs e)
        {
            visualTextEdit.OnKeyDown(e);

        }
        protected override void OnKeyUp(ArtKeyEventArgs e)
        {

        }
        protected override bool OnProcessDialogKey(ArtKeyEventArgs e)
        {
            return visualTextEdit.OnProcessDialogKey(e);

        }
        protected override void OnMouseDown(ArtMouseEventArgs e)
        {

            visualTextEdit.OnMouseDown(e);
        }
        protected override void OnMouseUp(ArtMouseEventArgs e)
        {
            visualTextEdit.OnMouseUp(e);
        }

        protected override void OnDoubleClick(ArtMouseEventArgs e)
        {
            visualTextEdit.OnDoubleClick(e);
        }
        protected override void OnDragDrop(ArtDragEventArgs e)
        {
        }
        protected override void OnDragStart(ArtDragEventArgs e)
        {
            visualTextEdit.OnDragStart(e);
        }
        protected override void OnDragging(ArtDragEventArgs e)
        {
            visualTextEdit.OnDrag(e);
        }
        protected override void OnDragStop(ArtDragEventArgs e)
        {
            visualTextEdit.OnDragStop(e);
        }

        static BoxStyle textBoxRole;
        static ArtUIMultiLineTextBox()
        {

            textBoxRole = InternalVisualRoleHelper.CreateSimpleRole(Color.White);
        }


    }
}