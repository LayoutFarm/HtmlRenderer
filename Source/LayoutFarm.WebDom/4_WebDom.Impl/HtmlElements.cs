//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo 

using System;
using System.Collections.Generic;
using System.Text;

namespace LayoutFarm.WebDom.Impl
{
    //--------------------------------------------------

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
        [System.ThreadStatic]
        static Stack<T> s_pool;
        [System.ThreadStatic]
        static Func<T> s_newHandler;
        [System.ThreadStatic]
        static Action<T> s_releaseCleanUp;

        public static TempContext<T> Borrow(out T freeItem)
        {
            return new TempContext<T>(out freeItem);
        }

        public static void SetNewHandler(Func<T> newHandler, Action<T> releaseCleanUp = null)
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

        public bool HasSpecialPresentation { get; set; }

        public System.Action<object> SpecialPresentationUpdate;

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