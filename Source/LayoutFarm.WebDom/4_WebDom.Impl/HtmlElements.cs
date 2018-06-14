//BSD, 2014-2018, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo 

using System.Text;
namespace LayoutFarm.WebDom.Impl
{
    public partial class HtmlElement : DomElement, IHtmlElement
    {
        CssRuleSet elementRuleSet;
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


        public static void InvokeNotifyChangeOnIdleState(HtmlElement elem, ElementChangeKind changeKind)
        {
            elem.OnElementChangedInIdleState(changeKind);
        }

        protected override void OnContentUpdate()
        {
            base.OnContentUpdate();
            OnElementChangedInIdleState(ElementChangeKind.ContentUpdate);
        }

        protected override void OnElementChangedInIdleState(ElementChangeKind changeKind)
        {
            //1. 
            this.OwnerDocument.SetDocumentState(DocumentState.ChangedAfterIdle);
            if (this.OwnerDocument.IsDocFragment) return;
            HtmlDocument owner = this.OwnerDocument as HtmlDocument;
            owner.DomUpdateVersion++;
        }

        public CssRuleSet ElementRuleSet
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


        protected override void OnElementChanged()
        {
        }
        //------------------------------------
        public virtual string GetInnerHtml()
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
        public virtual void SetInnerHtml(string innerHtml)
        {
            //parse html and create dom node
            //clear content of this node
            this.ClearAllElements();
            //parse 
            //throw new NotSupportedException();
            //then apply new content ***

            //WebDocumentParser.ParseHtmlDom(
            //    new LayoutFarm.WebDom.Parser.TextSource(innerHtml.ToCharArray()),
            //    (HtmlDocument)this.OwnerDocument,
            //    this); 

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
        public override void GetViewport(out int x, out int y)
        {
            x = y = 0;//temp

        }
        public virtual void SetLocation(int x, int y)
        {
        }
        public virtual float ActualHeight
        {
            get
            {
                return 0;
            }
        }
        public virtual float ActualWidth
        {
            get
            {
                return 0;
            }
        }
    }
}