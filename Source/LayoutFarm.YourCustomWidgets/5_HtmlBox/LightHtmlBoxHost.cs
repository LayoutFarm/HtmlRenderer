//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm;
using LayoutFarm.ContentManagers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;

using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.HtmlBoxes;

namespace LayoutFarm.CustomWidgets
{
    /// <summary>
    /// common host for light htmlbox
    /// </summary>
    public class LightHtmlBoxHost
    {
        HtmlIslandHost islandHost;

        RootGraphic rootgfx;
        GraphicsPlatform gfxPlatform;
     
        LayoutFarm.Composers.RenderTreeBuilder renderTreeBuilder;
        Queue<HtmlInputEventAdapter> inputEventAdapterStock = new Queue<HtmlInputEventAdapter>();
        Queue<LayoutFarm.HtmlBoxes.LayoutVisitor> htmlLayoutVisitorStock = new Queue<LayoutVisitor>();

         
        static LightHtmlBoxHost()
        {
            //TODO: revise this again
            LayoutFarm.Composers.BoxCreator.RegisterCustomCssBoxGenerator(
               new  MyCssBoxGenerator());
        }

        public LightHtmlBoxHost(HtmlIslandHost islandHost, GraphicsPlatform gfxPlatform)
        {
            this.gfxPlatform = gfxPlatform;
            this.islandHost = islandHost; 
        }
        public void SetRootGraphic(RootGraphic rootgfx)
        {
            this.rootgfx = rootgfx;
        }
        internal LayoutFarm.HtmlBoxes.LayoutVisitor GetSharedHtmlLayoutVisitor(HtmlIsland island)
        {
            LayoutFarm.HtmlBoxes.LayoutVisitor lay = null;
            if (htmlLayoutVisitorStock.Count == 0)
            {
                lay = new LayoutVisitor(this.gfxPlatform);
            }
            else
            {
                lay = this.htmlLayoutVisitorStock.Dequeue();
            }
            lay.Bind(island);
            return lay;
        }
        internal void ReleaseHtmlLayoutVisitor(LayoutFarm.HtmlBoxes.LayoutVisitor lay)
        {
            lay.UnBind();
            this.htmlLayoutVisitorStock.Enqueue(lay);
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
                adapter = this.inputEventAdapterStock.Dequeue();
            }
            adapter.Bind(island);
            return adapter;
        }
        internal void ReleaseSharedInputEventAdapter(HtmlInputEventAdapter adapter)
        {
            adapter.Unbind();
            this.inputEventAdapterStock.Enqueue(adapter);
        }


        //internal void ChildRequestImage(LightHtmlBox lightBox, ImageRequestEventArgs imgReqArgs)
        //{
        //    if (this.RequestImage != null)
        //    {
        //        this.RequestImage(lightBox, imgReqArgs);
        //    }
        //}
        //internal void ChildRequestStylesheet(LightHtmlBox lightBox, TextLoadRequestEventArgs textReqArgs)
        //{
        //    if (this.RequestStylesheet != null)
        //    {
        //        this.RequestStylesheet(lightBox, textReqArgs);
        //    }
        //}
        public LightHtmlBox CreateLightBox(int w, int h)
        {
            LightHtmlBox lightBox = new LightHtmlBox(this, w, h);
            return lightBox;
        }

        //----------------------------------------------------
        void CreateRenderTreeBuidler()
        {
            this.renderTreeBuilder = new LayoutFarm.Composers.RenderTreeBuilder(rootgfx);
            this.renderTreeBuilder.RequestStyleSheet += (e) =>
            {
                var req = new TextLoadRequestEventArgs(e.Src);
                islandHost.RequestStyleSheet(req);
                e.SetStyleSheet = req.SetStyleSheet;

            };
        }
        public void CreateHtmlFragment(string htmlFragment, RenderElement container, out MyHtmlIsland newIsland, out CssBox newCssBox)
        {
            //1. builder
            if (this.renderTreeBuilder == null) CreateRenderTreeBuidler();

            //-------------------------------------------------------------------
            //2. parse
            //parse fragment to dom
            //temp fix
            //TODO: improve technique here
            htmlFragment = "<html><head></head><body>" + htmlFragment + "</body></html>";
            var htmldoc = LayoutFarm.Composers.WebDocumentParser.ParseDocument(
                           new LayoutFarm.WebDom.Parser.TextSnapshot(htmlFragment.ToCharArray()));


            //3. generate render tree
            ////build rootbox from htmldoc
            var rootElement = renderTreeBuilder.BuildCssRenderTree(htmldoc,
                rootgfx.SampleIFonts,
                islandHost.BaseStylesheet,
                container);
            //4. create small island

            var htmlIsland = new MyHtmlIsland(islandHost);
            htmlIsland.RootCssBox = rootElement;
            htmlIsland.Document = htmldoc;
            htmlIsland.SetMaxSize(container.Width, 0);

            var lay = this.GetSharedHtmlLayoutVisitor(htmlIsland);
            htmlIsland.PerformLayout(lay);
            this.ReleaseHtmlLayoutVisitor(lay);

            newIsland = htmlIsland;
            newCssBox = rootElement;

        }
        public void CreateHtmlFragment(LayoutFarm.Composers.HtmlDocument htmldoc,
            RenderElement container,
            out MyHtmlIsland newIsland,
            out CssBox newCssBox)
        {
            //1. builder 
            if (this.renderTreeBuilder == null) CreateRenderTreeBuidler();
            //-------------------------------------------------------------------
            //2. skip parse

            //3. generate render tree
            ////build rootbox from htmldoc
            var rootElement = renderTreeBuilder.BuildCssRenderTree(htmldoc,
                rootgfx.SampleIFonts,
                this.islandHost.BaseStylesheet,
                container);
            //4. create small island

            var htmlIsland = new MyHtmlIsland(this.islandHost);

            htmlIsland.Document = htmldoc;
            htmlIsland.RootCssBox = rootElement;
            htmlIsland.SetMaxSize(container.Width, 0);

            var lay = this.GetSharedHtmlLayoutVisitor(htmlIsland);
            htmlIsland.PerformLayout(lay);
            this.ReleaseHtmlLayoutVisitor(lay);


            newIsland = htmlIsland;
            newCssBox = rootElement;

        }

        internal void RefreshCssTree(LayoutFarm.WebDom.WebDocument webdoc)
        {
            if (this.renderTreeBuilder == null) CreateRenderTreeBuidler();
            renderTreeBuilder.RefreshCssTree(webdoc);
             
        }
    }
}