// 2015,2014 ,BSD, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;
using LayoutFarm.Css;

namespace LayoutFarm.WebDom
{

    public partial class HtmlDocument : LayoutFarm.WebDom.Impl.HtmlDocument
    {

        //foc custom elements 
        Dictionary<string, CreateCssBoxDelegate> registedCustomElemenGens = new Dictionary<string, CreateCssBoxDelegate>();
        public HtmlDocument()
        {
            this.SetRootElement(new HtmlRootElement(this));
        }
        internal HtmlDocument(UniqueStringTable sharedUniqueStringTable)
            : base(sharedUniqueStringTable)
        {
            //default root
            this.SetRootElement(new HtmlRootElement(this));
            //test only
            this.RegisterCustomElement("fivespace", CustomBoxGenSample1.CreateCssBox);
        }

        public override DomElement CreateElement(string prefix, string localName)
        {
            //actual implementation
            return new HtmlElement(this,
                AddStringIfNotExists(prefix),
                AddStringIfNotExists(localName));
        }

        public override DomTextNode CreateTextNode(char[] strBufferForElement)
        {
            return new HtmlTextNode(this, strBufferForElement);
        }
        public override WebDocument CreateDocumentFragment()
        {
            return new HtmlDocumentFragment(this);
        }
        //---------------------------------------------------------
        public DomElement CreateWrapperElement(
            string wrapperElementName,
            LazyCssBoxCreator lazyCssBoxCreator)
        {
            return new ExternalHtmlElement(this,
                AddStringIfNotExists(null),
                AddStringIfNotExists(wrapperElementName),
                lazyCssBoxCreator);
        }
        public override DomElement CreateShadowRootElement()
        {
            return new ShadowRootElement(this,
                AddStringIfNotExists(null),
                AddStringIfNotExists("shadow-root"));
        }


        //-------------------------------------------------------------
        public void RegisterCustomElement(string customElementName, CreateCssBoxDelegate cssBoxGen)
        {
            //replace
            registedCustomElemenGens[customElementName] = cssBoxGen;
        }
        public bool TryGetCustomBoxGenerator(string customElementName, out CreateCssBoxDelegate cssBoxGen)
        {
            return this.registedCustomElemenGens.TryGetValue(customElementName, out cssBoxGen);
        }



    }





}