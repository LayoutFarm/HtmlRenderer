// 2015,2014 ,BSD, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;


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
                    } break;
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
            scrollView.SetSize(box.SizeWidth, box.SizeHeight);
            scrollView.SetExpectedSize(box.SizeWidth, box.SizeHeight);

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
        
        internal void SetPrincipalBox(CssBox box)
        {
            this.principalBox = box;
            this.SkipPrincipalBoxEvalulation = true;
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


    }



}