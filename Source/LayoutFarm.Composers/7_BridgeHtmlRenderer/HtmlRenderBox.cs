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

        public event EventHandler<StylesheetLoadEventArgs> StylesheetLoad;

        /// <summary>
        /// Raised when an image is about to be loaded by file path or URI.<br/>
        /// This event allows to provide the image manually, if not handled the image will be loaded from file or download from URI.
        /// </summary>
        public event EventHandler<HtmlRenderer.ContentManagers.ImageRequestEventArgs> ImageLoad;


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

            _htmlIsland = new MyHtmlIsland();
            _htmlIsland.TextContentMan = new TextContentManager();
            _htmlIsland.ImageContentMan = new ImageContentManager();

            _htmlIsland.Refresh += OnRefresh;
            _htmlIsland.TextContentMan.StylesheetLoadingRequest += OnStylesheetLoad;
            _htmlIsland.ImageContentMan.ImageLoadingRequest += OnImageLoad;

            //-------------------------------------------
            _htmlEventBridge = new InputEventBridge();
            _htmlEventBridge.Bind(_htmlIsland);
            //-------------------------------------------
            _baseCssData = HtmlRenderer.Composers.CssParserHelper.ParseStyleSheet(null, true);
        }

        /// <summary>
        /// Propagate the stylesheet load event from root container.
        /// </summary>
        private void OnStylesheetLoad(object sender, StylesheetLoadEventArgs e)
        {
            if (StylesheetLoad != null)
            {
                StylesheetLoad(this, e);
            }
        }

        /// <summary>
        /// Propagate the image load event from root container.
        /// </summary>
        private void OnImageLoad(object sender, HtmlRenderer.ContentManagers.ImageRequestEventArgs e)
        {
            if (ImageLoad != null)
            {
                ImageLoad(this, e);
            }
        }
        /// <summary>
        /// Handle html renderer invalidate and re-layout as requested.
        /// </summary>
        private void OnRefresh(object sender, HtmlRefreshEventArgs e)
        {
            //if (e.Layout)
            //{
            //    if (InvokeRequired)
            //        Invoke(new MethodInvoker(PerformLayout));
            //    else
            //        PerformLayout();
            //}
            //if (InvokeRequired)
            //    Invoke(new MethodInvoker(Invalidate));
            //else
            //    Invalidate();
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





