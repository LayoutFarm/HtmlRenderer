//BSD, 2014-2017, WinterDev 

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
        LazyCssBoxCreator lazyCreator;
        public ExternalHtmlElement(HtmlDocument owner, int prefix, int localNameIndex, LazyCssBoxCreator lazyCreator)
            : base(owner, prefix, localNameIndex)
        {
            this.lazyCreator = lazyCreator;
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
                RenderElement re;
                object controller;
                lazyCreator(parentCssBox.GetInternalRootGfx(), out re, out controller);
                CssBox wrapper = CustomCssBoxGenerator.CreateWrapper(controller, re, this.Spec, false);
                this.SetPrincipalBox(wrapper);
                return wrapper;
            }
        }
    }

    

    sealed class ShadowRootElement : HtmlElement
    {
        //note: this version is not conform with w3c  
        HtmlShadowDocument shadowDoc;
        CssBox rootbox;
        public ShadowRootElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            shadowDoc = new HtmlShadowDocument(owner);
            shadowDoc.SetDomUpdateHandler(owner.DomUpdateHandler);
        }
        internal override bool HasCustomPrincipalBoxGenerator
        {
            get
            {
                return true;
            }
        }
        internal override CssBox GetPrincipalBox(CssBox parentCssBox, HtmlHost htmlHost)
        {
            if (rootbox != null)
            {
                return this.rootbox;
            }
            else
            {
                var root = (HtmlElement)shadowDoc.RootNode;
                //1. builder 
                var renderTreeBuilder = htmlHost.GetRenderTreeBuilder();
                //------------------------------------------------------------------- 
                //2. generate render tree
                ////build rootbox from htmldoc

                var rootElement = renderTreeBuilder.BuildCssRenderTree2(shadowDoc,
                    htmlHost.BaseStylesheet,
                    htmlHost.RootGfx);
                //3. create small htmlContainer

                rootbox = new CssBox(this.Spec, parentCssBox.RootGfx);
                root.SetPrincipalBox(rootbox);
                htmlHost.UpdateChildBoxes(root, true);
                return rootbox;
            }
        }
        public override void AddChild(DomNode childNode)
        {
            //add dom node to this node
            if (childNode.ParentNode != null)
            {
                throw new NotSupportedException("remove from its parent first");
            }
            shadowDoc.RootNode.AddChild(childNode);
        }
    }
}