//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm;
using LayoutFarm.UI;
using LayoutFarm.Text;


namespace LayoutFarm.CustomWidgets
{

    public class UITextBox : UIBox, IUserEventPortal
    {

        TextEditRenderBox visualTextEdit;
        bool _multiline;


        public UITextBox(int width, int height, bool multiline)
            : base(width, height)
        {
            this._multiline = multiline;
        }
        public override bool AcceptKeyboardFocus
        {
            get
            {
                return true;
            }
        }
        public void Focus()
        {
            //request keyboard focus
            visualTextEdit.Focus();
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.visualTextEdit != null; }
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.visualTextEdit; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (visualTextEdit == null)
            {
                var tbox = new TextEditRenderBox(rootgfx, this.Width, this.Height, _multiline);
                RenderElement.DirectSetVisualElementLocation(tbox, this.Left, this.Top);

                tbox.HasSpecificSize = true;

                tbox.SetController(this);
                RegisterNativeEvent(
                  1 << UIEventIdentifier.NE_MOUSE_DOWN
                  | 1 << UIEventIdentifier.NE_LOST_FOCUS
                  | 1 << UIEventIdentifier.NE_SIZE_CHANGED
                  );

                this.visualTextEdit = tbox;
            }
            return visualTextEdit;
        }
         
        protected override void OnMouseLeave(UIMouseEventArgs e)
        {
            e.MouseCursorStyle = MouseCursorStyle.Arrow;
            e.CancelBubbling = true;
        }
        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            visualTextEdit.OnDoubleClick(e);
        }
        public override void InvalidateGraphic()
        {
            if (visualTextEdit != null)
            {
                visualTextEdit.InvalidateGraphic();
            }
        }

        protected override void OnKeyPress(UIKeyEventArgs e)
        {
            visualTextEdit.OnKeyPress(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            visualTextEdit.OnKeyDown(e);
            e.CancelBubbling = true;

        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            e.CancelBubbling = true;
        }
        protected override bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            if (visualTextEdit.OnProcessDialogKey(e))
            {
                e.CancelBubbling = true;
                return true;
            }
            return false;
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            this.isMouseDown = true;

            this.Focus();
            e.MouseCursorStyle = MouseCursorStyle.IBeam;
            e.CancelBubbling = true;
            e.CurrentContextElement = this;
            visualTextEdit.OnMouseDown(e);
        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            visualTextEdit.OnMouseUp(e);
            this.isMouseDown = this.isDragging = false;
            e.MouseCursorStyle = MouseCursorStyle.Default;
            e.CancelBubbling = true;
        }

        protected override void OnDragBegin(UIMouseEventArgs e)
        {
            visualTextEdit.OnDragBegin(e);
            e.CancelBubbling = true;
            e.MouseCursorStyle = MouseCursorStyle.IBeam;
        }
        protected override void OnDragging(UIMouseEventArgs e)
        {
            visualTextEdit.OnDrag(e);
            e.CancelBubbling = true;
            e.MouseCursorStyle = MouseCursorStyle.IBeam;
        }
        protected override void OnDragEnd(UIMouseEventArgs e)
        {
            visualTextEdit.OnDragEnd(e);
            this.isMouseDown = this.isDragging = false;
            e.MouseCursorStyle = MouseCursorStyle.Default;
            e.CancelBubbling = true;
        }
        //------------------------------------------------------
        bool isMouseDown;
        bool isDragging;

        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            this.OnKeyPress(e);
        } 
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            this.OnKeyDown(e);
        } 
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            this.OnKeyUp(e); 
        }

        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return this.OnProcessDialogKey(e);
        } 
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            this.OnMouseDown(e);  
        } 
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            if (this.isMouseDown)
            {
                if (isDragging)
                {
                    this.OnDragging(e);                   
                }
                else
                {                    
                    isDragging = true;
                    this.OnDragBegin(e);                    
                }
            }
            else
            {
                this.OnMouseMove(e);                
            } 
        }
        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            if (isDragging)
            {
                this.OnDragEnd(e);                
            }
            else
            {
                this.OnMouseUp(e);     
            }  
        }

        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {

        }

        void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {

        }

        void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {

        }
    }
}