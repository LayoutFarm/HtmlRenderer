//BSD 2014, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using HtmlRenderer.Diagnostics;
using HtmlRenderer.Drawing;
using HtmlRenderer.WebDom;
using HtmlRenderer.Boxes;

using HtmlRenderer.Composers.BridgeHtml;

namespace HtmlRenderer.Composers
{

    public class BridgeHtmlDocument : WebDocument
    {
        DomElement rootNode;
        public BridgeHtmlDocument()
            : base(HtmlPredefineNames.CreateUniqueStringTableClone())
        {
            //default root
            rootNode = new RootElement(this);
        }
        public override DomElement RootNode
        {
            get
            {
                return rootNode;
            }
        }
        public override DomElement CreateElement(string prefix, string localName)
        {
            return new HtmlElement(this,
                AddStringIfNotExists(prefix),
                AddStringIfNotExists(localName));
        }
        public DomAttribute CreateAttribute(WellknownElementName attrName)
        {
            return new DomAttribute(this,
                0,
                (int)attrName);
        }
        public override DomTextNode CreateTextNode(char[] strBufferForElement)
        {
            return new HtmlTextNode(this, strBufferForElement);
        }


        internal ActiveCssTemplate ActiveCssTemplate
        {
            get;
            set;
        }
    }

}