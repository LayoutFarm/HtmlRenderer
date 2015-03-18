//2014,2015,2015 Apache2, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.WebDom;
using LayoutFarm;
using LayoutFarm.Css;
using LayoutFarm.ContentManagers;
using LayoutFarm.Composers;
namespace LayoutFarm.HtmlBoxes
{
    using SpecialInternalWrappers;


    sealed class CssExternalRun : CssRun
    {

        RenderElement externalRenderE;
        Rectangle renderElementRect;
        public CssExternalRun(RenderElement externalRenderE)
            : base(CssRunKind.Image) //act as image run****
        {
            //in this version we make it as as image run
            this.externalRenderE = externalRenderE;
        }
        public RenderElement RenderElement
        {
            get { return this.externalRenderE; }
        }
        /// <summary>
        /// the image rectange restriction as returned from image load event
        /// </summary>
        public Rectangle ExternalRunRect
        {
            get { return renderElementRect; }
            set { renderElementRect = value; }
        }

#if DEBUG
        /// <summary>
        /// Represents this word for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "extRun";
        }
#endif
    }
    abstract class WrapperCssBoxBase : CssBox
    {
        protected int globalXForRenderElement;
        protected int globalYForRenderElement;
        protected CssBoxWrapperRenderElement wrapper;
        public WrapperCssBoxBase(object controller,
             BoxSpec spec,
             RootGraphic root, CssDisplay display)
            : base(controller, spec, root, display)
        {
        }
        internal abstract RenderElement GetParentRenderElement(out int globalX, out int globalY);

    }

    sealed class WrapperInlineCssBox : WrapperCssBoxBase
    {


        CssExternalRun externalRun;
        public WrapperInlineCssBox(object controller, Css.BoxSpec boxSpec,
            IRootGraphics rootgfx, RenderElement re)
            : base(controller, boxSpec, re.Root, CssDisplay.Inline)
        {
            int w = re.Width;
            int h = re.Height;
            wrapper = new CssBoxWrapperRenderElement(re.Root, w, h, re);
            ChangeDisplayType(this, CssDisplay.Inline);

            this.externalRun = new CssExternalRun(wrapper);
            this.externalRun.SetOwner(this);

            var runlist = new List<CssRun>(1);
            runlist.Add(externalRun);
            CssBox.UnsafeSetContentRuns(this, runlist, false);
            ChangeDisplayType(this, Css.CssDisplay.Inline);
            //--------------------------------------------------- 

            LayoutFarm.RenderElement.SetParentLink(
            wrapper,
             new RenderBoxWrapperLink(this));

            LayoutFarm.RenderElement.SetParentLink(
                re,
                new RenderBoxWrapperLink2(wrapper));
        }
        public override void Clear()
        {
            base.Clear();

            var runlist = new List<CssRun>(1);
            runlist.Add(externalRun);
            CssBox.UnsafeSetContentRuns(this, runlist, false);

        }
        public override void Paint(PaintVisitor p, RectangleF r)
        {
            var updateArea = new Rectangle((int)r.Left, (int)r.Top, (int)r.Width, (int)r.Height);
            int x = (int)updateArea.Left;
            int y = (int)updateArea.Top;
            var canvasPage = p.InnerCanvas;
            canvasPage.OffsetCanvasOrigin(x, y);
            updateArea.Offset(-x, -y);
            externalRun.RenderElement.DrawToThisCanvas(canvasPage, updateArea);

            canvasPage.OffsetCanvasOrigin(-x, -y);


        }
        public RenderElement RenderElement
        {
            get
            {
                return externalRun.RenderElement;
            }
        }

        protected override void PaintImp(PaintVisitor p)
        {
            Paint(p, new RectangleF(0, 0, this.SizeWidth, this.SizeHeight));
        }


        public override void MeasureRunsSize(LayoutVisitor lay)
        {
            if (this.RunSizeMeasurePass)
            {
                return;
            }
            this.RunSizeMeasurePass = true;
            this.externalRun.Width = this.externalRun.RenderElement.Width;
            this.externalRun.Height = this.externalRun.RenderElement.Height;
        }



        //---------------------------------------------------------------------------------------
        internal override RenderElement GetParentRenderElement(out int globalX, out int globalY)
        {
            CssBox cbox = this;
            //start 
            globalX = (int)this.externalRun.Left;
            globalY = (int)this.externalRun.Top;

            while (cbox != null)
            {
                globalX += (int)cbox.LocalX;
                globalY += (int)cbox.LocalY;
                var renderRoot = cbox as LayoutFarm.Composers.CssRenderRoot;

                if (renderRoot != null)
                {
                    //found root then stop
                    this.wrapper.AdjustX = globalX;
                    this.wrapper.AdjustY = globalY;
                    return renderRoot.ContainerElement;
                }
                cbox = cbox.ParentBox;
            }
            return null;
        }
    }


    sealed class WrapperBlockCssBox : WrapperCssBoxBase
    {

        public WrapperBlockCssBox(object controller,
             BoxSpec spec,
             RenderElement renderElement)
            : base(controller, spec, renderElement.Root, CssDisplay.Block)
        {
            SetAsCustomCssBox(this);
            int w = renderElement.Width;
            int h = renderElement.Height;
            this.wrapper = new CssBoxWrapperRenderElement(renderElement.Root, w, h, renderElement);
            ChangeDisplayType(this, CssDisplay.Block);

            this.SetSize(w, h);
            LayoutFarm.RenderElement.SetParentLink(
             wrapper,
             new RenderBoxWrapperLink(this));
            LayoutFarm.RenderElement.SetParentLink(
                renderElement,
                new RenderBoxWrapperLink2(wrapper));

        }


        protected override CssBox GetElementGlobalLocationImpl(out float globalX, out  float globalY)
        {
            globalX = globalXForRenderElement;
            globalY = globalYForRenderElement;
            return null;//             
        }
        public override bool CustomContentHitTest(float x, float y, CssBoxHitChain hitChain)
        {
            return false;
        }
        public override void CustomRecomputedValue(CssBox containingBlock, GraphicsPlatform gfxPlatform)
        {
            var ibox = CssBox.UnsafeGetController(this) as IBoxElement;
            if (ibox != null)
            {
                //todo: user minimum font height of the IBoxElement
                int w = (int)this.SizeWidth;
                int h = Math.Max((int)this.SizeHeight, ibox.MinHeight);

                ibox.ChangeElementSize(w, h);
                this.SetSize(w, h);
            }
            else
            {
                this.SetSize(100, 20);
            }

        }

        protected override void PaintImp(PaintVisitor p)
        {
            if (wrapper != null)
            {

                GetParentRenderElement(out this.globalXForRenderElement, out this.globalYForRenderElement);
                Rectangle rect = new Rectangle(0, 0, wrapper.Width, wrapper.Height);
                this.wrapper.DrawToThisCanvas(p.InnerCanvas, rect);
                p.FillRectangle(Color.Red, 0, 0, 10, 10);

            }
            else
            {
                //for debug!
                p.FillRectangle(Color.Red, 0, 0, 100, 100);
            }
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
                var renderRoot = cbox as LayoutFarm.Composers.CssRenderRoot;

                if (renderRoot != null)
                {
                    this.wrapper.AdjustX = globalX;
                    this.wrapper.AdjustY = globalY;
                    return renderRoot.ContainerElement;
                }
                cbox = cbox.ParentBox;
            }
            return null;
        }
    }


    //-----------------------------------------

    namespace SpecialInternalWrappers
    {

        /// <summary>
        /// special render element that bind RenderElement and CssBox
        /// </summary>
        class CssBoxWrapperRenderElement : RenderElement
        {
            RenderElement renderElement;
            int adjustX;
            int adjustY;

            public CssBoxWrapperRenderElement(RootGraphic rootgfx, int w, int h, RenderElement renderElement)
                : base(rootgfx, w, h)
            {
                this.renderElement = renderElement;
            }
            public int AdjustX
            {
                get { return this.adjustX; }
                set
                {
                    this.adjustX = value;
                }
            }
            public int AdjustY
            {
                get { return this.adjustY; }
                set
                {
                    if (this.adjustY > 0 && value == 0)
                    {

                    }
                    this.adjustY = value;
                }
            }
            public override int BubbleUpX
            {
                get
                {

                    return this.AdjustX;
                }
            }
            public override int BubbleUpY
            {
                get
                {
                    return this.AdjustY;
                }
            }

            public override void CustomDrawToThisCanvas(Canvas canvasPage, Rectangle updateArea)
            {
                renderElement.CustomDrawToThisCanvas(canvasPage, updateArea);
            }
        }
        class RenderBoxWrapperLink : LayoutFarm.RenderBoxes.IParentLink
        {
            WrapperCssBoxBase box;
            public RenderBoxWrapperLink(WrapperCssBoxBase box)
            {
                this.box = box;
            }

            public bool MayHasOverlapChild { get { return false; } }
            public RenderElement ParentRenderElement
            {
                get
                {
                    int globalX;
                    int globalY;
                    return box.GetParentRenderElement(out globalX, out globalY);
                }
            }
            public void AdjustLocation(ref Point p) { }
            public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
            {
                return null;
            }
            public RenderElement NotifyParentToInvalidate(out bool goToFinalExit

#if DEBUG
, RenderElement ve
#endif
)
            {
                goToFinalExit = false;
                int globalX;
                int globalY;
                var parent = box.GetParentRenderElement(out globalX, out globalY);

                if (parent != null)
                {
                    parent.InvalidateGraphics();
                }
                return parent;
            }

#if DEBUG
            public string dbugGetLinkInfo() { return ""; }
#endif
        }
        class RenderBoxWrapperLink2 : LayoutFarm.RenderBoxes.IParentLink
        {
            RenderElement box;
            public RenderBoxWrapperLink2(RenderElement box)
            {
                this.box = box;
            }

            public bool MayHasOverlapChild { get { return false; } }
            public RenderElement ParentRenderElement
            {
                get
                {
                    return box;
                }
            }
            public void AdjustLocation(ref Point p) { }
            public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
            {
                return null;
            }
            public RenderElement NotifyParentToInvalidate(out bool goToFinalExit

#if DEBUG
, RenderElement ve
#endif
)
            {
                goToFinalExit = false;
                //int globalX;
                //int globalY;
                var parent = box.ParentRenderElement;

                if (parent != null)
                {
                    parent.InvalidateGraphics();
                }
                return parent;
            }

#if DEBUG
            public string dbugGetLinkInfo() { return ""; }
#endif
        }
    }

}