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
        } 
        /// <summary>
        /// Perform html container layout by the current panel client size.
        /// </summary>
        void PerformHtmlLayout(IGraphics g)
        {
            if (myHtmlIsland != null)
            {
                myHtmlIsland.MaxSize = new LayoutFarm.Drawing.SizeF(this.myWidth, 0);
                myHtmlIsland.PerformLayout(g);

                //using (var g = CreateGraphics())
                //{
                //    _visualRootBox.PerformLayout(g);
                //}
                //AutoScrollMinSize = Size.Round(_visualRootBox.ActualSize);
            }
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
        public override void ChildrenHitTestCore(HitPointChain artHitResult)
        {
            //hit test in another system 
        }

    }
}





