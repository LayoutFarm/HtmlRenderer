// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{

    public class EaseBox : UIBox
    {
        bool draggable;
        bool dropable;
        CustomRenderBox primElement;
        Color backColor = Color.LightGray;


        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseMove;
        public event EventHandler<UIMouseEventArgs> MouseUp;

        public event EventHandler<UIMouseEventArgs> DragRelease;
        public event EventHandler<UIMouseEventArgs> MouseLeave;
        public event EventHandler<UIMouseEventArgs> LostSelectedFocus;


        public EaseBox(int width, int height)
            : base(width, height)
        {

        }


        protected override bool HasReadyRenderElement
        {
            get { return this.primElement != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                if (HasReadyRenderElement)
                {
                    this.primElement.BackColor = value;
                }
            }
        }
        protected void SetPrimaryRenderElement(CustomRenderBox primElement)
        {
            this.primElement = primElement;
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                renderE.SetController(this);

                renderE.BackColor = backColor;
                renderE.SetLocation(this.Left, this.Top);
                renderE.SetVisible(this.Visible);

                primElement = renderE;
            }
            return primElement;
        }
        //----------------------------------------------------

        public bool AcceptKeyboardFocus
        {
            get;
            set;
        }

        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            this.MouseCaptureX = e.X;
            this.MouseCaptureY = e.Y;

            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }

            if (this.AcceptKeyboardFocus)
            {
                this.Focus();
            }
        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (this.MouseMove != null)
            {
                this.MouseMove(this, e);
            }
        }
        protected override void OnMouseLeave(UIMouseEventArgs e)
        {
            if (this.MouseLeave != null)
            {
                this.MouseLeave(this, e);
            }
        }
        protected override void OnDragRelease(UIMouseEventArgs e)
        {
            if (DragRelease != null)
            {
                DragRelease(this, e);
            }
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
        }
        protected override void OnLostSelectedFocus(UIMouseEventArgs e)
        {
            if (this.LostSelectedFocus != null)
            {
                this.LostSelectedFocus(this, e);
            }
        }

        public bool Draggable
        {
            get { return this.draggable; }
            set
            {
                this.draggable = value;
            }
        }
        public bool Droppable
        {
            get { return this.dropable; }
            set
            {
                this.dropable = value;
            }
        }
        public int MouseCaptureX
        {
            get;
            set;
        }
        public int MouseCaptureY
        {
            get;
            set;
        }
        public void RemoveSelf()
        {    
            var parentBox = this.CurrentPrimaryRenderElement.ParentRenderElement as LayoutFarm.RenderElement;
            if (parentBox != null)
            {
                parentBox.RemoveChild(this.CurrentPrimaryRenderElement);
            } 
            this.InvalidateOuterGraphics(); 
        }
    }


}