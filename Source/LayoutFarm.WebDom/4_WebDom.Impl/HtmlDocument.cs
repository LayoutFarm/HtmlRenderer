//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using System;
namespace LayoutFarm.WebDom.Impl
{
    public partial class HtmlDocument : WebDocument
    {
        DomElement _bodyElement;
        DomElement _rootNode;
        int _domUpdateVersion;
        EventHandler _domUpdatedHandler;
        public HtmlDocument()
            : this(HtmlPredefineNames.CreateUniqueStringTableClone())
        {
        }
        public HtmlDocument(UniqueStringTable sharedUniqueStringTable)
            : base(sharedUniqueStringTable)
        {
            //auto create root doc
            _rootNode = new HtmlRootElement(this);
        }
        protected void SetRootElement(DomElement rootE)
        {
            _rootNode = rootE;
        }
        public override DomElement RootNode => _rootNode;
        public HtmlElement QuerySelector(string pattern)
        {
            HtmlElement rootElm = _rootNode as HtmlElement;
            if (rootElm == null) return null;
            return rootElm.QuerySelector(pattern);
        }
        public HtmlElement QuerySelector(QuerySelectorPatterns patts)
        {
            HtmlElement rootElm = _rootNode as HtmlElement;
            if (rootElm == null) return null;
            return rootElm.QuerySelector(patts);
        }
        public HtmlNodeList QuerySelectAll(string pattern)
        {
            HtmlElement rootElm = _rootNode as HtmlElement;
            if (rootElm == null) return null;
            return rootElm.QuerySelectAll(pattern);
        }
        public HtmlNodeList QuerySelectAll(QuerySelectorPatterns patts)
        {
            HtmlElement rootElm = _rootNode as HtmlElement;
            if (rootElm == null) return null;
            return rootElm.QuerySelectAll(patts);
        }
        public DomElement BodyElement
        {
            get
            {
                if (_bodyElement == null)
                {
                    //TODO: review here again!!!
                    //
                    int j = _rootNode.ChildrenCount;
                    for (int i = 0; i < j; ++i)
                    {
                        HtmlElement node = _rootNode.GetChildNode(i) as HtmlElement;
                        if (node != null)
                        {
                            if (node.Name == "body")
                            {
                                _bodyElement = node;
                                break;
                            }
                            else if (node.Name == "html")
                            {
                                foreach (var childNode in node.GetChildNodeIterForward())
                                {
                                    HtmlElement childNodeE = childNode as HtmlElement;
                                    if (childNodeE != null && childNodeE.Name == "body")
                                    {
                                        return _bodyElement = childNodeE;
                                    }
                                }
                            }
                        }
                    }
                }

                return _bodyElement;
            }
        }
        //
        public override int DomUpdateVersion => _domUpdateVersion;

        public override void IncDomVersion()
        {
            _domUpdateVersion++;
            if (_domUpdatedHandler != null)
            {
                _domUpdatedHandler(this, EventArgs.Empty);
            }
        }

        class MyHtmlElement : HtmlElement
        {
            public MyHtmlElement(HtmlDocument owner, int prefixNameIndex, int localNameIndex)
                : base(owner, prefixNameIndex, localNameIndex)
            {
            }
        }

        public override DomElement CreateElement(string prefix, string localName)
        {
            //actual implementation
            return new MyHtmlElement(this,
                AddStringIfNotExists(prefix),
                AddStringIfNotExists(localName));
        }
        public override DomElement CreateShadowRootElement()
        {
            return new ShadowRootElement(this,
                AddStringIfNotExists(null),
                AddStringIfNotExists("shadow-root"));
        }
        public override DomNode CreateDocumentNodeElement()
        {
            return new DomDocumentNode(this);
        }
        public DomAttribute CreateAttribute(WellknownName attrName)
        {
            return new DomAttribute(this, 0, (int)attrName);
        }
        public override DomTextNode CreateTextNode(char[] strBufferForElement)
        {
            return new HtmlTextNode(this, strBufferForElement);
        }

        public virtual WebDocument CreateDocumentFragment()
        {
            return new HtmlDocumentFragment(this);
        }

        //---------------------------------------------------------
        public void SetDomUpdateHandler(EventHandler h)
        {
            _domUpdatedHandler = h;
        }
        public CssActiveSheet CssActiveSheet
        {
            get;
            set;
        }
        public EventHandler DomUpdateHandler => _domUpdatedHandler;

        protected void RaiseUpdateEvent()
        {
            _domUpdatedHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}