// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;

using LayoutFarm;
using LayoutFarm.ContentManagers;
using LayoutFarm.HtmlBoxes;

using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{

    public class LightHtmlBox : UIBox, IUserEventPortal
    {
        WaitingContentKind waitingContentKind;
        string waitingHtmlFragmentString;
        FragmentHtmlDocument waitingHtmlDoc;

        enum WaitingContentKind : byte
        {
            NoWaitingContent,
            HtmlFragmentString,
            HtmlDocument
        }

        MyHtmlContainer myHtmlCont;
        HtmlHost htmlhost;
        //presentation
        HtmlFragmentRenderBox frgmRenderBox;
        HtmlInputEventAdapter inputEventAdapter;

        public LightHtmlBox(HtmlHost htmlHost, int width, int height)
            : base(width, height)
        {
           
            this.htmlhost = htmlHost;
        }
        protected override void OnContentLayout()
        {
            this.PerformContentLayout();
        }
        public override void PerformContentLayout()
        {

            this.RaiseLayoutFinished();
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.frgmRenderBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.frgmRenderBox != null; }
        }

        HtmlInputEventAdapter GetInputEventAdapter()
        {
            if (inputEventAdapter == null)
            {
                inputEventAdapter = this.htmlhost.GetNewInputEventAdapter();
                inputEventAdapter.Bind(myHtmlCont);
            }
            return inputEventAdapter;
        }

        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            GetInputEventAdapter().MouseUp(e, frgmRenderBox.CssBox);

            ////1. get share input adapter
            //var inputAdapter = GetInputEventAdapter();
            ////2. send event
            //inputAdapter.MouseUp(e, frgmRenderBox.CssBox);
            ////3. release back to host
            //this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {

            e.CurrentContextElement = this;
            GetInputEventAdapter().MouseDown(e, frgmRenderBox.CssBox);

            //var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);

            //inputAdapter.MouseDown(e, frgmRenderBox.CssBox);

            //this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            GetInputEventAdapter().MouseMove(e, frgmRenderBox.CssBox);

            //var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);

            //inputAdapter.MouseMove(e, frgmRenderBox.CssBox);

            //this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;


            //var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);
            ////?
            //this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            GetInputEventAdapter().KeyDown(e, frgmRenderBox.CssBox);
            //var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);

            //inputAdapter.KeyDown(e, frgmRenderBox.CssBox);

            //this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            GetInputEventAdapter().KeyPress(e, frgmRenderBox.CssBox);

            //var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);
            //inputAdapter.KeyPress(e, frgmRenderBox.CssBox);
            //this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            GetInputEventAdapter().KeyUp(e, frgmRenderBox.CssBox);
            //var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);

            //inputAdapter.KeyUp(e, frgmRenderBox.CssBox);

            //this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            var result = GetInputEventAdapter().ProcessDialogKey(e, frgmRenderBox.CssBox);
            return result;

            //var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);
            //inputAdapter.KeyUp(e, frgmRenderBox.CssBox);

            //var result = inputAdapter.ProcessDialogKey(e, frgmRenderBox.CssBox);
            //this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
            //return result;
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
                case WaitingContentKind.NoWaitingContent:
                    break;
                case WaitingContentKind.HtmlDocument:
                    {
                        LoadHtmlDom(this.waitingHtmlDoc);
                    } break;
                case WaitingContentKind.HtmlFragmentString:
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
            waitingContentKind = WaitingContentKind.NoWaitingContent;

        }
        public void LoadHtmlDom(FragmentHtmlDocument htmldoc)
        {
            if (frgmRenderBox == null)
            {
                this.waitingContentKind = WaitingContentKind.HtmlDocument;
                this.waitingHtmlDoc = htmldoc;
            }
            else
            {
                //just parse content and load 
                this.myHtmlCont = HtmlContainerHelper.CreateHtmlContainer(this.htmlhost, htmldoc, frgmRenderBox);
                SetHtmlContainerEventHandlers();
                ClearWaitingContent();
                RaiseLayoutFinished();
            }
        }

        public void LoadHtmlFragmentString(string fragmentHtmlString)
        {
            if (frgmRenderBox == null)
            {
                this.waitingContentKind = WaitingContentKind.HtmlFragmentString;
                this.waitingHtmlFragmentString = fragmentHtmlString;
            }
            else
            {
                //just parse content and load 
                this.myHtmlCont = HtmlContainerHelper.CreateHtmlContainer(this.htmlhost, fragmentHtmlString, frgmRenderBox);

                SetHtmlContainerEventHandlers();

                ClearWaitingContent();
            }
        }


        void SetHtmlContainerEventHandlers()
        {
            myHtmlCont.AttachEssentialHandlers(
                //1.
                (s, e) => this.InvalidateGraphics(),
                //2.
                (s, e) =>
                {
                    //---------------------------
                    if (frgmRenderBox == null) return;
                    //--------------------------- 
                    htmlhost.GetRenderTreeBuilder().RefreshCssTree(myHtmlCont.RootElement);

                    var lay = this.htmlhost.GetSharedHtmlLayoutVisitor(myHtmlCont);
                    myHtmlCont.PerformLayout(lay);
                    this.htmlhost.ReleaseHtmlLayoutVisitor(lay);
                },
                //3.
                (s, e) => this.InvalidateGraphics(),
                //4
                (s, e) => { this.RaiseLayoutFinished(); });

        }
        public MyHtmlContainer HtmlContainer
        {
            get { return this.myHtmlCont; }
        }
        public override void SetViewport(int x, int y)
        {
            base.SetViewport(x, y);
            if (frgmRenderBox != null)
            {
                frgmRenderBox.SetViewport(x, y);
            }
        }
        public override int DesiredWidth
        {
            get
            {
                return this.frgmRenderBox.HtmlWidth;
            }
        }
        public override int DesiredHeight
        {
            get
            {
                return this.frgmRenderBox.HtmlHeight;
            }
        }


    }
}





