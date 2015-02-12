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

        public void SetLocation(int left, int top)
        {
            this.left = left;
            this.top = top;
        }
    }

    public abstract class LightHtmlWidgetBase : WidgetBase, IScrollable
    {

        LightHtmlBox lightHtmlBox;
        HtmlHost myHtmlHost;
        int viewportX;
        int viewportY;
        public event EventHandler LayoutFinished;

        public LightHtmlWidgetBase(int w, int h)
            : base(w, h)
        {

        }
        public override UIElement GetPrimaryUIElement(HtmlHost htmlhost)
        {
            if (this.lightHtmlBox == null)
            {
                this.myHtmlHost = htmlhost;
                var lightHtmlBox = new LightHtmlBox(htmlhost, this.Width, this.Height);
                FragmentHtmlDocument htmldoc = htmlhost.CreateNewFragmentHtml();
                var presentationDom = GetPresentationDomNode(htmldoc.RootNode);
                if (presentationDom != null)
                {
                    htmldoc.RootNode.AddChild(presentationDom);
                    lightHtmlBox.LoadHtmlDom(htmldoc);
                }
                lightHtmlBox.SetLocation(this.Left, this.Top);
                lightHtmlBox.LayoutFinished += (s, e) =>
                {
                    if (LayoutFinished != null)
                    {
                        this.LayoutFinished(this, EventArgs.Empty);
                    }

                };
                this.lightHtmlBox = lightHtmlBox;
            }
            return this.lightHtmlBox;
        }
        protected HtmlHost HtmlHost
        {
            get { return this.myHtmlHost; }
        }
        protected void InvalidateGraphics()
        {
            this.lightHtmlBox.InvalidateGraphics();
        }
        protected abstract WebDom.DomElement GetPresentationDomNode(WebDom.DomElement hostNode);

        //------------------------------------------
        public void SetViewport(int x, int y)
        {
            this.viewportX = x;
            this.viewportY = y;
            lightHtmlBox.SetViewport(x, y);
        }
        public int ViewportX
        {
            get { return this.viewportX; }

        }

        public int ViewportY
        {
            get { return this.viewportY; }
        }
        public int ViewportWidth
        {
            get { return this.Width; }
        }
        public int ViewportHeight
        {
            get { return this.Height; }
        }
        public int DesiredWidth
        {
            get
            {
                return this.Width;
            }
        }
        public int DesiredHeight
        {
            get
            {
                if (this.lightHtmlBox != null)
                {
                    return this.lightHtmlBox.DesiredHeight;
                }
                else
                {
                    return this.Height;
                }

            }
        }
    }
}