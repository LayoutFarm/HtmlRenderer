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
    public static class WinHtmlRootVisualBoxExtension
    {
        public static void SetHtml(this MyHtmlIslandImpl container, string html, CssActiveSheet cssData)
        {
            HtmlRenderer.Composers.BoxModelBuilder builder = new Composers.BoxModelBuilder();
            builder.RequestStyleSheet += (e) =>
            {
                var textContentManager = container.TextContentMan;
                if (textContentManager != null)
                {
                    textContentManager.AddStyleSheetRequest(e);
                }
            };


            var htmldoc = builder.ParseDocument(new WebDom.Parser.TextSnapshot(html.ToCharArray()));


            //build rootbox from htmldoc
            var rootBox = builder.BuildCssTree(htmldoc,
                LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics,
                container, cssData);
            MyHtmlIslandImpl containerImp = container as MyHtmlIslandImpl;
            if (containerImp != null)
            {
                containerImp.SetHtmlDoc(htmldoc);
                containerImp.SetRootCssBox(rootBox, cssData);
            }


        }
        public static void SetHtml(this MyHtmlIslandImpl container, HtmlRenderer.WebDom.WebDocument doc, CssActiveSheet cssData)
        {
            HtmlRenderer.Composers.BoxModelBuilder builder = new Composers.BoxModelBuilder();
            builder.RequestStyleSheet += (e) =>
            {
                var textContentManager = container.TextContentMan;
                if (textContentManager != null)
                {
                    textContentManager.AddStyleSheetRequest(e);
                }
            };



            var rootBox = builder.BuildCssTree(doc,
                LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics,
                container, cssData);

            container.SetHtmlDoc(doc);
            container.SetRootCssBox(rootBox, cssData);

        }
        public static void RefreshHtmlDomChange(this MyHtmlIslandImpl container,
            HtmlRenderer.WebDom.WebDocument doc, CssActiveSheet cssData)
        {

            PartialRebuildCssTree(container, doc);
        }
        static void FullRebuildCssTree(MyHtmlIslandImpl container,
            HtmlRenderer.WebDom.WebDocument doc,
            CssActiveSheet cssData)
        {
            HtmlRenderer.Composers.BoxModelBuilder builder = new Composers.BoxModelBuilder();
            builder.RequestStyleSheet += (e) =>
            {
                var textContentManager = container.TextContentMan;
                if (textContentManager != null)
                {
                    textContentManager.AddStyleSheetRequest(e);
                }
            };

            var rootBox = builder.BuildCssTree(doc, LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics, container, cssData);

            container.SetHtmlDoc(doc);
            container.SetRootCssBox(rootBox, cssData);

        }
        static void PartialRebuildCssTree(MyHtmlIslandImpl container,
            HtmlRenderer.WebDom.WebDocument doc)
        {
            HtmlRenderer.Composers.BoxModelBuilder builder = new Composers.BoxModelBuilder();
            builder.RequestStyleSheet += (e) =>
            {
                var textContentManager = container.TextContentMan;
                if (textContentManager != null)
                {
                    textContentManager.AddStyleSheetRequest(e);
                }
            };

            var rootBox = builder.RefreshCssTree(doc, LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics, container);

            //container.SetHtmlDoc(doc);
            //container.SetRootCssBox(rootBox, cssData);

        }
    }

}