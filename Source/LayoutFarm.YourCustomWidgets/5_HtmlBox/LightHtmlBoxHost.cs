//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HtmlRenderer;
using HtmlRenderer.ContentManagers;
using HtmlRenderer.Boxes;
using HtmlRenderer.Composers;

using LayoutFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.Boxes;

namespace LayoutFarm.CustomWidgets
{
    /// <summary>
    /// common host for light htmlbox
    /// </summary>
    public class LightHtmlBoxHost
    {
        HtmlRenderer.WebDom.WebDocument currentdoc;
        bool hasWaitingDocToLoad;
        HtmlRenderer.WebDom.CssActiveSheet waitingCssData;
        GraphicsPlatform gfxPlatform;
        MyHtmlIsland myHtmlIsland;
        public event EventHandler<TextLoadRequestEventArgs> RequestStylesheet;
        public event EventHandler<ImageRequestEventArgs> RequestImage;
        int _width = 800; //temp
        RootGraphic rootgfx;
        HtmlRenderBox myCssBoxWrapper;
        HtmlInputEventAdapter _htmlInputEventBridge;

        static LightHtmlBoxHost()
        {
            //TODO: revise this again
            HtmlRenderer.Composers.BridgeHtml.BoxCreator.RegisterCustomCssBoxGenerator(
               new HtmlRenderer.Boxes.LeanBoxCreator());
        }

        public LightHtmlBoxHost(GraphicsPlatform gfxPlatform)
        {
            this.gfxPlatform = gfxPlatform;
            myHtmlIsland = new MyHtmlIsland(gfxPlatform);
            myHtmlIsland.BaseStylesheet = HtmlRenderer.Composers.CssParserHelper.ParseStyleSheet(null, true);
            myHtmlIsland.Refresh += OnRefresh;
            myHtmlIsland.NeedUpdateDom += myHtmlIsland_NeedUpdateDom;
            myHtmlIsland.RequestResource += myHtmlIsland_RequestResource;

            _htmlInputEventBridge = new HtmlInputEventAdapter();
            _htmlInputEventBridge.Bind(myHtmlIsland, gfxPlatform.SampleIFonts);

        }
        public void SetRootGraphic(RootGraphic rootgfx)
        {
            this.rootgfx = rootgfx;
        }
        internal GraphicsPlatform P
        {
            get { return this.gfxPlatform; }
        }
        internal HtmlInputEventAdapter InputEventAdaper
        {
            get { return this._htmlInputEventBridge; }
        }
        /// <summary>
        /// Handle html renderer invalidate and re-layout as requested.
        /// </summary>
        void OnRefresh(object sender, HtmlRenderer.WebDom.HtmlRefreshEventArgs e)
        {
            //this.InvalidateGraphic();
        }

        void myHtmlIsland_RequestResource(object sender, HtmlResourceRequestEventArgs e)
        {
            if (this.RequestImage != null)
            {
                RequestImage(this, new ImageRequestEventArgs(e.binder));
            }
        }
        void myHtmlIsland_NeedUpdateDom(object sender, EventArgs e)
        {
            hasWaitingDocToLoad = true;
            //---------------------------
            if (myCssBoxWrapper == null) return;
            //---------------------------

            var builder = new HtmlRenderer.Composers.RenderTreeBuilder(this.rootgfx);
            builder.RequestStyleSheet += (e2) =>
            {
                if (this.RequestStylesheet != null)
                {
                    var req = new TextLoadRequestEventArgs(e2.Src);
                    RequestStylesheet(this, req);
                    e2.SetStyleSheet = req.SetStyleSheet;
                }
            };
            var rootBox2 = builder.RefreshCssTree(this.currentdoc,
                gfxPlatform.SampleIFonts,
                this.myHtmlIsland);

            this.myHtmlIsland.PerformLayout();

        }
        void UpdateWaitingHtmlDoc(RootGraphic rootgfx)
        {
            var builder = new HtmlRenderer.Composers.RenderTreeBuilder(rootgfx);
            builder.RequestStyleSheet += (e) =>
            {
                if (this.RequestStylesheet != null)
                {
                    var req = new TextLoadRequestEventArgs(e.Src);
                    RequestStylesheet(this, req);
                    e.SetStyleSheet = req.SetStyleSheet;
                }
            };

            //build rootbox from htmldoc
            var rootBox = builder.BuildCssRenderTree(this.currentdoc,
                rootgfx.SampleIFonts,
                this.myHtmlIsland,
                this.waitingCssData,
                this.myCssBoxWrapper);

            //update htmlIsland
            var htmlIsland = this.myHtmlIsland;
            htmlIsland.SetHtmlDoc(this.currentdoc);
            htmlIsland.SetRootCssBox(rootBox, this.waitingCssData);
            htmlIsland.MaxSize = new LayoutFarm.Drawing.SizeF(this._width, 0);
            htmlIsland.PerformLayout();
        }


        void SetHtml(MyHtmlIsland htmlIsland, string html, HtmlRenderer.WebDom.CssActiveSheet cssData)
        {
            var htmldoc = HtmlRenderer.Composers.WebDocumentParser.ParseDocument(
                             new HtmlRenderer.WebDom.Parser.TextSnapshot(html.ToCharArray()));
            this.currentdoc = htmldoc;
            this.hasWaitingDocToLoad = true;
            this.waitingCssData = cssData;
            //---------------------------
            if (myCssBoxWrapper == null) return;
            //---------------------------
            UpdateWaitingHtmlDoc(this.myCssBoxWrapper.Root);
        }

        public LightHtmlBox CreateLightBox(int w, int h)
        {
            LightHtmlBox lightBox = new LightHtmlBox(this, w, h);
            return lightBox;
        }
        public void LoadNewFragment(string htmlFragment)
        {

        }

    }
}