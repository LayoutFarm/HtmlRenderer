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

        public abstract UIElement GetPrimaryUIElement(HtmlHost htmlhost);

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

        protected void RaiseEventLayoutFinished()
        {
            if (this.LayoutFinished != null)
            {
                this.LayoutFinished(this, EventArgs.Empty);
            }
        }

    }

    public abstract class LightHtmlWidgetBase : WidgetBase
    {

        LightHtmlBox lightHtmlBox; //primary ui element


        public LightHtmlWidgetBase(int w, int h)
            : base(w, h)
        {

        }
        public override UIElement GetPrimaryUIElement(HtmlHost htmlhost)
        {
            if (this.lightHtmlBox == null)
            {

                var lightHtmlBox = new LightHtmlBox(htmlhost, this.Width, this.Height);
                FragmentHtmlDocument htmldoc = htmlhost.CreateNewFragmentHtml();
                var presentationDom = GetPresentationDomNode(htmldoc.RootNode);
                if (presentationDom != null)
                {
                    htmldoc.RootNode.AddChild(presentationDom);
                    lightHtmlBox.LoadHtmlDom(htmldoc);
                }

                lightHtmlBox.SetLocation(this.Left, this.Top);

                lightHtmlBox.LayoutFinished += (s, e) => this.RaiseEventLayoutFinished();

                this.lightHtmlBox = lightHtmlBox;
                //first time
                OnPrimaryUIElementCrated(htmlhost);
            }
            return this.lightHtmlBox;
        }
        public RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            return lightHtmlBox.GetPrimaryRenderElement(rootgfx);
        }
        protected virtual void OnPrimaryUIElementCrated(HtmlHost htmlhost)
        {

        }
        protected UIElement CurrentPrimaryUIElement
        {
            get { return this.lightHtmlBox; }
        }

        public HtmlHost HtmlHost
        {
            get
            {
                return lightHtmlBox.HtmlHost;
            }
        }
        protected void InvalidateGraphics()
        {
            this.lightHtmlBox.InvalidateGraphics();
        }

        protected abstract WebDom.DomElement GetPresentationDomNode(WebDom.DomElement hostNode);
        //------------------------------------------

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

    }
}