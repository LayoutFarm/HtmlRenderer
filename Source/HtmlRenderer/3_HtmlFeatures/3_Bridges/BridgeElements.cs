//BSD 2014, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Dom
{


    enum BridgeNodeType
    {
        Element,
        Root,
        Text
    }
    class BridgeHtmlNode
    {
        BridgeNodeType nodeType;
        public BridgeHtmlNode(BridgeNodeType nodeType)
        {
            this.nodeType = nodeType;
        }
        public bool IsTextNode
        {
            get { return this.nodeType == BridgeNodeType.Text; }
        }
        public BridgeNodeType NodeType
        {
            get { return this.nodeType; }
        }
    }

    class BridgeHtmlElement : BridgeHtmlNode, IHtmlElement
    {
        BridgeHtmlElement parentElement;
        BoxSpec boxSpec;
        readonly HtmlElement elem;
        List<BridgeHtmlNode> children;
        public BridgeHtmlElement(HtmlElement elem, WellknownHtmlTagName wellKnownTagName)
            : base(BridgeNodeType.Element)
        {
            this.elem = elem;
            this.children = new List<BridgeHtmlNode>();
            this.WellknownTagName = wellKnownTagName;
            this.Spec = new BoxSpec();
        }
        public IEnumerable<IHtmlAttribute> GetAttributeIter()
        {
            foreach (WebDom.HtmlAttribute attr in this.elem.GetAttributeIterForward())
            {
                yield return new HtmlAttributeBridge(attr);
            }
        }

        //-------------
        public WellknownHtmlTagName WellknownTagName
        {
            get;
            private set;
        }
        public BoxSpec Spec
        {
            get { return this.boxSpec; }
            private set
            {
                this.boxSpec = value;
            }
        }
        public void AddChildElement(BridgeHtmlNode child)
        {
            this.children.Add(child);
            switch (child.NodeType)
            {
                case BridgeNodeType.Element:
                    {
                        ((BridgeHtmlElement)child).parentElement = this;
                    } break;
            }

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
        public string GetAttributeValue(string attrName, string defaultValue)
        {
            var attr = elem.FindAttribute(attrName);
            if (attr == null)
            {
                return defaultValue;
            }
            else
            {
                return attr.Value;
            }
        }
        public bool TryGetAttribute2(string attrName, out string value)
        {
            var attr = elem.FindAttribute(attrName);
            if (attr == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = attr.Value;
                return true;
            }
        }
        public BridgeHtmlElement Parent
        {
            get { return this.parentElement; }
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
            //TODO: indicate that it use default value or not 


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
#if DEBUG
        public override string ToString()
        {
            return this.WellknownTagName.ToString() + " " + ChildCount;
        }
#endif



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
    sealed class BrigeRootElement : BridgeHtmlElement
    {
        public BrigeRootElement(HtmlRootNode htmlRootNode)
            : base(htmlRootNode, WellknownHtmlTagName.Unknown)
        {

        }
    }

    class BridgeHtmlTextNode : BridgeHtmlNode
    {
        //---------------------------------
        //this node may be simple text node

        ContentRuns content;
        public BridgeHtmlTextNode(ContentRuns content)
            : base(BridgeNodeType.Text)
        {
            this.content = content;
        }

        public char[] CopyTextBuffer()
        {
            return content.GetOriginalBuffer();
        }
         
        internal ContentRuns GetContentRuns()
        {
            return this.content;
        }
#if DEBUG
        public override string ToString()
        {
            return new string(this.content.GetOriginalBuffer());
        }
#endif
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