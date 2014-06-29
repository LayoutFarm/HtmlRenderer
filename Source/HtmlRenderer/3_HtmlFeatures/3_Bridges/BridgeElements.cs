//BSD 2014, WinterCore 
//ArthurHub

using System;
using System.Collections.Generic;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Dom
{

    

    sealed class BrigeRootElement : BridgeHtmlNode
    {
        HtmlRootNode htmlRootNode;
        public BrigeRootElement(HtmlRootNode htmlRootNode)
        {
            this.htmlRootNode = htmlRootNode;
        }
    }

    class BridgeHtmlNode : IHtmlElement
    {
        readonly HtmlElement elem;
        BridgeHtmlNode parentElement;

        //---------------------------------
        //this node may be simple text node
        HtmlTextNode htmlTextNode;
        //---------------------------------
        List<BridgeHtmlNode> children;
        //--------------------------------- 
        CssBoxTemplate boxSpec;

        bool isTextNode;
        protected BridgeHtmlNode()
        {
            this.children = new List<BridgeHtmlNode>();
        }
        public BridgeHtmlNode(HtmlElement elem, WellknownHtmlTagName wellKnownTagName)
        {
            this.elem = elem;
            this.WellknownTagName = wellKnownTagName;
            this.children = new List<BridgeHtmlNode>();
        }
        public BridgeHtmlNode(HtmlTextNode htmlTextNode)
        {
            this.isTextNode = true;
            this.htmlTextNode = htmlTextNode;
            //no childnode 
        }


        public bool IsTextNode
        {
            get { return this.isTextNode; }
        }
        public void PrepareChildrenNodes()
        {
            this.children = new List<BridgeHtmlNode>();
        }
        public void AddChildElement(BridgeHtmlNode child)
        {
            this.children.Add(child);
            child.parentElement = this;
        }
        public int ChildCount
        {
            get
            {
                if (children != null)
                {
                    return children.Count;
                }
                return 0;
            }
        }
        public BridgeHtmlNode GetNode(int index)
        {
            return this.children[index];
        }
        public WellknownHtmlTagName WellknownTagName
        {
            get;
            private set;
        }
        public HtmlRenderer.WebDom.HtmlElement HtmlElement
        {
            get { return this.elem; }
        }

        public string Name { get { return this.elem.LocalName; } }

        public bool HasAttributes()
        {
            return this.elem.AttributeCount > 0;
        }
        public bool HasAttribute(string attrName)
        {
            return this.elem.FindAttribute(attrName) != null;
        }
        public string TryGetAttribute(string attribute, string defaultValue = null)
        {
            var foundAttr = this.elem.FindAttribute(attribute);
            if (foundAttr != null)
            {
                return foundAttr.Value;
            }
            else
            {
                return defaultValue;
            }
        }

        public char[] CopyTextBuffer()
        {
            return this.htmlTextNode.CopyTextBuffer();
        }
        public override string ToString()
        {
            return string.Format("<{0}>", this.Name);
        }
        public IEnumerable<IHtmlAttribute> GetAttributeIter()
        {
            foreach (WebDom.HtmlAttribute attr in this.elem.GetAttributeIterForward())
            {
                yield return new HtmlAttributeBridge(attr);
            }
        }
        //-------------
        public CssBoxTemplate Spec
        {
            get { return this.boxSpec; }
            set { this.boxSpec = value; }
        }

        public string ClassName
        {
            get
            {
                var class_value = this.elem.FindAttribute("class");
                if (class_value != null)
                {
                    return class_value.Value;
                }
                return null;
            }
        }
        public string Id { get; set; }
        public string Style { get; set; }
    }

    struct HtmlAttributeBridge : IHtmlAttribute
    {
        WebDom.HtmlAttribute attr;
        public HtmlAttributeBridge(WebDom.HtmlAttribute attr)
        {
            this.attr = attr;
        }
        public string Name
        {
            get { return this.attr.LocalName; }
        }
        public int LocalNameIndex
        {
            get
            {
                return this.attr.LocalNameIndex;
            }
        }
        public string Value
        {
            get { return this.attr.Value; }
        }
    }
}