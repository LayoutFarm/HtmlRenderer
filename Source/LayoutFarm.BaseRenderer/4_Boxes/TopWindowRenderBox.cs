// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{


    public abstract class TopWindowRenderBox : RenderBoxBase, ITopWindowRenderBox
    {

        VisualPlainLayer groundLayer;
        PaintToOutputDelegate paintToOutputHandler;
        public TopWindowRenderBox(RootGraphic rootGfx, int width, int height)
            : base(rootGfx, width, height)
        {

            groundLayer = new VisualPlainLayer(this);
            this.Layers = new VisualLayerCollection();
            this.Layers.AddLayer(groundLayer);

            this.IsTopWindow = true;
            this.HasSpecificSize = true;
        }

        public void SetPaintToOutputDelegate(PaintToOutputDelegate paintToOutputHandler)
        {
            this.paintToOutputHandler = paintToOutputHandler;
        }
        public void ForcePaint()
        {
            paintToOutputHandler();
        }
        public abstract void SetCanvasInvalidateRequest(CanvasInvalidateRequestDelegate canvasInvaliddateReqDel);
        public abstract void ChangeRootGraphicSize(int width, int height);

        public void AddChild(RenderElement renderE)
        {
            groundLayer.AddChild(renderE);
        }
       


        protected override void DrawContent(Canvas canvas, Rectangle updateArea)
        {
            canvas.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
            base.DrawContent(canvas, updateArea);
        }


        //----------------------------------------------------------------------------
        //public abstract void RootBeginGraphicUpdate();
        //public abstract void RootEndGraphicUpdate();
        public abstract void AddToLayoutQueue(RenderElement vs);
        public abstract void FlushGraphic(Rectangle rect);
        public abstract void PrepareRender();
        //---------------------------------------------------------------------------- 
        public override void ClearAllChildren()
        {
            this.groundLayer.Clear();
        }
        public void MakeCurrentTopWindow()
        {

            CurrentActiveTopWindow = this;
        }

#if DEBUG
        public abstract void dbugShowRenderPart(Canvas canvasPage, Rectangle updateArea);
        public RootGraphic dbugVisualRoot
        {
            get
            {
                return this.Root;
            }
        }
#endif


        public static TopWindowRenderBox CurrentActiveTopWindow { get; private set; }
    }
}