//BSD, 2014-present, WinterDev 

using System;
using System.Text;
using System.Collections.Generic;
namespace LayoutFarm.WebDom
{
    public enum HtmlNodeKind
    {
        OpenElement,
        CloseElement,
        ShortElement,
        Attribute,
        TextNode,
        CData,
        ProcessInstruction,
        Comment,
        DocumentNode //not root node
    }



    public abstract class DomNode : INode
    {
        WebDocument ownerDoc;
        DomNode parentNode;
        HtmlNodeKind _nodeKind;
#if DEBUG
        static int dbugTotalId;
        public int dbugId;
        public int dbugMark;
#endif

        internal DomNode(WebDocument ownerDoc)
        {
            this.ownerDoc = ownerDoc;
#if DEBUG
            this.dbugId = dbugTotalId;
            dbugTotalId++;
#endif

        }
        public DocumentState DocState
        {
            get
            {
                return this.ownerDoc.DocumentState;
            }
        }
        public DomNode ParentNode
        {
            get
            {
                return this.parentNode;
            }
        }
        protected void SetNodeType(HtmlNodeKind nodekind)
        {
            this._nodeKind = nodekind;
        }
        public HtmlNodeKind NodeKind
        {
            get
            {
                return _nodeKind
                    ;
            }
        }

        public WebDocument OwnerDocument
        {
            get
            {
                return ownerDoc;
            }
        }

        internal void SetParent(DomNode parentNode)
        {
            this.parentNode = parentNode;
        }
    }


    public abstract class DomTextNode : DomNode, ITextNode
    {
        char[] copyBuffer;
        public DomTextNode(WebDocument ownerDoc, char[] copyBuffer)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeKind.TextNode);
            this.copyBuffer = copyBuffer;
        }
        public void AppendTextContent(char[] newCopyBuffer)
        {
            if (copyBuffer != null)
            {
                char[] newbuffer = new char[copyBuffer.Length + newCopyBuffer.Length];
                Array.Copy(copyBuffer, newbuffer, copyBuffer.Length);
                Array.Copy(newCopyBuffer, 0, newbuffer, copyBuffer.Length, newCopyBuffer.Length);
                this.copyBuffer = newbuffer;
            }
            else
            {
                this.copyBuffer = newCopyBuffer;
            }
        }
        public char[] GetOriginalBuffer()
        {
            return copyBuffer;
        }
#if DEBUG
        public override string ToString()
        {
            if (copyBuffer != null)
            {
                return "t-node" + string.Empty;
            }
            else
            {
                return "t-node " + new string(this.copyBuffer);
            }
        }
#endif
    }

    public class DomComment : DomNode
    {
        internal DomComment(WebDocument ownerDoc)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeKind.Comment);
        }
        public StringBuilder Content
        {
            get;
            set;
        }
    }
    public class DomDocumentNode : DomNode
    {
        List<string> docNodeAttrList;
        public DomDocumentNode(WebDocument ownerDoc)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeKind.DocumentNode);
        }
        public string DocNodeName { get; set; }
        public void AddParameter(string nodeParameter)
        {
            if (docNodeAttrList == null)
            {
                docNodeAttrList = new List<string>();
            }
            docNodeAttrList.Add(nodeParameter);
        }
#if DEBUG
        public override string ToString()
        {
            StringBuilder stbuilder = new StringBuilder();
            stbuilder.Append("<!");
            stbuilder.Append(this.DocNodeName);
            if (docNodeAttrList != null)
            {
                foreach (var str in docNodeAttrList)
                {
                    stbuilder.Append(' ');
                    stbuilder.Append(str);
                }
            }
            stbuilder.Append(">");
            return stbuilder.ToString();
        }
#endif
    }
    public class DomProcessInstructionNode : DomNode
    {
        int procName;
        internal DomProcessInstructionNode(WebDocument ownerDoc, int procName)
            : base(ownerDoc)
        {
            this.procName = procName;
            SetNodeType(HtmlNodeKind.ProcessInstruction);
        }
        public StringBuilder Content
        {
            get;
            set;
        }
    }
    public class DomCDataNode : DomNode
    {
        internal DomCDataNode(WebDocument ownerDoc)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeKind.CData);
        }
        public string Content
        {
            get;
            set;
        }
    }

    /// <summary>
    /// attribute node
    /// </summary>
    public class DomAttribute : DomNode
    {
        internal int nodePrefixNameIndex;
        internal int nodeLocalNameIndex;
        string attrValue;
        public DomAttribute(WebDocument ownerDoc,
            int nodePrefixNameIndex,
            int nodeLocalNameIndex)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeKind.Attribute);
            this.nodePrefixNameIndex = nodePrefixNameIndex;
            this.nodeLocalNameIndex = nodeLocalNameIndex;
        }

        public string Value
        {
            get { return this.attrValue; }
            set { this.attrValue = value; }
        }

        public string Prefix
        {
            get
            {
                return OwnerDocument.GetString(this.nodePrefixNameIndex);
            }
        }
        public string LocalName
        {
            get
            {
                return OwnerDocument.GetString(this.nodeLocalNameIndex);
            }
        }
        public int LocalNameIndex
        {
            get
            {
                return this.nodeLocalNameIndex;
            }
        }
        public int PrefixNameIndex
        {
            get
            {
                return this.nodePrefixNameIndex;
            }
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.nodePrefixNameIndex > 0)
            {
                sb.Append(Prefix);
                sb.Append(':');
                sb.Append(LocalName);
            }
            else
            {
                sb.Append(this.LocalName);
            }
            if (this.Value != null)
            {
                sb.Append('=');
                sb.Append(this.Value);
            }

            return sb.ToString();
        }
#endif
        //-------------------------------

        public string Name
        {
            get
            {
                return this.LocalName;
            }
        }
    }

    
    public static class WellKnownDomNodeMap
    {
        static readonly ValueMap<WellKnownDomNodeName> _wellknownHtmlTagNameMap = new ValueMap<WellKnownDomNodeName>();
        public static WellKnownDomNodeName EvaluateTagName(string name)
        {
            return _wellknownHtmlTagNameMap.GetValueFromString(name, WellKnownDomNodeName.Unknown);
        }

    }

    public enum WellKnownDomNodeName : byte
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
        [Map("body")]
        body,
        [Map("style")]
        style,
        [Map("script")]
        script,
        [Map("img")]
        img,
        [Map("input")]
        input,

        [Map("option")]
        option,
        [Map("select")]
        select,

        [Map("canvas")]
        canvas,
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
        [Map("template")]
        template, //html5 template
        [Map("td")]
        td,//table-cell                
        [Map("th")]
        th,//table-cell
        [Map("caption")]
        caption,//table-caption element
        //----------------------------------------


        [Map("iframe")]
        iframe,
        ////----------------------------------------
        //[FeatureDeprecated("not support in Html5")]
        //[Map("frame")]
        //frame,
        //[FeatureDeprecated("not support in Html5,Use Css instead")]
        //[Map("font")]
        //font,
        //[FeatureDeprecated("not support in Html5,Use Css instead")]
        //[Map("basefont")]
        basefont,
        [Map("base")]
        _base,
        [Map("meta")]
        meta,
        [Map("param")]
        _param,
        [Map("svg")]
        svg,
        [Map("rect")]
        svg_rect,
        [Map("circle")]
        svg_circle,
        [Map("ellipse")]
        svg_ellipse,
        [Map("polygon")]
        svg_polygon,
        [Map("polyline")]
        svg_polyline,
        [Map("defs")]
        svg_defs,
        [Map("linearGradient")]
        svg_linearGradient,
        [Map("stop")]
        svg_stop,
        [Map("path")]
        svg_path,
        [Map("image")]
        svg_image,
        [Map("g")]
        svg_g
    }

}