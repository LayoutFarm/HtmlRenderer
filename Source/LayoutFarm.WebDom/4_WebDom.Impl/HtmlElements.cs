//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo 

using System;
using System.Collections.Generic;
using System.Text;

namespace LayoutFarm.WebDom.Impl
{

    //--------
    //copy temp context from pixelfarm
    struct TempContext<T> : IDisposable
    {
        internal readonly T _tool;
        internal TempContext(out T tool)
        {
            Temp<T>.GetFreeItem(out _tool);
            tool = _tool;
        }
        public void Dispose()
        {
            Temp<T>.Release(_tool);
        }
    }

    static class Temp<T>
    {

        public delegate T CreateNewItemDelegate();
        public delegate void ReleaseItemDelegate(T item);


        [System.ThreadStatic]
        static Stack<T> s_pool;
        [System.ThreadStatic]
        static CreateNewItemDelegate s_newHandler;
        [System.ThreadStatic]
        static ReleaseItemDelegate s_releaseCleanUp;

        public static TempContext<T> Borrow(out T freeItem)
        {
            return new TempContext<T>(out freeItem);
        }

        public static void SetNewHandler(CreateNewItemDelegate newHandler, ReleaseItemDelegate releaseCleanUp = null)
        {
            //set new instance here, must set this first***
            if (s_pool == null)
            {
                s_pool = new Stack<T>();
            }
            s_newHandler = newHandler;
            s_releaseCleanUp = releaseCleanUp;
        }
        internal static void GetFreeItem(out T freeItem)
        {
            if (s_pool.Count > 0)
            {
                freeItem = s_pool.Pop();
            }
            else
            {
                freeItem = s_newHandler();
            }
        }
        internal static void Release(T item)
        {
            s_releaseCleanUp?.Invoke(item);
            s_pool.Push(item);
            //... 
        }
        public static bool IsInit()
        {
            return s_pool != null;
        }
    }


    static class Temp<Owner, T>
    {
        public struct TempContext : IDisposable
        {
            internal readonly T _tool;
            internal TempContext(out T tool)
            {
                Temp<Owner, T>.GetFreeItem(out _tool);
                tool = _tool;
            }
            public void Dispose()
            {
                Temp<Owner, T>.Release(_tool);
            }
        }

        public delegate T CreateNewItemDelegate();
        public delegate void ReleaseItemDelegate(T item);


        [System.ThreadStatic]
        static Stack<T> s_pool;
        [System.ThreadStatic]
        static CreateNewItemDelegate s_newHandler;
        [System.ThreadStatic]
        static ReleaseItemDelegate s_releaseCleanUp;

        public static TempContext Borrow(out T freeItem)
        {
            return new TempContext(out freeItem);
        }

        public static void SetNewHandler(CreateNewItemDelegate newHandler, ReleaseItemDelegate releaseCleanUp = null)
        {
            //set new instance here, must set this first***
            if (s_pool == null)
            {
                s_pool = new Stack<T>();
            }
            s_newHandler = newHandler;
            s_releaseCleanUp = releaseCleanUp;
        }
        internal static void GetFreeItem(out T freeItem)
        {
            if (s_pool.Count > 0)
            {
                freeItem = s_pool.Pop();
            }
            else
            {
                freeItem = s_newHandler();
            }
        }
        internal static void Release(T item)
        {
            s_releaseCleanUp?.Invoke(item);
            s_pool.Push(item);
            //... 
        }
        public static bool IsInit()
        {
            return s_pool != null;
        }
    }


    //--------

    static class DomTextWriterPool
    {
        public static TempContext<DomTextWriter> Borrow(out DomTextWriter writer)
        {
            if (!Temp<DomTextWriter>.IsInit())
            {
                Temp<DomTextWriter>.SetNewHandler(() => new DomTextWriter(new StringBuilder()),
                s => s.Clear()
                );
            }
            return Temp<DomTextWriter>.Borrow(out writer);
        }
    }

    public class HtmlNodeList : INodeList
    {
        LinkedList<HtmlElement> _items = new LinkedList<HtmlElement>();
        public void AddSelectedItem(HtmlElement elem)
        {
            _items.AddLast(elem);
        }

        public int Count => _items.Count;

        public IEnumerable<HtmlElement> GetElemIter()
        {
            var node = _items.First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }
    }
    public class QuerySelectorPatterns
    {
        LinkedList<QuerySelectorPattern> _patts = new LinkedList<QuerySelectorPattern>();
        public void AddPattern(QuerySelectorPattern patt) => _patts.AddLast(patt);
        public bool Evaluate(HtmlElement elem)
        {
            var node = _patts.First;
            while (node != null)
            {
                if (node.Value.Test(elem))
                {
                    //test pass
                    return true;
                }
                node = node.Next;
            }
            return false;
        }
    }

    public abstract class QuerySelectorPattern
    {
        public abstract bool Test(HtmlElement elem);
    }


    public sealed class QuerySelectorPatternById : QuerySelectorPattern
    {
        public QuerySelectorPatternById(string id)
        {
            Id = id;
        }
        public string Id { get; private set; }
        public override bool Test(HtmlElement elem)
        {
            //by id***
            return elem.AttrElementId == Id;
        }
    }
    public sealed class QuerySelectorPatternByNodeName : QuerySelectorPattern
    {
        public QuerySelectorPatternByNodeName(string nodeName) => NodeName = nodeName;
        public string NodeName { get; private set; }
        public string ClassName { get; set; }
        public override bool Test(HtmlElement elem)
        {
            //by id***
            if (NodeName != null)
            {
                if (elem.Name == NodeName)
                {
                    if (ClassName != null)
                    {
                        if (elem.HasAttributeClass &&
                            elem.AttrClassValue == ClassName)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else if (ClassName != null && elem.HasAttributeClass &&
                     elem.AttrClassValue == ClassName)
            {
                return true;
            }
            return false;
        }
    }
    public static class QuerySelectorStringParser
    {

        static readonly char[] seps = new char[] { ',' };
        public static QuerySelectorPatterns Parse(string querySelectorStr)
        {
            QuerySelectorPatterns querySelectorPattern = new QuerySelectorPatterns();


            //https://developer.mozilla.org/en-US/docs/Web/API/Document_object_model/Locating_DOM_elements_using_selectors
            //Selectors

            //The selector methods accept one or more comma-separated selectors to determine what element or 
            //elements should be returned. For example, to select all paragraph(p) elements in 
            //a document whose CSS class is either warning or note, you can do the following:

            //var special = document.querySelectorAll("p.warning, p.note");
            //        You can also query by ID.For example:
            //        var el = document.querySelector("#main, #basic, #exclamation");
            //        After executing the above code, el contains the first element in the document whose ID is one of main, basic, or exclamation.
            //        You may use any CSS selectors with the querySelector() and querySelectorAll() methods.

            //TODO: review here again
            string[] patts = querySelectorStr.Split(seps, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < patts.Length; ++i)
            {
                //parse each pattern
                QuerySelectorPattern patt = ParsePattern(patts[i].Trim());
                if (patt != null)
                {
                    querySelectorPattern.AddPattern(patt);
                }
            }
            return querySelectorPattern;
        }
        static QuerySelectorPattern ParsePattern(string patt)
        {
            if (patt.StartsWith("#"))
            {
                //by ID
                return new QuerySelectorPatternById(patt.Substring(1));
            }
            else
            {
                //TODO: implement more selector pattern 
                //https://dom.spec.whatwg.org/#dom-parentnode-queryselectorall

                //by elem name
                int indexOfDot = patt.IndexOf('.');
                if (indexOfDot < 0)
                {
                    return new QuerySelectorPatternByNodeName(patt);
                }
                else if (indexOfDot == 0)
                {
                    //no node name
                    //class name only
                    return new QuerySelectorPatternByNodeName(null)
                    {
                        ClassName = patt
                    };
                }
                else
                {
                    //has some dot                     
                    return new QuerySelectorPatternByNodeName(patt.Substring(indexOfDot))
                    {
                        ClassName = patt.Substring(indexOfDot)
                    };
                }
            }
        }
    }


    public abstract partial class HtmlElement : DomElement
    {
        CssRuleSet _elementRuleSet;
        public HtmlElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {

        }
        public WellKnownDomNodeName WellknownElementName { get; set; }
        public bool TryGetAttribute(WellknownName wellknownHtmlName, out DomAttribute result)
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
        public bool TryGetAttribute(string attrName, out DomAttribute result)
        {
            int foundIndex = this.OwnerDocument.FindStringIndex(attrName);
            if (foundIndex < 1)
            {
                result = null;
                return false;
            }
            return (result = FindAttribute(foundIndex)) != null;
        }
        public bool TryGetAttribute(WellknownName wellknownHtmlName, out string value)
        {
            DomAttribute found;
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


        public static void InvokeNotifyChangeOnIdleState(HtmlElement elem, ElementChangeKind changeKind, DomAttribute attr)
        {
            elem.OnElementChangedInIdleState(changeKind, attr);
        }

        protected override void OnContentUpdate()
        {
            base.OnContentUpdate();
            OnElementChangedInIdleState(ElementChangeKind.ContentUpdate, null);
        }

        protected override void OnElementChangedInIdleState(ElementChangeKind changeKind, DomAttribute attr)
        {
            //1. 
            this.OwnerDocument.SetDocumentState(DocumentState.ChangedAfterIdle);
            if (this.OwnerDocument.IsDocFragment) return;
            HtmlDocument owner = this.OwnerDocument as HtmlDocument;
            owner.IncDomVersion();
        }

        public CssRuleSet ElementRuleSet
        {
            get
            {
                return _elementRuleSet;
            }
            set
            {
                _elementRuleSet = value;
            }
        }


        protected override void OnElementChanged()
        {
        }
        //------------------------------------
        public virtual string GetInnerHtml()
        {
            //get inner html*** 

            using (DomTextWriterPool.Borrow(out DomTextWriter textWriter))
            {
                foreach (var childnode in this.GetChildNodeIterForward())
                {
                    HtmlElement childHtmlNode = childnode as HtmlElement;
                    if (childHtmlNode != null)
                    {
                        childHtmlNode.WriteNode(textWriter);
                    }
                    HtmlTextNode htmlTextNode = childnode as HtmlTextNode;
                    if (htmlTextNode != null)
                    {
                        htmlTextNode.WriteTextNode(textWriter);
                    }
                }
                return textWriter.ToString();
            }
        }

        public HtmlNodeList QuerySelectAll(string pattern)
        {
            QuerySelectorPatterns patts = QuerySelectorStringParser.Parse(pattern);
            if (patts == null) return null;
            return QuerySelectAll(patts);
        }
        public HtmlNodeList QuerySelectAll(QuerySelectorPatterns patts)
        {
            HtmlNodeList nodelist = new HtmlNodeList();
            QuerySelectAll(this, patts, nodelist);
            return nodelist;
        }
        static void QuerySelectAll(HtmlElement elem, QuerySelectorPatterns patts, HtmlNodeList nodelist)
        {
            if (elem.ChildrenCount < 1) return;
            //----------
            foreach (DomNode childnode in elem.GetChildNodeIterForward())
            {
                if (childnode.NodeKind == HtmlNodeKind.OpenElement)
                {
                    HtmlElement htmlElem = (HtmlElement)childnode;
                    if (patts.Evaluate((HtmlElement)childnode))
                    {
                        //found
                        nodelist.AddSelectedItem(htmlElem);
                    }
                    QuerySelectAll(htmlElem, patts, nodelist);
                }
            }
        }
        public HtmlElement QuerySelector(string pattern)
        {
            //TODO: 
            //parse selector pattern 
            QuerySelectorPatterns patts = QuerySelectorStringParser.Parse(pattern);
            if (patts == null) return null;
            return QuerySelector(patts);
        }
        public HtmlElement QuerySelector(QuerySelectorPatterns patts)
        {
            //eval child node
            if (ChildrenCount < 1) return null;
            //----------
            foreach (DomNode childnode in this.GetChildNodeIterForward())
            {
                if (childnode.NodeKind == HtmlNodeKind.OpenElement)
                {
                    HtmlElement htmlElem = (HtmlElement)childnode;
                    if (!patts.Evaluate((HtmlElement)childnode))
                    {
                        //not found
                        HtmlElement found = ((HtmlElement)childnode).QuerySelector(patts);
                        if (found != null) return found;
                    }
                    else
                    {
                        //found
                        return (HtmlElement)childnode;
                    }
                }

            }
            return null;
        }
        public virtual string GetInnerText()
        {

            using (DomTextWriterPool.Borrow(out DomTextWriter textWriter))
            {
                foreach (var childnode in this.GetChildNodeIterForward())
                {
                    HtmlElement childHtmlNode = childnode as HtmlElement;
                    if (childHtmlNode != null)
                    {
                        childHtmlNode.CopyInnerText(textWriter);
                    }
                    DomTextNode textnode = childnode as DomTextNode;
                    if (textnode != null)
                    {
                        textnode.CopyInnerText(textWriter);
                    }
                }
                return textWriter.ToString();
            }

        }
        public virtual void SetInnerHtml(string innerHtml)
        {
            //parse html and create dom node
            //clear content of this node
            this.ClearAllElements();
        }

        public virtual void WriteNode(DomTextWriter writer)
        {
            //write node
            writer.Write("<", this.Name);
            //count attribute 
            foreach (var attr in this.GetAttributeIterForward())
            {
                //name=value
                writer.Write(' ');
                writer.Write(attr.Name);
                writer.Write("=\"");
                writer.Write(attr.Value);
                writer.Write("\"");
            }
            writer.Write('>');
            //content
            foreach (var childnode in this.GetChildNodeIterForward())
            {
                HtmlElement childHtmlNode = childnode as HtmlElement;
                if (childHtmlNode != null)
                {
                    childHtmlNode.WriteNode(writer);
                }
                HtmlTextNode htmlTextNode = childnode as HtmlTextNode;
                if (htmlTextNode != null)
                {
                    htmlTextNode.WriteTextNode(writer);
                }
            }
            //close tag
            writer.Write("</", this.Name, ">");
        }
        public override void GetGlobalLocation(out int x, out int y)
        {
            x = y = 0;
        }
        public override void GetGlobalLocationRelativeToRoot(out int x, out int y)
        {
            x = y = 0;
        }
        public override void GetViewport(out int x, out int y)
        {
            x = y = 0;//temp

        }
        public virtual void SetLocation(int x, int y)
        {
        }
        public virtual float ActualHeight => 0;
        public virtual float ActualWidth => 0;


    }
}