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


        MyHtmlContainer myHtmlCont;
        HtmlHost htmlhost;





        static LightHtmlBox()
        {
            LayoutFarm.Composers.BoxCreator.RegisterCustomCssBoxGenerator(
                typeof(MyCssBoxGenerator),
                new MyCssBoxGenerator());
        }


        //presentation
        HtmlFragmentRenderBox frgmRenderBox;
        public LightHtmlBox(HtmlHost htmlhost, int width, int height)
            : base(width, height)
        {
            this.htmlhost = htmlhost;
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
            var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);
            //2. send event
            inputAdapter.MouseUp(e, frgmRenderBox.CssBox);
            //3. release back to host
            this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);

            inputAdapter.MouseDown(e, frgmRenderBox.CssBox);

            this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {   //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);

            inputAdapter.MouseMove(e, frgmRenderBox.CssBox);

            this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);
            //?
            this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);

            inputAdapter.KeyDown(e, frgmRenderBox.CssBox);

            this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);
            inputAdapter.KeyPress(e, frgmRenderBox.CssBox);
            this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;
            var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);

            inputAdapter.KeyUp(e, frgmRenderBox.CssBox);

            this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            //0. set context
            e.CurrentContextElement = this;

            var inputAdapter = this.htmlhost.GetSharedInputEventAdapter(this.myHtmlCont);
            inputAdapter.KeyUp(e, frgmRenderBox.CssBox);
            var result = inputAdapter.ProcessDialogKey(e, frgmRenderBox.CssBox);
            this.htmlhost.ReleaseSharedInputEventAdapter(inputAdapter);
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
                this.myHtmlCont = HtmlContainerHelper.CreateHtmlContainer(this.htmlhost, htmldoc, frgmRenderBox);
                SetHtmlContainerEventHandlers();
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
                (s, e) => this.InvalidateGraphics());

        }
        public MyHtmlContainer HtmlContainer
        {
            get { return this.myHtmlCont; }
        }

    }
}





