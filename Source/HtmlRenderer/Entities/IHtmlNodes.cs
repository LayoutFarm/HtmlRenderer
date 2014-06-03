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
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    public enum WellknownHtmlTagName : byte
    {
        NotAssign,

        Unknown,
        [CssName("html")]        
        HTML,
        [CssName("a")]
        A,
        [CssName("hr")]
        HR,
        [CssName("br")]
        BR,
        [CssName("style")]
        STYLE,
        [CssName("script")]
        SCRIPT,
        [CssName("img")]
        IMG,
        [CssName("div")]
        DIV,
        [CssName("span")]
        SPAN,
        [CssName("link")]
        LINK,
        [CssName("p")]
        P,
        [CssName("table")]
        TABLE,
        [CssName("td")]
        TD,
        [CssName("tr")]
        TR,
        [CssName("tbody")]
        TBody,
        [CssName("thead")]
        THead,
        [CssName("th")]
        TH,
        [CssName("tfoot")]
        TFoot,
        [CssName("iframe")]
        IFREAME,
        [CssName("col")]
        COL,
        [CssName("colgroup")]
        COLGROUP,
        [CssName("font")]
        FONT,
        [CssName("caption")]
        CAPTION,
        [CssName("x")]
        X//test for extension 
    }

    public interface IHtmlAttribute
    {
        string Name { get; }
        string Value { get; }
    }

    public interface IHtmlElement
    {
        WellknownHtmlTagName WellknownTagName { get; }
        /// <summary>
        /// Gets the name of this tag
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets if the tag is single placed; in other words it doesn't need a closing tag; 
        /// e.g. &lt;br&gt;
        /// </summary>
        bool IsSingle { get; }

        /// <summary>
        /// is the html tag has attributes.
        /// </summary>
        /// <returns>true - has attributes, false - otherwise</returns>
        bool HasAttributes();
        /// <summary>
        /// Gets a boolean indicating if the attribute list has the specified attribute
        /// </summary>
        /// <param name="attribute">attribute name to check if exists</param>
        /// <returns>true - attribute exists, false - otherwise</returns>
        bool HasAttribute(string attribute);
        /// <summary>
        /// Get attribute value for given attribute name or null if not exists.
        /// </summary>
        /// <param name="attribute">attribute name to get by</param>
        /// <param name="defaultValue">optional: value to return if attribute is not specified</param>
        /// <returns>attribute value or null if not found</returns>
        string TryGetAttribute(string attribute, string defaultValue = null);
        IEnumerable<IHtmlAttribute> GetAttributeIter();
    }


    sealed class HtmlTagBridge : IHtmlElement
    {
        readonly HtmlRenderer.WebDom.HtmlElement elem;
        public HtmlTagBridge(HtmlRenderer.WebDom.HtmlElement elem)
        {
            this.elem = elem;
            this.WellknownTagName = CssBoxUserUtilExtension.EvaluateTagName(elem.LocalName);
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
        public bool IsSingle { get { return HtmlUtils.IsSingleTag(Name); } }
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
            public string Value
            {
                get { return this.attr.Value; }
            }
        }
    }


}