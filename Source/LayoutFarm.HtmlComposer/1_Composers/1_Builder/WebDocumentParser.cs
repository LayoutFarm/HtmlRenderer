//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

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


using System.Collections.Generic;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Parser;
namespace LayoutFarm.Composers
{
    public static class WebDocumentParser
    {
        /// <summary>
        /// Parses the source html to css boxes tree structure.
        /// </summary>
        /// <param name="source">the html source to parse</param>
        public static HtmlDocument ParseDocument(LayoutFarm.HtmlBoxes.HtmlHost htmlHost, TextSource snapSource)
        {
            HtmlParser parser = GetHtmlParser();
            //------------------------
            HtmlDocument blankHtmlDoc = new HtmlDocument(htmlHost);
            parser.Parse(snapSource, blankHtmlDoc, blankHtmlDoc.RootNode);
            FreeHtmlParser(parser);
            return blankHtmlDoc;
        }

        public static void ParseHtmlDom(TextSource snapSource, IHtmlDocument htmldoc, WebDom.DomElement parentElement)
        {
            var parser = GetHtmlParser();
            //------------------------ 
            parser.Parse(snapSource, (LayoutFarm.WebDom.Impl.HtmlDocument)htmldoc, parentElement);
            FreeHtmlParser(parser);
        }
        static Queue<HtmlParser> sharedParsers = new Queue<HtmlParser>();
        static object sharedParserLock1 = new object();
        static HtmlParser GetHtmlParser()
        {
            lock (sharedParserLock1)
            {
                if (sharedParsers.Count == 0)
                {
                    return HtmlParser.CreateHtmlParser(ParseEngineKind.HtmlKitParser);
                }
                else
                {
                    return sharedParsers.Dequeue();
                }
            }
        }
        static void FreeHtmlParser(HtmlParser parser)
        {
            parser.ResetParser();
            lock (sharedParserLock1)
            {
                sharedParsers.Enqueue(parser);
            }
        }
    }
}