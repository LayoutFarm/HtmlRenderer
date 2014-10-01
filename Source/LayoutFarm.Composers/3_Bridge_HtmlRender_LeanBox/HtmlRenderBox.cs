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
            if (hitChain.dbugBreak)
            {
                //hit test in another system ***  
                BoxHitChain boxHitChain = new BoxHitChain();
                //_latestMouseDownHitChain = hitChain;
                Point testPoint = hitChain.TestPoint;
                boxHitChain.SetRootGlobalPosition(testPoint.X, testPoint.Y);
                ////1. prob hit chain only
                BoxUtils.HitTest(myHtmlIsland.GetRootCssBox(), testPoint.X, testPoint.Y, boxHitChain);
                ///-----------------------------
                //add box hit chain to hit point chain



            }

        }

    }
}





