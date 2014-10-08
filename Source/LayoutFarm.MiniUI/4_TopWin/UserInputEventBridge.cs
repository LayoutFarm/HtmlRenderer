//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing; 

namespace LayoutFarm.UI
{

    public abstract class UserInputEventBridge
    {
        public abstract void Bind(TopWindowRenderBox topwin); 
        public abstract void ClearAllFocus(); 
        //--------------------------------------------------
        public abstract void OnDoubleClick(UIMouseEventArgs e);
        public abstract void OnMouseWheel(UIMouseEventArgs e);
        public abstract void OnMouseDown(UIMouseEventArgs e);
        public abstract void OnMouseMove(UIMouseEventArgs e);

        public abstract void OnDragStart(UIDragEventArgs e);
        public abstract void OnDrag(UIDragEventArgs e);
        
     
        public abstract void OnDragStop(UIDragEventArgs e);

        public abstract void OnGotFocus(UIFocusEventArgs e);
        public abstract void OnLostFocus(UIFocusEventArgs e);
        public abstract void OnMouseUp(UIMouseEventArgs e);
        public abstract void OnKeyDown(UIKeyEventArgs e);
        public abstract void OnKeyUp(UIKeyEventArgs e);
        public abstract void OnKeyPress(UIKeyPressEventArgs e);
        public abstract bool OnProcessDialogKey(UIKeyEventArgs e); 
    }


}