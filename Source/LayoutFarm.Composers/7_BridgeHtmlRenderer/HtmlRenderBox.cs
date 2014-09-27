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
        InputEventBridge _htmlEventBridge;

         
        /// <summary>
        /// the base stylesheet data used in the control
        /// </summary>
        CssActiveSheet _baseCssData;

        int myWidth;
        int myHeight;
        public HtmlRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.myWidth = width;
            this.myHeight = height; 
        }

        

        /// <summary>
        /// Perform html container layout by the current panel client size.
        /// </summary>
        void PerformHtmlLayout(IGraphics g)
        {
            if (_htmlIsland != null)
            {
                _htmlIsland.MaxSize = new LayoutFarm.Drawing.SizeF(this.myWidth, 0);
                _htmlIsland.PerformLayout(g);

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
            _htmlIsland.PhysicalViewportBound = new LayoutFarm.Drawing.RectangleF(0, 0, myWidth, myHeight);
            _htmlIsland.PerformPaint(canvasPage);
        }
        public override void ChildrenHitTestCore(HitPointChain artHitResult)
        {
            //hit test in another system 
        }
        public void LoadHtmlText(string html)
        {
            _htmlIsland.SetHtml(html, _baseCssData);
            this.PerformHtmlLayout(CurrentGraphicPlatform.P.SampleIGraphics);

        }
    }
}





