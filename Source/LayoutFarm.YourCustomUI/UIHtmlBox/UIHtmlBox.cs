//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.UI;

using HtmlRenderer;
using HtmlRenderer.ContentManagers;

namespace LayoutFarm
{

    public class UIHtmlBox : UIElement
    {
        HtmlRenderBox myHtmlBox;
        int _width, _height;
        MyHtmlIsland myHtmlIsland;

        HtmlRenderer.Composers.InputEventBridge _htmlEventBridge;
        public event EventHandler<StylesheetLoadEventArgs> StylesheetLoad;

        /// <summary>
        /// Raised when an image is about to be loaded by file path or URI.<br/>
        /// This event allows to provide the image manually, if not handled the image will be loaded from file or download from URI.
        /// </summary>
        public event EventHandler<ImageRequestEventArgs> ImageLoad;

        TextContentManager _textMan;
        ImageContentManager _imageMan;

        public UIHtmlBox(int width, int height)
        {
            this._width = width;
            this._height = height;

            myHtmlIsland = new MyHtmlIsland();
            myHtmlIsland.BaseStylesheet = HtmlRenderer.Composers.CssParserHelper.ParseStyleSheet(null, true);
            myHtmlIsland.Refresh += OnRefresh;

            _textMan = new TextContentManager();
            _imageMan = new ImageContentManager();

            //myHtmlIsland.TextContentMan = new TextContentManager();
            //myHtmlIsland.ImageContentMan = new ImageContentManager();


            _textMan.StylesheetLoadingRequest += OnStylesheetLoad;
            _imageMan.ImageLoadingRequest += OnImageLoad;

        }
        /// <summary>
        /// Propagate the stylesheet load event from root container.
        /// </summary>
        void OnStylesheetLoad(object sender, StylesheetLoadEventArgs e)
        {
            if (StylesheetLoad != null)
            {
                StylesheetLoad(this, e);
            }
        }

        /// <summary>
        /// Propagate the image load event from root container.
        /// </summary>
        void OnImageLoad(object sender, ImageRequestEventArgs e)
        {
            if (ImageLoad != null)
            {
                ImageLoad(this, e);
            }
        }
        /// <summary>
        /// Handle html renderer invalidate and re-layout as requested.
        /// </summary>
        void OnRefresh(object sender, HtmlRenderer.WebDom.HtmlRefreshEventArgs e)
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
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (myHtmlBox == null)
            {
                _htmlEventBridge = new HtmlRenderer.Composers.InputEventBridge();
                _htmlEventBridge.Bind(this.myHtmlIsland, rootgfx.SampleIGraphics);

                myHtmlBox = new HtmlRenderBox(rootgfx, _width, _height, myHtmlIsland);

            }
            return myHtmlBox;
        }
        public void LoadHtmlText(string html)
        {
            SetHtml(myHtmlIsland, html, myHtmlIsland.BaseStylesheet);
            myHtmlBox.PerformHtmlLayout(LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics);
            myHtmlBox.InvalidateGraphic();
        }

        public override void InvalidateGraphic()
        {
            myHtmlBox.InvalidateGraphic();
        }
        void SetHtml(MyHtmlIsland container, string html, HtmlRenderer.WebDom.CssActiveSheet cssData)
        {

            HtmlRenderer.Composers.BoxModelBuilder builder = new HtmlRenderer.Composers.BoxModelBuilder();
            builder.RequestStyleSheet += (e) =>
            {
                this._textMan.AddStyleSheetRequest(e); 
            };

            var htmldoc = builder.ParseDocument(new HtmlRenderer.WebDom.Parser.TextSnapshot(html.ToCharArray()));


            //build rootbox from htmldoc
            var rootBox = builder.BuildCssTree(htmldoc,LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics, container, cssData);
            MyHtmlIsland containerImp = container as MyHtmlIsland;
            if (containerImp != null)
            {
                containerImp.SetHtmlDoc(htmldoc);
                containerImp.SetRootCssBox(rootBox, cssData);
            }
        }
    }
}





