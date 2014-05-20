//BSD ,2014, WinterCore

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
using System.Text;
using System.Collections.Generic;
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;

using HtmlRenderer.WebDom;
using HtmlRenderer.WebDom.Parser;

namespace HtmlRenderer.Parse
{

    static class HtmlParser2
    {

        
        /// <summary>
        /// Parses the source html to css boxes tree structure.
        /// </summary>
        /// <param name="source">the html source to parse</param>
        public static CssBox ParseDocument(TextSnapshot snapSource)
        {
           var parser = new HtmlRenderer.WebDom.Parser.HtmlParser();
            
            //------------------------
            parser.Parse(snapSource);
            WebDom.HtmlDocument resultHtmlDoc = parser.ResultHtmlDoc;
            var rootCssBox = CssBox.CreateRootBlock();
            var curBox = rootCssBox;
            //walk on tree and create cssbox
            foreach (HtmlNode node in resultHtmlDoc.RootNode.GetChildNodeIterForward())
            {
                HtmlElement elemNode = node as HtmlElement;
                if (elemNode != null)
                {
                    CreateCssBox(elemNode, curBox);
                }
            }
            return rootCssBox;
        }
        static void CreateCssBox(HtmlElement htmlElement, CssBox parentNode)
        {
            //recursive  
            CssBox box = CssBox.CreateBox(new HtmlTagBridge(htmlElement), parentNode);
            foreach (HtmlNode node in htmlElement.GetChildNodeIterForward())
            {
                switch (node.NodeType)
                {
                    case HtmlNodeType.OpenElement:
                    case HtmlNodeType.ShortElement:
                        {
                            //recursive
                            CreateCssBox((HtmlElement)node, box);

                        } break;
                    case HtmlNodeType.TextNode:
                        {

                            /// Add html text anon box to the current box, this box will have the rendered text<br/>
                            /// Adding box also for text that contains only whitespaces because we don't know yet if
                            /// the box is preformatted. At later stage they will be removed if not relevant.                             
                            HtmlTextNode textNode = (HtmlTextNode)node;
                            CssBox.CreateBox(box).SetTextContent(textNode.CopyTextBuffer());

                        } break;
                    default:
                        {
                        } break;
                }
            }

        }
    }
}