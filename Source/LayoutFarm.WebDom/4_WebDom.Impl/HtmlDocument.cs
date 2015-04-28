// 2015,2014 ,BSD, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;

using LayoutFarm.HtmlBoxes;
using LayoutFarm.Css;

namespace LayoutFarm.WebDom.Impl
{
    public partial class HtmlDocument : WebDocument
    {

        DomElement rootNode;
        int domUpdateVersion;
        EventHandler domUpdatedHandler;
        public HtmlDocument()
            : this(HtmlPredefineNames.CreateUniqueStringTableClone())
        {
        }
        public HtmlDocument(UniqueStringTable sharedUniqueStringTable)
            : base(sharedUniqueStringTable)
        {
            //auto create root doc
            rootNode = new HtmlRootElement(this);
        }
        protected void SetRootElement(DomElement rootE)
        {
            this.rootNode = rootE;
        }
        public override DomElement RootNode
        {
            get
            {
                return rootNode;
            }
        }
        public override int DomUpdateVersion
        {
            get { return this.domUpdateVersion; }
            set
            {
                this.domUpdateVersion = value;
                if (domUpdatedHandler != null)
                {
                    domUpdatedHandler(this, EventArgs.Empty);
                }
            }
        }
        public override DomElement CreateElement(string prefix, string localName)
        {
            //actual implementation
            return new HtmlElement(this,
                AddStringIfNotExists(prefix),
                AddStringIfNotExists(localName));
        }
        public override DomNode CreateDocumentNodeElement()
        {
            return new DomDocumentNode(this);
        }
        public DomAttribute CreateAttribute(WellknownName attrName)
        {

            return new DomAttribute(this, 0, (int)attrName);
        }
        public override DomTextNode CreateTextNode(char[] strBufferForElement)
        {
            return new HtmlTextNode(this, strBufferForElement);
        }


        public override DomElement CreateShadowRootElement()
        {
            return new ShadowRootElement(this,
                AddStringIfNotExists(null),
                AddStringIfNotExists("shadow-root"));
        }

        public virtual WebDocument CreateDocumentFragment()
        {
            return new HtmlDocumentFragment(this);
        }

        //---------------------------------------------------------
        public void SetDomUpdateHandler(EventHandler h)
        {
            this.domUpdatedHandler = h;
        }
        public CssActiveSheet CssActiveSheet
        {
            get;
            set;
        }
        public EventHandler DomUpdateHandler
        {
            get { return this.domUpdatedHandler; }
        }
        protected void RaiseUpdateEvent()
        {
            if (this.domUpdatedHandler != null)
            {
                this.domUpdatedHandler(this, EventArgs.Empty);
            }
        }
    }



}