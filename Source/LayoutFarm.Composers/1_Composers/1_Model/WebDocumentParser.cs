// 2015,2014 ,BSD, WinterDev
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
using PixelFarm.Drawing;
using System.Collections.Generic;
using LayoutFarm.Css;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Parser;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.InternalHtmlDom;

namespace LayoutFarm.Composers
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
            var blankHtmlDoc = new HtmlDocument();
            parser.Parse(snapSource, blankHtmlDoc);
            return blankHtmlDoc;
        }
    }

}