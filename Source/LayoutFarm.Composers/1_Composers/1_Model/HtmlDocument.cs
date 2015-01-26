// 2015,2014 ,BSD, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.InternalHtmlDom;
using LayoutFarm.Composers;

namespace LayoutFarm.Composers
{

    public class HtmlDocument : WebDocument
    {
        DomElement rootNode;
        public HtmlDocument()
            : base(HtmlPredefineNames.CreateUniqueStringTableClone())
        {
            //default root
            rootNode = new RootElement(this);
        }
        public override DomElement RootNode
        {
            get
            {
                return rootNode;
            }
            set
            {
                this.rootNode = value;
            }
        }
        public override int DomUpdateVersion
        {
            get;
            set;
        }
        public override DomElement CreateElement(string prefix, string localName)
        {
            return new HtmlElement(this,
                AddStringIfNotExists(prefix),
                AddStringIfNotExists(localName));
        }
        public DomAttribute CreateAttribute(WellknownName attrName)
        {
            return new DomAttribute(this,
                0,
                (int)attrName);
        }
        public override DomTextNode CreateTextNode(char[] strBufferForElement)
        {
            return new HtmlTextNode(this, strBufferForElement);
        }
        internal ActiveCssTemplate ActiveCssTemplate
        {
            get;
            set;
        }
    }


}