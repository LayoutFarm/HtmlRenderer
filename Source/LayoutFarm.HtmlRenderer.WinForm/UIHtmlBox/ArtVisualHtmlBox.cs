//2014 Apache2, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using HtmlRenderer.WebDom;
using HtmlRenderer.Drawing;
using HtmlRenderer.Css;
using HtmlRenderer.ContentManagers;

using HtmlRenderer;
using HtmlRenderer.Composers;

namespace LayoutFarm.Presentation
{

    public class ArtVisualHtmlBox : ArtVisualContainerBase
    {
        WinRootVisualBox _visualRootBox;
        BoxComposer _boxComposer;
        InputEventBridge _htmlEventBridge;

        public event EventHandler<StylesheetLoadEventArgs> StylesheetLoad;

        /// <summary>
        /// Raised when an image is about to be loaded by file path or URI.<br/>
        /// This event allows to provide the image manually, if not handled the image will be loaded from file or download from URI.
        /// </summary>
        public event EventHandler<HtmlRenderer.ContentManagers.ImageRequestEventArgs> ImageLoad;


        /// <summary>
        /// the raw base stylesheet data used in the control
        /// </summary>
        string _baseRawCssData;

        /// <summary>
        /// the base stylesheet data used in the control
        /// </summary>
        CssActiveSheet _baseCssData;


        int myWidth;
        int myHeight;
        public ArtVisualHtmlBox(int width, int height)
            : base(width, height, VisualElementNature.HtmlContainer)
        {
            this.myWidth = width;
            this.myHeight = height;

            //-------------------------------------------------------
            _boxComposer = new BoxComposer();

            _visualRootBox = new WinRootVisualBox();
            _visualRootBox.BoxComposer = _boxComposer;

            //_visualRootBox.RenderError += OnRenderError;
            _visualRootBox.Refresh += OnRefresh;
            //_visualRootBox.ScrollChange += OnScrollChange;
            _visualRootBox.TextContentMan.StylesheetLoadingRequest += OnStylesheetLoad;
            _visualRootBox.ImageContentMan.ImageLoadingRequest += OnImageLoad;

            //-------------------------------------------
            _htmlEventBridge = new InputEventBridge();
            _htmlEventBridge.Bind(_visualRootBox);
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
            if (_visualRootBox != null)
            {
                _visualRootBox.MaxSize = new SizeF(this.myWidth, 0);
                _visualRootBox.PerformLayout(g);

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
        protected override bool HasGroundLayer()
        {
            return true;
        }
        protected override VisualLayer GetGroundLayer()
        {
            return null;
            
        }
        public override void CustomDrawToThisPage(CanvasBase canvasPage, InternalRect updateArea)
        {
            _visualRootBox.PhysicalViewportBound = new RectangleF(0, 0, myWidth, myHeight);
            _visualRootBox.PerformPaint(canvasPage.GetGfx());
        }
        public override void ChildrenHitTestCore(ArtHitPointChain artHitResult)
        {
            //hit test in another system 
        }
        internal void LoadHtmlText(string html)
        {
            _visualRootBox.SetHtml(html, _baseCssData);
            using (Bitmap bb = new Bitmap(2, 2))
            using (Graphics g = Graphics.FromImage(bb))
            using (WinGraphics winGfx = new WinGraphics(g, false))
            {   
                this.PerformHtmlLayout(winGfx);
            }

        }
    }
}





