using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;
using System.Diagnostics;

using HtmlRenderer.Boxes;
using HtmlRenderer.WebDom;
using HtmlRenderer.Drawing;
using HtmlRenderer.ContentManagers;
using HtmlRenderer.Diagnostics;

namespace HtmlRenderer
{
    static class WinHtmlRootVisualBoxExtension
    {
        //public static void SetHtml(this MyHtmlIsland htmlIsland, string html, CssActiveSheet cssData)
        //{
        //    HtmlRenderer.Composers.BoxModelBuilder builder = new Composers.BoxModelBuilder();
        //    builder.RequestStyleSheet += (e) =>
        //    {
        //        var textContentManager = htmlIsland.TextContentMan;
        //        if (textContentManager != null)
        //        {
        //            textContentManager.AddStyleSheetRequest(e);
        //        }
        //    };


        //    var htmldoc = builder.ParseDocument(new WebDom.Parser.TextSnapshot(html.ToCharArray()));


        //    //build rootbox from htmldoc
        //    var rootBox = builder.BuildCssTree(htmldoc,
        //        LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics,
        //        htmlIsland, cssData);

        //    htmlIsland.SetHtmlDoc(htmldoc);
        //    htmlIsland.SetRootCssBox(rootBox, cssData);


        //}
        
        public static void RefreshHtmlDomChange(this MyHtmlIsland htmlIsland,
            HtmlRenderer.WebDom.WebDocument doc, CssActiveSheet cssData)
        {

            PartialRebuildCssTree(htmlIsland, doc);
        }
        static void FullRebuildCssTree(MyHtmlIsland container,
            HtmlRenderer.WebDom.WebDocument doc,
            CssActiveSheet cssData)
        {
            HtmlRenderer.Composers.BoxModelBuilder builder = new Composers.BoxModelBuilder();
            builder.RequestStyleSheet += (e) =>
            {
                //var textContentManager = container.TextContentMan;
                //if (textContentManager != null)
                //{
                //    textContentManager.AddStyleSheetRequest(e);
                //}
            };

            var rootBox = builder.BuildCssTree(doc, LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics, container, cssData);

            container.SetHtmlDoc(doc);
            container.SetRootCssBox(rootBox, cssData);

        }
        static void PartialRebuildCssTree(MyHtmlIsland htmlIsland,
            HtmlRenderer.WebDom.WebDocument doc)
        {
            HtmlRenderer.Composers.BoxModelBuilder builder = new Composers.BoxModelBuilder();
            builder.RequestStyleSheet += (e) =>
            {
                //var textContentManager = htmlIsland.TextContentMan;
                //if (textContentManager != null)
                //{
                //    textContentManager.AddStyleSheetRequest(e);
                //}
            };

            var rootBox = builder.RefreshCssTree(doc, LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics, htmlIsland);

            //container.SetHtmlDoc(doc);
            //container.SetRootCssBox(rootBox, cssData);

        }
    }

}