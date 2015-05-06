// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
using LayoutFarm.CustomWidgets;
using LayoutFarm.Composers;

namespace LayoutFarm.HtmlWidgets
{

    public abstract class WidgetBase
    {
        int width;
        int height;
        int left;
        int top;
        int viewportX;
        int viewportY;
        public WidgetBase(int w, int h)
        {
            this.width = w;
            this.height = h;
        }
        public virtual int Width
        {
            get { return this.width; }
        }
        public virtual int Height
        {
            get { return this.height; }
        }
        public int Left
        {
            get { return this.left; }
        }
        public int Top
        {
            get { return this.top; }
        }
        public virtual void SetLocation(int left, int top)
        {
            this.left = left;
            this.top = top;
        }
        public virtual void SetSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public virtual void SetViewport(int x, int y)
        {
            this.viewportX = x;
            this.viewportY = y;
        }
        public int ViewportX { get { return this.viewportX; } }
        public int ViewportY { get { return this.viewportY; } }
        public int ViewportWidth { get { return this.Width; } }
        public int ViewportHeight { get { return this.Height; } }
    }
    public abstract class HtmlWidgetBase : WidgetBase
    {
        public HtmlWidgetBase(int w, int h)
            : base(w, h)
        {
        }
        public abstract WebDom.DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc);

        //public UIElement GetPrimaryUIElement(HtmlHost htmlhost)
        //{
        //    this.htmlhost = htmlhost;
        //    if (this.lightHtmlBox == null)
        //    {

        //        var lightHtmlBox = new HtmlBox(htmlhost, this.Width, this.Height);
        //        HtmlDocument htmldoc = htmlhost.CreateNewSharedHtmlDoc();
        //        myPresentationDom = GetPresentationDomNode(htmldoc);
        //        if (myPresentationDom != null)
        //        {
        //            htmldoc.RootNode.AddChild(myPresentationDom);
        //            lightHtmlBox.LoadHtmlDom(htmldoc);
        //        }

        //        lightHtmlBox.SetLocation(this.Left, this.Top);
        //        lightHtmlBox.LayoutFinished += (s, e) => this.RaiseEventLayoutFinished();

        //        this.lightHtmlBox = lightHtmlBox;

        //    }
        //    return this.lightHtmlBox;
        //}

    }
}