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

namespace LayoutFarm.HtmlBoxes.InternalWrappers
{

    sealed class CssExternalRun : CssRun
    {

        RenderElement externalRenderE;
        Rectangle renderElementRect;
        public CssExternalRun(RenderElement externalRenderE)
            : base(CssRunKind.SolidContent) //act as image run****
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
            return "extRun";
        }
#endif
    }

    abstract class WrapperCssBoxBase : CssBox, LayoutFarm.RenderBoxes.IParentLink
    {
        protected int globalXForRenderElement;
        protected int globalYForRenderElement;
        protected CssBoxWrapperRenderElement wrapper;
        public WrapperCssBoxBase(object controller,
             BoxSpec spec,
             RootGraphic root, CssDisplay display)
            : base(spec, root, display)
        {
            this.SetController(controller);
        }
        //public override void InvalidateGraphics()
        //{
        //    int globalX;
        //    int globalY;
        //    var parent = this.GetParentRenderElement(out globalX, out globalY);
        //    parent.InvalidateGraphics();
        //}
        //protected override void InvalidateBubbleUp(Rectangle clientArea)
        //{
            
        //}
        internal abstract RenderElement GetParentRenderElement(out int globalX, out int globalY);

        RenderElement RenderBoxes.IParentLink.ParentRenderElement
        {
            get
            {
                int globalX;
                int globalY;
                return this.GetParentRenderElement(out globalX, out globalY);
            }
        }
        void RenderBoxes.IParentLink.AdjustLocation(ref Point p)
        {

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
            LayoutFarm.RenderElement.SetParentLink(re, wrapper);
            LayoutFarm.RenderElement.SetParentLink(wrapper, this);

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
                var renderRoot = cbox as LayoutFarm.HtmlBoxes.RenderElementBridgeCssBox;

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
        //this is a cssbox that wrap renderElement inside

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
            LayoutFarm.RenderElement.SetParentLink(wrapper, this);
            LayoutFarm.RenderElement.SetParentLink(renderElement, wrapper);
        }


        protected override CssBox GetGlobalLocationImpl(out float globalX, out  float globalY)
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
#if DEBUG
                p.FillRectangle(Color.Red, 0, 0, 10, 10);
#endif

            }
            else
            {
                //for debug!
#if DEBUG
                p.FillRectangle(Color.Red, 0, 0, 100, 100);
#endif
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
                var renderRoot = cbox as LayoutFarm.HtmlBoxes.RenderElementBridgeCssBox;

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



    /// <summary>
    /// special render element that bind RenderElement and CssBox
    /// </summary>
    class CssBoxWrapperRenderElement : RenderElement, LayoutFarm.RenderBoxes.IParentLink
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
        RenderElement RenderBoxes.IParentLink.ParentRenderElement
        {
            //yes return this 
            get { return this; }
        }
        void RenderBoxes.IParentLink.AdjustLocation(ref Point p)
        {
            p.X += adjustX;
            p.Y += adjustY;
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


}