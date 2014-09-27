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
        MyHtmlIsland _htmlIsland;

        int myWidth;
        int myHeight;
        public HtmlRenderBox(RootGraphic rootgfx,
            int width, int height,
            MyHtmlIsland htmlIsland)
            : base(rootgfx, width, height)
        {
            this.myWidth = width;
            this.myHeight = height;
            this._htmlIsland = htmlIsland;
        }
        /// <summary>
        /// Perform html container layout by the current panel client size.
        /// </summary>
        public void PerformHtmlLayout(IGraphics g)
        {
            if (_htmlIsland != null)
            {
                _htmlIsland.MaxSize = new LayoutFarm.Drawing.SizeF(this.myWidth, 0);
                _htmlIsland.PerformLayout(g);
            }
        }
        public override void ClearAllChildren()
        {

        }
        protected override void BoxDrawContent(Canvas canvasPage, InternalRect updateArea)
        {
            _htmlIsland.PhysicalViewportBound = new RectangleF(0, 0, myWidth, myHeight);
            _htmlIsland.PerformPaint(canvasPage);
        }
        public override void ChildrenHitTestCore(HitPointChain artHitResult)
        {
            //hit test in another system 
        }
        
    }
}





