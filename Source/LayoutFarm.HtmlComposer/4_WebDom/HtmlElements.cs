//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
namespace LayoutFarm.Composers
{

    static class CssParserPool
    {
        static Stack<WebDom.Parser.CssParser> s_pool = new Stack<WebDom.Parser.CssParser>();
        public static WebDom.Parser.CssParser GetFreeParser()
        {
            if (s_pool.Count > 0) return s_pool.Pop();
            return new WebDom.Parser.CssParser();
        }
        public static void ReleaseParser(ref WebDom.Parser.CssParser parser)
        {
            s_pool.Push(parser);
        }
    }

    class HtmlElement : LayoutFarm.WebDom.Impl.HtmlElement
    {
        CssBox _principalBox;
        Css.BoxSpec _boxSpec;
        internal HtmlElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            this._boxSpec = new Css.BoxSpec();
        }
        public override void AddChild(DomNode childNode)
        {
            if (_principalBox != null && childNode.NodeKind == HtmlNodeKind.TextNode)
            {

            }
            base.AddChild(childNode);
        }

        public override void SetAttribute(DomAttribute attr)
        {
            base.SetAttribute(attr); //to base
            //----------------------

            switch ((WellknownName)attr.LocalNameIndex)
            {
                case WellknownName.Src:
                    {
                        switch (this.WellknownElementName)
                        {
                            case WellKnownDomNodeName.img:
                                {
                                    if (_principalBox != null)
                                    {
                                        CssBoxImage boxImg = (CssBoxImage)_principalBox;
                                        boxImg.ImageBinder = new ClientImageBinder(attr.Value);
                                        boxImg.InvalidateGraphics();
                                    }
                                }
                                break;
                        }

                    }
                    break;
                case WellknownName.Style:
                    {
                        //TODO: parse and evaluate style here 
                        //****
                        WebDom.Parser.CssParser miniCssParser = CssParserPool.GetFreeParser();
                        //parse and evaluate the ruleset
                        CssRuleSet parsedRuleSet = miniCssParser.ParseCssPropertyDeclarationList(attr.Value.ToCharArray());

                        Css.BoxSpec spec = null;
                        if (this.ParentNode != null)
                        {
                            spec = ((HtmlElement)this.ParentNode).Spec;
                        }
                        foreach (WebDom.CssPropertyDeclaration propDecl in parsedRuleSet.GetAssignmentIter())
                        {

                            SpecSetter.AssignPropertyValue(
                                _boxSpec,
                                spec,
                                propDecl);
                        }
                        CssParserPool.ReleaseParser(ref miniCssParser);
                    }
                    break;
            }
        }
        public override void ClearAllElements()
        {
            //clear presentation 
            if (_principalBox != null)
            {
                _principalBox.Clear();
            }
            base.ClearAllElements();
        }

        protected override void OnElementChangedInIdleState(ElementChangeKind changeKind)
        {

            //1. 
            this.OwnerDocument.SetDocumentState(DocumentState.ChangedAfterIdle);
            if (this.OwnerDocument.IsDocFragment) return;
            //------------------------------------------------------------------
            //2. need box evaluation again
            this.SkipPrincipalBoxEvalulation = false;
            //3. propag

            HtmlElement cnode = (HtmlElement)this.ParentNode;
            HtmlDocument owner = this.OwnerDocument as HtmlDocument;
            while (cnode != null)
            {
                cnode.SkipPrincipalBoxEvalulation = false;
                if (cnode.ParentNode != null)
                {
                    cnode = (HtmlElement)cnode.ParentNode;
                }
                else
                {
                    if (cnode.SubParentNode != null)
                    {
                        cnode = (HtmlElement)cnode.SubParentNode;
                    }
                    else
                    {
                        cnode = null;
                    }
                }
            }
            owner.IncDomVersion();
        }

        protected override void OnElementChanged()
        {
            CssBox box = this._principalBox;
            if (box is CssScrollView)
            {
                return;
            }
            //change 
            var boxSpec = CssBox.UnsafeGetBoxSpec(box);

            //create scrollbar
            //...?

            var scrollView = new CssScrollView(boxSpec, box.RootGfx);
            scrollView.SetController(this);
            scrollView.SetVisualSize(box.VisualWidth, box.VisualHeight);
            scrollView.SetExpectedSize(box.VisualWidth, box.VisualHeight);
            box.ParentBox.InsertChild(box, scrollView);
            box.ParentBox.RemoveChild(box);
            //scrollbar width= 10
            scrollView.SetInnerBox(box);
            //change primary render element
            this._principalBox = scrollView;
            scrollView.InvalidateGraphics();
        }
        public override void SetInnerHtml(string innerHtml)
        {
            // parse html and create dom node
            //clear content of this node
            this.ClearAllElements();
            //then apply new content 
            WebDocumentParser.ParseHtmlDom(
                new LayoutFarm.WebDom.Parser.TextSource(innerHtml.ToCharArray()),
                (HtmlDocument)this.OwnerDocument,
                this);
        }
        public override void GetGlobalLocation(out int x, out int y)
        {
            float globalX, globalY;
            this._principalBox.GetGlobalLocation(out globalX, out globalY);
            x = (int)globalX;
            y = (int)globalY;
        }
        public override void GetGlobalLocationRelativeToRoot(out int x, out int y)
        {
            float globalX, globalY;
            this._principalBox.GetGlobalLocationRelativeToRoot(out globalX, out globalY);
            x = (int)globalX;
            y = (int)globalY;
        }
        public override void SetLocation(int x, int y)
        {
            if (_principalBox != null)
            {
                _principalBox.SetLocation(x, y);
            }
            else
            {
                DomAttribute domAttr;
                if (this.TryGetAttribute(WellknownName.Style, out domAttr))
                {
                    domAttr.Value += ";left:" + x + "px;top:" + y + "px;";
                }
                else
                {
                    //create new
                    throw new NotSupportedException();
                }
            }
        }
        internal void SetPrincipalBox(CssBox box)
        {
            this._principalBox = box;
            this.SkipPrincipalBoxEvalulation = true;
        }
        public override float ActualWidth
        {
            get
            {
                return this._principalBox.VisualWidth;
            }
        }
        public override float ActualHeight
        {
            get
            {
                return this._principalBox.VisualHeight;
            }
        }


        //-------------------------------------------
        internal virtual CssBox GetPrincipalBox(CssBox parentCssBox, HtmlHost host)
        {
            //this method is called when HasCustomPrincipalBoxGenerator = true
            throw new NotImplementedException();
        }
        internal virtual bool HasCustomPrincipalBoxGenerator
        {
            //use builtin cssbox generator***
            get { return false; }
        }


        internal bool IsStyleEvaluated
        {
            get;
            set;
        }
        internal bool SkipPrincipalBoxEvalulation
        {
            get;
            set;
        }
        internal static CssBox InternalGetPrincipalBox(HtmlElement element)
        {
            return element._principalBox;
        }
        internal Css.BoxSpec Spec
        {
            get { return this._boxSpec; }
            //set { this.boxSpec = value; }
        }
        internal CssBox CurrentPrincipalBox
        {
            get { return this._principalBox; }
        }

        public override bool RemoveChild(DomNode childNode)
        {
            //remove presentation
            var childElement = childNode as HtmlElement;
            if (childElement != null && childElement.ParentNode == this)
            {
                if (this._principalBox != null)
                {
                    var cssbox = childElement.CurrentPrincipalBox;
                    if (cssbox != null)
                    {
                        //remove from parent
                        _principalBox.RemoveChild(cssbox);
                    }
                }
            }
            return base.RemoveChild(childNode);
        }


    }
}