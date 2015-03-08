// 2015,2014 ,BSD, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;

using LayoutFarm.Scripting;
namespace LayoutFarm.WebDom
{
    //only for scripting
    public interface IHtmlDocument
    {
        HtmlElement getElementById(string id);
        HtmlTextNode createTextNode(string textContent);
        HtmlElement createElement(string nodeName);

    }

    partial class HtmlDocument : IHtmlDocument
    {
        [JsMethod]
        HtmlElement IHtmlDocument.getElementById(string id)
        {
            return this.GetElementById(id) as HtmlElement;
        }
        [JsMethod]
        HtmlTextNode IHtmlDocument.createTextNode(string content)
        {
            return (HtmlTextNode)this.CreateTextNode(content.ToCharArray());
        }
        [JsMethod]
        HtmlElement IHtmlDocument.createElement(string nodeName)
        {
            return (HtmlElement)this.CreateElement(nodeName);
        }


    }
    //---------------------------------------------------------
    public interface IHtmlElement
    {
        //string innerHtml { get; set; } 
        void setAttribute(string attrName, string value);
        void appendChild(DomNode childNode);
    }

    partial class HtmlElement : IHtmlElement
    {
        [JsMethod]
        void IHtmlElement.setAttribute(string attrName, string value)
        {
            this.SetAttribute(attrName, value);
        }
        [JsMethod]
        void IHtmlElement.appendChild(DomNode childNode)
        {
            this.AddChild(childNode);
        }
    }


}