//BSD, 2014-present, WinterDev 

using System;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
namespace LayoutFarm.Composers
{
    sealed class HtmlRootElement : HtmlElement
    {
        public HtmlRootElement(HtmlDocument ownerDoc)
            : base(ownerDoc, 0, 0)
        {
        }
    }

    sealed class ExternalHtmlElement : HtmlElement
    {
        LazyCssBoxCreator _lazyCreator;
        public ExternalHtmlElement(HtmlDocument owner, int prefix, int localNameIndex, LazyCssBoxCreator lazyCreator)
            : base(owner, prefix, localNameIndex)
        {
            _lazyCreator = lazyCreator;
        }
        internal override bool HasCustomPrincipalBoxGenerator
        {
            get
            {
                return true;
            }
        }
        internal override CssBox GetPrincipalBox(CssBox parentCssBox, HtmlHost host)
        {
            if (this.CurrentPrincipalBox != null)
            {
                return this.CurrentPrincipalBox;
            }
            else
            {

                _lazyCreator(parentCssBox.GetInternalRootGfx(), out RenderElement re, out object controller);
                CssBox wrapper = CustomCssBoxGenerator.CreateWrapper(((HtmlDocument)this.OwnerDocument).Host, controller, re, this.Spec, false);
                this.SetPrincipalBox(wrapper);
                return wrapper;
            }
        }
    }



    sealed class ShadowRootElement : HtmlElement
    {
        //note: this version is not conform with w3c  
        HtmlShadowDocument _shadowDoc;
        CssBox _rootbox;
        public ShadowRootElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            _shadowDoc = new HtmlShadowDocument(owner.Host, owner);
            _shadowDoc.SetDomUpdateHandler(owner.DomUpdateHandler);
        }
        //
        internal override bool HasCustomPrincipalBoxGenerator => true;
        //
        internal override CssBox GetPrincipalBox(CssBox parentCssBox, HtmlHost htmlHost)
        {
            if (_rootbox != null)
            {
                return _rootbox;
            }
            else
            {
                var root = (HtmlElement)_shadowDoc.RootNode;
                //1. builder 
                var renderTreeBuilder = htmlHost.GetRenderTreeBuilder();
                //------------------------------------------------------------------- 
                //2. generate render tree
                ////build rootbox from htmldoc

                var rootElement = renderTreeBuilder.BuildCssRenderTree2(_shadowDoc,
                    htmlHost.BaseStylesheet,
                    htmlHost.RootGfx);
                //3. create small htmlContainer

                _rootbox = new CssBox(this.Spec, parentCssBox.RootGfx);
                root.SetPrincipalBox(_rootbox);
                htmlHost.UpdateChildBoxes(root, true);
                return _rootbox;
            }
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
    }
}