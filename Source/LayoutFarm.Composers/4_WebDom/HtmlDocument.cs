// 2015,2014 ,BSD, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;

namespace LayoutFarm.WebDom
{

    public partial class HtmlDocument : WebDocument
    {
        DomElement rootNode;
        int domUpdateVersion;
        EventHandler domUpdatedHandler;

        public HtmlDocument()
            : base(HtmlPredefineNames.CreateUniqueStringTableClone())
        {
            //default root
            rootNode = new HtmlRootElement(this);
        }
        internal HtmlDocument(UniqueStringTable sharedUniqueStringTable)
            : base(sharedUniqueStringTable)
        {
            //default root
            rootNode = new HtmlRootElement(this);
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
        public DomAttribute CreateAttribute(WellknownName attrName)
        {

            return new DomAttribute(this, 0, (int)attrName);
        }
        public override DomTextNode CreateTextNode(char[] strBufferForElement)
        {
            return new HtmlTextNode(this, strBufferForElement);
        }

        public DomElement CreateWrapperElement(
            LazyCssBoxCreator lazyCssBoxCreator)
        {
            return new ExternalHtmlElement(this,
                AddStringIfNotExists(null),
                AddStringIfNotExists("x"),
                lazyCssBoxCreator);
        }

        internal void SetDomUpdateHandler(EventHandler h)
        {
            this.domUpdatedHandler = h;
        }
        internal CssActiveSheet CssActiveSheet
        {
            get;
            set;
        }
    }



    public partial class FragmentHtmlDocument : HtmlDocument
    {
        HtmlDocument primaryHtmlDoc;
        internal FragmentHtmlDocument(HtmlDocument primaryHtmlDoc)
            : base(primaryHtmlDoc.UniqueStringTable)
        {
            this.primaryHtmlDoc = primaryHtmlDoc;
        }
    }

    //------------------------------------------------------------
    public delegate void LazyCssBoxCreator(RootGraphic rootgfx, out RenderElement re, out object controller);


    static class HtmlPredefineNames
    {

        static readonly ValueMap<WellknownName> _wellKnownHtmlNameMap =
            new ValueMap<WellknownName>();

        static UniqueStringTable htmlUniqueStringTableTemplate = new UniqueStringTable();

        static HtmlPredefineNames()
        {
            int j = _wellKnownHtmlNameMap.Count;
            for (int i = 0; i < j; ++i)
            {
                htmlUniqueStringTableTemplate.AddStringIfNotExist(_wellKnownHtmlNameMap.GetStringFromValue((WellknownName)(i + 1)));
            }
        }
        public static UniqueStringTable CreateUniqueStringTableClone()
        {
            return htmlUniqueStringTableTemplate.Clone();
        }

    }
}