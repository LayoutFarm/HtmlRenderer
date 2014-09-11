
//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm.Presentation
{
    public abstract partial class TopWindowRenderBox : RenderBoxBase
    {
        public TopWindowRenderBox(int width, int height)
            : base(width, height, ElementNature.WindowRoot)
        {
            this.Layers = new VisualLayerCollection();
            this.Layers.AddLayer(new VisualPlainLayer(this));
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
        public abstract dbugRootElement dbugVisualRoot { get; }
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