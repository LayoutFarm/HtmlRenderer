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

    public abstract class NewHtmlWindowWidget : NewHtmlWidgetBase
    {
        HtmlBox lightHtmlBox;
        public NewHtmlWindowWidget(int w, int h)
            : base(w, h)
        {

        }

        UIElement GetPrimaryUIElement(HtmlHost htmlhost)
        {
            this.htmlhost = htmlhost;
            if (this.lightHtmlBox == null)
            {
                var lightHtmlBox = new HtmlBox(htmlhost, this.Width, this.Height);
                HtmlDocument htmldoc = htmlhost.CreateNewSharedHtmlDoc();
                myPresentationDom = GetPresentationDomNode(htmldoc);
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

                //get dom/Cssbox  
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

        protected virtual void InvalidateGraphics()
        {
            this.lightHtmlBox.InvalidateGraphics();
        }
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

        public override void SetLocation(int left, int top)
        {
            base.SetLocation(left, top);
            if (this.lightHtmlBox != null)
            {
                lightHtmlBox.SetLocation(left, top);
            }
        }
        internal static void RaiseOnPrimaryUIElementCrated(NewHtmlWindowWidget widget, HtmlHost htmlhost)
        {
            widget.OnPrimaryUIElementCreated(htmlhost);
        }
        protected virtual void OnPrimaryUIElementCreated(HtmlHost htmlhost)
        {

        }
    }

}