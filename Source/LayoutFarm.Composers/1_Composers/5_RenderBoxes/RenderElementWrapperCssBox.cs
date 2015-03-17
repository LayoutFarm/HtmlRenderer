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


    public sealed class RenderElementWrapperCssBox : CustomCssBox
    {
        CssBoxWrapperRenderElement wrapper;
        int globalXForRenderElement;
        int globalYForRenderElement;

        public RenderElementWrapperCssBox(object controller,
             BoxSpec spec,
             RenderElement renderElement)
            : base(controller, spec, renderElement.Root, CssDisplay.Block)
        {
            int mmw = renderElement.Width;
            int mmh = renderElement.Height;

            //IBoxElement boxElement = controller as IBoxElement;
            //if (boxElement != null)
            //{
            //    boxElement.ChangeElementSize(mmw, mmh);
            //}

            this.wrapper = new CssBoxWrapperRenderElement(renderElement.Root, mmw, mmh, renderElement);

            ChangeDisplayType(this, CssDisplay.Block);

            this.SetSize(mmw, mmh);

            LayoutFarm.RenderElement.SetParentLink(
             wrapper,
             new RenderBoxWrapperLink(this));

            LayoutFarm.RenderElement.SetParentLink(
                renderElement,
                new RenderBoxWrapperLink2(wrapper));


        }


        protected override Point GetElementGlobalLocationImpl()
        {
            return new Point(globalXForRenderElement, globalYForRenderElement);
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




            //var svgElement = this.SvgSpec;
            ////recompute value if need 
            //var cnode = svgElement.GetFirstNode();
            //float containerW = containingBlock.SizeWidth;
            //float emH = containingBlock.GetEmHeight();
            //while (cnode != null)
            //{
            //    cnode.Value.ReEvaluateComputeValue(containerW, 100, emH);
            //    cnode = cnode.Next;
            //} 
            //this.SetSize(500, 500);
        }
        protected override void PaintImp(PaintVisitor p)
        {
            if (wrapper != null)
            {

                GetParentRenderElement(out this.globalXForRenderElement, out this.globalYForRenderElement);

                Rectangle rect = new Rectangle(0, 0, wrapper.Width, wrapper.Height);
                this.wrapper.DrawToThisCanvas(p.InnerCanvas, rect);

            }
            else
            {
                //for debug!
                p.FillRectangle(Color.Red, 0, 0, 100, 100);
            }
        }

        RenderElement GetParentRenderElement(out int globalX, out int globalY)
        {
            CssBox cbox = this;
            globalX = 0;
            globalY = 0;//reset

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
                //int x = this.adjustX;
                //int y = this.adjustY;
                renderElement.CustomDrawToThisCanvas(canvasPage, updateArea);

            }
        }
        class RenderBoxWrapperLink : LayoutFarm.RenderBoxes.IParentLink
        {
            RenderElementWrapperCssBox box;
            public RenderBoxWrapperLink(RenderElementWrapperCssBox box)
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