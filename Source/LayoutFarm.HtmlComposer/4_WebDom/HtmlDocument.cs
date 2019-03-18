//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using LayoutFarm.WebDom;
namespace LayoutFarm.Composers
{

    public class HtmlDocument : LayoutFarm.WebDom.Impl.HtmlDocument
    {
        //implementation specific...
        //foc custom elements 
        Dictionary<string, CreateCssBoxDelegate> _registeredCustomElemenGens = new Dictionary<string, CreateCssBoxDelegate>();
        internal HtmlDocument(HtmlBoxes.HtmlHost host)
        {
            this.Host = host;
            this.SetRootElement(new HtmlRootElement(this));
        }
        internal HtmlDocument(HtmlBoxes.HtmlHost host, UniqueStringTable sharedUniqueStringTable)
            : base(sharedUniqueStringTable)
        {
            this.Host = host;
            //default root
            this.SetRootElement(new HtmlRootElement(this));
            //TODO: test only
#if DEBUG
            this.RegisterCustomElement("custom_div", CustomBoxGenSample1.CreateCssBox);
#endif
        }

        internal HtmlBoxes.HtmlHost Host { get; private set; }

        public override DomElement CreateElement(string prefix, string localName)
        {
            //actual dom implementation ***

            HtmlElement htmlElement = null;
            switch (localName)
            {
                case "img":
                    {
                        htmlElement = new HtmlImageElement(
                            this,
                            AddStringIfNotExists(prefix),
                            AddStringIfNotExists(localName));
                    }
                    break;
                case "input":
                    {
                        //input type
                        htmlElement = new HtmlInputElement(
                           this,
                           AddStringIfNotExists(prefix),
                           AddStringIfNotExists(localName));
                    }
                    break;
                case "option":
                    {
                        htmlElement = new HtmlOptionElement(
                           this,
                           AddStringIfNotExists(prefix),
                           AddStringIfNotExists(localName));
                    }
                    break;
                default:
                    {
                        htmlElement = new HtmlElement(this,
                           AddStringIfNotExists(prefix),
                           AddStringIfNotExists(localName));
                        htmlElement.WellknownElementName = WellKnownDomNodeMap.EvaluateTagName(htmlElement.LocalName);
                    }
                    break;
            }
            return htmlElement;
        }
        public override DomNode CreateDocumentNodeElement()
        {
            return new DomDocumentNode(this);
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
            _registeredCustomElemenGens[customElementName] = cssBoxGen;
        }
        public bool TryGetCustomBoxGenerator(string customElementName, out CreateCssBoxDelegate cssBoxGen)
        {
            return _registeredCustomElemenGens.TryGetValue(customElementName, out cssBoxGen);
        }
        public ImageBinder GetImageBinder(string imgurl)
        {
            ImageBinder imgBinder = new ImageBinder(imgurl);
            Host.ChildRequestImage(imgBinder, null);
            return imgBinder;
        }
    }
}