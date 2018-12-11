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
        public CssExternalRun(RenderElement externalRenderE)
            : base(CssRunKind.SolidContent) //act as image run****
        {
            //in this version we make it as as image run
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
            throw new NotImplementedException();
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
             BoxSpec spec,
             RootGraphic rootgfx, CssDisplay display)
            : base(spec, new CssBoxRootGfxBridge(rootgfx), display)
        {
            this.SetController(controller);
        }

        internal abstract RenderElement GetParentRenderElement(out int globalX, out int globalY);
        //
        RenderElement RenderBoxes.IParentLink.ParentRenderElement => this.GetParentRenderElement(out int globalX, out int globalY);
        //
        protected abstract void AdjustLocalLocation(ref Point p);
        void RenderBoxes.IParentLink.AdjustLocation(ref Point p)
        {
            AdjustLocalLocation(ref p);
            this.GetGlobalLocationRelativeToRoot(out float gx, out float gy);
            _adjustX = (int)gx;
            _adjustY = (int)gy;

            p.Offset(_adjustX, _adjustY);
        }
        RenderElement RenderBoxes.IParentLink.FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            return null;
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
            RootGraphic rootgfx, RenderElement re)
            : base(controller, boxSpec, re.Root, CssDisplay.Inline)
        {
            _htmlhost = htmlhost;

            //int w = re.Width;
            //int h = re.Height;

            ChangeDisplayType(this, CssDisplay.Inline);
            _externalRun = new CssExternalRun(re);
            _externalRun.SetOwner(this);
            var runlist = new List<CssRun>(1);
            runlist.Add(_externalRun);
            CssBox.UnsafeSetContentRuns(this, runlist, false);
            ChangeDisplayType(this, Css.CssDisplay.Inline);
            //---------------------------------------------------  
            LayoutFarm.RenderElement.SetParentLink(re, this);
        }
        protected override void AdjustLocalLocation(ref Point p)
        {
            p.Offset((int)_externalRun.Left, (int)_externalRun.Top);
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
            DrawBoard canvasPage = p.InnerDrawBoard;
            canvasPage.OffsetCanvasOrigin(x, y);
            updateArea.Offset(-x, -y);
            _externalRun.RenderElement.DrawToThisCanvas(canvasPage, updateArea);
            canvasPage.OffsetCanvasOrigin(-x, -y);
        }

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
             RenderElement renderElement)
            : base(controller, spec, renderElement.Root, CssDisplay.Block)
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
        protected override void AdjustLocalLocation(ref Point p)
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
            Rectangle rect = new Rectangle(0, 0, _renderE.Width, _renderE.Height);
            _renderE.DrawToThisCanvas(p.InnerDrawBoard, rect);
#if DEBUG
            p.dbugExitContext();
#endif
        }

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