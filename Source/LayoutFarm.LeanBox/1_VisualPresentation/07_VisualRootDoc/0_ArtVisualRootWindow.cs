
//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm.Presentation
{
    public abstract partial class ArtVisualRootWindow : ArtVisualContainerBase
    {
        VisualRoot visualRoot;
        VisualLayer groundLayer;

        public ArtVisualRootWindow(VisualRoot visualRoot, int width, int height)
            : base(width, height, VisualElementNature.WindowRoot)
        {
            this.visualRoot = visualRoot;
        }

        public abstract ArtVisualElement CurrentKeyboardFocusedElement
        {
            get;
            set;
        }
        public abstract Graphics CreateGraphics();

        public abstract void RootBeginGraphicUpdate();
        public abstract void RootEndGraphicUpdate();
        public abstract ArtVisualElement CurrentDraggingElement { get; set; }

        public abstract void AddToLayoutQueue(ArtVisualElement vs);
        public abstract VisualRoot VisualRoot { get; }
        public abstract bool IsLayoutQueueClearing { get; }
        public abstract void InvalidateGraphicArea(ArtVisualElement fromElement, InternalRect elementClientRect);

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


        protected override void GroundLayerAddChild(ArtVisualElement child)
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