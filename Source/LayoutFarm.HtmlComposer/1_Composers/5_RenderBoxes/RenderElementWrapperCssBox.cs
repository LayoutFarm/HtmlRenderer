//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.Css;
namespace LayoutFarm.HtmlBoxes.InternalWrappers
{
    sealed class CssExternalRun : CssRun
    {
        RenderElement _externalRenderE;
        Rectangle _renderElementRect;
        LayoutFarm.Composers.ISubDomExtender _subDomExtender;

        public CssExternalRun(RenderElement externalRenderE, LayoutFarm.Composers.ISubDomExtender subDomExtender)
            : base(CssRunKind.SolidContent) //act as image run****
        {
            //in this version we make it as as image run
            _subDomExtender = subDomExtender;
            _externalRenderE = externalRenderE;
        }
        public RenderElement RenderElement => _externalRenderE;

        /// <summary>
        /// the image rectange restriction as returned from image load event
        /// </summary>
        public Rectangle ExternalRunRect
        {
            get => _renderElementRect;
            set => _renderElementRect = value;
        }

        public override void WriteContent(StringBuilder stbuilder, int start, int length)
        {
            _subDomExtender?.Write(stbuilder);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("write_content: on CssBlockRun");
#endif

        }
        public override void WriteContent(StringBuilder stbuilder, int start = 0)
        {
            _subDomExtender?.Write(stbuilder);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("write_content: on CssBlockRun");
#endif
        }
#if DEBUG
        /// <summary>
        /// Represents this word for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "extRun:" + _externalRenderE.ToString();
        }
#endif
    }

    abstract class WrapperCssBoxBase : CssBox, LayoutFarm.RenderBoxes.IParentLink
    {
        /// <summary>
        /// offset x from nearest ancester render element 
        /// </summary>
        protected int _adjustX;
        /// <summary>
        /// offset y from nearest ancester render element 
        /// </summary>
        protected int _adjustY;
        public WrapperCssBoxBase(object controller,
             BoxSpec spec, CssDisplay display)
            : base(spec, display)
        {
            this.SetController(controller);
        }

        internal abstract RenderElement GetParentRenderElement(out int globalX, out int globalY);
        //
        RenderElement RenderBoxes.IParentLink.ParentRenderElement => this.GetParentRenderElement(out int globalX, out int globalY);
        //
        protected abstract void AdjustLocalLocation(ref int p_x, ref int p_y);
        void RenderBoxes.IParentLink.AdjustLocation(ref int p_x, ref int p_y)
        {
            AdjustLocalLocation(ref p_x, ref p_y);

            this.GetGlobalLocationRelativeToRoot(out float gx, out float gy);
            _adjustX = (int)gx;
            _adjustY = (int)gy;

            p_x += _adjustX;
            p_y += _adjustY;
        }
        

#if DEBUG
        string LayoutFarm.RenderBoxes.IParentLink.dbugGetLinkInfo()
        {
            return "";
        }
#endif
    }

    /// <summary>
    /// css box that wrap render element inside (as a single external run)
    /// </summary>
    sealed class WrapperInlineCssBox : WrapperCssBoxBase
    {
        CssExternalRun _externalRun;
        HtmlHost _htmlhost;
        public WrapperInlineCssBox(
            HtmlHost htmlhost,
            object controller, Css.BoxSpec boxSpec,
            RenderElement re,
            LayoutFarm.Composers.ISubDomExtender subDomExtender)
            : base(controller, boxSpec, CssDisplay.Inline)
        {
            _htmlhost = htmlhost;

            ChangeDisplayType(this, CssDisplay.Inline);
            _externalRun = new CssExternalRun(re, subDomExtender);
            _externalRun.SetOwner(this);

            CssBox.UnsafeSetContentRuns(this, new List<CssRun>(1) { _externalRun }, false);
            ChangeDisplayType(this, Css.CssDisplay.Inline);

            LayoutFarm.RenderElement.SetParentLink(re, this);
        }
        protected override void AdjustLocalLocation(ref int p_x, ref int p_y)
        {
            p_x += (int)_externalRun.Left;
            p_y += (int)_externalRun.Top;
        }
        public override void Clear()
        {
            base.Clear();
            var runlist = new List<CssRun>(1);
            runlist.Add(_externalRun);
            CssBox.UnsafeSetContentRuns(this, runlist, false);
        }
        public override void Paint(PaintVisitor p, RectangleF r)
        {
            var updateArea = new Rectangle((int)r.Left, (int)r.Top, (int)r.Width, (int)r.Height);
            int x = (int)updateArea.Left;
            int y = (int)updateArea.Top;
            DrawBoard d = p.InnerDrawBoard;

            int enter_canvas_x = d.OriginX;
            int enter_canvas_y = d.OriginY;
            d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
            updateArea.Offset(-x, -y);

            UpdateArea u = GetFreeUpdateArea();
            u.CurrentRect = updateArea;
            RenderElement.Render(_externalRun.RenderElement, d, u);
            ReleaseUpdateArea(u);

            d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);//restore
        }

        //-------

        static Stack<UpdateArea> _updateAreaPool = new Stack<UpdateArea>();

        static UpdateArea GetFreeUpdateArea() => (_updateAreaPool.Count == 0) ? new UpdateArea() : _updateAreaPool.Pop();

        static void ReleaseUpdateArea(UpdateArea u)
        {
            u.Reset();
            _updateAreaPool.Push(u);
        }

        //-------


        protected override void PaintImp(PaintVisitor p)
        {
#if DEBUG
            p.dbugEnterNewContext(this, PaintVisitor.PaintVisitorContextName.Init);
#endif
            Paint(p, new RectangleF(0, 0, this.VisualWidth, this.VisualHeight));
#if DEBUG
            p.dbugExitContext();
#endif
        }
        public override void MeasureRunsSize(LayoutVisitor lay)
        {
            if (this.RunSizeMeasurePass)
            {
                return;
            }
            this.RunSizeMeasurePass = true;
            _externalRun.Width = _externalRun.RenderElement.Width;
            _externalRun.Height = _externalRun.RenderElement.Height;
        }
        //---------------------------------------------------------------------------------------
        internal override RenderElement GetParentRenderElement(out int globalX, out int globalY)
        {
            CssBox cbox = this;
            //start  
            globalX = (int)_externalRun.Left;
            globalY = (int)_externalRun.Top;
            while (cbox != null)
            {
                globalX += (int)cbox.LocalX;
                globalY += (int)cbox.LocalY;
                var renderRoot = cbox as LayoutFarm.HtmlBoxes.RenderElementBridgeCssBox;
                if (renderRoot != null)
                {
                    _adjustX = globalX;
                    _adjustY = globalY;
                    return renderRoot.ContainerElement;
                }
                else
                {
                    if (cbox.ParentBox == null)
                    {
                        return (RenderElement)_htmlhost.RootGfx.TopWindowRenderBox;
                    }
                }
                cbox = cbox.ParentBox;
            }
            return null;
        }
    }

    /// <summary>
    /// css box that wrap a render element inside 
    /// </summary>
    sealed class WrapperBlockCssBox : WrapperCssBoxBase
    {
        HtmlHost _htmlHost;
        RenderElement _renderE;

        public WrapperBlockCssBox(
             HtmlHost htmlHost,
             object controller,
             BoxSpec spec,
             RenderElement renderElement,
             LayoutFarm.Composers.ISubDomExtender subDomExtender)
            : base(controller, spec, CssDisplay.Block)
        {
            _htmlHost = htmlHost;


            SetAsCustomCssBox(this);
            int w = renderElement.Width;
            int h = renderElement.Height;
            _renderE = renderElement;
            ChangeDisplayType(this, CssDisplay.Block);
            this.SetVisualSize(w, h);
            LayoutFarm.RenderElement.SetParentLink(renderElement, this);
        }
        protected override void AdjustLocalLocation(ref int p_x, ref int p_y)
        {

        }
        public override bool CustomContentHitTest(float x, float y, CssBoxHitChain hitChain)
        {
            return false;
        }
        public override void CustomRecomputedValue(CssBox containingBlock)
        {
            var ibox = CssBox.UnsafeGetController(this) as LayoutFarm.UI.IBoxElement;
            if (ibox != null)
            {
                //todo: user minimum font height of the IBoxElement
                int w = (int)this.VisualWidth;
                int h = Math.Max((int)this.VisualHeight, ibox.MinHeight);
                ibox.ChangeElementSize(w, h);
                this.SetVisualSize(w, h);
            }
            else
            {
                //TODO: review this
                this.SetVisualSize(100, 20);
            }
        }
        protected override void PaintImp(PaintVisitor p)
        {
#if DEBUG
            p.dbugEnterNewContext(this, PaintVisitor.PaintVisitorContextName.Init);
#endif

            UpdateArea u = GetFreeUpdateArea();
            u.CurrentRect = new Rectangle(0, 0, _renderE.Width, _renderE.Height);
            RenderElement.Render(_renderE, p.InnerDrawBoard, u);
            ReleaseUpdateArea(u);

#if DEBUG
            p.dbugExitContext();
#endif
        }
        //-------

        static Stack<UpdateArea> _updateAreaPool = new Stack<UpdateArea>();

        static UpdateArea GetFreeUpdateArea() => (_updateAreaPool.Count == 0) ? new UpdateArea() : _updateAreaPool.Pop();

        static void ReleaseUpdateArea(UpdateArea u)
        {
            u.Reset();
            _updateAreaPool.Push(u);
        }

        //-------
        internal override RenderElement GetParentRenderElement(out int globalX, out int globalY)
        {
            CssBox cbox = this;

            //start 
            globalX = 0;
            globalY = 0;
            while (cbox != null)
            {
                globalX += (int)cbox.LocalX;
                globalY += (int)cbox.LocalY;
                var renderRoot = cbox as LayoutFarm.HtmlBoxes.RenderElementBridgeCssBox;
                if (renderRoot != null)
                {
                    _adjustX = globalX;
                    _adjustY = globalY;
                    return renderRoot.ContainerElement;
                }
                else
                {
                    if (cbox.ParentBox == null)
                    {
                        return (RenderElement)_htmlHost.RootGfx.TopWindowRenderBox;
                    }
                }
                cbox = cbox.ParentBox;
            }
            return null;
        }
    }
}