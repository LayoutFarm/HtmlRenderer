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

    public abstract class OldWidgetBase : IScrollable
    {
        int width;
        int height;
        int left;
        int top;
        int viewportX;
        int viewportY;
        public event EventHandler LayoutFinished;

        public OldWidgetBase(int w, int h)
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

<<<<<<< HEAD
    sealed class WidgetHolder
    {

        LightHtmlBoxWidgetBase widget;
        public WidgetHolder(LightHtmlBoxWidgetBase widget)
        {
            this.widget = widget;
        }
        //public UIElement GetPrimaryUIElement(HtmlHost htmlhost)
        //{
        //    if (this.lightHtmlBox == null)
        //    {

        //        var lightHtmlBox = new HtmlBox(htmlhost, widget.Width, widget.Height);
        //        HtmlDocument htmldoc = htmlhost.CreateNewSharedHtmlDoc();
        //        var presentationDom = widget.GetPresentationDomNode(htmldoc);
        //        if (presentationDom != null)
        //        {
        //            htmldoc.RootNode.AddChild(presentationDom);
        //            lightHtmlBox.LoadHtmlDom(htmldoc);
        //        }

        //        lightHtmlBox.SetLocation(widget.Left, widget.Top);
        //        lightHtmlBox.LayoutFinished += (s, e) => widget.RaiseEventLayoutFinished();

        //        this.lightHtmlBox = lightHtmlBox;
        //        //first time
        //        LightHtmlBoxWidgetBase.RaiseOnPrimaryUIElementCrated(widget, htmlhost);
        //    }
        //    return this.lightHtmlBox;
        //}

        public CssBox CreateCssBox(HtmlHost htmlhost, Css.BoxSpec spec)
        {
            HtmlDocument htmldoc = htmlhost.CreateNewSharedHtmlDoc();
            DomElement domE = widget.GetPresentationDomNode(htmldoc);

            //create cssbox for the domE

            return null;
        }

    }

    public abstract class LightHtmlBoxWidgetBase : WidgetBase
    {

        CssBox cssbox;
        HtmlHost htmlhost;
        public LightHtmlBoxWidgetBase(int w, int h)
=======
    //sealed class WidgetHolder
    //{
    //    HtmlBox lightHtmlBox;
    //    LightHtmlWidgetBase widget;

    //    public WidgetHolder(LightHtmlWidgetBase widget)
    //    {
    //        this.widget = widget;
    //    }
    //    public UIElement GetPrimaryUIElement(HtmlHost htmlhost)
    //    {
    //        if (this.lightHtmlBox == null)
    //        {

    //            var lightHtmlBox = new HtmlBox(htmlhost, widget.Width, widget.Height);
    //            HtmlDocument htmldoc = htmlhost.CreateNewSharedHtmlDoc();
    //            var presentationDom = widget.GetPresentationDomNode(htmldoc.RootNode);
    //            if (presentationDom != null)
    //            {
    //                htmldoc.RootNode.AddChild(presentationDom);
    //                lightHtmlBox.LoadHtmlDom(htmldoc);
    //            }

    //            lightHtmlBox.SetLocation(widget.Left, widget.Top);
    //            lightHtmlBox.LayoutFinished += (s, e) => widget.RaiseEventLayoutFinished();

    //            this.lightHtmlBox = lightHtmlBox;
    //            //first time
    //            LightHtmlWidgetBase.RaiseOnPrimaryUIElementCrated(widget, htmlhost);
    //        }
    //        return this.lightHtmlBox;
    //    }

    //}

    public abstract class OldHtmlWidgetBase : OldWidgetBase
    {

        DomElement myPresentationDom;
        HtmlBox lightHtmlBox; //primary ui element 
        HtmlHost htmlhost;
        public OldHtmlWidgetBase(int w, int h)
>>>>>>> v_widget2
            : base(w, h)
        {
        }
        public abstract DomElement GetPresentationDomNode(HtmlDocument htmldoc);
        internal CssBox CssBox
        {
            get { return this.cssbox; }
            set { this.cssbox = value; }
        }
        //public UIElement GetPrimaryUIElement(HtmlHost htmlhost)
        //{
        //    this.htmlhost = htmlhost;
        //    if (this.lightHtmlBox == null)
        //    {

<<<<<<< HEAD
        //        var lightHtmlBox = new HtmlBox(htmlhost, this.Width, this.Height);
        //        HtmlDocument htmldoc = htmlhost.CreateNewSharedHtmlDoc();
        //        myPresentationDom = GetPresentationDomNode(htmldoc);
        //        if (myPresentationDom != null)
        //        {
        //            htmldoc.RootNode.AddChild(myPresentationDom);
        //            lightHtmlBox.LoadHtmlDom(htmldoc);
        //        }
=======
                var lightHtmlBox = new HtmlBox(htmlhost, this.Width, this.Height);
                HtmlDocument htmldoc = htmlhost.CreateNewSharedHtmlDoc();
                myPresentationDom = GetPresentationDomNode(htmldoc);
                if (myPresentationDom != null)
                {
                    htmldoc.RootNode.AddChild(myPresentationDom);
                    lightHtmlBox.LoadHtmlDom(htmldoc);
                }
>>>>>>> v_widget2

        //        lightHtmlBox.SetLocation(this.Left, this.Top);
        //        lightHtmlBox.LayoutFinished += (s, e) => this.RaiseEventLayoutFinished();

<<<<<<< HEAD
        //        this.lightHtmlBox = lightHtmlBox;
        //        //first time
        //        OnPrimaryUIElementCreated(htmlhost);
        //    }
        //    return this.lightHtmlBox;
        //}

        //protected void AddSelfToTopWindow()
        //{
        //    var htmlhost = this.HtmlHost;
        //    if (htmlhost == null) return;
=======
                this.lightHtmlBox = lightHtmlBox;
                //first time
                OnPrimaryUIElementCreated(htmlhost);
            }
            return this.lightHtmlBox;
        }
        public abstract WebDom.DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc);
        protected void AddSelfToTopWindow()
        {
            var htmlhost = this.HtmlHost;
            if (htmlhost == null) return;
>>>>>>> v_widget2

        //    var topWindow = htmlhost.TopWindowRenderBox;
        //    if (topWindow != null)
        //    {
        //        var primUI = this.GetPrimaryUIElement(htmlhost) as HtmlBox;
        //        if (this.myPresentationDom != null)
        //        {
        //            var parent = myPresentationDom.ParentNode as IHtmlElement;
        //            if (parent == null)
        //            {
        //                var htmldoc = primUI.HtmlContainer.WebDocument as HtmlDocument;
        //                htmldoc.RootNode.AddChild(myPresentationDom);
        //            }
        //        }
        //        topWindow.AddChild(primUI);
        //    }
        //}
        //protected void RemoveSelfFromTopWindow()
        //{
        //    //TODO: review here again 
        //    if (lightHtmlBox != null)
        //    {
        //        RenderElement currentRenderE = lightHtmlBox.CurrentPrimaryRenderElement;

        //        if (currentRenderE != null)
        //        {
        //            var topRenderBox = currentRenderE.GetTopWindowRenderBox();
        //            //var topRenderBox = currentRenderE.ParentRenderElement as TopWindowRenderBox;
        //            if (topRenderBox != null)
        //            {
        //                topRenderBox.RemoveChild(currentRenderE);
        //            }
        //        }
        //    }
        //}

        protected virtual void OnPrimaryUIElementCreated(HtmlHost htmlhost)
        {

        }

        public HtmlHost HtmlHost
        {
            get
            {

                return htmlhost;
            }
            set
            {
                this.htmlhost = value;
            }
        }
        protected void InvalidateGraphics()
        {
            cssbox.InvalidateGraphics();
            //this.lightHtmlBox.InvalidateGraphics();
        }

<<<<<<< HEAD

=======
     
>>>>>>> v_widget2
        public override void SetViewport(int x, int y)
        {
            base.SetViewport(x, y);
            if (this.cssbox != null)
            {
                this.cssbox.SetViewport(x, y);
            }
        }
        public override int DesiredHeight
        {
            get
            {
                if (this.cssbox != null)
                {
                    return (int)this.cssbox.InnerContentHeight;
                }
                //if (this.lightHtmlBox != null)
                //{
                //    return this.lightHtmlBox.DesiredHeight;
                // }
                return base.DesiredHeight;
            }
        }

<<<<<<< HEAD
        internal static void RaiseOnPrimaryUIElementCrated(LightHtmlBoxWidgetBase widget, HtmlHost htmlhost)
=======
        internal static void RaiseOnPrimaryUIElementCrated(OldHtmlWidgetBase widget, HtmlHost htmlhost)
>>>>>>> v_widget2
        {
            widget.OnPrimaryUIElementCreated(htmlhost);
        }
        public override void SetLocation(int left, int top)
        {
            base.SetLocation(left, top);
            //TODO: review here, can we freely move cssbox?, 
            //it should depennd on its context (line,abs)
            if (this.cssbox != null)
            {
                cssbox.SetLocation(left, top);
            }
            //if (this.lightHtmlBox != null)
            //{
            //    lightHtmlBox.SetLocation(left, top);
            //}
        }
    }
}