// 2015,2014 ,BSD, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
using LayoutFarm.Composers;

namespace LayoutFarm.WebDom
{
    
    public   partial class HtmlElement : DomElement
    {
        CssBox principalBox;
        Css.BoxSpec boxSpec;
        CssRuleSet elementRuleSet;

        internal HtmlElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            this.boxSpec = new Css.BoxSpec();
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

        public override void SetAttribute(DomAttribute attr)
        {
            base.SetAttribute(attr);
            switch ((WellknownName)attr.LocalNameIndex)
            {
                case WellknownName.Style:
                    {
                        //parse and evaluate style here 
                        //****
                    } break;
            }
        }
        //------------------------------------
        public Point GetActualElementGlobalLocation()
        {
            return this.principalBox.GetElementGlobalLocation();
        }
        
        public void SetPrincipalBox(CssBox box)
        {
            this.principalBox = box;
            this.SkipPrincipalBoxEvalulation = true;
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

        protected override void OnContentUpdate()
        {
            base.OnContentUpdate();
            OnChangeInIdleState(ElementChangeKind.ContentUpdate);
        }
        
        protected override void OnChangeInIdleState(ElementChangeKind changeKind)
        {
            //1. 
            this.OwnerDocument.SetDocumentState(DocumentState.ChangedAfterIdle);
            //2.
            this.SkipPrincipalBoxEvalulation = false;
            var cnode = this.ParentNode;
            while (cnode != null)
            {
                ((HtmlElement)cnode).SkipPrincipalBoxEvalulation = false;
                cnode = cnode.ParentNode;
            }
        }
         


        //------------------------------------
        internal static void InvokeNotifyChangeOnIdleState(HtmlElement elem, ElementChangeKind changeKind)
        {
            elem.OnChangeInIdleState(changeKind);
        } 
        internal CssRuleSet ElementRuleSet
        {
            get
            {
                return this.elementRuleSet;
            }
            set
            {
                this.elementRuleSet = value;
            }
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
        }
        internal CssBox GetPrincipalBox()
        {
            return this.principalBox;
        }
         
        //------------------------------------
        public string GetInnerHtml()
        {
            //get inner html*** 
            StringBuilder stbuilder = new StringBuilder();
            DomTextWriter textWriter = new DomTextWriter(stbuilder);
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
            return stbuilder.ToString();
        }
        public void SetInnerHtml(string innerHtml)
        {
            //parse html and create dom node
            //clear content of this node
            this.ClearAllElements();
            //then apply new content 
            WebDocumentParser.ParseHtmlDom(
                new LayoutFarm.WebDom.Parser.TextSnapshot(innerHtml.ToCharArray()),
                (HtmlDocument)this.OwnerDocument,
                this);
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
    }



}