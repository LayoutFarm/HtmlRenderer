// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm;
using LayoutFarm.ContentManagers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.InternalHtmlDom;
using LayoutFarm.Composers;

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{

    public class LightHtmlBox : UIBox, IUserEventPortal
    {
        WaitingContent waitingContentKind;
        string waitingHtmlFragmentString;
        FragmentHtmlDocument waitingHtmlDoc;

        enum WaitingContent : byte
        {
            NoWaitingContent,
            HtmlFragmentString,
            HtmlDocument
        }


        MyHtmlIsland myHtmlIsland;
        HtmlIslandHost htmlIslandHost;

        //presentation
        HtmlFragmentRenderBox frgmRenderBox; 
        static LightHtmlBox()
        {
            LayoutFarm.Composers.BoxCreator.RegisterCustomCssBoxGenerator(
                new MyCssBoxGenerator());
        }

        public LightHtmlBox(HtmlIslandHost htmlIslandHost, int width, int height)
            : base(width, height)
        {
            this.htmlIslandHost = htmlIslandHost;
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
            var inputAdapter = this.htmlIslandHost.GetSharedInputEventAdapter(this.myHtmlIsland);
            //2. send event
            inputAdapter.MouseUp(e, frgmRenderBox.CssBox);
            //3. release back to host
            this.htmlIslandHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.htmlIslandHost.GetSharedInputEventAdapter(this.myHtmlIsland);

            inputAdapter.MouseDown(e, frgmRenderBox.CssBox);

            this.htmlIslandHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {   //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.htmlIslandHost.GetSharedInputEventAdapter(this.myHtmlIsland);

            inputAdapter.MouseMove(e, frgmRenderBox.CssBox);

            this.htmlIslandHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            var inputAdapter = this.htmlIslandHost.GetSharedInputEventAdapter(this.myHtmlIsland);
            //?
            this.htmlIslandHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.htmlIslandHost.GetSharedInputEventAdapter(this.myHtmlIsland);

            inputAdapter.KeyDown(e, frgmRenderBox.CssBox);

            this.htmlIslandHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            var inputAdapter = this.htmlIslandHost.GetSharedInputEventAdapter(this.myHtmlIsland);
            inputAdapter.KeyPress(e, frgmRenderBox.CssBox);
            this.htmlIslandHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            var inputAdapter = this.htmlIslandHost.GetSharedInputEventAdapter(this.myHtmlIsland);

            inputAdapter.KeyUp(e, frgmRenderBox.CssBox);

            this.htmlIslandHost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.htmlIslandHost.GetSharedInputEventAdapter(this.myHtmlIsland);
            inputAdapter.KeyUp(e, frgmRenderBox.CssBox);
            var result = inputAdapter.ProcessDialogKey(e, frgmRenderBox.CssBox);
            this.htmlIslandHost.ReleaseSharedInputEventAdapter(inputAdapter);
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
            switch (this.waitingContentKind)
            {
                default:
                case WaitingContent.NoWaitingContent:
                    break;
                case WaitingContent.HtmlDocument:
                    {
                        LoadHtmlDom(this.waitingHtmlDoc);
                    } break;
                case WaitingContent.HtmlFragmentString:
                    {
                        LoadHtmlFragmentString(this.waitingHtmlFragmentString);
                    } break;
            }

            return frgmRenderBox;
        }
        //-----------------------------------------------------------------------------------------------------
        void ClearWaitingContent()
        {
            this.waitingHtmlDoc = null;
            this.waitingHtmlFragmentString = null;
            waitingContentKind = WaitingContent.NoWaitingContent;

        }
        public void LoadHtmlDom(FragmentHtmlDocument htmldoc)
        {
            if (frgmRenderBox == null)
            {
                this.waitingContentKind = WaitingContent.HtmlDocument;
                this.waitingHtmlDoc = htmldoc;
            }
            else
            {
                //just parse content and load 
                this.myHtmlIsland = HtmlIslandHelper.CreateHtmlIsland(this.htmlIslandHost, htmldoc, frgmRenderBox);
                SetHtmlIslandEventHandlers();
                ClearWaitingContent();
            }
        }

        public void LoadHtmlFragmentString(string fragmentHtmlString)
        {
            if (frgmRenderBox == null)
            {
                this.waitingContentKind = WaitingContent.HtmlFragmentString;
                this.waitingHtmlFragmentString = fragmentHtmlString;
            }
            else
            {
                //just parse content and load 
                this.myHtmlIsland = HtmlIslandHelper.CreateHtmlIsland(this.htmlIslandHost, fragmentHtmlString, frgmRenderBox);

                SetHtmlIslandEventHandlers();

                ClearWaitingContent();
            }
        }


        void SetHtmlIslandEventHandlers()
        {
            myHtmlIsland.DomVisualRefresh += (s, e) => this.InvalidateGraphics();
            myHtmlIsland.DomRequestRebuild += (s, e) =>
            {

                //---------------------------
                if (frgmRenderBox == null) return;
                //--------------------------- 
                htmlIslandHost.GetRenderTreeBuilder().RefreshCssTree(myHtmlIsland.RootElement);

                var lay = this.htmlIslandHost.GetSharedHtmlLayoutVisitor(myHtmlIsland);
                myHtmlIsland.PerformLayout(lay);
                this.htmlIslandHost.ReleaseHtmlLayoutVisitor(lay);
            };
        }
        public MyHtmlIsland HtmlIsland
        {
            get { return this.myHtmlIsland; }
        }

    }
}





