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

    public static class HtmlIslandHelper
    {
        static MyHtmlIsland CreateHtmlIsland(
         HtmlIslandHost islandHost,
         HtmlDocument htmldoc,
         HtmlFragmentRenderBox container)
        {

            //1. builder 
            var renderTreeBuilder = islandHost.GetRenderTreeBuilder(container.Root);
            //-------------------------------------------------------------------


            //2. generate render tree
            ////build rootbox from htmldoc

            var rootElement = renderTreeBuilder.BuildCssRenderTree(htmldoc,
                islandHost.BaseStylesheet,
                container);
            //3. create small island

            var htmlIsland = new MyHtmlIsland(islandHost);

            htmlIsland.WebDocument = htmldoc;
            htmlIsland.RootCssBox = rootElement;
            htmlIsland.SetMaxSize(container.Width, 0);

            var lay = islandHost.GetSharedHtmlLayoutVisitor(htmlIsland);
            htmlIsland.PerformLayout(lay);
            islandHost.ReleaseHtmlLayoutVisitor(lay);


            container.SetHtmlIsland(htmlIsland, rootElement);
            return htmlIsland;
        }

        public static MyHtmlIsland CreateHtmlIsland(
            HtmlIslandHost islandHost,
            string htmlFragment,
            HtmlFragmentRenderBox container)
        {

            var htmldoc = new HtmlDocument();
            var myHtmlBodyElement = htmldoc.CreateElement("body");
            htmldoc.RootNode.AddChild(myHtmlBodyElement);

            WebDocumentParser.ParseHtmlDom(
                     new LayoutFarm.WebDom.Parser.TextSnapshot(htmlFragment.ToCharArray()),
                     htmldoc,
                     myHtmlBodyElement);

            return CreateHtmlIsland(islandHost, myHtmlBodyElement, container);
        }


        static MyHtmlIsland CreateHtmlIsland(
         HtmlIslandHost islandHost,
         WebDom.DomElement myHtmlBodyElement,
         HtmlFragmentRenderBox container)
        {

            //1. builder 
            var renderTreeBuilder = islandHost.GetRenderTreeBuilder(container.Root);
            //-------------------------------------------------------------------
            //2. generate render tree
            ////build rootbox from htmldoc
            var hostDivElement = myHtmlBodyElement.OwnerDocument.CreateElement("div");
            myHtmlBodyElement.AddChild(hostDivElement);


            var rootElement = renderTreeBuilder.BuildCssRenderTree(
                hostDivElement,
                hostDivElement,
                container);
            //3. create small island

            var htmlIsland = new MyHtmlIsland(islandHost);

            htmlIsland.WebDocument = hostDivElement.OwnerDocument;
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