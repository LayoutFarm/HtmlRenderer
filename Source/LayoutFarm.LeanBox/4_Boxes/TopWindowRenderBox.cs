
//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm
{
    public abstract partial class TopWindowRenderBox : RenderBoxBase
    {
        RootGraphic rootGfx;
        public TopWindowRenderBox(RootGraphic rootGfx, int width, int height)
            : base(width, height)
        {
            this.rootGfx = rootGfx;
            this.Layers = new VisualLayerCollection();
            this.Layers.AddLayer(new VisualPlainLayer(this));

            SetIsWindowRoot(this, true);
        }
        public RootGraphic RootGraphic
        {
            get { return this.rootGfx; }
        }
        public abstract RenderElement CurrentKeyboardFocusedElement
        {
            get;
            set;
        }
        public abstract Graphics CreateGraphics();

        public abstract void RootBeginGraphicUpdate();
        public abstract void RootEndGraphicUpdate();
        public abstract RenderElement CurrentDraggingElement { get; set; }

        public abstract void AddToLayoutQueue(RenderElement vs);
#if DEBUG
        public abstract RootGraphic dbugVisualRoot { get; }
#endif
        public abstract bool IsLayoutQueueClearing { get; }

        public abstract void InvalidateGraphicArea(RenderElement fromElement, InternalRect elementClientRect);

        public override void ClearAllChildren()
        {
            this.Layers.ClearAllContentInEachLayer();

        }



#if DEBUG
        public abstract void dbugShowRenderPart(CanvasBase canvasPage, InternalRect updateArea);
#endif
    }
}