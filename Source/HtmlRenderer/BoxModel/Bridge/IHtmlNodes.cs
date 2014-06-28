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
        NotAssign, //extension , for anonymous element
        Unknown,

        [Map("html")]
        html,
        [Map("a")]
        a,
        [Map("area")]
        area,
        [Map("hr")]
        hr,
        [Map("br")]
        br,
        [Map("style")]
        style,
        [Map("script")]
        script,
        [Map("img")]
        img,
        [Map("input")]
        input,

        [Map("div")]
        div, 
        [Map("span")]
        span,

        [Map("link")]
        link,
        [Map("p")]
        p,

        //----------------------------------
        [Map("table")]
        table,

        [Map("tr")]
        tr,//table-row

        [Map("tbody")]
        tbody,//table-row-group

        [Map("thead")]
        thead, //table-header-group
        //from css2.1 spec:
        //thead: like 'table-row-group' ,but for visual formatting.
        //the row group is always displayed before all other rows and row groups and
        //after any top captions...

        [Map("tfoot")]
        tfoot, //table-footer-group
        //css2.1: like 'table-row-group',but for visual formatting

        [Map("col")]
        col,//table-column, specifics that an element describes a column of cells

        [Map("colgroup")]
        colgroup,//table-column-group, specific that an element groups one or more columns;

        [Map("td")]
        td,//table-cell                
        [Map("th")]
        th,//table-cell

        [Map("caption")]
        caption,//table-caption element
        //----------------------------------------


        [Map("iframe")]
        iframe,


        //----------------------------------------
        [FeatureDeprecated("not support in Html5")]
        [Map("frame")]
        frame,
        [FeatureDeprecated("not support in Html5,Use Css instead")]
        [Map("font")]
        font,
        [FeatureDeprecated("not support in Html5,Use Css instead")]
        [Map("basefont")]
        basefont,


        [Map("base")]
        _base, 

        [Map("meta")]
        meta,
        [Map("param")]
        _param, 

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




        string Id { get; }
        string ClassName { get; }
        string Style { get; }

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