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
        BridgeHtmlDocument waitingHtmlDomFragment;

        LightHtmlBoxHost lightBoxHost;
        MyHtmlIsland myHtmlIsland;
        CssBox myCssBox;

        //presentation
        HtmlFragmentRenderBox frgmRenderBox;

        object uiHtmlTask = new object();


        internal LightHtmlBox(LightHtmlBoxHost lightBoxHost, int width, int height)
            : base(width, height)
        {
            this.lightBoxHost = lightBoxHost;

        }
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
            //0. set context
            e.CurrentContextElement = this;

            //1. get share input adapter
            var inputAdapter = this.lightBoxHost.GetSharedInputEventAdapter(this.myHtmlIsland);
            //2. send event
            inputAdapter.MouseUp(e, frgmRenderBox.CssBox);
            //3. release back to host
            this.lightBoxHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.lightBoxHost.GetSharedInputEventAdapter(this.myHtmlIsland);

            inputAdapter.MouseDown(e, frgmRenderBox.CssBox);

            this.lightBoxHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {   //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.lightBoxHost.GetSharedInputEventAdapter(this.myHtmlIsland);

            inputAdapter.MouseMove(e, frgmRenderBox.CssBox);

            this.lightBoxHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            var inputAdapter = this.lightBoxHost.GetSharedInputEventAdapter(this.myHtmlIsland);
            //?
            this.lightBoxHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.lightBoxHost.GetSharedInputEventAdapter(this.myHtmlIsland);

            inputAdapter.KeyDown(e, frgmRenderBox.CssBox);

            this.lightBoxHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            var inputAdapter = this.lightBoxHost.GetSharedInputEventAdapter(this.myHtmlIsland);
            inputAdapter.KeyPress(e, frgmRenderBox.CssBox);
            this.lightBoxHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            var inputAdapter = this.lightBoxHost.GetSharedInputEventAdapter(this.myHtmlIsland);

            inputAdapter.KeyUp(e, frgmRenderBox.CssBox);

            this.lightBoxHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.lightBoxHost.GetSharedInputEventAdapter(this.myHtmlIsland);
            inputAdapter.KeyUp(e, frgmRenderBox.CssBox);
            var result = inputAdapter.ProcessDialogKey(e, frgmRenderBox.CssBox);
            this.lightBoxHost.ReleaseSharedInputEventAdapter(inputAdapter);
            return result;
        }
        void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
        }
        void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
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

                this.lightBoxHost.CreateHtmlFragment(htmlFragment, frgmRenderBox, out myHtmlIsland, out myCssBox);
                this.frgmRenderBox.SetHtmlIsland(myHtmlIsland, myCssBox);
                this.waitingHtmlFragment = null;
            }
            //send fragment html to lightbox host 
            //SetHtml(myHtmlIsland, html, myHtmlIsland.BaseStylesheet);
            //if (this.myCssBoxWrapper != null)
            //{
            //    myCssBoxWrapper.InvalidateGraphic();
            //}
        }
        public void LoadHtmlFragmentDom(BridgeHtmlDocument htmldoc)
        {
            if (frgmRenderBox == null)
            {
                this.hasWaitingDocToLoad = true;
                this.waitingHtmlDomFragment = htmldoc;
            }
            else
            {
                //just parse content and load 
                this.lightBoxHost.CreateHtmlFragment(htmldoc, frgmRenderBox, out myHtmlIsland, out myCssBox);
                this.frgmRenderBox.SetHtmlIsland(myHtmlIsland, myCssBox);

                myHtmlIsland.Refresh += myHtmlIsland_Refresh;
                myHtmlIsland.NeedUpdateDom += myHtmlIsland_NeedUpdateDom;
                myHtmlIsland.RequestResource += myHtmlIsland_RequestResource;

                this.waitingHtmlDomFragment = null;
            }
        }
        void myHtmlIsland_RequestResource(object sender, HtmlResourceRequestEventArgs e)
        {
            //send request image to LightBoxHost
            this.lightBoxHost.ChildRequestImage(this, new ImageRequestEventArgs(e.binder));
        }
        void myHtmlIsland_NeedUpdateDom(object sender, EventArgs e)
        {
            hasWaitingDocToLoad = true;
            //---------------------------
            if (frgmRenderBox == null) return;
            //--------------------------- 
            
            this.lightBoxHost.RefreshCssTree(myHtmlIsland.Document);

            var lay = this.lightBoxHost.GetSharedHtmlLayoutVisitor(myHtmlIsland);
            myHtmlIsland.PerformLayout(lay);
            this.lightBoxHost.ReleaseHtmlLayoutVisitor(lay);

        }
        /// <summary>
        /// Handle html renderer invalidate and re-layout as requested.
        /// </summary>
        void myHtmlIsland_Refresh(object sender, HtmlRenderer.WebDom.HtmlRefreshEventArgs e)
        {
            this.InvalidateGraphic();
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





