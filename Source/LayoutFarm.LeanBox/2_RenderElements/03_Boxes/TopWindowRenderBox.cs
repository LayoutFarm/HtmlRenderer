
//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm.Presentation
{
    public abstract partial class TopWindowRenderBox : RenderBoxBase
    {

        VisualLayer groundLayer; 
        public TopWindowRenderBox(int width, int height)
            : base(width, height, ElementNature.WindowRoot)
        { 
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
            if (groundLayer != null)
            {
                groundLayer.Clear();
            }
            ClearAllChildrenInOtherLayers();
        }
        protected override bool HasGroundLayer()
        {
            return groundLayer != null;
        }
        protected override VisualLayer GetGroundLayer()
        {
            return groundLayer;
        }


        protected override void GroundLayerAddChild(RenderElement child)
        {
            if (groundLayer == null)
            {
                groundLayer = new VisualPlainLayer(this);
            }

            groundLayer.AddTop(child);
        }

#if DEBUG
        public abstract void dbugShowRenderPart(CanvasBase canvasPage, InternalRect updateArea);
#endif
    }
}