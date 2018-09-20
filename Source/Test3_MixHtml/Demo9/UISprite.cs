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
        public bool EnableSubSvgHitTest { get; set; }

        /// <summary>
        /// find svg element at specific pos
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public SvgHitInfo FindRenderElementAtPos(float x, float y, bool makeCopyOfHitVxs)
        {

            VgHitChainPool.GetFreeHitTestArgs(out SvgHitChain svgHitChain); //get chain from pool
            svgHitChain.WithSubPartTest = true;
            svgHitChain.MakeCopyOfHitVxs = makeCopyOfHitVxs;
            svgHitChain.SetHitTestPos(x - RenderOriginXOffset, y - RenderOriginYOffset);

            HitTestOnSubPart(this, svgHitChain);
            int hitCount = svgHitChain.Count;

            SvgHitInfo hitInfo = hitCount > 0 ?
                svgHitChain.GetHitInfo(hitCount - 1) ://get latest hit info, or
                new SvgHitInfo(); //empty hit info

            VgHitChainPool.ReleaseHitTestArgs(ref svgHitChain); //release chain
            return hitInfo;

        }
        public void FindRenderElementAtPos(float x, float y, Action<SvgRenderElement, float, float, VertexStore> onHitSvg)
        {
            this._vgRenderVx._renderE.HitTest(x, y, onHitSvg);
        }

        public float RenderOriginXOffset { get; set; }
        public float RenderOriginYOffset { get; set; }


        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            RectD bound = _vgRenderVx.GetBounds();
            bound.Offset(RenderOriginXOffset, RenderOriginYOffset);
            if (bound.Contains(hitChain.TestPoint.x, hitChain.TestPoint.y))
            {
                //we hit in svg bounds area  

                if (!EnableSubSvgHitTest)
                {
                    //not test further
                    hitChain.AddHitObject(this);
                }
                else
                {
                    VgHitChainPool.GetFreeHitTestArgs(out SvgHitChain svgHitChain);
                    //check if we hit on some part of the svg 
#if DEBUG
                    if (hitChain.dbugHitPhase == dbugHitChainPhase.MouseDown)
                    {

                    }
#endif
                    svgHitChain.WithSubPartTest = this.EnableSubSvgHitTest;
                    svgHitChain.SetHitTestPos(hitChain.TextPointX, hitChain.TextPointY);
                    if (HitTestOnSubPart(this, svgHitChain))
                    {
                        hitChain.AddHitObject(this);
                    }
                    VgHitChainPool.ReleaseHitTestArgs(ref svgHitChain);
                }
            }
        }


        static class VgHitChainPool
        {
            //
            //
            [System.ThreadStatic]
            static Stack<SvgHitChain> s_hitChains = new Stack<SvgHitChain>();
            public static void GetFreeHitTestArgs(out SvgHitChain hitTestArgs)
            {
                if (s_hitChains.Count > 0)
                {
                    hitTestArgs = s_hitChains.Pop();
                }
                else
                {
                    hitTestArgs = new SvgHitChain();
                }
            }
            public static void ReleaseHitTestArgs(ref SvgHitChain hitTestArgs)
            {
                hitTestArgs.Clear();
                s_hitChains.Push(hitTestArgs);
                hitTestArgs = null;
            }
        }
        static bool HitTestOnSubPart(VgBridgeRenderElement _svgRenderVx, SvgHitChain hitChain)
        {
            SvgRenderElement renderE = _svgRenderVx._vgRenderVx._renderE;
            renderE.HitTest(hitChain);
            return hitChain.Count > 0;//found some    
        }
        void CustomDrawToThisPageWithBmpCache(DrawBoard canvas, Rectangle updateArea)
        {

            if (_vgRenderVx.HasBitmapSnapshot)
            {
                Image backimg = _vgRenderVx.BackingImage;
                canvas.DrawImage(backimg, new RectangleF(0, 0, backimg.Width, backimg.Height));
            }
            else
            {


                PixelFarm.CpuBlit.RectD bounds = _vgRenderVx.GetBounds();
                int width = (int)Math.Ceiling(bounds.Width);
                int height = (int)Math.Ceiling(bounds.Height);
                //create 
                if (bounds.Left > 0)
                {
                    width = (int)Math.Ceiling(bounds.Right);
                }
                if (bounds.Bottom > 0)
                {
                    height = (int)Math.Ceiling(bounds.Top);
                }
                PixelFarm.CpuBlit.ActualBitmap backimg = new PixelFarm.CpuBlit.ActualBitmap(width, height);
                PixelFarm.CpuBlit.AggPainter painter = PixelFarm.CpuBlit.AggPainter.Create(backimg);

                double prevStrokeW = painter.StrokeWidth;
                Color prevFill = painter.FillColor;
                Color prevStrokeColor = painter.StrokeColor;

                painter.StrokeWidth = 1;//default 
                //painter.FillColor = Color.Black;
                //painter.StrokeColor = Color.Black;
                VgPainterArgsPool.GetFreePainterArgs(painter, out VgPaintArgs paintArgs);
                _vgRenderVx._renderE.Paint(paintArgs);
                VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);

                painter.StrokeWidth = prevStrokeW;//restore
                                                  //painter.FillColor = prevFill;
                                                  //painter.StrokeColor = prevStrokeColor;

                _vgRenderVx.SetBitmapSnapshot(backimg);
                canvas.DrawImage(backimg, new RectangleF(0, 0, backimg.Width, backimg.Height));
            }
        }

        public bool DisableBitmapCache { get; set; } //default = false => EnableBitmapCache
        public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {
            if (_vgRenderVx == null) return;
            //-----------------------
            PixelFarm.CpuBlit.AggPainter painter = null;
            if (DisableBitmapCache &&
               ((painter = canvas.GetPainter() as PixelFarm.CpuBlit.AggPainter) != null))
            {
                //temp fix***

                float prev_x = painter.OriginX;
                float prev_y = painter.OriginY;

                painter.SetOrigin(prev_x + X + RenderOriginXOffset, prev_y + Y + RenderOriginYOffset);
                double prevStrokeW = painter.StrokeWidth;
                VgPainterArgsPool.GetFreePainterArgs(painter, out VgPaintArgs paintArgs);
                _vgRenderVx._renderE.Paint(paintArgs);
                VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);
                painter.StrokeWidth = prevStrokeW;
                painter.SetOrigin(prev_x, prev_y);
            }
            else
            {
                //enable bitmap cache
                CustomDrawToThisPageWithBmpCache(canvas, updateArea);
            }
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {

        }
    }

    public class UISprite : UIElement
    {
        bool _enableSubSvgTest;
        VgBridgeRenderElement _vgRenderElemBridge;
        VgRenderVx _renderVx;
        bool _disableBmpCache;
        float _actualXOffset;
        float _actualYOffset;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public UISprite(float width, float height)
        {
            SetElementBoundsWH(width, height);
            this.AutoStopMouseEventPropagation = true;
        }
        public bool EnableSubSvgTest
        {
            get
            {
                return _enableSubSvgTest;
            }
            set
            {
                _enableSubSvgTest = value;
                if (_vgRenderElemBridge != null)
                {
                    _vgRenderElemBridge.EnableSubSvgHitTest = value;
                }
            }
        }
        public bool DisableBmpCache
        {
            get => _disableBmpCache;
            set
            {
                _disableBmpCache = value;
                if (_vgRenderElemBridge != null)
                {
                    _vgRenderElemBridge.DisableBitmapCache = value;
                }
            }
        }
        public void LoadVg(PaintLab.Svg.VgRenderVx renderVx)
        {
            _renderVx = renderVx;
            if (_vgRenderElemBridge != null)
            {
                _vgRenderElemBridge.VgRenderVx = renderVx;
                RectD bounds = _renderVx.GetBounds();
                this.SetSize((int)bounds.Width, (int)bounds.Height);
            }
        }
        public float ActualXOffset => _actualXOffset;
        public float ActualYOffset => _actualYOffset;

        public SvgHitInfo FindRenderElementAtPos(float x, float y, bool makeCopyOfVxs)
        {
            return _vgRenderElemBridge.FindRenderElementAtPos(x, y, makeCopyOfVxs);
        }
        public void FindRenderElementAtPos(float x, float y, Action<SvgRenderElement, float, float, VertexStore> onHitSvg)
        {
            _vgRenderElemBridge.FindRenderElementAtPos(x, y, onHitSvg);
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
                RectD bounds = _renderVx.GetBounds();


                //****
                _actualXOffset = (float)-bounds.Left;
                _actualYOffset = (float)-bounds.Bottom;

                //offset to 0,0
                this.SetLocation((int)(this.Left + _actualXOffset), (int)(this.Top + _actualYOffset));
                _vgRenderElemBridge = new VgBridgeRenderElement(rootgfx, 10, 10)
                {
                    RenderOriginXOffset = _actualXOffset,
                    RenderOriginYOffset = _actualYOffset,
                    VgRenderVx = _renderVx,
                    DisableBitmapCache = this.DisableBmpCache,
                    EnableSubSvgHitTest = this.EnableSubSvgTest
                };

                _vgRenderElemBridge.SetLocation((int)(this.Left), (int)(this.Top));
                _vgRenderElemBridge.SetController(this);
                //
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
                this.CurrentPrimaryRenderElement.InvalidateParentGraphics();
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