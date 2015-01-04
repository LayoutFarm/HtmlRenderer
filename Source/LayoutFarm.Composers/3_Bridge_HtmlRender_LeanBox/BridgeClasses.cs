//2014 Apache2, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

using HtmlRenderer.WebDom;
using HtmlRenderer;
using HtmlRenderer.Css;
using HtmlRenderer.ContentManagers;
using HtmlRenderer.Composers;
using HtmlRenderer.Boxes;

namespace LayoutFarm.Boxes
{

    public class HtmlRenderBox : RenderBoxBase
    {
        MyHtmlIsland myHtmlIsland;
        int myWidth;
        int myHeight;
        public HtmlRenderBox(RootGraphic rootgfx,
            int width, int height,
            MyHtmlIsland htmlIsland)
            : base(rootgfx, width, height)
        {
            this.myWidth = width;
            this.myHeight = height;
            this.myHtmlIsland = htmlIsland;
            this.Focusable = false;
        }
        public override void ClearAllChildren()
        {

        }
        protected override void BoxDrawContent(Canvas canvasPage, Rect updateArea)
        {
            myHtmlIsland.PhysicalViewportBound = new LayoutFarm.Drawing.RectangleF(0, 0, myWidth, myHeight);
            myHtmlIsland.CheckDocUpdate();
            myHtmlIsland.PerformPaint(canvasPage);
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {
        }
    }


    public sealed class RenderElementInsideCssBox : CustomCssBox
    {
        CssBoxInsideRenderElement wrapper;
        int globalXForRenderElement;
        int globalYForRenderElement;

        public RenderElementInsideCssBox(object controller,
             BoxSpec spec,
             RenderElement renderElement)
            : base(controller, spec, CssDisplay.Block)
        {
            int mmw = 100;
            int mmh = 20;

            this.wrapper = new CssBoxInsideRenderElement(renderElement.Root, mmw, mmh, renderElement);

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
            this.SetSize(100, 20);

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
        protected override void PaintImp(Painter p)
        {
            if (wrapper != null)
            {

                GetParentRenderElement(out this.globalXForRenderElement, out this.globalYForRenderElement);

                Rect rect = Rect.CreateFromRect(
                     new Rectangle(0, 0, wrapper.Width, wrapper.Height));
                this.wrapper.DrawToThisPage(p.InnerCanvas, rect);

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
                var renderRoot = cbox as HtmlRenderer.Composers.BridgeHtml.CssRenderRoot;
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



        class CssBoxInsideRenderElement : RenderElement
        {
            RenderElement renderElement;
            int adjustX;
            int adjustY;

            public CssBoxInsideRenderElement(RootGraphic rootgfx, int w, int h, RenderElement renderElement)
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

            public override void CustomDrawToThisPage(Canvas canvasPage, Rect updateArea)
            {
                int x = this.adjustX;
                int y = this.adjustY;
                renderElement.CustomDrawToThisPage(canvasPage, updateArea);

            }
        }
        class RenderBoxWrapperLink : IParentLink
        {
            RenderElementInsideCssBox box;
            public RenderBoxWrapperLink(RenderElementInsideCssBox box)
            {
                this.box = box;
            }

            public bool MayHasOverlapChild { get { return false; } }
            public RenderElement ParentVisualElement
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
                    parent.InvalidateGraphic();
                }
                return parent;
            }

#if DEBUG
            public string dbugGetLinkInfo() { return ""; }
#endif
        }
        class RenderBoxWrapperLink2 : IParentLink
        {
            RenderElement box;
            public RenderBoxWrapperLink2(RenderElement box)
            {
                this.box = box;
            }

            public bool MayHasOverlapChild { get { return false; } }
            public RenderElement ParentVisualElement
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
                int globalX;
                int globalY;
                var parent = box.ParentVisualElement;

                if (parent != null)
                {
                    parent.InvalidateGraphic();
                }
                return parent;
            }

#if DEBUG
            public string dbugGetLinkInfo() { return ""; }
#endif
        }
    }



    //===================================================================
    public class HtmlFragmentRenderBox : RenderBoxBase
    {

        CssBox cssBox;
        int myWidth;
        int myHeight;
        public HtmlFragmentRenderBox(RootGraphic rootgfx,
            int width, int height)
            : base(rootgfx, width, height)
        {
            this.myWidth = width;
            this.myHeight = height; 
            this.Focusable = false;
        }
        public CssBox CssBox
        {
            get { return this.cssBox; }
            set { this.cssBox = value; }
        }
        public override void ClearAllChildren()
        {

        }
        protected override void BoxDrawContent(Canvas canvasPage, Rect updateArea)
        {
            //myHtmlIsland.PhysicalViewportBound = new LayoutFarm.Drawing.RectangleF(0, 0, myWidth, myHeight);
            //myHtmlIsland.CheckDocUpdate();
            //myHtmlIsland.PerformPaint(canvasPage);
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {

        }
    }



}





