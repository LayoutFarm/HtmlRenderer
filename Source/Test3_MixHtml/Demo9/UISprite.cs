//MIT, 2018-present, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using LayoutFarm.RenderBoxes;
using PaintLab.Svg;

namespace LayoutFarm.UI
{
    class VgBridgeRenderElement : RenderElement
    {
        PaintLab.Svg.VgRenderVx _vgRenderVx;

        public VgBridgeRenderElement(RootGraphic rootGfx, int width, int height)
            : base(rootGfx, width, height)
        {

            //this.dbug_ObjectNote = "AAA";
            //this.NeedClipArea = false;
            this.MayHasChild = true;
            this.TransparentForAllEvents = true;
        }

        public PaintLab.Svg.VgRenderVx VgRenderVx
        {
            get { return _vgRenderVx; }
            set
            {
                _vgRenderVx = value;
            }
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            RectD bound = new RectD();

            if (bound.Contains(hitChain.TestPoint.x, hitChain.TestPoint.y))
            {
                //check exact hit or the vxs part
                if (HitTestOnSubPart(this, hitChain.TextPointX, hitChain.TextPointY))
                {
                    hitChain.AddHitObject(this);
                    return; //return after first hit
                }
            }

            base.ChildrenHitTestCore(hitChain);
        }
        static bool HitTestOnSubPart(VgBridgeRenderElement _svgRenderVx, float x, float y)
        {
            //int partCount = _svgRenderVx.VgCmdCount; 
            //for (int i = partCount - 1; i >= 0; --i)
            //{
            //    //we do hittest top to bottom => (so => iter backward)

            //    VgCmd vx = _svgRenderVx.GetVgCmd(i);
            //    if (vx.Name != VgCommandName.Path)
            //    {
            //        continue;
            //    }
            //    //
            //    VgCmdPath path = (VgCmdPath)vx;
            //    //fine tune
            //    //hit test ***
            //    if (PixelFarm.CpuBlit.VertexProcessing.VertexHitTester.IsPointInVxs(path.Vxs, x, y))
            //    {
            //        return true;
            //    }
            //}
            //return false;


            return false;

        }
        public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {
            if (_vgRenderVx == null) return;

            if (_vgRenderVx.HasBitmapSnapshot)
            {
                Image backimg = _vgRenderVx.BackingImage;
                canvas.DrawImage(backimg, new RectangleF(0, 0, backimg.Width, backimg.Height));
            }
            else
            {

                PixelFarm.CpuBlit.RectD bound = _vgRenderVx.GetBounds();
                //create 
                PixelFarm.CpuBlit.ActualBitmap backimg = new PixelFarm.CpuBlit.ActualBitmap((int)bound.Width + 200, (int)bound.Height + 200);
                PixelFarm.CpuBlit.AggPainter painter = PixelFarm.CpuBlit.AggPainter.Create(backimg);

                double prevStrokeW = painter.StrokeWidth;
                painter.StrokeWidth = 1;//default 
                VgPainterArgsPool.GetFreePainterArgs(painter, out VgPaintArgs paintArgs);
                _vgRenderVx._renderE.Paint(paintArgs);
                VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);
                painter.StrokeWidth = prevStrokeW;//restore

                _vgRenderVx.SetBitmapSnapshot(backimg);
                canvas.DrawImage(backimg, new RectangleF(0, 0, backimg.Width, backimg.Height));

            }
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {

        }
    }

    public class UISprite : UIElement
    {

        VgBridgeRenderElement _vgRenderElemBridge;
        PaintLab.Svg.VgRenderVx _renderVx;
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public UISprite(float width, float height)
        {
            SetElementBoundsWH(width, height);
            this.AutoStopMouseEventPropagation = true;

        }
        public void LoadSvg(PaintLab.Svg.VgRenderVx renderVx)
        {
            _renderVx = renderVx;
            if (_vgRenderElemBridge != null)
            {
                _vgRenderElemBridge.VgRenderVx = renderVx;

                RectD bounds = _renderVx.GetBounds();
                this.SetSize((int)bounds.Width, (int)bounds.Height);

            }

        }
        protected override void OnElementChanged()
        {

            if (_vgRenderElemBridge != null)
            {
                _renderVx.SetBitmapSnapshot(null);//clear
                _renderVx.InvalidateBounds(); 
                //_svgRenderVx.SetBitmapSnapshot(null); 
                //_svgRenderElement.RenderVx = _svgRenderVx;
                //_svgRenderVx.InvalidateBounds(); 
                RectD bounds = _renderVx.GetBounds();
                this.SetSize((int)bounds.Width, (int)bounds.Height);

            }
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            base.OnMouseDown(e);
        }
        public override void Walk(UIVisitor visitor)
        {

        }
        protected override bool HasReadyRenderElement
        {
            get { return _vgRenderElemBridge != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return _vgRenderElemBridge; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_vgRenderElemBridge == null)
            {
                _vgRenderElemBridge = new VgBridgeRenderElement(rootgfx, 10, 10);
                _vgRenderElemBridge.SetLocation((int)this.Left, (int)this.Top);
                _vgRenderElemBridge.SetController(this);
                _vgRenderElemBridge.VgRenderVx = _renderVx;

                //
                RectD bounds = _renderVx.GetBounds();
                this.SetSize((int)bounds.Width, (int)bounds.Height);

            }
            return _vgRenderElemBridge;
        }
        public virtual void SetLocation(float left, float top)
        {
            SetElementBoundsLT(left, top);
            if (this.HasReadyRenderElement)
            {
                //TODO: review rounding here
                this.CurrentPrimaryRenderElement.SetLocation((int)left, (int)top);
            }
        }

        public virtual void SetSize(float width, float height)
        {
            SetElementBoundsWH(width, height);
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize((int)width, (int)height);
            }
        }

        public void SetBounds(float left, float top, float width, float height)
        {
            SetLocation(left, top);
            SetSize(width, height);
        }
        public float Left
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.X;
                }
                else
                {
                    return BoundLeft;
                }
            }
        }
        public float Top
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Y;
                }
                else
                {
                    return BoundTop;
                }
            }
        }
        public float Right
        {
            get { return this.Left + Width; }
        }
        public float Bottom
        {
            get { return this.Top + Height; }
        }

        public float Width
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Width;
                }
                else
                {
                    return BoundWidth;
                }
            }
        }
        public float Height
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Height;
                }
                else
                {
                    return BoundHeight;
                }
            }
        }

        public override void InvalidateGraphics()
        {
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
        }
        public void InvalidateOuterGraphics()
        {
            if (this.CurrentPrimaryRenderElement != null)
            {
                this.CurrentPrimaryRenderElement.InvalidateGraphicBounds();
            }
        }
        public virtual void PerformContentLayout()
        {
        }
        protected virtual void Describe(UIVisitor visitor)
        {
            visitor.Attribute("left", this.Left);
            visitor.Attribute("top", this.Top);
            visitor.Attribute("width", this.Width);
            visitor.Attribute("height", this.Height);
        }


    }
}