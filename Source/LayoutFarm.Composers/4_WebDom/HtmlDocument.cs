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
    //delegate for create cssbox
    public delegate LayoutFarm.HtmlBoxes.CssBox CreateCssBoxDelegate(
            HtmlElement domE,
            LayoutFarm.HtmlBoxes.CssBox parentBox,
            LayoutFarm.Css.BoxSpec spec,
            LayoutFarm.HtmlBoxes.HtmlHost htmlhost);

    //temp !, test only for custom box creation
    static class CustomBoxGenSample1
    {
        internal static LayoutFarm.HtmlBoxes.CssBox CreateCssBox(
            HtmlElement domE,
            LayoutFarm.HtmlBoxes.CssBox parentBox,
            LayoutFarm.Css.BoxSpec spec,
            LayoutFarm.HtmlBoxes.HtmlHost htmlhost)
        {


            //create cssbox 
            //test only!           
            var newspec = new BoxSpec();
            BoxSpec.InheritStyles(newspec, spec);
            newspec.BackgroundColor = Color.Blue;
            newspec.Width = new CssLength(50, CssUnitOrNames.Pixels);
            newspec.Height = new CssLength(50, CssUnitOrNames.Pixels);
            newspec.Position = CssPosition.Absolute;
            newspec.Freeze(); //freeze before use

            HtmlElement htmlElement = (HtmlElement)domE;
            var newBox = new CssBox(newspec, parentBox.RootGfx);
            newBox.SetController(domE);
            htmlElement.SetPrincipalBox(newBox);
            //auto set bc of the element

            parentBox.AppendChild(newBox);
            htmlhost.UpdateChildBoxes(htmlElement, true);
            //----------
            return newBox;
        }

    }

    public partial class HtmlDocument : WebDocument
    {
        DomElement rootNode;
        int domUpdateVersion;
        EventHandler domUpdatedHandler;

        //foc custom elements 
        Dictionary<string, CreateCssBoxDelegate> registedCustomElemenGens = new Dictionary<string, CreateCssBoxDelegate>();

        public HtmlDocument()
            : this(HtmlPredefineNames.CreateUniqueStringTableClone())
        {
        }
        internal HtmlDocument(UniqueStringTable sharedUniqueStringTable)
            : base(sharedUniqueStringTable)
        {
            //default root
            rootNode = new HtmlRootElement(this);
            //test only
            this.RegisterCustomElement("fivespace", CustomBoxGenSample1.CreateCssBox);
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
        public override DomElement CreateShadowRootElement(string prefix, string localName)
        {
            return new ShadowRootElement(this,
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
            string wrapperElementName,
            LazyCssBoxCreator lazyCssBoxCreator)
        {
            return new ExternalHtmlElement(this,
                AddStringIfNotExists(null),
                AddStringIfNotExists(wrapperElementName),
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