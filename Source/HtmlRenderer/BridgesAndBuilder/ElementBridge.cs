//BSD 2014, WinterCore 
//ArthurHub

using System.Collections.Generic;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Dom
{

    sealed class ElementBridge : IHtmlElement
    {
        readonly HtmlElement elem;
        public ElementBridge(HtmlElement elem)
        {
            this.elem = elem;
            this.WellknownTagName = UserMapUtil.EvaluateTagName(elem.LocalName);
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

        public bool HasAttributes() { return this.elem.AttributeCount > 0; }
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


        public string ClassName { get; set; }
        public string Id { get; set; }
        public string Style { get; set; }

        //----------------------------------------------------------
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

}