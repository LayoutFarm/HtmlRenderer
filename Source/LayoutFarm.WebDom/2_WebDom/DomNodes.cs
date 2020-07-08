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
        WebDocument _ownerDoc;
        DomElement _parentNode;
        HtmlNodeKind _nodeKind;
#if DEBUG
        static int dbugTotalId;
        public int dbugId;
        public int dbugMark;
#endif

        internal DomNode(WebDocument ownerDoc)
        {
            _ownerDoc = ownerDoc;
#if DEBUG
            this.dbugId = dbugTotalId;
            dbugTotalId++;
#endif
        }
        public DocumentState DocState => _ownerDoc.DocumentState;

        public DomNode ParentNode => _parentNode;

        protected void SetNodeType(HtmlNodeKind nodekind)
        {
            _nodeKind = nodekind;
        }
        //
        public HtmlNodeKind NodeKind => _nodeKind;

        public WebDocument OwnerDocument => _ownerDoc;

        internal void SetParent(DomElement parentNode)
        {
            _parentNode = parentNode;
        }
        public virtual void CopyInnerText(DomTextWriter stbuilder)
        {
        }
    }


    public abstract class DomTextNode : DomNode, ITextNode
    {
        char[] _copyBuffer;
        public DomTextNode(WebDocument ownerDoc, char[] copyBuffer)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeKind.TextNode);
            _copyBuffer = copyBuffer;
        }
        public void AppendTextContent(char[] newCopyBuffer)
        {
            if (_copyBuffer != null)
            {
                char[] newbuffer = new char[_copyBuffer.Length + newCopyBuffer.Length];
                Array.Copy(_copyBuffer, newbuffer, _copyBuffer.Length);
                Array.Copy(newCopyBuffer, 0, newbuffer, _copyBuffer.Length, newCopyBuffer.Length);
                _copyBuffer = newbuffer;
            }
            else
            {
                _copyBuffer = newCopyBuffer;
            }
        }
        public char[] GetOriginalBuffer() => _copyBuffer;


        public override void CopyInnerText(DomTextWriter domTextWriter)
        {
            domTextWriter.Write(_copyBuffer);
        }
#if DEBUG
        public override string ToString()
        {
            if (_copyBuffer != null)
            {
                return "t-node" + string.Empty;
            }
            else
            {
                return "t-node " + new string(_copyBuffer);
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
        public StringBuilder Content { get; set; }
    }
    public class DomDocumentNode : DomNode
    {
        List<string> _docNodeAttrList;
        public DomDocumentNode(WebDocument ownerDoc)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeKind.DocumentNode);
        }
        public string DocNodeName { get; set; }
        public void AddParameter(string nodeParameter)
        {
            if (_docNodeAttrList == null)
            {
                _docNodeAttrList = new List<string>();
            }
            _docNodeAttrList.Add(nodeParameter);
        }
#if DEBUG
        public override string ToString()
        {
            StringBuilder stbuilder = new StringBuilder();
            stbuilder.Append("<!");
            stbuilder.Append(this.DocNodeName);
            if (_docNodeAttrList != null)
            {
                foreach (var str in _docNodeAttrList)
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
        int _procName;
        internal DomProcessInstructionNode(WebDocument ownerDoc, int procName)
            : base(ownerDoc)
        {
            _procName = procName;
            SetNodeType(HtmlNodeKind.ProcessInstruction);
        }
        public StringBuilder Content { get; set; }
    }
    public class DomCDataNode : DomNode
    {
        internal DomCDataNode(WebDocument ownerDoc)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeKind.CData);
        }
        public StringBuilder Content { get; set; }
    }

    /// <summary>
    /// attribute node
    /// </summary>
    public class DomAttribute : DomNode
    {
        internal int _nodePrefixNameIndex;
        internal int _nodeLocalNameIndex;
        string _attrValue;
        public DomAttribute(WebDocument ownerDoc,
            int nodePrefixNameIndex,
            int nodeLocalNameIndex)
            : base(ownerDoc)
        {
            SetNodeType(HtmlNodeKind.Attribute);
            _nodePrefixNameIndex = nodePrefixNameIndex;
            _nodeLocalNameIndex = nodeLocalNameIndex;
        }

        public string Value
        {
            get => _attrValue;
            set => _attrValue = value;
        }

        public string Prefix => OwnerDocument.GetString(_nodePrefixNameIndex);

        public string LocalName => OwnerDocument.GetString(_nodeLocalNameIndex);

        public int LocalNameIndex => _nodeLocalNameIndex;

        public int PrefixNameIndex => _nodePrefixNameIndex;

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_nodePrefixNameIndex > 0)
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

        public string Name => this.LocalName;
    }


    public static class WellKnownDomNodeMap
    {
        static readonly ValueMap<WellKnownDomNodeName> s_wellknownHtmlTagNameMap = new ValueMap<WellKnownDomNodeName>();
        public static WellKnownDomNodeName EvaluateTagName(string name) => s_wellknownHtmlTagNameMap.GetValueFromString(name, WellKnownDomNodeName.Unknown);

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
        [Map("textarea")]
        textarea,
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




        //----------------------------------------
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
        svg_g,
        //----------------------------------------

        [Map("math")]
        math,
    }

}