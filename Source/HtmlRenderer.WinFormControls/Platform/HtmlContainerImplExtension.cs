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
    static class HtmlContainerImplExtension
    {
        public static void SetHtml(this HtmlContainerImpl container, string html, CssActiveSheet cssData)
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

            using (var img = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(img))
            {
                WinGraphics winGfx = new WinGraphics(g, false);

                //build rootbox from htmldoc
                var rootBox = builder.BuildCssTree(htmldoc, winGfx, container, cssData);
                HtmlContainerImpl containerImp = container as HtmlContainerImpl;
                if (containerImp != null)
                {
                    containerImp.SetHtmlDoc(htmldoc);
                    containerImp.SetRootCssBox(rootBox, cssData);
                }

            }
        }
        public static void SetHtml(this HtmlContainerImpl container, HtmlRenderer.WebDom.HtmlDocument doc, CssActiveSheet cssData)
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


            using (var img = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(img))
            {
                WinGraphics winGfx = new WinGraphics(g, false);
                var rootBox = builder.BuildCssTree(doc, winGfx, container, cssData);
                container.SetHtmlDoc(doc);
                container.SetRootCssBox(rootBox, cssData);
            }
        }
        public static void RefreshHtmlDomChange(this HtmlContainerImpl container,
            HtmlRenderer.WebDom.HtmlDocument doc,
            CssActiveSheet cssData)
        {

            PartialRebuildCssTree(container, doc, cssData);
            //FullRebuildCssTree(container, doc, cssData);
        }
        static void FullRebuildCssTree(HtmlContainerImpl container,
            HtmlRenderer.WebDom.HtmlDocument doc,
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
            using (var img = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(img))
            {
                WinGraphics winGfx = new WinGraphics(g, false);
                var rootBox = builder.BuildCssTree(doc, winGfx, container, cssData);

                container.SetHtmlDoc(doc);
                container.SetRootCssBox(rootBox, cssData);
            }
        }
        static void PartialRebuildCssTree(HtmlContainerImpl container,
            HtmlRenderer.WebDom.HtmlDocument doc,
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
            using (var img = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(img))
            {
                WinGraphics winGfx = new WinGraphics(g, false);
                var rootBox = builder.BuildCssTree(doc, winGfx, container, cssData);

                container.SetHtmlDoc(doc);
                container.SetRootCssBox(rootBox, cssData);
            }
        }
    }

}