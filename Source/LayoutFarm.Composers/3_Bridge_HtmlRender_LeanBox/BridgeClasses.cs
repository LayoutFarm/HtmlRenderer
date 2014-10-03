//2014 Apache2, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HtmlRenderer.WebDom;
using LayoutFarm.Drawing;
using HtmlRenderer;
using HtmlRenderer.Css;
using HtmlRenderer.ContentManagers;
using HtmlRenderer.Composers;
using HtmlRenderer.Boxes;

namespace LayoutFarm
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
        protected override void BoxDrawContent(Canvas canvasPage, InternalRect updateArea)
        {
            myHtmlIsland.PhysicalViewportBound = new LayoutFarm.Drawing.RectangleF(0, 0, myWidth, myHeight);
            myHtmlIsland.CheckDocUpdate();
            myHtmlIsland.PerformPaint(canvasPage);
        }
        public override void ChildrenHitTestCore(HitPointChain hitChain)
        {
            // bridge to another system
            // test only ***


            //hit test in another system ***  
            BoxHitChain boxHitChain = new BoxHitChain();
            //_latestMouseDownHitChain = hitChain;
            Point testPoint = hitChain.TestPoint;
            boxHitChain.SetRootGlobalPosition(testPoint.X, testPoint.Y);
            ////1. prob hit chain only
            BoxUtils.HitTest(myHtmlIsland.GetRootCssBox(), testPoint.X, testPoint.Y, boxHitChain);
            ///-----------------------------
            //add box hit chain to hit point chain  
            hitChain.AddHit(new BoxHitChainWrapper(boxHitChain));

        }
    }


    public sealed class RenderElementInsideCssBox : CssBox
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
            SetAsCustomCssBox(this);
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

        public override void CustomRecomputedValue(CssBox containingBlock)
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

        protected override void PaintImp(IGraphics g, Painter p)
        {
            if (wrapper != null)
            {

                GetParentRenderElement(out this.globalXForRenderElement, out this.globalYForRenderElement);

                LayoutFarm.InternalRect rect = LayoutFarm.InternalRect.CreateFromRect(
                    new Rectangle(0, 0, wrapper.Width, wrapper.Height));
                var canvas = g.CurrentCanvas;
                this.wrapper.DrawToThisPage(canvas, rect);

                LayoutFarm.InternalRect.FreeInternalRect(rect);

            }
            else
            {
                //for debug!
                g.FillRectangle(LayoutFarm.Drawing.Brushes.Red,
                    0, 0, 100, 20);
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
                    //return base.BubbleUpX;
                    return this.AdjustX;
                }
            }
            public override int BubbleUpY
            {
                get
                {
                    return this.AdjustY;
                    //return base.BubbleUpY;
                    //return this.AdjustY;
                }
            }

            public override void CustomDrawToThisPage(Canvas canvasPage, InternalRect updateArea)
            {
                int x = this.adjustX;
                int y = this.adjustY;

                // canvasPage.OffsetCanvasOrigin(x, y);
                //updateArea.Offset(-x, -y);


                renderElement.CustomDrawToThisPage(canvasPage, updateArea);

                //canvasPage.OffsetCanvasOrigin(-x, -y);
                //updateArea.Offset(x, y);

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










    public class BoxHitChainWrapper : LayoutFarm.IHitElement
    {
        object controller;
        BoxHitChain boxHitChain;
        public BoxHitChainWrapper(BoxHitChain boxHitChain)
        {
            this.boxHitChain = boxHitChain;
            this.controller = boxHitChain;
        }
        public object GetController()
        {
            return controller;
        }
        public void SetController(object controller)
        {
            this.controller = controller;
        }
        bool IHitElement.IsTestable()
        {
            return true;
        }
        IHitElement IHitElement.FindOverlapSibling(Drawing.Point p)
        {
            return null;
        }
        Point IHitElement.ElementLocation
        {
            get { return Point.Empty; }
        }
        Point IHitElement.GetElementGlobalLocation()
        {
            return Point.Empty;
        }
        Rectangle IHitElement.ElementBoundRect
        {
            get { return Rectangle.Empty; }
        }
        bool IHitElement.Focusable
        {
            get { return false; }
        }
        bool IHitElement.HasParent
        {
            get { return true; }
        }
        bool IHitElement.ContainsSubChain
        {
            get { return true; }
        }
        bool IHitElement.Contains(LayoutFarm.Drawing.Point p)
        {
            return true;
        }

        bool IHitElement.HitTestCore(HitPointChain chain)
        {
            return true;
        }

        public BoxHitChain HitChain
        {
            get { return this.boxHitChain; }
        }
    }
}





