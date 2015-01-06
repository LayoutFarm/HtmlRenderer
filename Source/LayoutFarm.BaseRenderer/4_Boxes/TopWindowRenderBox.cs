
//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
namespace LayoutFarm
{


    public abstract class TopWindowRenderBox : RenderBoxBase, ITopWindowRenderBox
    {

        VisualPlainLayer groundLayer;
        public event EventHandler<EventArgs> CanvasForcePaint;
        public TopWindowRenderBox(RootGraphic rootGfx, int width, int height)
            : base(rootGfx, width, height)
        {

            groundLayer = new VisualPlainLayer(this);
            this.Layers = new VisualLayerCollection();
            this.Layers.AddLayer(groundLayer);
            SetIsWindowRoot(this, true);
            this.HasSpecificSize = true;
        }

        public abstract void SetCanvasInvalidateRequest(CanvasInvalidateRequestDelegate canvasInvaliddateReqDel);
        public abstract void ChangeRootGraphicSize(int width, int height);

        public void AddChild(RenderElement renderE)
        {
            groundLayer.AddChild(renderE);
        }
        public void ForcePaint()
        {
            if (this.CanvasForcePaint != null)
            {
                CanvasForcePaint(this, EventArgs.Empty);
            }
        }


        protected override void DrawContent(Canvas canvas, Rect updateArea)
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
            CurrentTopWindowRenderBox = this;
        }

        public static TopWindowRenderBox CurrentTopWindowRenderBox
        {
            get;
            set;
        }

#if DEBUG
        public abstract void dbugShowRenderPart(Canvas canvasPage, Rect updateArea);
        public RootGraphic dbugVisualRoot
        {
            get
            {
                return this.Root;
            }
        }
#endif
    }
}