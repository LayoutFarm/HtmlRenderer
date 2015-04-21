//2015 MIT, WinterDev  
using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;

using LayoutFarm.Scripting;
namespace LayoutFarm.WebDom
{

    public interface IHtmlDocument
    {
        HtmlElement getElementById(string id);
        HtmlTextNode createTextNode(object nodeContent);
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
        HtmlTextNode IHtmlDocument.createTextNode(object nodeContent)
        {
            return (HtmlTextNode)this.CreateTextNode(nodeContent.ToString().ToCharArray());
        }
        [JsMethod]
        HtmlElement IHtmlDocument.createElement(string nodeName)
        {
            return (HtmlElement)this.CreateElement(nodeName);
        }
    }

}