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

namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// common host for light htmlbox
    /// </summary>
    public class LightHtmlBoxHost
    {
        HtmlIslandHost islandHost;
        HtmlDocument myDefaultHtmlDoc;
        WebDom.DomElement myHtmlBodyElement;

        public LightHtmlBoxHost(HtmlIslandHost islandHost)
        {
            this.islandHost = islandHost;
            //-----------------------------------------------------------------------------------------------
            this.myDefaultHtmlDoc = new HtmlDocument();
            this.myDefaultHtmlDoc.ActiveCssTemplate = new ActiveCssTemplate(this.islandHost.BaseStylesheet);
            this.myHtmlBodyElement = myDefaultHtmlDoc.CreateElement("body");
            myDefaultHtmlDoc.RootNode.AddChild(myHtmlBodyElement);
        }
        public WebDom.DomElement SharedBodyElement
        {
            get { return this.myHtmlBodyElement; }
        }

        //----------------------------------------------------

        public MyHtmlIsland CreateHtmlIsland(string htmlFragment, HtmlFragmentRenderBox container)
        {
            var newDivHost = this.myDefaultHtmlDoc.CreateElement("div");
            myHtmlBodyElement.AddChild(newDivHost);
            WebDocumentParser.ParseHtmlDom(
                     new LayoutFarm.WebDom.Parser.TextSnapshot(htmlFragment.ToCharArray()),
                     this.myDefaultHtmlDoc,
                     newDivHost);
            return CreateHtmlIsland(newDivHost, container);
        }
        public MyHtmlIsland CreateHtmlIsland(HtmlDocument htmldoc,
            HtmlFragmentRenderBox container)
        {

            //1. builder 
            var renderTreeBuilder = this.islandHost.GetRenderTreeBuilder(container.Root);
            //-------------------------------------------------------------------


            //2. generate render tree
            ////build rootbox from htmldoc

            var rootElement = renderTreeBuilder.BuildCssRenderTree(htmldoc,
                this.islandHost.BaseStylesheet,
                container);
            //3. create small island

            var htmlIsland = new MyHtmlIsland(this.islandHost);

            htmlIsland.RootElement = htmldoc.RootNode;
            htmlIsland.RootCssBox = rootElement;
            htmlIsland.SetMaxSize(container.Width, 0);

            var lay = islandHost.GetSharedHtmlLayoutVisitor(htmlIsland);
            htmlIsland.PerformLayout(lay);
            islandHost.ReleaseHtmlLayoutVisitor(lay);


            container.SetHtmlIsland(htmlIsland, rootElement);
            return htmlIsland;
        }

        public MyHtmlIsland CreateHtmlIsland(WebDom.DomElement domElement,
           HtmlFragmentRenderBox container)
        {

            //1. builder 
            var renderTreeBuilder = this.islandHost.GetRenderTreeBuilder(container.Root);
            //-------------------------------------------------------------------
            //2. generate render tree
            ////build rootbox from htmldoc
            var hostDivElement = myHtmlBodyElement.OwnerDocument.CreateElement("div");
            myHtmlBodyElement.AddChild(hostDivElement);


            var rootElement = renderTreeBuilder.BuildCssRenderTree(
                hostDivElement,
                domElement,
                container);
            //3. create small island

            var htmlIsland = new MyHtmlIsland(this.islandHost);

            htmlIsland.RootElement = domElement;
            htmlIsland.RootCssBox = rootElement;
            htmlIsland.SetMaxSize(container.Width, 0);

            var lay = islandHost.GetSharedHtmlLayoutVisitor(htmlIsland);
            htmlIsland.PerformLayout(lay);
            islandHost.ReleaseHtmlLayoutVisitor(lay);


            container.SetHtmlIsland(htmlIsland, rootElement);
            return htmlIsland;
        }

        
    }
}