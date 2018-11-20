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
            //this.TransparentForAllEvents = true;
        }

        public PixelFarm.CpuBlit.VertexProcessing.ICoordTransformer Tx { get; set; }
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
        public VgHitInfo FindRenderElementAtPos(float x, float y, bool makeCopyOfHitVxs)
        {

            VgHitChainPool.GetFreeHitTestArgs(out VgHitChain svgHitChain); //get chain from pool
            svgHitChain.WithSubPartTest = true;
            svgHitChain.MakeCopyOfHitVxs = makeCopyOfHitVxs;
            svgHitChain.SetHitTestPos(x - RenderOriginXOffset, y - RenderOriginYOffset);

            HitTestOnSubPart(this, svgHitChain);
            int hitCount = svgHitChain.Count;

            VgHitInfo hitInfo = hitCount > 0 ?
                svgHitChain.GetHitInfo(hitCount - 1) ://get latest hit info, or
                new VgHitInfo(); //empty hit info

            VgHitChainPool.ReleaseHitTestArgs(ref svgHitChain); //release chain
            return hitInfo;

        }
        public void FindRenderElementAtPos(float x, float y, Action<VgVisualElement, float, float, VertexStore> onHitSvg)
        {
            this._vgRenderVx._renderE.HitTest(x, y, onHitSvg);
        }

        public float RenderOriginXOffset
        {
            get => 0;
            set
            {
                //_renderOffsetX = value;
            }
        }
        public float RenderOriginYOffset
        {
            get => 0;
            set
            {
                //_renderOffsetY = value;
            }
        }

        //float _renderOffsetX;
        //float _renderOffsetY;
        //public void SetRenderOffset(float renderOffsetX, float renderOffsetY)
        //{
        //    _renderOffsetX = renderOffsetX;
        //    _renderOffsetY = renderOffsetY;
        //}

        public override void ChildrenHitTestCore(HitChain hitChain)
        {

            RectD bound = _vgRenderVx.GetRectBounds();
            bound.Offset(RenderOriginXOffset, RenderOriginYOffset);
            if (bound.Contains(hitChain.TestPoint.X, hitChain.TestPoint.Y))
            {
                //we hit in svg bounds area  

                if (!EnableSubSvgHitTest)
                {
                    //not test further
                    if (hitChain.TopMostElement != this)
                    {
                        hitChain.AddHitObject(this);
                    }

                }
                else
                {
                    VgHitChainPool.GetFreeHitTestArgs(out VgHitChain svgHitChain);
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
            static Stack<VgHitChain> s_hitChains = new Stack<VgHitChain>();
            public static void GetFreeHitTestArgs(out VgHitChain hitTestArgs)
            {
                if (s_hitChains.Count > 0)
                {
                    hitTestArgs = s_hitChains.Pop();
                }
                else
                {
                    hitTestArgs = new VgHitChain();
                }
            }
            public static void ReleaseHitTestArgs(ref VgHitChain hitTestArgs)
            {
                hitTestArgs.Clear();
                s_hitChains.Push(hitTestArgs);
                hitTestArgs = null;
            }
        }
        static bool HitTestOnSubPart(VgBridgeRenderElement _svgRenderVx, VgHitChain hitChain)
        {
            VgVisualElement renderE = _svgRenderVx._vgRenderVx._renderE;
            renderE.HitTest(hitChain);
            return hitChain.Count > 0;//found some    
        }


        public bool DisableBitmapCache { get; set; }

        public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {

            if (_vgRenderVx == null) return;
            //-----------------------

#if DEBUG
            if (_vgRenderVx.dbugId == 0)
            {
                if (X != 0)
                {

                }
            }
#endif
            AggPainter painter = null;
            if (DisableBitmapCache &&
               ((painter = canvas.GetPainter() as PixelFarm.CpuBlit.AggPainter) != null))
            {
                PaintVgWithPainter(painter, _vgRenderVx);
            }
            else
            {
                //enable bitmap cache
                if (_vgRenderVx.HasBitmapSnapshot)
                {
                    Image backimg = _vgRenderVx.BackingImage;
                    canvas.DrawImage(backimg, new RectangleF(0, 0, backimg.Width, backimg.Height));
                }
                else
                {
                    //convert vg to bitmap 
                    //**

                    PixelFarm.CpuBlit.RectD bounds = _vgRenderVx.GetRectBounds();
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

                    //--------------------
                    //TODO: use reusable agg painter***
                    MemBitmap backimg = new MemBitmap(width, height);
                    painter = AggPainter.Create(backimg);
                    painter.CurrentFont = canvas.CurrentFont;
                    painter.Clear(Color.FromArgb(0, Color.White));
                    //
                    PaintVgWithPainter(painter, _vgRenderVx);
                    //
                    _vgRenderVx.SetBitmapSnapshot(backimg);
                    canvas.DrawImage(backimg, new RectangleF(0, 0, backimg.Width, backimg.Height));
                }
            }
        }


        static void PaintVgWithPainter(Painter painter, VgRenderVx vgRenderVx)
        {

            double prevStrokeW = painter.StrokeWidth;
            painter.StrokeWidth = 1;//default  

            using (VgPainterArgsPool.Borrow(painter, out VgPaintArgs paintArgs))
            {
                if (vgRenderVx._coordTx != null)
                {

                }
                vgRenderVx._renderE.Paint(paintArgs);
            }
            painter.StrokeWidth = prevStrokeW;//restore 

        }


        public override void ResetRootGraphics(RootGraphic rootgfx)
        {

        }
        //post transform corners
        double _b_x0, _b_y0, //top-left
               _b_x1, _b_y1, //top-right
               _b_x2, _b_y2, //bottom-right
               _b_x3, _b_y3; //bottom -left

        //post transform bounds
        RectD _post_TransformRectBounds;
        public void SetPostTransformationBounds(RectD postTransformationBounds)
        {
            _post_TransformRectBounds = postTransformationBounds;

            int left = (int)Math.Floor(_post_TransformRectBounds.Left);
            int right = (int)Math.Ceiling(_post_TransformRectBounds.Right);

            int top = (int)Math.Floor(_post_TransformRectBounds.Top);
            int bottom = (int)Math.Ceiling(_post_TransformRectBounds.Bottom);

            SetBounds(left, top, right - left, -(bottom - top));
        }
        public void SetPostTransformCorners(
            double b_x0, double b_y0, //top-left
            double b_x1, double b_y1, //top-right
            double b_x2, double b_y2, //bottom-right
            double b_x3, double b_y3)
        {
            _b_x0 = b_x0; _b_y0 = b_y0;
            _b_x1 = b_x1; _b_y1 = b_y1;
            _b_x2 = b_x2; _b_y2 = b_y2;
            _b_x3 = b_x3; _b_y3 = b_y3;
        }

    }

    public class UISprite : UIElement
    {
        bool _enableSubSvgTest;

        VgRenderVx _vgRenderVx;
        VgBridgeRenderElement _vgBridgeRenderElement;

        bool _disableBmpCache;
        double _actualXOffset;
        double _actualYOffset;

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
                if (_vgBridgeRenderElement != null)
                {
                    _vgBridgeRenderElement.EnableSubSvgHitTest = value;
                }
            }
        }
        public bool DisableBmpCache
        {
            get => _disableBmpCache;
            set
            {
                _disableBmpCache = value;
                if (_vgBridgeRenderElement != null)
                {
                    _vgBridgeRenderElement.DisableBitmapCache = value;
                }
            }
        }
        public void LoadVg(PaintLab.Svg.VgRenderVx renderVx)
        {
            _vgRenderVx = renderVx;
            if (_vgBridgeRenderElement != null)
            {
                _vgBridgeRenderElement.VgRenderVx = renderVx;
                RectD bounds = _vgRenderVx.GetRectBounds();
                _actualXOffset = (float)-bounds.Left;
                _actualYOffset = (float)-bounds.Bottom;

                this.SetSize((int)bounds.Width, (int)bounds.Height);
            }
        }


        internal PaintLab.Svg.VgRenderVx RenderVx => _vgRenderVx;
        internal VgBridgeRenderElement VgBridgeRenderElement => _vgBridgeRenderElement;


        //--------------------
        public void BringToTopMost()
        {
            CustomWidgets.AbstractBox parentBox = this.ParentUI as CustomWidgets.AbstractBox;
            if (parentBox != null)
            {
                this.RemoveSelf();
                parentBox.AddChild(this);
            }
            else
            {
                if (_vgBridgeRenderElement != null && _vgBridgeRenderElement.HasParent)
                {

                }
            }
        }


        //--------------------

        //
        public float ActualXOffset
        {
            get
            {
                if (_vgBridgeRenderElement != null)
                {
                    return _vgBridgeRenderElement.RenderOriginXOffset;
                }
                else
                {
                    return (float)_actualXOffset;
                }
            }

        }
        public float ActualYOffset
        {
            get
            {
                if (_vgBridgeRenderElement != null)
                {
                    return _vgBridgeRenderElement.RenderOriginYOffset;
                }
                else
                {
                    return (float)_actualYOffset;
                }
            }

        }

        public VgHitInfo FindRenderElementAtPos(float x, float y, bool makeCopyOfVxs)
        {
            return _vgBridgeRenderElement.FindRenderElementAtPos(x, y, makeCopyOfVxs);
        }
        public void FindRenderElementAtPos(float x, float y, Action<VgVisualElement, float, float, VertexStore> onHitSvg)
        {
            _vgBridgeRenderElement.FindRenderElementAtPos(x, y, onHitSvg);
        }
        protected override void OnElementChanged()
        {

            if (_vgBridgeRenderElement != null)
            {
                _vgRenderVx.SetBitmapSnapshot(null);//clear
                _vgRenderVx.InvalidateBounds();
                //_svgRenderVx.SetBitmapSnapshot(null); 
                //_svgRenderElement.RenderVx = _svgRenderVx;
                //_svgRenderVx.InvalidateBounds(); 
                RectD bounds = _vgRenderVx.GetRectBounds();
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
            get { return _vgBridgeRenderElement != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return _vgBridgeRenderElement; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_vgBridgeRenderElement == null)
            {
                RectD bounds = _vgRenderVx.GetRectBounds();
                //****

                //offset to 0,0
                //this.SetLocation((int)(this.Left + _actualXOffset), (int)(this.Top + _actualYOffset));


                var vgBridgeRenderElem = new VgBridgeRenderElement(rootgfx, 10, 10)
                {
                    RenderOriginXOffset = (float)_actualXOffset,
                    RenderOriginYOffset = (float)_actualYOffset,
                    VgRenderVx = _vgRenderVx,
                    DisableBitmapCache = this.DisableBmpCache,
                    EnableSubSvgHitTest = this.EnableSubSvgTest
                };

                //vgBridgeRenderElem.DisableBitmapCache = true;
                vgBridgeRenderElem.SetLocation((int)(this.Left), (int)(this.Top));
                vgBridgeRenderElem.SetController(this);

                _vgBridgeRenderElement = vgBridgeRenderElem;//set to field after init-setting.

                //_vgBridgeRenderElement.TransparentForAllEvents = this.TransparentAllMouseEvents;
                this.SetSize((int)bounds.Width, (int)bounds.Height);
            }
            return _vgBridgeRenderElement;
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


        PixelFarm.CpuBlit.VertexProcessing.ICoordTransformer _tx;


        //post transform corners
        double _b_x0, _b_y0, //top-left
               _b_x1, _b_y1, //top-right
               _b_x2, _b_y2, //bottom-right
               _b_x3, _b_y3; //bottom -left

        //post transform bounds
        RectD _post_TransformRectBounds;

        public void SetTransformation(PixelFarm.CpuBlit.VertexProcessing.ICoordTransformer tx)
        {
            _tx = tx;
            RectD bounds = _vgRenderVx.GetRectBounds(); //org bounds
            _actualXOffset = -bounds.Left;
            _actualYOffset = -bounds.Bottom;
            _post_TransformRectBounds = bounds;

            _b_x0 = bounds.Left; _b_y0 = bounds.Top;
            _b_x1 = bounds.Right; _b_y1 = bounds.Top;
            _b_x2 = bounds.Right; _b_y2 = bounds.Bottom;
            _b_x3 = bounds.Left; _b_y3 = bounds.Bottom;

            if (tx != null)
            {
                tx.Transform(ref _actualXOffset, ref _actualYOffset); //we need to translate actual offset too! 
                tx.Transform(ref _b_x0, ref _b_y0);
                tx.Transform(ref _b_x1, ref _b_y1);
                tx.Transform(ref _b_x2, ref _b_y2);
                tx.Transform(ref _b_x3, ref _b_y3);

                _post_TransformRectBounds = RectD.ZeroIntersection;
                PixelFarm.CpuBlit.VertexProcessing.BoundingRect.GetBoundingRect(_b_x0, _b_y0, ref _post_TransformRectBounds);
                PixelFarm.CpuBlit.VertexProcessing.BoundingRect.GetBoundingRect(_b_x1, _b_y1, ref _post_TransformRectBounds);
                PixelFarm.CpuBlit.VertexProcessing.BoundingRect.GetBoundingRect(_b_x2, _b_y2, ref _post_TransformRectBounds);
                PixelFarm.CpuBlit.VertexProcessing.BoundingRect.GetBoundingRect(_b_x3, _b_y3, ref _post_TransformRectBounds);

            }

            if (_vgRenderVx != null)
            {
                _vgRenderVx._coordTx = tx;
            }

            if (_vgBridgeRenderElement != null)
            {
                //set this data to vgRenderElemBridge too

                //_post_TransformRectBounds.Offset(-_post_TransformRectBounds.Left, -_post_TransformRectBounds.Bottom);
                _vgBridgeRenderElement.RenderOriginXOffset = -(int)_actualXOffset;
                _vgBridgeRenderElement.RenderOriginYOffset = -(int)_actualYOffset;
                _vgBridgeRenderElement.SetPostTransformationBounds(_post_TransformRectBounds);
                _vgBridgeRenderElement.SetPostTransformCorners(
                    _b_x0, _b_y0, //left,top
                    _b_x1, _b_y1, //right,top
                    _b_x2, _b_y2, //right,bottom
                    _b_x3, _b_y3 //left,bottom
                    );
                _vgBridgeRenderElement.SetLocation(
                    (int)(this.Left - _post_TransformRectBounds.Left),
                    (int)(this.Top - _post_TransformRectBounds.Top));
            }
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
                _vgBridgeRenderElement.InvalidateGraphics();
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