//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HtmlRenderer;
using HtmlRenderer.ContentManagers;
using HtmlRenderer.Boxes;
using HtmlRenderer.Composers;

using LayoutFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.Boxes;

namespace LayoutFarm.CustomWidgets
{

    public class LightHtmlBox : UIElement, IUserEventPortal
    {
        bool hasWaitingDocToLoad;
        string waitingHtmlFragment;

        LightHtmlBoxHost lightBoxHost;
        int _width;
        int _height;

        //presentation
        HtmlFragmentRenderBox frRenderBox;
        object uiHtmlTask = new object();


        internal LightHtmlBox(LightHtmlBoxHost lightBoxHost, int width, int height)
        {
            this.lightBoxHost = lightBoxHost;
            this._width = width;
            this._height = height;
        }

        HtmlInputEventAdapter _htmlInputEventBridge { get { return this.lightBoxHost.InputEventAdaper; } }

        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            _htmlInputEventBridge.MouseUp(e, frRenderBox.CssBox);
        }
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            e.CurrentContextElement = this;
            _htmlInputEventBridge.MouseDown(e, frRenderBox.CssBox);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            _htmlInputEventBridge.MouseMove(e, frRenderBox.CssBox);
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {

        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            _htmlInputEventBridge.KeyDown(e, frRenderBox.CssBox);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            _htmlInputEventBridge.KeyPress(e, frRenderBox.CssBox);
        }
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            _htmlInputEventBridge.KeyUp(e, frRenderBox.CssBox);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return this._htmlInputEventBridge.ProcessDialogKey(e, frRenderBox.CssBox);
        }
        void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
        }
        void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {

            if (frRenderBox == null)
            {
                frRenderBox = new HtmlFragmentRenderBox(rootgfx, _width, _height);
                frRenderBox.SetController(this);
                frRenderBox.HasSpecificSize = true;
            }
            if (this.hasWaitingDocToLoad)
            {
                LoadHtmlFragmentText(this.waitingHtmlFragment);
            }
            return frRenderBox;
        }
        public void LoadHtmlFragmentText(string htmlFragment)
        {
            if (frRenderBox == null)
            {
                this.hasWaitingDocToLoad = true;
                this.waitingHtmlFragment = htmlFragment;
            }
            else
            {
                //just parse content and load
                MyHtmlIsland newIsland;
                CssBox newCssBox;
                this.lightBoxHost.CreateHtmlFragment(htmlFragment, frRenderBox, out newIsland, out newCssBox);
                this.frRenderBox.SetHtmlIsland(newIsland, newCssBox);
                this.waitingHtmlFragment = null;
            }
            //send fragment html to lightbox host 
            //SetHtml(myHtmlIsland, html, myHtmlIsland.BaseStylesheet);
            //if (this.myCssBoxWrapper != null)
            //{
            //    myCssBoxWrapper.InvalidateGraphic();
            //}
        }
        public void LoadHtmlFragmentDom()
        {


        }
        public override void InvalidateGraphic()
        {
            //if (this.myCssBoxWrapper != null)
            //{
            //    myCssBoxWrapper.InvalidateGraphic();
            //}
        }

    }
}





