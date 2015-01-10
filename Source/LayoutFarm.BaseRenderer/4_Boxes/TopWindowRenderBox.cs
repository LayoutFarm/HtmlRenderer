// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{


    public sealed class TopWindowRenderBox : RenderBoxBase, ITopWindowRenderBox
    {   
        VisualPlainLayer groundLayer;
        public TopWindowRenderBox(RootGraphic rootGfx, int width, int height)
            : base(rootGfx, width, height)
        {

            groundLayer = new VisualPlainLayer(this);
            this.Layers = new VisualLayerCollection();
            this.Layers.AddLayer(groundLayer);

            this.IsTopWindow = true;
            this.HasSpecificSize = true;
        }

        public void ChangeRootGraphicSize(int width, int height)
        {
            Size currentSize = this.Size;
            if (currentSize.Width != width || currentSize.Height != height)
            {
                this.SetSize(width, height);

                this.InvalidateContentArrangementFromContainerSizeChanged();
                this.TopDownReCalculateContentSize();
                this.TopDownReArrangeContentIfNeed();
            }
        }

        public void AddChild(RenderElement renderE)
        {
            groundLayer.AddChild(renderE);
        }



        protected override void DrawContent(Canvas canvas, Rectangle updateArea)
        {
            canvas.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
            base.DrawContent(canvas, updateArea);
        }


        public void AddToLayoutQueue(RenderElement vs)
        {
            this.Root.AddToLayoutQueue(vs);
        }

        public void PrepareRender()
        {
            this.Root.PrepareRender();
        }
        //---------------------------------------------------------------------------- 
        public override void ClearAllChildren()
        {
            this.groundLayer.Clear();
        }


#if DEBUG
        public void dbugShowRenderPart(Canvas canvasPage, Rectangle updateArea)
        {
            RootGraphic visualroot = this.dbugVRoot;
            if (visualroot.dbug_ShowRootUpdateArea)
            {
                canvasPage.FillRectangle(Color.FromArgb(50, Color.Black),
                     updateArea.Left, updateArea.Top,
                        updateArea.Width - 1, updateArea.Height - 1);
                canvasPage.FillRectangle(Color.White,
                     updateArea.Left, updateArea.Top, 5, 5);
                canvasPage.DrawRectangle(Color.Yellow,
                        updateArea.Left, updateArea.Top,
                        updateArea.Width - 1, updateArea.Height - 1);

                Color c_color = canvasPage.CurrentTextColor;
                canvasPage.CurrentTextColor = Color.White;
                canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea.Left, updateArea.Top);
                if (updateArea.Height > 25)
                {
                    canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea.Left, updateArea.Top + (updateArea.Height - 20));
                }
                canvasPage.CurrentTextColor = c_color;
                visualroot.dbug_RootUpdateCounter++;
            }
        }
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