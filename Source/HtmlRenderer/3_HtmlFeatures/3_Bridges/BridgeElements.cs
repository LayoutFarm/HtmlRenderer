//BSD 2014, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Dom
{
    public enum WellknownElementName : byte
    {
        NotAssign, //extension , for anonymous element
        Unknown,
        //---------------- 
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
    public class BridgeHtmlElement : HtmlElement
    {
        BoxSpec boxSpec;
        public BridgeHtmlElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            this.boxSpec = new BoxSpec();
        }
        public BoxSpec Spec
        {
            get { return this.boxSpec; }

        }
        public string GetAttributeValue(string attrName, string defaultValue)
        {
            var attr = base.FindAttribute(attrName);
            if (attr == null)
            {
                return defaultValue;
            }
            else
            {
                return attr.Value;
            }
        }
        public WellknownElementName WellknownElementName { get; set; }
    }
     
    sealed class BrigeRootElement : BridgeHtmlElement
    {
        public BrigeRootElement(HtmlDocument ownerDoc)
            : base(ownerDoc, 0, 0)
        {
        }
    }

    class BridgeHtmlTextNode : HtmlTextNode
    {
        //---------------------------------
        //this node may be simple text node  
        bool hasSomeChar;
        List<CssRun> runs;
        public BridgeHtmlTextNode(HtmlDocument ownerDoc, char[] buffer)
            : base(ownerDoc, buffer)
        {
        }
        public bool IsWhiteSpace
        {
            get
            {
                return !this.hasSomeChar;
            }
        }
        internal void SetSplitParts(List<CssRun> runs, bool hasSomeChar)
        {
            this.runs = runs;
            this.hasSomeChar = hasSomeChar;
        }

#if DEBUG
        public override string ToString()
        {
            return new string(base.GetOriginalBuffer());
        }
#endif

        internal List<CssRun> InternalGetRuns()
        {
            return this.runs;
        }

    }
     
    enum TextSplitPartKind : byte
    {
        Text = 1,
        Whitespace,
        SingleWhitespace,
        LineBreak,
    } 
}