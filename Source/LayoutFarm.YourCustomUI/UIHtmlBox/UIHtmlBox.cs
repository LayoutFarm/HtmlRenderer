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
        HtmlRenderer.WebDom.WebDocument currentdoc;

        HtmlRenderer.Composers.InputEventBridge _htmlEventBridge;

        public event EventHandler<TextLoadRequestEventArgs> StylesheetLoad;
        public event EventHandler<ImageRequestEventArgs> ImageLoad;

        TextContentManager _textMan;
        ImageContentManager _imageMan;
        public UIHtmlBox(int width, int height)
        {
            this._width = width;
            this._height = height;
            _textMan = new TextContentManager();
            _imageMan = new ImageContentManager();


            myHtmlIsland = new MyHtmlIsland();
            myHtmlIsland.BaseStylesheet = HtmlRenderer.Composers.CssParserHelper.ParseStyleSheet(null, true);
            myHtmlIsland.Refresh += OnRefresh;
            myHtmlIsland.NeedUpdateDom += new EventHandler<EventArgs>(myHtmlIsland_NeedUpdateDom);


            _textMan.StylesheetLoadingRequest += OnStylesheetLoad;
            _imageMan.ImageLoadingRequest += OnImageLoad;

        }

        void myHtmlIsland_NeedUpdateDom(object sender, EventArgs e)
        {
            
            HtmlRenderer.Composers.BoxModelBuilder builder = new HtmlRenderer.Composers.BoxModelBuilder();
            builder.RequestStyleSheet += (e2) =>
            {

                TextLoadRequestEventArgs req = new TextLoadRequestEventArgs(e2.Src); 
                _textMan.AddStyleSheetRequest(req);
                e2.SetStyleSheet = req.SetStyleSheet; 
            };
            var rootBox2 = builder.RefreshCssTree(this.currentdoc, LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics, this.myHtmlIsland);
            this.myHtmlIsland.PerformLayout(LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics);             

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

        /// <summary>
        /// Propagate the stylesheet load event from root container.
        /// </summary>
        void OnStylesheetLoad(object sender, TextLoadRequestEventArgs e)
        {
            if (StylesheetLoad != null)
            {
                StylesheetLoad(this, e);
            }
        }

        /// <summary>
        /// Propagate the image load event from root container.
        /// </summary>
        void OnImageLoad(object sender, HtmlRenderer.ContentManagers.ImageRequestEventArgs e)
        {
            if (ImageLoad != null)
            {
                ImageLoad(this, e);
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (myHtmlBox == null)
            {
                _htmlEventBridge = new HtmlRenderer.Composers.InputEventBridge();
                _htmlEventBridge.Bind(myHtmlIsland, rootgfx.SampleIFonts);

                myHtmlBox = new HtmlRenderBox(rootgfx, _width, _height, myHtmlIsland);

            }
            return myHtmlBox;
        }
        void SetHtml(MyHtmlIsland htmlIsland, string html, HtmlRenderer.WebDom.CssActiveSheet cssData)
        {

            HtmlRenderer.Composers.BoxModelBuilder builder = new HtmlRenderer.Composers.BoxModelBuilder();
            builder.RequestStyleSheet += (e) =>
            {
                var req = new TextLoadRequestEventArgs(e.Src); 
                this._textMan.AddStyleSheetRequest(req);
                e.SetStyleSheet = req.SetStyleSheet;
            };


            var htmldoc = builder.ParseDocument(new HtmlRenderer.WebDom.Parser.TextSnapshot(html.ToCharArray()));
            this.currentdoc = htmldoc;

            //build rootbox from htmldoc
            var rootBox = builder.BuildCssTree(htmldoc, LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics, htmlIsland, cssData);

            htmlIsland.SetHtmlDoc(htmldoc);
            htmlIsland.SetRootCssBox(rootBox, cssData);
        }
        public void LoadHtmlText(string html)
        {
            //myHtmlBox.LoadHtmlText(html);
            SetHtml(myHtmlIsland, html, myHtmlIsland.BaseStylesheet);
            myHtmlBox.InvalidateGraphic();
        }


        public override void InvalidateGraphic()
        {
            myHtmlBox.InvalidateGraphic();
        }
    }
}





