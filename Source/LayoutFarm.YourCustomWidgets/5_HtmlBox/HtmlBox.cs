// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm;
using LayoutFarm.ContentManagers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;

using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{

    public class HtmlBox : UIBox, IUserEventPortal
    {
        RootGraphic rootgfx;
        HtmlRenderBox htmlRenderBox;

        MyHtmlContainer myHtmlContainer;
        LayoutVisitor htmlLayoutVisitor;

        LayoutFarm.WebDom.WebDocument currentdoc;



        bool hasWaitingDocToLoad;
        LayoutFarm.WebDom.CssActiveSheet waitingCssData;
        HtmlInputEventAdapter inputEventAdapter;
        object uiHtmlTask = new object();


        LayoutFarm.Composers.RenderTreeBuilder renderTreeBuilder = null;
        HtmlHost htmlHost;

        //static HtmlBox()
        //{
        //    LayoutFarm.Composers.BoxCreator.RegisterCustomCssBoxGenerator(
        //       typeof(MyCssBoxGenerator),
        //       new MyCssBoxGenerator());
        //}

        public HtmlBox(HtmlHost htmlHost, int width, int height)
            : base(width, height)
        {
            if (htmlHost.HasRegisterCssBoxGenerator(typeof(MyCssBoxGenerator)))
            {
                htmlHost.RegisterCssBoxGenerator(new MyCssBoxGenerator());
            }
            //--------
            this.htmlHost = htmlHost;
            myHtmlContainer = new MyHtmlContainer(htmlHost);

            myHtmlContainer.AttachEssentialHandlers(
                (s, e) => this.InvalidateGraphics(),//visual refresh
                 htmlContainer_NeedUpdateDom,
                (s, e) => this.InvalidateGraphics(),
                (s, e) => { this.RaiseLayoutFinished(); });
            //request ui timer *** 
            //tim.Interval = 30;
            //tim.Elapsed += new System.Timers.ElapsedEventHandler(tim_Elapsed);
        }
        public HtmlHost HtmlHost
        {
            get { return this.htmlHost; }
        }
        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            inputEventAdapter.MouseUp(e);
        }
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            e.CurrentContextElement = this;
            inputEventAdapter.MouseDown(e);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            inputEventAdapter.MouseMove(e);
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
        }

        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            inputEventAdapter.KeyDown(e);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            inputEventAdapter.KeyPress(e);
        }
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            inputEventAdapter.KeyUp(e);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return this.inputEventAdapter.ProcessDialogKey(e);
        }
        void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
        }
        void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
        }
        internal MyHtmlContainer HtmlContainer
        {
            get { return this.myHtmlContainer; }
        }

        void CreateRenderTreeBuilder()
        {
            this.renderTreeBuilder = this.htmlHost.GetRenderTreeBuilder();

            //this.renderTreeBuilder = new LayoutFarm.Composers.RenderTreeBuilder();
            //this.renderTreeBuilder.RequestStyleSheet += (e2) =>
            //{
            //    if (this.RequestStylesheet != null)
            //    {
            //        var req = new TextLoadRequestEventArgs(e2.Src);
            //        RequestStylesheet(this, req);
            //        e2.SetStyleSheet = req.SetStyleSheet;
            //    }
            //};

        }
        void htmlContainer_NeedUpdateDom(object sender, EventArgs e)
        {
            hasWaitingDocToLoad = true;
            //---------------------------
            if (htmlRenderBox == null) return;
            //--------------------------- 
            if (this.renderTreeBuilder == null) CreateRenderTreeBuilder();

            //var builder = new HtmlRenderer.Composers.RenderTreeBuilder(htmlRenderBox.Root);
            //builder.RequestStyleSheet += (e2) =>
            //{
            //    if (this.RequestStylesheet != null)
            //    {
            //        var req = new TextLoadRequestEventArgs(e2.Src);
            //        RequestStylesheet(this, req);
            //        e2.SetStyleSheet = req.SetStyleSheet;
            //    }
            //};

            renderTreeBuilder.RefreshCssTree(this.currentdoc.RootNode);
            this.myHtmlContainer.PerformLayout(htmlLayoutVisitor);

        }

        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (htmlRenderBox == null)
            {
                this.rootgfx = rootgfx;
                htmlRenderBox = new HtmlRenderBox(rootgfx, this.Width, this.Height, myHtmlContainer);
                htmlRenderBox.SetController(this);
                htmlRenderBox.HasSpecificSize = true;

                inputEventAdapter = new HtmlInputEventAdapter(rootgfx.SampleIFonts);
                inputEventAdapter.Bind(this.myHtmlContainer);

                htmlLayoutVisitor = new LayoutVisitor(rootgfx.P);
                htmlLayoutVisitor.Bind(this.myHtmlContainer);
            }


            if (this.hasWaitingDocToLoad)
            {
                UpdateWaitingHtmlDoc(this.htmlRenderBox.Root);
            }
            return htmlRenderBox;
        }
        void UpdateWaitingHtmlDoc(RootGraphic rootgfx)
        {
            if (this.renderTreeBuilder == null) CreateRenderTreeBuilder();
            //------------------------------------------------------------


            //build rootbox from htmldoc
            var rootBox = renderTreeBuilder.BuildCssRenderTree((HtmlDocument)this.currentdoc,
                this.waitingCssData,
                this.htmlRenderBox);


            var htmlCont = this.myHtmlContainer;
            htmlCont.WebDocument = this.currentdoc;
            htmlCont.RootCssBox = rootBox;
            htmlCont.SetMaxSize(this.Width, 0);
            htmlCont.PerformLayout(this.htmlLayoutVisitor);
        }
        void SetHtml(MyHtmlContainer htmlCont, string html, LayoutFarm.WebDom.CssActiveSheet cssData)
        {
            var htmldoc = LayoutFarm.Composers.WebDocumentParser.ParseDocument(
                             new LayoutFarm.WebDom.Parser.TextSnapshot(html.ToCharArray()));
            this.currentdoc = htmldoc;
            this.hasWaitingDocToLoad = true;
            this.waitingCssData = cssData;
            //---------------------------
            if (htmlRenderBox == null) return;
            //---------------------------
            UpdateWaitingHtmlDoc(this.htmlRenderBox.Root);
            htmlRenderBox.InvalidateGraphics();
        }
        public void LoadHtmlText(string html)
        {
            SetHtml(myHtmlContainer, html, this.htmlHost.BaseStylesheet);
        }
        public override void InvalidateGraphics()
        {
            this.htmlRenderBox.InvalidateGraphics();
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.htmlRenderBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.htmlRenderBox != null; }
        }
        public override int DesiredWidth
        {
            get
            {
                return this.htmlRenderBox.HtmlWidth;
            }
        }
        public override int DesiredHeight
        {
            get
            {
                return this.htmlRenderBox.HtmlHeight;
            }
        }
    }
}





