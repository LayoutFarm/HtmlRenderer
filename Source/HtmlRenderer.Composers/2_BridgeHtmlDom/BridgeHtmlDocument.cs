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

    public class BridgeHtmlDocument : HtmlDocument
    {
        HtmlElement rootNode;
        public BridgeHtmlDocument()
            : base(HtmlPredefineNames.CreateUniqueStringTableClone())
        {
            //default root
            rootNode = new BridgeRootElement(this);
        }
        public override HtmlElement RootNode
        {
            get
            {
                return rootNode;
            }
        }
        public override HtmlElement CreateElement(string prefix, string localName)
        {
            return new BridgeHtmlElement(this,
                AddStringIfNotExists(prefix),
                AddStringIfNotExists(localName));
        }
        public HtmlAttribute CreateAttribute(WellknownHtmlName attrName)
        {
            return new HtmlAttribute(this,
                0,
                (int)attrName);
        }
        public override HtmlTextNode CreateTextNode(char[] strBufferForElement)
        {
            return new BridgeHtmlTextNode(this, strBufferForElement);
        }


        internal ActiveCssTemplate ActiveCssTemplate
        {
            get;
            set;
        }
    }

}