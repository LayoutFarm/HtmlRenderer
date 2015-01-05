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

    public class LightHtmlBox : UIBox, IUserEventPortal
    {
        bool hasWaitingDocToLoad;
        string waitingHtmlFragment;

        LightHtmlBoxHost lightBoxHost;


        //presentation
        HtmlFragmentRenderBox frgmRenderBox;

        object uiHtmlTask = new object();


        internal LightHtmlBox(LightHtmlBoxHost lightBoxHost, int width, int height)
            : base(width, height)
        {
            this.lightBoxHost = lightBoxHost;

        }

        HtmlInputEventAdapter _htmlInputEventBridge { get { return this.lightBoxHost.InputEventAdaper; } }

        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.frgmRenderBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.frgmRenderBox != null; }
        }
        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            _htmlInputEventBridge.MouseUp(e, frgmRenderBox.CssBox);
        }
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            e.CurrentContextElement = this;
            _htmlInputEventBridge.MouseDown(e, frgmRenderBox.CssBox);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            _htmlInputEventBridge.MouseMove(e, frgmRenderBox.CssBox);
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {

        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            _htmlInputEventBridge.KeyDown(e, frgmRenderBox.CssBox);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            _htmlInputEventBridge.KeyPress(e, frgmRenderBox.CssBox);
        }
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            _htmlInputEventBridge.KeyUp(e, frgmRenderBox.CssBox);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return this._htmlInputEventBridge.ProcessDialogKey(e, frgmRenderBox.CssBox);
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

            if (frgmRenderBox == null)
            {
                var newFrRenderBox = new HtmlFragmentRenderBox(rootgfx, this.Width, this.Height);
                newFrRenderBox.SetController(this);
                newFrRenderBox.HasSpecificSize = true;
                newFrRenderBox.SetLocation(this.Left, this.Top);
                //set to this field if ready
                this.frgmRenderBox = newFrRenderBox;
            }
            if (this.hasWaitingDocToLoad)
            {
                LoadHtmlFragmentText(this.waitingHtmlFragment);
            }
            return frgmRenderBox;
        }
        public void LoadHtmlFragmentText(string htmlFragment)
        {
            if (frgmRenderBox == null)
            {
                this.hasWaitingDocToLoad = true;
                this.waitingHtmlFragment = htmlFragment;
            }
            else
            {
                //just parse content and load
                MyHtmlIsland newIsland;
                CssBox newCssBox;
                this.lightBoxHost.CreateHtmlFragment(htmlFragment, frgmRenderBox, out newIsland, out newCssBox);
                this.frgmRenderBox.SetHtmlIsland(newIsland, newCssBox);
                
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





