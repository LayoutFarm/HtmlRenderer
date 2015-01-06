//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;
using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{

    public class UIEaseBox : UIBox
    {
        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseMove;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        public event EventHandler<UIMouseEventArgs> MouseLeave;

        public event EventHandler<UIMouseEventArgs> Dragging;
        public event EventHandler<UIMouseEventArgs> DragLeave;
        public event EventHandler<UIMouseEventArgs> DragBegin;
        public event EventHandler<UIMouseEventArgs> DragEnd;

        CustomRenderBox primElement;
        Color backColor = Color.LightGray;

        public UIEaseBox(int width, int height)
            : base(width, height)
        {

        }

        protected override bool HasReadyRenderElement
        {
            get { return this.primElement != null; }
        }
        protected override RenderElement CurrentPrimaryRenderElement
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
                RenderElement.DirectSetVisualElementLocation(renderE, this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.SetController(this);
                renderE.SetVisible(this.Visible);
                
                primElement = renderE;
            }
            return primElement;
        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
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
        protected override void OnDragLeave(UIMouseEventArgs e)
        {
            if (this.DragLeave != null)
            {
                this.DragLeave(this, e);
            }
        }
        protected override void OnDragBegin(UIMouseEventArgs e)
        {
            if (this.DragBegin != null)
            {
                this.DragBegin(this, e);
            }

        }
        protected override void OnDragEnd(UIMouseEventArgs e)
        {
            if (this.DragEnd != null)
            {
                this.DragEnd(this, e);
            }

        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
        }
        protected override void OnDragging(UIMouseEventArgs e)
        {
            if (this.Dragging != null)
            {
                Dragging(this, e);
            }
        }
        //----------------------------------------------------
        //for general data
        public object Tag { get; set; }
    }


}