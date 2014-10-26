﻿//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

using LayoutFarm;
using LayoutFarm.UI;
using LayoutFarm.Text;


namespace LayoutFarm.SampleControls
{

    public class UITextBox : UIBox
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

        protected override void OnKeyPress(UIKeyEventArgs e)
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
            this.Focus();
            visualTextEdit.OnMouseDown(e);
            e.CancelBubbling = true;
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            visualTextEdit.OnMouseUp(e);
        }

        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            visualTextEdit.OnDoubleClick(e);
        }
       
        protected override void OnDragStart(UIMouseEventArgs e)
        {
            visualTextEdit.OnDragStart(e);
        }
        protected override void OnDragging(UIMouseEventArgs e)
        {
            visualTextEdit.OnDrag(e);
        }
        protected override void OnDragStop(UIMouseEventArgs e)
        {
            visualTextEdit.OnDragStop(e);
        }
        public override void InvalidateGraphic()
        {
            if (visualTextEdit != null)
            {
                visualTextEdit.InvalidateGraphic();
            }
        }



    }
}