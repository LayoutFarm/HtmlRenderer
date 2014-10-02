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

    public class CssBoxWrapperRenderBox : RenderBoxBase
    {

        MyHtmlIsland myHtmlIsland;
        int myWidth;
        int myHeight;
        public CssBoxWrapperRenderBox(RootGraphic rootgfx,
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

            hitChain.dbugBreak = false;

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



    public sealed class LeanWrapperCssBox : CssBox
    {
        //bridge between parent CssBox and  inner RenderElement

        RenderElement renderElement;
        public LeanWrapperCssBox(object controller,
             BoxSpec spec,
            RenderElement renderElement)
            : base(controller, spec, CssDisplay.Block)
        {
            this.renderElement = renderElement;
            ChangeDisplayType(this, CssDisplay.Block);
            SetAsCustomCssBox(this);
            this.SetSize(100, 20);
        }

        public LayoutFarm.RenderElement RenderElement
        {
            get { return this.renderElement; }
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
            if (renderElement != null)
            {
                LayoutFarm.InternalRect rect = LayoutFarm.InternalRect.CreateFromRect(
                    new Rectangle(0, 0, renderElement.Width, renderElement.Height));
                this.renderElement.DrawToThisPage(g.CurrentCanvas, rect);
                LayoutFarm.InternalRect.FreeInternalRect(rect);



            }
            else
            {
                //for debug!
                g.FillRectangle(LayoutFarm.Drawing.Brushes.Red,
                    0, 0, 100, 20);
            }
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





