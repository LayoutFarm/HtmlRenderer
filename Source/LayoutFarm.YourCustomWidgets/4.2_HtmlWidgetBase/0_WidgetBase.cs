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

    public abstract class LightHtmlWidgetBase : WidgetBase
    {
        HtmlHost htmlhost;
        LightHtmlBox lightHtmlBox;
        public LightHtmlWidgetBase(HtmlHost htmlhost, int w, int h)
            : base(w, h)
        {
            this.htmlhost = htmlhost;
        }
        public override UIElement GetPrimaryUIElement(HtmlHost htmlhost)
        {
            if (this.lightHtmlBox == null)
            {
                var lightHtmlBox = new LightHtmlBox(htmlhost, this.Width, this.Height);

                lightHtmlBox.LoadHtmlDom(CreatePresentationDom());
                lightHtmlBox.SetLocation(this.Left, this.Top);
                this.lightHtmlBox = lightHtmlBox;
            }
            return this.lightHtmlBox;
        }
        protected HtmlHost HtmlHost
        {
            get { return this.htmlhost; }
        }
        protected void InvalidateGraphics()
        {
            this.lightHtmlBox.InvalidateGraphics();
        }
        protected abstract FragmentHtmlDocument CreatePresentationDom();
    }
}