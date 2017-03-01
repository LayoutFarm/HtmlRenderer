//BSD, 2014-2017, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
namespace LayoutFarm.Composers
{
    class HtmlElement : LayoutFarm.WebDom.Impl.HtmlElement
    {
        CssBox principalBox;
        Css.BoxSpec boxSpec;
        internal HtmlElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            this.boxSpec = new Css.BoxSpec();
        }

        public override void SetAttribute(DomAttribute attr)
        {
            base.SetAttribute(attr);
            switch ((WellknownName)attr.LocalNameIndex)
            {
                case WellknownName.Style:
                    {
                        //TODO: parse and evaluate style here 
                        //****
                    }
                    break;
            }
        }
        public override void ClearAllElements()
        {
            //clear presentation 
            if (principalBox != null)
            {
                principalBox.Clear();
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
            var cnode = this.ParentNode;
            while (cnode != null)
            {
                ((HtmlElement)cnode).SkipPrincipalBoxEvalulation = false;
                cnode = cnode.ParentNode;
            }

            HtmlDocument owner = this.OwnerDocument as HtmlDocument;
            owner.DomUpdateVersion++;
        }


        protected override void OnElementChanged()
        {
            CssBox box = this.principalBox;
            if (box is CssScrollView)
            {
                return;
            }
            //change 
            var boxSpec = CssBox.UnsafeGetBoxSpec(box);
            //create scrollbar


            var scrollView = new CssScrollView(boxSpec, box.RootGfx);
            scrollView.SetController(this);
            scrollView.SetVisualSize(box.VisualWidth, box.VisualHeight);
            scrollView.SetExpectedSize(box.VisualWidth, box.VisualHeight);
            box.ParentBox.InsertChild(box, scrollView);
            box.ParentBox.RemoveChild(box);
            //scrollbar width= 10
            scrollView.SetInnerBox(box);
            //change primary render element
            this.principalBox = scrollView;
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
            this.principalBox.GetGlobalLocation(out globalX, out globalY);
            x = (int)globalX;
            y = (int)globalY;
        }
        public override void SetLocation(int x, int y)
        {
            if (principalBox != null)
            {
                principalBox.SetLocation(x, y);
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
            this.principalBox = box;
            this.SkipPrincipalBoxEvalulation = true;
        }
        public override float ActualWidth
        {
            get
            {
                return this.principalBox.VisualWidth;
            }
        }
        public override float ActualHeight
        {
            get
            {
                return this.principalBox.VisualHeight;
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
            return element.principalBox;
        }
        internal Css.BoxSpec Spec
        {
            get { return this.boxSpec; }
            //set { this.boxSpec = value; }
        }
        internal CssBox CurrentPrincipalBox
        {
            get { return this.principalBox; }
        }

        public override bool RemoveChild(DomNode childNode)
        {
            //remove presentation
            var childElement = childNode as HtmlElement;
            if (childElement != null && childElement.ParentNode == this)
            {
                if (this.principalBox != null)
                {
                    var cssbox = childElement.CurrentPrincipalBox;
                    if (cssbox != null)
                    {
                        //remove from parent
                        principalBox.RemoveChild(cssbox);
                    }
                }
            }
            return base.RemoveChild(childNode);
        }
    }
}