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
        LightHtmlBoxHost lightBoxHost;
        int _width;
        int _height;

        //presentation
        HtmlFragmentRenderBox frRenderBox;
        object uiHtmlTask = new object();

        RootGraphic rootgfx;
        internal LightHtmlBox(LightHtmlBoxHost lightBoxHost, int width, int height)
        {
            this.lightBoxHost = lightBoxHost;
            this._width = width;
            this._height = height;
        }

        HtmlInputEventAdapter _htmlInputEventBridge { get { return this.lightBoxHost.InputEventAdaper; } }

        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {

            _htmlInputEventBridge.MouseUp(e);
        }
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            e.CurrentContextElement = this;
            _htmlInputEventBridge.MouseDown(e);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            _htmlInputEventBridge.MouseMove(e);
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {

        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            _htmlInputEventBridge.KeyDown(e);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            _htmlInputEventBridge.KeyPress(e);
        }
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            _htmlInputEventBridge.KeyUp(e);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return this._htmlInputEventBridge.ProcessDialogKey(e);
        }
        void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
        }
        void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {

            if (frRenderBox == null)
            {
                this.rootgfx = rootgfx;
                frRenderBox = new HtmlFragmentRenderBox(rootgfx, _width, _height);
                frRenderBox.SetController(this);
                frRenderBox.HasSpecificSize = true;
            }
            //-------------------------
            //rootgfx.RequestGraphicsIntervalTask(uiHtmlTask,
            //     TaskIntervalPlan.Animation, 25,
            //     (s, e) =>
            //     {
            //         if (this.myHtmlIsland.InternalRefreshRequest())
            //         {
            //             e.NeedUpdate = 1;
            //         }
            //     });
            //------------------------- 
            if (this.hasWaitingDocToLoad)
            {
                UpdateWaitingHtmlDoc(this.frRenderBox.Root);
            }
            return frRenderBox;
        }
        public void LoadHtmlFragmentText(string html)
        {
            //send fragment html to lightbox host

            //SetHtml(myHtmlIsland, html, myHtmlIsland.BaseStylesheet);

            //if (this.myCssBoxWrapper != null)
            //{
            //    myCssBoxWrapper.InvalidateGraphic();
            //}
        }
        public override void InvalidateGraphic()
        {
            //if (this.myCssBoxWrapper != null)
            //{
            //    myCssBoxWrapper.InvalidateGraphic();
            //}
        }
        void UpdateWaitingHtmlDoc(RootGraphic rootgfx)
        {
            //var builder = new HtmlRenderer.Composers.RenderTreeBuilder(rootgfx);
            //builder.RequestStyleSheet += (e) =>
            //{
            //    if (this.RequestStylesheet != null)
            //    {
            //        var req = new TextLoadRequestEventArgs(e.Src);
            //        RequestStylesheet(this, req);
            //        e.SetStyleSheet = req.SetStyleSheet;
            //    }
            //};

            ////build rootbox from htmldoc
            //var rootBox = builder.BuildCssRenderTree(this.currentdoc,
            //    rootgfx.SampleIFonts,
            //    this.myHtmlIsland,
            //    this.waitingCssData,
            //    this.myCssBoxWrapper);

            ////update htmlIsland
            //var htmlIsland = this.myHtmlIsland;
            //htmlIsland.SetHtmlDoc(this.currentdoc);
            //htmlIsland.SetRootCssBox(rootBox, this.waitingCssData);
            //htmlIsland.MaxSize = new LayoutFarm.Drawing.SizeF(this._width, 0);
            //htmlIsland.PerformLayout();
        }
    }
}





