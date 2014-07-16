//BSD 2014, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using HtmlRenderer.Diagnostics;
using HtmlRenderer.Drawing;
using HtmlRenderer.WebDom;
using HtmlRenderer.Boxes;

namespace HtmlRenderer.Composers
{

    public class BridgeHtmlDocument : HtmlDocument
    {
        HtmlElement rootNode;
        public BridgeHtmlDocument()
            : base(HtmlPredefineNames.CreateUniqueStringTableClone())
        {
            rootNode = new BrigeRootElement(this);
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
        public override HtmlTextNode CreateTextNode(char[] strBufferForElement)
        {
            return new BridgeHtmlTextNode(this, strBufferForElement);
        }
    }

    enum WellknownElementName : byte
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

    class BridgeHtmlElement : HtmlElement
    {
        CssBox principalBox;
        Css.BoxSpec boxSpec;

        bool hasMyOwnCssBoxFactory;

        public BridgeHtmlElement(BridgeHtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            this.boxSpec = new Css.BoxSpec();
        }
        public Css.BoxSpec Spec
        {
            get { return this.boxSpec; }
        }
        public WellknownElementName WellknownElementName { get; set; }
        public bool TryGetAttribute(WellknownHtmlName wellknownHtmlName, out HtmlAttribute result)
        {
            var found = base.FindAttribute((int)wellknownHtmlName);
            if (found != null)
            {
                result = found;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        public bool TryGetAttribute(WellknownHtmlName wellknownHtmlName, out string value)
        {
            HtmlAttribute found;
            if (this.TryGetAttribute(wellknownHtmlName, out found))
            {
                value = found.Value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }

        }

        //------------------------------------
        protected CssBox GetPrincipalBox()
        {
            return this.principalBox;
        }

        internal void SetPrincipalBox(CssBox box)
        {
            this.principalBox = box;
        }
        internal static CssBox InternalGetPrincipalBox(BridgeHtmlElement element)
        {
            return element.principalBox;
        }

        //------------------------------------
    }

    sealed class BrigeRootElement : BridgeHtmlElement
    {
        public BrigeRootElement(BridgeHtmlDocument ownerDoc)
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


    static class HtmlPredefineNames
    {

        static readonly ValueMap<WellknownHtmlName> _wellKnownHtmlNameMap =
            new ValueMap<WellknownHtmlName>();

        static UniqueStringTable htmlUniqueStringTableTemplate = new UniqueStringTable();

        static HtmlPredefineNames()
        {
            int j = _wellKnownHtmlNameMap.Count;
            for (int i = 0; i < j; ++i)
            {
                htmlUniqueStringTableTemplate.AddStringIfNotExist(_wellKnownHtmlNameMap.GetStringFromValue((WellknownHtmlName)(i + 1)));
            }
        }
        public static UniqueStringTable CreateUniqueStringTableClone()
        {
            return htmlUniqueStringTableTemplate.Clone();
        }

    }

}