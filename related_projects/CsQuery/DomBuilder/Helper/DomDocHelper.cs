using System;
using System.Collections.Generic;

using System.Text;
using System.IO;


using CsQuery.Utility;
using CsQuery.Engine;
using CsQuery.HtmlParser;

namespace CsQuery.Implementation
{

    public static class DomDocHelper
    {
        /// <summary>
        /// Creates a new DomDocument (or derived) object.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML source for the document.
        /// </param>
        /// <param name="encoding">
        /// (optional) the character set encoding.
        /// </param>
        /// <param name="parsingMode">
        /// (optional) the HTML parsing mode.
        /// </param>
        /// <param name="parsingOptions">
        /// (optional) options for controlling the parsing.
        /// </param>
        /// <param name="docType">
        /// The DocType for this document.
        /// </param>
        ///
        /// <returns>
        /// A new IDomDocument object.
        /// </returns>
        public static IDomDocument Create(Stream html,
            Encoding encoding = null,
            HtmlParsingMode parsingMode = HtmlParsingMode.Content,
            HtmlParsingOptions parsingOptions = HtmlParsingOptions.Default,
            DocType docType = DocType.Default)
        {

            return ElementFactory.Create(html, encoding, parsingMode, parsingOptions, docType);
        }
        /// <summary>
        /// Creates a new DomDocument (or derived) object
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML source for the document
        /// </param>
        /// <param name="parsingMode">
        /// (optional) the parsing mode.
        /// </param>
        /// <param name="parsingOptions">
        /// (optional) options for controlling the parsing.
        /// </param>
        /// <param name="docType">
        /// The DocType for this document.
        /// </param>
        ///
        /// <returns>
        /// A new IDomDocument object
        /// </returns>

        public static IDomDocument Create(string html,
            HtmlParsingMode parsingMode = HtmlParsingMode.Auto,
            HtmlParsingOptions parsingOptions = HtmlParsingOptions.Default,
            DocType docType = DocType.Default)
        {

            var encoding = Encoding.UTF8; 
            using (var stream = new MemoryStream(encoding.GetBytes(html)))
            {
                return ElementFactory.Create(stream, encoding, parsingMode, parsingOptions, docType);
            }
        }
        /// <summary>
        /// Creates a new fragment in a given context.
        /// </summary>
        ///
        /// <param name="html">
        /// The elements.
        /// </param>
        /// <param name="context">
        /// (optional) the context. If omitted, will be automatically determined.
        /// </param>
        /// <param name="docType">
        /// (optional) type of the document.
        /// </param>
        ///
        /// <returns>
        /// A new fragment.
        /// </returns>

        public static IDomDocument CreateDocFragment(string html,
           string context = null,
           DocType docType = DocType.Default)
        {
            
            var factory = new ElementFactory();
            factory.FragmentContext = context;
            factory.HtmlParsingMode = HtmlParsingMode.Fragment;
            factory.HtmlParsingOptions = HtmlParsingOptions.AllowSelfClosingTags;
            factory.DocType = docType;

            Encoding encoding = Encoding.UTF8;
            using (var stream = new MemoryStream(encoding.GetBytes(html)))
            {
                return factory.Parse(stream, encoding);
            }
        }
    }
}