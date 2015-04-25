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

    public abstract class WidgetBase : IScrollable
    {
        int width;
        int height;
        int left;
        int top;
        int viewportX;
        int viewportY;
        public event EventHandler LayoutFinished;

        public WidgetBase(int w, int h)
        {
            this.width = w;
            this.height = h;
        }
        public int Width
        {
            get { return this.width; }
        }
        public int Height
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

        //public abstract UIElement GetPrimaryUIElement(HtmlHost htmlhost);

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
        public virtual int DesiredWidth { get { return this.Width; } }
        public virtual int DesiredHeight { get { return this.height; } }

        internal void RaiseEventLayoutFinished()
        {
            if (this.LayoutFinished != null)
            {
                this.LayoutFinished(this, EventArgs.Empty);
            }
        }
    }

    public sealed class WidgetHolder
    {
        HtmlBox lightHtmlBox;
        LightHtmlWidgetBase widget;

        public WidgetHolder(LightHtmlWidgetBase widget)
        {
            this.widget = widget;
        }
        public UIElement GetPrimaryUIElement(HtmlHost htmlhost)
        {
            if (this.lightHtmlBox == null)
            {

                var lightHtmlBox = new HtmlBox(htmlhost, widget.Width, widget.Height);
                HtmlDocument htmldoc = htmlhost.CreateNewSharedHtmlDoc();
                var presentationDom = widget.GetPresentationDomNode(htmldoc.RootNode);
                if (presentationDom != null)
                {
                    htmldoc.RootNode.AddChild(presentationDom);
                    lightHtmlBox.LoadHtmlDom(htmldoc);
                }

                lightHtmlBox.SetLocation(widget.Left, widget.Top);
                lightHtmlBox.LayoutFinished += (s, e) => widget.RaiseEventLayoutFinished();

                this.lightHtmlBox = lightHtmlBox;
                //first time
                LightHtmlWidgetBase.RaiseOnPrimaryUIElementCrated(widget, htmlhost);
            }
            return this.lightHtmlBox;
        }

    }

    public abstract class LightHtmlWidgetBase : WidgetBase
    {

        DomElement myPresentationDom;
        HtmlBox lightHtmlBox; //primary ui element

        HtmlHost htmlhost;
        public LightHtmlWidgetBase(int w, int h)
            : base(w, h)
        {
        }
        public UIElement GetPrimaryUIElement(HtmlHost htmlhost)
        {
            this.htmlhost = htmlhost;
            if (this.lightHtmlBox == null)
            {

                var lightHtmlBox = new HtmlBox(htmlhost, this.Width, this.Height);
                HtmlDocument htmldoc = htmlhost.CreateNewSharedHtmlDoc();
                myPresentationDom = GetPresentationDomNode(htmldoc.RootNode);
                if (myPresentationDom != null)
                {
                    htmldoc.RootNode.AddChild(myPresentationDom);
                    lightHtmlBox.LoadHtmlDom(htmldoc);
                }

                lightHtmlBox.SetLocation(this.Left, this.Top);
                lightHtmlBox.LayoutFinished += (s, e) => this.RaiseEventLayoutFinished();

                this.lightHtmlBox = lightHtmlBox;
                //first time
                OnPrimaryUIElementCreated(htmlhost);
            }
            return this.lightHtmlBox;
        }

        protected void AddSelfToTopWindow()
        {
            var htmlhost = this.HtmlHost;
            if (htmlhost == null) return;

            var topWindow = htmlhost.TopWindowRenderBox;
            if (topWindow != null)
            {
                var primUI = this.GetPrimaryUIElement(htmlhost) as HtmlBox;
                if (this.myPresentationDom != null)
                {
                    var parent = myPresentationDom.ParentNode as IHtmlElement;
                    if (parent == null)
                    {
                        var htmldoc = primUI.HtmlContainer.WebDocument as HtmlDocument;
                        htmldoc.RootNode.AddChild(myPresentationDom);
                    }
                }
                topWindow.AddChild(primUI);
            }
        }
        protected void RemoveSelfFromTopWindow()
        {
            //TODO: review here again 
            if (lightHtmlBox != null)
            {
                RenderElement currentRenderE = lightHtmlBox.CurrentPrimaryRenderElement;

                if (currentRenderE != null)
                {
                    var topRenderBox = currentRenderE.GetTopWindowRenderBox();
                    //var topRenderBox = currentRenderE.ParentRenderElement as TopWindowRenderBox;
                    if (topRenderBox != null)
                    {
                        topRenderBox.RemoveChild(currentRenderE);
                    }
                }
            }
        }
        protected virtual void OnPrimaryUIElementCreated(HtmlHost htmlhost)
        {

        }

        public HtmlHost HtmlHost
        {
            get
            {

                return htmlhost;
            }
        }
        protected void InvalidateGraphics()
        {
            this.lightHtmlBox.InvalidateGraphics();
        }

        public abstract WebDom.DomElement GetPresentationDomNode(WebDom.DomElement hostNode);
        public override void SetViewport(int x, int y)
        {
            base.SetViewport(x, y);
            if (this.lightHtmlBox != null)
            {
                lightHtmlBox.SetViewport(x, y);
            }
        }
        public override int DesiredHeight
        {
            get
            {
                if (this.lightHtmlBox != null)
                {
                    return this.lightHtmlBox.DesiredHeight;
                }
                return base.DesiredHeight;
            }
        }

        internal static void RaiseOnPrimaryUIElementCrated(LightHtmlWidgetBase widget, HtmlHost htmlhost)
        {
            widget.OnPrimaryUIElementCreated(htmlhost);
        }
        public override void SetLocation(int left, int top)
        {
            base.SetLocation(left, top);
            if (this.lightHtmlBox != null)
            {
                lightHtmlBox.SetLocation(left, top);
            }
        }
    }
}