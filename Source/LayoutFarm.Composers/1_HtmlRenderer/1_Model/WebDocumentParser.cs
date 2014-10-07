//BSD 2014, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using LayoutFarm.Drawing;
using System.Collections.Generic;
using HtmlRenderer.Css;
using HtmlRenderer.WebDom;
using HtmlRenderer.WebDom.Parser;
using HtmlRenderer.Boxes;
using HtmlRenderer.Composers.BridgeHtml;

namespace HtmlRenderer.Composers
{

    public static class WebDocumentParser
    {
        /// <summary>
        /// Parses the source html to css boxes tree structure.
        /// </summary>
        /// <param name="source">the html source to parse</param>
        public static WebDocument ParseDocument(TextSnapshot snapSource)
        {
            var parser = new HtmlParser();
            //------------------------
            var blankHtmlDoc = new BridgeHtmlDocument();
            parser.Parse(snapSource, blankHtmlDoc);
            return blankHtmlDoc;
        }
    }

}