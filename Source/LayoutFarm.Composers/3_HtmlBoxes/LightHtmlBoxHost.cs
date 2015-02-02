// 2015,2014 ,Apache2, WinterDev
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



        public LightHtmlBoxHost(HtmlIslandHost islandHost, GraphicsPlatform gfxPlatform)
        {
            this.gfxPlatform = gfxPlatform;
            this.islandHost = islandHost;
        }

        public RootGraphic RootGfx
        {
            get { return this.rootgfx; }
            set
            {
                this.rootgfx = value;
            }
        }
        public LayoutFarm.HtmlBoxes.LayoutVisitor GetSharedHtmlLayoutVisitor(HtmlIsland island)
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
        public void ReleaseHtmlLayoutVisitor(LayoutFarm.HtmlBoxes.LayoutVisitor lay)
        {
            lay.UnBind();
            this.htmlLayoutVisitorStock.Enqueue(lay);
        }
        public HtmlInputEventAdapter GetSharedInputEventAdapter(HtmlIsland island)
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
        public void ReleaseSharedInputEventAdapter(HtmlInputEventAdapter adapter)
        {
            adapter.Unbind();
            this.inputEventAdapterStock.Enqueue(adapter);
        }
        //----------------------------------------------------
        void CreateRenderTreeBuilder()
        {
            this.renderTreeBuilder = new LayoutFarm.Composers.RenderTreeBuilder(rootgfx);
            this.renderTreeBuilder.RequestStyleSheet += (e) =>
            {
                var req = new TextLoadRequestEventArgs(e.Src);
                islandHost.RequestStyleSheet(req);
                e.SetStyleSheet = req.SetStyleSheet;

            };
        }
        public MyHtmlIsland CreateHtmlIsland(string htmlFragment, HtmlFragmentRenderBox container)
        {

            return CreateHtmlIsland(
                WebDocumentParser.ParseDocument(
                    new LayoutFarm.WebDom.Parser.TextSnapshot(htmlFragment.ToCharArray())),
                container);
        }
        public MyHtmlIsland CreateHtmlIsland(HtmlDocument htmldoc,
            HtmlFragmentRenderBox container)
        {

            //1. builder 
            if (this.renderTreeBuilder == null) CreateRenderTreeBuilder();
            //-------------------------------------------------------------------


            //2. generate render tree
            ////build rootbox from htmldoc

            var rootElement = renderTreeBuilder.BuildCssRenderTree(htmldoc,
                rootgfx.SampleIFonts,
                this.islandHost.BaseStylesheet,
                container);
            //3. create small island

            var htmlIsland = new MyHtmlIsland(this.islandHost);

            htmlIsland.RootElement = htmldoc.RootNode;
            htmlIsland.RootCssBox = rootElement;
            htmlIsland.SetMaxSize(container.Width, 0);

            var lay = this.GetSharedHtmlLayoutVisitor(htmlIsland);
            htmlIsland.PerformLayout(lay);
            this.ReleaseHtmlLayoutVisitor(lay);


            container.SetHtmlIsland(htmlIsland, rootElement);
            return htmlIsland;
        }

        public MyHtmlIsland CreateHtmlIsland(WebDom.DomElement domElement,
           HtmlFragmentRenderBox container)
        {

            //1. builder 
            if (this.renderTreeBuilder == null) CreateRenderTreeBuilder();
            //-------------------------------------------------------------------


            //2. generate render tree
            ////build rootbox from htmldoc

            var rootElement = renderTreeBuilder.BuildCssRenderTree(domElement,
                rootgfx.SampleIFonts, 
                container);
            //3. create small island

            var htmlIsland = new MyHtmlIsland(this.islandHost);

            htmlIsland.RootElement = domElement;
            htmlIsland.RootCssBox = rootElement;
            htmlIsland.SetMaxSize(container.Width, 0);

            var lay = this.GetSharedHtmlLayoutVisitor(htmlIsland);
            htmlIsland.PerformLayout(lay);
            this.ReleaseHtmlLayoutVisitor(lay);


            container.SetHtmlIsland(htmlIsland, rootElement);
            return htmlIsland;
        }

        public void RefreshCssTree(WebDom.DomElement element)
        {
            if (this.renderTreeBuilder == null) CreateRenderTreeBuilder();
            renderTreeBuilder.RefreshCssTree(element);

        }
    }
}