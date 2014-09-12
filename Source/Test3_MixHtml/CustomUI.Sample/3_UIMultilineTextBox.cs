//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LayoutFarm.Presentation;
using LayoutFarm.Presentation.Text;

using LayoutFarm.Presentation.UI;
namespace LayoutFarm.Presentation.SampleControls
{

    public class UIMultiLineTextBox : UIElement
    {
      
        TextEditRenderBox visualTextEdit; 
        public UIMultiLineTextBox(int width, int height, bool multiline)
        {
                 
            visualTextEdit = new TextEditRenderBox(width, height, multiline);
           
            visualTextEdit.HasSpecificSize = true;
             
            visualTextEdit.SetController(this);
            RegisterNativeEvent(
              1 << UIEventIdentifier.NE_MOUSE_DOWN
              | 1 << UIEventIdentifier.NE_LOST_FOCUS
              | 1 << UIEventIdentifier.NE_SIZE_CHANGED
              );
        }

        public override RenderElement PrimaryRenderElement
        {
            get { return this.visualTextEdit; }
        }
       
        protected override void OnKeyPress(UIKeyPressEventArgs e)
        {
            visualTextEdit.OnKeyPress(e);
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            visualTextEdit.OnKeyDown(e);

        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {

        }
        protected override bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            return visualTextEdit.OnProcessDialogKey(e);

        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {

            visualTextEdit.OnMouseDown(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            visualTextEdit.OnMouseUp(e);
        }

        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            visualTextEdit.OnDoubleClick(e);
        }
        protected override void OnDragDrop(UIDragEventArgs e)
        {
        }
        protected override void OnDragStart(UIDragEventArgs e)
        {
            visualTextEdit.OnDragStart(e);
        }
        protected override void OnDragging(UIDragEventArgs e)
        {
            visualTextEdit.OnDrag(e);
        }
        protected override void OnDragStop(UIDragEventArgs e)
        {
            visualTextEdit.OnDragStop(e);
        } 
    }
}