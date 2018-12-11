//BSD, 2014-present, WinterDev 

using System;
namespace LayoutFarm.WebDom.Impl
{
    sealed class HtmlRootElement : HtmlElement
    {
        public HtmlRootElement(HtmlDocument ownerDoc)
            : base(ownerDoc, 0, 0)
        {
        }
#if DEBUG
        public override string ToString()
        {
            return "!root";
        }
#endif
    }




    sealed class ShadowRootElement : HtmlElement
    {
        //note: this version is not conform with w3c  
        HtmlShadowDocument _shadowDoc;
        public ShadowRootElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            _shadowDoc = new HtmlShadowDocument(owner);
            _shadowDoc.SetDomUpdateHandler(owner.DomUpdateHandler);
        }
        public override void AddChild(DomNode childNode)
        {
            //add dom node to this node
            if (childNode.ParentNode != null)
            {
                throw new NotSupportedException("remove from its parent first");
            }
            _shadowDoc.RootNode.AddChild(childNode);
        }
#if DEBUG
        public override string ToString()
        {
            return "shadow-root";
        }
#endif
    }
}