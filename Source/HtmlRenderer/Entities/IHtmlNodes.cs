//BSD 2014, WinterCore

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
        [Map("html")]
        HTML,
        [Map("a")]
        A,
        [Map("area")]
        AREA,
        [Map("hr")]
        HR,
        [Map("br")]
        BR,
        [Map("style")]
        STYLE,
        [Map("script")]
        SCRIPT,
        [Map("img")]
        IMG,
        [Map("input")]
        INPUT,


        [Map("isindex")]
        ISINDEX,

        [Map("div")]
        DIV,

        [Map("span")]
        SPAN,
        [Map("link")]
        LINK,
        [Map("p")]
        P,
        [Map("table")]
        TABLE,
        [Map("td")]
        TD,
        [Map("tr")]
        TR,
        [Map("tbody")]
        TBody,
        [Map("thead")]
        THead,
        [Map("th")]
        TH,
        [Map("tfoot")]
        TFoot,
        [Map("iframe")]
        IFREAME,
        [Map("frame")]
        FRAME,
        [Map("col")]
        COL,
        [Map("colgroup")]
        COLGROUP,
        [Map("font")]
        FONT,
        [Map("caption")]
        CAPTION,

        [Map("base")]
        BASE,
        [Map("basefont")]
        BASEFONT,
        [Map("meta")]
        META,
        [Map("param")]
        PARAM,
        [Map("x")]
        X//test for extension 
    }

    public interface IHtmlAttribute
    {
        string Name { get; }
        string Value { get; }
        int LocalNameIndex { get; }
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
        public bool IsSingle
        {
            get
            {
                throw new System.NotSupportedException();
                return false;
            }
            //get { return HtmlUtils.IsSingleTag(Name); }
        }
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