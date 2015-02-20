// 2015,2014 ,MIT, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;


using LayoutFarm;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.Composers;
namespace LayoutFarm.Ease
{

    public struct EaseDomElement
    {
        EaseScriptElement easeScriptElement;
        public EaseDomElement(WebDom.DomElement domElement)
        {
            this.easeScriptElement = new EaseScriptElement(domElement);
        }
        public void SetBackgroundColor(System.Drawing.Color c)
        {
            this.easeScriptElement.ChangeBackgroundColor(new Color(c.A, c.R, c.G, c.B));
        }
    }
    public struct EaseCanvas
    {
        Canvas canvas;
        internal EaseCanvas(Canvas canvas)
        {
            this.canvas = canvas;
        }
        internal Canvas Canvas
        {
            get { return this.canvas; }
        }
    }
}