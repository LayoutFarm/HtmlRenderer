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
        bool hasWaitingDocToLoad;
        HtmlRenderer.WebDom.WebDocument currentdoc;
        GraphicsPlatform gfxPlatform;
        public event EventHandler<TextLoadRequestEventArgs> RequestStylesheet;
        public event EventHandler<ImageRequestEventArgs> RequestImage;
        int _width = 800; //temp
        RootGraphic rootgfx;

        Queue<HtmlInputEventAdapter> inputEventAdapterStock = new Queue<HtmlInputEventAdapter>();

        HtmlRenderer.WebDom.CssActiveSheet baseStyleSheet;


        static LightHtmlBoxHost()
        {
            //TODO: revise this again
            HtmlRenderer.Composers.BridgeHtml.BoxCreator.RegisterCustomCssBoxGenerator(
               new HtmlRenderer.Boxes.LeanBoxCreator());
        }

        public LightHtmlBoxHost(GraphicsPlatform gfxPlatform)
        {
            this.gfxPlatform = gfxPlatform;
            baseStyleSheet = HtmlRenderer.Composers.CssParserHelper.ParseStyleSheet(null, true);
            //myHtmlIsland = new MyHtmlIsland(gfxPlatform);
            //myHtmlIsland.BaseStylesheet = HtmlRenderer.Composers.CssParserHelper.ParseStyleSheet(null, true);
            //myHtmlIsland.Refresh += OnRefresh;
            //myHtmlIsland.NeedUpdateDom += myHtmlIsland_NeedUpdateDom;
            //myHtmlIsland.RequestResource += myHtmlIsland_RequestResource;
            inputEventAdapterStock.Enqueue(new HtmlInputEventAdapter(gfxPlatform.SampleIFonts));
            //_htmlInputEventBridge.Bind(myHtmlIsland);

        }
        public void SetRootGraphic(RootGraphic rootgfx)
        {
            this.rootgfx = rootgfx;
        }
        internal GraphicsPlatform P
        {
            get { return this.gfxPlatform; }
        }

        internal HtmlInputEventAdapter GetSharedInputEventAdapter(HtmlIsland island)
        {
            HtmlInputEventAdapter adapter = null;
            if (inputEventAdapterStock.Count == 0)
            {
                adapter = new HtmlInputEventAdapter(this.gfxPlatform.SampleIFonts); 
            }
            else
            { 
                adapter= this.inputEventAdapterStock.Dequeue();
            }
            adapter.Bind(island);
            return adapter; 
        }
        internal void ReleaseSharedInputEventAdapter(HtmlInputEventAdapter adapter)
        {
            adapter.Unbind();
            this.inputEventAdapterStock.Enqueue(adapter);
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
            //hasWaitingDocToLoad = true;
            ////---------------------------
            ////if (htmlRenderBox == null) return;
            ////--------------------------- 
            //var builder = new HtmlRenderer.Composers.RenderTreeBuilder(this.rootgfx);
            //builder.RequestStyleSheet += (e2) =>
            //{
            //    if (this.RequestStylesheet != null)
            //    {
            //        var req = new TextLoadRequestEventArgs(e2.Src);
            //        RequestStylesheet(this, req);
            //        e2.SetStyleSheet = req.SetStyleSheet;
            //    }
            //}; 
            //var rootBox2 = builder.RefreshCssTree(this.currentdoc); 
            //this.myHtmlIsland.PerformLayout();

        }


        public LightHtmlBox CreateLightBox(int w, int h)
        {
            LightHtmlBox lightBox = new LightHtmlBox(this, w, h);
            return lightBox;
        }

        //----------------------------------------------------

        public void CreateHtmlFragment(string htmlFragment, RenderElement container, out MyHtmlIsland newIsland, out CssBox newCssBox)
        {
            //1. builder
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

            //-------------------------------------------------------------------
            //2. parse
            //parse fragment to dom
            //temp fix
            //TODO: improve technique here
            htmlFragment = "<html><head></head><body>" + htmlFragment + "</body></html>";
            var htmldoc = HtmlRenderer.Composers.WebDocumentParser.ParseDocument(
                           new HtmlRenderer.WebDom.Parser.TextSnapshot(htmlFragment.ToCharArray()));


            //3. generate render tree
            ////build rootbox from htmldoc
            var rootElement = builder.BuildCssRenderTree(htmldoc,
                rootgfx.SampleIFonts,
                baseStyleSheet,
                container);
            //4. create small island

            var htmlIsland = new MyHtmlIsland(this.gfxPlatform);
            htmlIsland.SetRootCssBox(rootElement);
            htmlIsland.SetHtmlDoc(htmldoc);
            htmlIsland.SetMaxSize(this._width, 0);
            htmlIsland.PerformLayout();

            newIsland = htmlIsland;
            newCssBox = rootElement;

        }

    }
}