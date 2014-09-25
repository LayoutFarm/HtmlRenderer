//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;
using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm.SampleControls
{

    public class UIButton : UIBox
    {
        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;

        public event EventHandler<UIDragEventArgs> Dragging;
        public event EventHandler<UIDragEventArgs> DragStart;
        public event EventHandler<UIDragEventArgs> DragStop;

        CustomRenderBox primElement;
        Color backColor = Color.LightGray;

        public UIButton(int width, int height)
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
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                RenderElement.DirectSetVisualElementLocation(renderE, this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.SetController(this);
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
        protected override void OnDragStart(UIDragEventArgs e)
        {
            if (this.DragStart != null)
            {
                this.DragStart(this, e);
            }
            base.OnDragStart(e);
        }
        protected override void OnDragStop(UIDragEventArgs e)
        {
            if (this.DragStop != null)
            {
                this.DragStop(this, e);
            }
            base.OnDragStop(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }
        protected override void OnDragging(UIDragEventArgs e)
        {
            if (this.Dragging != null)
            {
                Dragging(this, e);
            }
            base.OnDragging(e);
        }
    }


}