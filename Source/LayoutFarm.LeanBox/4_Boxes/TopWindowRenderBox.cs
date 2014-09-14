
//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace LayoutFarm
{


    public abstract class TopWindowRenderBox : RenderBoxBase
    {
        RootGraphic rootGfx;
        VisualPlainLayer groundLayer;
        public TopWindowRenderBox(RootGraphic rootGfx, int width, int height)
            : base(rootGfx, width, height)
        {
            this.rootGfx = rootGfx; 
            groundLayer = new VisualPlainLayer(this);
            this.Layers = new VisualLayerCollection();
            this.Layers.AddLayer(groundLayer);
            SetIsWindowRoot(this, true);
        }
        public void AddChild(RenderElement renderE)
        {
            groundLayer.AddTop(renderE);
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
       
        public abstract void RootBeginGraphicUpdate();
        public abstract void RootEndGraphicUpdate();
        public abstract RenderElement CurrentDraggingElement { get; set; }

        public abstract void AddToLayoutQueue(RenderElement vs);
#if DEBUG
        public abstract RootGraphic dbugVisualRoot { get; }
#endif
        public abstract bool IsLayoutQueueClearing { get; } 
      
        public abstract void InvalidateGraphicArea(RenderElement fromElement, ref Rectangle elementClientRect);

        public override void ClearAllChildren()
        {
            this.groundLayer.Clear();
        }

#if DEBUG
        public abstract void dbugShowRenderPart(Canvas canvasPage, InternalRect updateArea);
#endif
    }
}