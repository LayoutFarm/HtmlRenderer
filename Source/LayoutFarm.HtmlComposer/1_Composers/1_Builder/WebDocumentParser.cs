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
    public abstract class ExternalHtmlTreeWalker
    {
        public abstract IEnumerable<ExternalHtmlNode> GetHtmlNodeIter();
    }
    public enum ExternalHtmlNodeKind
    {
        Element,
        Document,
        TextNode,

        Attribute,

        EnterChildContext,//special
        ExitChildContext,//special
    }
    public abstract class ExternalHtmlNode
    {
        public abstract object ActualHtmlNode { get; }
        public abstract string HtmlElementName { get; }
        public abstract ExternalHtmlNodeKind HtmlNodeKind { get; }
        public abstract string CurrentTextNodeContent { get; }
        public abstract int Level { get; }
        public abstract void GetAttributeNameAndValue(out string name, out string value);
    }


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
            HtmlDocument newdoc = new HtmlDocument(htmlHost);
            parser.Parse(snapSource, newdoc, newdoc.RootNode);
            FreeHtmlParser(parser);
            return newdoc;
        }
        public static HtmlDocument ParseDocument(LayoutFarm.HtmlBoxes.HtmlHost htmlHost, ExternalHtmlTreeWalker externalTreeWalker)
        {
            HtmlDocument newdoc = new HtmlDocument(htmlHost);
            //start from 
            HtmlElement domElem = (HtmlElement)newdoc.RootNode;
            Stack<HtmlElement> elemStack = new Stack<HtmlElement>();
            HtmlElement newDomElem = null;
            foreach (ExternalHtmlNode node in externalTreeWalker.GetHtmlNodeIter())
            {
                switch (node.HtmlNodeKind)
                {
                    case ExternalHtmlNodeKind.EnterChildContext:
                        {
                            elemStack.Push(domElem);
                            if (newDomElem != null)
                            {
                                domElem = newDomElem;
                            }
                        }
                        break;
                    case ExternalHtmlNodeKind.ExitChildContext:
                        {
                            domElem = elemStack.Pop();
                        }
                        break;
                    case ExternalHtmlNodeKind.Attribute:
                        {
                            node.GetAttributeNameAndValue(out string attrName, out string attrValue);
                            DomAttribute attr = newdoc.CreateAttribute(attrName, attrValue);
                            newDomElem.SetAttribute(attr);
                        }
                        break;
                    case ExternalHtmlNodeKind.Element:

                        newDomElem = (HtmlElement)newdoc.CreateElement(node.HtmlElementName);
                        domElem.AddChild(newDomElem);

                        //System.Diagnostics.Debug.WriteLine(new string(' ', node.Level) + node.HtmlElementName);
                        break;
                    case ExternalHtmlNodeKind.TextNode:
                        DomTextNode textnode = newdoc.CreateTextNode(node.CurrentTextNodeContent.ToCharArray());
                        domElem.AddChild(textnode);
                        //System.Diagnostics.Debug.WriteLine(new string(' ', node.Level) + node.CurrentTextNodeContent);
                        break;
                    case ExternalHtmlNodeKind.Document:
                        //System.Diagnostics.Debug.WriteLine("Root");
                        break;
                }

            }
            return newdoc;
        }
        public static void ParseHtmlDom(TextSource snapSource, IHtmlDocument htmldoc, WebDom.DomElement parentElement)
        {
            HtmlParser parser = GetHtmlParser();
            //------------------------ 
            parser.Parse(snapSource, (LayoutFarm.WebDom.Impl.HtmlDocument)htmldoc, parentElement);
            FreeHtmlParser(parser);
        }

        static Queue<HtmlParser> s_sharedParsers = new Queue<HtmlParser>();
        static object s_sharedParserLock1 = new object();
        static HtmlParser GetHtmlParser()
        {
            lock (s_sharedParserLock1)
            {
                if (s_sharedParsers.Count == 0)
                {
                    return HtmlParser.CreateHtmlParser(ParseEngineKind.HtmlKitParser);
                }
                else
                {
                    return s_sharedParsers.Dequeue();
                }
            }
        }
        static void FreeHtmlParser(HtmlParser parser)
        {
            parser.ResetParser();
            lock (s_sharedParserLock1)
            {
                s_sharedParsers.Enqueue(parser);
            }
        }
    }
}